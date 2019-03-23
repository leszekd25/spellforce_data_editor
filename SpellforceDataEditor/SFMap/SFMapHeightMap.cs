using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpellforceDataEditor.SF3D;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapHeightMapChunk
    {
        public SFMapHeightMap hmap = null;
        public int width, height;
        public int id, ix, iy;
        
        public byte[] material_id;
        public Vector3[] vertices;
        public Vector4[] colors;
        public Vector2[] uvs;
        public Vector3[] texture_id;
        public Vector3[] texture_weights;

        public SF3D.Physics.BoundingBox aabb;
        public List<SFMapObject> objects = new List<SFMapObject>();
        public List<SFMapUnit> units = new List<SFMapUnit>();
        public List<SFMapDecoration> decorations = new List<SFMapDecoration>();
        public List<bool> lakes_contained = new List<bool>();
        private bool visible = false;              // if false, units and objects in the chunk are not rendered
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                SF3D.SceneSynchro.SFSceneManager scene = hmap.map.render_engine.scene_manager;
                foreach (SFMapUnit u in units)
                    scene.objects_static[u.GetObjectName()].Visible = value;
                foreach (SFMapObject o in objects)
                    scene.objects_static[o.GetObjectName()].Visible = value;
                foreach (SFMapDecoration d in decorations)
                    scene.objects_static[d.GetObjectName()].Visible = value;
            }
        }

        public int vertex_array, position_buffer, color_buffer, uv_buffer, tex_id_buffer, tex_weight_buffer;

        public void Generate(short[] data, byte[] tex_data, int map_size, int size, int x, int y)
        {
            float flatten_factor = 100;

            width = size;
            height = size;
            ix = x;
            iy = y;
            id = y * (map_size / size) + x;

            int chunk_count = map_size / size;

            int row_start = (chunk_count - y - 1) * size;
            int col_start = x * size;

            material_id = new byte[(size + 1) * (size + 1)];

            vertices = new OpenTK.Vector3[size * size * 6];
            colors = new OpenTK.Vector4[size * size * 6];
            uvs = new OpenTK.Vector2[size * size * 6];
            texture_id = new Vector3[size * size * 6];
            texture_weights = new Vector3[size * size * 6];

            // precalculate material data
            for (int i = 0; i <= size; i++)
            {
                for (int j = 0; j <= size; j++)
                {
                    SFCoord p = new SFCoord(col_start + j, row_start + i);
                    if (p.InRect(0, 0, map_size - 1, map_size - 1))
                        material_id[i * (size + 1) + j] = tex_data[(row_start + i) * map_size + col_start + j];
                    else
                        material_id[i * (size + 1) + j] = 0;
                }
            }

            // todo: roll into some nice loops (though is it really necessary?)
            // generate mesh data
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int t = (i * size + j) * 6;
                    Vector3 triangle_mats;
                    Vector3 triangle_ws;

                    if ((i + j) % 2 == 0)
                    {
                        // left triangle
                        vertices[t + 0] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 1] = new Vector3((float)j + 1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);
                        vertices[t + 2] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        triangle_mats = new Vector3(material_id[i * (size + 1) + j], material_id[(i + 1) * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j]);
                        triangle_mats = SortV3(triangle_mats);
                        texture_id[t + 0] = triangle_mats;
                        texture_id[t + 1] = triangle_mats;
                        texture_id[t + 2] = triangle_mats;

                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[i * (size + 1) + j])] = 1.0f;
                        texture_weights[t + 0] = triangle_ws;
                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[(i + 1) * (size + 1) + (j + 1)])] = 1.0f;
                        texture_weights[t + 1] = triangle_ws;
                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[(i + 1) * (size + 1) + j])] = 1.0f;
                        texture_weights[t + 2] = triangle_ws;

                        // right triangle
                        vertices[t + 3] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 4] = new Vector3((float)j + 1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 5] = new Vector3((float)j + 1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        triangle_mats = new Vector3(material_id[i * (size + 1) + j], material_id[i * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j + 1]);
                        triangle_mats = SortV3(triangle_mats);
                        texture_id[t + 3] = triangle_mats;
                        texture_id[t + 4] = triangle_mats;
                        texture_id[t + 5] = triangle_mats;

                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[i * (size + 1) + j])] = 1.0f;
                        texture_weights[t + 3] = triangle_ws;
                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[i * (size + 1) + (j + 1)])] = 1.0f;
                        texture_weights[t + 4] = triangle_ws;
                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[(i + 1) * (size + 1) + (j + 1)])] = 1.0f;
                        texture_weights[t + 5] = triangle_ws;
                    }
                    else
                    {
                        // left triangle
                        vertices[t + 0] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 1] = new Vector3((float)j + 1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 2] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        triangle_mats = new Vector3(material_id[i * (size + 1) + j], material_id[i * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j]);
                        triangle_mats = SortV3(triangle_mats);
                        texture_id[t + 0] = triangle_mats;
                        texture_id[t + 1] = triangle_mats;
                        texture_id[t + 2] = triangle_mats;

                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[i * (size + 1) + j])] = 1.0f;
                        texture_weights[t + 0] = triangle_ws;
                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[i * (size + 1) + (j + 1)])] = 1.0f;
                        texture_weights[t + 1] = triangle_ws;
                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[(i + 1) * (size + 1) + j])] = 1.0f;
                        texture_weights[t + 2] = triangle_ws;
                        // right triangle
                        vertices[t + 3] = new Vector3((float)j + 1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 4] = new Vector3((float)j + 1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);
                        vertices[t + 5] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        triangle_mats = new Vector3(material_id[i * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j]);
                        triangle_mats = SortV3(triangle_mats);
                        texture_id[t + 3] = triangle_mats;
                        texture_id[t + 4] = triangle_mats;
                        texture_id[t + 5] = triangle_mats;

                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[i * (size + 1) + (j + 1)])] = 1.0f;
                        texture_weights[t + 3] = triangle_ws;
                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[(i + 1) * (size + 1) + (j + 1)])] = 1.0f;
                        texture_weights[t + 4] = triangle_ws;
                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[(i + 1) * (size + 1) + j])] = 1.0f;
                        texture_weights[t + 5] = triangle_ws;
                    }
                }
            }

            float max_height = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                // color (debug heightmap)
                float h = vertices[i].Y;
                byte col = (byte)(Math.Min(255, (int)(h * 8)));
                float norm_color = (float)col / 255.0f;
                colors[i] = new OpenTK.Vector4(norm_color, norm_color, norm_color, 1.0f);
                uvs[i] = vertices[i].Xz / 4;
                max_height = Math.Max(max_height, h);
            }

            aabb = new SF3D.Physics.BoundingBox(new Vector3(x * size, 0, y * size), new Vector3((x + 1) * size, max_height, (y * size) + size));
        }

        public void TransformMaterialsToTextures(SFMapTerrainTextureManager man)
        {
            for (int i = 0; i < (width * width * 6); i++)
            {
                for (int j = 0; j < 3; j++)
                    texture_id[i][j] = (float)man.texture_reindex[(int)texture_id[i][j]].real_index;
            }
        }

        public void Init()
        {
            vertex_array = GL.GenVertexArray();
            position_buffer = GL.GenBuffer();
            uv_buffer = GL.GenBuffer();
            color_buffer = GL.GenBuffer();
            tex_id_buffer = GL.GenBuffer();
            tex_weight_buffer = GL.GenBuffer();

            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, position_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * 12, vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, color_buffer);
            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, colors.Length * 16, colors, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, uv_buffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, uvs.Length * 8, uvs, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, tex_id_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, texture_id.Length * 12, texture_id, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, tex_weight_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, texture_weights.Length * 12, texture_weights, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindVertexArray(0);
        }

        private short GetHeightAt(short[] data, int size, int x, int y)
        {
            int pos = (y * size) + x;
            if ((pos < 0) || (pos >= data.Length))
                return 0;
            return data[pos];
        }

        // used for sorting materials
        private Vector3 SortV3(Vector3 r)
        {
            Vector3 v = new Vector3();
            int max_id = 0, min_id = 0, mid_id = 0;

            float t = r[0];
            if (r[1] > t)
            {
                t = r[1];
                max_id = 1;
            }
            if (r[2] > t)
                max_id = 2;

            t = r[0];
            if (r[1] < t)
            {
                t = r[1];
                min_id = 1;
            }
            if (r[2] < t)
                min_id = 2;

            if (max_id == min_id)
                mid_id = max_id;
            else
                mid_id = 3 - max_id - min_id;

            v[0] = r[min_id];
            v[1] = r[mid_id];
            v[2] = r[max_id];

            return v;
        }

        private int GetIndex(Vector3 v, int id)
        {
            for (int i = 0; i < 3; i++)
                if ((int)v[i] == id)
                    return i;
            return -1;
        }

        public void Unload()
        {
            GL.DeleteBuffer(position_buffer);
            GL.DeleteBuffer(uv_buffer);
            GL.DeleteBuffer(color_buffer);
            GL.DeleteBuffer(tex_id_buffer);
            GL.DeleteBuffer(tex_weight_buffer);
            GL.DeleteVertexArray(vertex_array);
            units.Clear();
            objects.Clear();
            decorations.Clear();
        }

        public void AddUnit(SFMapUnit u)
        {
            units.Add(u);
        }

        public void AddObject(SFMapObject o)
        {
            objects.Add(o);
        }

        public void AddDecoration(SFMapDecoration d)
        {
            decorations.Add(d);
        }
    }

    public class SFMapHeightMap
    {
        public SFMap map = null;
        public SFMapTerrainTextureManager texture_manager { get; private set; } = new SFMapTerrainTextureManager();
        public int width, height;
        short[] height_data;
        byte[] tile_data;
        bool[] temporary_mask;

        public int chunk_size { get; private set; }
        public SFMapHeightMapChunk[] chunks { get; private set; }
        public List<SFMapHeightMapChunk> visible_chunks = new List<SFMapHeightMapChunk>();

        public SFMapHeightMap(int w, int h)
        {
            width = w;
            height = h;
            height_data = new short[w * h];
            tile_data = new byte[w * h];
            temporary_mask = new bool[w * h];
        }

        public SFMapHeightMapChunk GetChunk(SFCoord pos)
        {
            int chunk_count_y = height / chunk_size;
            return chunks[(chunk_count_y * ((height - pos.y - 1) / chunk_size) + (pos.x / chunk_size))];
        }

        public void SetRowRaw(int row, byte[] chunk_data)
        {
            for(int i = 0; i < width; i++)
                height_data[row * width + i] = BitConverter.ToInt16(chunk_data, i * 2);
        }

        public void SetTilesRaw(byte[] _tiles)
        {
            tile_data = _tiles;
        }

        public void Generate()
        {
            chunk_size = 16;
            int chunk_count_x = width / chunk_size;
            int chunk_count_y = height / chunk_size;
            chunks = new SFMapHeightMapChunk[chunk_count_x * chunk_count_y];
            for (int i = 0; i < chunk_count_y; i++)
                for (int j = 0; j < chunk_count_x; j++)
                {
                    chunks[i * chunk_count_x + j] = new SFMapHeightMapChunk();
                    chunks[i * chunk_count_x + j].hmap = this;
                    chunks[i * chunk_count_x + j].Generate(height_data, tile_data, width, chunk_size, j, i);   // assumes width = height, todo: remove this condition
                }
            foreach (SFMapHeightMapChunk chunk in chunks)
            {
                chunk.TransformMaterialsToTextures(texture_manager);
                chunk.Init();
            }
        }

        public short GetZ(SFCoord pos)
        {
            return height_data[pos.y * width + pos.x];
        }

        public float GetRealZ(OpenTK.Vector2 pos)
        {
            int chunk_count_x = width / chunk_size;
            int chunk_count_y = height / chunk_size;
            // get chunk id
            int cx = (int)(pos.X / chunk_size);
            int cy = (int)(pos.Y / chunk_size);
            if ((cx < 0) || (cx >= chunk_count_x) || (cy < 0) || (cy >= chunk_count_y))
                return 0;
            // calculate ray collision point
            SF3D.Physics.Ray ray = new SF3D.Physics.Ray(new Vector3(pos.X, 1000, width - pos.Y+chunk_size), new Vector3(0, -1100, 0));
            Vector3 result;
            if (ray.Intersect(chunks[cy * chunk_count_x + cx].vertices, new Vector3(cx * chunk_size, 0, width - (cy * chunk_size)), out result))
                return result.Y;
            return 0;
        }

        public void ResetMask()
        {
            for (int i = 0; i < width * height; i++)
                temporary_mask[i] = false;
        }

        // used in generation of lakes
        // flood fill based on z difference and return result
        public List<SFCoord> GetIslandByHeight(SFCoord start, short z_diff)
        {
            ResetMask();
            List<SFCoord> island = new List<SFCoord>();
            Queue<SFCoord> to_be_checked = new Queue<SFCoord>();
            SFCoord cur_pos;
            SFCoord next_pos;

            short start_z = GetZ(start);
            temporary_mask[start.y * width + start.x] = true;
            to_be_checked.Enqueue(start);

            while(to_be_checked.Count != 0)
            {
                cur_pos = to_be_checked.Dequeue();
                island.Add(cur_pos);

                next_pos = cur_pos; next_pos.x += 1;
                if ((next_pos.x < width) && (temporary_mask[next_pos.y * width + next_pos.x] != true) && (GetZ(next_pos) - start_z < z_diff))
                {
                    to_be_checked.Enqueue(next_pos);
                    temporary_mask[next_pos.y * width + next_pos.x] = true;
                }
                next_pos = cur_pos; next_pos.y += 1;
                if ((next_pos.y < height) && (temporary_mask[next_pos.y * width + next_pos.x] != true) && (GetZ(next_pos) - start_z < z_diff))
                {
                    to_be_checked.Enqueue(next_pos);
                    temporary_mask[next_pos.y * width + next_pos.x] = true;
                }
                next_pos = cur_pos; next_pos.x -= 1;
                if ((next_pos.x >= 0) && (temporary_mask[next_pos.y * width + next_pos.x] != true) && (GetZ(next_pos) - start_z < z_diff))
                {
                    to_be_checked.Enqueue(next_pos);
                    temporary_mask[next_pos.y * width + next_pos.x] = true;
                }
                next_pos = cur_pos; next_pos.y -= 1;
                if ((next_pos.y >= 0) && (temporary_mask[next_pos.y * width + next_pos.x] != true) && (GetZ(next_pos) - start_z < z_diff))
                {
                    to_be_checked.Enqueue(next_pos);
                    temporary_mask[next_pos.y * width + next_pos.x] = true;
                }
            }

            return island;
        }

        public void Unload()
        {
            texture_manager.Unload();
            foreach (SFMapHeightMapChunk chunk in chunks)
                chunk.Unload();
        }
    }
}
