using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.SF3D.SFRender
{
    // describes mesh position in the buffer
    public struct MeshCacheElement
    {
        public int VertexStart;     // index of first vertex of this mesh fragment in the buffer
        public int VertexCount;     // vertices are laid out continuously in the buffer
        public int VertexNextFree { get { return VertexStart + VertexCount; } }

        public int ElementStart;    // index of first vertex index of this mesh fragment in the index buffer
        public int ElementCount;    // likewise, vertex indices are laid out continuously in the index buffer
        public int ElementNextFree { get { return ElementStart + ElementCount; } }
    }

    public struct VertexAttribDescription
    {
        public int ComponentCount;
        public VertexAttribPointerType ComponentType;
        public bool Normalized;
    }

    public class MeshCache
    {
        static Dictionary<VertexAttribPointerType, int> VertexAttribTypeSize = new Dictionary<VertexAttribPointerType, int>();

        int VertexArrayObjectID = 0;

        int VertexBufferObjectID = 0;
        int BytesPerVertex = 0;
        int LastVertexUsed = -1;
        int FirstVertexUnused = 0;

        int ElementBufferObjectID = 0;
        int LastElementUsed = -1;
        int FirstElementUnused = 0;

        List<MeshCacheElement> Meshes = new List<MeshCacheElement>();

        byte[] VertexBufferObjectData;
        uint[] ElementBufferObjectData;

        List<VertexAttribDescription> VertexAttributes = new List<VertexAttribDescription>();

        private void VertexBufferResizeDouble()
        {
            int current_vbo_size = VertexBufferObjectData.Length;

            byte[] NewVBOData = new byte[current_vbo_size * 2];
            Array.Copy(VertexBufferObjectData, NewVBOData, current_vbo_size);
            VertexBufferObjectData = NewVBOData;
        }

        private void ElementBufferResizeDouble()
        {
            int current_ebo_size = ElementBufferObjectData.Length;

            uint[] NewEBOData = new uint[current_ebo_size * 2];
            Array.Copy(ElementBufferObjectData, NewEBOData, current_ebo_size);
            ElementBufferObjectData = NewEBOData;
        }

        private int FindFirstVertexIndex(int vertex_count_needed)
        {
            // goal: find empty space between meshes that could fit X vertices
            // 1. get last mesh before first unused vertex
            // 2. find all empty spaces between remaining meshes
            // if there is a free space big enough to fit X vertices, return first vertex of that space
            // 3. if there was no space, get first vertex after last mesh
            int cur_mesh = 0;
            for (cur_mesh = 0; cur_mesh < Meshes.Count; cur_mesh++)
            {
                if (Meshes[cur_mesh].VertexStart >= FirstVertexUnused)
                {
                    cur_mesh -= 1;
                    break;
                }
            }
            if (cur_mesh == Meshes.Count)
                return Meshes[cur_mesh - 1].VertexNextFree;

            int cur_vertex = 0;
            if (cur_mesh != -1)
                cur_vertex = Meshes[cur_mesh].VertexNextFree;

            for (; cur_mesh < Meshes.Count - 1; cur_mesh++)
            {
                int dist = Meshes[cur_mesh + 1].VertexStart - cur_vertex;
                if (dist >= vertex_count_needed)
                    return cur_vertex;

                cur_vertex = Meshes[cur_mesh + 1].VertexNextFree;
            }

            return cur_vertex;
        }

        private int FindFirstElementIndex(int element_count_needed)
        {
            int cur_mesh = 0;
            for (cur_mesh = 0; cur_mesh < Meshes.Count; cur_mesh++)
            {
                if (Meshes[cur_mesh].ElementStart >= FirstElementUnused)
                {
                    cur_mesh -= 1;
                    break;
                }
            }
            if (cur_mesh == Meshes.Count)
                return Meshes[cur_mesh - 1].ElementNextFree;

            int cur_element = 0;
            if (cur_mesh != -1)
                cur_element = Meshes[cur_mesh].ElementNextFree;

            for (; cur_mesh < Meshes.Count - 1; cur_mesh++)
            {
                int dist = Meshes[cur_mesh + 1].ElementStart - cur_element;
                if (dist >= element_count_needed)
                    return cur_element;

                cur_element = Meshes[cur_mesh + 1].ElementNextFree;
            }

            return cur_element;
        }

        static MeshCache()
        {
            VertexAttribTypeSize.Add(VertexAttribPointerType.Byte, 1);
            VertexAttribTypeSize.Add(VertexAttribPointerType.Double, 2);
            VertexAttribTypeSize.Add(VertexAttribPointerType.Fixed, 4);
            VertexAttribTypeSize.Add(VertexAttribPointerType.Float, 4);
            VertexAttribTypeSize.Add(VertexAttribPointerType.HalfFloat, 2);
            VertexAttribTypeSize.Add(VertexAttribPointerType.Int, 4);
            VertexAttribTypeSize.Add(VertexAttribPointerType.Int2101010Rev, 4);
            VertexAttribTypeSize.Add(VertexAttribPointerType.Short, 2);
            VertexAttribTypeSize.Add(VertexAttribPointerType.UnsignedByte, 1);
            VertexAttribTypeSize.Add(VertexAttribPointerType.UnsignedInt, 4);
            VertexAttribTypeSize.Add(VertexAttribPointerType.UnsignedInt10F11F11FRev, 4);
            VertexAttribTypeSize.Add(VertexAttribPointerType.UnsignedInt2101010Rev, 4);
            VertexAttribTypeSize.Add(VertexAttribPointerType.UnsignedShort, 2);
        }

        public MeshCache()
        {
            VertexArrayObjectID = GL.GenVertexArray();
            VertexBufferObjectID = GL.GenBuffer();
            ElementBufferObjectID = GL.GenBuffer();
        }

        public void AddVertexAttribute(int component_count, VertexAttribPointerType component_type, bool normalized)
        {
            VertexAttributes.Add(new VertexAttribDescription() { ComponentCount = component_count, ComponentType = component_type, Normalized = normalized });
        }

        public void Init(int vertex_count, int element_count)
        {
            GL.BindVertexArray(VertexArrayObjectID);

            // compute total bytes per vertex
            for (int i = 0; i < VertexAttributes.Count; i++)
            {
                int attrib_bytesize = VertexAttributes[i].ComponentCount * VertexAttribTypeSize[VertexAttributes[i].ComponentType];
                BytesPerVertex += attrib_bytesize;
            }

            for (int i = 0; i < VertexAttributes.Count; i++)
            {
                int attrib_bytesize = VertexAttributes[i].ComponentCount * VertexAttribTypeSize[VertexAttributes[i].ComponentType];

                GL.EnableVertexAttribArray(i);
                GL.VertexAttribPointer(i, VertexAttributes[i].ComponentCount, VertexAttributes[i].ComponentType, VertexAttributes[i].Normalized, BytesPerVertex, attrib_bytesize);
            }

            VertexBufferObjectData = new byte[BytesPerVertex * vertex_count];
            ElementBufferObjectData = new uint[element_count];

            FullVertexUpload();
            FullElementUpload();
        }

        public void FullVertexUpload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObjectID);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexBufferObjectData.Length, VertexBufferObjectData, BufferUsageHint.StaticDraw);
        }

        public void VertexUpload(int mesh_index)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObjectID);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 
                new IntPtr(Meshes[mesh_index].VertexStart * BytesPerVertex),
                Meshes[mesh_index].VertexCount * BytesPerVertex,
                ref VertexBufferObjectData[Meshes[mesh_index].VertexStart * BytesPerVertex]);
        }

        public void FullElementUpload()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObjectID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, ElementBufferObjectData.Length * 4, ElementBufferObjectData, BufferUsageHint.StaticDraw);
        }

        public void ElementUpload(int mesh_index)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObjectID);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer,
                new IntPtr(Meshes[mesh_index].ElementStart * 4),
                Meshes[mesh_index].ElementCount * 4,
                ref ElementBufferObjectData[Meshes[mesh_index].ElementStart]);
        }


        public int AddMesh(byte[] vertex_data, uint[] element_data)
        {
            int vertex_count = vertex_data.Length / BytesPerVertex;
            int vertex_offset = FindFirstVertexIndex(vertex_count);
            int element_offset = FindFirstElementIndex(element_data.Length);

            Meshes.Add(new MeshCacheElement() { VertexStart = vertex_offset, VertexCount = vertex_count, ElementStart = element_offset, ElementCount = element_data.Length });
            int mesh_index = Meshes.Count - 1;

            bool do_full_vertex_reload = false;
            bool do_full_element_reload = false;
            while (true)
            {
                if ((vertex_offset + vertex_count) * BytesPerVertex <= VertexBufferObjectData.Length)
                    break;

                VertexBufferResizeDouble();
                do_full_vertex_reload = true;
            }

            while (true)
            {
                if (element_offset + element_data.Length <= ElementBufferObjectData.Length)
                    break;

                ElementBufferResizeDouble();
                do_full_element_reload = true;
            }

            Array.Copy(vertex_data, 0, VertexBufferObjectData, vertex_offset * BytesPerVertex, vertex_count * BytesPerVertex);
            Array.Copy(element_data, 0, ElementBufferObjectData, element_offset, element_data.Length);

            if (do_full_vertex_reload)
                FullVertexUpload();
            else
                VertexUpload(mesh_index);

            if (do_full_element_reload)
                FullElementUpload();
            else
                VertexUpload(mesh_index);

            return mesh_index;
        }
    }
}
