using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap
{
    public enum SFMapInteractiveObjectType { OTHER = 0, COOP_CAMP, BINDSTONE, MONUMENT }

    public class SFMapInteractiveObject: SFMapEntity
    {
        static int max_id = 0;

        public int unk_byte = 0;

        public override string GetName()
        {
            return "INT_OBJECT_" + id.ToString();
        }

        public SFMapInteractiveObject()
        {
            id = max_id;
            max_id += 1;
        }
    }

    public class SFMapInteractiveObjectManager
    {
        public List<SFMapInteractiveObject> int_objects { get; private set; } = new List<SFMapInteractiveObject>();
        public List<SFMapInteractiveObjectType> int_object_types { get; private set; } = new List<SFMapInteractiveObjectType>();
        public List<int> coop_camps_index { get; private set; } = new List<int>();
        public List<int> bindstones_index { get; private set; } = new List<int>();
        public List<int> monuments_index { get; private set; } = new List<int>();
        public SFMap map = null;

        public SFMapInteractiveObject AddInteractiveObject(int id, SFCoord position, int angle, int unk_byte)
        {
            SFMapInteractiveObject obj = new SFMapInteractiveObject();
            obj.grid_position = position;
            obj.game_id = id;
            obj.angle = angle;
            obj.unk_byte = unk_byte;
            int_objects.Add(obj);

            // find out object type and submit metadata
            if (id == 2541)
            {
                coop_camps_index.Add(int_objects.Count - 1);
                int_object_types.Add(SFMapInteractiveObjectType.COOP_CAMP);
            }
            else if (id == 769)
            {
                bindstones_index.Add(int_objects.Count - 1);
                int_object_types.Add(SFMapInteractiveObjectType.BINDSTONE);
            }
            else if ((id >= 771) && (id <= 777))
            {
                monuments_index.Add(int_objects.Count - 1);
                int_object_types.Add(SFMapInteractiveObjectType.MONUMENT);
            }
            else
                int_object_types.Add(SFMapInteractiveObjectType.OTHER);

            string obj_name = obj.GetName();
            SF3D.SceneSynchro.SceneNode node = SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(id, obj_name, true);
            node.SetParent(map.heightmap.GetChunkNode(position));
            return obj;
        }

        public void RemoveInteractiveObject(SFMapInteractiveObject int_obj)
        {
            int obj_index = int_objects.IndexOf(int_obj);
            int_objects.Remove(int_obj);

            // remove object type metadata
            List<int> index_to_modify = null;
            switch(int_object_types[obj_index])
            {
                case SFMapInteractiveObjectType.COOP_CAMP:
                    index_to_modify = coop_camps_index;
                    break;
                case SFMapInteractiveObjectType.BINDSTONE:
                    index_to_modify = bindstones_index;
                    break;
                case SFMapInteractiveObjectType.MONUMENT:
                    index_to_modify = monuments_index;
                    break;
            }
            if(index_to_modify != null)
            {
                index_to_modify.Remove(obj_index);
                for (int i = 0; i < index_to_modify.Count; i++)
                    if (index_to_modify[i] > obj_index)
                        index_to_modify[i] -= 1;
            }
            int_object_types.RemoveAt(obj_index);

            SF3D.SceneSynchro.SceneNode chunk_node = map.heightmap.GetChunkNode(int_obj.grid_position);
            SF3D.SceneSynchro.SceneNode obj_node = chunk_node.FindNode<SF3D.SceneSynchro.SceneNode>(int_obj.GetName());
            if (obj_node != null)
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(obj_node);

            map.heightmap.GetChunk(int_obj.grid_position).RemoveInteractiveObject(int_obj);
        }
    }
}
