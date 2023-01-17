using OpenTK;
using System.Collections.Generic;

namespace SFEngine.SFMap
{
    public class SFMapBuilding : SFMapEntity
    {
        static int max_id = 0;

        public int level = 1;
        public int race_id = 0;
        // debug only
        public SF3D.SceneSynchro.SceneNodeSimple boundary_outline = null;

        public override string GetName()
        {
            return "BUILDING_" + id.ToString();
        }

        public SFMapBuilding()
        {
            id = max_id;
            max_id += 1;
        }
    }

    public class SFMapBuildingManager
    {
        public List<SFMapBuilding> buildings { get; private set; } = new List<SFMapBuilding>();
        // building game id, collision boundary
        public Dictionary<ushort, SFMapCollisionBoundary> building_collision { get; private set; } = new Dictionary<ushort, SFMapCollisionBoundary>();
        public SFMap map = null;

        public void AddBuildingCollisionBoundary(int id)
        {
            // add new collision boundary (PENDING TESTS!!!!!!!!!!!)
            if (building_collision.ContainsKey((ushort)id))
            {
                return;
            }

            // load building origin vector
            Vector2 org = new Vector2(0, 0);
            int org_index = SFCFF.SFCategoryManager.gamedata[2029].GetElementIndex(id);
            if (org_index != -1)
            {
                SFCFF.SFCategoryElement org_data = SFCFF.SFCategoryManager.gamedata[2029][org_index]; // 6, 7
                org.X = ((short)org_data[6]) / 140.0f;
                org.Y = ((short)org_data[7]) / 140.0f;
            }

            // load building collision data from gamedata
            int col_index = SFCFF.SFCategoryManager.gamedata[2030].GetElementIndex(id);
            if (col_index == Utility.NO_INDEX)
            {
                return;
            }

            SFMapCollisionBoundary cb = new SFMapCollisionBoundary() { origin = org };
            SFCFF.SFCategoryElementList col_data = SFCFF.SFCategoryManager.gamedata[2030].element_lists[col_index];
            for (int i = 0; i < col_data.Elements.Count; i++)
            {
                SFCFF.SFOutlineData outline = (SFCFF.SFOutlineData)(col_data[i][3]);
                int vertex_count = outline.Data.Count / 2;
                Vector2[] vertex_list = new Vector2[vertex_count];
                for (int j = 0; j < vertex_count; j++)
                {
                    vertex_list[j] = new Vector2();
                    vertex_list[j].X = outline.Data[j * 2 + 0] / 140.0f;
                    vertex_list[j].Y = outline.Data[j * 2 + 1] / 140.0f;
                }
                cb.polygons.Add(new SFMapCollisionPolygon2D(vertex_list, org));
            }

            //cb.RebuildModel3D();

            building_collision.Add((byte)id, cb);
        }


        public SFMapBuilding AddBuilding(int id, SFCoord position, int angle, int npc_id, int level, int race_id, int index)
        {
            AddBuildingCollisionBoundary(id);
            HashSet<SFCoord> pot_cells_taken = new HashSet<SFCoord>();

            // todo: this is broken and MUST be fixed
            //if(!BoundaryFits(id, position, angle, pot_cells_taken))
            //{
            //RemoveBuildingCollisionBoundary(id);
            //return null;
            //}

            SFMapBuilding bld = new SFMapBuilding();
            bld.grid_position = position;
            bld.game_id = id;
            bld.angle = angle;
            bld.npc_id = npc_id;
            bld.level = level;
            bld.race_id = race_id;
            if (race_id == -1)
            {
                // find race ID in gamedata
                int bld_id = SFCFF.SFCategoryManager.gamedata[2029].GetElementIndex(id);
                bld.race_id = (byte)SFCFF.SFCategoryManager.gamedata[2029][bld_id][1];
            }

            if (index == -1)
            {
                index = buildings.Count;
            }

            buildings.Insert(index, bld);

            string bld_name = bld.GetName();
            bld.node = SF3D.SFRender.SFRenderEngine.scene.AddSceneBuilding(id, bld_name);
            bld.node.SetParent(map.heightmap.GetChunkNode(position));

            //SF3D.SceneSynchro.SceneNodeSimple node = SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeSimple(bld.node, building_collision[(ushort)id].b_outline.GetName(), "_OUTLINE_");
            //node.Scale = new Vector3(1.4f / 1.28f);

            return bld;
        }

        public void RemoveBuilding(SFMapBuilding b)
        {
            buildings.Remove(b);

            if (b.node != null)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(b.node);
            }

            map.heightmap.GetChunk(b.grid_position).RemoveBuilding(b);
        }

        public void ApplyBuildingBlockFlags(SFMapBuilding b, bool set)
        {
            if (!building_collision.ContainsKey((ushort)b.game_id))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapBuildingManager.ApplyBuildingBlockFlags(): Could not find boundary of building id " + b.id.ToString());
                return;
            }

            SFMapCollisionBoundary bcb = building_collision[(ushort)b.game_id];
            foreach (SFCoord p in bcb.GetCells(b.grid_position, b.angle))
            {
                map.heightmap.SetFlag(p, SFMapHeightMapFlag.ENTITY_BUILDING_COLLISION, set);
            }
        }

        // get rid of collision meshes
        public void Dispose()
        {

        }
    }
}
