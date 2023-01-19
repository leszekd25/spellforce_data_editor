using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFEngine.SF3D
{
    public struct MeshCacheRange
    {
        public int Start;    // first natural number of this range
        public int Count;    // amount of sequential numbers for this range (from Start to Start+Count-1)
        public int NextFree { get { return Start + Count; } }  // smallest natural number greater than any of the numbers from this range

        public override string ToString()
        {
            return String.Format("[{0}, {1}]", Start, Count);
        }
    }

    // this collection stores ranges of natural numbers
    // allowed operations will always ensure that the ranges do not overlap, and they will be stored in ascending order
    public class MeshCacheRangeCollection
    {
        public List<MeshCacheRange> Ranges { get; private set; } = new List<MeshCacheRange>();

        public int LastUsed = -1;
        public int FirstUnused = 0;

        public MeshCacheRange this[int key]
        {
            get => Ranges[key];
        }

        public int Count { get { return Ranges.Count; } }

        // adds a new range to the collection
        // the range is put at the first place where it can fit count elements
        // return index of newly created range, and first value of the range
        public int Add(int count, out int first_val)
        {
            int index;
            first_val = FindFirstIndex(count, out index);

            MeshCacheRange val = new MeshCacheRange() { Start = first_val, Count = count };
            if (LastUsed < val.NextFree - 1)
            {
                LastUsed = val.NextFree - 1;
            }

            if (FirstUnused == val.Start)
            {
                FirstUnused = val.NextFree;
            }

            Ranges.Insert(index, val);

            return index;
        }

        public void RemoveAt(int index)
        {
            if (LastUsed == Ranges[index].NextFree - 1)
            {
                LastUsed = Ranges[index].Start;
            }

            if (FirstUnused > Ranges[index].Start)
            {
                FirstUnused = Ranges[index].Start;
            }

            Ranges.RemoveAt(index);
        }

        // finds first index that could fit the next count_needed amount of things starting from this index, without overwriting existing data; also returns the new range index
        // basically, finds a hole big enough to fit N things in it sequentially
        private int FindFirstIndex(int count_needed, out int range_index)
        {
            range_index = 0;
            if (Ranges.Count == 0)
            {
                return 0;
            }

            // invariant: FirstUnused and LastUsed

            // goal: find a hole that could fit N things
            // 1. get last range before first unused
            // 2. find all holes between remaining ranges
            // if there is a hole big enough to fit N things, return first index of that space, and the index of new range
            // 3. if there was no hole, get first index after last range
            int cur_range = 0;
            for (cur_range = 0; cur_range < Ranges.Count; cur_range++)
            {
                if (Ranges[cur_range].Start >= FirstUnused)
                {
                    cur_range -= 1;
                    break;
                }
            }
            if (cur_range == Ranges.Count)
            {
                range_index = cur_range;
                return Ranges[cur_range - 1].NextFree;
            }

            int cur_vertex = 0;
            if (cur_range != -1)
            {
                cur_vertex = Ranges[cur_range].NextFree;
            }

            for (; cur_range < Ranges.Count - 1; cur_range++)
            {
                int dist = Ranges[cur_range + 1].Start - cur_vertex;
                if (dist >= count_needed)
                {
                    range_index = cur_range + 1;
                    return cur_vertex;
                }

                cur_vertex = Ranges[cur_range + 1].NextFree;
            }

            range_index = cur_range + 1;
            return cur_vertex;
        }

        public void Clear()
        {
            Ranges.Clear();

            LastUsed = -1;
            FirstUnused = 0;
        }
    }


    // stores vertex and element ranges as they are in the buffers
    public struct MeshCacheElement
    {
        public int VertexRangeIndex;
        public int ElementRangeIndex;

        public MeshCacheElement(int vri, int eri)
        {
            VertexRangeIndex = vri;
            ElementRangeIndex = eri;
        }
    }

    // description of vertex attribute
    public struct VertexAttribDescription
    {
        public int ComponentCount;
        public VertexAttribPointerType ComponentType;
        public bool Normalized;
    }

    public class MeshCache
    {
        static Dictionary<VertexAttribPointerType, int> VertexAttribTypeSize = new Dictionary<VertexAttribPointerType, int>();

        public int VertexArrayObjectID { get; private set; } = 0;

        int VertexBufferObjectID = 0;
        int BytesPerVertex = 0;
        bool UseCustomBytesPerVertex = false;

        int ElementBufferObjectID = 0;

        int MatrixBufferID = 0;


        public MeshCacheRangeCollection VertexRanges { get; private set; } = new MeshCacheRangeCollection();
        public MeshCacheRangeCollection ElementRanges { get; private set; } = new MeshCacheRangeCollection();

        public LinearPool<MeshCacheElement> Meshes { get; private set; } = new LinearPool<MeshCacheElement>();

        public byte[] VertexBufferObjectData;
        public uint[] ElementBufferObjectData;

        List<VertexAttribDescription> VertexAttributes = new List<VertexAttribDescription>();

        public bool EnableInstancing { get; }
        public Matrix4[] MatrixBufferData;
        public int CurrentMatrix;

        // resizes vertex buffer if necessary
        // note that after this, the buffer has to be fully updated (taken care of in addmesh)
        private void VertexBufferResizeDouble()
        {
            int current_vbo_size = VertexBufferObjectData.Length;

            byte[] NewVBOData = new byte[current_vbo_size * 2];
            Array.Copy(VertexBufferObjectData, NewVBOData, current_vbo_size);
            VertexBufferObjectData = NewVBOData;
        }

        // resizes element buffer if necessary
        // note that after this, the buffer has to be fully updated (taken care of in addmesh)
        private void ElementBufferResizeDouble()
        {
            int current_ebo_size = ElementBufferObjectData.Length;

            uint[] NewEBOData = new uint[current_ebo_size * 2];
            Array.Copy(ElementBufferObjectData, NewEBOData, current_ebo_size);
            ElementBufferObjectData = NewEBOData;
        }

        private void MatrixBufferResizeDouble()
        {
            int current_mb_size = MatrixBufferData.Length;

            Matrix4[] NewMBData = new Matrix4[current_mb_size * 2];
            Array.Copy(MatrixBufferData, NewMBData, current_mb_size);
            MatrixBufferData = NewMBData;
        }


        // this function moves all ranges and shifts data so that all holes are removed
        private void Defragment()
        {
            // defragment vertex range
            int max_index = 0;
            for (int i = 0; i < VertexRanges.Ranges.Count; i++)
            {
                var range = VertexRanges.Ranges[i];
                if (range.Start > max_index)
                {
                    // find size of hole
                    int dist = range.Start - max_index;
                    // move vertex data
                    Array.Copy(VertexBufferObjectData, range.Start * BytesPerVertex, VertexBufferObjectData, max_index * BytesPerVertex, range.Count * BytesPerVertex);
                    VertexRanges.Ranges[i] = new MeshCacheRange() { Start = max_index, Count = range.Count };
                }
                max_index += range.Count;
            }
            VertexRanges.LastUsed = max_index - 1;
            VertexRanges.FirstUnused = max_index;

            // defragment element range
            max_index = 0;
            for (int i = 0; i < ElementRanges.Ranges.Count; i++)
            {
                var range = ElementRanges.Ranges[i];
                if (range.Start > max_index)
                {
                    // find size of hole
                    int dist = range.Start - max_index;
                    // move vertex data
                    Array.Copy(ElementBufferObjectData, range.Start, ElementBufferObjectData, max_index, range.Count);
                    ElementRanges.Ranges[i] = new MeshCacheRange() { Start = max_index, Count = range.Count };
                }
                max_index += range.Count;
            }
            ElementRanges.LastUsed = max_index - 1;
            ElementRanges.FirstUnused = max_index;
        }

        private void InitMatrix(int count)
        {
            MatrixBufferData = new Matrix4[count];
        }

        // definitions of vertex attribute sizes
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

        public MeshCache(bool enable_instancing)
        {
            VertexArrayObjectID = GL.GenVertexArray();
            VertexBufferObjectID = GL.GenBuffer();
            ElementBufferObjectID = GL.GenBuffer();
            EnableInstancing = enable_instancing;
            if (EnableInstancing)
            {
                MatrixBufferID = GL.GenBuffer();
            }
        }

        // adds a vertex attribute
        public void AddVertexAttribute(int component_count, VertexAttribPointerType component_type, bool normalized)
        {
            VertexAttributes.Add(new VertexAttribDescription() { ComponentCount = component_count, ComponentType = component_type, Normalized = normalized });
        }

        // sets vertex byte size (used for custom vertex data)
        public void SetVertexSize(int size)
        {
            UseCustomBytesPerVertex = true;
            BytesPerVertex = size;
        }

        // initializes the cache, using given vertex attributes, and sets up the minimum vertices and elements in the batch
        public void Init(int vertex_count, int element_count)
        {
            GL.BindVertexArray(VertexArrayObjectID);

            // compute total bytes per vertex
            if (!UseCustomBytesPerVertex)
            {
                BytesPerVertex = 0;
                for (int i = 0; i < VertexAttributes.Count; i++)
                {
                    int attrib_bytesize = VertexAttributes[i].ComponentCount * VertexAttribTypeSize[VertexAttributes[i].ComponentType];
                    BytesPerVertex += attrib_bytesize;
                }
            }

            VertexBufferObjectData = new byte[BytesPerVertex * vertex_count];
            ElementBufferObjectData = new uint[element_count];

            FullVertexUpload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObjectID);
            int current_offset = 0;
            for (int i = 0; i < VertexAttributes.Count; i++)
            {
                int attrib_bytesize = VertexAttributes[i].ComponentCount * VertexAttribTypeSize[VertexAttributes[i].ComponentType];

                GL.EnableVertexAttribArray(i);
                GL.VertexAttribPointer(i, VertexAttributes[i].ComponentCount, VertexAttributes[i].ComponentType, VertexAttributes[i].Normalized, BytesPerVertex, current_offset);
                current_offset += attrib_bytesize;
            }

            if (EnableInstancing)
            {
                InitMatrix(20000);
                GL.BindBuffer(BufferTarget.ArrayBuffer, MatrixBufferID);
                GL.BufferData(BufferTarget.ArrayBuffer, 64 * 20000, MatrixBufferData, BufferUsageHint.DynamicDraw);

                for (int i = 0; i < 4; i++)
                {
                    GL.EnableVertexAttribArray(VertexAttributes.Count + i);
                    GL.VertexAttribPointer(VertexAttributes.Count + i, 4, VertexAttribPointerType.Float, false, 64, 16 * i);
                    GL.VertexAttribDivisor(VertexAttributes.Count + i, 1);
                }
            }

            FullElementUpload();
        }

        // clears cache (deletes all meshes, does not update or free buffers)
        public void Clear()
        {
            VertexRanges.Clear();
            ElementRanges.Clear();
            Meshes.Clear();
            //MeshesIndex.Clear();
        }

        // uploads all vertices to the buffer
        public void FullVertexUpload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObjectID);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexBufferObjectData.Length, VertexBufferObjectData, BufferUsageHint.StaticDraw);
        }

        // updates vertices in the buffer
        public void VertexUpload(int vertex_range_index)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObjectID);
            GL.BufferSubData(BufferTarget.ArrayBuffer,
                new IntPtr(VertexRanges[vertex_range_index].Start * BytesPerVertex),
                VertexRanges[vertex_range_index].Count * BytesPerVertex,
                ref VertexBufferObjectData[VertexRanges[vertex_range_index].Start * BytesPerVertex]);
        }

        // uploads all elements in the buffer
        public void FullElementUpload()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObjectID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, ElementBufferObjectData.Length * 4, ElementBufferObjectData, BufferUsageHint.StaticDraw);
        }

        // updates elements in the buffer
        public void ElementUpload(int element_range_indexx)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObjectID);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer,
                new IntPtr(ElementRanges[element_range_indexx].Start * 4),
                ElementRanges[element_range_indexx].Count * 4,
                ref ElementBufferObjectData[ElementRanges[element_range_indexx].Start]);
        }

        public void MatrixUpload()
        {
            if (!EnableInstancing)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, MatrixBufferID);
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(0), 64 * CurrentMatrix, MatrixBufferData);
        }

        public void ResizeInstanceMatrixBuffer(int m_count)
        {
            if (m_count <= MatrixBufferData.Length)
            {
                return;
            }

            int current_mbo_size = MatrixBufferData.Length;

            MatrixBufferData = new Matrix4[current_mbo_size * 2];

            GL.BindBuffer(BufferTarget.ArrayBuffer, MatrixBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, MatrixBufferData.Length * 64, MatrixBufferData, BufferUsageHint.DynamicDraw);
        }


        // adds mesh to the cache, returns index to the mesh index table, and that index directs to mesh data
        public int AddMesh(byte[] vertex_data, int vertex_data_offset, int vertex_data_count, uint[] element_data)
        {
            int vertex_count = vertex_data_count  / BytesPerVertex;

            int vertex_offset;
            int element_offset;

            int vertex_range_index = VertexRanges.Add(vertex_count, out vertex_offset);
            int element_range_index = ElementRanges.Add(element_data.Length, out element_offset);

            for (int i = 0; i < Meshes.elements.Count; i++)
            {
                if (!Meshes.elem_active[i])
                {
                    continue;
                }

                MeshCacheElement em = Meshes.elements[i];
                if (em.VertexRangeIndex >= vertex_range_index)
                {
                    em.VertexRangeIndex++;
                }

                if (em.ElementRangeIndex >= element_range_index)
                {
                    em.ElementRangeIndex++;
                }

                Meshes.elements[i] = em;
            }

            int mesh_index = Meshes.Add(new MeshCacheElement() { VertexRangeIndex = vertex_range_index, ElementRangeIndex = element_range_index });

            bool do_full_vertex_reload = false;
            bool do_full_element_reload = false;

            while (true)
            {
                if ((vertex_offset + vertex_count) * BytesPerVertex <= VertexBufferObjectData.Length)
                {
                    break;
                }

                VertexBufferResizeDouble();
                do_full_vertex_reload = true;
            }

            while (true)
            {
                if (element_offset + element_data.Length <= ElementBufferObjectData.Length)
                {
                    break;
                }

                ElementBufferResizeDouble();
                do_full_element_reload = true;
            }

            Array.Copy(vertex_data, vertex_data_offset, VertexBufferObjectData, vertex_offset * BytesPerVertex, vertex_data_count);
            Array.Copy(element_data, 0, ElementBufferObjectData, element_offset, element_data.Length);

            // defragment if need be
            if ((VertexRanges.LastUsed - VertexRanges.FirstUnused > VertexRanges.FirstUnused / 2)
                || (ElementRanges.LastUsed - ElementRanges.FirstUnused > ElementRanges.FirstUnused / 2))
            {
                Defragment();
                do_full_vertex_reload = true;
                do_full_element_reload = true;
            }

            if (do_full_vertex_reload)
            {
                FullVertexUpload();
            }
            else
            {
                VertexUpload(vertex_range_index);
            }

            if (do_full_element_reload)
            {
                FullElementUpload();
            }
            else
            {
                ElementUpload(element_range_index);
            }

            //return mesh_index_index;
            return mesh_index;
        }

        // removes mesh from the cache
        // with correct usage, meshes that were deleted with this function will not be further referenced with this index
        public void RemoveMesh(int mesh_index)//_index)
        {
            if (mesh_index >= Meshes.elements.Count)
            {
                return;
            }

            if (!Meshes.elem_active[mesh_index])
            {
                return;
            }

            int vertex_range_index = Meshes.elements[mesh_index].VertexRangeIndex;
            if (vertex_range_index >= VertexRanges.Count)
            {
                return;
            }

            int element_range_index = Meshes.elements[mesh_index].ElementRangeIndex;
            if (element_range_index >= ElementRanges.Count)
            {
                return;
            }

            // 1. delete vertex and element ranges
            VertexRanges.RemoveAt(vertex_range_index);
            ElementRanges.RemoveAt(element_range_index);

            for (int i = 0; i < Meshes.elements.Count; i++)
            {
                if (!Meshes.elem_active[i])
                {
                    continue;
                }

                MeshCacheElement em = Meshes.elements[i];
                if (em.VertexRangeIndex > vertex_range_index)
                {
                    em.VertexRangeIndex--;
                }

                if (em.ElementRangeIndex > element_range_index)
                {
                    em.ElementRangeIndex--;
                }

                Meshes.elements[i] = em;
            }
            // 2. delete mesh from the mesh list
            Meshes.RemoveAt(mesh_index);
        }

        public void Dispose()
        {
            if (VertexArrayObjectID != 0)
            {
                if (EnableInstancing)
                {
                    GL.DeleteBuffer(MatrixBufferID);
                    MatrixBufferData = null;
                }

                GL.DeleteBuffer(ElementBufferObjectID);
                GL.DeleteBuffer(VertexBufferObjectID);
                GL.DeleteVertexArray(VertexArrayObjectID);

                VertexArrayObjectID = 0;
                VertexBufferObjectData = null;
                ElementBufferObjectData = null;
            }
        }

        public void LogMemoryUsage()
        {
            StringBuilder log_msg = new StringBuilder();

            log_msg.Append("Cache memory usage (bytes, RAM): ");
            log_msg.Append("VERTEX BUFFER " + (VertexBufferObjectData != null? VertexBufferObjectData.Length: 0).ToString());
            log_msg.Append(", INDEX BUFFER " + (ElementBufferObjectData != null ? ElementBufferObjectData.Length * 4 : 0).ToString());
            log_msg.Append(", INSTANCE BUFFER " + ((EnableInstancing && MatrixBufferData != null) ? (MatrixBufferData.Length * 64) : (0)).ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, log_msg.ToString());
            log_msg.Clear();

            log_msg.Append("Cache memory usage (bytes, device): ");
            log_msg.Append("VERTEX BUFFER " + (VertexBufferObjectData != null ? VertexBufferObjectData.Length : 0).ToString());
            log_msg.Append(", INDEX BUFFER " + (ElementBufferObjectData != null ? ElementBufferObjectData.Length * 4 : 0).ToString());
            log_msg.Append(", INSTANCE BUFFER " + ((EnableInstancing && MatrixBufferData != null) ? (MatrixBufferData.Length * 64) : (0)).ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, log_msg.ToString());
            log_msg.Clear();
        }

        /* usage:
         * 
         * - initialization
         * MeshCache cache = new MeshCache();
         * cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // positions
         * cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // normals
         * cache.AddVertexAttribute(2, VertexAttribPointerType.Float, false);   // UVs
         * cache.AddVertexAttribute(4, VertexAttribPointerType.Float, false);   // colors
         * cache.Init(2 << 16, 2 << 16);
         * 
         * - adding a mesh
         * ind = cache.AddMesh(vertex_data, element_data);
         * 
         * - getting mesh data
         * cache.Meshes[ind].<data>
         * 
         * - closing
         * cache.Dispose();
        */
    }
}
