using SFEngine.SFMap;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public enum MonumentType { HUMAN = 0, DWARF, ELF, DARKELF, ORC, TROLL, HERO }

    public class MapMonumentEditor : MapEditor
    {
        bool first_click = false;
        public MonumentType selected_type = MonumentType.HERO;
        public int selected_intobj { get; private set; } = -1;       // interactive object index
        public int selected_monument { get; private set; } = -1;    // spawn index

        // undo/redo
        map_operators.MapOperatorEntityChangeProperty op_change_pos = null;

        public override void Select(int index)
        {
            if (first_click)
            {
                return;
            }

            selected_monument = index;
            selected_intobj = (selected_monument == SFEngine.Utility.NO_INDEX ? SFEngine.Utility.NO_INDEX : map.int_object_manager.monuments_index[index]);
        }

        public override void OnMousePress(SFCoord pos, MouseButtons b, ref special_forms.SpecialKeysPressed specials)
        {
            // 1. find clicked bindstone if it exists
            int intobj_index = -1;
            int monument_index = -1;
            SFMapInteractiveObject int_obj = null;

            for (int i = 0; i < map.int_object_manager.monuments_index.Count; i++)
            {
                int j = map.int_object_manager.monuments_index[i];
                if (SFCoord.Distance(map.int_object_manager.int_objects[j].grid_position, pos) <= 2)
                {
                    intobj_index = j;
                    monument_index = i;
                    int_obj = map.int_object_manager.int_objects[j];
                    break;
                }
            }

            // 2. if not clicked, create new bindstone
            if (int_obj == null)
            {
                if (b == MouseButtons.Left)
                {
                    // undo/redo
                    SFCoord previous_pos = new SFCoord(0, 0);

                    if ((specials.Shift) && (selected_intobj != SFEngine.Utility.NO_INDEX))
                    {
                        if (map.heightmap.CanMoveToPosition(pos))
                        {
                            // undo/redo
                            previous_pos = map.int_object_manager.int_objects[selected_intobj].grid_position;

                            map.MoveInteractiveObject(selected_intobj, pos);
                        }
                    }
                    else if (!first_click)
                    {
                        int new_object_id = 771 + (int)selected_type;

                        // slot count?
                        byte unk_byte = 1;
                        if (new_object_id == 777)
                        {
                            unk_byte = 5;
                        }

                        map.AddInteractiveObject(new_object_id, pos, 0, unk_byte);
                        // undo/redo
                        previous_pos = pos;

                        ((map_controls.MapMonumentInspector)MainForm.mapedittool.selected_inspector).LoadNextMonument(map.int_object_manager.monuments_index.Count - 1);
                        Select(map.int_object_manager.monuments_index.Count - 1);
                        MainForm.mapedittool.InspectorSelect(
                            map.int_object_manager.int_objects[map.int_object_manager.int_objects.Count - 1]);

                        MainForm.mapedittool.ui.RedrawMinimapIcons();

                        // undo/redo
                        MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorMonumentAddOrRemove()
                        {
                            intobj = map.int_object_manager.int_objects[map.int_object_manager.int_objects.Count - 1],
                            intobj_index = map.int_object_manager.int_objects.Count - 1,
                            monument_index = map.int_object_manager.monuments_index.Count - 1,
                            is_adding = true
                        });
                    }

                    // undo/redo
                    if ((selected_intobj != SFEngine.Utility.NO_INDEX) && (!first_click))
                    {
                        op_change_pos = new map_operators.MapOperatorEntityChangeProperty()
                        {
                            type = map_operators.MapOperatorEntityType.MONUMENT,
                            index = selected_monument,
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
                        if (map.heightmap.CanMoveToPosition(pos))
                        {
                            map.MoveInteractiveObject(selected_intobj, pos);
                        }
                    }
                    else
                    {
                        Select(monument_index);
                        MainForm.mapedittool.InspectorSelect(
                            map.int_object_manager.int_objects[selected_intobj]);
                    }
                }
                else if (b == MouseButtons.Right)
                {
                    if (selected_intobj == intobj_index)
                    {
                        Select(SFEngine.Utility.NO_INDEX);
                        MainForm.mapedittool.InspectorSelect(null);
                    }

                    // undo/redo
                    MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorMonumentAddOrRemove()
                    {
                        intobj = map.int_object_manager.int_objects[intobj_index],
                        intobj_index = intobj_index,
                        monument_index = monument_index,
                        is_adding = false
                    });

                    map.DeleteInteractiveObject(intobj_index);
                    ((map_controls.MapMonumentInspector)MainForm.mapedittool.selected_inspector).RemoveMonument(monument_index);

                    MainForm.mapedittool.ui.RedrawMinimapIcons();
                }
            }
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                first_click = false;
                if (selected_monument != -1)
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
