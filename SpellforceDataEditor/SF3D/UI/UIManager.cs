using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SpellforceDataEditor.SF3D.UI
{
    public struct UIElementIndex
    {
        public SFTexture tex;
        public int span_index;
    }

    public class UIManager
    {
        public Dictionary<SFTexture, UIQuadStorage> storages { get; private set; } = new Dictionary<SFTexture, UIQuadStorage>();

        public void AddStorage(SFTexture tex, int quad_count)
        {
            if(storages.ContainsKey(tex))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "UIManager.AddStorage(): There already exists a storage for texture " + tex.GetName() + "!");
                return;
            }

            UIQuadStorage st = new UIQuadStorage();
            st.Init(quad_count);
            storages.Add(tex, st);
        }

        public UIElementIndex AddElementImage(SFTexture tex, Vector2 size, Vector2 origin, Vector2 pos)
        {
            UIElementIndex elem_index;
            elem_index.tex = tex;
            elem_index.span_index = storages[tex].AllocateQuad(size, origin, 0.5f);
            storages[tex].UpdateQuadPosition(elem_index.span_index, pos);
            return elem_index;
        }

        public void MoveElement(UIElementIndex e, Vector2 pos)
        {
            storages[e.tex].UpdateQuadPosition(e.span_index, pos);
        }

        public void SetElementVisible(UIElementIndex e, bool visible)
        {
            storages[e.tex].spans[e.span_index].visible = visible;
        }

        public void Update()
        {
            foreach (UIQuadStorage qs in storages.Values)
                qs.Update();
        }

        public void ForceUpdate()
        {
            foreach (UIQuadStorage qs in storages.Values)
                qs.ForceUpdate();
        }

        public void Dispose()
        {
            foreach(var kv in storages)
            {
                SFResources.SFResourceManager.Textures.Dispose(kv.Key.GetName());
                kv.Value.Dispose();
            }

            storages.Clear();
        }
    }
}
