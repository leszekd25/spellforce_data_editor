/* 
 * SceneNode is the basic component of a scene
 * Every component of the scene inherits from SceneNode
 * SceneNode has one parent and many children
 * Each node contains transform data which is updated only when needed
 * */

using OpenTK;
using System;
using System.Collections.Generic;

namespace SFEngine.SF3D.SceneSynchro
{
    public class SceneNode
    {
        public string Name { get; set; } = "";
        public SceneNode Parent { get; set; } = null;
        public List<SceneNode> Children { get; protected set; } = new List<SceneNode>();

        // if true, on the  next  update  local transform will be  updated
        protected Matrix4 local_transform = Matrix4.Identity;
        public Matrix4 result_transform = Matrix4.Identity;
        protected bool needsanyupdate = true;
        protected bool needsupdatelocaltransform = true;
        protected bool needsupdateresulttransform = true;

        public bool visible = true;
        public Vector3 position = Vector3.Zero;
        public Quaternion rotation = Quaternion.Identity;
        public Vector3 scale = Vector3.One;
        protected Physics.BoundingBox aabb = Physics.BoundingBox.Zero;

        // todo: add a LocalVisible, so even when parent changes to visible while this is invisible, this is still invisible
        public bool Visible
        {
            set
            {
                if (visible != value)
                {
                    visible = value;
                    OnVisibleSwitch();
                    for(int i = 0; i < Children.Count; i++)
                    {
                        Children[i].Visible = value;
                    }
                }
            }
        }
        public Vector3 Position { set { position = value; TouchLocalTransform(); } }
        public Quaternion Rotation { set { rotation = value; TouchLocalTransform(); } }
        public Vector3 Scale { set { scale = value; TouchLocalTransform(); } }

        public SceneNode(string n)
        {
            Name = n;
        }

        // adds a given node as a child of this node
        public void AddNode(SceneNode node)
        {
            if (node == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SceneNode.AddNode(): Node to add is null!");
                return;
            }
            Children.Add(node);
            node.Parent = this;
            node.Visible = visible;
            if(node.visible)
            {
                node.TouchResultTransform();
            }
        }

        // removes a given node from the children of this node
        public void RemoveNode(SceneNode node)
        {
            if (node == null)
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
            {
                n.Parent.RemoveNode(n);
            }
        }

        // changes parent of this node from current parent to a given node
        public void SetParent(SceneNode node)
        {
            if (Parent != null)
            {
                Parent.RemoveNode(this);
            }

            if (node != null)
            {
                node.AddNode(this);
            }
            else
            {
                Visible = false;
            }
        }

        // utility function which sets rotation of this node to a given angle (degrees) around the UP axis (0, 1, 0)
        public void SetAnglePlane(int angle_deg)
        {
            Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2)
                     * Quaternion.FromAxisAngle(new Vector3(0f, 0f, 1f), (float)(angle_deg * Math.PI / 180.0f));
        }

        // if something requires updating, notify all parents about that, root including, so the engine knows to run update routine
        public void TouchParents()
        {
            if(needsanyupdate)
            {
                return;
            }

            needsanyupdate = true;
            Parent?.TouchParents();
        }

        // if one local transform changes, so must the result transform
        public void TouchLocalTransform()
        {
            if(needsupdatelocaltransform)
            {
                return;
            }

            needsupdatelocaltransform = true;
            TouchParents();
            TouchResultTransform();
        }

        // if one result transform changes, so must all subsequent result transforms
        public void TouchResultTransform()
        {
            if(needsupdateresulttransform)
            {
                return;
            }

            needsanyupdate = true;
            needsupdateresulttransform = true;
            for(int i = 0; i < Children.Count; i++)
            {
                Children[i].TouchResultTransform();
            }
        }

        // updates the node and all children nodes
        public void Update(float dt)
        {
            if (visible)
            {
                UpdateInternal(dt);

                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].Update(dt);
                }

