using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpellforceDataEditor.SF3D;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace SpellforceDataEditor.SFMap
{
    // represents pool of physical heightmap chunks
    // each heightmap chunk points to exactly one physical heightmap chunk, for rendering purposes
    public class SFMapHeightMapGeometryPool
    {
        public const int POOL_SIZE = 768;          // how many heightmap chunks are at most allowed
        public const int CHUNK_SIZE = 16;          // width/height of one chunk

        // opengl VAO and VBOs
        public int vertex_array = -1;
        public int position_buffer, normal_buffer, element_buffer;

        // geometry data for all chunks is found in one buffer per attribute
        public Vector3[] vertices_pool = new Vector3[POOL_SIZE * (CHUNK_SIZE + 1) * (CHUNK_SIZE + 1)];    // vec3
        public Vector3[] normals_pool = new Vector3[POOL_SIZE * (CHUNK_SIZE + 1) * (CHUNK_SIZE + 1)];     // vec3
        public ushort[] indices_pool = new ushort[6 * POOL_SIZE * (CHUNK_SIZE * CHUNK_SIZE)];       // ushort, 6 per quad
        // each chunk would use the same indices, so here's a copy of those for quick memory copy operation
        public ushort[] indices_base = new ushort[6 * (CHUNK_SIZE * CHUNK_SIZE)];
        // a chunk is only rendered if it's active per this array
        public bool[] active = new bool[POOL_SIZE];

        public int first_unused = 0;
        public int last_used = -1;
        public int used_count = 0;

        public SFMapHeightMapGeometryPool()
        {
            // generate indices base
            for (uint i = 0; i < CHUNK_SIZE; i++)
                for (uint j = 0; j < CHUNK_SIZE; j++)
                {
                    indices_base[6 * (i * CHUNK_SIZE + j) + 0] = (ushort)(i * (CHUNK_SIZE + 1) + j);
                    indices_base[6 * (i * CHUNK_SIZE + j) + 1] = (ushort)(i * (CHUNK_SIZE + 1) + j + 1);
                    indices_base[6 * (i * CHUNK_SIZE + j) + 2] = (ushort)(i * (CHUNK_SIZE + 1) + j + (CHUNK_SIZE + 1));
                    indices_base[6 * (i * CHUNK_SIZE + j) + 3] = (ushort)(i * (CHUNK_SIZE + 1) + j + 1);
                    indices_base[6 * (i * CHUNK_SIZE + j) + 4] = (ushort)(i * (CHUNK_SIZE + 1) + j + (CHUNK_SIZE + 2));
                    indices_base[6 * (i * CHUNK_SIZE + j) + 5] = (ushort)(i * (CHUNK_SIZE + 1) + j + (CHUNK_SIZE + 1));
                }

            for (int i = 0; i < POOL_SIZE; i++)
            {
                active[i] = false;
                // indices generated only once per run
                Array.Copy(indices_base, 0, indices_pool, 6 * i * (CHUNK_SIZE * CHUNK_SIZE), 6 * (CHUNK_SIZE * CHUNK_SIZE));
            }

            Init();
        }

        public void Init()
        {
            if (vertex_array == -1)
                vertex_array = GL.GenVertexArray();
            position_buffer = GL.GenBuffer();
            normal_buffer = GL.GenBuffer();
            element_buffer = GL.GenBuffer();

            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, position_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices_pool.Length * 12, vertices_pool, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, normal_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, normals_pool.Length * 12, normals_pool, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);
            GL.BufferData<ushort>(BufferTarget.ElementArrayBuffer, indices_pool.Length * 2, indices_pool, BufferUsageHint.DynamicDraw);

            GL.BindVertexArray(0);
        }

        // creates new chunk and returns its index in the pool
        // arguments: heightmap, chunk xoffset, chunk yoffset (offsets are multiplied by CHUNK_SIZE)
        // will fail if there's no room for a new chunk
        public int BuildNewChunk(SFMapHeightMap hmap, int ix, int iy)
        {
            if (used_count == POOL_SIZE)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapHeightMapGeometryPool.BuildNewChunk(): Pool is closed!");
                throw new ArgumentOutOfRangeException("SFMapHeightMapGeometryPool.BuildNewChunk(): No room to allocate next chunk!");
            }

            int ret = first_unused;
            for (int i = first_unused + 1; i < POOL_SIZE; i++)
                if (!active[i])
                {
                    first_unused = i;
                    break;
                }

            if (last_used < ret)
                last_used = ret;

            active[ret] = true;
            used_count += 1;

            //System.Diagnostics.Debug.WriteLine("IX " + ix.ToString() + " | IY " + iy.ToString() + " | RET " + ret.ToString());

            // generate geometry for the chunk
            UpdateChunk(ret, hmap, ix, iy);

            return ret;
        }

        // updates chunk geometry (both CPU- and GPU-based buffers)
        // arguments: chunk index in the pool, heightmap, chunk xoffset, chunk yoffset (offsets are multiplied by CHUNK_SIZE)
        public void UpdateChunk(int offset, SFMapHeightMap hmap, int ix, int iy)
        {
            float flatten_factor = 100.0f;

            int map_size = hmap.width;
            int chunk_count = map_size / CHUNK_SIZE;
            int row_start = (chunk_count - iy - 1) * CHUNK_SIZE;
            int col_start = ix * CHUNK_SIZE;

            byte[] tex_data = hmap.tile_data;

            for (int i = 0; i < (CHUNK_SIZE+1); i++)
            {
                for (int j = 0; j < (CHUNK_SIZE+1); j++)
                {
                    vertices_pool[(CHUNK_SIZE + 1) * (CHUNK_SIZE + 1) * offset + (CHUNK_SIZE + 1) * i + j] = new Vector3(
                        col_start + j, 
                        hmap.GetHeightAt(col_start + j, row_start + i) / flatten_factor, 
                        map_size - (row_start + i) - 1);

                    normals_pool[(CHUNK_SIZE + 1) * (CHUNK_SIZE + 1) * offset + (CHUNK_SIZE + 1) * i + j] = 
                        hmap.GetVertexNormal(col_start + j, row_start + i);
                }
            }

            // buffer subdata
            GL.BindBuffer(BufferTarget.ArrayBuffer, position_buffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 
                new IntPtr(12 * (CHUNK_SIZE + 1) * (CHUNK_SIZE + 1) * offset), 
                12 * (CHUNK_SIZE + 1) * (CHUNK_SIZE + 1),
                ref vertices_pool[(CHUNK_SIZE + 1) * (CHUNK_SIZE + 1) * offset]);

            GL.BindBuffer(BufferTarget.ArrayBuffer, normal_buffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 
                new IntPtr(12 * (CHUNK_SIZE + 1) * (CHUNK_SIZE + 1) * offset),
                12 * (CHUNK_SIZE + 1) * (CHUNK_SIZE + 1),
                ref normals_pool[(CHUNK_SIZE + 1) * (CHUNK_SIZE + 1) * offset]);
        }

        // frees chunk from use
        // note that this doesn't free the memory, only marks it as unused, so it can still be used as valid chunk rendering-wise
        public void FreeChunk(int offset)
        {
            if (active[offset])
            {
                active[offset] = false;
                used_count -= 1;
                if (offset < first_unused)
                    first_unused = offset;
                if (offset == last_used)
                    for (int i = last_used - 1; i >= 0; i--)
                        if (active[offset])
                        {
                            last_used = i;
                            break;
                        }
                if (used_count == 0)
                    last_used = -1;
            }
        }

        // frees VAO and VBOs
        public void Unload()
        {
            if (vertex_array != -1)
            {
                GL.DeleteBuffer(position_buffer);
                GL.DeleteBuffer(normal_buffer);
                GL.DeleteBuffer(element_buffer);
                GL.DeleteVertexArray(vertex_array);
                vertex_array = -1;
            }
        }
    }


    public class SFMapHeightMapChunk
    {
        public SFMapHeightMap hmap = null;
        public SF3D.SceneSynchro.SceneNodeMapChunk owner = null;
        public int width, height;
        public int id, ix, iy;
        public bool visible = false;
        public bool decoration_visible = false;
        public bool unit_visible = false;

        // heightmap
        public int pool_index = -1;
        public SF3D.Physics.BoundingBox aabb;

        // lake
        public SFModel3D lake_model = null;    // generated here, but owned by ResourceManager

        // all entities on map are stored per chunk
        public List<SFMapBuilding> buildings = new List<SFMapBuilding>();
        public List<SFMapObject> objects = new List<SFMapObject>();
        public List<SFMapInteractiveObject> int_objects = new List<SFMapInteractiveObject>();
        public List<SFMapUnit> units = new List<SFMapUnit>();
        public List<SFMapPortal> portals = new List<SFMapPortal>();
        public List<SFMapDecoration> decorations = new List<SFMapDecoration>();

        // generates heightmap chunk (allocates physical chunk and turns stuff visible)
        public void Generate()
        {
            if (!visible)
                return;

            Degenerate();

            pool_index = hmap.geometry_pool.BuildNewChunk(hmap, ix, iy);

            GenerateAABB();

            RebuildLake();
            UpdateSettingsVisible();
        }

        // only used when there's no physical heightmap chunk associated with this chunk
        private void GenerateTemporaryAABB()
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
            aabb = new SF3D.Physics.BoundingBox(new Vector3(ix * size, 0, iy * size), new Vector3((ix + 1) * size, max_height / 100.0f, (iy * size) + size));
        }

        // generates bounding box for the heightmap
        // bounding box is used for chunk visibility calculations, and for ray collision
        public void GenerateAABB()
        {
            if(pool_index == -1)
            {
                GenerateTemporaryAABB();
                return;
            }

            int size = width;

            int o1 = SFMapHeightMapGeometryPool.CHUNK_SIZE + 1;
            int o2 = o1 * o1 * pool_index;

            float y1, y2;
            y1 = 10000;
            y2 = -10000;
            for (int i = 0; i < (SFMapHeightMapGeometryPool.CHUNK_SIZE + 1) * (SFMapHeightMapGeometryPool.CHUNK_SIZE + 1); i++)
            {
                Vector3 v = hmap.geometry_pool.vertices_pool[i + o2];
                y1 = Math.Min(y1, v.Y);
                y2 = Math.Max(y2, v.Y);
            }
            aabb = new SF3D.Physics.BoundingBox(new Vector3(ix * size, y1, iy * size), new Vector3((ix + 1) * size, y2, (iy * size) + size));
        }

        // frees physical heightmap chunk from use, and destroys lake model associated with this chunk
        public void Degenerate()
        {
            if (pool_index != -1)
            {
                hmap.geometry_pool.FreeChunk(pool_index);
                pool_index = -1;
            }

            if (lake_model != null)
            {
                SFResources.SFResourceManager.Models.Dispose(lake_model.GetName());
                lake_model = null;
            }
        }

        // updates physical heightmap chunk, fixes entity positions
        // called by heightmap when a chunk is modified
        public void RebuildGeometry()
        {
            if (!visible)
                return;

            hmap.geometry_pool.UpdateChunk(pool_index, hmap, ix, iy);
            GenerateAABB();


            // fix all object positions (without lakes for now...)
            foreach (SFMapUnit u in units)
            {
                SF3D.SceneSynchro.SceneNode _obj = owner.FindNode<SF3D.SceneSynchro.SceneNode>(u.GetName());
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(u.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapObject o in objects)
            {
                SF3D.SceneSynchro.SceneNode _obj = owner.FindNode<SF3D.SceneSynchro.SceneNode>(o.GetName());
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(o.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapInteractiveObject io in int_objects)
            {
                SF3D.SceneSynchro.SceneNode _obj = owner.FindNode<SF3D.SceneSynchro.SceneNode>(io.GetName());
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(io.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapDecoration d in decorations)
            {
                SF3D.SceneSynchro.SceneNode _obj = owner.FindNode<SF3D.SceneSynchro.SceneNode>(d.GetObjectName());
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(d.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapBuilding b in buildings)
            {
                SF3D.SceneSynchro.SceneNode _obj = owner.FindNode<SF3D.SceneSynchro.SceneNode>(b.GetName());
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(b.grid_position) / 100.0f, _obj.Position.Z);
            }
            foreach (SFMapPortal p in portals)
            {
                SF3D.SceneSynchro.SceneNode _obj = owner.FindNode<SF3D.SceneSynchro.SceneNode>(p.GetName());
                _obj.Position = new Vector3(_obj.Position.X, hmap.GetZ(p.grid_position) / 100.0f, _obj.Position.Z);
            }
        }

        struct SFMapLakeCellInfo
        {
            public byte lake_id;          // lake id (each lake has uniquely assigned id)
            public short lake_type;       // lake type (water, lava, swamp, ice)
            public float lake_height;    // height of the lake at particular cell
        }

        // updates lake model in correspondence with lake data for this chunk
        public void RebuildLake()
        {
            if (!visible)
                return;
            // these are lake ids... different lake ids != different lake types
            // one material dedicated to each lake type on the chunk!
            Dictionary<SFCoord, SFMapLakeCellInfo> lake_info = new Dictionary<SFCoord, SFMapLakeCellInfo>();
            Dictionary<short, int> lake_types = new Dictionary<short, int>();

            int chunk_count = hmap.width / width;
            int row_start = (chunk_count - iy - 1) * height;
            int col_start = ix * width;

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    SFCoord pos = new SFCoord(j, width - i - 1);
                    SFMapLakeCellInfo lc_info = new SFMapLakeCellInfo();
                    int index = ((row_start + i) * hmap.width) + col_start + j;
                    byte lake_index = hmap.lake_data[index];
                    if (lake_index != 0)
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

            // generate submodels
            SFSubModel3D[] submodels = new SFSubModel3D[lake_types.Count];

            int submodel_index = 0;
            foreach (short t in lake_types.Keys)
            {
                int k = 0;
                Vector3[] vertices = new Vector3[lake_info.Keys.Count * 4];
                Vector2[] uvs = new Vector2[lake_info.Keys.Count * 4];
                Vector4[] colors = new Vector4[lake_info.Keys.Count * 4];
                Vector3[] normals = new Vector3[lake_info.Keys.Count * 4];
                uint[] indices = new uint[lake_info.Keys.Count * 6];
                // generate geometry for each lake type
                foreach (SFCoord pos in lake_info.Keys)
                {
                    SFMapLakeCellInfo lc_info = lake_info[pos];
                    if (lc_info.lake_type != t)
                        continue;

                    float real_z = lc_info.lake_height;

                    vertices[k * 4 + 0] = new Vector3((float)(pos.x) - 0.5f, real_z, (float)(pos.y) - 0.5f);
                    vertices[k * 4 + 1] = new Vector3((float)(pos.x + 1) - 0.5f, real_z, (float)(pos.y) - 0.5f);
                    vertices[k * 4 + 2] = new Vector3((float)(pos.x) - 0.5f, real_z, (float)(pos.y + 1) - 0.5f);
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
                SFMaterial material = new SFMaterial();
                material.indexStart = (uint)0;
                material.indexCount = (uint)(lake_info.Keys.Count * 6);

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
                material.texture = tex;

                SFSubModel3D sbm = new SFSubModel3D();
                sbm.CreateRaw(vertices, uvs, colors, normals, indices, material);
                sbm.instance_matrices.AddElem(owner.ResultTransform);
                submodels[submodel_index] = sbm;
                submodel_index += 1;
            }

            lake_model = new SFModel3D();
            lake_model.CreateRaw(submodels);
            SFResources.SFResourceManager.Models.AddManually(lake_model, "LAKE_" + ix.ToString() + "_" + iy.ToString());
        }

        public void UpdateVisible(bool vis)
        {
            if (visible)
            {
                if (!vis)
                {
                    // visibility switches from visible to invisible
                    visible = false;

                    Degenerate();
                }
            }
            else
            {
                if (vis)
                {
                    // visibility switches from invisible to visible
                    visible = true;
                    decoration_visible = true;

                    owner.Update(0);                  // sets model matrix, needed for lake generation

                    Generate();
                }
            }
        }

        public void UpdateDecorationVisible(float camera_dist, float camera_hdiff)
        {
            if (decoration_visible)
            {
                if ((camera_dist > 71) || (camera_hdiff > 81))  // magic number...
                {
                    decoration_visible = false;
                    foreach (SFMapDecoration d in decorations)
                        owner.FindNode<SF3D.SceneSynchro.SceneNode>(d.GetObjectName()).Visible = false;
                }
            }
            else
            {
                if ((camera_dist <= 71) && (camera_hdiff <= 81))
                {
                    decoration_visible = true;
                    if (Settings.DecorationsVisible)
                        foreach (SFMapDecoration d in decorations)
                            owner.FindNode<SF3D.SceneSynchro.SceneNode>(d.GetObjectName()).Visible = true;
                }
            }
        }

        public void UpdateUnitVisible(float camera_dist, float camera_hdiff)
        {
            if (unit_visible)
            {
                if ((camera_dist > 91) || (camera_hdiff > 104))  // magic number...
                {
                    unit_visible = false;
                    foreach (SFMapUnit u in units)
                    {
                        var node = owner.FindNode<SF3D.SceneSynchro.SceneNode>(u.GetName());
                        node.FindNode<SF3D.SceneSynchro.SceneNode>("Unit").Visible = false;
                        node.FindNode<SF3D.SceneSynchro.SceneNodeSimple>("Billboard").Visible = true;
                    }
                }
            }
            else
            {
                if ((camera_dist <= 91) && (camera_hdiff <= 104))
                {
                    unit_visible = true;
                    if (Settings.DecorationsVisible)
                        foreach (SFMapUnit u in units)
                        {
                            var node = owner.FindNode<SF3D.SceneSynchro.SceneNode>(u.GetName());
                            node.FindNode<SF3D.SceneSynchro.SceneNode>("Unit").Visible = true;
                            node.FindNode<SF3D.SceneSynchro.SceneNodeSimple>("Billboard").Visible = false;
                        }
                }
            }
        }

        public void UpdateSettingsVisible()
        {
            bool vis1 = Settings.UnitsVisible & unit_visible;
            bool vis2 = Settings.UnitsVisible & (!unit_visible);
            foreach (SFMapUnit u in units)
            {
                var node = owner.FindNode<SF3D.SceneSynchro.SceneNode>(u.GetName());
                node.Visible = true;
                node.FindNode<SF3D.SceneSynchro.SceneNode>("Unit").Visible = vis1;
                node.FindNode<SF3D.SceneSynchro.SceneNodeSimple>("Billboard").Visible = vis2;
            }

            foreach (SFMapBuilding b in buildings)
                owner.FindNode<SF3D.SceneSynchro.SceneNode>(b.GetName()).Visible = Settings.BuildingsVisible;
            foreach (SFMapObject o in objects)
                owner.FindNode<SF3D.SceneSynchro.SceneNode>(o.GetName()).Visible = Settings.ObjectsVisible;
            foreach (SFMapInteractiveObject io in int_objects)
                owner.FindNode<SF3D.SceneSynchro.SceneNode>(io.GetName()).Visible = Settings.ObjectsVisible;
            foreach (SFMapPortal p in portals)
                owner.FindNode<SF3D.SceneSynchro.SceneNode>(p.GetName()).Visible = Settings.ObjectsVisible;

            if ((Settings.DecorationsVisible) && (decoration_visible))
                foreach (SFMapDecoration d in decorations)
                    owner.FindNode<SF3D.SceneSynchro.SceneNode>(d.GetObjectName()).Visible = Settings.DecorationsVisible;
            if (!Settings.DecorationsVisible)
                foreach (SFMapDecoration d in decorations)
                    owner.FindNode<SF3D.SceneSynchro.SceneNode>(d.GetObjectName()).Visible = Settings.DecorationsVisible;

        }

        public void Unload()
        {
            if (pool_index != -1)
            {
                hmap.geometry_pool.FreeChunk(pool_index);
                pool_index = -1;
            }

            if (lake_model != null)
            {
                SFResources.SFResourceManager.Models.Dispose(lake_model.GetName());
                lake_model = null;
            }

            units.Clear();
            objects.Clear();
            buildings.Clear();
            decorations.Clear();
            int_objects.Clear();
            portals.Clear();

            hmap = null;
            owner = null;
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
        public SFMapHeightMapGeometryPool geometry_pool { get; private set; } = new SFMapHeightMapGeometryPool();
        public int width, height;
        public ushort[] height_data;
        public byte[] tile_data;
        public byte[] lake_data;    // 0 - no lake, 1-255 - lakes 0-254
        public ushort[] building_data;   // 0 - no building, 1-65535 - buildings 0-65534
        public bool[] temporary_mask;   // for calculating islands by height
        public List<SFCoord> chunk42_data = new List<SFCoord>();
        public List<SFCoord> chunk56_data = new List<SFCoord>();
        public List<SFMapChunk60Data> chunk60_data = new List<SFMapChunk60Data>();
        
        public SF3D.SceneSynchro.SceneNodeMapChunk[] chunk_nodes { get; private set; }
        public List<SF3D.SceneSynchro.SceneNodeMapChunk> visible_chunks = new List<SF3D.SceneSynchro.SceneNodeMapChunk>();

        // tile_data translates directly to a texture
        public int tile_data_texture = -1;

        // UBO for overlay data to the shader
        int uniformOverlays_buffer;
        Vector4[] uniformOverlays = new Vector4[16];     // 16 available colors
        public byte[] overlay_data_flags;
        public int overlay_texture_flags = -1;
        public byte[] overlay_data_decals;
        public int overlay_texture_decals = -1;
        public int overlay_active_texture = -1;

        public SFMapHeightMap(int w, int h)
        {
            width = w;
            height = h;
            height_data = new ushort[w * h];
            tile_data = new byte[w * h];
            lake_data = new byte[w * h]; lake_data.Initialize();
            building_data = new ushort[w * h]; building_data.Initialize();
            temporary_mask = new bool[w * h];
            overlay_data_flags = new byte[w * h];
            overlay_data_decals = new byte[w * h];

            tile_data_texture = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, tile_data_texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, width, height, 0,
                PixelFormat.Red, PixelType.UnsignedByte, tile_data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0);

            // create uniform buffer object for overlays
            uniformOverlays_buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, uniformOverlays_buffer);
            GL.BufferData(BufferTarget.UniformBuffer, 16 * 4 * 4, new IntPtr(0), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 1, uniformOverlays_buffer, new IntPtr(0), 16 * 4 * 4);
            SetOverlayColors();

            // overlay data
            overlay_texture_flags = GL.GenTexture();
            overlay_texture_decals = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture3);

            GL.BindTexture(TextureTarget.Texture2D, overlay_texture_flags);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, width, height, 0,
                PixelFormat.Red, PixelType.UnsignedByte, overlay_data_flags);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);

            GL.BindTexture(TextureTarget.Texture2D, overlay_texture_decals);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, width, height, 0,
                PixelFormat.Red, PixelType.UnsignedByte, overlay_data_decals);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        public void UpdateTileMap()
        {
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, tile_data_texture);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Red, PixelType.UnsignedByte, tile_data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        public void ResetFlagOverlay()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (texture_manager.texture_tiledata[tile_data[j * width + i]].blocks_movement)
                        overlay_data_flags[j * width + i] = 9;
                    else
                        overlay_data_flags[j * width + i] = 0;
                }
            }
            foreach (SFCoord p in chunk42_data)
                overlay_data_flags[p.y * width + p.x] = 2;
            foreach (SFCoord p in chunk56_data)
            {
                if (overlay_data_flags[p.y * width + p.x] == 0)
                    overlay_data_flags[p.y * width + p.x] = 5;
                else
                    overlay_data_flags[p.y * width + p.x] = 10;
            }
        }

        // refreshes currently active overlay
        public void RefreshOverlay()
        {
            if (overlay_active_texture == Utility.NO_INDEX)
                return;

            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, overlay_active_texture);
            if(overlay_active_texture == overlay_texture_flags)
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Red, PixelType.UnsignedByte, overlay_data_flags);
            else
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Red, PixelType.UnsignedByte, overlay_data_decals);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        public SFMapHeightMapChunk GetChunk(SFCoord pos)
        {
            int chunk_count_y = height / SFMapHeightMapGeometryPool.CHUNK_SIZE;
            return chunk_nodes[(chunk_count_y * ((height - pos.y - 1) / SFMapHeightMapGeometryPool.CHUNK_SIZE) + (pos.x / SFMapHeightMapGeometryPool.CHUNK_SIZE))].MapChunk;
        }

        public SF3D.SceneSynchro.SceneNodeMapChunk GetChunkNode(SFCoord pos)
        {
            int chunk_count_y = height / SFMapHeightMapGeometryPool.CHUNK_SIZE;
            return chunk_nodes[(chunk_count_y * ((height - pos.y - 1) / SFMapHeightMapGeometryPool.CHUNK_SIZE) + (pos.x / SFMapHeightMapGeometryPool.CHUNK_SIZE))];
        }

        // takes into account that each object on the map is bound to a chunk
        public Vector3 GetFixedPosition(SFCoord pos)
        {
            return new Vector3(pos.x % SFMapHeightMapGeometryPool.CHUNK_SIZE, GetZ(pos) / 100.0f, (height - pos.y - 1) % SFMapHeightMapGeometryPool.CHUNK_SIZE);
        }

        public void SetRowRaw(int row, byte[] chunk_data)
        {
            for (int i = 0; i < width; i++)
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
            UpdateTileMap();
        }

        public void Generate()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapHeightMap.Generate() called");

            int chunk_count_x = width / SFMapHeightMapGeometryPool.CHUNK_SIZE;
            int chunk_count_y = height / SFMapHeightMapGeometryPool.CHUNK_SIZE;
            chunk_nodes = new SF3D.SceneSynchro.SceneNodeMapChunk[chunk_count_x * chunk_count_y];
            for (int i = 0; i < chunk_count_y; i++)
                for (int j = 0; j < chunk_count_x; j++)
                {
                    chunk_nodes[i * chunk_count_x + j] = new SF3D.SceneSynchro.SceneNodeMapChunk("hmap_" + j.ToString() + "_" + i.ToString());
                    SF3D.SceneSynchro.SceneNodeMapChunk chunk_node = chunk_nodes[i * chunk_count_x + j];
                    chunk_node.Position = new Vector3(j * SFMapHeightMapGeometryPool.CHUNK_SIZE, 0, i * SFMapHeightMapGeometryPool.CHUNK_SIZE);
                    chunk_node.Visible = false;
                    chunk_node.MapChunk = new SFMapHeightMapChunk();
                    chunk_node.MapChunk.hmap = this;
                    chunk_node.MapChunk.owner = chunk_nodes[i * chunk_count_x + j];
                    chunk_node.MapChunk.ix = j;
                    chunk_node.MapChunk.iy = i;
                    chunk_node.MapChunk.id = i * (width / SFMapHeightMapGeometryPool.CHUNK_SIZE) + j;
                    chunk_node.MapChunk.width = SFMapHeightMapGeometryPool.CHUNK_SIZE;
                    chunk_node.MapChunk.height = SFMapHeightMapGeometryPool.CHUNK_SIZE;
                    chunk_node.MapChunk.GenerateAABB();
                }
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapHeightMap.Generate(): Chunks generated: " + chunk_nodes.Length.ToString());
        }

        public int ImportHeights(System.Drawing.Bitmap bitmap, byte step, byte offset)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapHeightMap.ImportHeights() called");
            if ((bitmap.Width > 1024) || (bitmap.Height > 1024))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapHeightMap.ImportHeights(): Invalid heightmap dimensions!");
                return -1;
            }

            int newmap_size = 256;
            if ((bitmap.Width > 256) || (bitmap.Height > 256))
                newmap_size = 512;
            if ((bitmap.Width > 512) || (bitmap.Height > 512))
                newmap_size = 1024;

            SFCoord hmap_offset = new SFCoord((width - bitmap.Width) / 2, (height - bitmap.Height) / 2);

            // if only_change_height is set to true and new map size is greater than the current one, reorder the chunks
            if (newmap_size > width)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapHeightMap.Import(): New heightmap is bigger than the current one!");
                return -3;
            }

            if (step == 0)
                step = 1;

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    if ((i >= hmap_offset.y) || (i < width - hmap_offset.y) || (j >= hmap_offset.x) || (j < height - hmap_offset.x))
                        height_data[i * width + j] = (ushort)(Math.Max((bitmap.GetPixel(
                            j - hmap_offset.x,
                            bitmap.Height - (i - hmap_offset.y) - 1).R - offset), 0) * step);
                    else
                        height_data[i * width + j] = 0;
                }

            RebuildGeometry(new SFCoord(0, 0), new SFCoord(width - 1, height - 1));

            return 0;
        }

        public int ExportHeights(System.Drawing.Bitmap bitmap, byte step, byte offset)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapHeightMap.ExportHeights() called");

            if (bitmap == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapHeightMap.ExportHeights(): Bitmap is null!");
                return -1;
            }

            if ((bitmap.Width != width) || (bitmap.Height != height))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapHeightMap.ExportHeights(): Bitmap dimensions do not match map size!");
                return -2;
            }

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    byte col = (byte)Math.Max(0,
                                        Math.Min(255,
                                            (GetZ(new SFCoord(j, i)) / step) + offset));
                    if (col <= offset)
                        col = 0;
                    bitmap.SetPixel(j, height - i - 1, System.Drawing.Color.FromArgb(col, col, col));
                }

            return 0;
        }

        // returns bounding box for given area
        public void GetBoxFromArea(IEnumerable<SFCoord> area, out SFCoord tl, out SFCoord br)
        {
            int t, l, r, b;
            if(area.Count() == 0)
            {
                tl = new SFCoord(0, 0); br = new SFCoord(0, 0); return;
            }
            t = area.First<SFCoord>().y; l = area.First<SFCoord>().x;
            b = area.First<SFCoord>().y; r = area.First<SFCoord>().x;

            foreach(SFCoord p in area)
            {
                if (p.x < l)
                    l = p.x;
                if (p.x > r)
                    r = p.x;
                if (p.y < t)
                    t = p.y;
                if (p.y > b)
                    b = p.y;
            }

            tl = new SFCoord(l, t);
            br = new SFCoord(r, b);
        }

        // returns height for given grid position (as found in map file, 0 = lowest, 65535 - highest available)
        public ushort GetZ(SFCoord pos)
        {
            return height_data[pos.y * width + pos.x];
        }

        // returns whether a posiiton is within map bounds
        private bool FitsInMap(SFCoord p)
        {
            return ((p.x >= 0) && (p.x < width) && (p.y >= 0) && (p.y < height));
        }

        // returns height for given grid position (physical height)
        public float GetRealZ(Vector2 pos)
        {
            short left = (short)pos.X;
            short top = (short)(height - pos.Y - 1);
            float tx = pos.X - left;
            float ty = (height - pos.Y - 1) - top;

            ushort[] val = new ushort[] { 0, 0, 0, 0 };
            SFCoord p = new SFCoord(left, top);   // top left
            if (FitsInMap(p))
                val[0] = GetZ(p);
            p = new SFCoord(left + 1, top);   // top right
            if (FitsInMap(p))
                val[1] = GetZ(p);
            p = new SFCoord(left, top + 1);   // bottom left
            if (FitsInMap(p))
                val[2] = GetZ(p);
            p = new SFCoord(left + 1, top + 1);   // bottom right
            if (FitsInMap(p))
                val[3] = GetZ(p);

            return Utility.BilinearInterpolation(val[0], val[1], val[2], val[3], tx, ty) / 100.0f;
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

        // helper function for GetIslandByHeight()
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

            // for each coordinate from the queue, add up to 4 neighbors to the queue if they're unchecked and fulfill the criteria
            while (to_be_checked.Count != 0)
            {
                cur_pos = to_be_checked.Dequeue();
                // every position that's in the queue will belong to an island
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

        // rebuilds heightmap chunks within the given bounding box
        public void RebuildGeometry(SFCoord topleft, SFCoord bottomright)
        {
            int chunk_count_x = width / SFMapHeightMapGeometryPool.CHUNK_SIZE;
            int chunk_count_y = height / SFMapHeightMapGeometryPool.CHUNK_SIZE;

            int topchunkx = topleft.x / SFMapHeightMapGeometryPool.CHUNK_SIZE;
            int topchunky = topleft.y / SFMapHeightMapGeometryPool.CHUNK_SIZE;
            int botchunkx = bottomright.x / SFMapHeightMapGeometryPool.CHUNK_SIZE;
            int botchunky = bottomright.y / SFMapHeightMapGeometryPool.CHUNK_SIZE;

            for (int i = topchunkx; i <= botchunkx; i++)
                for (int j = topchunky; j <= botchunky; j++)
                    chunk_nodes[(chunk_count_y - j - 1) * chunk_count_x + i].MapChunk.RebuildGeometry();
        }

        // rebuilds heightmap texturing within the given bounding box
        public void RebuildTerrainTexture(SFCoord topleft, SFCoord bottomright)
        {
            int chunk_count_x = width / SFMapHeightMapGeometryPool.CHUNK_SIZE;
            int chunk_count_y = height / SFMapHeightMapGeometryPool.CHUNK_SIZE;

            int topchunkx = topleft.x / SFMapHeightMapGeometryPool.CHUNK_SIZE;
            int topchunky = topleft.y / SFMapHeightMapGeometryPool.CHUNK_SIZE;
            int botchunkx = bottomright.x / SFMapHeightMapGeometryPool.CHUNK_SIZE;
            int botchunky = bottomright.y / SFMapHeightMapGeometryPool.CHUNK_SIZE;

            UpdateTileMap();
        }

        private void SetOverlayColors()
        {
            uniformOverlays[0] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);    // empty
            uniformOverlays[1] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);    // white
            uniformOverlays[2] = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);    // full red
            uniformOverlays[3] = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);    // full green
            uniformOverlays[4] = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);    // full blue
            uniformOverlays[5] = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);    // full yellow
            uniformOverlays[6] = new Vector4(1.0f, 0.0f, 1.0f, 1.0f);    // full fuchsia
            uniformOverlays[7] = new Vector4(0.0f, 1.0f, 1.0f, 1.0f);    // full teal
            uniformOverlays[8] = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);    // full black
            uniformOverlays[9] = new Vector4(0.6f, 0.0f, 0.0f, 1.0f);    // dark red
            uniformOverlays[10] = new Vector4(1.0f, 0.7f, 0.0f, 1.0f);    // orange
            uniformOverlays[11] = new Vector4(0.6f, 0.0f, 0.0f, 1.0f);    // dark red
            uniformOverlays[12] = new Vector4(0.6f, 0.0f, 0.0f, 1.0f);    // dark red
            uniformOverlays[13] = new Vector4(0.6f, 0.0f, 0.0f, 1.0f);    // dark red
            uniformOverlays[14] = new Vector4(0.6f, 0.0f, 0.0f, 1.0f);    // dark red
            uniformOverlays[15] = new Vector4(0.6f, 0.0f, 0.0f, 1.0f);    // dark red
            GL.BindBuffer(BufferTarget.UniformBuffer, uniformOverlays_buffer);
            GL.BufferSubData(BufferTarget.UniformBuffer, new IntPtr(0), 16 * 4 * 4, ref uniformOverlays[0]);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        // returns all scene chunk nodes that contain any of the points
        // it's pretty slow though...
        public List<SF3D.SceneSynchro.SceneNodeMapChunk> GetAreaMapNodes(HashSet<SFCoord> points)
        {
            List<SF3D.SceneSynchro.SceneNodeMapChunk> list = new List<SF3D.SceneSynchro.SceneNodeMapChunk>();
            foreach (SFCoord p in points)
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
            if (GetZ(pos) == 0)
                return false;

            SFMapHeightMapChunk chunk = GetChunk(pos);
            // check if another unit is on position
            foreach (SFMapUnit u in chunk.units)
                if (u.grid_position == pos)
                    return false;
            // check if building is on position
            if (building_data[pos.y * width + pos.x] != 0)
                return false;
            // check if another object is on position
            foreach (SFMapObject o in chunk.objects)
                if (o.grid_position == pos)
                    return false;
            // check if lake is on position
            if (lake_data[pos.y * width + pos.x] != 0)
                return false;

            return true;
        }

        public bool PositionOccupiedByObject(SFCoord pos)
        {
            SFMapHeightMapChunk chunk = GetChunk(pos);
            // check if another object is on position
            foreach (SFMapObject o in chunk.objects)
                if (o.grid_position == pos)
                    return true;

            return false;
        }

        // returns heightmap vertex normal
        // https://www.gamedev.net/forums/topic/163625-fast-way-to-calculate-heightmap-normals/
        public Vector3 GetVertexNormal(int x, int y)
        {
            float hscale = 100.0f;
            float az = (x < width - 1) ? (GetHeightAt(x + 1, y)) : (0);
            float bz = (y < height - 1) ? (GetHeightAt(x, y + 1)) : (0);
            float cz = (x > 0) ? (GetHeightAt(x - 1, y)) : (0);
            float dz = (y > 0) ? (GetHeightAt(x, y - 1)) : (0);

            return (new Vector3(cz - az, 2 * hscale, bz - dz)).Normalized();
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

            if (tile_data_texture != -1)
            {
                GL.DeleteTexture(tile_data_texture);
                tile_data_texture = -1;
            }

            if (overlay_texture_flags != -1)
            {
                GL.DeleteTexture(overlay_texture_flags);
                overlay_texture_flags = -1;
            }
            if (overlay_texture_decals != -1)
            {
                GL.DeleteTexture(overlay_texture_decals);
                overlay_texture_decals = -1;
            }
            overlay_active_texture = -1;

            if (texture_manager != null)
                texture_manager.Unload();
            if (chunk_nodes != null)
                foreach (SF3D.SceneSynchro.SceneNodeMapChunk chunk in chunk_nodes)
                    SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(chunk);
            geometry_pool.Unload();
            geometry_pool = null;

            chunk42_data.Clear();
            chunk56_data.Clear();
            chunk60_data.Clear();
            visible_chunks.Clear();
            chunk_nodes = null;
            map = null;
        }

        // when visibility settings change, this is called
        public void SetVisibilitySettings()
        {
            foreach (var chunk in visible_chunks)
                chunk.MapChunk.UpdateSettingsVisible();
        }
    }
}
