using OpenTK;
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

        public void CalculateDepth(SFMapHeightMap hmap, ushort lake_level)
        {
            z_diff = (short)(lake_level - hmap.GetZ(cells.ElementAt(0)));
            foreach (SFCoord p in cells)
            {
                if (z_diff < (short)(lake_level - hmap.GetZ(p)))
                {
                    z_diff = (short)(lake_level - hmap.GetZ(p));
                    start = p;
                }
            }
        }
    }

    public class SFMapLakeManager
    {
        public List<SFMapLake> lakes { get; private set; } = new List<SFMapLake>();
        public List<bool> lake_visible { get; private set; } = new List<bool>();

        public SFMap map = null;

        public const int LAKE_SHALLOW_DEPTH = 50;

        // ugly 4th parameter to make undo/redo work
        // uglier 5th and 6th parameters to make undo/redo work (assumed to not be null
        public SFMapLake AddLake(SFCoord start, short z_diff, int type, int lake_index, List<SFMapLake> consumed_lakes, List<int> consumed_lakes_indices)
        {
            if (lake_index == -1)
            {
                lake_index = lakes.Count;
            }

            ushort lake_level = (ushort)(map.heightmap.GetZ(start) + z_diff);

            SFMapLake lake = new SFMapLake();
            lakes.Insert(lake_index, lake);
            lake_visible.Insert(lake_index, true);
            lake.start = start;
            lake.z_diff = z_diff;
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
            if (lake.node.Mesh != null)
            {
                SFResources.SFResourceManager.Models.Dispose(lake.node.Mesh.Name);
                lake.node.Mesh = null;
            }
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
            }
            foreach (SFCoord p in lake.shore)
            {
                map.heightmap.SetFlag(p, SFMapHeightMapFlag.LAKE_SHORE, false);
            }

            ushort lake_level = (ushort)(map.heightmap.GetZ(lake.start) + lake.z_diff);

            lake.cells = map.heightmap.GetIslandByHeight(lake.start, lake.z_diff, out lake.shore);
            lake.CalculateDepth(map.heightmap, lake_level);

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

            lake_index = lakes.IndexOf(lake);

            ushort lake_z = (ushort)(map.heightmap.GetZ(lake.start) + lake.z_diff);
            foreach (SFCoord p in lake.cells)
            {
                short lake_cell_z_diff = (short)(lake_z - map.heightmap.GetZ(p));
                map.heightmap.SetFlag(p, (lake_cell_z_diff < LAKE_SHALLOW_DEPTH ? SFMapHeightMapFlag.LAKE_SHALLOW : SFMapHeightMapFlag.LAKE_DEEP), true);
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

            // generate mesh
            SFSubModel3D submodel = new SFSubModel3D();
            int v_count = (lake.cells.Count + lake.shore.Count) * 4;
            int i_count = (lake.cells.Count + lake.shore.Count) * 6;

            int k = 0;
            Vector3[] vertices = new Vector3[v_count];
            Vector2[] uvs = new Vector2[v_count];
            byte[] colors = new byte[v_count * 4];
            Vector3[] normals = new Vector3[v_count];
            uint[] indices = new uint[i_count];

            ushort lake_z = (ushort)(map.heightmap.GetZ(lake.start) + lake.z_diff);
            ushort[] lake_cell_z = new ushort[4];
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
            // generate geometry for each lake type
            foreach (HashSet<SFCoord> set in new HashSet<SFCoord>[] { lake.cells, lake.shore })
            {
                foreach (SFCoord pos in set)
                {
                    lake_cell_z[0] = map.heightmap.GetZ(pos);
                    lake_cell_z[1] = map.heightmap.GetZ(pos + new SFCoord(1, 0));
                    lake_cell_z[2] = map.heightmap.GetZ(pos + new SFCoord(0, -1));
                    lake_cell_z[3] = map.heightmap.GetZ(pos + new SFCoord(1, -1));

                    vertices[k * 4 + 0] = new Vector3((float)(pos.x), lake_height, (float)(map.height - pos.y - 1));
                    vertices[k * 4 + 1] = new Vector3((float)(pos.x + 1), lake_height, (float)(map.height - pos.y - 1));
                    vertices[k * 4 + 2] = new Vector3((float)(pos.x), lake_height, (float)(map.height - pos.y));
                    vertices[k * 4 + 3] = new Vector3((float)(pos.x + 1), lake_height, (float)(map.height - pos.y));
                    uvs[k * 4 + 0] = new Vector2(pos.x / 4.0f, pos.y / 4.0f);
                    uvs[k * 4 + 1] = new Vector2((pos.x + 1) / 4.0f, pos.y / 4.0f);
                    uvs[k * 4 + 2] = new Vector2(pos.x / 4.0f, (pos.y + 1) / 4.0f);
                    uvs[k * 4 + 3] = new Vector2((pos.x + 1) / 4.0f, (pos.y + 1) / 4.0f);
                    for (int i = 0; i < 16; i += 4)
                    {
                        short lake_cell_z_diff = (short)(lake_z - lake_cell_z[i / 4]);
                        colors[k * 16 + i + 0] = colR;
                        colors[k * 16 + i + 1] = colG;
                        colors[k * 16 + i + 2] = colB;
                        colors[k * 16 + i + 3] = (byte)((colA / 256.0f) * (lake_cell_z_diff < LAKE_SHALLOW_DEPTH ? shallow_lake : deep_lake));
                    }
                    normals[k * 4 + 0] = new Vector3(0, 1, 0);
                    normals[k * 4 + 1] = new Vector3(0, 1, 0);
                    normals[k * 4 + 2] = new Vector3(0, 1, 0);
                    normals[k * 4 + 3] = new Vector3(0, 1, 0);
                    indices[k * 6 + 0] = (uint)(k * 4 + 0);
                    indices[k * 6 + 1] = (uint)(k * 4 + 2);
                    indices[k * 6 + 2] = (uint)(k * 4 + 1);
                    indices[k * 6 + 3] = (uint)(k * 4 + 1);
                    indices[k * 6 + 4] = (uint)(k * 4 + 2);
                    indices[k * 6 + 5] = (uint)(k * 4 + 3);

                    k += 1;
                }
            }
            // generate material for this geometry
            SFMaterial material = new SFMaterial();

            string tex_name = GetLakeTextureName(lake.type);
            SFTexture tex = null;
            int tex_code = SFResources.SFResourceManager.Textures.Load(tex_name, SFUnPak.FileSource.ANY);
            if ((tex_code != 0) && (tex_code != -1))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFMapLake.Generate(): Could not load texture (texture name = " + tex_name + ")");
                tex = SF3D.SFRender.SFRenderEngine.opaque_tex;
            }
            else
            {
                tex = SFResources.SFResourceManager.Textures.Get(tex_name);
                tex.SetWrapMode((int)OpenTK.Graphics.OpenGL.All.Repeat);
            }
            material.texture = tex;
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
            foreach (var lake in lakes)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(lake.node);
                DisposeLakeMesh(lake);
            }
        }
    }
}
