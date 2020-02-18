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
        public int start, count;
        public bool visible;
    }

    // one per texture
    public class UIQuadStorage
    {
        public List<UIQuadStorageSpan> spans { get; private set; } = new List<UIQuadStorageSpan>();
        Vector3[] vertices;
        Vector2[] uvs;

        public int vertex_array = -1;
        public int vertex_buffer, uv_buffer;

        Vector2[] pxsizes;
        Vector2[] origins;
        Vector2[] positions;
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
            positions = new Vector2[quad_count];
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

        // for images
        public int AllocateQuad(Vector2 pxsize, Vector2 origin, float depth = 0)
        {
            pxsizes[cur_quad] = pxsize;
            origins[cur_quad] = origin;
            positions[cur_quad] = new Vector2(0, 0);
            depths[cur_quad] = depth;
            uvs_start[cur_quad] = new Vector2(0, 0);
            uvs_end[cur_quad] = new Vector2(1, 1);
            InitQuad(cur_quad);

            spans.Add(new UIQuadStorageSpan() { start = cur_quad, count = 1, visible = true });
            cur_quad += 1;

            return cur_quad - 1;
        }
        // for text
        public void AllocateQuadsUV(Vector2[] _pxiszes, Vector2[] _origins, Vector2[] _uvs_start, Vector2[] _uvs_end)
        {

        }

        public void UpdateQuadPosition(int span_index, Vector2 pos)
        {
            positions[span_index] = pos;

            UpdateQuadVertices(span_index);
            updated = false;
        }

        public void UpdateQuadVertices(int span_index)
        {
            vertices[span_index * 6 + 0] = new Vector3(
                -origins[span_index].X + positions[span_index].X - SFRender.SFRenderEngine.render_size.X/2,
                -origins[span_index].Y + positions[span_index].Y - SFRender.SFRenderEngine.render_size.X / 2,
                depths[span_index]);
            vertices[span_index * 6 + 1] = new Vector3(
                pxsizes[span_index].X - origins[span_index].X + positions[span_index].X - SFRender.SFRenderEngine.render_size.X / 2,
                vertices[span_index * 6 + 0].Y,
                depths[span_index]);
            vertices[span_index * 6 + 2] = new Vector3(
                vertices[span_index * 4 + 0].X,
                pxsizes[span_index].Y - origins[span_index].Y + positions[span_index].Y - SFRender.SFRenderEngine.render_size.X / 2,
                depths[span_index]);
            vertices[span_index * 6 + 3] = vertices[span_index * 6 + 1];
            vertices[span_index * 6 + 4] = vertices[span_index * 6 + 2];
            vertices[span_index * 6 + 5] = new Vector3(
                vertices[span_index * 6 + 1].X,
                vertices[span_index * 6 + 2].Y,
                depths[span_index]);
        }

        public void InitQuad(int span_index)
        {
            UpdateQuadVertices(span_index);

            uvs[span_index * 6 + 0] = new Vector2(0, 0);
            uvs[span_index * 6 + 1] = new Vector2(1, 0);
            uvs[span_index * 6 + 2] = new Vector2(0, 1);
            uvs[span_index * 6 + 3] = new Vector2(1, 0);
            uvs[span_index * 6 + 4] = new Vector2(0, 1);
            uvs[span_index * 6 + 5] = new Vector2(1, 1);

            updated = false;
        }

        public void ForceUpdate()
        {
            for(int s = 0; s < spans.Count; s++)
                UpdateQuadVertices(s);

            updated = false;
        }

        public void Update()
        {
            if (updated)
                return;

            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * 12, vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, uv_buffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, uvs.Length * 8, uvs, BufferUsageHint.StaticDraw);

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
