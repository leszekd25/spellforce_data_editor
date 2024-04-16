using NAudio.Gui;
using OpenTK.Mathematics;
using SFEngine.SF3D;
using System.Collections.Generic;
using System.Linq;

namespace SFEngine.SFMap
{
    public class SFMapLake
    {
        static int max_id = 0;

        public HashSet<SFCoord> cells = new HashSet<SFCoord>();
        public HashSet<SFCoord> shore = new HashSet<SFCoord>();
        public SFCoord start;
        public short z_diff;
        public int type;
        public int id;
        public SF3D.SceneSynchro.SceneNodeSimple node = null;

        public string GetObjectName()
        {
            return "LAKE_" + id.ToString();
        }

        public SFMapLake()
        {
            id = max_id;
            max_id += 1;
        }

        public override string ToString()
        {
            return GetObjectName();
        }

        public ushort GetWaterLevel(SFMapHeightMap hmap)
        {
            return (ushort)(hmap.GetZ(start) + z_diff);
        }
    }

    public class SFMapLakeManager
    {
        public List<SFMapLake> lakes { get; private set; } = new List<SFMapLake>();
        public List<bool> lake_visible { get; private set; } = new List<bool>();

        public SFMap map = null;
        public uint[] index_helper;
        public sbyte[] lake_type_helper;

        public const int LAKE_SHALLOW_DEPTH = 50;

        // ugly 4th parameter to make undo/redo work
        // uglier 5th and 6th parameters to make undo/redo work (assumed to not be null
        public SFMapLake AddLake(SFCoord start, ushort lake_level, int type, int lake_index, List<SFMapLake> consumed_lakes, List<int> consumed_lakes_indices)
        {
            if (lake_index == -1)
            {
                lake_index = lakes.Count;
            }

            SFMapLake lake = new SFMapLake();
            lakes.Insert(lake_index, lake);
            lake_visible.Insert(lake_index, true);
            lake.start = start;
            lake.z_diff = (short)(lake_level - map.heightmap.GetZ(start));
            lake.type = type;

            lake.node = SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeSimple(SF3D.SFRender.SFRenderEngine.scene.root, "_none_", lake.GetObjectName());
            lake.node.Position = new Vector3(0, 0, 0);

            UpdateLake(lake, consumed_lakes, consumed_lakes_indices);

            if (lake.cells.Count == 0)
            {
                RemoveLake(lake);
                return null;
            }

            return lake;
        }

        public int GetLakeIndexAt(SFCoord pos)
        {
            if (!map.heightmap.IsAnyFlagSet(pos, SFMapHeightMapFlag.LAKE_DEEP | SFMapHeightMapFlag.LAKE_SHALLOW))
            {
                return Utility.NO_INDEX;
            }

            for (int i = 0; i < lakes.Count; i++)
            {
                if (lakes[i].cells.Contains(pos))
                {
                    return i;
                }
            }

            return Utility.NO_INDEX;
        }

        private void DisposeLakeMesh(SFMapLake lake)
        {
            SFModel3D lake_model = lake.node.Mesh;
            lake.node.Mesh = null;
            SFResources.SFResourceManager.Models.Dispose(lake_model);
        }

        public void RemoveLake(SFMapLake lake)
        {
            int lake_index = lakes.IndexOf(lake);
            if (lake_index < 0)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapLakeManager.RemoveLake(): Lake not found in lake table!");
                return;
            }

            foreach (SFCoord p in lake.cells)
            {
                map.heightmap.SetFlag(p, SFMapHeightMapFlag.LAKE_DEEP | SFMapHeightMapFlag.LAKE_SHALLOW, false);
                lake_type_helper[p.x + p.y * map.width] = -1;
            }

            foreach (SFCoord p in lake.shore)
            {
                map.heightmap.SetFlag(p, SFMapHeightMapFlag.LAKE_SHORE, false);
            }

            lakes.RemoveAt(lake_index);
            lake_visible.RemoveAt(lake_index);

            // reload shore flags
            foreach (SFMapLake l in lakes)
            {
                foreach (SFCoord p in l.shore)
                {
                    map.heightmap.SetFlag(p, SFMapHeightMapFlag.LAKE_SHORE, true);
                }
            }

            SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(lake.node);
        }

        public string GetLakeTextureName(int type)
        {
            string tex_name = "";
            if (type == 0)
            {
                tex_name = "landscape_lake_water";
            }
            else if (type == 1)
            {
                tex_name = "landscape_swamp_l8";
            }
            else if (type == 2)
            {
                tex_name = "landscape_lake_lava1";
            }
            else if (type == 3)
            {
                tex_name = "landscape_lake_ice_l8";
            }

            return tex_name;
        }

