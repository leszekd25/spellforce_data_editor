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

        // ugly 4th parameter to make undo/redo work
        public SFMapLake AddLake(SFCoord start, short z_diff, int type, int lake_index = -1)
        {
            if (lake_index == -1)
                lake_index = lakes.Count;

            ushort lake_level = (ushort)(map.heightmap.GetZ(start) + z_diff);

            // shift lakes forward by one, to preserve lake ordering
            for (int i = 0; i < map.width * map.height; i++)
                if (map.heightmap.lake_data[i] >= lake_index + 1)
                    map.heightmap.lake_data[i] += 1;

            SFMapLake lake = new SFMapLake();
            lakes.Insert(lake_index, lake);
            lake.start = start;
            lake.z_diff = z_diff;
            lake.type = type;

            UpdateLake(lake);

            if (lake.cells.Count == 0)
            {
                RemoveLake(lake);
                return null;
            }
            /*else
            {
                lake_index = lakes.IndexOf(lake);
                if ((MainForm.mapedittool != null) && (MainForm.mapedittool.op_queue != null) && (MainForm.mapedittool.op_queue.IsClusterOpen()))
                {
                    map_operators.MapOperatorLake op_lake = new map_operators.MapOperatorLake() { pos = start, z_diff = z_diff, type = type, lake_index = lake_index, change_add = true };
                    MainForm.mapedittool.op_queue.Push(op_lake);
                }
                return lake;
            }*/
            return lake;
        }

        public int GetLakeIndexAt(SFCoord pos)
        {
            return map.heightmap.lake_data[pos.x + pos.y * map.width] - 1;
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

            // shift lakes back by one, to preserve lake ordering
            for (int i = 0; i < map.width * map.height; i++)
                if (map.heightmap.lake_data[i] > lake_index + 1)
                    map.heightmap.lake_data[i] -= 1;

            if ((lake.cells.Count > 0) && (MainForm.mapedittool != null) && (MainForm.mapedittool.op_queue != null) && (MainForm.mapedittool.op_queue.IsClusterOpen()))
            {
                map_operators.MapOperatorLake op_lake = new map_operators.MapOperatorLake() { pos = lake.start, z_diff = lake.z_diff, type = lake.type, lake_index = lake_index, change_add = false };
                MainForm.mapedittool.op_queue.Push(op_lake);
            }
            lakes.Remove(lake);

            RebuildLake(lake);
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

            RebuildLake(lake);
        }

        // updates all heightmap nodes (rebuilds lake mesh for each of those)
        public void RebuildLake(SFMapLake lake)
        {
            var map_nodes = map.heightmap.GetAreaMapNodes(lake.cells);
            foreach (var node in map_nodes)
                node.MapChunk.RebuildLake();
        }
    }
}
