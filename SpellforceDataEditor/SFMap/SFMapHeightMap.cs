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
    public enum SFMapHeightMapLOD { NONE = 0, TESSELATION }

    public class SFMapHeightMapMesh
    {
        public const int CHUNK_SIZE = 16;          // width/height of one chunk

        // opengl VAO and VBOs
        public int vertex_array = 0;
        public int position_buffer, normal_buffer, element_buffer;

        public Vector3[] vertices = null;
        public Vector3[] normals = null;

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
                    indices[2 * (i * (hmap.width + 1) + j) + 0] = (uint)(i * (hmap.width + 1) + j);
                    indices[2 * (i * (hmap.width + 1) + j) + 1] = (uint)((i + 1) * (hmap.width + 1) + j);
                }

                i += 1;

                for (uint j = 0; j < hmap.width + 1; j++)
                {
                    indices[2 * (i * (hmap.width + 1) + j) + 0] = (uint)(i * (hmap.width + 1) + (hmap.width - j));
                    indices[2 * (i * (hmap.width + 1) + j) + 1] = (uint)((i + 1) * (hmap.width + 1) + (hmap.width - j));
                }
            }

            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, position_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * 12, vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, indices.Length * 4, indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);

            Update(hmap, new SFCoord(0, 0), new SFCoord(hmap.width, hmap.height));
        }

        public void Update(SFMapHeightMap hmap, SFCoord topleft, SFCoord size)
        {
            topleft = topleft.Clamp(new SFCoord(0, 0), new SFCoord(hmap.width, hmap.height));
            size = ((topleft + size).Clamp(new SFCoord(0, 0), new SFCoord(hmap.width, hmap.height))) - topleft;

            int v_size = hmap.width+1;

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


            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, position_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * 12, vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindVertexArray(0);

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
                return;

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
                    if (h > y2)
                        y2 = h;
                    if (h < y1)
                        y1 = h;

                }
            }

            aabb = new SF3D.Physics.BoundingBox(new Vector3(ix * size, y1 / 100.0f, iy * size), new Vector3((ix + 1) * size, y2 / 100.0f, (iy +1) * size));
        }

        // updates physical heightmap chunk, fixes entity positions
        // called by heightmap when a chunk is modified
        public void RebuildGeometry()
        {
            if (!visible)
                return;

            /*hmap.mesh.Update(hmap, 
                new SFCoord(ix * SFMapHeightMapMesh.CHUNK_SIZE, hmap.height - (iy + 1) * SFMapHeightMapMesh.CHUNK_SIZE), 
                new SFCoord(SFMapHeightMapMesh.CHUNK_SIZE, SFMapHeightMapMesh.CHUNK_SIZE));*/

            GenerateAABB();


            // fix all object heights
            foreach (SFMapUnit u in units)
                u.node.SetPosition(hmap.GetFixedPosition(u.grid_position));
            foreach (SFMapObject o in objects)
                o.node.SetPosition(hmap.GetFixedPosition(o.grid_position));
            foreach (SFMapInteractiveObject io in int_objects)
                io.node.SetPosition(hmap.GetFixedPosition(io.grid_position));
            foreach (SFMapDecoration d in decorations)
                d.node.SetPosition(hmap.GetFixedPosition(d.grid_position));
            foreach (SFMapBuilding b in buildings)
                b.node.SetPosition(new Vector3(b.node.position.X, hmap.GetZ(b.grid_position) / 100.0f, b.node.position.Z));// special case, offset information is preserved this way
            foreach (SFMapPortal p in portals)
                p.node.SetPosition(hmap.GetFixedPosition(p.grid_position));
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

                    owner.Update(0);                  // sets model matrix, needed for lake generation

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
                        d.node.Visible = false;
                }
            }
            else
            {
                if ((camera_dist <= Settings.DecorationFade) && (camera_hdiff <= (Settings.DecorationFade * 1.14f)))
                {
                    decoration_visible = true;
                    if (Settings.DecorationsVisible)
                        foreach (SFMapDecoration d in decorations)
                            d.node.Visible = true;
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
                        u.node.Visible = false;
                }
            }
            else
            {
                if ((camera_dist <= Settings.UnitFade) && (camera_hdiff <= (Settings.UnitFade * 1.14f)))
                {
                    unit_visible = true;
                    if (Settings.UnitsVisible)
                        foreach (SFMapUnit u in units)
                            u.node.Visible = true;
                }
            }
        }

        public void UpdateSettingsVisible()
        {
            foreach (SFMapUnit u in units)
                u.node.Visible = (Settings.UnitsVisible & unit_visible);
            foreach (SFMapBuilding b in buildings)
                b.node.Visible = Settings.BuildingsVisible;
            foreach (SFMapObject o in objects)
                o.node.Visible = Settings.ObjectsVisible;
            foreach (SFMapInteractiveObject io in int_objects)
                io.node.Visible = Settings.ObjectsVisible;
            foreach (SFMapPortal p in portals)
                p.node.Visible = Settings.ObjectsVisible;

            if ((Settings.DecorationsVisible) && (decoration_visible))
                foreach (SFMapDecoration d in decorations)
                    d.node.Visible = Settings.DecorationsVisible;
            if (!Settings.DecorationsVisible)
                foreach (SFMapDecoration d in decorations)
                    d.node.Visible = Settings.DecorationsVisible;

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

        // only one of those meshes is used at any time, depending on Settings.TerrainLOD
        public SFMapHeightMapMeshTesselated mesh_tesselated { get; private set; } = new SFMapHeightMapMeshTesselated();
        public SFMapHeightMapMesh mesh { get; private set; } = new SFMapHeightMapMesh();

        public int width, height;
        public ushort[] height_data;
        public byte[] tile_data;
        public byte[] lake_data;    // 0 - no lake, 1-255 - lakes 0-254
        public ushort[] building_data;   // 0 - no building, 1-65535 - buildings 0-65534
        public bool[] temporary_mask;   // for calculating islands by height
        public List<SFCoord> chunk42_data = new List<SFCoord>();
        public List<SFCoord> chunk56_data = new List<SFCoord>();
        public List<SFMapChunk60Data> chunk60_data = new List<SFMapChunk60Data>();

        public SF3D.SceneSynchro.SceneNodeMapChunk[] chunk_nodes;
        public List<SF3D.SceneSynchro.SceneNodeMapChunk> visible_chunks = new List<SF3D.SceneSynchro.SceneNodeMapChunk>();

        // height data translates directly to a texture
        public int height_data_texture = -1;

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
            GL.BindTexture(TextureTarget.Texture2D, tile_data_texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, width, height, 0,
                PixelFormat.Red, PixelType.UnsignedByte, tile_data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            height_data_texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, height_data_texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R16, width, height, 0,
                PixelFormat.Red, PixelType.UnsignedShort, height_data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);

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
            GL.BindTexture(TextureTarget.Texture2D, tile_data_texture);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Red, PixelType.UnsignedByte, tile_data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void UpdateHeightMap()
        {
            GL.BindTexture(TextureTarget.Texture2D, height_data_texture);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Red, PixelType.UnsignedShort, height_data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
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

            int chunk_count_x = width / SFMapHeightMapMesh.CHUNK_SIZE;
            int chunk_count_y = height / SFMapHeightMapMesh.CHUNK_SIZE;
            chunk_nodes = new SF3D.SceneSynchro.SceneNodeMapChunk[chunk_count_x * chunk_count_y];
            for (int i = 0; i < chunk_count_y; i++)
                for (int j = 0; j < chunk_count_x; j++)
                {
                    chunk_nodes[i * chunk_count_x + j] = new SF3D.SceneSynchro.SceneNodeMapChunk("hmap_" + j.ToString() + "_" + i.ToString());
                    SF3D.SceneSynchro.SceneNodeMapChunk chunk_node = chunk_nodes[i * chunk_count_x + j];
                    chunk_node.SetPosition(new Vector3(j * SFMapHeightMapMesh.CHUNK_SIZE, 0, i * SFMapHeightMapMesh.CHUNK_SIZE));
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

            UpdateHeightMap();
            UpdateTileMap();
            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
                mesh.Init(this);
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
                mesh_tesselated.Init(this);

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
            if (!FitsInMap(pos))
                return 0;
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
            int chunk_count_x = width / SFMapHeightMapMesh.CHUNK_SIZE;
            int chunk_count_y = height / SFMapHeightMapMesh.CHUNK_SIZE;

            int topchunkx = topleft.x / SFMapHeightMapMesh.CHUNK_SIZE;
            int topchunky = topleft.y / SFMapHeightMapMesh.CHUNK_SIZE;
            int botchunkx = bottomright.x / SFMapHeightMapMesh.CHUNK_SIZE;
            int botchunky = bottomright.y / SFMapHeightMapMesh.CHUNK_SIZE;

            for (int i = topchunkx; i <= botchunkx; i++)
                for (int j = topchunky; j <= botchunky; j++)
                    chunk_nodes[(chunk_count_y - j - 1) * chunk_count_x + i].MapChunk.RebuildGeometry();

            UpdateHeightMap();
        }

        // rebuilds heightmap texturing within the given bounding box
        public void RebuildTerrainTexture(SFCoord topleft, SFCoord bottomright)
        {
            int chunk_count_x = width / SFMapHeightMapMesh.CHUNK_SIZE;
            int chunk_count_y = height / SFMapHeightMapMesh.CHUNK_SIZE;

            int topchunkx = topleft.x / SFMapHeightMapMesh.CHUNK_SIZE;
            int topchunky = topleft.y / SFMapHeightMapMesh.CHUNK_SIZE;
            int botchunkx = bottomright.x / SFMapHeightMapMesh.CHUNK_SIZE;
            int botchunky = bottomright.y / SFMapHeightMapMesh.CHUNK_SIZE;

            UpdateTileMap();
        }

        public void UpdateDecorationMap(int dec_group)
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    map.heightmap.overlay_data_decals[j * map.width + i] = 0;
            if (dec_group != 0)
            {
                for (int i = 0; i < 1048576; i++)
                {
                    if (map.decoration_manager.dec_assignment[i] == dec_group)
                    {
                        SFCoord p = map.decoration_manager.GetDecPosition(i);
                        map.heightmap.overlay_data_decals[p.y * map.width + p.x] = 6;
                    }
                }
            }
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

            if(height_data_texture != -1)
            {
                GL.DeleteTexture(height_data_texture);
                height_data_texture = -1;
            }
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
            //geometry_pool.Unload();
            //geometry_pool = null;
            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
                mesh.Unload();
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
                mesh_tesselated.Unload();

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
