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
            elem_index.span_index = storages[tex].ReserveQuads(1);
            storages[tex].AllocateQuad(elem_index.span_index, size, origin, 0.5f);
            storages[tex].UpdateSpanPosition(elem_index.span_index, pos);
            return elem_index;
        }

        public UIElementIndex AddElementText(UIFont font, int max_length, Vector2 pos)
        {
            UIElementIndex elem_index;
            elem_index.tex = font.font_texture;
            elem_index.span_index = storages[font.font_texture].ReserveQuads(max_length);
            storages[font.font_texture].UpdateSpanPosition(elem_index.span_index, pos);
            return elem_index;
        }

        public void SetElementText(UIElementIndex elem, UIFont font, string text)
        {
            // generate text quads
            Vector2[] _pxsizes = new Vector2[text.Length];
            Vector2[] _origins = new Vector2[text.Length];
            Vector2[] _uvs_start = new Vector2[text.Length];
            Vector2[] _uvs_end = new Vector2[text.Length];

            float total_x = 0;
            for (int i = 0; i < text.Length; i++)
            {
                int char_id = (int)text[i];
                _pxsizes[i] = font.character_vertex_sizes[char_id];
                _origins[i] = font.character_offsets[char_id] - new Vector2(total_x, 0);
                _uvs_start[i] = font.character_uvs_start[char_id];
                _uvs_end[i] = font.character_uvs_end[char_id];
                total_x += font.character_sizes[char_id].X + font.space_between_letters;
            }

            storages[elem.tex].AllocateQuadsUV(elem.span_index, _pxsizes, _origins, _uvs_start, _uvs_end, 0.5f);
        }

        public void MoveElement(UIElementIndex e, Vector2 pos)
        {
            storages[e.tex].UpdateSpanPosition(e.span_index, pos);
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
