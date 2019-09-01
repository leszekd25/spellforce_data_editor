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
        public SF3D.SceneSynchro.SceneNodeMapChunk owner = null;
        public int width, height;
        public int id, ix, iy;
        public bool visible = false;
        
        // heightmap
        public byte[] material_id;
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector2[] uvs;
        public Vector3[] texture_id;

        public SF3D.Physics.CollisionMesh collision_cache = new SF3D.Physics.CollisionMesh();
        //public SF3D.Physics.Triangle[] collision_cache;

        // lake
        public SFModel3D lake_model = null;    // generated here, but owned by ResourceManager

        // overlays
        public Dictionary<string, MapEdit.MapOverlayChunk> overlays { get; private set; } = new Dictionary<string, MapEdit.MapOverlayChunk>();

        //public SF3D.Physics.BoundingBox aabb;
        public List<SFMapBuilding> buildings = new List<SFMapBuilding>();
        public List<SFMapObject> objects = new List<SFMapObject>();
        public List<SFMapInteractiveObject> int_objects = new List<SFMapInteractiveObject>();
        public List<SFMapUnit> units = new List<SFMapUnit>();
        public List<SFMapPortal> portals = new List<SFMapPortal>();
        public List<SFMapDecoration> decorations = new List<SFMapDecoration>();

        public int vertex_array, position_buffer, normal_buffer, uv_buffer, tex_id_buffer = -1;


        public void Generate()
        {
            if (!visible)
                return;

            Degenerate();

            ushort[] data = hmap.height_data;
            byte[] tex_data = hmap.tile_data;

            float flatten_factor = 100;

            int map_size = hmap.width;
            int size = width;
            
            id = iy * (map_size / size) + ix;

            int chunk_count = map_size / size;

            int row_start = (chunk_count - iy - 1) * size;
            int col_start = ix * size;

            material_id = new byte[(size + 1) * (size + 1)];

            vertices = new OpenTK.Vector3[size * size * 6];
            normals = new OpenTK.Vector3[size * size * 6];
            uvs = new OpenTK.Vector2[size * size * 6];
            texture_id = new Vector3[size * size * 6];

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

                    if ((i + j) % 2 == 0)
                    {
                        // left triangle
                        vertices[t + 0] = new Vector3((float)j, hmap.GetHeightAt(col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 1] = new Vector3((float)j + 1, hmap.GetHeightAt(col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);
                        vertices[t + 2] = new Vector3((float)j, hmap.GetHeightAt(col_start + j, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        triangle_mats = new Vector3(material_id[i * (size + 1) + j], material_id[(i + 1) * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j]);
                        texture_id[t + 0] = triangle_mats;
                        texture_id[t + 1] = triangle_mats;
                        texture_id[t + 2] = triangle_mats;

                        // right triangle
                        vertices[t + 3] = new Vector3((float)j, hmap.GetHeightAt(col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 4] = new Vector3((float)j + 1, hmap.GetHeightAt(col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 5] = new Vector3((float)j + 1, hmap.GetHeightAt(col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        triangle_mats = new Vector3(material_id[i * (size + 1) + j], material_id[i * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j + 1]);
                        texture_id[t + 3] = triangle_mats;
                        texture_id[t + 4] = triangle_mats;
                        texture_id[t + 5] = triangle_mats;

                        normals[t + 0] = hmap.GetVertexNormal(col_start + j, row_start + i);
                        normals[t + 1] = hmap.GetVertexNormal(col_start + j+1, row_start + i+1);
                        normals[t + 2] = hmap.GetVertexNormal(col_start + j, row_start + i+1);
                        normals[t + 3] = normals[(i * size + j) * 6 + 0]; //GetVertexNormal(data, map_size, col_start + j, row_start + i);
                        normals[t + 4] = hmap.GetVertexNormal(col_start + j+1, row_start + i);
                        normals[t + 5] = normals[(i * size + j) * 6 + 1]; //GetVertexNormal(data, map_size, col_start + j+1, row_start + i+1);
                    }
                    else
                    {
                        // left triangle
                        vertices[t + 0] = new Vector3((float)j, hmap.GetHeightAt(col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 1] = new Vector3((float)j + 1, hmap.GetHeightAt(col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 2] = new Vector3((float)j, hmap.GetHeightAt(col_start + j, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        triangle_mats = new Vector3(material_id[i * (size + 1) + j], material_id[i * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j]);
                        texture_id[t + 0] = triangle_mats;
                        texture_id[t + 1] = triangle_mats;
                        texture_id[t + 2] = triangle_mats;
                        // right triangle
                        vertices[t + 3] = new Vector3((float)j + 1, hmap.GetHeightAt(col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 4] = new Vector3((float)j + 1, hmap.GetHeightAt(col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);
                        vertices[t + 5] = new Vector3((float)j, hmap.GetHeightAt(col_start + j, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        triangle_mats = new Vector3(material_id[i * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j]);
                        texture_id[t + 3] = triangle_mats;
                        texture_id[t + 4] = triangle_mats;
                        texture_id[t + 5] = triangle_mats;

                        normals[t + 0] = hmap.GetVertexNormal(col_start + j, row_start + i);
                        normals[t + 1] = hmap.GetVertexNormal(col_start + j + 1, row_start + i);
                        normals[t + 2] = hmap.GetVertexNormal(col_start + j, row_start + i + 1);
                        normals[t + 3] = normals[(i * size + j) * 6 + 1]; //GetVertexNormal(data, map_size, col_start + j + 1, row_start + i);
                        normals[t + 4] = hmap.GetVertexNormal(col_start + j + 1, row_start + i + 1);
                        normals[t + 5] = normals[(i * size + j) * 6 + 2]; //GetVertexNormal(data, map_size, col_start + j, row_start + i + 1);
                    }
                }
            }
            
            for (int i = 0; i < vertices.Length; i++)
                uvs[i] = vertices[i].Xz / 4;
            
            collision_cache.Generate(new Vector3(ix*size,0,iy*size), vertices);
            //GenerateAABB();
            //GenerateGeometryCache();
            Init();
        }

        public void GenerateTemporaryAABB()
        {
            int size = width;

            ushort max_height = 0;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    ushort h = hmap.GetHeightAt(ix * size + j, iy * size + i);
                    if (h > max_height)
                        max_height = h;
                }
            }
            collision_cache.aabb = new SF3D.Physics.BoundingBox(new Vector3(ix * size, 0, iy * size), new Vector3((ix + 1) * size, max_height/100.0f, (iy * size) + size));
        }

        /*public void GenerateGeometryCache()
        {
            int triangle_count = vertices.Length / 3;
            collision_cache = new SF3D.Physics.Triangle[triangle_count];
            for (int i = 0; i < triangle_count; i++)
                collision_cache[i] = new SF3D.Physics.Triangle(vertices[i * 3 + 0],
                                                               vertices[i * 3 + 1],
                                                               vertices[i * 3 + 2]);
        }*/

        public void Init()
        {
            vertex_array = GL.GenVertexArray();
            position_buffer = GL.GenBuffer();
            uv_buffer = GL.GenBuffer();
            normal_buffer = GL.GenBuffer();
            tex_id_buffer = GL.GenBuffer();

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

            GL.BindVertexArray(0);
        }

        public void Degenerate()
        {
            if (vertex_array == -1)
                return;

            GL.DeleteBuffer(position_buffer);
            GL.DeleteBuffer(uv_buffer);
            GL.DeleteBuffer(normal_buffer);
            GL.DeleteBuffer(tex_id_buffer);
            GL.DeleteVertexArray(vertex_array);

            material_id = null;
            vertices = null;
            normals = null;
            uvs = null;
            texture_id = null;
            collision_cache.triangles = null;

            vertex_array = -1;
        }


        // for heightmap edit only
        public void RebuildGeometry()
        {
            if (!visible)
                return;

            float flatten_factor = 100;

            int size = width;
            int chunk_count = hmap.width / size;

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
                        vertices[t + 0] = new Vector3((float)j, hmap.GetHeightAt(col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 1] = new Vector3((float)j + 1, hmap.GetHeightAt(col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);
                        vertices[t + 2] = new Vector3((float)j, hmap.GetHeightAt(col_start + j, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        // right triangle
                        vertices[t + 3] = new Vector3((float)j, hmap.GetHeightAt(col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 4] = new Vector3((float)j + 1, hmap.GetHeightAt(col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 5] = new Vector3((float)j + 1, hmap.GetHeightAt(col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        normals[t + 0] = hmap.GetVertexNormal(col_start + j, row_start + i);
                        normals[t + 1] = hmap.GetVertexNormal(col_start + j + 1, row_start + i + 1);
                        normals[t + 2] = hmap.GetVertexNormal(col_start + j, row_start + i + 1);
                        normals[t + 3] = normals[(i * size + j) * 6 + 0]; //GetVertexNormal(data, map_size, col_start + j, row_start + i);
                        normals[t + 4] = hmap.GetVertexNormal(col_start + j + 1, row_start + i);
                        normals[t + 5] = normals[(i * size + j) * 6 + 1]; //GetVertexNormal(data, map_size, col_start + j+1, row_start + i+1);
                    }
                    else
                    {
                        // left triangle
                        vertices[t + 0] = new Vector3((float)j, hmap.GetHeightAt(col_start + j, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 1] = new Vector3((float)j + 1, hmap.GetHeightAt(col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 2] = new Vector3((float)j, hmap.GetHeightAt(col_start + j, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        // right triangle
                        vertices[t + 3] = new Vector3((float)j + 1, hmap.GetHeightAt(col_start + j + 1, row_start + i) / flatten_factor, ((float)size) - (float)i - 1);
                        vertices[t + 4] = new Vector3((float)j + 1, hmap.GetHeightAt(col_start + j + 1, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);
                        vertices[t + 5] = new Vector3((float)j, hmap.GetHeightAt(col_start + j, row_start + i + 1) / flatten_factor, ((float)size) - (float)(i + 1) - 1);

                        normals[t + 0] = hmap.GetVertexNormal(col_start + j, row_start + i);
                        normals[t + 1] = hmap.GetVertexNormal(col_start + j + 1, row_start + i);
                        normals[t + 2] = hmap.GetVertexNormal(col_start + j, row_start + i + 1);
                        normals[t + 3] = normals[(i * size + j) * 6 + 1]; //GetVertexNormal(data, map_size, col_start + j + 1, row_start + i);
                        normals[t + 4] = hmap.GetVertexNormal(col_start + j + 1, row_start + i + 1);
                        normals[t + 5] = normals[(i * size + j) * 6 + 2]; //GetVertexNormal(data, map_size, col_start + j, row_start + i + 1);
                    }
                }
            }

            collision_cache = new SF3D.Physics.CollisionMesh();
            collision_cache.Generate(new Vector3(ix * size, 0, iy * size), vertices);
            //GenerateAABB();
            //GenerateGeometryCache();

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
            foreach (SFMapUnit u in units)
            {
                SF3D.SceneSynchro.SceneNode _obj = owner.FindNode<SF3D.SceneSynchro.SceneNode>(u.GetObjectName());
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(u.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapObject o in objects)
            {
                SF3D.SceneSynchro.SceneNode _obj = owner.FindNode<SF3D.SceneSynchro.SceneNode>(o.GetObjectName());
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(o.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapInteractiveObject io in int_objects)
            {
                SF3D.SceneSynchro.SceneNode _obj = owner.FindNode<SF3D.SceneSynchro.SceneNode>(io.GetObjectName());
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(io.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapDecoration d in decorations)
            {
                SF3D.SceneSynchro.SceneNode _obj = owner.FindNode<SF3D.SceneSynchro.SceneNode>(d.GetObjectName());
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(d.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapBuilding b in buildings)
            {
                SF3D.SceneSynchro.SceneNode _obj = owner.FindNode<SF3D.SceneSynchro.SceneNode>(b.GetObjectName());
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(b.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapPortal p in portals)
            {
                SF3D.SceneSynchro.SceneNode _obj = owner.FindNode<SF3D.SceneSynchro.SceneNode>(p.GetObjectName());
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(p.grid_position) / 100.0f, _obj.Position.Z);
            }

            foreach (string o_name in overlays.Keys)
                OverlayUpdate(o_name);
        }

        // for texture edit only
        public void RebuildTerrainTexture()
        {
            byte[] tex_data = hmap.tile_data;
            int map_size = hmap.width;
            if (!visible)
                return;

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

                    if ((i + j) % 2 == 0)
                    {
                        // left triangle
                        triangle_mats = new Vector3(material_id[i * (size + 1) + j], material_id[(i + 1) * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j]);
                        texture_id[t + 0] = triangle_mats;
                        texture_id[t + 1] = triangle_mats;
                        texture_id[t + 2] = triangle_mats;

                        // right triangle
                        triangle_mats = new Vector3(material_id[i * (size + 1) + j], material_id[i * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j + 1]);
                        texture_id[t + 3] = triangle_mats;
                        texture_id[t + 4] = triangle_mats;
                        texture_id[t + 5] = triangle_mats;
                    }
                    else
                    {
                        // left triangle
                        triangle_mats = new Vector3(material_id[i * (size + 1) + j], material_id[i * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j]);
                        texture_id[t + 0] = triangle_mats;
                        texture_id[t + 1] = triangle_mats;
                        texture_id[t + 2] = triangle_mats;
                        // right triangle
                        triangle_mats = new Vector3(material_id[i * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j + 1], material_id[(i + 1) * (size + 1) + j]);
                        texture_id[t + 3] = triangle_mats;
                        texture_id[t + 4] = triangle_mats;
                        texture_id[t + 5] = triangle_mats;
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

            GL.BindVertexArray(0);
        }
        
        struct SFMapLakeCellInfo
        {
            public byte lake_id;          // lake id (each lake has uniquely assigned id)
            public short lake_type;       // lake type (water, lava, swamp, ice)
            public float lake_height;    // height of the lake at particular cell
        }

        // for lake edit only
        public void RebuildLake()
        {
            if (!visible)
                return;
            // these are lake ids... different lake ids != different lake types
            // one material dedicated to each lake type on the chunk!
            // LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFMapHeightMap.RebuildLake() called, lake name LAKE_" + ix.ToString() + "_" + iy.ToString());
            Dictionary<SFCoord, SFMapLakeCellInfo> lake_info = new Dictionary<SFCoord, SFMapLakeCellInfo>();
            Dictionary<short, int> lake_types = new Dictionary<short, int>();

            int chunk_count = hmap.width / width;
            int row_start = (chunk_count - iy - 1) * height;
            int col_start = ix * width;

            for (int i = 0; i < height; i++)
                for(int j = 0; j < width; j++)
                {
                    SFCoord pos = new SFCoord(j, width-i-1);
                    SFMapLakeCellInfo lc_info = new SFMapLakeCellInfo();
                    int index = ((row_start + i) * hmap.width) + col_start + j;
                    byte lake_index = hmap.lake_data[index];
                    if(lake_index != 0)
                    {
                        SFMapLake lake_object = hmap.map.lake_manager.lakes[lake_index - 1];
                        lc_info.lake_id = lake_index;
                        lc_info.lake_type = (short)lake_object.type;
                        if (!lake_types.ContainsKey(lc_info.lake_type))
                            lake_types.Add(lc_info.lake_type, 1);
                        else
                            lake_types[lc_info.lake_type] += 1;
                        lc_info.lake_height = ((hmap.GetZ(lake_object.start) + lake_object.z_diff)) / 100.0f;
                        lake_info.Add(pos, lc_info);
                    }
                }
            
            if (lake_model != null)
            {
                SFResources.SFResourceManager.Models.Dispose(lake_model.GetName());
                lake_model = null;
            }
            
            int total_cell_count = 0;
            foreach (short t in lake_types.Keys)
                total_cell_count += lake_types[t];
            if (total_cell_count == 0)
                return;

            // generate geometry
            Vector3[] vertices = new Vector3[total_cell_count * 4];
            Vector2[] uvs = new Vector2[total_cell_count * 4];
            Vector4[] colors = new Vector4[total_cell_count * 4];
            Vector3[] normals = new Vector3[total_cell_count * 4];
            uint[] indices = new uint[total_cell_count * 6];
            SFMaterial[] materials = new SFMaterial[lake_types.Count];

            int k = 0;
            int mat_index = 0;
            foreach (short t in lake_types.Keys)
            {
                // generate geometry for each lake type
                foreach (SFCoord pos in lake_info.Keys)
                {
                    SFMapLakeCellInfo lc_info = lake_info[pos];
                    if (lc_info.lake_type != t)
                        continue;

                    float real_z = lc_info.lake_height;

                    vertices[k * 4 + 0] = new Vector3((float)(pos.x) - 0.5f, real_z, (float)(pos.y) - 0.5f);
                    vertices[k * 4 + 1] = new Vector3((float)(pos.x + 1) - 0.5f, real_z,  (float)(pos.y) - 0.5f);
                    vertices[k * 4 + 2] = new Vector3((float)(pos.x) - 0.5f, real_z,  (float)(pos.y + 1) - 0.5f);
                    vertices[k * 4 + 3] = new Vector3((float)(pos.x + 1) - 0.5f, real_z, (float)(pos.y + 1) - 0.5f);
                    uvs[k * 4 + 0] = new Vector2(pos.x / 4.0f, pos.y / 4.0f);
                    uvs[k * 4 + 1] = new Vector2((pos.x + 1) / 4.0f, pos.y / 4.0f);
                    uvs[k * 4 + 2] = new Vector2(pos.x / 4.0f, (pos.y + 1) / 4.0f);
                    uvs[k * 4 + 3] = new Vector2((pos.x + 1) / 4.0f, (pos.y + 1) / 4.0f);
                    colors[k * 4 + 0] = new Vector4(1, 1, 1, 1);
                    colors[k * 4 + 1] = new Vector4(1, 1, 1, 1);
                    colors[k * 4 + 2] = new Vector4(1, 1, 1, 1);
                    colors[k * 4 + 3] = new Vector4(1, 1, 1, 1);
                    normals[k * 4 + 0] = new Vector3(0, 1, 0);
                    normals[k * 4 + 1] = new Vector3(0, 1, 0);
                    normals[k * 4 + 2] = new Vector3(0, 1, 0);
                    normals[k * 4 + 3] = new Vector3(0, 1, 0);
                    indices[k * 6 + 0] = (uint)(k * 4 + 0);
                    indices[k * 6 + 1] = (uint)(k * 4 + 1);
                    indices[k * 6 + 2] = (uint)(k * 4 + 2);
                    indices[k * 6 + 3] = (uint)(k * 4 + 1);
                    indices[k * 6 + 4] = (uint)(k * 4 + 2);
                    indices[k * 6 + 5] = (uint)(k * 4 + 3);

                    k += 1;
                }
                // generate material for this geometry
                materials[mat_index] = new SFMaterial();
                materials[mat_index].indexStart = (uint)((k - lake_types[t]) * 6);
                materials[mat_index].indexCount = (uint)(lake_types[t] * 6);

                string tex_name = hmap.map.lake_manager.GetLakeTextureName(t);
                SFTexture tex = null;
                int tex_code = SFResources.SFResourceManager.Textures.Load(tex_name);
                if ((tex_code != 0) && (tex_code != -1))
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFMapLake.Generate(): Could not load texture (texture name = " + tex_name + ")");
                else
                {
                    tex = SFResources.SFResourceManager.Textures.Get(tex_name);
                    tex.FreeMemory();
                }
                materials[mat_index].texture = tex;

                mat_index += 1;
            }

            lake_model = new SFModel3D();
            lake_model.CreateRaw(vertices, uvs, colors, normals, indices, materials);
            SFResources.SFResourceManager.Models.AddManually(lake_model, "LAKE_" + ix.ToString() + "_" + iy.ToString());
        }

        // only happens after visibility actually changes
        public void UpdateVisible(bool vis)
        {
            if(visible)
            {
                if (!vis)
                {
                    visible = false;

                    Degenerate();

                    if (lake_model != null)
                    {
                        SFResources.SFResourceManager.Models.Dispose(lake_model.GetName());
                        lake_model = null;
                    }

                    foreach (MapEdit.MapOverlayChunk ov in overlays.Values)
                        ov.Dispose();
                }
            }
            else
            {
                if (vis)
                {
                    visible = true;
                    Generate();
                    RebuildLake();
                    foreach (string o_name in overlays.Keys)
                        OverlayUpdate(o_name);
                }
            }
        }

        public void Unload()
        {
            if (vertex_array == -1)
                return;

            GL.DeleteBuffer(position_buffer);
            GL.DeleteBuffer(uv_buffer);
            GL.DeleteBuffer(normal_buffer);
            GL.DeleteBuffer(tex_id_buffer);
            GL.DeleteVertexArray(vertex_array);

            if(lake_model != null)
            {
                SFResources.SFResourceManager.Models.Dispose(lake_model.GetName());
                lake_model = null;
            }

            foreach (MapEdit.MapOverlayChunk ov in overlays.Values)
                ov.Dispose();

            units.Clear();
            objects.Clear();
            buildings.Clear();
            decorations.Clear();
            int_objects.Clear();
            portals.Clear();
            overlays.Clear();

            hmap = null;
            owner = null;
            material_id = null;
            vertices = null;
            normals = null;
            uvs = null;
            texture_id = null;
            collision_cache = null;

            vertex_array = -1;
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
            if (!visible)
                return;
            overlays[o_name].Update(this, o_name);
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
        public ushort[] height_data;
        public byte[] tile_data;
        public byte[] lake_data;    // 0 - no lake, 1-255 - lakes 0-254
        public ushort[] building_data;   // 0 - no building, 1-65535 - buildings 0-65534
        public bool[] temporary_mask;   // for calculating islands by height
        public List<SFCoord> chunk42_data = new List<SFCoord>();
        public List<SFCoord> chunk56_data = new List<SFCoord>();
        public List<SFMapChunk60Data> chunk60_data = new List<SFMapChunk60Data>();

        public int chunk_size { get; private set; }
        public SF3D.SceneSynchro.SceneNodeMapChunk[] chunk_nodes { get; private set; }
        public List<SF3D.SceneSynchro.SceneNodeMapChunk> visible_chunks = new List<SF3D.SceneSynchro.SceneNodeMapChunk>();

        public List<string> visible_overlays { get; private set; } = new List<string>();

        public SFMapHeightMap(int w, int h)
        {
            width = w;
            height = h;
            height_data = new ushort[w * h];
            tile_data = new byte[w * h];
            lake_data = new byte[w * h];  lake_data.Initialize();
            building_data = new ushort[w * h]; building_data.Initialize();
            temporary_mask = new bool[w * h];
        }

        public SFMapHeightMapChunk GetChunk(SFCoord pos)
        {
            int chunk_count_y = height / chunk_size;
            return chunk_nodes[(chunk_count_y * ((height - pos.y - 1) / chunk_size) + (pos.x / chunk_size))].MapChunk;
        }

        public SF3D.SceneSynchro.SceneNodeMapChunk GetChunkNode(SFCoord pos)
        {
            int chunk_count_y = height / chunk_size;
            return chunk_nodes[(chunk_count_y * ((height - pos.y - 1) / chunk_size) + (pos.x / chunk_size))];
        }

        // takes into account that each object on the map is bound to a chunk
        public Vector3 GetFixedPosition(SFCoord pos)
        {
            return new Vector3(pos.x % chunk_size, GetZ(pos)/100.0f, (height - pos.y - 1) % chunk_size);
        }

        public void SetRowRaw(int row, byte[] chunk_data)
        {
            for(int i = 0; i < width; i++)
                height_data[row * width + i] = BitConverter.ToUInt16(chunk_data, i * 2);
        }

        public void GetRowRaw(int row, ref ushort[] chunk_data)
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
            chunk_nodes = new SF3D.SceneSynchro.SceneNodeMapChunk[chunk_count_x * chunk_count_y];
            for(int i = 0; i < chunk_count_y; i++)
                for(int j = 0; j < chunk_count_x; j++)
                {
                    chunk_nodes[i * chunk_count_x + j] = new SF3D.SceneSynchro.SceneNodeMapChunk("hmap_" + j.ToString() + "_" + i.ToString());
                    SF3D.SceneSynchro.SceneNodeMapChunk chunk_node = chunk_nodes[i * chunk_count_x + j];
                    chunk_node.Visible = false;
                    chunk_node.MapChunk = new SFMapHeightMapChunk();
                    chunk_node.MapChunk.hmap = this;
                    chunk_node.MapChunk.owner = chunk_nodes[i * chunk_count_x + j];
                    chunk_node.MapChunk.ix = j;
                    chunk_node.MapChunk.iy = i;
                    chunk_node.MapChunk.width = chunk_size;
                    chunk_node.MapChunk.height = chunk_size;
                    chunk_node.MapChunk.GenerateTemporaryAABB();

                    chunk_node.MapChunk.Generate();
                    chunk_node.Position = new Vector3(j * chunk_size, 0, i * chunk_size);
                }
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapHeightMap.Generate(): Chunks generated: "+chunk_nodes.Length.ToString());
        }

        public ushort GetZ(SFCoord pos)
        {
            return height_data[pos.y * width + pos.x];
        }

        private bool FitsInMap(SFCoord p)
        {
            return ((p.x >= 0) && (p.x < width) && (p.y >= 0) && (p.y < height));
        }

        public float GetRealZ(Vector2 pos)
        {
            short left = (short)pos.X;
            short top = (short)(height-pos.Y-1);
            float tx = pos.X - left;
            float ty = (height-pos.Y-1) - top;

            ushort[] val = new ushort[] { 0, 0, 0, 0 };
            SFCoord p = new SFCoord(left, top);   // top left
            if (FitsInMap(p))
                val[0] = GetZ(p);
            p = new SFCoord(left+1, top);   // top right
            if (FitsInMap(p))
                val[1] = GetZ(p);
            p = new SFCoord(left, top+1);   // bottom left
            if (FitsInMap(p))
                val[2] = GetZ(p);
            p = new SFCoord(left+1, top+1);   // bottom right
            if (FitsInMap(p))
                val[3] = GetZ(p);

            return Utility.BilinearInterpolation(val[0], val[1], val[2], val[3], tx, ty)/100.0f;
        }

        public byte GetTile(SFCoord pos)
        {
            return tile_data[pos.y * width + pos.x];
        }

        // an ugly hack... todo: move it to texture manager
        public byte GetTileFixed(SFCoord pos)
        {
            byte b = tile_data[pos.y * width + pos.x];
            return (b > 223 ? (byte)(b - 223) : b);
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

            ushort start_z = GetZ(start);
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
                for (int j = topchunky; j <= botchunky; j++)
                    chunk_nodes[j * chunk_count_x + i].MapChunk.RebuildGeometry();
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
                for (int j = topchunky; j <= botchunky; j++)
                    chunk_nodes[j * chunk_count_x + i].MapChunk.RebuildTerrainTexture();
        }

        public void OverlayCreate(string o_name, Vector4 col)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapHeightMap.OverlayCreate() called, overlay name: "+o_name);

            foreach (SF3D.SceneSynchro.SceneNodeMapChunk chunk in chunk_nodes)
            {
                chunk.MapChunk.overlays.Add(o_name, new MapEdit.MapOverlayChunk());
                chunk.MapChunk.overlays[o_name].color = col;
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
            foreach (SF3D.SceneSynchro.SceneNodeMapChunk chunk in chunk_nodes)
                if (chunk.MapChunk.overlays[o_name].points.Count != 0)
                {
                    chunk.MapChunk.overlays[o_name].points.Clear();
                    chunk.MapChunk.OverlayUpdate(o_name);
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
                for (int j = topchunky; j <= botchunky; j++)
                    chunk_nodes[j * chunk_count_x + i].MapChunk.OverlayUpdate(o_name);
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

        public List<SF3D.SceneSynchro.SceneNodeMapChunk> GetAreaMapNodes(HashSet<SFCoord> points)
        {
            List<SF3D.SceneSynchro.SceneNodeMapChunk> list = new List<SF3D.SceneSynchro.SceneNodeMapChunk>();
            foreach(SFCoord p in points)
            {
                SF3D.SceneSynchro.SceneNodeMapChunk node = GetChunkNode(p);
                if (!list.Contains(node))
                    list.Add(node);
            }

            return list;
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

        // https://www.gamedev.net/forums/topic/163625-fast-way-to-calculate-heightmap-normals/
        public Vector3 GetVertexNormal(int x, int y)
        {
            float hscale = 100.0f;
            float az = (x < width - 1) ? (GetHeightAt(x + 1, y)) : (0);
            float bz = (y < height - 1) ? (GetHeightAt(x, y + 1)) : (0);
            float cz = (x > 0) ? (GetHeightAt(x - 1, y)) : (0);
            float dz = (y > 0) ? (GetHeightAt(x, y - 1)) : (0);

            return (new Vector3(cz - az, 2 * hscale, dz - bz)).Normalized();
        }

        public ushort GetHeightAt(int x, int y)
        {
            int pos = (y * width) + x;
            if ((pos < 0) || (pos >= height_data.Length))
                return 0;
            return height_data[pos];
        }

        public void Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapHeightMap.Unload() called");
            if (map == null)
                return;

            if (texture_manager != null)
                texture_manager.Unload();
            if (chunk_nodes != null)
                foreach (SF3D.SceneSynchro.SceneNodeMapChunk chunk in chunk_nodes)
                    SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(chunk);

            chunk42_data.Clear();
            chunk56_data.Clear();
            chunk60_data.Clear();
            visible_chunks.Clear();
            visible_overlays.Clear();
            chunk_nodes = null;
            map = null;
        }
    }
}
