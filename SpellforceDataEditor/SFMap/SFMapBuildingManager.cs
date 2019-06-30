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
        // debug only
        public SF3D.ObjectSimple3D boundary_outline = null;

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
        public SF3D.SFModel3D b_outline { get; private set; } = new SF3D.SFModel3D();

        public SFMapBuildingCollisionBoundary(SFMapCollisionBoundary cb, Vector2 v)
        {
            collision_mesh = cb;
            collision_mesh.SetOffset(v);
            //RebuildModel3D();
        }

        public void RebuildModel3D()
        {
            int seg_count = 0;
            foreach (SFMapCollisionPolygon2D s in collision_mesh.polygons)
                seg_count += s.vertices.Length;

            Vector3[] vertices = new Vector3[seg_count*4];
            Vector2[] uvs = new Vector2[seg_count * 4];
            Vector4[] colors = new Vector4[seg_count * 4];
            Vector3[] normals = new Vector3[seg_count * 4];
            uint[] indices = new uint[seg_count * 6];

            seg_count = 0;
            float line_width = 0.03f;
            Vector2 o = collision_mesh.origin;
            foreach(SFMapCollisionPolygon2D s in collision_mesh.polygons)
            {
                for(int i = 0; i < s.vertices.Length; i++)
                {
                    Vector2 v1 = s.vertices[i];
                    Vector2 v2 = s.vertices[(i + 1) % s.vertices.Length];
                    Vector2 n = ((v2 - v1).Normalized().PerpendicularLeft) * line_width;

                    vertices[(seg_count + i) * 4 + 0] = new Vector3((v1 + n).X - o.X, 1, (v1 + n).Y + o.Y);
                    vertices[(seg_count + i) * 4 + 1] = new Vector3((v1 - n).X - o.X, 1, (v1 - n).Y + o.Y);
                    vertices[(seg_count + i) * 4 + 2] = new Vector3((v2 + n).X - o.X, 1, (v2 + n).Y + o.Y);
                    vertices[(seg_count + i) * 4 + 3] = new Vector3((v2 - n).X - o.X, 1, (v2 - n).Y + o.Y);

                    indices[(seg_count + i) * 6 + 0] = (uint)((seg_count + i) * 4 + 0);
                    indices[(seg_count + i) * 6 + 1] = (uint)((seg_count + i) * 4 + 1);
                    indices[(seg_count + i) * 6 + 2] = (uint)((seg_count + i) * 4 + 2);
                    indices[(seg_count + i) * 6 + 3] = (uint)((seg_count + i) * 4 + 1);
                    indices[(seg_count + i) * 6 + 4] = (uint)((seg_count + i) * 4 + 2);
                    indices[(seg_count + i) * 6 + 5] = (uint)((seg_count + i) * 4 + 3);
                }
                seg_count += s.vertices.Length;
            }
            for(int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(0, 0);
                colors[i] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            }

            b_outline.CreateRaw(vertices, uvs, colors, normals, indices, "");
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
                    org.X = ((short)org_data.get_single_variant(6).value) / 100.0f;
                    org.Y = ((short)org_data.get_single_variant(7).value) / 100.0f;
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
                            vertex_list[i].X = (float)((short)(col_raw[current_col_offset + 4 + i * 2 + 0].value)) / 128;
                            vertex_list[i].Y = -(float)((short)(col_raw[current_col_offset + 4 + i * 2 + 1].value)) / 128;
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
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapBuildingManager.BoundaryFits(): Could not find boundary of building id " + id.ToString());
                return false;
            }

            cells_taken.Clear();
            SFMapBuildingCollisionBoundary bcb = building_collision[(ushort)id];

            bcb.collision_mesh.SetRotation(angle-180);     // why does this work?
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

            // todo: this is broken and MUST be fixed
            /*foreach (SFCoord p in pot_cells_taken)
                map.heightmap.building_data[p.y * map.width + p.x] = (ushort)buildings.Count;*/

            string bld_name = bld.GetObjectName();
            SF3D.SFRender.SFRenderEngine.scene_manager.AddObjectBuilding(id, bld_name);

            /*SFResources.SFResourceManager.Models.AddManually(building_collision[(ushort)id].b_outline, bld_name + "_OUTLINE");
            map.render_engine.scene_manager.AddObjectStatic(bld_name + "_OUTLINE", "", bld_name + "_OUTLINE");

            float _z = map.heightmap.GetZ(position) / 100.0f;
            SF3D.ObjectSimple3D outline_obj = map.render_engine.scene_manager.objects_static[bld_name + "_OUTLINE"];
            outline_obj.Position = new Vector3((float)position.x, (float)_z, (float)(map.height - position.y - 1));
            outline_obj.Rotation = Quaternion.FromEulerAngles(0, (float)(angle*Math.PI/180), 0);*/

            return bld;
        }

        public void RemoveBuilding(SFMapBuilding b)
        {
            RemoveBuildingCollisionBoundary(b.game_id);
            buildings.Remove(b);

            SF3D.SFRender.SFRenderEngine.scene_manager.DeleteObject(b.GetObjectName());

            map.heightmap.GetChunk(b.grid_position).RemoveBuilding(b);
        }
    }
}
