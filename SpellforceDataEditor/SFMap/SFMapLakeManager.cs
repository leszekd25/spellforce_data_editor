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

        public void Generate(SFMap map)
        {
            if (SFResources.SFResourceManager.Models.Get(GetObjectName()) != null)
                SFResources.SFResourceManager.Models.Dispose(GetObjectName());

            float flatten_factor = 100;
            Vector3[] vertices = new Vector3[cells.Count*4];
            Vector2[] uvs = new Vector2[cells.Count * 4];
            Vector4[] colors = new Vector4[cells.Count * 4];
            Vector3[] normals = new Vector3[cells.Count * 4];
            uint[] indices = new uint[cells.Count * 6];

            short z = (short)(map.heightmap.GetZ(start) + z_diff);
            float real_z = z / flatten_factor;
            int size = map.width;
            // for each coordinate, create a rectangle in world space
            int i = 0;
            foreach(SFCoord pos in cells)
            {
                vertices[i * 4 + 0] = new Vector3((float)(pos.x), real_z, ((float)size) - (float)(pos.y) - 1);
                vertices[i * 4 + 1] = new Vector3((float)(pos.x+1), real_z, ((float)size) - (float)(pos.y) - 1);
                vertices[i * 4 + 2] = new Vector3((float)(pos.x), real_z, ((float)size) - (float)(pos.y+1) - 1);
                vertices[i * 4 + 3] = new Vector3((float)(pos.x+1), real_z, ((float)size) - (float)(pos.y+1) - 1);
                uvs[i * 4 + 0] = new Vector2(0.0f, 0.0f);
                uvs[i * 4 + 1] = new Vector2(1.0f, 0.0f);
                uvs[i * 4 + 2] = new Vector2(0.0f, 1.0f);
                uvs[i * 4 + 3] = new Vector2(1.0f, 1.0f);
                colors[i * 4 + 0] = new Vector4(1, 1, 1, 1);
                colors[i * 4 + 1] = new Vector4(1, 1, 1, 1);
                colors[i * 4 + 2] = new Vector4(1, 1, 1, 1);
                colors[i * 4 + 3] = new Vector4(1, 1, 1, 1);
                normals[i * 4 + 0] = new Vector3(0, 1, 0);
                normals[i * 4 + 1] = new Vector3(0, 1, 0);
                normals[i * 4 + 2] = new Vector3(0, 1, 0);
                normals[i * 4 + 3] = new Vector3(0, 1, 0);
                indices[i * 6 + 0] = (uint)(i * 4 + 0);
                indices[i * 6 + 1] = (uint)(i * 4 + 1);
                indices[i * 6 + 2] = (uint)(i * 4 + 2);
                indices[i * 6 + 3] = (uint)(i * 4 + 1);
                indices[i * 6 + 4] = (uint)(i * 4 + 2);
                indices[i * 6 + 5] = (uint)(i * 4 + 3);
                i++;
            }

            string tex_name = map.lake_manager.GetLakeTextureName(type);

            SF3D.SFModel3D lake_mesh = new SF3D.SFModel3D();
            lake_mesh.CreateRaw(vertices, uvs, colors, normals, indices, tex_name);
            SFResources.SFResourceManager.Models.AddManually(lake_mesh, GetObjectName());
        }

        public void RecalculateBoundary()
        {
            boundary = new HashSet<SFCoord>();
            foreach(SFCoord p in cells)
            {
                if (!cells.Contains(new SFCoord(p.x + 1, p.y)))
                    boundary.Add(p);
                if (!cells.Contains(new SFCoord(p.x - 1, p.y)))
                    boundary.Add(p);
                if (!cells.Contains(new SFCoord(p.x, p.y + 1)))
                    boundary.Add(p);
                if (!cells.Contains(new SFCoord(p.x, p.y - 1)))
                    boundary.Add(p);
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
        public List<bool> lake_visible { get; private set; } = new List<bool>();
        public SFMap map = null;

        public SFMapLakeManager()
        {
            SFResources.SFResourceManager.Textures.Load("landscape_lake_water");
        }

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
            lake.Generate(map);   // creates a lake mesh
            lake.RecalculateBoundary();

            // add new visibility flag for render engine
            lake_visible.Add(false);
            // add a flag to each heightmap chunk that a new lake was generated
            foreach(SFMapHeightMapChunk chunk in map.heightmap.chunks)
                chunk.lakes_contained.Add(false);
            // update chunk lake visibility by looping each lake coordinate
            foreach(SFCoord pos in lake.cells)
            {
                SFMapHeightMapChunk chunk = map.heightmap.GetChunk(pos);
                chunk.lakes_contained[lakes.Count - 1] = true;
            }

            string obj_name = lake.GetObjectName();
            map.render_engine.scene_manager.AddObjectStatic(obj_name, "", obj_name);
            return lake;
        }

        public void RemoveLake(SFMapLake lake)
        {
            int lake_index = lakes.IndexOf(lake);
            if (lake_index < 0)
                return;
            
            foreach (SFCoord p in lake.cells)
                map.heightmap.lake_data[p.y * map.width + p.x] = 0;

            for (int i = 0; i < map.width * map.height; i++)
                if (map.heightmap.lake_data[i] > lake_index + 1)
                    map.heightmap.lake_data[i] -= 1;

            lake_visible.RemoveAt(lake_index);

            foreach (SFMapHeightMapChunk chunk in map.heightmap.chunks)
                chunk.lakes_contained.RemoveAt(lake_index);

            string obj_name = lake.GetObjectName();
            map.render_engine.scene_manager.DeleteObject(obj_name);

            lakes.Remove(lake);
        }

        public string GetLakeTextureName(int type)
        {
            string tex_name = "";
            if (type == 0)
                tex_name = "landscape_lake_water";
            else if (type == 1)
                tex_name = "landscape_lake_lava1";
            else if (type == 2)
                tex_name = "landscape_lake_swamp";
            return tex_name;
        }

        public void UpdateLake(SFMapLake lake)
        {
            int lake_index = lakes.IndexOf(lake);
            if (lake_index < 0)
                return;

            lake.cells = map.heightmap.GetIslandByHeight(lake.start, lake.z_diff);
            for (int i = 0; i < map.width * map.height; i++)
                if (map.heightmap.lake_data[i] == lake_index + 1)
                    map.heightmap.lake_data[i] = 0;

            foreach (SFCoord p in lake.cells)
                map.heightmap.lake_data[p.y * map.width + p.x] = (byte)(lake_index+1);
            lake.Generate(map);   // creates a lake mesh
            lake.RecalculateBoundary();
            
            foreach (SFMapHeightMapChunk chunk in map.heightmap.chunks)
                chunk.lakes_contained[lake_index] = false;
            foreach (SFCoord pos in lake.cells)
            {
                SFMapHeightMapChunk chunk = map.heightmap.GetChunk(pos);
                chunk.lakes_contained[lake_index] = true;
            }

            string obj_name = lake.GetObjectName();
            map.render_engine.scene_manager.objects_static[obj_name].Mesh =
                SFResources.SFResourceManager.Models.Get(obj_name);
        }
    }
}
