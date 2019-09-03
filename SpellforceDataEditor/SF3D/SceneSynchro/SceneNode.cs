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
        public string Name { get; private set; } = "";
        public SceneNode Parent { get; set; } = null;
        public List<SceneNode> Children { get; protected set; } = new List<SceneNode>();

        // if true, on the  next  update  local transform will be  updated
        public virtual bool NeedsUpdateLocalTransform { get; protected set; } = true;
        public virtual bool NeedsUpdateResultTransform { get; protected set; } = true;

        protected bool visible = true;
        protected Vector3 position = new Vector3(0);
        protected Quaternion rotation = new Quaternion(0, 0, 0, 1);
        protected Vector3 scale = new Vector3(1);
        protected Matrix4 local_transform = Matrix4.Identity;
        protected Matrix4 result_transform = Matrix4.Identity;
        
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
        public Vector3 Position { get { return position; } set { position = value; TouchLocalTransform(); TouchParents(); } }
        public Quaternion Rotation { get { return rotation; } set { rotation = value; TouchLocalTransform(); TouchParents(); } }
        public Vector3 Scale { get { return scale; } set { scale = value; TouchLocalTransform(); TouchParents(); } }
        public Matrix4 LocalTransform { get { return local_transform; } protected set { local_transform = value; } }
        public Matrix4 ResultTransform { get { return result_transform; } protected set { result_transform = value; } }

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

        // utility function which sets rotation of this node to a given angle (degrees) around the UP axis (0,  1, 0)
        public void SetAnglePlane(int angle_deg)
        {
            Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2)
                     * Quaternion.FromAxisAngle(new Vector3(0, 0, 1), MathUtils.DegToRad(angle_deg));
        }

        // if something requires updating, notify all parents about that, root including, so the engine knows to run update routine
        protected void TouchParents()
        {
            NeedsUpdateLocalTransform = true;
            if ((Parent != null) && (Parent.NeedsUpdateLocalTransform != true))
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
            NeedsUpdateResultTransform = true;
            foreach (SceneNode node in Children)
                node.TouchResultTransform();
        }

        // updates the node and all children nodes
        public void Update(float t)
        {
            UpdateTime(t);
            UpdateTransform();

            foreach (SceneNode node in Children)
                node.Update(t);
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
                    result_transform = local_transform * Parent.ResultTransform;
                else
                    result_transform = local_transform;
                NeedsUpdateResultTransform = false;
            }
        }

        // updates node according to the given time parameter
        protected virtual void UpdateTime(float t)
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
                mesh = value;
                if (mesh == null)
                    ClearTexGeometry();
                else if(Visible)
                    AddTexGeometry();
            }
        }
        public override bool Visible
        {
            get => base.Visible;
            set
            {
                if (value == false)
                    ClearTexGeometry();
                else
                    AddTexGeometry();
                base.Visible = value;
            }
        }
        // list of scene cache pointers
        public TexGeometryLinkSimple[] TextureGeometryIndex { get; private set; } = null;

        public SceneNodeSimple(string n) : base(n) { }

        // clears scene cache of all elements drawn by this node
        // todo: also do this if transparent = true, let transparent object list deal with those
        private void ClearTexGeometry()
        {
            if (TextureGeometryIndex == null)
                return;

            foreach (TexGeometryLinkSimple link in TextureGeometryIndex)
            {
                if (link.texture == null)
                {
                    SFRender.SFRenderEngine.scene.untextured_list_simple.RemoveAt(link.index);
                    if (SFRender.SFRenderEngine.scene.untextured_list_simple.used_count == 0)
                        SFRender.SFRenderEngine.scene.untextured_list_simple.Clear();
                }
                else
                {
                    SFRender.SFRenderEngine.scene.tex_list_simple[link.texture].RemoveAt(link.index);
                    if (SFRender.SFRenderEngine.scene.tex_list_simple[link.texture].used_count == 0)
                        SFRender.SFRenderEngine.scene.tex_list_simple.Remove(link.texture);
                }
            }

            TextureGeometryIndex = null;
        }

        // adds elements drawn by this node to scene cache
        // todo: also do this if transparent = false, let transparent object list deal with those
        private void AddTexGeometry()
        {
            if (TextureGeometryIndex != null)
                ClearTexGeometry();
            if (mesh == null)
                return;

            TextureGeometryIndex = new TexGeometryLinkSimple[mesh.materials.Length];

            for(int i = 0; i < Mesh.materials.Length; i++)
            {
                TexturedGeometryListElementSimple elem = new TexturedGeometryListElementSimple();
                elem.node = this;
                elem.submodel_index = i;
                TextureGeometryIndex[i].texture = mesh.materials[i].texture;
                if (mesh.materials[i].texture == null)
                    TextureGeometryIndex[i].index = SFRender.SFRenderEngine.scene.AddUntexturedEntrySimple(elem);
                else
                    TextureGeometryIndex[i].index = SFRender.SFRenderEngine.scene.AddTextureEntrySimple(mesh.materials[i].texture, elem);
            }
        }

        // disposes mesh used by this node (reference counted, dw)
        protected override void InternalDispose()
        {
            if(Mesh != null)
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

    public class SceneNodeAnimated : SceneNode
    {
        private SFSkeleton skeleton = null;
        private SFModelSkin skin = null;
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
                if (value == false)
                    ClearTexGeometry();
                else
                    AddTexGeometry();
                base.Visible = value;
            }
        }
        // list of scene cache pointers
        public TexGeometryLinkAnimated[] TextureGeometryIndex { get; private set; } = null;

        public SceneNodeAnimated(string n) : base(n) { }

        // this must be used
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
            if (TextureGeometryIndex == null)
                return;

            foreach (TexGeometryLinkAnimated link in TextureGeometryIndex)
            {
                SFRender.SFRenderEngine.scene.tex_list_animated[link.texture].RemoveAt(link.index);
                if (SFRender.SFRenderEngine.scene.tex_list_animated[link.texture].used_count == 0)
                    SFRender.SFRenderEngine.scene.tex_list_animated.Remove(link.texture);
            }

                TextureGeometryIndex = null;
        }

        // adds elements drawn by this node to scene cache
        // also do this if transparent = false, let transparent object list deal with those
        private void AddTexGeometry()
        {
            if (TextureGeometryIndex != null)
                ClearTexGeometry();
            if (skin == null)
                return;

            TextureGeometryIndex = new TexGeometryLinkAnimated[skin.submodels.Count];

            for (int i = 0; i < Skin.submodels.Count; i++)
            {
                TextureGeometryIndex[i].texture = skin.submodels[i].material.texture;
                // long name :^)
                TexturedGeometryListElementAnimated elem = new TexturedGeometryListElementAnimated();
                elem.node = this;
                elem.submodel_index = i;
                TextureGeometryIndex[i].index = SFRender.SFRenderEngine.scene.AddTextureEntryAnimated(skin.submodels[i].material.texture, elem);
            }
        }

        // disposes skeleton and skin used by this node (reference counted)
        protected override void InternalDispose()
        {
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

    //  this node should be attached to a SceneNodeAnimated node
    public class SceneNodeBone : SceneNode
    {
        // parent must be SceneNodeAnimated
        private int BoneIndex = -1;
        public override bool NeedsUpdateLocalTransform { get { return true; } }

        public SceneNodeBone(string n) : base(n) { }

        protected override void UpdateTransform()
        {
            if (Parent != null)
            {
                if((Parent.GetType() == typeof(SceneNodeAnimated))&&(BoneIndex != -1))
                    result_transform = local_transform * ((SceneNodeAnimated)(Parent)).Skeleton.bone_reference_matrices[BoneIndex]
                        * ((SceneNodeAnimated)(Parent)).BoneTransforms[BoneIndex] * Parent.ResultTransform;
                else
                    result_transform = local_transform * Parent.ResultTransform;
            }
            else
                result_transform = local_transform;

            TouchLocalTransform();
        }

        // sets to which bone this node should be attached
        public void SetBone(string name)
        {
            BoneIndex = -1;
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
    
    // this is handled somewhere else
    public class SceneNodeMapChunk: SceneNode
    {
        public SFMap.SFMapHeightMapChunk MapChunk;

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
        private Vector3 lookat = new Vector3(0, 1, 0);
        private Vector2 direction = new Vector2(0, 0);
        private Matrix4 proj_matrix = new Matrix4();
        private Matrix4 viewproj_matrix = new Matrix4();
        private Vector3[] frustum_vertices;// = camera.get_frustrum_vertices();
        // construct frustrum planes
        private Physics.Plane[] frustum_planes;// = new Physics.Plane[6];

        public Vector3 Lookat
        {
            get
            {
                return lookat;
            }
            set
            {
                lookat = value;

                direction.X = (float)Math.Atan2(-(lookat.Z - Position.Z), lookat.X - Position.X) + 2 * 1.570796f;
                if (Math.Abs(lookat.X - Position.Z) > 0.0001)
                {
                    direction.Y = (float)Math.Atan2(lookat.Y - Position.Y, new Vector3(lookat.X - Position.X, -(lookat.Z - Position.Z), 0).Length);
                    direction.Y = (direction.Y > 1.5 ? 1.5f : (direction.Y < -1.5 ? -1.5f : direction.Y));
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
                direction.Y = (direction.Y > 1.5 ? 1.5f : (direction.Y < -1.5 ? -1.5f : direction.Y));
                //calculate rotation vector
                lookat = Position + new Vector3((float)Math.Cos(direction.X) * (float)Math.Cos(direction.Y),
                                                (float)Math.Sin(direction.Y),
                                                (float)Math.Sin(direction.X) * (float)Math.Cos(direction.Y));
                TouchLocalTransform(); TouchParents();
            }
        }

        // view matrix: modelmatrix
        // proj matrix: projmatrix
        public Matrix4 ViewProjMatrix { get { return viewproj_matrix; } }
        public Matrix4 ProjMatrix { get { return proj_matrix; } set { proj_matrix = value; viewproj_matrix = proj_matrix; } }
        public Physics.Plane[] FrustumPlanes { get { return frustum_planes; } }
        public Vector3[] FrustumVertices { get { return frustum_vertices; } }

        public SceneNodeCamera(string s): base(s)
        {
            frustum_vertices = new Vector3[8];
            frustum_planes = new Physics.Plane[6];
            UpdateTransform();
        }

        // todo: can be optimized
        protected override void UpdateTransform()
        {
            if (NeedsUpdateLocalTransform)
            {
                local_transform = Matrix4.LookAt(Position, lookat, new Vector3(0, 1, 0));
                viewproj_matrix = local_transform * ProjMatrix;

                // calculate frustrum geometry
                CalculateFrustumVertices();
                frustum_planes[0] = new Physics.Plane(frustum_vertices[0], frustum_vertices[4], frustum_vertices[1]);  // top plane
                frustum_planes[1] = new Physics.Plane(frustum_vertices[2], frustum_vertices[3], frustum_vertices[6]);  // bottom plane
                frustum_planes[2] = new Physics.Plane(frustum_vertices[0], frustum_vertices[2], frustum_vertices[4]);  // left plane
                frustum_planes[3] = new Physics.Plane(frustum_vertices[1], frustum_vertices[5], frustum_vertices[3]);  // right plane
                frustum_planes[4] = new Physics.Plane(frustum_vertices[0], frustum_vertices[1], frustum_vertices[2]);  // near plane
                frustum_planes[5] = new Physics.Plane(frustum_vertices[4], frustum_vertices[6], frustum_vertices[5]);  // far plane

                NeedsUpdateLocalTransform = false;
            }
        }

        public void translate(Vector3 tr)
        {
            position += tr;
            lookat += tr;
            TouchLocalTransform(); TouchParents();
        }

        // todo: this is wrong at certain angles?
        private void CalculateFrustumVertices()
        {
            // get forward, up, right direction
            Vector3 forward = (lookat - Position).Normalized();
            Vector3 right = Vector3.Cross(forward, new Vector3(0, 1, 0)).Normalized();
            Vector3 up = Vector3.Cross(forward, right);

            // 100f, Math.Pi/4 are magic for now.....
            float deviation = (float)Math.Tan(Math.PI / 4) / 2;
            Vector3 center = position + forward * 0.1f;
            Vector3 center2 = position + forward * 100f;
            frustum_vertices[0] = center + (-right - up) * deviation * 0.1f;
            frustum_vertices[1] = center + (right - up) * deviation * 0.1f;
            frustum_vertices[2] = center + (-right + up) * deviation * 0.1f;
            frustum_vertices[3] = center + (right + up) * deviation * 0.1f;
            frustum_vertices[4] = center2 + (-right - up) * deviation * 100f;
            frustum_vertices[5] = center2 + (right - up) * deviation * 100f;
            frustum_vertices[6] = center2 + (-right + up) * deviation * 100f;
            frustum_vertices[7] = center2 + (right + up) * deviation * 100f;
        }
    }
}
