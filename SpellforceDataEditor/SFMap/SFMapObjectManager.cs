using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapObject: SFMapEntity
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
        public SFMap map = null;

        public SFMapObject AddObject(int id, SFCoord position, int angle, int unk1)
        {
            SFMapObject obj = new SFMapObject();
            obj.grid_position = position;
            obj.game_id = id;
            obj.angle = angle;
            obj.unknown1 = unk1;
            objects.Add(obj);

            string obj_name = obj.GetName();

            SF3D.SceneSynchro.SceneNode node = SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(id, obj_name, true);
            // custom resource mesh setting :^)
            if (node.Children.Count == 0)
                ObjectSetResourceIfAvailable(id, node);

            node.SetParent(map.heightmap.GetChunkNode(position));
            return obj;
        }

        public void RemoveObject(SFMapObject o)
        {
            objects.Remove(o);
            
            SF3D.SceneSynchro.SceneNode chunk_node = map.heightmap.GetChunkNode(o.grid_position);
            SF3D.SceneSynchro.SceneNode obj_node = chunk_node.FindNode<SF3D.SceneSynchro.SceneNode>(o.GetName());
            if (obj_node != null)
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(obj_node);

            map.heightmap.GetChunk(o.grid_position).RemoveObject(o);
        }

        public bool ObjectIDIsReserved(int obj_id)
        {
            if ((obj_id == 65) || (obj_id == 66) || (obj_id == 67))   // editor flags (ignored)
                return true;
            if ((obj_id == 769)  || (obj_id == 778))                  // bindstone, world portal
                return true;
            if ((obj_id >= 771) && (obj_id <= 777))                   // monuments
                return true;
            if (obj_id == 2541)                                       // spawn point
                return true;
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
                mesh_obj_name = "nature_mushroom_06";
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
                SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeSimple(node, mesh_obj_name, "0");
                SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeSimple(node, mesh_decal_name, "0");
            }
        }
    }
}
