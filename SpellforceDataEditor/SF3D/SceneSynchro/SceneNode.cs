/* 
 * SceneNode is the basic component of a scene
 * Every component of the scene inherits from SceneNode
 * SceneNode has one parent and many children
 * Each node contains transform data which is updated only when needed
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SpellforceDataEditor.SF3D.SceneSynchro
{
    [Flags]
    public enum RenderFlags { NONE = 0, APPLY_SHADING = 1, CAST_SHADOW = 2, RECEIVE_SHADOW = 4, TRANSPARENT = 8, }
    // ultimately might become obsolete...
    public class SceneNode
    {
        public string Name { get; set; } = "";
        public SceneNode Parent { get; set; } = null;
        public List<SceneNode> Children { get; protected set; } = new List<SceneNode>();

        // if true, on the  next  update  local transform will be  updated
        protected bool needsanyupdate = true;
        public virtual bool NeedsAnyUpdate { get { return needsanyupdate; } protected set { needsanyupdate = value; } }
        public virtual bool NeedsUpdateLocalTransform { get; protected set; } = true;
        public virtual bool NeedsUpdateResultTransform { get; protected set; } = true;

        protected bool visible = true;
        public Vector3 position = Vector3.Zero;
        protected Quaternion rotation = Quaternion.Identity;
        protected Vector3 scale = Vector3.One;
        protected Matrix4 local_transform = Matrix4.Identity;
        //protected Matrix4 result_transform = Matrix4.Identity;
        protected Physics.BoundingBox aabb = Physics.BoundingBox.Zero;
        
        // todo: add a LocalVisible, so even when parent changes to visible while this is invisible, this is still invisible
        public virtual bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                if (visible != value)
                {
                    visible = value;
                    foreach (SceneNode n in Children)
                        n.Visible = value;
                }
            }
        }
        public Quaternion Rotation { get { return rotation; } set { rotation = value; TouchLocalTransform(); TouchParents(); } }
        public Vector3 Scale { get { return scale; } set { scale = value; TouchLocalTransform(); TouchParents(); } }
        public Matrix4 LocalTransform { get { return local_transform; } protected set { local_transform = value; } }
        public Matrix4 ResultTransform = Matrix4.Identity;

        public Physics.BoundingBox AABB { get; }

        public RenderFlags render_flags;

        public SceneNode(string n)
        {
            Name = n;
        }

        // adds a given node as a child of this node
        public void AddNode(SceneNode node)
        {
            if(node == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SceneNode.AddNode(): Node to add is null!");
                return;
            }
            Children.Add(node);
            node.Parent = this;
            node.Visible = Visible;
        }

        // removes a given node from the children of this node
        public void RemoveNode(SceneNode node)
        {
            if(node == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SceneNode.RemoveNode(): Node to remove is null!");
                return;
            }
            node.Parent = null;
            node.Visible = false;
            Children.Remove(node);
        }

        // removes node from hierarchy based on provided path
        public void RemoveNode(string path)
        {
            SceneNode n = FindNode<SceneNode>(path);
            if (n != null)
                n.Parent.RemoveNode(n);
        }

        // changes parent of this node from current parent to a given node
        public void SetParent(SceneNode node)
        {
            if (Parent != null)
                Parent.RemoveNode(this);
            if (node != null)
                node.AddNode(this);
            else
                Visible = false;
        }

        // utility function which sets rotation of this node to a given angle (degrees) around the UP axis (0, 1, 0)
        public void SetAnglePlane(int angle_deg)
        {
            Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2)
                     * Quaternion.FromAxisAngle(new Vector3(0f, 0f, 1f), MathUtils.DegToRad(angle_deg));
        }

        public void SetPosition(Vector3 pos)
        {
            position = pos;
            TouchLocalTransform();
            TouchParents();
        }

        // if something requires updating, notify all parents about that, root including, so the engine knows to run update routine
        protected void TouchParents()
        {
            NeedsAnyUpdate = true;
            if ((Parent != null) && (Parent.NeedsAnyUpdate != true))
                Parent.TouchParents();
        }

        // if one local transform changes, so must the result transform
        protected void TouchLocalTransform()
        {
            NeedsUpdateLocalTransform = true;
            TouchResultTransform();
        }

        // if one result transform changes, so must all subsequent result transforms
        protected void TouchResultTransform()
        {
            NeedsAnyUpdate = true;
            NeedsUpdateResultTransform = true;
            foreach (SceneNode node in Children)
                node.TouchResultTransform();
        }

        // updates the node and all children nodes
        public void Update(float dt)
        {
            if (Visible)
            {
                UpdateTime(dt);

                if (needsanyupdate)
                    UpdateTransform();

                GatherSceneInstances();

                foreach (SceneNode node in Children)
                    node.Update(dt);

                needsanyupdate = false;
            }
        }

        public void SetTime(float t)
        {
            SetTimeInternal(t);

            foreach (SceneNode node in Children)
                node.SetTime(t);
        }

        protected virtual void SetTimeInternal(float t)
        {

        }

        // updates local transform if needed, and result transform if needed
        protected virtual void UpdateTransform()
        {
            if (NeedsUpdateLocalTransform)
            {
                Matrix4 translation_matrix = Matrix4.CreateTranslation(position);
                Matrix4 rotation_matrix = Matrix4.CreateFromQuaternion(rotation);
                Matrix4 scale_matrix = Matrix4.CreateScale(scale);
                local_transform = scale_matrix * rotation_matrix * translation_matrix;

                NeedsUpdateLocalTransform = false;
            }

            if(NeedsUpdateResultTransform)
            {
                if (Parent != null)
                    ResultTransform = local_transform * Parent.ResultTransform;
                else
                    ResultTransform = local_transform;
                NeedsUpdateResultTransform = false;
            }
        }

        // updates node according to the given time parameter
        protected virtual void UpdateTime(float dt)
        {

        }

        // used by instanced meshes (SceneNodeSimple)
        protected virtual void GatherSceneInstances()
        {

        }

        // finds a node of a given type, given a path to node
        public T FindNode<T>(string path) where T: SceneNode
        {
            string[] names = (Name+'.'+path).Split('.');
            return FindNode<T>(names, 0);
        }

        private T FindNode<T>(string[] names, int current_index) where T: SceneNode
        {
            if (names[current_index] == Name)
            {
                if ((current_index == names.Length - 1)&&(this.GetType() == typeof(T)))
                    return (T)this;

                T result = null;
                foreach (SceneNode node in Children)
                {
                    result = node.FindNode<T>(names, current_index + 1);
                    if (result != null)
                        break;
                }
                return result;
            }
            return null;
        }

        // returns full name of the node (recursive, slow)
        public string GetFullPath()
        {
            if (Parent == null)
                return this.Name;
            return Parent.GetFullPath() + '.' + this.Name;
        }

        // disposes node and all resources its using
        public void Dispose()
        {
            InternalDispose();

            //System.Diagnostics.Debug.WriteLine("DISPOSING " + Name);

            foreach (SceneNode c in Children)
                c.Dispose();

            while(Children.Count != 0)
                Children[0].SetParent(null);
        }

        protected virtual void InternalDispose()
        {

        }

        public override string ToString()
        {
            return Name + " ("+Children.Count+" children)";
        }
    }

    // struct holds info on where in the scene cache an element of this mesh can be found
    public struct TexGeometryLinkSimple
    {
        public SFTexture texture;
        public int index;
    }

    // use this for displaying non-animated 3d objects
    public class SceneNodeSimple : SceneNode
    {
        private SFModel3D mesh;
        public SFModel3D Mesh
        {
            get
            {
                return mesh;
            }
            set
            {
                if(mesh != null)
                {
                    if(value == null)
                    {
                        aabb = Physics.BoundingBox.Zero;
                        if (visible)
                            ClearTexGeometry();
                        mesh = null;
                    }
                    else
                    {
                        if (visible)
                            ClearTexGeometry();
                        mesh = value;
                        if (visible)
                            AddTexGeometry();
                        aabb = mesh.aabb;
                    }
                }
                else
                {
                    if(value != null)
                    {
                        mesh = value;
                        if (visible)
                            AddTexGeometry();
                        aabb = mesh.aabb;
                    }
                }
            }
        }

        public override bool Visible
        {
            get => base.Visible;
            set
            {
                if (Visible != value)
                {
                    visible = value;
                    if (mesh != null)
                    {
                        if (value == false)
                        {
                            ClearTexGeometry();
                        }
                        else
                        {
                            AddTexGeometry();
                        }
                    }
                    foreach (SceneNode n in Children)
                        n.Visible = value;
                }
            }
        }

        public bool Billboarded { get; set; } = false;

        protected override void UpdateTransform()
        {
            if(Billboarded)
            {
                SceneNodeCamera camera = SFRender.SFRenderEngine.scene.camera;
                Rotation = Matrix4.LookAt(camera.position, position, new Vector3(0, 1, 0)).ExtractRotation();
                rotation.Conjugate();
            }

            base.UpdateTransform();
        }

        protected override void GatherSceneInstances()
        {
            if((visible)&&(mesh != null))
            {
                SFSubModel3D.Cache.MatrixBufferData[mesh.MatrixOffset + mesh.CurrentMatrixIndex] = ResultTransform;
                mesh.CurrentMatrixIndex += 1;
            }
        }

        public SceneNodeSimple(string n) : base(n) { }

        // removes this node from scene cache
        // assumes mesh exists
        private void ClearTexGeometry()
        {
            mesh.MatrixCount -= 1;
            if (mesh.MatrixCount == 0)
            {
                SFRender.SFRenderEngine.scene.model_set_simple.Remove(mesh);

                foreach (var submodel in mesh.submodels)
                {
                    if (submodel.material.transparent_pass)
                    {
                        SFRender.SFRenderEngine.scene.transparent_pass_models.Remove(submodel);
                    }
                    if (submodel.material.water_pass)
                    {
                        SFRender.SFRenderEngine.scene.water_pass_models.Remove(submodel);
                    }
                    else if (submodel.material.additive_pass)
                    {
                        SFRender.SFRenderEngine.scene.additive_pass_models.Remove(submodel);
                    }
                    else
                    {
                        SFRender.SFRenderEngine.scene.opaque_pass_models.Remove(submodel);
                    }
                }
            }
        }

        // adds this node to scene cache
        // todo: work out transparency :)
        private void AddTexGeometry()
        {
            mesh.MatrixCount += 1;
            if (mesh.MatrixCount == 1)
            {
                SFRender.SFRenderEngine.scene.model_set_simple.Add(mesh);

                foreach(var submodel in mesh.submodels)
                {
                    if (submodel.material.transparent_pass)
                    {
                        SFRender.SFRenderEngine.scene.transparent_pass_models.Add(submodel);
                    }
                    if (submodel.material.water_pass)
                    {
                        SFRender.SFRenderEngine.scene.water_pass_models.Add(submodel);
                    }
                    else if (submodel.material.additive_pass)
                    {
                        SFRender.SFRenderEngine.scene.additive_pass_models.Add(submodel);
                    }
                    else
                    {
                        SFRender.SFRenderEngine.scene.opaque_pass_models.Add(submodel);
                    }
                }
            }
        }

        // disposes mesh used by this node (reference counted, dw)
        protected override void InternalDispose()
        {
            if (Mesh != null)
            {
                SFResources.SFResourceManager.Models.Dispose(Mesh.GetName());
                Mesh = null;
            }
        }
    }

    // struct holds info on where in the scene cache an element of this mesh can be found
    public struct TexGeometryLinkAnimated
    {
        public SFTexture texture;
        public int index;
    }

    // use this for displaying animated 3d meshes
    public class SceneNodeAnimated : SceneNode
    {
        private SFModel3D mesh = null;        // this is only for extraction convenience!
        private SFSkeleton skeleton = null;
        private SFModelSkin skin = null;
        public SFModel3D Mesh
        {
            get
            {
                return mesh;
            }
            set
            {
                mesh = value;
                if (mesh != null)
                    aabb = mesh.aabb;
                else
                    aabb = new Physics.BoundingBox(Vector3.Zero, Vector3.Zero);
            }
        }
        public SFSkeleton Skeleton { get { return skeleton; } private set { skeleton = value; } }
        public SFModelSkin Skin
        {
            get
            {
                return skin;
            }
            private set
            {
                skin = value;
                if (skin == null)
                    ClearTexGeometry();
                else if(Visible)
                    AddTexGeometry();
            }
        }

        public SFAnimation Animation { get; private set; } = null;
        public Matrix4[] BoneTransforms = null;
        public float AnimCurrentTime { get; private set; } = 0;
        public bool AnimPlaying { get; set; } = false;

        // if this node has a primary, it inherits all skin calculations
        // primary node must be the first node in children hierarchy
        // primare must have the same skeleton as this node
        // if this node has a primary, it better not have children...
        public SceneNodeAnimated Primary { get; set; } = null;

        public override bool Visible
        {
            get => base.Visible;
            set
            {
                if (Visible != value)
                {
                    if (value == false)
                        ClearTexGeometry();
                    else
                        AddTexGeometry();

                    visible = value;
                    foreach (SceneNode n in Children)
                        n.Visible = value;
                }
            }
        }

        public SceneNodeAnimated(string n) : base(n) { }

        // this must be used
        // also this bypasses resource system (for now), so they need to be managed elsewhere
        public void SetSkeletonSkin(SFSkeleton _skeleton, SFModelSkin _skin)
        {
            if (_skeleton != null)
            {
                BoneTransforms = new Matrix4[_skeleton.bone_count];
                for (int i = 0; i < _skeleton.bone_count; i++)
                    BoneTransforms[i] = Matrix4.Identity;

                TouchResultTransform();
            }
            else
                BoneTransforms = null;

            Skeleton = _skeleton;
            Skin = _skin;
        }

        public void SetAnimation(SFAnimation _animation, bool play = true)
        {
            if(skeleton == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SceneNodeAnimated.SetAnimation(): Skeleton is missing!");
                return;
            }
            Animation = _animation;
            AnimCurrentTime = 0f;
            AnimPlaying = play;
            if (play == true)
                UpdateBoneTransforms();
        }

        // calculates bone transforms for this node
        private void UpdateBoneTransforms()
        {
            if (Primary == null)
            {
                for (int i = 0; i < BoneTransforms.Length; i++)
                {
                    Animation.bone_animations[i].GetMatrix4(AnimCurrentTime, ref BoneTransforms[i]);

                    if (Skeleton.bone_parents[i] != Utility.NO_INDEX)
                        BoneTransforms[i] = BoneTransforms[i] * BoneTransforms[Skeleton.bone_parents[i]];
                }

                for (int i = 0; i < BoneTransforms.Length; i++)
                    BoneTransforms[i] = skeleton.bone_inverted_matrices[i] * BoneTransforms[i];

                TouchResultTransform();
            }
            else
            {
                for (int i = 0; i < BoneTransforms.Length; i++)
                    BoneTransforms[i] = Primary.BoneTransforms[i];
            }
        }

        public void SetAnimationCurrentTime(float t)
        {
            if (Animation == null)
                return;


            AnimCurrentTime = t;
            if (AnimCurrentTime > Animation.max_time)
                AnimCurrentTime -= (int)(AnimCurrentTime / Animation.max_time) * Animation.max_time;

            if ((BoneTransforms != null) && (AnimPlaying))
            {
                // determine if should update, based on scene camera distance and on bounding box
                float dist = (SFRender.SFRenderEngine.scene.camera.position - ResultTransform.Row3.Xyz).Length;
                float size;

                if(Primary == null)
                    size = (aabb.a - aabb.center).Length;
                else
                    size = (Primary.aabb.a - Primary.aabb.center).Length;
                if (size == 0)
                    return;

                int skip_count = 1 + (int)((dist/size) * 0.1f);
                if (skip_count == 1)
                    UpdateBoneTransforms();
                else if ((SFRender.SFRenderEngine.scene.frame_counter % skip_count) == 0)
                    UpdateBoneTransforms();
            }
        }

        protected override void UpdateTime(float dt)
        {
            if(dt != 0)
                SetAnimationCurrentTime(AnimCurrentTime + dt);
        }

        protected override void SetTimeInternal(float t)
        {
            if (Animation == null)
                return;

            SetAnimationCurrentTime(t);
        }
        // clears scene cache of all elements drawn by this node
        // also do this if transparent = true, let transparent object list deal with those
        private void ClearTexGeometry()
        {
            SF3D.SFRender.SFRenderEngine.scene.an_nodes.Remove(this);
        }

        // adds elements drawn by this node to scene cache
        // also do this if transparent = false, let transparent object list deal with those
        private void AddTexGeometry()
        {
            SF3D.SFRender.SFRenderEngine.scene.an_nodes.Add(this);
        }

        // disposes skeleton and skin used by this node (reference counted)
        protected override void InternalDispose()
        {
            Primary = null;
            if (Mesh != null)
            {
                SFResources.SFResourceManager.Models.Dispose(Mesh.GetName());
                Mesh = null;
            }
            if (Skeleton != null)
            {
                SFResources.SFResourceManager.Skeletons.Dispose(Skeleton.GetName());
                Skeleton = null;

                if (Animation != null)
                {
                    Animation = null;
                    AnimCurrentTime = 0f;
                    AnimPlaying = false;
                }

                BoneTransforms = null;
            }
            if (Skin != null)
            {
                SFResources.SFResourceManager.Skins.Dispose(Skin.GetName());
                Skin = null;
            }
        }
    }

    // this node should be attached to a SceneNodeAnimated node,
    // for example if you want a weapon node to follow an arm bone of a character node
    public class SceneNodeBone : SceneNode
    {
        // parent must be SceneNodeAnimated
        private int BoneIndex = Utility.NO_INDEX;
        public override bool NeedsUpdateLocalTransform { get { return true; } }

        public SceneNodeBone(string n) : base(n) { }

        protected override void UpdateTransform()
        {
            if (Parent != null)
            {
                if((Parent.GetType() == typeof(SceneNodeAnimated))&&(BoneIndex != Utility.NO_INDEX))
                    ResultTransform = local_transform * ((SceneNodeAnimated)(Parent)).Skeleton.bone_reference_matrices[BoneIndex]
                        * ((SceneNodeAnimated)(Parent)).BoneTransforms[BoneIndex] * Parent.ResultTransform;
                else
                    ResultTransform = local_transform * Parent.ResultTransform;
            }
            else
                ResultTransform = local_transform;

            TouchLocalTransform();
        }

        // sets to which bone this node should be attached
        public void SetBone(string name)
        {
            BoneIndex = Utility.NO_INDEX;
            if (Parent == null)
                return;
            if (((SceneNodeAnimated)Parent).Skeleton == null)
                return;
            for (int i = 0; i < ((SceneNodeAnimated)Parent).Skeleton.bone_count; i++)
            {
                if (((SceneNodeAnimated)Parent).Skeleton.bone_names[i].Contains(name))
                {
                    BoneIndex = i;
                    return;
                }
            }

            LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SceneNodeBone.SetBone(): Bone does not exist (bone name: " + name + ")");
        }
    }
    
    // this is mainly used as a convenience for certain operations
    public class SceneNodeMapChunk: SceneNode
    {
        public SFMap.SFMapHeightMapChunk MapChunk;
        public float DistanceToCamera { get; set; } = 0f;
        public float CameraHeightDifference { get; set; } = 0f;

        public SceneNodeMapChunk(string n) : base(n) { }

        protected override void InternalDispose()
        {
            if(MapChunk != null)
                MapChunk.Unload();
        }
    }

    // camera node should not have any children or unexpected behavior might occur
    public class SceneNodeCamera: SceneNode
    {
        const float EPSILON = 0.0001f;
        const float MAX_DIR_Y = 1.5f;

        private Vector3 lookat = new Vector3(0f, 1f, 0f);
        private Vector2 direction = Vector2.Zero;
        private Matrix4 proj_matrix = new Matrix4();
        private float aspect_ratio = 1;
        private Matrix4 view_matrix = new Matrix4();
        private Matrix4 viewproj_matrix = new Matrix4();

        private Physics.Frustum frustum;

        public Vector3 Lookat { get { return lookat; } }
        public Vector2 Direction { get { return direction; } }

        // view matrix: modelmatrix
        // proj matrix: projmatrix
        public Matrix4 ViewProjMatrix { get { return viewproj_matrix; } }
        public Matrix4 ProjMatrix { get { return proj_matrix; } set { proj_matrix = value; viewproj_matrix = proj_matrix; needsanyupdate = true; NeedsUpdateLocalTransform = true; } }
        public Matrix4 ViewMatrix { get { return view_matrix; } }
        public float AspectRatio { get { return aspect_ratio; } set { aspect_ratio = value; needsanyupdate = true; NeedsUpdateLocalTransform = true; } }
        public Physics.Frustum Frustum { get { return frustum; } }

        // 250.0f is a magic constant for now...
        public SceneNodeCamera(string s): base(s)
        {
            frustum = new Physics.Frustum(position, (lookat-position), SFRender.SFRenderEngine.min_render_distance, 1.136f * Settings.ObjectFadeMax, aspect_ratio);
            UpdateTransform();
        }

        // in radians
        public void SetAzimuthAltitude(Vector2 dir)
        {
            direction = dir;
            if (direction.Y < -MAX_DIR_Y)
                direction.Y = -MAX_DIR_Y;
            if (direction.Y > MAX_DIR_Y)
                direction.Y = MAX_DIR_Y;

            // modify lookat to match
            Vector3 lookat_dir = Vector3.UnitX * Matrix3.CreateFromQuaternion(Quaternion.FromAxisAngle(Vector3.UnitY, direction.X) * Quaternion.FromAxisAngle(Vector3.UnitZ, direction.Y));
            lookat = position + lookat_dir;

            TouchLocalTransform(); TouchParents();
        }

        public void SetLookat(Vector3 pos)
        {
            lookat = pos;

            // modify direction to match
            Vector3 DirVector = (lookat - position).Normalized();
            Vector3 DirVector2 = DirVector;

            DirVector2.Y = 0;
            if (DirVector2.LengthSquared == 0)
                direction.X = 0;
            else
            {
                DirVector2.Normalize();
                if(DirVector.Z < 0)
                    direction.X = (float)(Vector3.CalculateAngle(Vector3.UnitX, DirVector2));
                else
                    direction.X = -(float)(Vector3.CalculateAngle(Vector3.UnitX, DirVector2));
            }

            direction.Y = (float)(Math.PI/2) - (float)(Vector3.CalculateAngle(DirVector, Vector3.UnitY));

            TouchLocalTransform(); TouchParents();
        }

        // todo: can be optimized
        protected override void UpdateTransform()
        {
            if (NeedsUpdateLocalTransform)
            {
                view_matrix = Matrix4.LookAt(position, lookat, new Vector3(0, 1, 0));
                viewproj_matrix = view_matrix * ProjMatrix;

                // calculate frustum
                frustum.start = position;
                frustum.direction = (lookat - position);
                frustum.aspect_ratio = aspect_ratio;
                frustum.Calculate();

                NeedsUpdateLocalTransform = false;
            }
        }

        // use this to shift camera around
        public void translate(Vector3 tr)
        {
            position += tr;
            lookat += tr;
            TouchLocalTransform(); TouchParents();
        }

        public Vector3 ScreenToWorld(Vector2 uv)
        {
            Matrix4 inv = viewproj_matrix.Inverted();
            float depth = 1.0f;

            Vector4 vIn = new Vector4((2.0f * uv.X) - 1.0f, 1.0f - (2.0f * uv.Y), 2.0f * depth - 1.0f, 1.0f);
            Vector4 pos = vIn * inv;

            pos.W = 1.0f / pos.W;

            pos.X *= pos.W;
            pos.Y *= pos.W;
            pos.Z *= pos.W;

            return pos.Xyz;
        }

        // pos: coordinate in 3d world space
        // result: coordinate in the window (top-left: (0, 0), bottom-right: (1, 1))
        public Vector2 WorldToScreen(Vector3 pos)
        {
            Vector4 clip_space_vec = new Vector4(pos, 1.0f) * viewproj_matrix;

            if (clip_space_vec.W == 0)
                return Vector2.Zero;

            Vector3 NDC_space_vec = clip_space_vec.Xyz / clip_space_vec.W;

            return new Vector2(((NDC_space_vec.X + 1.0f) / 2.0f), ((1.0f - NDC_space_vec.Y) / 2.0f));
        }
    }
}
