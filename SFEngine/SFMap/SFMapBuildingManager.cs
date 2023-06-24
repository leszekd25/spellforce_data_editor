using OpenTK;
using System.Collections.Generic;
using System;

namespace SFEngine.SFMap
{
    public class SFMapBuilding : SFMapEntity
    {
        static int max_id = 0;

        public int level = 1;
        public int race_id = 0;

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


        public int AddBuilding(int id, SFCoord position, int angle, int npc_id, int level, int race_id, int index = -1)
        {
            AddBuildingCollisionBoundary(id);

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

            map.heightmap.SetFlag(bld.grid_position, SFMapHeightMapFlag.ENTITY_BUILDING, true);
            ApplyBuildingBlockFlags(bld, true);

            Vector2 b_offset = building_collision[(ushort)bld.game_id].origin;
            Vector2 b_offset_rotated = MathUtils.RotateVec2(b_offset, (float)(angle * Math.PI / 180));

            bld.node.Position = map.heightmap.GetFixedPosition(position) + new Vector3(-b_offset_rotated.X, 0, b_offset_rotated.Y);
            bld.node.Scale = new Vector3(100 / 128f);
            bld.node.SetAnglePlane(angle);
            map.UpdateNodeDecal(bld.node, new Vector2(position.x, position.y), b_offset, angle);

            map.heightmap.GetChunk(position).buildings.Add(bld);

            return index;
        }

        public void RemoveBuilding(int bld_index)
        {
            SFMapBuilding building = buildings[bld_index];
            buildings.RemoveAt(bld_index);

            if (building.node != null)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(building.node);
            }

            map.heightmap.GetChunk(building.grid_position).buildings.Remove(building);

            map.heightmap.SetFlag(building.grid_position, SFMapHeightMapFlag.ENTITY_BUILDING, false);
            ApplyBuildingBlockFlags(building, false);
        }

        public void ReplaceBuilding(int building_map_index, ushort new_building_id)
        {
            SFMapBuilding building = buildings[building_map_index];

            ApplyBuildingBlockFlags(building, false);
            if (building.node != null)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(building.node);
            }

            AddBuildingCollisionBoundary(new_building_id);
            building.node = SF3D.SFRender.SFRenderEngine.scene.AddSceneBuilding(new_building_id, building.GetName());
            building.node.SetParent(map.heightmap.GetChunkNode(building.grid_position));

            building.game_id = new_building_id;
            ApplyBuildingBlockFlags(building, true);

            Vector2 b_offset = map.building_manager.building_collision[(ushort)building.game_id].origin;
            Vector2 b_offset_rotated = MathUtils.RotateVec2(b_offset, (float)(building.angle * Math.PI / 180));

            building.node.Position = map.heightmap.GetFixedPosition(building.grid_position) + new Vector3(b_offset_rotated.X, 0, +b_offset_rotated.Y);
            building.node.Scale = new Vector3(100 / 128f);
            building.node.SetAnglePlane(building.angle);
            map.UpdateNodeDecal(building.node, new Vector2(building.grid_position.x, building.grid_position.y), b_offset, building.angle);
        }

        public void RotateBuilding(int building_map_index, int angle)
        {
            SFMapBuilding building = buildings[building_map_index];

            ApplyBuildingBlockFlags(building, false);
            building.angle = angle;
            ApplyBuildingBlockFlags(building, true);

            Vector2 b_offset = building_collision[(ushort)building.game_id].origin;
            Vector2 b_offset_rotated = MathUtils.RotateVec2(b_offset, (float)(angle * Math.PI / 180));

            building.node.Position = map.heightmap.GetFixedPosition(building.grid_position) + new Vector3(-b_offset_rotated.X, 0, +b_offset_rotated.Y);
            building.node.Scale = new Vector3(100 / 128f);
            building.node.SetAnglePlane(building.angle);
            map.UpdateNodeDecal(building.node, new Vector2(building.grid_position.x, building.grid_position.y), b_offset, building.angle);
        }

        public void MoveBuilding(int building_map_index, SFCoord new_pos)
        {
            SFMapBuilding building = buildings[building_map_index];

            // move unit and set chunk dependency
            map.heightmap.GetChunkNode(building.grid_position).MapChunk.buildings.Remove(building);
            ApplyBuildingBlockFlags(building, false);
            map.heightmap.SetFlag(building.grid_position, SFMapHeightMapFlag.ENTITY_BUILDING, false);
            building.grid_position = new_pos;
            map.heightmap.SetFlag(building.grid_position, SFMapHeightMapFlag.ENTITY_BUILDING, true);
            ApplyBuildingBlockFlags(building, true);
            map.heightmap.GetChunkNode(building.grid_position).MapChunk.buildings.Add(building);
            building.node.SetParent(map.heightmap.GetChunkNode(building.grid_position));

            // change visual transform
            Vector2 b_offset = building_collision[(ushort)building.game_id].origin;
            Vector2 b_offset_rotated = MathUtils.RotateVec2(b_offset, (float)(building.angle * Math.PI / 180));

            building.node.Position = map.heightmap.GetFixedPosition(new_pos) + new Vector3(-b_offset_rotated.X, 0, +b_offset_rotated.Y);
            building.node.Scale = new Vector3(100 / 128f);
            building.node.SetAnglePlane(building.angle);
            map.UpdateNodeDecal(building.node, new Vector2(building.grid_position.x, building.grid_position.y), b_offset, building.angle);
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
    }
}
