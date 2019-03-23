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

        public List<SFCoord> cells = new List<SFCoord>();
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
            for(int i = 0; i < cells.Count; i++)
            {
                SFCoord pos = cells[i];
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
            }

            SF3D.SFModel3D lake_mesh = new SF3D.SFModel3D();
            lake_mesh.CreateRaw(vertices, uvs, colors, normals, indices, "landscape_lake_water");
            SFResources.SFResourceManager.Models.AddManually(lake_mesh, GetObjectName());

            System.Diagnostics.Debug.WriteLine("LAKE COUNT: " + cells.Count);
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
            lake.Generate(map);   // creates a lake mesh

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

    }
}
