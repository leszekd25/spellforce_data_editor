using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.SF3D.UI
{
    public class UIQuadStorageSpan
    {
        public int start, reserved, used;
        public bool visible;
        public Vector2 position;
    }

    // one per texture
    public class UIQuadStorage
    {
        public List<UIQuadStorageSpan> spans { get; private set; } = new List<UIQuadStorageSpan>();
        public Vector3[] vertices;
        Vector2[] uvs;

        public int vertex_array = -1;
        public int vertex_buffer, uv_buffer;

        Vector2[] pxsizes;
        Vector2[] origins;
        float[] depths;
        Vector2[] uvs_start;
        Vector2[] uvs_end;

        int cur_quad = 0;
        public bool updated { get; private set; } = false;

        public void Init(int quad_count)
        {
            vertices = new Vector3[quad_count * 6];
            uvs = new Vector2[quad_count * 6];

            pxsizes = new Vector2[quad_count];
            origins = new Vector2[quad_count];
            depths = new float[quad_count];
            uvs_start = new Vector2[quad_count];
            uvs_end = new Vector2[quad_count];

            vertex_array = GL.GenVertexArray();
            vertex_buffer = GL.GenBuffer();
            uv_buffer = GL.GenBuffer();

            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * 12, vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, uv_buffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, uvs.Length * 8, uvs, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindVertexArray(0);
        }

        public int ReserveQuads(int quad_count)
        {
            spans.Add(new UIQuadStorageSpan() { start = cur_quad, reserved = quad_count, used = 0, visible = true, position = new Vector2(0, 0) });

            cur_quad += quad_count;

            return spans.Count - 1;
        }

        // for images
        public void AllocateQuad(int span_index, Vector2 pxsize, Vector2 origin, float depth, bool invert_uv)
        {
            UIQuadStorageSpan span = spans[span_index];
            pxsizes[span.start] = pxsize;
            origins[span.start] = origin;
            depths[span.start] = depth;
            if (invert_uv)
            {
                uvs_start[span.start] = new Vector2(0, 1);
                uvs_end[span.start] = new Vector2(1, 0);
            }
            else
            {
                uvs_start[span.start] = new Vector2(0, 0);
                uvs_end[span.start] = new Vector2(1, 1);
            }

            spans[span_index] = new UIQuadStorageSpan { start = span.start, reserved = span.reserved, used = 1, position = span.position, visible = span.visible };
            InitSpan(span_index);
        }

        // for text
        public void AllocateQuadsUV(int span_index, Vector2[] _pxsizes, Vector2[] _origins, Vector2[] _uvs_start, Vector2[] _uvs_end, float depth = 0)
        {
            UIQuadStorageSpan span = spans[span_index];
            Array.Copy(_pxsizes, 0, pxsizes, span.start, _pxsizes.Length);
            Array.Copy(_origins, 0, origins, span.start, _origins.Length);
            Array.Copy(_uvs_start, 0, uvs_start, span.start, _uvs_start.Length);
            Array.Copy(_uvs_end, 0, uvs_end, span.start, _uvs_end.Length);
            for (int i = 0; i < _pxsizes.Length; i++)
                depths[span.start + i] = depth;

            spans[span_index] = new UIQuadStorageSpan { start = span.start, reserved = span.reserved, used = _pxsizes.Length, position = span.position, visible = span.visible };
            InitSpan(span_index);
        }

        public void UpdateSpanPosition(int span_index, Vector2 pos)
        {
            spans[span_index].position = pos;
        }

        public void UpdateSpanQuads(int span_index)
        {
            for (int i = 0; i < spans[span_index].used; i++)
            {
                int offset = spans[span_index].start + i;
                vertices[offset * 6 + 0] = new Vector3(
                    -origins[offset].X,
                    -origins[offset].Y,
                    depths[offset]);
                vertices[offset * 6 + 1] = new Vector3(
                    pxsizes[offset].X - origins[offset].X,
                    vertices[offset * 6 + 0].Y,
                    depths[offset]);
                vertices[offset * 6 + 2] = new Vector3(
                    vertices[offset * 6 + 0].X,
                    pxsizes[offset].Y - origins[offset].Y,
                    depths[offset]);
                vertices[offset * 6 + 3] = vertices[offset * 6 + 1];
                vertices[offset * 6 + 4] = vertices[offset * 6 + 2];
                vertices[offset * 6 + 5] = new Vector3(
                    vertices[offset * 6 + 1].X,
                    vertices[offset * 6 + 2].Y,
                    depths[offset]);
            }
        }

        public void InitSpan(int span_index)
        {
            UpdateSpanQuads(span_index);

            for (int i = 0; i < spans[span_index].used; i++)
            {
                int offset = spans[span_index].start + i;

                uvs[offset * 6 + 0] = new Vector2(uvs_start[offset].X, uvs_start[offset].Y);
                uvs[offset * 6 + 1] = new Vector2(uvs_end[offset].X, uvs_start[offset].Y);
                uvs[offset * 6 + 2] = new Vector2(uvs_start[offset].X, uvs_end[offset].Y);
                uvs[offset * 6 + 3] = new Vector2(uvs_end[offset].X, uvs_start[offset].Y);
                uvs[offset * 6 + 4] = new Vector2(uvs_start[offset].X, uvs_end[offset].Y);
                uvs[offset * 6 + 5] = new Vector2(uvs_end[offset].X, uvs_end[offset].Y);
            }

            updated = false;
        }

        public void SetQuadPxSize(int index, Vector2 size)
        {
            pxsizes[index] = size;
            updated = false;
        }

        public void ForceUpdate()
        {
            for(int s = 0; s < spans.Count; s++)
                UpdateSpanQuads(s);

            updated = false;
        }

        // for now entire buffer is updated
        public void Update()
        {
            if (updated)
                return;

            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(0), vertices.Length * 12, vertices);

            GL.BindBuffer(BufferTarget.ArrayBuffer, uv_buffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(0), uvs.Length * 8, uvs);

            GL.BindVertexArray(0);

            updated = true;
        }

        public void Dispose()
        {
            if (vertex_array != -1)
            {
                GL.DeleteBuffer(vertex_buffer);
                GL.DeleteBuffer(uv_buffer);
                GL.DeleteVertexArray(vertex_array);
                vertex_array = -1;
            }
        }
    }
}
