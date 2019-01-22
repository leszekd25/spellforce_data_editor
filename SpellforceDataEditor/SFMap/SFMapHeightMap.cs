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
        public int width, height;
        public int ix, iy;
        public byte[] material_id;
        public Vector3[] vertices;
        public Vector4[] colors;
        public Vector2[] uvs;
        public Vector3[] texture_id;
        public Vector3[] texture_weights;

        public int vertex_array, position_buffer, color_buffer, uv_buffer, tex_id_buffer, tex_weight_buffer;

        // public BoundingBox aabb;

        public void Generate(short[] data, byte[] tex_data, int map_size, int size, int x, int y)
        {
            float flatten_factor = 100;

            width = size;
            height = size;
            ix = x;
            iy = y;

            int chunk_count = map_size / size;

            int row_start = (chunk_count-y-1) * size;
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
                for(int j = 0; j < size; j++)
                {
                    int t = (i * size + j) * 6;
                    Vector3 triangle_mats;
                    Vector3 triangle_ws;

                    if ((i + j) % 2 == 0)
                    {
                        // left triangle
                        vertices[t + 0] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 1] = new Vector3((float)j+1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i+1) - 1);
                        vertices[t + 2] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i+1) - 1);

                        triangle_mats = new Vector3(material_id[i*(size+1)+j], material_id[(i+1)* (size + 1) + j+1], material_id[(i+1)* (size + 1) + j]);
                        triangle_mats = SortV3(triangle_mats);
                        texture_id[t + 0] = triangle_mats;
                        texture_id[t + 1] = triangle_mats;
                        texture_id[t + 2] = triangle_mats;

                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[i * (size + 1) + j])] = 1.0f;
                        texture_weights[t + 0] = triangle_ws;
                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[(i+1) * (size + 1) + (j+1)])] = 1.0f;
                        texture_weights[t + 1] = triangle_ws;
                        triangle_ws = new Vector3(0.0f);
                        triangle_ws[GetIndex(triangle_mats, material_id[(i+1) * (size + 1) + j])] = 1.0f;
                        texture_weights[t + 2] = triangle_ws;

                        // right triangle
                        vertices[t + 3] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 4] = new Vector3((float)j+1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 5] = new Vector3((float)j+1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

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
                        vertices[t + 1] = new Vector3((float)j+1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
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
                        vertices[t + 3] = new Vector3((float)j+1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 4] = new Vector3((float)j+1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);
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

            for(int i = 0; i < vertices.Length; i++)
            {
                // color (debug heightmap)
                float h = vertices[i].Y;
                byte col = (byte)(Math.Min(255, (int)(h * 8)));
                float norm_color = (float)col / 255.0f;
                colors[i] = new OpenTK.Vector4(norm_color, norm_color, norm_color, 1.0f);
                uvs[i] = vertices[i].Xz / 4;
            }
        }

        public void TransformMaterialsToTextures(SFMapTerrainTextureManager man)
        {
            for (int i = 0; i < (width * width * 6); i++)
            {
                for(int j = 0; j < 3; j++)
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
            if(r[1] > t)
            {
                t = r[1];
                max_id = 1;
            }
            if (r[2] > t)
                max_id = 2;

            t = r[0];
            if(r[1] < t)
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
        }
    }

    public class SFMapHeightMap
    {
        public SFMapTerrainTextureManager texture_manager { get; private set; } = new SFMapTerrainTextureManager();
        public int width, height;
        short[] height_data;
        byte[] tile_data;
        Object3D[] units;
        Object3D[] decorations;

        public int chunk_size { get; private set; }
        public SFMapHeightMapChunk[] chunks { get; private set; }

        public SFMapHeightMap(int w, int h)
        {
            width = w;
            height = h;
            height_data = new short[w * h];
            tile_data = new byte[w * h];
            units = new Object3D[w * h];
            decorations = new Object3D[w * h];
        }

        public void SetUnit(int x, int y, Object3D u)
        {
            units[y * width + x] = u;
        }

        public void SetDecoration(int x, int y, Object3D d)
        {
            decorations[y * width + x] = d;
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

        public void Unload()
        {
            texture_manager.Unload();
            foreach (SFMapHeightMapChunk chunk in chunks)
                chunk.Unload();
        }
    }
}