        public System.Drawing.Color GetLakeMinimapColor(int type)
        {
            if (type == 0)
            {
                return System.Drawing.Color.FromArgb(255, 100, 100, 200);
            }
            else if (type == 1)
            {
                return System.Drawing.Color.FromArgb(255, 130, 90, 65);
            }
            else if (type == 2)
            {
                return System.Drawing.Color.FromArgb(255, 240, 140, 40);
            }
            else if (type == 3)
            {
                return System.Drawing.Color.FromArgb(255, 240, 240, 255);
            }

            return System.Drawing.Color.FromArgb(255, 0, 0, 0);
        }

        // should be updated after lake_start or lake_depth was modified
        // if lake_cells are given, lake's start point is trusted to be the lowest
        public void UpdateLake(SFMapLake lake, List<SFMapLake> consumed_lakes, List<int> consumed_lakes_indices)
        {
            int lake_index = lakes.IndexOf(lake);
            if (lake_index < 0)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapLakeManager.UpdateLake(): Lake not found in lake table!");
                return;
            }

            // clear current lake data
            foreach (SFCoord p in lake.cells)
            {
                map.heightmap.SetFlag(p, SFMapHeightMapFlag.LAKE_DEEP | SFMapHeightMapFlag.LAKE_SHALLOW, false);
                lake_type_helper[p.x + p.y * map.width] = -1;
            }
            foreach (SFCoord p in lake.shore)
            {
                map.heightmap.SetFlag(p, SFMapHeightMapFlag.LAKE_SHORE, false);
            }

            ushort lake_level = lake.GetWaterLevel(map.heightmap);

            lake.cells = map.heightmap.GetIslandByLevel(lake.start, lake_level, out lake.start);
            lake.shore = map.heightmap.GetBorder(lake.cells);
            lake.z_diff = (short)(lake_level - map.heightmap.GetZ(lake.start));

            // check if lake collides with other lakes
            List<SFMapLake> lakes_to_remove = new List<SFMapLake>();
            int lake_index_offset = 0;
            for (int i = 0; i < lakes.Count; i++)
            {
                SFMapLake l = lakes[i];
                if (l == lake)
                {
                    continue;
                }

                // if overlap is detected, this MUST mean that l is fully contained in lake
                if (l.cells.Overlaps(lake.cells))
                {
                    consumed_lakes.Add(l);
                    consumed_lakes_indices.Add(i - lake_index_offset);
                    lake_index_offset += 1;
                    lakes_to_remove.Add(l);
                }
            }

            foreach (SFMapLake l in lakes_to_remove)
            {
                RemoveLake(l);
            }

            ushort lake_z = lake.GetWaterLevel(map.heightmap);
            foreach (SFCoord p in lake.cells)
            {
                short lake_cell_z_diff = (short)(lake_z - map.heightmap.GetZ(p));
                map.heightmap.SetFlag(p, (lake_cell_z_diff < LAKE_SHALLOW_DEPTH ? SFMapHeightMapFlag.LAKE_SHALLOW : SFMapHeightMapFlag.LAKE_DEEP), true);
                lake_type_helper[p.x + p.y * map.width] = (sbyte)lake.type;
            }
            // reload shore flags
            foreach (SFMapLake l in lakes)
            {
                foreach (SFCoord p in lake.shore)
                {
                    map.heightmap.SetFlag(p, SFMapHeightMapFlag.LAKE_SHORE, true);
                }
            }

