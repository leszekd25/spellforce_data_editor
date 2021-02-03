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
        public void Update(float t)
        {
            if (Visible)
            {
                UpdateTime(t);

                if (needsanyupdate)
                    UpdateTransform();

                GatherSceneInstances();

                foreach (SceneNode node in Children)
                    node.Update(t);

                needsanyupdate = false;
            }
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
        protected virtual void UpdateTime(float t)
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
        //public int ModelIndex { get; private set; } = Utility.NO_INDEX;

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
            //if (ModelIndex == Utility.NO_INDEX)
            //    return;

            mesh.MatrixCount -= 1;
            if (mesh.MatrixCount == 0)
                SFRender.SFRenderEngine.scene.model_set_simple.Remove(mesh);

            /*SFRender.SFRenderEngine.scene.model_list_simple[mesh].RemoveAt(ModelIndex);
            if (SFRender.SFRenderEngine.scene.model_list_simple[mesh].used_count == 0)
                SFRender.SFRenderEngine.scene.model_list_simple.Remove(mesh);

            ModelIndex = Utility.NO_INDEX;*/
        }

        // adds this node to scene cache
        // todo: work out transparency :)
        private void AddTexGeometry()
        {
            //if (ModelIndex != Utility.NO_INDEX)
            //    ClearTexGeometry();

            mesh.MatrixCount += 1;
            if (mesh.MatrixCount == 1)
                SFRender.SFRenderEngine.scene.model_set_simple.Add(mesh);
            //ModelIndex = SFRender.SFRenderEngine.scene.AddModelEntrySimple(mesh, this);
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
            }
            else
                BoneTransforms = null;

            Skeleton = _skeleton;
            Skin = _skin;
        }

        public void SetAnimation(SFAnimation _animation, bool play = true)
        {
            Animation = _animation;
            AnimCurrentTime = 0f;
            AnimPlaying = play;
            if (play == true)
                UpdateBoneTransforms();
        }

        public void SetAnimationTime(float t)
        {
            if (Animation == null)
                return;
            if (t < Animation.max_time)
                AnimCurrentTime = t;
            else  // looping
                AnimCurrentTime = t - (int)(t / Animation.max_time);

            if((BoneTransforms != null)&&(AnimPlaying))
                UpdateBoneTransforms();
        }

        // calculates bone transforms for this node
        private void UpdateBoneTransforms()
        {
            for (int i = 0; i < BoneTransforms.Length; i++)
                BoneTransforms[i] = Animation.CalculateBoneMatrix(i, AnimCurrentTime).to_mat4();
            skeleton.CalculateTransformation(BoneTransforms, ref BoneTransforms);
            for (int i = 0; i < BoneTransforms.Length; i++)
                BoneTransforms[i] = skeleton.bone_inverted_matrices[i] * BoneTransforms[i];
        }


        protected override void UpdateTime(float t)
        {
            SetAnimationTime(t);
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
            if (Mesh != null)
            {
                SFResources.SFResourceManager.Models.Dispose(Mesh.GetName());
                Mesh = null;
            }
            if (Skeleton != null)
            {
                SFResources.SFResourceManager.Skeletons.Dispose(Skeleton.GetName());
                Skeleton = null;
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

        public Vector3 Lookat
        {
            get
            {
                return lookat;
            }
            set
            {
                lookat = value;

                direction.X = (float)(Math.Atan2(-(lookat.Z - position.Z), lookat.X - position.X) + Math.PI);
                if (Math.Abs(lookat.X - position.Z) > EPSILON)
                {
                    direction.Y = (float)Math.Atan2(lookat.Y - position.Y, new Vector3(lookat.X - position.X, -(lookat.Z - position.Z), 0).Length);
                    direction.Y = (direction.Y > MAX_DIR_Y ? MAX_DIR_Y : (direction.Y < -MAX_DIR_Y ? -MAX_DIR_Y : direction.Y));
                }
                //System.Diagnostics.Debug.WriteLine(direction.ToString());
                TouchLocalTransform(); TouchParents();
            }
        }
        public Vector2 Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
                direction.Y = (direction.Y > MAX_DIR_Y ? MAX_DIR_Y : (direction.Y < -MAX_DIR_Y ? -MAX_DIR_Y : direction.Y));
                //calculate rotation vector
                lookat = position + new Vector3((float)Math.Cos(direction.X) * (float)Math.Cos(direction.Y),
                                                (float)Math.Sin(direction.Y),
                                                (float)Math.Sin(direction.X) * (float)Math.Cos(direction.Y));
                TouchLocalTransform(); TouchParents();
            }
        }

        // view matrix: modelmatrix
        // proj matrix: projmatrix
        public Matrix4 ViewProjMatrix { get { return viewproj_matrix; } }
        public Matrix4 ProjMatrix { get { return proj_matrix; } set { proj_matrix = value; viewproj_matrix = proj_matrix; } }
        public Matrix4 ViewMatrix { get { return view_matrix; } }
        public float AspectRatio { get { return aspect_ratio; } set { aspect_ratio = value; } }
        public Physics.Frustum Frustum { get { return frustum; } }

        public SceneNodeCamera(string s): base(s)
        {
            frustum = new Physics.Frustum(position, (lookat-position), SFRender.SFRenderEngine.min_render_distance, SFRender.SFRenderEngine.max_render_distance, aspect_ratio);
            UpdateTransform();
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

        // returns vector2 in screen coordinates in [0, 1] (if on screen)
        public Vector2 WorldToScreen(Vector3 pos)
        {
            float dist = Vector3.Dot(pos - position, (Lookat - position).Normalized()) / SFRender.SFRenderEngine.max_render_distance;

            Vector3 top_left = position + (frustum.frustum_vertices[4] - position) * dist;
            Vector3 top_right = position + (frustum.frustum_vertices[5] - position) * dist;
            Vector3 bottom_left = position + (frustum.frustum_vertices[6] - position) * dist;

            Vector3 top_point_norm = (top_right - top_left).Normalized();
            Vector3 top_point = top_left + top_point_norm * Vector3.Dot(pos - top_left, top_point_norm);
            
            Vector3 left_point_norm = (bottom_left-top_left).Normalized();
            Vector3 left_point = top_left + top_point_norm * Vector3.Dot(pos - top_left, left_point_norm);

            return new Vector2(
                Math.Sign(Vector3.Dot(pos - top_left, top_right - top_left)) * (top_point - top_left).Length / (top_right - top_left).Length,
                Math.Sign(Vector3.Dot(pos - top_left, bottom_left - top_left)) * (left_point - top_left).Length / (bottom_left - top_left).Length);
        }
    }
}
