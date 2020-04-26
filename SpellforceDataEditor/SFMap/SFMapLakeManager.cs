using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapLake
    {
        static int max_id = 0;

        public HashSet<SFCoord> cells = new HashSet<SFCoord>();
        public SFCoord start;
        public short z_diff;
        public int type;
        public int id;

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
            foreach(SFCoord p in cells)
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
        public SFMap map = null;

        public SFMapLake AddLake(SFCoord start, short z_diff, int type)
        {
            ushort lake_level = (ushort)(map.heightmap.GetZ(start) + z_diff);

            SFMapLake lake = new SFMapLake();
            lakes.Add(lake);
            lake.start = start;
            lake.z_diff = z_diff;
            lake.type = type;

            UpdateLake(lake);

            if(lake.cells.Count == 0)
            {
                RemoveLake(lake);
                return null;
            }
            else
                return lake;
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
                map.heightmap.lake_data[p.y * map.width + p.x] = 0;

            for (int i = 0; i < map.width * map.height; i++)
                if (map.heightmap.lake_data[i] > lake_index + 1)
                    map.heightmap.lake_data[i] -= 1;

            lakes.Remove(lake);
            
            var map_nodes = map.heightmap.GetAreaMapNodes(lake.cells);
            foreach (var node in map_nodes)
                node.MapChunk.RebuildLake();
        }

        public string GetLakeTextureName(int type)
        {
            string tex_name = "";
            if (type == 0)
                tex_name = "landscape_lake_water";
            else if (type == 1)
                tex_name = "landscape_swamp_l8";
            else if (type == 2)
                tex_name = "landscape_lake_lava1";
            else if (type == 3)
                tex_name = "landscape_lake_ice_l8";
            return tex_name;
        }

        public System.Drawing.Color GetLakeMinimapColor(int type)
        {
            if (type == 0)
                return System.Drawing.Color.FromArgb(255, 100, 100, 200);
            else if (type == 1)
                return System.Drawing.Color.FromArgb(255, 175, 125, 90);
            else if (type == 2)
                return System.Drawing.Color.FromArgb(255, 240, 140, 40);
            else if (type == 3)
                return System.Drawing.Color.FromArgb(255, 240, 240, 255);
            return System.Drawing.Color.FromArgb(255, 0, 0, 0);
        }

        // should be updated after lake_start or lake_depth was modified
        public void UpdateLake(SFMapLake lake)
        {
            int lake_index = lakes.IndexOf(lake);
            if (lake_index < 0)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapLakeManager.UpdateLake(): Lake not found in lake table!");
                return;
            }

            ushort lake_level = (ushort)(map.heightmap.GetZ(lake.start) + lake.z_diff);

            lake.cells = map.heightmap.GetIslandByHeight(lake.start, lake.z_diff);
            lake.CalculateDepth(map.heightmap, lake_level);

            for (int i = 0; i < map.width * map.height; i++)
                if (map.heightmap.lake_data[i] == lake_index + 1)
                    map.heightmap.lake_data[i] = 0;

            // check if lake collides with other lakes
            List<SFMapLake> lakes_to_remove = new List<SFMapLake>();
            foreach(SFMapLake l in lakes)
            {
                if (l == lake)
                    continue;

                // if overlap is detected, this MUST mean that l is fully contained in lake
                if (l.cells.Overlaps(lake.cells))
                    lakes_to_remove.Add(l);
            }
            foreach (SFMapLake l in lakes_to_remove)
                RemoveLake(l);
            lake_index = lakes.IndexOf(lake);

            foreach (SFCoord p in lake.cells)
                map.heightmap.lake_data[p.y * map.width + p.x] = (byte)(lake_index+1);

            var map_nodes = map.heightmap.GetAreaMapNodes(lake.cells);
            foreach (var node in map_nodes)
                node.MapChunk.RebuildLake();
        }
    }
}
