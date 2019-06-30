using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapOverlayChunk
    {
        public List<SFCoord> points { get; private set; } = new List<SFCoord>();
        public Vector3[] vertices;
        public uint[] elements;
        public int v_array { get; private set; } = -1;
        int vb_vertices = -1;
        int vb_elements = -1;
        public Vector4 color = new Vector4(1);

        public void Init()
        {
            v_array = GL.GenVertexArray();
            vb_vertices = GL.GenBuffer();
            vb_elements = GL.GenBuffer();

            GL.BindVertexArray(v_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vb_vertices);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindVertexArray(0);
        }

        public void Update(SFMapHeightMapChunk hmap_chunk)
        {
            vertices = new Vector3[points.Count * 5];
            elements = new uint[points.Count * 12];
            uint k = 0;
            foreach(SFCoord p in points)
            {
                int offset = (p.y * hmap_chunk.hmap.chunk_size + p.x) * 6;
                float h = hmap_chunk.vertices[offset].Y;
                // vertices
                vertices[k * 5 + 0] = new Vector3(p.x - 0.2f, h, (hmap_chunk.hmap.chunk_size - p.y - 1) - 0.2f);
                vertices[k * 5 + 1] = new Vector3(p.x + 0.2f, h, (hmap_chunk.hmap.chunk_size - p.y - 1) - 0.2f);
                vertices[k * 5 + 2] = new Vector3(p.x - 0.2f, h, (hmap_chunk.hmap.chunk_size - p.y - 1) + 0.2f);
                vertices[k * 5 + 3] = new Vector3(p.x + 0.2f, h, (hmap_chunk.hmap.chunk_size - p.y - 1) + 0.2f);
                vertices[k * 5 + 4] = new Vector3(p.x, h + 1f, (hmap_chunk.hmap.chunk_size - p.y - 1));
                // elements
                elements[k * 12 + 0] = k * 5 + 0;
                elements[k * 12 + 1] = k * 5 + 1;
                elements[k * 12 + 2] = k * 5 + 4;
                elements[k * 12 + 3] = k * 5 + 1;
                elements[k * 12 + 4] = k * 5 + 3;
                elements[k * 12 + 5] = k * 5 + 4;
                elements[k * 12 + 6] = k * 5 + 3;
                elements[k * 12 + 7] = k * 5 + 2;
                elements[k * 12 + 8] = k * 5 + 4;
                elements[k * 12 + 9] = k * 5 + 2;
                elements[k * 12 + 10] = k * 5 + 0;
                elements[k * 12 + 11] = k * 5 + 4;

                k++;
            }

            GL.BindVertexArray(v_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vb_vertices);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * 12, vertices, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vb_elements);
            GL.BufferData(BufferTarget.ElementArrayBuffer, elements.Length * 4, elements, BufferUsageHint.DynamicDraw);

            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            if(v_array != -1)
            {
                GL.DeleteBuffer(vb_vertices);
                GL.DeleteBuffer(vb_elements);
                GL.DeleteVertexArray(v_array);
                points.Clear();
                v_array = -1;
            }
        }
    }
}
