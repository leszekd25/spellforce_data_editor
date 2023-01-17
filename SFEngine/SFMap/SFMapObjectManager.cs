using OpenTK;
using System.Collections.Generic;

namespace SFEngine.SFMap
{
    public class SFMapObject : SFMapEntity
    {
        static int max_id = 0;

        public int unknown1 = 0;

        public override string GetName()
        {
            return "OBJECT_" + id.ToString();
        }

        public SFMapObject()
        {
            id = max_id;
            max_id += 1;
        }
    }

    public class SFMapObjectManager
    {
        public List<SFMapObject> objects { get; private set; } = new List<SFMapObject>();
        // object game id, collision boundary
        public Dictionary<ushort, SFMapCollisionBoundary> object_collision { get; private set; } = new Dictionary<ushort, SFMapCollisionBoundary>();
        public SFMap map = null;

        public void AddObjectCollisionBoundary(int id)
        {
            // add new collision boundary (PENDING TESTS!!!!!!!!!!!)
            if (object_collision.ContainsKey((ushort)id))
            {
                return;
            }

            // load building collision data from gamedata
            int col_index = SFCFF.SFCategoryManager.gamedata[2057].GetElementIndex(id);
            if (col_index == Utility.NO_INDEX)
            {
                return;
            }

            SFMapCollisionBoundary cb = new SFMapCollisionBoundary() { origin = Vector2.Zero };
            SFCFF.SFCategoryElementList col_data = SFCFF.SFCategoryManager.gamedata[2057].element_lists[col_index];
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
                cb.polygons.Add(new SFMapCollisionPolygon2D(vertex_list, Vector2.Zero));
            }

            //cb.RebuildModel3D();

            object_collision.Add((ushort)id, cb);
        }

        public SFMapObject AddObject(int id, SFCoord position, int angle, int unk1, int index)
        {
            AddObjectCollisionBoundary(id);

            SFMapObject obj = new SFMapObject();
            obj.grid_position = position;
            obj.game_id = id;
            obj.angle = angle;
            obj.unknown1 = unk1;

            if (index == -1)
            {
                index = objects.Count;
            }

            objects.Insert(index, obj);

            string obj_name = obj.GetName();

            obj.node = SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(id, obj_name, true, true);
            // custom resource mesh setting :^)
            ObjectSetResourceIfAvailable(id, obj.node);

            obj.node.SetParent(map.heightmap.GetChunkNode(position));
            return obj;
        }

        public void RemoveObject(SFMapObject o)
        {
            objects.Remove(o);

            SF3D.SceneSynchro.SceneNode obj_node = o.node;
            if (obj_node != null)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(obj_node);
            }

            map.heightmap.GetChunk(o.grid_position).RemoveObject(o);
        }

        public void ApplyObjectBlockFlags(SFCoord pos, int angle, ushort id, bool set)
        {
            bool blocks_terrain = false;

            int col_index = SFCFF.SFCategoryManager.gamedata[2050].GetElementIndex(id);
            if (col_index != Utility.NO_INDEX)
            {
                blocks_terrain = ((byte)(SFCFF.SFCategoryManager.gamedata[2050][col_index][2]) & 1) == 1;
            }

            if (!object_collision.ContainsKey(id))
            {
                map.heightmap.SetFlag(pos, SFMapHeightMapFlag.ENTITY_OBJECT_COLLISION, set & blocks_terrain);
                return;
            }

            SFMapCollisionBoundary bcb = object_collision[(ushort)id];
            foreach (SFCoord p in bcb.GetCells(pos, angle))
            {
                map.heightmap.SetFlag(p, SFMapHeightMapFlag.ENTITY_OBJECT_COLLISION, set);
            }
        }

        public bool ObjectIDIsReserved(int obj_id)
        {
            if ((obj_id == 65) || (obj_id == 66) || (obj_id == 67))   // editor flags (ignored)
            {
                return true;
            }

            if ((obj_id == 769) || (obj_id == 778))                  // bindstone, world portal
            {
                return true;
            }

            if ((obj_id >= 771) && (obj_id <= 777))                   // monuments
            {
                return true;
            }

            if (obj_id == 2541)                                       // spawn point
            {
                return true;
            }

            return false;
        }

        public void ObjectSetResourceIfAvailable(int obj_id, SF3D.SceneSynchro.SceneNode node)
        {
            string mesh_obj_name = "";
            string mesh_decal_name = "";
            // berries
            if ((obj_id >= 0x80) && (obj_id < 0x80 + 6))
            {
                mesh_obj_name = "nature_berry_" + (obj_id - 0x80 + 1).ToString("00");
                mesh_decal_name = "nature_berry_decal";
            }
            else if (obj_id == 0x300)
            {
                mesh_obj_name = "nature_wheat_step06";
                mesh_decal_name = "nature_wheat_decal";
            }
            else if (obj_id == 0x302)
            {
                mesh_obj_name = "nature_mushroom_06";
            }
            else if ((obj_id >= 0x100) && (obj_id < 0x100 + 9))
            {
                mesh_obj_name = "nature_crushable_rock" + (obj_id - 0x100 + 1).ToString();
                mesh_decal_name = "nature_crushable_rock_decal";
            }
            else if ((obj_id >= 0x580) && (obj_id < 0x580 + 9))
            {
                mesh_obj_name = "nature_lenya_" + (obj_id - 0x580 + 1).ToString("00");
                mesh_decal_name = "nature_lenya_decal";
            }
            else if ((obj_id >= 0x600) && (obj_id < 0x600 + 9))
            {
                mesh_obj_name = "nature_iron_" + (obj_id - 0x600 + 1).ToString("00");
                mesh_decal_name = "nature_iron_decal";
            }
            else if ((obj_id >= 0x680) && (obj_id < 0x680 + 9))
            {
                mesh_obj_name = "nature_mitthril_" + (obj_id - 0x680 + 1).ToString("00");
                mesh_decal_name = "nature_mithril_decal";
            }

            if (mesh_obj_name != "")
            {
                if (node.Children.Count == 1)
                {
                    SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(node.Children[0]);         // remove missing mesh node
                }
                SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeSimple(node, mesh_obj_name, "0");
                SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeSimple(node, mesh_decal_name, "0");
            }
        }
    }
}
