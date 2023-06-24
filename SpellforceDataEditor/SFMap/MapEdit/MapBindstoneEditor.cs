using SFEngine.SFMap;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapBindstoneEditor : MapEditor
    {
        bool first_click = false;
        public int selected_intobj { get; private set; } = -1;       // interactive object index
        public int selected_bindstone { get; private set; } = -1;    // spawn index

        // undo/redo
        map_operators.MapOperatorEntityChangeProperty op_change_pos = null;

        // select entity
        // if editor is being currently used, selection fails
        public override void Select(int index)
        {
            if (first_click)
            {
                return;
            }

            selected_bindstone = index;
            selected_intobj = (selected_bindstone == SFEngine.Utility.NO_INDEX ? SFEngine.Utility.NO_INDEX : map.int_object_manager.bindstones_index[index]);
        }

        public override void OnMousePress(SFCoord pos, MouseButtons b, ref special_forms.SpecialKeysPressed specials)
        {
            // 1. find clicked bindstone if it exists
            int intobj_index = -1;
            int bindstone_index = -1;
            SFMapInteractiveObject int_obj = null;

            for (int i = 0; i < map.int_object_manager.bindstones_index.Count; i++)
            {
                int j = map.int_object_manager.bindstones_index[i];
                if (SFCoord.Distance(map.int_object_manager.int_objects[j].grid_position, pos) <= 2)
                {
                    intobj_index = j;
                    bindstone_index = i;
                    int_obj = map.int_object_manager.int_objects[j];
                    break;
                }
            }

            // 2. if not clicked, create new bindstone
            if (int_obj == null)
            {
                if (b == MouseButtons.Left)
                {
                    if (!map.heightmap.CanMoveToPosition(pos))
                    {
                        return;
                    }

                    // undo/redo
                    SFCoord previous_pos = new SFCoord(0, 0);

                    if ((specials.Shift) && (selected_intobj != SFEngine.Utility.NO_INDEX))
                    {
                        // undo/redo
                        previous_pos = map.int_object_manager.int_objects[selected_intobj].grid_position;

                        int player = map.metadata.FindPlayerByBindstoneIndex(selected_bindstone);

                        map.int_object_manager.MoveInteractiveObject(selected_intobj, pos);
                        if (player != -1)
                        {
                            map.metadata.spawns[player].pos = pos;
                        }
                    }
                    else if (!first_click)
                    {
                        int obj_index = map.int_object_manager.AddInteractiveObject(769, pos, 0, 1);
                        // undo/redo
                        previous_pos = pos;

                        map.metadata.spawns.Add(new SFMapSpawn());
                        map.metadata.spawns[map.metadata.spawns.Count - 1].pos = pos;
                        map.metadata.spawns[map.metadata.spawns.Count - 1].bindstone_index = map.int_object_manager.bindstones_index.Count - 1;

                        ((map_controls.MapBindstoneInspector)MainForm.mapedittool.selected_inspector).LoadNextBindstone(map.int_object_manager.bindstones_index.Count - 1);
                        Select(map.int_object_manager.bindstones_index.Count - 1);
                        MainForm.mapedittool.InspectorSelect(
                            map.int_object_manager.int_objects[obj_index]);


                        MainForm.mapedittool.ui.RedrawMinimapIcons();

                        MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorBindstoneAddOrRemove()
                        {
                            bindstone_index = map.int_object_manager.bindstones_index.Count - 1,
                            bindstone_pos = pos,
                            bindstone_textid = 0,
                            bindstone_unknown = 0,
                            intobj_index = obj_index,
                            player_index = map.metadata.spawns.Count - 1,
                            is_adding = true
                        });

                        map.heightmap.RefreshOverlay();
                    }

                    // undo/redo
                    if ((selected_bindstone != SFEngine.Utility.NO_INDEX) && (!first_click))
                    {
                        op_change_pos = new map_operators.MapOperatorEntityChangeProperty()
                        {
                            type = map_operators.MapOperatorEntityType.BINDSTONE,
                            index = selected_bindstone,
                            property = map_operators.MapOperatorEntityProperty.POSITION,
                            PreChangeProperty = previous_pos
                        };
                    }

                    first_click = true;

                }
                else if (b == MouseButtons.Right)
                {
                    Select(SFEngine.Utility.NO_INDEX);
                    MainForm.mapedittool.InspectorSelect(null);
                }
            }
            else
            {
                if (b == MouseButtons.Left)
                {
                    if ((specials.Shift) && (selected_intobj != -1))
                    {
                        if (!(map.heightmap.CanMoveToPosition(pos)))
                        {
                            return;
                        }

                        // selected_intobj out of bounds!
                        int player = map.metadata.FindPlayerByBindstoneIndex(selected_bindstone);

                        map.int_object_manager.MoveInteractiveObject(selected_intobj, pos);
                        if (player != -1)
                        {
                            map.metadata.spawns[player].pos = pos;
                        }
                    }
                    else
                    {
                        Select(bindstone_index);
                        MainForm.mapedittool.InspectorSelect(
                            map.int_object_manager.int_objects[selected_intobj]);
                    }
                }
                else if (b == MouseButtons.Right)
                {
                    int player = map.metadata.FindPlayerByBindstoneIndex(bindstone_index);

                    if (!map.metadata.IsPlayerActive(player))
                    {
                        if (selected_intobj == intobj_index)
                        {
                            Select(SFEngine.Utility.NO_INDEX);
                            MainForm.mapedittool.InspectorSelect(null);
                        }

                        MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorBindstoneAddOrRemove()
                        {
                            bindstone_index = bindstone_index,
                            bindstone_pos = int_obj.grid_position,
                            bindstone_textid = map.metadata.spawns[player].text_id,
                            intobj_index = intobj_index,
                            bindstone_unknown = map.metadata.spawns[player].unknown,
                            player_index = player,
                            is_adding = false,
                            ApplyOnPush = true
                        });
                    }
                }
            }
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                first_click = false;
                if (selected_bindstone != -1)
                {
                    // undo/redo
                    if (op_change_pos != null)
                    {
                        op_change_pos.PostChangeProperty = map.int_object_manager.int_objects[selected_intobj].grid_position;
                        if ((SFCoord)op_change_pos.PreChangeProperty != (SFCoord)op_change_pos.PostChangeProperty)
                        {
                            op_change_pos.Finish(map);
                            MainForm.mapedittool.op_queue.Push(op_change_pos);

                            map.heightmap.RefreshOverlay();
                            MainForm.mapedittool.ui.RedrawMinimapIcons();
                        }
                    }
                    op_change_pos = null;

                    MainForm.mapedittool.InspectorSelect(map.int_object_manager.int_objects[selected_intobj]);
                }
            }
        }
    }
}
