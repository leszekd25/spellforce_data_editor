using SFEngine.SFCFF;
using SFEngine.SFMap;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapUnitEditor : MapEditor
    {
        bool first_click = false;
        public int selected_unit { get; private set; } = -1;    // unit index
        public int placement_unit { get; set; } = 0;

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

            selected_unit = index;
        }

        public override void OnMousePress(SFCoord pos, MouseButtons b, ref special_forms.SpecialKeysPressed specials)
        {
            if (map == null)
            {
                return;
            }

            // get unit under position
            SFMapUnit unit = map.FindUnit(pos);

            // if no unit under the cursor and left mouse clicked, create new unit
            if (unit == null)
            {
                if (b == MouseButtons.Left)
                {
                    if (!(map.heightmap.CanMoveToPosition(pos)))
                    {
                        return;
                    }

                    // undo/redo
                    SFCoord previous_pos = new SFCoord(0, 0);

                    if ((specials.Shift) && (selected_unit != SFEngine.Utility.NO_INDEX))
                    {
                        // undo/redo
                        previous_pos = map.unit_manager.units[selected_unit].grid_position;

                        map.MoveUnit(selected_unit, pos);
                    }
                    else if (!first_click)
                    {
                        ushort new_unit_id = (ushort)placement_unit;
                        if (SFCategoryManager.gamedata[2024].GetElementIndex(new_unit_id) == SFEngine.Utility.NO_INDEX)
                        {
                            return;
                        }
                        // create new unit and drag it until mouse released
                        map.AddUnit(new_unit_id, pos, 0, 0, 0, 0, 0);
                        // undo/redo
                        previous_pos = pos;

                        ((map_controls.MapUnitInspector)MainForm.mapedittool.selected_inspector).LoadNextUnit(map.unit_manager.units.Count - 1);
                        Select(map.unit_manager.units.Count - 1);
                        MainForm.mapedittool.InspectorSelect(map.unit_manager.units[map.unit_manager.units.Count - 1]);

                        map.heightmap.RefreshOverlay();
                        MainForm.mapedittool.ui.RedrawMinimapIcons();

                        // undo/redo
                        MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorUnitAddOrRemove
                        { unit = map.unit_manager.units[map.unit_manager.units.Count - 1], index = map.unit_manager.units.Count - 1, is_adding = true });
                    }

                    // undo/redo
                    if ((selected_unit != SFEngine.Utility.NO_INDEX) && (!first_click))
                    {
                        op_change_pos = new map_operators.MapOperatorEntityChangeProperty()
                        {
                            type = map_operators.MapOperatorEntityType.UNIT,
                            index = selected_unit,
                            property = map_operators.MapOperatorEntityProperty.POSITION,
                            PreChangeProperty = previous_pos
                        };
                    }

                    first_click = true;
                }
                else if (b == MouseButtons.Right)
                {
                    MainForm.mapedittool.InspectorSelect(null);
                    Select(SFEngine.Utility.NO_INDEX);
                }
            }
            else
            {
                int unit_map_index = map.unit_manager.units.IndexOf(unit);
                if (unit_map_index == -1)
                {
                    return;
                }

                if (b == MouseButtons.Left)
                {
                    // if dragging unit, just move selected unit, dont create a new one
                    if ((specials.Shift) && (selected_unit != -1))
                    {
                        if (map.heightmap.CanMoveToPosition(pos))
                        {
                            map.MoveUnit(selected_unit, pos);
                        }
                    }
                    else
                    {
                        Select(unit_map_index);
                        MainForm.mapedittool.InspectorSelect(map.unit_manager.units[selected_unit]);
                    }
                }
                // delete unit
                else if (b == MouseButtons.Right)
                {
                    if (unit_map_index == selected_unit)
                    {
                        MainForm.mapedittool.InspectorSelect(null);
                        Select(SFEngine.Utility.NO_INDEX);
                    }

                    // undo/redo
                    MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorUnitAddOrRemove
                    { unit = map.unit_manager.units[unit_map_index], index = unit_map_index, is_adding = false });

                    map.DeleteUnit(unit_map_index);
                    ((map_controls.MapUnitInspector)MainForm.mapedittool.selected_inspector).RemoveUnit(unit_map_index);

                    map.heightmap.RefreshOverlay();
                    MainForm.mapedittool.ui.RedrawMinimapIcons();
                }
            }
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                first_click = false;
                if (selected_unit != SFEngine.Utility.NO_INDEX)
                {
                    // undo/redo
                    if (op_change_pos != null)
                    {
                        op_change_pos.PostChangeProperty = map.unit_manager.units[selected_unit].grid_position;
                        if ((SFCoord)op_change_pos.PreChangeProperty != (SFCoord)op_change_pos.PostChangeProperty)
                        {
                            op_change_pos.Finish(map);
                            MainForm.mapedittool.op_queue.Push(op_change_pos);

                            map.heightmap.RefreshOverlay();
                            MainForm.mapedittool.ui.RedrawMinimapIcons();
                        }
                    }
                    op_change_pos = null;

                    MainForm.mapedittool.InspectorSelect(map.unit_manager.units[selected_unit]);
                }
            }
        }
    }
}