                needsanyupdate = false;
            }
        }

        protected virtual void UpdateInternal(float dt)
        {
            if (needsupdatelocaltransform)
            {
                Matrix4 translation_matrix = Matrix4.CreateTranslation(position);
                Matrix4 rotation_matrix = Matrix4.CreateFromQuaternion(rotation);
                Matrix4 scale_matrix = Matrix4.CreateScale(scale);
                local_transform = scale_matrix * rotation_matrix * translation_matrix;

                needsupdatelocaltransform = false;
            }

            if (needsupdateresulttransform)
            {
                if (Parent != null)
                {
                    result_transform = local_transform * Parent.result_transform;
                }
                else
                {
                    result_transform = local_transform;
                }

                needsupdateresulttransform = false;
            }
        }

        public void SetTime(float t)
        {
            SetTimeInternal(t);

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].SetTime(t);
            }
        }

        protected virtual void SetTimeInternal(float t)
        {

        }

        protected virtual void OnVisibleSwitch()
        {

        }

        // finds a node of a given type, given a path to node
        public T FindNode<T>(string path) where T : SceneNode
        {
            string[] names = (Name + '.' + path).Split('.');
            return FindNode<T>(names, 0);
        }

        private T FindNode<T>(string[] names, int current_index) where T : SceneNode
        {
            if (names[current_index] == Name)
            {
                if ((current_index == names.Length - 1) && (GetType() == typeof(T)))
                {
                    return (T)this;
                }

                T result = null;
                for (int i = 0; i < Children.Count; i++)
                {
                    result = Children[i].FindNode<T>(names, current_index + 1);
                    if (result != null)
                    {
                        break;
                    }
                }
                return result;
            }
            return null;
        }

        // returns full name of the node (recursive, slow)
        public string GetFullPath()
        {
            if (Parent == null)
            {
                return Name;
            }

            return Parent.GetFullPath() + '.' + Name;
        }

        // disposes node and all resources its using
        public void Dispose()
        {
            InternalDispose();

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Dispose();
            }

            while (Children.Count != 0)
            {
                Children[0].SetParent(null);
            }
        }

        protected virtual void InternalDispose()
        {

        }

        public override string ToString()
        {
            return Name + " (" + Children.Count + " children)";
        }
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
                if(value == mesh)
                {
                    return;
                }

                if (visible)
                {
                    ClearTexGeometry();
                }
                mesh = value;
                if (visible)
                {
                    AddTexGeometry();
                }
            }
        }

        public bool IsDecal = false;
        public int DecalIndex = Utility.NO_INDEX;
        public int CurrentMeshMatrixIndex = Utility.NO_INDEX;

        public SceneNodeSimple(string n) : base(n) { }

        // removes this node from scene cache
        // assumes mesh exists
        private void ClearTexGeometry()
        {
            if (mesh == null)
            {
                return;
            }

            aabb = Physics.BoundingBox.Zero;
            mesh.ForceUpdateInstanceMatrices = true;
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
            if(mesh == null)
            {
                return;
            }

            aabb = mesh.aabb;
            mesh.ForceUpdateInstanceMatrices = true;
            mesh.MatrixCount += 1;
            if (mesh.MatrixCount == 1)
            {
                SFRender.SFRenderEngine.scene.model_set_simple.Add(mesh);

                foreach (var submodel in mesh.submodels)
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

        protected override void UpdateInternal(float dt)
        {
            if (needsanyupdate)
            {
                if (needsupdatelocaltransform)
                {
                    Matrix4 translation_matrix = Matrix4.CreateTranslation(position);
                    Matrix4 rotation_matrix = Matrix4.CreateFromQuaternion(rotation);
                    Matrix4 scale_matrix = Matrix4.CreateScale(scale);
                    local_transform = scale_matrix * rotation_matrix * translation_matrix;

                    needsupdatelocaltransform = false;
                }

                if (needsupdateresulttransform)
                {
                    if (Parent != null)
                    {
                        result_transform = local_transform * Parent.result_transform;
                    }
                    else
                    {
                        result_transform = local_transform;
                    }

                    needsupdateresulttransform = false;
                }
            }

            if (mesh != null)
            {
                CurrentMeshMatrixIndex = mesh.CurrentMatrixIndex;
                if ((mesh.ForceUpdateInstanceMatrices) || (needsanyupdate))
                {
                    SFSubModel3D.Cache.MatrixBufferData[mesh.MatrixOffset + CurrentMeshMatrixIndex] = result_transform;
                }
                mesh.CurrentMatrixIndex += 1;
            }
        }

        protected override void OnVisibleSwitch()
        {
            if (mesh != null)
            {
                if (!visible)
                {
                    ClearTexGeometry();
                }
                else
                {
                    AddTexGeometry();
                }
            }
        }

        // disposes mesh used by this node (reference counted, dw)
        protected override void InternalDispose()
        {
            SFResources.SFResourceManager.Models.Dispose(Mesh);
            Mesh = null;

            if (IsDecal)
            {
                SFRender.SFRenderEngine.scene.decal_info.RemoveAt(DecalIndex);
            }
        }
    }

    // use this for displaying animated 3d meshes
    public class SceneNodeAnimated : SceneNode
    {
        private SFModel3D mesh = null;        // this is only for extraction convenience!
        private SFSkeleton skeleton = null;
        private SFModelSkin skin = null;
        private SFAnimation animation = null;
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
                {
                    aabb = mesh.aabb;
                }
                else
                {
                    aabb = new Physics.BoundingBox(Vector3.Zero, Vector3.Zero);
                }
            }
        }
        public SFSkeleton Skeleton { get { return skeleton; } }
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
                {
                    ClearTexGeometry();
                }
                else if (visible)
                {
                    AddTexGeometry();
                }
            }
        }

        public SFAnimation Animation { get { return animation; } }
        public Matrix4[] BoneTransforms = null;

        private float anim_current_time = 0;
        public float AnimCurrentTime { get { return anim_current_time; } }
        public bool AnimPlaying = false;

        // if this node is not primary, it inherits all skeleton calculations from its primary
        // primary node can't be a secondary of another
        public bool Primary = false;
        // driven nodes are affected by this node (in particular, this node is driven by itself)
        // if primary, DrivenNodes[0] = this, ensured by SFScene
        public List<SceneNodeAnimated> DrivenNodes = null;

        public SceneNodeAnimated(string n) : base(n) { }

        public void SetSkin(SFModelSkin _skin)
        {
            Skin = _skin;
        }

        public void SetSkeleton(SFSkeleton _skeleton)
        {
            if(!Primary)
            {
                return;
            }

            if (_skeleton != null)
            {
                BoneTransforms = new Matrix4[_skeleton.bone_count];
                for (int i = 0; i < _skeleton.bone_count; i++)
                {
                    BoneTransforms[i] = Matrix4.Identity;
                }
            }
            else
            {
                BoneTransforms = null;
            }

            skeleton = _skeleton;
        }

        public void SetAnimation(SFAnimation _animation, bool play = true)
        {
            if (!Primary)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SceneNodeAnimated.SetAnimation(): Node is not primary; this call has no effect");
                return;
            }
            if (skeleton == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SceneNodeAnimated.SetAnimation(): Skeleton is missing!");
                return;
            }

            animation = _animation;
            anim_current_time = 0f;
            AnimPlaying = play;
            if (play == true)
            {
                UpdateBoneTransforms();
            }
        }

        // calculates bone transforms for this node
        // only ever called if primary is not set - means this node is primary
        private void UpdateBoneTransforms()
        {
            float t = anim_current_time;
            MathUtils.Clamp(ref t, 0, animation.max_time);
            t *= SFAnimation.ANIMATION_FPS;
            int k = (int)t;
            t = t - k;

            for (int i = 0; i < BoneTransforms.Length; i++)
            {
                BoneAnimationState[] ba = animation.bone_animations[i];

                BoneAnimationState bas;
                BoneAnimationState.FastLerp(ref ba[k], ref ba[k + 1], t, out bas);

                BoneAnimationState.Multiply(ref skeleton.bone_inverted_state[i], ref bas, out bas);
                bas.ToMatrix(out BoneTransforms[i]);
            }
        }

        public void SetAnimationCurrentTime(float t)
        {
            // if this node has a primary animation node, early exit - primary always drives its children's animation
            if (!Primary)
            {
                return;
            }

            if (animation == null)
            {
                return;
            }

            anim_current_time = t;
            if (anim_current_time > animation.max_time)
            {
                anim_current_time -= (int)(anim_current_time / animation.max_time) * animation.max_time;
            }

            if ((visible) && (BoneTransforms != null) && (AnimPlaying))
            {
                // determine if should update, based on scene camera distance and on bounding box
                float dist = (SFRender.SFRenderEngine.scene.camera.position - result_transform.Row3.Xyz).Length;
                float size = (aabb.a - aabb.center).Length;
                if (size == 0)
                {
                    return;
                }

                int skip_count = 1 + (int)((dist / size) * 0.1f);
                if (skip_count == 1)
                {
                    UpdateBoneTransforms();
                }
                else if ((SFRender.SFRenderEngine.scene.frame_counter % skip_count) == 0)
                {
                    UpdateBoneTransforms();
                }
            }
        }

        protected override void UpdateInternal(float dt)
        {
            if (dt != 0)
            {
                SetAnimationCurrentTime(anim_current_time + dt);
            }

            if (needsanyupdate)
            {
                if (needsupdatelocaltransform)
                {
                    Matrix4 translation_matrix = Matrix4.CreateTranslation(position);
                    Matrix4 rotation_matrix = Matrix4.CreateFromQuaternion(rotation);
                    Matrix4 scale_matrix = Matrix4.CreateScale(scale);
                    local_transform = scale_matrix * rotation_matrix * translation_matrix;

                    needsupdatelocaltransform = false;
                }

                if (needsupdateresulttransform)
                {
                    if (Parent != null)
                    {
                        result_transform = local_transform * Parent.result_transform;
                    }
                    else
                    {
                        result_transform = local_transform;
                    }

                    needsupdateresulttransform = false;
                }
            }
        }

        protected override void SetTimeInternal(float t)
        {
            if (animation == null)
            {
                return;
            }

            SetAnimationCurrentTime(t);
        }

        private void ClearTexGeometry()
        {
            if (Primary)
            {
                SF3D.SFRender.SFRenderEngine.scene.an_primary_nodes.Remove(this);
            }
        }

        private void AddTexGeometry()
        {
            if (Primary)
            {
                SF3D.SFRender.SFRenderEngine.scene.an_primary_nodes.Add(this);
            }
        }

        protected override void OnVisibleSwitch()
        {
            if (skin != null)
            {
                if (!visible)
                {
                    ClearTexGeometry();
                }
                else
                {
                    AddTexGeometry();
                }
            }
        }

        // disposes skeleton and skin used by this node (reference counted)
        protected override void InternalDispose()
        {
            SFResources.SFResourceManager.Models.Dispose(Mesh);
            Mesh = null;
            SFResources.SFResourceManager.Skins.Dispose(Skin);
            Skin = null;
            if (Primary)
            {
                SFResources.SFResourceManager.Skeletons.Dispose(skeleton);
                skeleton = null;

                animation = null;
                anim_current_time = 0f;
                AnimPlaying = false;

                BoneTransforms = null;

                for(int i = 1; i < DrivenNodes.Count; i++)
                {
                    DrivenNodes[i].Dispose();
                }
                DrivenNodes.Clear();
            }

            Primary = false;
        }
    }

    // this node should be attached to a SceneNodeAnimated node,
    // for example if you want a weapon node to follow an arm bone of a character node
    public class SceneNodeBone : SceneNode
    {
        // parent must be SceneNodeAnimated
        private int BoneIndex = Utility.NO_INDEX;
        public int SceneIndex = Utility.NO_INDEX;

        public SceneNodeBone(string n) : base(n) { }

        protected override void UpdateInternal(float dt)
        {
            if (needsupdateresulttransform)
            {
                SceneNodeAnimated pp = (SceneNodeAnimated)Parent;
                if (BoneIndex != Utility.NO_INDEX)
                {
                    pp.Skeleton.bone_reference_state[BoneIndex].ToMatrix(out Matrix4 skel_ref);

                    result_transform = local_transform * skel_ref * pp.BoneTransforms[BoneIndex] * Parent.result_transform;
                }

                needsupdateresulttransform = false;
            }
        }

        // sets to which bone this node should be attached
        public void SetBone(string name)
        {
            BoneIndex = Utility.NO_INDEX;
            if (Parent == null)
            {
                return;
            }

            if (((SceneNodeAnimated)Parent).Skeleton == null)
            {
                return;
            }

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

        protected override void InternalDispose()
        {
            SFRender.SFRenderEngine.scene.an_bone_nodes.RemoveAt(SceneIndex);
        }
    }

    // this is mainly used as a convenience for certain operations
    public class SceneNodeMapChunk : SceneNode
    {
        public SFMap.SFMapHeightMapChunk MapChunk;
        public float DistanceToCamera { get; set; } = 0f;
        public float CameraHeightDifference { get; set; } = 0f;

        public SceneNodeMapChunk(string n) : base(n) { }

        protected override void InternalDispose()
        {
            if (MapChunk != null)
            {
                MapChunk.Unload();
            }
        }
    }

    // camera node should not have any children or unexpected behavior might occur
    public class SceneNodeCamera : SceneNode
    {
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
        public Matrix4 ProjMatrix { get { return proj_matrix; } set { proj_matrix = value; viewproj_matrix = proj_matrix; needsanyupdate = true; needsupdatelocaltransform = true; } }
        public Matrix4 ViewMatrix { get { return view_matrix; } }
        public float AspectRatio { get { return aspect_ratio; } set { aspect_ratio = value; needsanyupdate = true; needsupdatelocaltransform = true; } }
        public Physics.Frustum Frustum { get { return frustum; } }

        // 250.0f is a magic constant for now...
        public SceneNodeCamera(string s) : base(s)
        {
            frustum = new Physics.Frustum(position, (lookat - position), SFRender.SFRenderEngine.min_render_distance, 1.136f * Settings.ObjectFadeMax, aspect_ratio);
            UpdateInternal(0);
        }

        // in radians
        public void SetAzimuthAltitude(Vector2 dir)
        {
            direction = dir;
            if (direction.Y < -MAX_DIR_Y)
            {
                direction.Y = -MAX_DIR_Y;
            }

            if (direction.Y > MAX_DIR_Y)
            {
                direction.Y = MAX_DIR_Y;
            }

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
            {
                direction.X = 0;
            }
            else
            {
                DirVector2.Normalize();
                if (DirVector.Z < 0)
                {
                    direction.X = (float)(Vector3.CalculateAngle(Vector3.UnitX, DirVector2));
                }
                else
                {
                    direction.X = -(float)(Vector3.CalculateAngle(Vector3.UnitX, DirVector2));
                }
            }

            direction.Y = (float)(Math.PI / 2) - (float)(Vector3.CalculateAngle(DirVector, Vector3.UnitY));

            TouchLocalTransform(); TouchParents();
        }

        protected override void UpdateInternal(float dt)
        {
            if (needsupdatelocaltransform)
            {
                view_matrix = Matrix4.LookAt(position, lookat, new Vector3(0, 1, 0));
                viewproj_matrix = view_matrix * ProjMatrix;

                // calculate frustum
                frustum.start = position;
                frustum.direction = (lookat - position);
                frustum.aspect_ratio = aspect_ratio;
                frustum.Calculate();

                needsupdatelocaltransform = false;
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
            {
                return Vector2.Zero;
            }

            Vector3 NDC_space_vec = clip_space_vec.Xyz / clip_space_vec.W;

            return new Vector2(((NDC_space_vec.X + 1.0f) / 2.0f), ((1.0f - NDC_space_vec.Y) / 2.0f));
        }
    }
}
