using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SpellforceDataEditor.SF3D.SFRender
{
    public class RenderNode
    {
        public string Name = "";
        public RenderNode Parent = null;
        public List<RenderNode> Children { get; private set; } = new List<RenderNode>();

        public bool NeedsUpdate { get; private set; } = true;
        public bool NeedsUpdateLocalTransform { get; private set; } = true;
        private Matrix4 local_transform = Matrix4.Identity;
        private Matrix4 result_transform = Matrix4.Identity;
        public Matrix4 LocalTransform { get { return local_transform; } set { local_transform = value; TouchLocalTransform(); Touch(); } }
        public Matrix4 ResultTransform { get { return result_transform; } private set { result_transform = value; } }

        public RenderNode(string n)
        {
            Name = n;
        }

        public void AddNode(RenderNode node)
        {
            Children.Add(node);
            node.Parent = this;
        }

        public void RemoveNode(RenderNode node)
        {
            node.Parent = null;
            Children.Remove(node);
        }

        public void SetParent(RenderNode node)
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
            foreach (RenderNode node in Children)
                node.TouchLocalTransform();
        }

        public void UpdateTransform()
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

            foreach (RenderNode node in Children)
                node.UpdateTransform();
        }

        public void PreRender()
        {
            OnPreRender();
            foreach (RenderNode node in Children)
                node.PreRender();
        }

        public virtual void OnPreRender()
        {

        }

        public void Render()
        {
            OnRender();
            foreach (RenderNode node in Children)
                node.Render();
        }

        public virtual void OnRender()
        {

        }

        public void PostRender()
        {
            OnPostRender();
            foreach (RenderNode node in Children)
                node.PostRender();
        }

        public virtual void OnPostRender()
        {

        }
    }
}
