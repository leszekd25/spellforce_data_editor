using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapBuilding
    {
        static int max_id = 0;

        public SFCoord grid_position = new SFCoord(0, 0);
        public int id = -1;
        public int game_id = -1;
        public int angle = 0;
        public int npc_id = 0;
        public int level = 1;
        public int race_id = 0;

        public string GetObjectName()
        {
            return "BUILDING_" + id.ToString();
        }

        public SFMapBuilding()
        {
            id = max_id;
            max_id += 1;
        }

        public override string ToString()
        {
            return GetObjectName();
        }
    }

    public class SFMapBuildingCollisionBoundary
    {
        public int reference_count = 1;
        public SFMapCollisionBoundary collision_mesh { get; private set; } = null;

        public SFMapBuildingCollisionBoundary(SFMapCollisionBoundary cb, Vector2 v)
        {
            collision_mesh = cb;
            collision_mesh.SetOffset(v);
        }
    }

    public class SFMapBuildingManager
    {
        public List<SFMapBuilding> buildings { get; private set; } = new List<SFMapBuilding>();
        // building game id, collision boundary
        public Dictionary<ushort, SFMapBuildingCollisionBoundary> building_collision { get; private set; } = new Dictionary<ushort, SFMapBuildingCollisionBoundary>();
        public SFMap map = null;

        public void AddBuildingCollisionBoundary(int id)
        {
            // add new collision boundary (PENDING TESTS!!!!!!!!!!!)
            if (building_collision.ContainsKey((ushort)id))
                building_collision[(ushort)id].reference_count += 1;
            else
            {
                SFMapCollisionBoundary cb = new SFMapCollisionBoundary();
                // load building origin vector
                Vector2 org = new Vector2(0, 0);
                int org_index = map.gamedata.categories[23].get_element_index(id);
                if(org_index != -1)
                {
                    SFCFF.SFCategoryElement org_data = map.gamedata.categories[23].get_element(org_index); // 6, 7
                    org.X = ((short)org_data.get_single_variant(6).value) / 128.0f;
                    org.Y = ((short)org_data.get_single_variant(7).value) / 128.0f;
                }
                // load building collision data from gamedata
                int col_index = map.gamedata.categories[24].get_element_index(id);
                if (col_index != -1)
                {
                    SFCFF.SFCategoryElement col_data = map.gamedata.categories[24].get_element(col_index);
                    List<SFCFF.SFVariant> col_raw = col_data.get();

                    int current_col_offset = 0;
                    while (current_col_offset < col_raw.Count)
                    {
                        int vertex_count = (byte)col_raw[current_col_offset + 3].value;

                        Vector2[] vertex_list = new Vector2[vertex_count];
                        for (int i = 0; i < vertex_count; i++)
                        {
                            vertex_list[i] = new Vector2();
                            vertex_list[i].X = (float)((short)(col_raw[current_col_offset + 4 + i * 2 + 0].value)) / 100;
                            vertex_list[i].Y = (float)((short)(col_raw[current_col_offset + 4 + i * 2 + 1].value)) / 100;
                        }
                        cb.AddPolygon(new SFMapCollisionPolygon2D(vertex_list, org));

                        current_col_offset += 4 + 2 * vertex_count;
                    }

                    building_collision.Add((byte)id, new SFMapBuildingCollisionBoundary(cb, org));
                }
            }
        }

        public void RemoveBuildingCollisionBoundary(int id)
        {
            if (building_collision.ContainsKey((ushort)id))
            {
                building_collision[(ushort)id].reference_count -= 1;
                if (building_collision[(ushort)id].reference_count == 0)
                    building_collision.Remove((ushort)id);
            }
        }

        public bool BoundaryFits(int id, SFCoord pos, int angle, HashSet<SFCoord> cells_taken)
        {
            if (!building_collision.ContainsKey((ushort)id))
                return false;

            cells_taken.Clear();
            SFMapBuildingCollisionBoundary bcb = building_collision[(ushort)id];

            bcb.collision_mesh.SetRotation(angle);
            foreach (SFCoord p in bcb.collision_mesh.interior_cells)
            {
                cells_taken.Add(p + pos);
            }

            foreach (SFCoord p in cells_taken)
                if (!map.heightmap.CanMoveToPosition(p))
                    return false;

            return true;
        }

        public SFMapBuilding AddBuilding(int id, SFCoord position, int angle, int npc_id, int level, int race_id)
        {
            AddBuildingCollisionBoundary(id);
            HashSet<SFCoord> pot_cells_taken = new HashSet<SFCoord>();

            // todo: this is broken and MUST be fixed
            if(!BoundaryFits(id, position, angle, pot_cells_taken))
            {
                //RemoveBuildingCollisionBoundary(id);
                //return null;
            }

            SFMapBuilding bld = new SFMapBuilding();
            bld.grid_position = position;
            bld.game_id = id;
            bld.angle = angle;
            bld.npc_id = npc_id;
            bld.level = level;
            bld.race_id = race_id;
            buildings.Add(bld);

            foreach (SFCoord p in pot_cells_taken)
                map.heightmap.building_data[p.y * map.width + p.x] = (ushort)(buildings.Count - 1);

            string bld_name = bld.GetObjectName();
            map.render_engine.scene_manager.AddObjectBuilding(id, bld_name);
            return bld;
        }
    }
}
