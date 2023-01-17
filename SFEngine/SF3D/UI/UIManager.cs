/* UIManager is a container for storages, and allows creating UI primitives: text and images
 * UIElementIndex is used as an identifier for individual ui elements
 * */

using OpenTK;
using System.Collections.Generic;

namespace SFEngine.SF3D.UI
{
    public struct UIElementIndex
    {
        public SFTexture tex;
        public int span_index;
    }

    public class UIManager
    {
        public Dictionary<SFTexture, UIQuadStorage> storages { get; private set; } = new Dictionary<SFTexture, UIQuadStorage>();

        // creates new storage with X quads - can only have one storage per texture
        public void AddStorage(SFTexture tex, int quad_count)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "UIManager.AddStorage() called, texture name: " + tex.Name);
            if (storages.ContainsKey(tex))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "UIManager.AddStorage(): There already exists a storage for texture " + tex.Name + "!");
                return;
            }

            UIQuadStorage st = new UIQuadStorage();
            st.Init(quad_count);
            storages.Add(tex, st);
        }

        // deletes storage for given texture
        public void RemoveStorage(SFTexture tex)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "UIManager.RemoveStorage() called, texture name: " + tex.Name);
            storages.Remove(tex);
        }

        // creates a new element which consists of a single image, and sets it immediately
        public UIElementIndex AddElementImage(SFTexture tex, Vector2 size, Vector2 origin, Vector2 pos, bool invert_image)
        {
            UIElementIndex elem_index;
            elem_index.tex = tex;
            elem_index.span_index = storages[tex].ReserveQuads(1);
            storages[tex].AllocateQuad(elem_index.span_index, size, origin, 0.5f, invert_image);
            storages[tex].UpdateSpanPosition(elem_index.span_index, pos);
            return elem_index;
        }

        // creates a new element which consists of multiple images, but doesnt set them immediately
        public UIElementIndex AddElementMulti(SFTexture tex, int count)
        {
            UIElementIndex elem_index;
            elem_index.tex = tex;
            elem_index.span_index = storages[tex].ReserveQuads(count);
            return elem_index;
        }

        // creates a new element which consists of multiple images that correspond to letters of font, and sets max length of the text displaed with this element and its position
        public UIElementIndex AddElementText(UIFont font, int max_length, Vector2 pos)
        {
            UIElementIndex elem_index;
            elem_index.tex = font.font_texture;
            elem_index.span_index = storages[font.font_texture].ReserveQuads(max_length);
            storages[font.font_texture].UpdateSpanPosition(elem_index.span_index, pos);
            return elem_index;
        }

        // sets a single quad of multi element
        public void SetElementMultiQuad(UIElementIndex elem, int quad_index, Vector2 size, Vector2 origin, Vector2 px_start, Vector2 px_end, Vector4 col)
        {
            UIQuadStorage storage = storages[elem.tex];
            storage.SetQuad(elem.span_index, quad_index, size, origin, new Vector2(px_start.X / elem.tex.width, px_start.Y / elem.tex.height), new Vector2(px_end.X / elem.tex.width, px_end.Y / elem.tex.height), 0.5f, col);
        }

        // sets the text of text element
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

        public float GetTextWidth(UIFont font, string text)
        {
            float total_x = 0;
            for (int i = 0; i < text.Length; i++)
            {
                int char_id = (int)text[i];
                total_x += font.character_sizes[char_id].X + font.space_between_letters;
            }

            return total_x;
        }

        public void ClearElementMulti(UIElementIndex elem)
        {
            storages[elem.tex].ResetSpan(elem.span_index);
        }

        public void MoveElement(UIElementIndex e, Vector2 pos)
        {
            storages[e.tex].UpdateSpanPosition(e.span_index, pos);
        }

        public void SetElementVisible(UIElementIndex e, bool visible)
        {
            storages[e.tex].spans[e.span_index].visible = visible;
        }
        public Vector2 GetElementPosition(UIElementIndex e)
        {
            return storages[e.tex].spans[e.span_index].position;
        }
        public bool GetElementVisible(UIElementIndex e)
        {
            return storages[e.tex].spans[e.span_index].visible;
        }

        public void SetImageSize(UIElementIndex e, Vector2 size)
        {
            storages[e.tex].SetQuadPxSize(storages[e.tex].spans[e.span_index].start, size);
        }

        public void UpdateElementGeometry(UIElementIndex e)
        {
            storages[e.tex].UpdateSpanQuads(e.span_index);
        }

        public void UpdateElementAll(UIElementIndex e)
        {
            storages[e.tex].InitSpan(e.span_index);
        }

        public void Update()
        {
            foreach (UIQuadStorage qs in storages.Values)
            {
                qs.Update();
            }
        }

        public void ForceUpdate()
        {
            foreach (UIQuadStorage qs in storages.Values)
            {
                qs.ForceUpdate();
            }
        }

        public void Dispose()
        {
            foreach (var kv in storages)
            {
                kv.Value.Dispose();
            }

            storages.Clear();
        }
    }
}