            RebuildLake(lake);
        }

        public void RebuildLake(SFMapLake lake)
        {
            DisposeLakeMesh(lake);

            if (lake.cells.Count + lake.shore.Count == 0)
            {
                return;
            }

            // create list of all vertex coords, sorted by Y first, then X
            HashSet<SFCoord> lake_extension = new HashSet<SFCoord>(lake.cells);
            foreach(SFCoord p in lake.shore)
            {
                lake_extension.Add(p);
                lake_extension.Add(p + new SFCoord(1, 0));
                lake_extension.Add(p + new SFCoord(0, -1));
                lake_extension.Add(p + new SFCoord(1, -1));
            }

            List<SFCoord> vertex_pos = new List<SFCoord>(lake_extension);

            // generate mesh
            SFSubModel3D submodel = new SFSubModel3D();
            int v_count = vertex_pos.Count;
            int i_count = (lake.cells.Count + lake.shore.Count) * 6;

            Vector3[] vertices = new Vector3[v_count];
            Vector2[] uvs = new Vector2[v_count];
            byte[] colors = new byte[v_count * 4];
            Vector3[] normals = new Vector3[v_count];
            uint[] indices = new uint[i_count];

            ushort lake_z = lake.GetWaterLevel(map.heightmap);
            float lake_height = ((lake_z)) / 100.0f;

            byte deep_lake = 0xFE;
            byte shallow_lake = 0x8F;
            if (lake.type == 2)
            {
                shallow_lake = 0xFE;
            }

            // these colors are supposed to be set via ConLakesInit.lua :/
            byte colR = 0, colG = 0, colB = 0, colA = 250;
            if (lake.type == 0)
            {
                colR = 30; colG = 100; colB = 180; colA = 250;
            }
            else if (lake.type == 1)
            {
                colR = 100; colG = 100; colB = 100; colA = 250;
            }
            else if (lake.type == 2)
            {
                colR = 255; colG = 255; colB = 255; colA = 250;  // superseded
            }
            else if (lake.type == 3)
            {
                colR = 240; colG = 240; colB = 230; colA = 250;
            }

            SFCoord area_tl, area_br;
            area_tl = area_br = vertex_pos[0];
            // generate geometry for each lake type
            for(int i = 0; i < v_count; i++)
            {
                SFCoord pos = vertex_pos[i];
                ushort lake_cell_z = map.heightmap.GetZ(pos);
                short lake_cell_z_diff = (short)(lake_z - lake_cell_z);

                vertices[i] = new Vector3(pos.x, lake_height, map.height - pos.y - 1);
                uvs[i] = new Vector2(pos.x / 4.0f, pos.y / 4.0f);
                colors[i * 4 + 0] = colR;
                colors[i * 4 + 1] = colG;
                colors[i * 4 + 2] = colB;
                colors[i * 4 + 3] = (byte)((colA / 256.0f) * (lake_cell_z_diff < LAKE_SHALLOW_DEPTH ? shallow_lake : deep_lake));
                normals[i] = Vector3.UnitY;

                index_helper[pos.x + (pos.y + 1) * (map.width + 1)] = (uint)i;

                MathUtils.Expand(pos.x, ref area_tl.x, ref area_br.x);
                MathUtils.Expand(pos.y, ref area_tl.y, ref area_br.y);
            }
            int cur_i = 0;
            foreach (HashSet<SFCoord> set in new HashSet<SFCoord>[] { lake.cells, lake.shore })
            {
                foreach (SFCoord pos in set)
                {
                    indices[cur_i * 6 + 0] = index_helper[pos.x + (pos.y + 1) * (map.width + 1)];
                    indices[cur_i * 6 + 1] = index_helper[pos.x + pos.y * (map.width + 1)];
                    indices[cur_i * 6 + 2] = index_helper[pos.x + 1 + (pos.y + 1) * (map.width + 1)];
                    indices[cur_i * 6 + 3] = indices[cur_i * 6 + 2];
                    indices[cur_i * 6 + 4] = indices[cur_i * 6 + 1];
                    indices[cur_i * 6 + 5] = index_helper[pos.x + 1 + pos.y * (map.width + 1)];

                    cur_i++;
                }
            }

            // generate material for this geometry
            SFMaterial material = new SFMaterial();

            string tex_name = GetLakeTextureName(lake.type);
            if(!SFResources.SFResourceManager.Textures.Load(tex_name, SFUnPak.FileSource.ANY, out material.texture, out int ec))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFMapLake.Generate(): Could not load texture (texture name = " + tex_name + ")");
                material.texture = SF3D.SFRender.SFRenderEngine.opaque_tex;
            }
            else
            {
                material.texture.SetWrapMode((int)OpenTK.Graphics.OpenGL.All.Repeat);
            }
            material.casts_shadow = false;
            material.transparent_pass = false;
            if ((lake.type == 0) || (lake.type == 1) || (lake.type == 3))   // water
            {
                material.water_pass = true;
                material.apply_shadow = false;
            }
            else                  // lava
            {
                material.water_pass = false;
                material.apply_shadow = true;
            }
            if (lake.type == 2)   // lava
            {
                material.emission_strength = 1.0f;
                if (!Settings.ToneMapping)
                {
                    material.emission_strength *= 0.8f;
                }

                material.emission_color = new Vector4(1.0f, 0.5f, 0.5f, 1.0f);
            }
            material.matFlags = 0;

            submodel.CreateRaw(vertices, uvs, colors, normals, indices, material);
            submodel.aabb = new SF3D.Physics.BoundingBox(
            new Vector3(area_tl.x, lake_height, map.height - area_tl.y - 1),
            new Vector3(area_br.x, lake_height, map.height - area_br.y - 1));

            SFModel3D mesh = new SFModel3D();
            mesh.CreateRaw(new SFSubModel3D[] { submodel });
            SFResources.SFResourceManager.Models.AddManually(mesh, lake.GetObjectName());

            lake.node.Mesh = mesh;
        }

        public void SetVisibilitySettings()
        {
            foreach (SFMapLake l in lakes)
            {
                l.node.Visible = Settings.LakesVisible;
            }
        }

        public void Dispose()
        {
            index_helper = null;
            lake_type_helper = null;
            foreach (var lake in lakes)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(lake.node);
                DisposeLakeMesh(lake);
            }
        }
    }
}
