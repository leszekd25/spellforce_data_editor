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
        
        // heightmap
        public byte[] material_id;
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector2[] uvs;
        public Vector3[] texture_id;
        public Vector3[] texture_weights;

        // overlays
        public Dictionary<string, MapEdit.MapOverlayChunk> overlays { get; private set; } = new Dictionary<string, MapEdit.MapOverlayChunk>();

        public SF3D.Physics.BoundingBox aabb;
        public List<SFMapBuilding> buildings = new List<SFMapBuilding>();
        public List<SFMapObject> objects = new List<SFMapObject>();
        public List<SFMapInteractiveObject> int_objects = new List<SFMapInteractiveObject>();
        public List<SFMapUnit> units = new List<SFMapUnit>();
        public List<SFMapPortal> portals = new List<SFMapPortal>();
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
                SF3D.SceneSynchro.SFScene scene = SF3D.SFRender.SFRenderEngine.scene;
                foreach (SFMapUnit u in units)
                    scene.objects_static[u.GetObjectName()].Visible = value;
                foreach (SFMapObject o in objects)
                    scene.objects_static[o.GetObjectName()].Visible = value;
                foreach (SFMapInteractiveObject io in int_objects)
                    scene.objects_static[io.GetObjectName()].Visible = value;
                foreach (SFMapDecoration d in decorations)
                    scene.objects_static[d.GetObjectName()].Visible = value;
                foreach (SFMapBuilding b in buildings)
                    scene.objects_static[b.GetObjectName()].Visible = value;
                foreach (SFMapPortal p in portals)
                    scene.objects_static[p.GetObjectName()].Visible = value;
            }
        }

        public int vertex_array, position_buffer, normal_buffer, uv_buffer, tex_id_buffer, tex_weight_buffer;


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
            normals = new OpenTK.Vector3[size * size * 6];
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

                        normals[t + 0] = GetVertexNormal(data, map_size, col_start + j, row_start + i);
                        normals[t + 1] = GetVertexNormal(data, map_size, col_start + j+1, row_start + i+1);
                        normals[t + 2] = GetVertexNormal(data, map_size, col_start + j, row_start + i+1);
                        normals[t + 3] = normals[(i * size + j) * 6 + 0]; //GetVertexNormal(data, map_size, col_start + j, row_start + i);
                        normals[t + 4] = GetVertexNormal(data, map_size, col_start + j+1, row_start + i);
                        normals[t + 5] = normals[(i * size + j) * 6 + 1]; //GetVertexNormal(data, map_size, col_start + j+1, row_start + i+1);
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

                        normals[t + 0] = GetVertexNormal(data, map_size, col_start + j, row_start + i);
                        normals[t + 1] = GetVertexNormal(data, map_size, col_start + j + 1, row_start + i);
                        normals[t + 2] = GetVertexNormal(data, map_size, col_start + j, row_start + i + 1);
                        normals[t + 3] = normals[(i * size + j) * 6 + 1]; //GetVertexNormal(data, map_size, col_start + j + 1, row_start + i);
                        normals[t + 4] = GetVertexNormal(data, map_size, col_start + j + 1, row_start + i + 1);
                        normals[t + 5] = normals[(i * size + j) * 6 + 2]; //GetVertexNormal(data, map_size, col_start + j, row_start + i + 1);
                    }
                }
            }

            float max_height = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                // color (debug heightmap)
                float h = vertices[i].Y;
                uvs[i] = vertices[i].Xz / 4;
                max_height = Math.Max(max_height, h);
            }

            aabb = new SF3D.Physics.BoundingBox(new Vector3(x * size, 0, y * size), new Vector3((x + 1) * size, max_height, (y * size) + size));
        }

        public void Init()
        {
            vertex_array = GL.GenVertexArray();
            position_buffer = GL.GenBuffer();
            uv_buffer = GL.GenBuffer();
            normal_buffer = GL.GenBuffer();
            tex_id_buffer = GL.GenBuffer();
            tex_weight_buffer = GL.GenBuffer();

            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, position_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * 12, vertices, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, normal_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, normals.Length * 12, normals, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, uv_buffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, uvs.Length * 8, uvs, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, tex_id_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, texture_id.Length * 12, texture_id, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, tex_weight_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, texture_weights.Length * 12, texture_weights, BufferUsageHint.DynamicDraw);
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
            LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapHeightMap.GetIndex(): Could not find id = "+id.ToString()+" in specified material vector! This should not happen!");
            throw new Exception("SFMapHeightMap.GetIndex(): Invalid id!");
        }

        // https://www.gamedev.net/forums/topic/163625-fast-way-to-calculate-heightmap-normals/
        private Vector3 GetVertexNormal(short[] data, int map_size, int x, int y)
        {
            float hscale = 100.0f;
            float az = (x < map_size - 1) ? (GetHeightAt(data, map_size, x + 1, y)) : (0);
            float bz = (y < map_size - 1) ? (GetHeightAt(data, map_size, x, y + 1)) : (0);
            float cz = (x > 0) ? (GetHeightAt(data, map_size, x - 1, y)) : (0);
            float dz = (y > 0) ? (GetHeightAt(data, map_size, x, y - 1)) : (0);

            return (new Vector3(cz - az, 2 * hscale, dz - bz)).Normalized();
        }

        public void RebuildGeometry(short[] data, int map_size)
        {
            float flatten_factor = 100;

            int size = width;
            int chunk_count = map_size / size;

            int row_start = (chunk_count - iy - 1) * size;
            int col_start = ix * size;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int t = (i * size + j) * 6;

                    if ((i + j) % 2 == 0)
                    {
                        // left triangle
                        vertices[t + 0] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 1] = new Vector3((float)j + 1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);
                        vertices[t + 2] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        // right triangle
                        vertices[t + 3] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 4] = new Vector3((float)j + 1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 5] = new Vector3((float)j + 1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        normals[t + 0] = GetVertexNormal(data, map_size, col_start + j, row_start + i);
                        normals[t + 1] = GetVertexNormal(data, map_size, col_start + j + 1, row_start + i + 1);
                        normals[t + 2] = GetVertexNormal(data, map_size, col_start + j, row_start + i + 1);
                        normals[t + 3] = normals[(i * size + j) * 6 + 0]; //GetVertexNormal(data, map_size, col_start + j, row_start + i);
                        normals[t + 4] = GetVertexNormal(data, map_size, col_start + j + 1, row_start + i);
                        normals[t + 5] = normals[(i * size + j) * 6 + 1]; //GetVertexNormal(data, map_size, col_start + j+1, row_start + i+1);
                    }
                    else
                    {
                        // left triangle
                        vertices[t + 0] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 1] = new Vector3((float)j + 1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 2] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        // right triangle
                        vertices[t + 3] = new Vector3((float)j + 1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 4] = new Vector3((float)j + 1, GetHeightAt(data, map_size, col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);
                        vertices[t + 5] = new Vector3((float)j, GetHeightAt(data, map_size, col_start + j, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        normals[t + 0] = GetVertexNormal(data, map_size, col_start + j, row_start + i);
                        normals[t + 1] = GetVertexNormal(data, map_size, col_start + j + 1, row_start + i);
                        normals[t + 2] = GetVertexNormal(data, map_size, col_start + j, row_start + i + 1);
                        normals[t + 3] = normals[(i * size + j) * 6 + 1]; //GetVertexNormal(data, map_size, col_start + j + 1, row_start + i);
                        normals[t + 4] = GetVertexNormal(data, map_size, col_start + j + 1, row_start + i + 1);
                        normals[t + 5] = normals[(i * size + j) * 6 + 2]; //GetVertexNormal(data, map_size, col_start + j, row_start + i + 1);
                    }
                }
            }

            float max_height = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                float h = vertices[i].Y;
                max_height = Math.Max(max_height, h);
            }

            aabb = new SF3D.Physics.BoundingBox(new Vector3(ix * size, 0, iy * size), new Vector3((ix + 1) * size, max_height, (iy * size) + size));

            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, position_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * 12, vertices, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, normal_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, normals.Length * 12, normals, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindVertexArray(0);


            // fix all object positions (without lakes for now...)
            SF3D.SceneSynchro.SFScene scene = SF3D.SFRender.SFRenderEngine.scene;
            foreach (SFMapUnit u in units)
            {
                SF3D.Object3D _obj = scene.objects_static[u.GetObjectName()];
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(u.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapObject o in objects)
            {
                SF3D.Object3D _obj = scene.objects_static[o.GetObjectName()];
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(o.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapInteractiveObject io in int_objects)
            {
                SF3D.Object3D _obj = scene.objects_static[io.GetObjectName()];
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(io.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapDecoration d in decorations)
            {
                SF3D.Object3D _obj = scene.objects_static[d.GetObjectName()];
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(d.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapBuilding b in buildings)
            {
                SF3D.Object3D _obj = scene.objects_static[b.GetObjectName()];
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(b.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapPortal p in portals)
            {
                SF3D.Object3D _obj = scene.objects_static[p.GetObjectName()];
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(p.grid_position) / 100.0f, _obj.Position.Z);
            }

            foreach (MapEdit.MapOverlayChunk o in overlays.Values)
                o.Update(this);
        }

        public void RebuildTerrainTexture(byte[] tex_data, int map_size)
        {
            int size = width;
            int chunk_count = map_size / size;

            int row_start = (chunk_count - iy - 1) * size;
            int col_start = ix * size;

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

            for (int i = 0; i < vertices.Length; i++)
            {
                // color (debug heightmap)
                float h = vertices[i].Y;
            }

            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, tex_id_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, texture_id.Length * 12, texture_id, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, tex_weight_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, texture_weights.Length * 12, texture_weights, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindVertexArray(0);
        }

        public void Unload()
        {
            if (vertex_array == -1)
                return;

            GL.DeleteBuffer(position_buffer);
            GL.DeleteBuffer(uv_buffer);
            GL.DeleteBuffer(normal_buffer);
            GL.DeleteBuffer(tex_id_buffer);
            GL.DeleteBuffer(tex_weight_buffer);
            GL.DeleteVertexArray(vertex_array);
            units.Clear();
            objects.Clear();
            buildings.Clear();
            decorations.Clear();
            int_objects.Clear();
            portals.Clear();
            overlays.Clear();
            lakes_contained.Clear();
            hmap = null;
            material_id = null;
            vertices = null;
            normals = null;
            uvs = null;
            texture_id = null;
            texture_weights = null;
        }

        public void AddUnit(SFMapUnit u)
        {
            units.Add(u);
        }

        public void RemoveUnit(SFMapUnit u)
        {
            units.Remove(u);
        }

        public void AddObject(SFMapObject o)
        {
            objects.Add(o);
        }

        public void RemoveObject(SFMapObject o)
        {
            objects.Remove(o);
        }

        public void AddInteractiveObject(SFMapInteractiveObject io)
        {
            int_objects.Add(io);
        }

        public void RemoveInteractiveObject(SFMapInteractiveObject io)
        {
            int_objects.Remove(io);
        }

        public void AddDecoration(SFMapDecoration d)
        {
            decorations.Add(d);
        }

        public void RemoveDecoration(SFMapDecoration d)
        {
            decorations.Remove(d);
        }

        public void AddBuilding(SFMapBuilding b)
        {
            buildings.Add(b);
        }

        public void RemoveBuilding(SFMapBuilding b)
        {
            buildings.Remove(b);
        }

        public void AddPortal(SFMapPortal p)
        {
            portals.Add(p);
        }

        public void RemovePortal(SFMapPortal p)
        {
            portals.Remove(p);
        }

        public void OverlayUpdate(string o_name)
        {
            overlays[o_name].Update(this);
        }
    }

    // didnt know where to put it
    public struct SFMapChunk60Data
    {
        public byte unknown;
        public SFCoord pos;

        public SFMapChunk60Data(byte u, SFCoord p)
        {
            unknown = u;
            pos = p;
        }
    }

    public class SFMapHeightMap
    {
        public SFMap map = null;
        public SFMapTerrainTextureManager texture_manager { get; private set; } = new SFMapTerrainTextureManager();
        public int width, height;
        public short[] height_data;
        public byte[] tile_data;
        public byte[] lake_data;    // 0 - no lake, 1-255 - lakes 0-254
        public ushort[] building_data;   // 0 - no building, 1-65535 - buildings 0-65534
        public bool[] temporary_mask;   // for calculating islands by height
        public List<SFCoord> chunk42_data = new List<SFCoord>();
        public List<SFCoord> chunk56_data = new List<SFCoord>();
        public List<SFMapChunk60Data> chunk60_data = new List<SFMapChunk60Data>();

        public int chunk_size { get; private set; }
        public SFMapHeightMapChunk[] chunks { get; private set; }
        public List<SFMapHeightMapChunk> visible_chunks = new List<SFMapHeightMapChunk>();

        public List<string> visible_overlays { get; private set; } = new List<string>();

        public SFMapHeightMap(int w, int h)
        {
            width = w;
            height = h;
            height_data = new short[w * h];
            tile_data = new byte[w * h];
            lake_data = new byte[w * h];  lake_data.Initialize();
            building_data = new ushort[w * h]; building_data.Initialize();
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

        public void GetRowRaw(int row, ref short[] chunk_data)
        {
            for (int i = 0; i < width; i++)
                chunk_data[i] = height_data[row * width + i];
        }

        public void SetTilesRaw(byte[] _tiles)
        {
            tile_data = _tiles;
        }

        public void Generate()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapHeightMap.Generate() called");

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
                chunk.Init();
            }
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapHeightMap.Generate(): Chunks generated: "+chunks.Length.ToString());
        }

        public short GetZ(SFCoord pos)
        {
            return height_data[pos.y * width + pos.x];
        }

        public float GetRealZ(Vector2 pos)
        {
            int chunk_count_x = width / chunk_size;
            int chunk_count_y = height / chunk_size;
            // get chunk id
            int cx = (int)(pos.X / chunk_size);
            int cy = (int)((map.height-pos.Y-1) / chunk_size);
            if ((cx < 0) || (cx >= chunk_count_x) || (cy < 0) || (cy >= chunk_count_y))
                return 0;
            // calculate ray collision point
            SF3D.Physics.Ray ray = new SF3D.Physics.Ray(new Vector3(pos.X, 1000, pos.Y), new Vector3(0, -1100, 0));
            Vector3 result;
            if (ray.Intersect(chunks[(chunk_count_y - cy - 1) * chunk_count_x + cx].vertices, new Vector3(cx * chunk_size, 0, height - ((cy+1) * chunk_size)), out result))
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
        public HashSet<SFCoord> GetIslandByHeight(SFCoord start, short z_diff)
        {
            ResetMask();
            HashSet<SFCoord> island = new HashSet<SFCoord>();
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

        public void RebuildGeometry(SFCoord topleft, SFCoord bottomright)
        {
            int chunk_size = 16;
            int chunk_count_x = width / chunk_size;

            int topchunkx = topleft.x / 16;
            int topchunky = topleft.y / 16;
            int botchunkx = bottomright.x / 16;
            int botchunky = bottomright.y / 16;

            for (int i = topchunkx; i <= botchunkx; i++)
            {
                for (int j = topchunky; j <= botchunky; j++)
                {
                    chunks[j * chunk_count_x + i].RebuildGeometry(height_data, width);  // room to optimize,
                }
            }
        }

        public void RebuildTerrainTexture(SFCoord topleft, SFCoord bottomright)
        {
            int chunk_size = 16;
            int chunk_count_x = width / chunk_size;

            int topchunkx = topleft.x / 16;
            int topchunky = topleft.y / 16;
            int botchunkx = bottomright.x / 16;
            int botchunky = bottomright.y / 16;

            for (int i = topchunkx; i <= botchunkx; i++)
            {
                for (int j = topchunky; j <= botchunky; j++)
                {
                    chunks[j * chunk_count_x + i].RebuildTerrainTexture(tile_data, width);  // room to optimize,
                }
            }
        }

        public void OverlayCreate(string o_name, Vector4 col)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapHeightMap.OverlayCreate() called, overlay name: "+o_name);

            foreach (SFMapHeightMapChunk chunk in chunks)
            {
                chunk.overlays.Add(o_name, new MapEdit.MapOverlayChunk());
                chunk.overlays[o_name].Init();
                chunk.overlays[o_name].color = col;
            }
        }

        public void OverlayAdd(string o_name, SFCoord pos)
        {
            SFMapHeightMapChunk chunk = GetChunk(pos);
            SFCoord new_pos = pos - new SFCoord(chunk.ix * chunk_size, ((width / chunk_size) - chunk.iy - 1) * chunk_size);
            if (!chunk.overlays[o_name].points.Contains(new_pos))
                chunk.overlays[o_name].points.Add(new_pos);
        }

        public void OverlayRemove(string o_name, SFCoord pos)
        {
            SFMapHeightMapChunk chunk = GetChunk(pos);
            SFCoord new_pos = pos - new SFCoord(chunk.ix * chunk_size, ((width / chunk_size) - chunk.iy - 1) * chunk_size);
            int i = chunk.overlays[o_name].points.IndexOf(new_pos);
            if (i != -1)
                chunk.overlays[o_name].points.RemoveAt(i);
        }

        public void OverlayClear(string o_name)
        {
            foreach (SFMapHeightMapChunk chunk in chunks)
                if(chunk.overlays[o_name].points.Count != 0)
                {
                    chunk.overlays[o_name].points.Clear();
                    chunk.overlays[o_name].Update(chunk);
                }
        }

        public void OverlaySetVisible(string o_name, bool visible)
        {
            if ((!visible_overlays.Contains(o_name)) && (visible))
                visible_overlays.Add(o_name);
            else if ((visible_overlays.Contains(o_name)) && (!visible))
                visible_overlays.Remove(o_name);
        }

        public bool OverlayIsVisible(string o_name)
        {
            return visible_overlays.Contains(o_name);
        }

        public void RebuildOverlay(SFCoord topleft, SFCoord bottomright, string o_name)
        {
            int chunk_count_x = width / chunk_size;

            int topchunkx = topleft.x / chunk_size;
            int topchunky = topleft.y / chunk_size;
            int botchunkx = bottomright.x / chunk_size;
            int botchunky = bottomright.y / chunk_size;

            for (int i = topchunkx; i <= botchunkx; i++)
            {
                for (int j = topchunky; j <= botchunky; j++)
                {
                    chunks[j * chunk_count_x + i].OverlayUpdate(o_name); 
                }
            }
        }

        public List<HashSet<SFCoord>> GetSeparateIslands(HashSet<SFCoord> src)
        {
            HashSet<SFCoord> src_copy = new HashSet<SFCoord>();
            foreach (SFCoord p in src)
                src_copy.Add(p);
            List<HashSet<SFCoord>> result = new List<HashSet<SFCoord>>();
            while(src_copy.Count != 0)
            {
                SFCoord start = src_copy.First();
                Queue<SFCoord> island_queue = new Queue<SFCoord>();
                HashSet<SFCoord> island = new HashSet<SFCoord>();
                island_queue.Enqueue(start);
                src_copy.Remove(start);
                while(island_queue.Count != 0)
                {
                    SFCoord next_pos = island_queue.Dequeue();
                    SFCoord new_pos = next_pos;
                    island.Add(next_pos);

                    new_pos = next_pos + new SFCoord(1, 0);
                    if (src_copy.Contains(new_pos))
                    {
                        island_queue.Enqueue(new_pos);
                        src_copy.Remove(new_pos);
                    }
                    new_pos = next_pos + new SFCoord(-1, 0);
                    if (src_copy.Contains(new_pos))
                    {
                        island_queue.Enqueue(new_pos);
                        src_copy.Remove(new_pos);
                    }
                    new_pos = next_pos + new SFCoord(0, 1);
                    if (src_copy.Contains(new_pos))
                    {
                        island_queue.Enqueue(new_pos);
                        src_copy.Remove(new_pos);
                    }
                    new_pos = next_pos + new SFCoord(0, -1);
                    if (src_copy.Contains(new_pos))
                    {
                        island_queue.Enqueue(new_pos);
                        src_copy.Remove(new_pos);
                    }
                }
                result.Add(island);
            }
            return result;
        }

        public bool CanMoveToPosition(SFCoord pos)
        {
            // check if tile is passable
            byte tex_index = tile_data[pos.y * width + pos.x];
            if (texture_manager.texture_tiledata[tex_index].blocks_movement)
                return false;

            SFMapHeightMapChunk chunk = GetChunk(pos);
            // check if another unit is on position
            foreach (SFMapUnit u in chunk.units)
                if (u.grid_position == pos)
                    return false;
            // check if another object is on position
            foreach (SFMapObject o in chunk.objects)
                if (o.grid_position == pos)
                    return false;
            // check if lake is on position
            if (lake_data[pos.y * width + pos.x] != 0)
                return false;
            // check if building is on position
            if (building_data[pos.y * width + pos.x] != 0)
                return false;

            return true;
        }

        public void Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapHeightMap.Unload() called");
            if (map == null)
                return;

            texture_manager.Unload();
            foreach (SFMapHeightMapChunk chunk in chunks)
                chunk.Unload();
            chunk42_data.Clear();
            chunk56_data.Clear();
            chunk60_data.Clear();
            visible_chunks.Clear();
            visible_overlays.Clear();
            chunks = null;
            map = null;
        }
    }
}
