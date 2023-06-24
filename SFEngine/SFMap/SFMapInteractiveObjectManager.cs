using System.Collections.Generic;

namespace SFEngine.SFMap
{
    public enum SFMapInteractiveObjectType { OTHER = 0, BINDSTONE, MONUMENT }

    public class SFMapInteractiveObject : SFMapEntity
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

        public int AddInteractiveObject(int id, SFCoord position, int angle, int unk_byte, int index = -1)
        {
            map.object_manager.AddObjectCollisionBoundary(id);

            SFMapInteractiveObject obj = new SFMapInteractiveObject();
            obj.grid_position = position;
            obj.game_id = id;
            obj.angle = angle;
            obj.unk_byte = unk_byte;

            if (index == -1)
            {
                index = int_objects.Count;
            }

            int_objects.Insert(index, obj);

            // find out object type and submit metadata
            if (id == 769)
            {
                // find where to put the element
                int new_bindstone_index = 0;
                for (int i = 0; i < index; i++)
                {
                    if (int_objects[i].game_id == 769)
                    {
                        new_bindstone_index += 1;
                    }
                }

                // all bindstone indices that point to a to-be-shifted bindstone are increased
                for (int i = 0; i < bindstones_index.Count; i++)
                {
                    if (bindstones_index[i] >= index)
                    {
                        bindstones_index[i] += 1;
                    }
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
                    {
                        new_monument_index += 1;
                    }
                }

                // all monument indices that point to a to-be-shifted monument are increased
                for (int i = 0; i < monuments_index.Count; i++)
                {
                    if (monuments_index[i] >= index)
                    {
                        monuments_index[i] += 1;
                    }
                }

                monuments_index.Insert(new_monument_index, index);
                int_object_types.Insert(index, SFMapInteractiveObjectType.MONUMENT);
            }
            else
            {
                int_object_types.Insert(index, SFMapInteractiveObjectType.OTHER);
            }

            string obj_name = obj.GetName();
            obj.node = SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(id, obj_name, true, true);
            obj.node.SetParent(map.heightmap.GetChunkNode(position));

            map.heightmap.SetFlag(obj.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, true);
            map.object_manager.ApplyObjectBlockFlags(obj.grid_position, obj.angle, (ushort)obj.game_id, true);

            SF3D.SceneSynchro.SceneNode _obj = obj.node;
            _obj.Position = map.heightmap.GetFixedPosition(position);
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(angle);
            map.UpdateNodeDecal(_obj, new OpenTK.Vector2(position.x, position.y), OpenTK.Vector2.Zero, angle);

            map.heightmap.GetChunk(position).int_objects.Add(obj);

            return index;
        }

        public void RemoveInteractiveObject(int int_obj_index)
        {
            SFMapInteractiveObject int_obj = int_objects[int_obj_index];

            int_objects.RemoveAt(int_obj_index);

            // remove object type metadata
            List<int> index_to_modify = null;
            switch (int_object_types[int_obj_index])
            {
                case SFMapInteractiveObjectType.BINDSTONE:
                    index_to_modify = bindstones_index;
                    break;
                case SFMapInteractiveObjectType.MONUMENT:
                    index_to_modify = monuments_index;
                    break;
            }
            if (index_to_modify != null)
            {
                index_to_modify.Remove(int_obj_index);
                for (int i = 0; i < index_to_modify.Count; i++)
                {
                    if (index_to_modify[i] > int_obj_index)
                    {
                        index_to_modify[i] -= 1;
                    }
                }
            }
            int_object_types.RemoveAt(int_obj_index);

            SF3D.SceneSynchro.SceneNode obj_node = int_obj.node;
            if (obj_node != null)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(obj_node);
            }

            map.heightmap.GetChunk(int_obj.grid_position).int_objects.Remove(int_obj);

            map.heightmap.SetFlag(int_obj.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, false);
            map.object_manager.ApplyObjectBlockFlags(int_obj.grid_position, int_obj.angle, (ushort)int_obj.game_id, true);
        }

        public int MoveInteractiveObject(int int_object_map_index, SFCoord new_pos)
        {
            SFMapInteractiveObject int_obj = int_objects[int_object_map_index];

            // move unit and set chunk dependency
            map.heightmap.GetChunkNode(int_obj.grid_position).MapChunk.int_objects.Remove(int_obj);

            map.object_manager.ApplyObjectBlockFlags(int_obj.grid_position, int_obj.angle, (ushort)int_obj.game_id, false);
            map.heightmap.SetFlag(int_obj.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, false);
            int_obj.grid_position = new_pos;
            map.heightmap.SetFlag(int_obj.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, true);
            map.object_manager.ApplyObjectBlockFlags(int_obj.grid_position, int_obj.angle, (ushort)int_obj.game_id, true);

            map.heightmap.GetChunkNode(int_obj.grid_position).MapChunk.int_objects.Add(int_obj);
            int_obj.node.SetParent(map.heightmap.GetChunkNode(int_obj.grid_position));

            // change visual transform
            float z = map.heightmap.GetZ(new_pos) / 100.0f;
            int_obj.node.Position = map.heightmap.GetFixedPosition(new_pos);
            map.UpdateNodeDecal(int_obj.node, new OpenTK.Vector2(int_obj.grid_position.x, int_obj.grid_position.y), OpenTK.Vector2.Zero, int_obj.angle);

            return 0;
        }

        // todo: probably account for offset?
        public int RotateInteractiveObject(int int_object_map_index, int angle)
        {
            SFMapInteractiveObject int_obj = int_objects[int_object_map_index];

            map.object_manager.ApplyObjectBlockFlags(int_obj.grid_position, int_obj.angle, (ushort)int_obj.game_id, false);
            int_obj.angle = angle;
            map.object_manager.ApplyObjectBlockFlags(int_obj.grid_position, int_obj.angle, (ushort)int_obj.game_id, true);

            SF3D.SceneSynchro.SceneNode _obj = int_obj.node;
            _obj.SetAnglePlane(angle);
            map.UpdateNodeDecal(int_obj.node, new OpenTK.Vector2(int_obj.grid_position.x, int_obj.grid_position.y), OpenTK.Vector2.Zero, int_obj.angle);

            return 0;
        }
    }
}
