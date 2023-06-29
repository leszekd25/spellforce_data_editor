using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFEngine.SF3D;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFEngine.SFMap
{
    public enum SFMapHeightMapLOD { NONE = 0, TESSELATION }

    public class SFMapHeightMapMesh
    {
        public const int CHUNK_SIZE = 16;          // width/height of one chunk

        // opengl VAO and VBOs
        public int vertex_array = 0;
        public int position_buffer, element_buffer;

        public Vector3[] vertices = null;

        public uint[] indices = null;

        public void Init(SFMapHeightMap hmap)
        {
            vertex_array = GL.GenVertexArray();
            position_buffer = GL.GenBuffer();
            element_buffer = GL.GenBuffer();

            vertices = new Vector3[(hmap.width + 1) * (hmap.height + 1)];
            indices = new uint[2 * (hmap.width + 1) * hmap.height];

            // generate indices
            for (uint i = 0; i < hmap.height; i++)
            {
                for (uint j = 0; j < hmap.width + 1; j++)
                {
                    indices[2 * (i * (hmap.width + 1) + j) + 0] = (uint)((i + 1) * (hmap.width + 1) + j);
                    indices[2 * (i * (hmap.width + 1) + j) + 1] = (uint)(i * (hmap.width + 1) + j);
                }

                i += 1;

                for (uint j = 0; j < hmap.width + 1; j++)
                {
                    indices[2 * (i * (hmap.width + 1) + j) + 0] = (uint)(i * (hmap.width + 1) + (hmap.width - j));
                    indices[2 * (i * (hmap.width + 1) + j) + 1] = (uint)((i + 1) * (hmap.width + 1) + (hmap.width - j));
                }
            }

            SF3D.SFRender.SFRenderEngine.SetVertexArrayObject(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, position_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * 12, vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, indices.Length * 4, indices, BufferUsageHint.StaticDraw);

            SF3D.SFRender.SFRenderEngine.SetVertexArrayObject(0);

            Update(hmap, new SFCoord(0, 0), new SFCoord(hmap.width, hmap.height));
        }

        public void Update(SFMapHeightMap hmap, SFCoord topleft, SFCoord size)
        {
            topleft = topleft.Clamp(new SFCoord(0, 0), new SFCoord(hmap.width, hmap.height));
            size = ((topleft + size).Clamp(new SFCoord(0, 0), new SFCoord(hmap.width, hmap.height))) - topleft;

            int v_size = hmap.width + 1;

            for (int i = topleft.y; i <= topleft.y + size.y; i++)
            {
                for (int j = topleft.x; j <= topleft.x + size.x; j++)
                {
                    vertices[i * v_size + j] = new Vector3(j, 0, v_size - i - 1);
                }
            }

            int index_min = topleft.y * v_size + topleft.x;
            int index_max = (topleft.y + size.y) * v_size + topleft.x + size.x;
            int v_count = index_max - index_min;

            GL.BindBuffer(BufferTarget.ArrayBuffer, position_buffer);
            GL.BufferSubData<Vector3>(BufferTarget.ArrayBuffer, new IntPtr(12 * index_min), 12 * v_count, ref vertices[index_min]);
        }

        public void Unload()
        {
            if (vertex_array != 0)
            {
                GL.DeleteBuffer(position_buffer);
                GL.DeleteBuffer(element_buffer);
                GL.DeleteVertexArray(vertex_array);
                vertex_array = 0;
            }
        }
    }

    // tesselated terrain mesh: quad patches of dimensions CHUNK_SIZE x CHUNK_SIZE will be tesselated in the shader
    public class SFMapHeightMapMeshTesselated
    {
        // opengl VAO and VBOs
        public int vertex_array = 0;
        public int position_buffer;
        public int patch_count;

        // patches
        public Vector3[] vertices = null;

        public void Init(SFMapHeightMap heightmap)
        {
            vertex_array = GL.GenVertexArray();
            position_buffer = GL.GenBuffer();


            patch_count = heightmap.width / SFMapHeightMapMesh.CHUNK_SIZE;
            vertices = new Vector3[4 * patch_count * patch_count];

            for (int y = 0; y < patch_count; y++)
            {
                for (int x = 0; x < patch_count; x++)
                {
                    vertices[4 * (y * patch_count + x) + 0] = new Vector3(x * SFMapHeightMapMesh.CHUNK_SIZE, 0, (y + 1) * SFMapHeightMapMesh.CHUNK_SIZE);
                    vertices[4 * (y * patch_count + x) + 1] = new Vector3(x * SFMapHeightMapMesh.CHUNK_SIZE, 0, y * SFMapHeightMapMesh.CHUNK_SIZE);
                    vertices[4 * (y * patch_count + x) + 2] = new Vector3((x + 1) * SFMapHeightMapMesh.CHUNK_SIZE, 0, y * SFMapHeightMapMesh.CHUNK_SIZE);
                    vertices[4 * (y * patch_count + x) + 3] = new Vector3((x + 1) * SFMapHeightMapMesh.CHUNK_SIZE, 0, (y + 1) * SFMapHeightMapMesh.CHUNK_SIZE);
                }
            }

            patch_count *= patch_count;


            SF3D.SFRender.SFRenderEngine.SetVertexArrayObject(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, position_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * 12, vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            SF3D.SFRender.SFRenderEngine.SetVertexArrayObject(0);

            GL.PatchParameter(PatchParameterInt.PatchVertices, 4);
        }

        public void Unload()
        {
            if (vertex_array != 0)
            {
                GL.DeleteBuffer(position_buffer);
                GL.DeleteVertexArray(vertex_array);
                vertex_array = 0;
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
        public SF3D.Physics.BoundingBox aabb;

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
            {
                return;
            }

            GenerateAABB();

            UpdateSettingsVisible();
        }

        // calculates bounding box of the terrain for this chunk
        public void GenerateAABB()
        {
            int size = width;

            ushort y1 = 65535;
            ushort y2 = 0;

            for (int i = 0; i <= size; i++)
            {
                for (int j = 0; j <= size; j++)
                {
                    ushort h = hmap.GetHeightAt(ix * size + j, hmap.height - (iy * size + i) - 1);
                    MathUtils.Expand(h, ref y1, ref y2);
                }
            }

            aabb = new SF3D.Physics.BoundingBox(new Vector3(ix * size, y1 / 100.0f, iy * size), new Vector3((ix + 1) * size, y2 / 100.0f, (iy + 1) * size));
        }

        // updates physical heightmap chunk, fixes entity positions
        // called by heightmap when a chunk is modified
        public void RebuildGeometry()
        {
            if (!visible)
            {
                return;
            }

            GenerateAABB();

            // fix all object heights
            foreach (SFMapUnit u in units)
            {
                u.node.Position = hmap.GetFixedPosition(u.grid_position);
            }

            foreach (SFMapObject o in objects)
            {
                o.node.Position = hmap.GetFixedPosition(o.grid_position);
            }

            foreach (SFMapInteractiveObject io in int_objects)
            {
                io.node.Position = hmap.GetFixedPosition(io.grid_position);
            }

            foreach (SFMapDecoration d in decorations)
            {
                foreach (SF3D.SceneSynchro.SceneNode n in d.node.Children)   // special case, offset information is preserved that way
                {
                    n.Position = new Vector3(n.position.X, hmap.GetRealZ(new Vector2(owner.position.X + n.position.X, owner.position.Z + n.position.Z)), n.position.Z);
                }
            }
            foreach (SFMapBuilding b in buildings)
            {
                b.node.Position = new Vector3(b.node.position.X, hmap.GetZ(b.grid_position) / 100.0f, b.node.position.Z);// special case, offset information is preserved this way
            }

            foreach (SFMapPortal p in portals)
            {
                p.node.Position = hmap.GetFixedPosition(p.grid_position);
            }
        }

        public void UpdateVisible(bool vis)
        {
            if (visible)
            {
                if (!vis)
                {
                    // visibility switches from visible to invisible
                    visible = false;
                }
            }
            else
            {
                if (vis)
                {
                    // visibility switches from invisible to visible
                    visible = true;
                    decoration_visible = true;

                    //owner.Update(0);                  // sets model matrix, needed for lake generation

                    Generate();
                }
            }
        }

        public void UpdateDecorationVisible(float camera_dist, float camera_hdiff)
        {
            if (decoration_visible)
            {
                if ((camera_dist > Settings.DecorationFade) || (camera_hdiff > (Settings.DecorationFade * 1.14f)))  // magic number...
                {
                    decoration_visible = false;
                    foreach (SFMapDecoration d in decorations)
                    {
                        d.node.Visible = false;
                    }
                }
            }
            else
            {
                if ((camera_dist <= Settings.DecorationFade) && (camera_hdiff <= (Settings.DecorationFade * 1.14f)))
                {
                    decoration_visible = true;
                    if (Settings.DecorationsVisible)
                    {
                        foreach (SFMapDecoration d in decorations)
                        {
                            d.node.Visible = true;
                        }
                    }
                }
            }
        }

        public void UpdateUnitVisible(float camera_dist, float camera_hdiff)
        {
            if (unit_visible)
            {
                if ((camera_dist > Settings.UnitFade) || (camera_hdiff > (Settings.UnitFade * 1.14f)))  // magic number...
                {
                    unit_visible = false;
                    foreach (SFMapUnit u in units)
                    {
                        u.node.Visible = false;
                    }
                }
            }
            else
            {
                if ((camera_dist <= Settings.UnitFade) && (camera_hdiff <= (Settings.UnitFade * 1.14f)))
                {
                    unit_visible = true;
                    if (Settings.UnitsVisible)
                    {
                        foreach (SFMapUnit u in units)
                        {
                            u.node.Visible = true;
                        }
                    }
                }
            }
        }

        public void UpdateSettingsVisible()
        {
            foreach (SFMapUnit u in units)
            {
                u.node.Visible = (Settings.UnitsVisible & unit_visible);
            }

            foreach (SFMapBuilding b in buildings)
            {
                b.node.Visible = Settings.BuildingsVisible;
            }

            foreach (SFMapObject o in objects)
            {
                o.node.Visible = Settings.ObjectsVisible;
            }

            foreach (SFMapInteractiveObject io in int_objects)
            {
                io.node.Visible = Settings.ObjectsVisible;
            }

            foreach (SFMapPortal p in portals)
            {
                p.node.Visible = Settings.ObjectsVisible;
            }

            if ((Settings.DecorationsVisible) && (decoration_visible))
            {
                foreach (SFMapDecoration d in decorations)
                {
                    d.node.Visible = Settings.DecorationsVisible;
                }
            }

            if (!Settings.DecorationsVisible)
            {
                foreach (SFMapDecoration d in decorations)
                {
                    d.node.Visible = Settings.DecorationsVisible;
                }
            }
        }

        public void Unload()
        {
            units.Clear();
            objects.Clear();
            buildings.Clear();
            decorations.Clear();
            int_objects.Clear();
            portals.Clear();

            hmap = null;
            owner = null;
        }
    }

    [Flags]
    public enum SFMapHeightMapFlag
    {
        NONE = 0x0000,

        FLAG_MOVEMENT = 0x0001,
        FLAG_VISION = 0x0002,
        TERRAIN_MOVEMENT = 0x0004,
        TERRAIN_VISION = 0x0008,
        LAKE_DEEP = 0x0010,
        LAKE_SHALLOW = 0x0020,
        LAKE_SHORE = 0x0040,
        ENTITY_OBJECT = 0x0080,
        ENTITY_OBJECT_COLLISION = 0x0100,
        ENTITY_UNIT = 0x0200,
        ENTITY_BUILDING = 0x0400,
        ENTITY_BUILDING_COLLISION = 0x0800,
        ENTITY_DECAL = 0x1000,
        EDITOR_MASK = 0x2000,
        EDITOR_DECAL = 0x4000,
        EDITOR_SELECTION = 0x8000,

        ALL = 0xFFFF
    };

    public class SFMapHeightMap
    {
        public SFMap map = null;
        public SFMapTerrainTextureManager texture_manager { get; private set; } = new SFMapTerrainTextureManager();

        // only one of those meshes is used at any time, depending on Settings.TerrainLOD
        public SFMapHeightMapMeshTesselated mesh_tesselated { get; private set; } = new SFMapHeightMapMeshTesselated();
        public SFMapHeightMapMesh mesh { get; private set; } = new SFMapHeightMapMesh();

        public int width, height;
        public ushort[] height_data;
        public uint[] tile_data;    // r, g, b, a for a single texture fetch; r offset (0, -1), g offset (1, -1), b offset (0, 0), a offset (1, 0)
        public ushort[] flag_data;
        public SFMapHeightMapFlag overlay_flags = 0;
        public byte overlay_decal_group = 0;    // 0 - no group
        public bool[] temporary_mask;   // for calculating islands by height

        public SF3D.SceneSynchro.SceneNodeMapChunk[] chunk_nodes;
        public List<SF3D.SceneSynchro.SceneNodeMapChunk> visible_chunks = new List<SF3D.SceneSynchro.SceneNodeMapChunk>();

        // UBO for overlay data to the shader
        int uniformOverlays_buffer;
        Vector4[] uniformOverlays = new Vector4[16];     // 16 available colors

        public SFTexture terrain_texture_lod_bump = null;
        public SFTexture overlay_texture = null;
        public SFTexture height_data_texture = null;
        public SFTexture tile_data_texture = null;

        public SFMapHeightMap(int w, int h)
        {
            width = w;
            height = h;
            height_data = new ushort[w * h];
            tile_data = new uint[w * h];
            flag_data = new ushort[w * h]; flag_data.Initialize();
            temporary_mask = new bool[w * h];

            tile_data_texture = SFTexture.DynamicTexture((ushort)w, (ushort)h, 1, TextureTarget.Texture2D, InternalFormat.Rgba8ui, PixelFormat.RgbaInteger, PixelType.UnsignedByte, (int)All.Nearest, (int)All.Nearest, (int)All.ClampToBorder, (int)All.ClampToBorder, Vector4.Zero, 0, false, false);
            SFResources.SFResourceManager.Textures.AddManually(tile_data_texture, "_TILES_TEXTURE_");
            tile_data_texture.UpdateImage(tile_data, 0, 0, 0);

            height_data_texture = SFTexture.DynamicTexture((ushort)w, (ushort)h, 1, TextureTarget.Texture2D, InternalFormat.R16, PixelFormat.Red, PixelType.UnsignedShort, (int)All.Nearest, (int)All.Nearest, (int)All.ClampToBorder, (int)All.ClampToBorder, Vector4.Zero, 0, false, false);
            SFResources.SFResourceManager.Textures.AddManually(height_data_texture, "_HEIGHTMAP_TEXTURE_");
            height_data_texture.UpdateImage(height_data, 0, 0, 0);


            // overlay data
            if (Settings.EditorMode)
            {
                // create uniform buffer object for overlays
                uniformOverlays_buffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.UniformBuffer, uniformOverlays_buffer);
                GL.BufferData(BufferTarget.UniformBuffer, 16 * 4 * 4, new IntPtr(0), BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.UniformBuffer, 0);
                GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 1, uniformOverlays_buffer, new IntPtr(0), 16 * 4 * 4);
                SetOverlayColors();

                overlay_texture = SFTexture.DynamicTexture((ushort)w, (ushort)h, 1, TextureTarget.Texture2D, InternalFormat.R16ui, PixelFormat.RedInteger, PixelType.UnsignedShort, (int)All.Nearest, (int)All.Nearest, (int)All.ClampToEdge, (int)All.ClampToEdge, Vector4.Zero, 0, false, false);
                SFResources.SFResourceManager.Textures.AddManually(overlay_texture, "_OVERLAY_TEXTURE_"); 
                overlay_texture.UpdateImage(flag_data, 0, 0, 0);
            }
        }

        public void UpdateTileMap()
        {
            SF3D.SFRender.SFRenderEngine.SetTexture(0, TextureTarget.Texture2D, tile_data_texture.tex_id);
            tile_data_texture.UpdateImage(tile_data, 0, 0, 0);
        }

        public void UpdateHeightMap()
        {
            SF3D.SFRender.SFRenderEngine.SetTexture(0, TextureTarget.Texture2D, height_data_texture.tex_id);
            height_data_texture.UpdateImage(height_data, 0, 0, 0);
        }

        public SFMapHeightMapFlag GetFlag(SFCoord pos)
        {
            return (SFMapHeightMapFlag)flag_data[pos.y * width + pos.x];
        }

        public void SetAllFlags(SFCoord pos, SFMapHeightMapFlag flag)
        {
            flag_data[pos.y * width + pos.x] = (ushort)flag;
        }

        public void SetFlag(SFCoord pos, SFMapHeightMapFlag flag, bool set)
        {
            if (set)
            {
                flag_data[pos.y * width + pos.x] |= (ushort)flag;
            }
            else
            {
                flag_data[pos.y * width + pos.x] &= (ushort)(flag_data[pos.y * width + pos.x] ^ (ushort)flag);
            }
        }

        // true if all flags are set
        public bool IsFlagSet(SFCoord pos, SFMapHeightMapFlag flag)
        {
            return (flag_data[pos.y * width + pos.x] & (ushort)flag) == (ushort)flag;
        }

        // true if any flags are set
        public bool IsAnyFlagSet(SFCoord pos, SFMapHeightMapFlag flag)
        {
            return (flag_data[pos.y * width + pos.x] & (ushort)flag) != 0;
        }

        // refreshes currently active overlay
        public void RefreshOverlay()
        {
            if (!Settings.EditorMode)
            {
                return;
            }

            SF3D.SFRender.SFRenderEngine.SetTexture(3, TextureTarget.Texture2D, 0);
            SF3D.SFRender.SFRenderEngine.SetTexture(3, TextureTarget.Texture2D, overlay_texture.tex_id);
            overlay_texture.UpdateImage(flag_data, 0, 0, 0);
            //GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.RedInteger, PixelType.UnsignedShort, flag_data);
        }

        public SFMapHeightMapChunk GetChunk(SFCoord pos)
        {
            int chunk_count_y = height / SFMapHeightMapMesh.CHUNK_SIZE;
            return chunk_nodes[(chunk_count_y * ((height - pos.y - 1) / SFMapHeightMapMesh.CHUNK_SIZE) + (pos.x / SFMapHeightMapMesh.CHUNK_SIZE))].MapChunk;
        }

        public SF3D.SceneSynchro.SceneNodeMapChunk GetChunkNode(SFCoord pos)
        {
            int chunk_count_y = height / SFMapHeightMapMesh.CHUNK_SIZE;
            return chunk_nodes[(chunk_count_y * ((height - pos.y - 1) / SFMapHeightMapMesh.CHUNK_SIZE) + (pos.x / SFMapHeightMapMesh.CHUNK_SIZE))];
        }

        // takes into account that each object on the map is bound to a chunk
        public Vector3 GetFixedPosition(SFCoord pos)
        {
            return new Vector3(pos.x % SFMapHeightMapMesh.CHUNK_SIZE, GetZ(pos) / 100.0f, (height - pos.y - 1) % SFMapHeightMapMesh.CHUNK_SIZE);
        }


        public void SetRowRaw(int row, byte[] chunk_data)
        {
            for (int i = 0; i < width; i++)
            {
                height_data[row * width + i] = BitConverter.ToUInt16(chunk_data, i * 2);
            }
        }

        public void GetRowRaw(int row, ref ushort[] chunk_data)
        {
            for (int i = 0; i < width; i++)
            {
                chunk_data[i] = height_data[row * width + i];
            }
        }

        public void SetTilesRaw(byte[] _tiles)
        {
            // fast path
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    SetTileFast(x, y, _tiles[y * width + x]);
                }
            }

            // slow path
            for (int x = 0; x < width; x++)
            {
                SetTile(new SFCoord(x, 0), _tiles[x]);
                SetTile(new SFCoord(x, height-1), _tiles[(height - 1) * width + x]);
            }
            for (int y = 0; y < height; y++)
            {
                SetTile(new SFCoord(0, y), _tiles[y * width]);
                SetTile(new SFCoord(width-1, y), _tiles[y * width + width - 1]);
            }
        }

        public void Generate()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapHeightMap.Generate() called");

            int chunk_count_x = width / SFMapHeightMapMesh.CHUNK_SIZE;
            int chunk_count_y = height / SFMapHeightMapMesh.CHUNK_SIZE;
            chunk_nodes = new SF3D.SceneSynchro.SceneNodeMapChunk[chunk_count_x * chunk_count_y];
            for (int i = 0; i < chunk_count_y; i++)
            {
                for (int j = 0; j < chunk_count_x; j++)
                {
                    chunk_nodes[i * chunk_count_x + j] = new SF3D.SceneSynchro.SceneNodeMapChunk("hmap_" + j.ToString() + "_" + i.ToString());
                    SF3D.SceneSynchro.SceneNodeMapChunk chunk_node = chunk_nodes[i * chunk_count_x + j];
                    chunk_node.Position = new Vector3(j * SFMapHeightMapMesh.CHUNK_SIZE, 0, i * SFMapHeightMapMesh.CHUNK_SIZE);
                    chunk_node.Visible = false;
                    chunk_node.MapChunk = new SFMapHeightMapChunk();
                    chunk_node.MapChunk.hmap = this;
                    chunk_node.MapChunk.owner = chunk_nodes[i * chunk_count_x + j];
                    chunk_node.MapChunk.ix = j;
                    chunk_node.MapChunk.iy = i;
                    chunk_node.MapChunk.id = i * (width / SFMapHeightMapMesh.CHUNK_SIZE) + j;
                    chunk_node.MapChunk.width = SFMapHeightMapMesh.CHUNK_SIZE;
                    chunk_node.MapChunk.height = SFMapHeightMapMesh.CHUNK_SIZE;
                    chunk_node.MapChunk.GenerateAABB();
                }
            }

            UpdateHeightMap();
            UpdateTileMap();
            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
            {
                mesh.Init(this);
            }
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
            {
                mesh_tesselated.Init(this);
            }

            // load bump map
            if(!SFResources.SFResourceManager.Textures.Load("landscape_island_worldd", SFUnPak.FileSource.PAK, out terrain_texture_lod_bump, out int ec))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFMapHeightMap.Generate(): Could not load texture (texture name = landscape_island_worldd)");
                terrain_texture_lod_bump = SF3D.SFRender.SFRenderEngine.opaque_tex;
            }
            else
            {
                SF3D.SFRender.SFRenderEngine.SetTexture(5, TextureTarget.Texture2D, terrain_texture_lod_bump.tex_id);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Repeat);
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
            {
                newmap_size = 512;
            }

            if ((bitmap.Width > 512) || (bitmap.Height > 512))
            {
                newmap_size = 1024;
            }

            SFCoord hmap_offset = new SFCoord((width - bitmap.Width) / 2, (height - bitmap.Height) / 2);

            // if only_change_height is set to true and new map size is greater than the current one, reorder the chunks
            if (newmap_size > width)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapHeightMap.Import(): New heightmap is bigger than the current one!");
                return -3;
            }

            if (step == 0)
            {
                step = 1;
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if ((i >= hmap_offset.y) || (i < width - hmap_offset.y) || (j >= hmap_offset.x) || (j < height - hmap_offset.x))
                    {
                        height_data[i * width + j] = (ushort)(Math.Max((bitmap.GetPixel(
                            j - hmap_offset.x,
                            bitmap.Height - (i - hmap_offset.y) - 1).R - offset), 0) * step);
                    }
                    else
                    {
                        height_data[i * width + j] = 0;
                    }
                }
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
            {
                for (int j = 0; j < width; j++)
                {
                    byte col = (byte)Math.Max(0,
                                        Math.Min(255,
                                            (GetZ(new SFCoord(j, i)) / step) + offset));
                    if (col <= offset)
                    {
                        col = 0;
                    }

                    bitmap.SetPixel(j, height - i - 1, System.Drawing.Color.FromArgb(col, col, col));
                }
            }

            return 0;
        }

        // returns bounding box for given area
        public void GetBoxFromArea(IEnumerable<SFCoord> area, out SFCoord tl, out SFCoord br)
        {
            if (area.Count() == 0)
            {
                tl = new SFCoord(0, 0); br = new SFCoord(0, 0);
                return;
            }

            tl = br = area.First();
            foreach (SFCoord p in area)
            {
                MathUtils.Expand(p.x, ref tl.x, ref br.x);
                MathUtils.Expand(p.y, ref tl.y, ref br.y);
            }
        }

        // returns height for given grid position (as found in map file, 0 = lowest, 65535 - highest available)
        public ushort GetZ(SFCoord pos)
        {
            if (!FitsInMap(pos))
            {
                return 0;
            }

            return height_data[pos.y * width + pos.x];
        }

        // returns whether a posiiton is within map bounds
        public bool FitsInMap(SFCoord p)
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

            ushort u1 = 0;
            ushort u2 = 0;
            ushort u3 = 0;
            ushort u4 = 0;
            SFCoord p = new SFCoord(left, top);   // top left
            u1 = GetZ(p);
            p = new SFCoord(left + 1, top);   // top right
            u2 = GetZ(p);
            p = new SFCoord(left, top + 1);   // bottom left
            u3 = GetZ(p);
            p = new SFCoord(left + 1, top + 1);   // bottom right
            u4 = GetZ(p);

            return Utility.BilinearInterpolation(u1, u2, u3, u4, tx, ty) / 100.0f;
        }

        public byte GetTile(SFCoord pos)
        {
            //return tile_data[pos.y * width + pos.x];
            return (byte)((tile_data[pos.y * width + pos.x] >> 16) & 0xFF);
        }

        // an ugly hack... todo: move it to texture manager
        public byte GetTileFixed(SFCoord pos)
        {
            //byte b = tile_data[pos.y * width + pos.x];
            byte b = GetTile(pos);
            return (b > 223 ? (byte)(b - 223) : b);
        }

        public void SetTileFast(int x, int y, byte b)
        {
            tile_data[y * width + x - 1] = (uint)((tile_data[y * width + x - 1] & 0x00FFFFFF) | (uint)(b << 24));
            tile_data[y * width + x] = (uint)((tile_data[y * width + x] & 0xFF00FFFF) | (uint)(b << 16));
            tile_data[(y + 1) * width + x - 1] = (uint)((tile_data[(y + 1) * width + x - 1] & 0xFFFF00FF) | (uint)(b << 8));
            tile_data[(y + 1) * width + x] = (uint)((tile_data[(y + 1) * width + x] & 0xFFFFFF00) | (uint)(b << 0));
        }

        public void SetTile(SFCoord pos, byte b)
        {
            if (!FitsInMap(pos))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapHeightMap.SetTile(): Position out of bounds: " + pos.ToString());
                throw new Exception("SFMapHeightMap.SetTile(): Bad position!");
            }
            SFCoord pos2 = pos + new SFCoord(-1, 0);
            if (FitsInMap(pos2))
            {
                tile_data[pos2.y * width + pos2.x] = (uint)((tile_data[pos2.y * width + pos2.x] & 0x00FFFFFF) | (uint)(b << 24));
            }
            pos2 = pos + new SFCoord(0, 0);
            //if (FitsInMap(pos2))
            {
                tile_data[pos2.y * width + pos2.x] = (uint)((tile_data[pos2.y * width + pos2.x] & 0xFF00FFFF) | (uint)(b << 16));
            }
            pos2 = pos + new SFCoord(-1, 1);
            if (FitsInMap(pos2))
            {
                tile_data[pos2.y * width + pos2.x] = (uint)((tile_data[pos2.y * width + pos2.x] & 0xFFFF00FF) | (uint)(b << 8));
            }
            pos2 = pos + new SFCoord(0, 1);
            if (FitsInMap(pos2))
            {
                tile_data[pos2.y * width + pos2.x] = (uint)((tile_data[pos2.y * width + pos2.x] & 0xFFFFFF00) | (uint)(b << 0));
            }
        }

        // used in generation of lakes
        // flood fill based on z difference and return result
        public HashSet<SFCoord> GetIslandByHeight(SFCoord start, short z_diff, out HashSet<SFCoord> shore)
        {
            for (int i = 0; i < width * height; i++)
            {
                temporary_mask[i] = false;
            }

            HashSet<SFCoord> island = new HashSet<SFCoord>();
            shore = new HashSet<SFCoord>();
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

            shore = GetBorder(island);

            return island;
        }

        // used in generation of lakes
        // flood fill based on z difference and return result
        public HashSet<SFCoord> GetIslandByWalkable(SFCoord start)
        {
            for (int i = 0; i < width * height; i++)
            {
                temporary_mask[i] = false;
            }

            SFMapHeightMapFlag block_flag = SFMapHeightMapFlag.ENTITY_BUILDING_COLLISION | SFMapHeightMapFlag.ENTITY_OBJECT_COLLISION | SFMapHeightMapFlag.FLAG_MOVEMENT | SFMapHeightMapFlag.LAKE_DEEP | SFMapHeightMapFlag.TERRAIN_MOVEMENT;

            if (IsAnyFlagSet(start, block_flag))
            {
                return new HashSet<SFCoord>();
            }

            HashSet<SFCoord> island = new HashSet<SFCoord>();
            Queue<SFCoord> to_be_checked = new Queue<SFCoord>();
            SFCoord cur_pos;
            SFCoord next_pos;

            temporary_mask[start.y * width + start.x] = true;
            to_be_checked.Enqueue(start);

            // for each coordinate from the queue, add up to 4 neighbors to the queue if they're unchecked and fulfill the criteria
            while (to_be_checked.Count != 0)
            {
                cur_pos = to_be_checked.Dequeue();
                // every position that's in the queue will belong to an island
                island.Add(cur_pos);

                next_pos = cur_pos; next_pos.x += 1;
                if ((next_pos.x < width) && (temporary_mask[next_pos.y * width + next_pos.x] != true) && (!IsAnyFlagSet(next_pos, block_flag)))
                {
                    to_be_checked.Enqueue(next_pos);
                    temporary_mask[next_pos.y * width + next_pos.x] = true;
                }
                next_pos = cur_pos; next_pos.y += 1;
                if ((next_pos.y < height) && (temporary_mask[next_pos.y * width + next_pos.x] != true) && (!IsAnyFlagSet(next_pos, block_flag)))
                {
                    to_be_checked.Enqueue(next_pos);
                    temporary_mask[next_pos.y * width + next_pos.x] = true;
                }
                next_pos = cur_pos; next_pos.x -= 1;
                if ((next_pos.x >= 0) && (temporary_mask[next_pos.y * width + next_pos.x] != true) && (!IsAnyFlagSet(next_pos, block_flag)))
                {
                    to_be_checked.Enqueue(next_pos);
                    temporary_mask[next_pos.y * width + next_pos.x] = true;
                }
                next_pos = cur_pos; next_pos.y -= 1;
                if ((next_pos.y >= 0) && (temporary_mask[next_pos.y * width + next_pos.x] != true) && (!IsAnyFlagSet(next_pos, block_flag)))
                {
                    to_be_checked.Enqueue(next_pos);
                    temporary_mask[next_pos.y * width + next_pos.x] = true;
                }
            }

            return island;
        }

        public HashSet<SFCoord> GetBorder(IEnumerable<SFCoord> island)
        {
            HashSet<SFCoord> border = new HashSet<SFCoord>();
            foreach (SFCoord pos in island)
            {
                SFCoord tmp;
                tmp.x = (short)(pos.x - 1); tmp.y = pos.y;
                if ((tmp.x >= 0) && (!island.Contains(tmp))) { border.Add(tmp); }
                tmp.x = (short)(pos.x + 1); tmp.y = pos.y;
                if ((tmp.x < width) && (!island.Contains(tmp))) { border.Add(tmp); }
                tmp.x = pos.x; tmp.y = (short)(pos.y - 1);
                if ((tmp.y >= 0) && (!island.Contains(tmp))) { border.Add(tmp); }
                tmp.x = pos.x; tmp.y = (short)(pos.y + 1);
                if ((tmp.y < height) && (!island.Contains(tmp))) { border.Add(tmp); }
                tmp.x = (short)(pos.x - 1); tmp.y = (short)(pos.y - 1);
                if ((tmp.x >= 0) && (tmp.y >= 0) && (!island.Contains(tmp))) { border.Add(tmp); }
                tmp.x = (short)(pos.x + 1); tmp.y = (short)(pos.y - 1);
                if ((tmp.x < width) && (tmp.y >= 0) && (!island.Contains(tmp))) { border.Add(tmp); }
                tmp.x = (short)(pos.x - 1); tmp.y = (short)(pos.y + 1);
                if ((tmp.x >= 0) && (tmp.y < height) && (!island.Contains(tmp))) { border.Add(tmp); }
                tmp.x = (short)(pos.x + 1); tmp.y = (short)(pos.y + 1);
                if ((tmp.x < width) && (tmp.y < height) && (!island.Contains(tmp))) { border.Add(tmp); }
            }

            return border;
        }

        public HashSet<SFCoord> GetInnerBorder(IEnumerable<SFCoord> island)
        {
            HashSet<SFCoord> border = new HashSet<SFCoord>();
            foreach (SFCoord pos in island)
            {
                SFCoord tmp;
                tmp.x = (short)(pos.x - 1); tmp.y = pos.y;
                if ((tmp.x >= 0) && (!island.Contains(tmp))) { border.Add(pos); }
                tmp.x = (short)(pos.x + 1); tmp.y = pos.y;
                if ((tmp.x < width) && (!island.Contains(tmp))) { border.Add(pos); }
                tmp.x = pos.x; tmp.y = (short)(pos.y - 1);
                if ((tmp.y >= 0) && (!island.Contains(tmp))) { border.Add(pos); }
                tmp.x = pos.x; tmp.y = (short)(pos.y + 1);
                if ((tmp.y < height) && (!island.Contains(tmp))) { border.Add(pos); }
                tmp.x = (short)(pos.x - 1); tmp.y = (short)(pos.y - 1);
                if ((tmp.x >= 0) && (tmp.y >= 0) && (!island.Contains(tmp))) { border.Add(pos); }
                tmp.x = (short)(pos.x + 1); tmp.y = (short)(pos.y - 1);
                if ((tmp.x < width) && (tmp.y >= 0) && (!island.Contains(tmp))) { border.Add(pos); }
                tmp.x = (short)(pos.x - 1); tmp.y = (short)(pos.y + 1);
                if ((tmp.x >= 0) && (tmp.y < height) && (!island.Contains(tmp))) { border.Add(pos); }
                tmp.x = (short)(pos.x + 1); tmp.y = (short)(pos.y + 1);
                if ((tmp.x < width) && (tmp.y < height) && (!island.Contains(tmp))) { border.Add(pos); }
            }

            return border;
        }

        public List<HashSet<SFCoord>> SplitIsland(HashSet<SFCoord> island)
        {
            List<HashSet<SFCoord>> result = new List<HashSet<SFCoord>>();
            while (island.Count != 0)
            {
                result.Add(ExtractFirstSubisland(island));
            }

            return result;
        }

        public HashSet<SFCoord> ExtractFirstSubisland(HashSet<SFCoord> island)
        {
            Queue<SFCoord> to_be_checked = new Queue<SFCoord>();
            HashSet<SFCoord> subisland = new HashSet<SFCoord>();
            SFCoord cur_pos;
            SFCoord next_pos;

            if (island.Count == 0)
            {
                return subisland;
            }

            to_be_checked.Enqueue(island.First());
            subisland.Add(to_be_checked.Peek());
            island.Remove(to_be_checked.Peek());

            while (to_be_checked.Count != 0)
            {
                cur_pos = to_be_checked.Dequeue();

                next_pos = cur_pos; next_pos.x += 1;
                if (island.Contains(next_pos))
                {
                    subisland.Add(next_pos);
                    island.Remove(next_pos);
                    to_be_checked.Enqueue(next_pos);
                }
                next_pos = cur_pos; next_pos.y += 1;
                if (island.Contains(next_pos))
                {
                    subisland.Add(next_pos);
                    island.Remove(next_pos);
                    to_be_checked.Enqueue(next_pos);
                }
                next_pos = cur_pos; next_pos.x -= 1;
                if (island.Contains(next_pos))
                {
                    subisland.Add(next_pos);
                    island.Remove(next_pos);
                    to_be_checked.Enqueue(next_pos);
                }
                next_pos = cur_pos; next_pos.y -= 1;
                if (island.Contains(next_pos))
                {
                    subisland.Add(next_pos);
                    island.Remove(next_pos);
                    to_be_checked.Enqueue(next_pos);
                }
            }

            return subisland;
        }

        // rebuilds heightmap chunks within the given bounding box
        public void RebuildGeometry(SFCoord topleft, SFCoord bottomright)
        {
            int chunk_count_x = width / SFMapHeightMapMesh.CHUNK_SIZE;
            int chunk_count_y = height / SFMapHeightMapMesh.CHUNK_SIZE;

            int topchunkx = topleft.x / SFMapHeightMapMesh.CHUNK_SIZE;
            int topchunky = topleft.y / SFMapHeightMapMesh.CHUNK_SIZE;
            int botchunkx = bottomright.x / SFMapHeightMapMesh.CHUNK_SIZE;
            int botchunky = bottomright.y / SFMapHeightMapMesh.CHUNK_SIZE;

            for (int i = topchunkx; i <= botchunkx; i++)
            {
                for (int j = topchunky; j <= botchunky; j++)
                {
                    chunk_nodes[(chunk_count_y - j - 1) * chunk_count_x + i].MapChunk.RebuildGeometry();
                }
            }

            UpdateHeightMap();

            // update decals here
            foreach(SF3D.SceneSynchro.SFDecalInfo decal_info in SF3D.SFRender.SFRenderEngine.scene.decal_info.GetItems())
            {
                // check bounding box
                if (((topleft.x <= decal_info.bottomright.x) && (topleft.x >= decal_info.topleft.x)) || ((bottomright.x <= decal_info.bottomright.x) && (bottomright.x >= decal_info.topleft.x)))
                {
                    if (((topleft.y <= decal_info.bottomright.y) && (topleft.y >= decal_info.topleft.y)) || ((bottomright.y <= decal_info.bottomright.y) && (bottomright.y >= decal_info.topleft.y)))
                    {
                        map.UpdateNodeDecal(decal_info.decal_node.Parent, decal_info.center, decal_info.offset, decal_info.angle);
                    }
                }
            }
        }

        // rebuilds heightmap texturing within the given bounding box
        public void RebuildTerrainTexture(SFCoord topleft, SFCoord bottomright)
        {
            for (int y = topleft.y; y <= bottomright.y; y++)
            {
                for (int x = topleft.x; x <= bottomright.x; x++)
                {
                    SFCoord pos = new SFCoord(x, y);
                    SetFlag(pos, SFMapHeightMapFlag.TERRAIN_MOVEMENT, texture_manager.texture_tiledata[GetTile(pos)].blocks_movement);
                    SetFlag(pos, SFMapHeightMapFlag.TERRAIN_VISION, texture_manager.texture_tiledata[GetTile(pos)].blocks_vision);
                }
            }

            UpdateTileMap();
            RefreshOverlay();
        }

        public void UpdateDecorationMap(int dec_group)
        {
            if (Settings.EditorMode)
            {
                overlay_decal_group = (byte)dec_group;
            }
        }

        private void SetOverlayColors()
        {
            if (!Settings.EditorMode)
            {
                return;
            }

            uniformOverlays[0] = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);    // movement flag
            uniformOverlays[1] = new Vector4(0.5f, 0.5f, 0.0f, 1.0f);    // vision flag
            uniformOverlays[2] = new Vector4(0.6f, 0.0f, 0.0f, 1.0f);    // terrain movement flag
            uniformOverlays[3] = new Vector4(0.6f, 0.6f, 0.0f, 1.0f);    // terrain vision flag
            uniformOverlays[4] = new Vector4(0.8f, 0.0f, 0.0f, 1.0f);    // deep lake flag
            uniformOverlays[5] = new Vector4(0.6f, 0.6f, 0.0f, 1.0f);    // shallow lake flag
            uniformOverlays[6] = new Vector4(0.0f, 0.8f, 0.0f, 1.0f);    // shore flag
            uniformOverlays[7] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);    // object flag
            uniformOverlays[8] = new Vector4(1.0f, 0.0f, 1.0f, 1.0f);    // object collision flag
            uniformOverlays[9] = new Vector4(1.0f, 0.0f, 1.0f, 1.0f);    // unit flag
            uniformOverlays[10] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);    // building flag
            uniformOverlays[11] = new Vector4(1.0f, 0.0f, 1.0f, 1.0f);    // building collision flag
            uniformOverlays[12] = new Vector4(1.0f, 0.3f, 1.0f, 2.0f);    // decal flag
            uniformOverlays[13] = new Vector4(0.0f, 0.0f, 1.0f, 2.0f);    // mask flag
            uniformOverlays[14] = new Vector4(1.0f, 0.3f, 1.0f, 2.0f);    // editor decal
            uniformOverlays[15] = new Vector4(0.45f, 0.45f, 0.6f, 0.7f);    // selection

            if (Settings.ToneMapping)
            {
                for (int i = 0; i < 16; i++)
                {
                    uniformOverlays[i] = new Vector4(uniformOverlays[i].Xyz * 2.0f, uniformOverlays[i].W);
                }
            }
            GL.BindBuffer(BufferTarget.UniformBuffer, uniformOverlays_buffer);
            GL.BufferSubData(BufferTarget.UniformBuffer, new IntPtr(0), 16 * 4 * 4, ref uniformOverlays[0]);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public bool CanMoveToPosition(SFCoord pos)
        {
            if (GetZ(pos) == 0)
            {
                return false;
            }

            SFMapHeightMapChunk chunk = GetChunk(pos);
            // check if tile is passable
            // check if another unit is on position
            // check if building is on position
            // check if another object is on position
            // check if lake is on position
            if ((flag_data[pos.y * width + pos.x] & (ushort)(
                  SFMapHeightMapFlag.FLAG_MOVEMENT
                | SFMapHeightMapFlag.TERRAIN_MOVEMENT
                | SFMapHeightMapFlag.ENTITY_UNIT
                | SFMapHeightMapFlag.ENTITY_BUILDING_COLLISION
                | SFMapHeightMapFlag.ENTITY_OBJECT
                | SFMapHeightMapFlag.LAKE_DEEP
                )) != 0)
            {
                return false;
            }

            return true;
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
            {
                return 0;
            }

            return height_data[pos];
        }

        public void GenerateDecalGeometry(
            Vector2 bb_topleft, Vector2 bb_bottomright, Vector2 decal_center, Vector2 decal_offset, float angle,
            out Vector3[] vertices, out Vector3[] normals, out Vector2[] uvs, out uint[] indices, out SFCoord map_topleft, out SFCoord map_bottomright)
        {
            // 1. calculate world space coordinates for bounding box
            Vector2 wcs_topleft_prime = bb_topleft + decal_center;
            Vector2 wcs_bottomright_prime = bb_bottomright + decal_center;
            Vector2 wcs_pivot = decal_center;      // rotation pivot

            // 2. generate vertices of the bounding box
            Vector2[] wcs_bbox_mesh = new Vector2[]
            {
                wcs_topleft_prime,
                new Vector2(wcs_bottomright_prime.X, wcs_topleft_prime.Y),
                new Vector2(wcs_topleft_prime.X, wcs_bottomright_prime.Y),
                wcs_bottomright_prime
            };

            // 3. rotate the vertices around the pivot
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);
            Vector2 b_offset_rotated = new Vector2(c * decal_offset.X - s * decal_offset.Y, s * decal_offset.X + c * decal_offset.Y);
            for (int i = 0; i < 4; i++)
            {
                wcs_bbox_mesh[i] = MathUtils.RotateVec2PivotSinCos(wcs_bbox_mesh[i], wcs_pivot, s, c);
            }

            // 4. calculate bounding box from the rotated vertices
            Vector2 wcs_topleft = wcs_topleft_prime;
            Vector2 wcs_bottomright = wcs_bottomright_prime;
            for (int i = 0; i < 4; i++)
            {
                MathUtils.Expand(wcs_bbox_mesh[i].X, ref wcs_topleft.X, ref wcs_bottomright.X);
                MathUtils.Expand(wcs_bbox_mesh[i].Y, ref wcs_topleft.Y, ref wcs_bottomright.Y);
            }

            // 5. get map coordinates to gather the points from
            map_topleft = new SFCoord((int)Math.Floor(wcs_topleft.X - b_offset_rotated.X), (int)Math.Floor(wcs_topleft.Y - b_offset_rotated.Y));
            map_bottomright = new SFCoord((int)Math.Ceiling(wcs_bottomright.X - b_offset_rotated.X), (int)Math.Ceiling(wcs_bottomright.Y - b_offset_rotated.Y));
            int grid_w = map_bottomright.x - map_topleft.x;
            int grid_h = map_bottomright.y - map_topleft.y;
            float base_height = GetRealZ(new Vector2(decal_center.X, map.height - decal_center.Y - 1));     // point of failure: invert Y?
            Vector2 uv_center = new Vector2(
                (decal_center.X - wcs_topleft_prime.X) / (wcs_bottomright_prime.X - wcs_topleft_prime.X),
                (wcs_bottomright_prime.Y - decal_center.Y) / (wcs_bottomright_prime.Y - wcs_topleft_prime.Y));
            float uv_shift_y = 2.0f * (uv_center.Y - 0.5f);

            // 6. create buffers
            vertices = new Vector3[(grid_w + 1) * (grid_h + 1)];
            normals = new Vector3[(grid_w + 1) * (grid_h + 1)];
            uvs = new Vector2[(grid_w + 1) * (grid_h + 1)];
            indices = new uint[grid_w * grid_h * 6];

            // 7. fill vertices/normals/uv/indices data
            for (int y = 0; y <= grid_h; y++)
            {
                for (int x = 0; x <= grid_w; x++)
                {
                    SFCoord cur_pos = map_topleft + new SFCoord(x, y);
                    SFCoord cur_pos2 = new SFCoord(map_topleft.x + x, map_topleft.y + grid_h - y);

                    vertices[x + y * (grid_w + 1)] = new Vector3(cur_pos.x - wcs_pivot.X + b_offset_rotated.X, GetZ(cur_pos2) / 100.0f - base_height + 0.008f, wcs_pivot.Y - cur_pos2.y - b_offset_rotated.Y);
                    normals[x + y * (grid_w + 1)] = GetVertexNormal(cur_pos.x, cur_pos.y);

                    // uv is (0, 0) exactly at wcs_topleft_prime, and (1, 1) exactly at wcs_bottomright_prime
                    // uv(P) = ((x_P - x_1)/(x_2 - x_1), (y_P - y_1)/(y_2 - y_1))
                    // then, we rotate the uv around the pivot
                    Vector2 base_uv = new Vector2(
                        (cur_pos.x - wcs_topleft_prime.X + b_offset_rotated.X) / (wcs_bottomright_prime.X - wcs_topleft_prime.X),
                        (wcs_bottomright_prime.Y - cur_pos2.y - b_offset_rotated.Y) / (wcs_bottomright_prime.Y - wcs_topleft_prime.Y));
                    Vector2 tmp = base_uv - uv_center;
                    base_uv = new Vector2(tmp.X * c - tmp.Y * s, -tmp.X * s - tmp.Y * c) + uv_center;
                    base_uv.Y -= uv_shift_y;
                    base_uv = ((base_uv - new Vector2(0.5f, 0.5f)) * 1.28f) + new Vector2(0.5f, 0.5f);

                    uvs[x + y * (grid_w + 1)] = base_uv;
                }
            }

            // right now all heightmap patches are constructed the same way, which doesnt match the game heightmap layout
            // uncomment code below once that is fixed (broke because of culling changes)
            for (int y = 0; y < grid_h; y++)
            {
                /*if ((map_bottomright.y - y) % 2 == 1)
                {*/
                    for (int x = 0; x < grid_w; x++)
                    {
                        indices[6 * (x + y * grid_w) + 0] = (uint)(y * (grid_w + 1) + x);
                        indices[6 * (x + y * grid_w) + 1] = (uint)(y * (grid_w + 1) + x + 1);
                        indices[6 * (x + y * grid_w) + 2] = (uint)((y + 1) * (grid_w + 1) + x);
                        indices[6 * (x + y * grid_w) + 3] = (uint)(y * (grid_w + 1) + x + 1);
                        indices[6 * (x + y * grid_w) + 4] = (uint)((y + 1) * (grid_w + 1) + x + 1);
                        indices[6 * (x + y * grid_w) + 5] = (uint)((y + 1) * (grid_w + 1) + x);
                    }
                /*}
                else
                {
                    for (int x = 0; x < grid_w; x++)
                    {
                        indices[6 * (x + y * grid_w) + 0] = (uint)(y * (grid_w + 1) + x);
                        indices[6 * (x + y * grid_w) + 1] = (uint)((y + 1) * (grid_w + 1) + x + 1);
                        indices[6 * (x + y * grid_w) + 2] = (uint)((y + 1) * (grid_w + 1) + x);
                        indices[6 * (x + y * grid_w) + 3] = (uint)(y * (grid_w + 1) + x);
                        indices[6 * (x + y * grid_w) + 4] = (uint)(y * (grid_w + 1) + x + 1);
                        indices[6 * (x + y * grid_w) + 5] = (uint)((y + 1) * (grid_w + 1) + x + 1);
                    }
                }*/
            }
        }

        public void Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapHeightMap.Unload() called");
            if (map == null)
            {
                return;
            }

            SFResources.SFResourceManager.Textures.Dispose(height_data_texture);
            height_data_texture = null;
            SFResources.SFResourceManager.Textures.Dispose(tile_data_texture);
            tile_data_texture = null;
            SFResources.SFResourceManager.Textures.Dispose(overlay_texture);
            overlay_texture = null;
            if (terrain_texture_lod_bump != SF3D.SFRender.SFRenderEngine.opaque_tex)
            {
                SFResources.SFResourceManager.Textures.Dispose(terrain_texture_lod_bump);
            }
            terrain_texture_lod_bump = null;

            if (texture_manager != null)
            {
                texture_manager.Unload();
            }

            if (chunk_nodes != null)
            {
                foreach (SF3D.SceneSynchro.SceneNodeMapChunk chunk in chunk_nodes)
                {
                    SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(chunk);
                }
            }

            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
            {
                mesh.Unload();
            }
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
            {
                mesh_tesselated.Unload();
            }

            visible_chunks.Clear();
            chunk_nodes = null;
            map = null;
        }

        // when visibility settings change, this is called
        public void SetVisibilitySettings()
        {
            foreach (var chunk in visible_chunks)
            {
                chunk.MapChunk.UpdateSettingsVisible();
            }
        }
    }
}
