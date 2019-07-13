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
        public HashSet<SFCoord> boundary = new HashSet<SFCoord>();   // boundary is all cells exactly 1 cell away from lake
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


        // the closest cells which do NOT belong to lake
        public void RecalculateBoundary()
        {
            boundary = new HashSet<SFCoord>();
            foreach(SFCoord p in cells)
            {
                if (!cells.Contains(new SFCoord(p.x + 1, p.y)))
                    boundary.Add(new SFCoord(p.x + 1, p.y));
                if (!cells.Contains(new SFCoord(p.x - 1, p.y)))
                    boundary.Add(new SFCoord(p.x - 1, p.y));
                if (!cells.Contains(new SFCoord(p.x, p.y + 1)))
                    boundary.Add(new SFCoord(p.x, p.y + 1));
                if (!cells.Contains(new SFCoord(p.x, p.y - 1)))
                    boundary.Add(new SFCoord(p.x, p.y - 1));
            }
        }
        
        public int GetDistanceFromBoundary(SFCoord pos)
        {
            int min_dist = 30000;  // arbitrary big number
            foreach (SFCoord p in boundary)
                min_dist = (int)Math.Min(min_dist, SFCoord.DistanceManhattan(pos, p));
            return min_dist;
        }
    }
    
    public class SFMapLakeManager
    {
        public List<SFMapLake> lakes { get; private set; } = new List<SFMapLake>();
        public SFMap map = null;

        public SFMapLake AddLake(SFCoord start, short z_diff, int type)
        {
            SFMapLake lake = new SFMapLake();
            lakes.Add(lake);
            lake.start = start;
            lake.z_diff = z_diff;
            lake.type = type;
            lake.cells = map.heightmap.GetIslandByHeight(start, z_diff);
            foreach(SFCoord p in lake.cells)
            {
                map.heightmap.lake_data[p.y * map.width + p.x] = (byte)(lakes.Count);   // position of lake on the list + 1
            }

            var map_nodes = map.heightmap.GetAreaMapNodes(lake.cells);
            foreach (var node in map_nodes)
                node.MapChunk.RebuildLake();

            lake.RecalculateBoundary();
            
            string obj_name = lake.GetObjectName();
            // TODO: make lake meshes per chunk
            //SF3D.SFRender.SFRenderEngine.scene.AddObjectStatic(obj_name, "", obj_name);
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

        public void UpdateLake(SFMapLake lake)
        {
            int lake_index = lakes.IndexOf(lake);
            if (lake_index < 0)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapLakeManager.UpdateLake(): Lake not found in lake table!");
                return;
            }

            lake.cells = map.heightmap.GetIslandByHeight(lake.start, lake.z_diff);
            for (int i = 0; i < map.width * map.height; i++)
                if (map.heightmap.lake_data[i] == lake_index + 1)
                    map.heightmap.lake_data[i] = 0;

            foreach (SFCoord p in lake.cells)
                map.heightmap.lake_data[p.y * map.width + p.x] = (byte)(lake_index+1);

            var map_nodes = map.heightmap.GetAreaMapNodes(lake.cells);
            foreach (var node in map_nodes)
                node.MapChunk.RebuildLake();

            lake.RecalculateBoundary();
        }
    }
}
