using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap
{
    public enum SFMapInteractiveObjectType { OTHER = 0, BINDSTONE, MONUMENT }

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
        public List<int> bindstones_index { get; private set; } = new List<int>();
        public List<int> monuments_index { get; private set; } = new List<int>();
        public SFMap map = null;

        public SFMapInteractiveObject AddInteractiveObject(int id, SFCoord position, int angle, int unk_byte, int index)
        {
            SFMapInteractiveObject obj = new SFMapInteractiveObject();
            obj.grid_position = position;
            obj.game_id = id;
            obj.angle = angle;
            obj.unk_byte = unk_byte;

            if (index == -1)
                index = int_objects.Count;
            int_objects.Insert(index, obj);

            // find out object type and submit metadata
            if (id == 769)
            {
                // find where to put the element
                int new_bindstone_index = 0;
                for(int i = 0; i < index; i++)
                {
                    if (int_objects[i].game_id == 769)
                        new_bindstone_index += 1;
                }

                // all bindstone indices that point to a to-be-shifted bindstone are increased
                for (int i = 0; i < bindstones_index.Count; i++)
                {
                    if (bindstones_index[i] >= index)
                        bindstones_index[i] += 1;
                }

                bindstones_index.Insert(new_bindstone_index, index);
                int_object_types.Insert(index, SFMapInteractiveObjectType.BINDSTONE);
            }
            else if ((id >= 771) && (id <= 777))
            {                
                // find where to put the element
                int new_monument_index = 0;
                for (int i = 0; i < index; i++)
                {
                    if ((int_objects[i].game_id >= 771) && (int_objects[i].game_id <= 777))
                        new_monument_index += 1;
                }

                // all monument indices that point to a to-be-shifted monument are increased
                for (int i = 0; i < monuments_index.Count; i++)
                {
                    if (monuments_index[i] >= index)
                        monuments_index[i] += 1;
                }

                monuments_index.Insert(new_monument_index, index);
                int_object_types.Insert(index, SFMapInteractiveObjectType.MONUMENT);
            }
            else
                int_object_types.Insert(index, SFMapInteractiveObjectType.OTHER);

            string obj_name = obj.GetName();
            obj.node = SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(id, obj_name, true);
            obj.node.SetParent(map.heightmap.GetChunkNode(position));

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

            SF3D.SceneSynchro.SceneNode obj_node = int_obj.node;
            if (obj_node != null)
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(obj_node);

            map.heightmap.GetChunk(int_obj.grid_position).RemoveInteractiveObject(int_obj); 
        }
    }
}
