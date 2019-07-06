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
        public string Name = "";
        public SceneNode Parent = null;
        public List<SceneNode> Children { get; protected set; } = new List<SceneNode>();

        public virtual bool NeedsUpdate { get; protected set; } = true;
        public virtual bool NeedsUpdateLocalTransform { get; protected set; } = true;
        protected Matrix4 local_transform = Matrix4.Identity;
        protected Matrix4 result_transform = Matrix4.Identity;
        public Matrix4 LocalTransform { get { return local_transform; } set { local_transform = value; TouchLocalTransform(); Touch(); } }
        public Matrix4 ResultTransform { get { return result_transform; } protected set { result_transform = value; } }
        public bool Visible { get; set; } = true;

        public RenderFlags render_flags;

        public SceneNode(string n)
        {
            Name = n;
        }

        public void AddNode(SceneNode node)
        {
            Children.Add(node);
            node.Parent = this;
        }

        public void RemoveNode(SceneNode node)
        {
            node.Parent = null;
            Children.Remove(node);
        }

        public void SetParent(SceneNode node)
        {
            if (Parent != null)
                Parent.RemoveNode(this);
            node.AddNode(this);
        }

        // public void FindNode()

        // if something requires updating, notify all parents about that, root including, so the engine knows to run update routine
        public void Touch()
        {
            NeedsUpdate = true;
            if ((Parent != null) && (Parent.NeedsUpdate != true))
                Parent.Touch();
        }

        // if one local transform changes, so must all subsequent transforms
        public void TouchLocalTransform()
        {
            NeedsUpdateLocalTransform = true;
            NeedsUpdate = true;
            foreach (SceneNode node in Children)
                node.TouchLocalTransform();
        }

        public virtual void UpdateTransform()
        {
            if (!NeedsUpdate)
                return;

            if (NeedsUpdateLocalTransform)
            {
                if (Parent != null)
                    result_transform = local_transform * Parent.ResultTransform;
                else
                    result_transform = local_transform;
                NeedsUpdateLocalTransform = false;
            }

            NeedsUpdate = false;

            foreach (SceneNode node in Children)
                node.UpdateTransform();
        }

        public T FindNode<T>(string path) where T: SceneNode
        {
            string[] names = path.Split('.');
            return FindNode<T>(names, 0);
        }

        private T FindNode<T>(string[] names, int current_index) where T: SceneNode
        {
            if ((names[current_index] == Name)&&(this.GetType() == typeof(T)))
                return (T)this;
            if (current_index == names.Length - 1)
                return null;
            T result = null;
            foreach(SceneNode node in Children)
            {
                result = node.FindNode<T>(names, current_index + 1);
                if (result != null)
                    break;
            }
            return result;
        }

        public string GetFullPath()
        {
            if (Parent == null)
                return this.Name;
            return Parent.GetFullPath() + '.' + this.Name;
        }
    }

    public struct TexGeometryLinkSimple
    {
        public SFTexture texture;
        public int index;
    }

    public class SceneNodeSimple : SceneNode
    {
        public SFModel3D Mesh
        {
            get
            {
                return Mesh;
            }
            set
            {
                Mesh = value;
                if (Mesh == null)
                    ClearTexGeometry();
                else
                    AddTexGeometry();
            }
        }
        public TexGeometryLinkSimple[] TextureGeometryIndex { get; private set; } = null;

        public SceneNodeSimple(string n) : base(n) { }

        // also do this if transparent = true, let transparent object list deal with those
        private void ClearTexGeometry()
        {
            if (TextureGeometryIndex == null)
                return;

            foreach (TexGeometryLinkSimple link in TextureGeometryIndex)
                SFRender.SFRenderEngine.scene.tex_list_simple[link.texture].RemoveAt(link.index);

            TextureGeometryIndex = null;
        }

        // also do this if transparent = false, let transparent object list deal with those
        private void AddTexGeometry()
        {
            if (TextureGeometryIndex != null)
                ClearTexGeometry();

            TextureGeometryIndex = new TexGeometryLinkSimple[Mesh.materials.Length];

            for(int i = 0; i < Mesh.materials.Length; i++)
            {
                TextureGeometryIndex[i].texture = Mesh.materials[i].texture;
                // long name :^)
                TexturedGeometryListElementSimple elem = new TexturedGeometryListElementSimple();
                elem.node = this;
                elem.submodel_index = i;
                TextureGeometryIndex[i].index = SFRender.SFRenderEngine.scene.AddTextureEntrySimple(Mesh.materials[i].texture, elem);
            }
        }
    }

    public class SceneNodeAnimated : SceneNode
    {
        public SFSkeleton Skeleton { get; private set; } = null;
        public SFModelSkin Skin { get; private set; } = null;
        public SFAnimation Animation { get; private set; } = null;
        public Matrix4[] BoneTransforms = null;
        public float AnimCurrentTime { get; private set; } = 0;
        public bool AnimPlaying { get; set; } = false;

        public SceneNodeAnimated(string n) : base(n) { }

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
            else
                AnimCurrentTime = t - (int)(t / Animation.max_time);

            UpdateBoneTransforms();
        }

        private void UpdateBoneTransforms()
        {
            for (int i = 0; i < BoneTransforms.Length; i++)
                BoneTransforms[i] = Animation.CalculateBoneMatrix(i, AnimCurrentTime).to_mat4();
            Skeleton.CalculateTransformation(BoneTransforms, ref BoneTransforms);
            for (int i = 0; i < BoneTransforms.Length; i++)
                BoneTransforms[i] = Skeleton.bone_inverted_matrices[i] * BoneTransforms[i];
        }
    }

    public class SceneNodeBone : SceneNode
    {
        // parent must be SceneNodeAnimated
        private int BoneIndex = -1;
        public override bool NeedsUpdate { get { return true; } }
        public override bool NeedsUpdateLocalTransform { get { return true; } }

        public SceneNodeBone(string n) : base(n) { }

        public override void UpdateTransform()
        {
            if (!NeedsUpdate)
                return;

            if (NeedsUpdateLocalTransform)
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
                NeedsUpdateLocalTransform = false;
            }

            NeedsUpdate = false;

            foreach (SceneNode node in Children)
                node.UpdateTransform();
        }

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
    

    public class SceneNodeMapChunk: SceneNode
    {
        private SFMap.SFMapHeightMapChunk MapChunk;

        public SceneNodeMapChunk(string n) : base(n) { }
    }
}
