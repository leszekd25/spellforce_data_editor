﻿using SFEngine.SFCFF;
using SFEngine.SFMap;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapBuildingEditor : MapEditor
    {
        bool first_click = false;
        public int selected_building { get; set; } = -1;    // unit index
        public int placement_building { get; set; } = 0;

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

            selected_building = index;
        }

        public override void OnMousePress(SFCoord pos, MouseButtons button, ref special_forms.SpecialKeysPressed specials)
        {
            if (map == null)
            {
                return;
            }

            // get unit under position
            SFMapBuilding building = map.FindBuildingApprox(pos);

            // if no unit under the cursor and left mouse clicked, create new unit
            if (building == null)
            {
                if (button == MouseButtons.Left)
                {
                    if (!map.heightmap.CanMoveToPosition(pos))
                    {
                        return;
                    }

                    // undo/redo
                    SFCoord previous_pos = new SFCoord(0, 0);

                    // if dragging unit, just move selected unit, dont create a new one
                    if ((specials.Shift) && (selected_building != -1))
                    {
                        // undo/redo
                        previous_pos = map.building_manager.buildings[selected_building].grid_position;

                        map.building_manager.MoveBuilding(selected_building, pos);
                    }
                    else if (!first_click)
                    {
                        ushort new_building_id = (ushort)placement_building;
                        if (SFCategoryManager.gamedata[2029].GetElementIndex(new_building_id) == -1)
                        {
                            return;
                        }
                        // create new building and drag it until mouse released
                        int bld_index = map.building_manager.AddBuilding(new_building_id, pos, 0, 0, 1, -1);
                        // undo/redo
                        previous_pos = pos;

                        ((map_controls.MapBuildingInspector)MainForm.mapedittool.selected_inspector).LoadNextBuilding(bld_index);
                        Select(bld_index);
                        MainForm.mapedittool.InspectorSelect(map.building_manager.buildings[bld_index]);

                        first_click = true;

                        map.heightmap.RefreshOverlay();
                        MainForm.mapedittool.ui.RedrawMinimapIcons();

                        // undo/redo
                        MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorBuildingAddOrRemove()
                        { building = map.building_manager.buildings[bld_index], index = bld_index, is_adding = true });
                    }

                    // undo/redo
                    if ((selected_building != SFEngine.Utility.NO_INDEX) && (!first_click))
                    {
                        op_change_pos = new map_operators.MapOperatorEntityChangeProperty()
                        {
                            type = map_operators.MapOperatorEntityType.BUILDING,
                            index = selected_building,
                            property = map_operators.MapOperatorEntityProperty.POSITION,
                            PreChangeProperty = previous_pos
                        };
                    }

                    first_click = true;
                }
                else if (button == MouseButtons.Right)
                {
                    Select(SFEngine.Utility.NO_INDEX);
                    MainForm.mapedittool.InspectorSelect(null);
                }
            }
            else
            {
                // find selected unit id
                int building_map_index = map.building_manager.buildings.IndexOf(building);
                if (building_map_index == -1)
                {
                    return;
                }

                if (button == MouseButtons.Left)
                {
                    // if dragging unit, just move selected unit, dont create a new one
                    if ((specials.Shift) && (selected_building != -1))
                    {
                        if (map.heightmap.CanMoveToPosition(pos))
                        {
                            map.building_manager.MoveBuilding(selected_building, pos);
                        }
                    }
                    else
                    {
                        Select(building_map_index);
                        MainForm.mapedittool.InspectorSelect(map.building_manager.buildings[selected_building]);
                    }
                }
                // delete unit
                else if (button == MouseButtons.Right)
                {
                    if (building_map_index == selected_building)
                    {
                        MainForm.mapedittool.InspectorSelect(null);
                    }

                    // undo/redo
                    MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorBuildingAddOrRemove()
                    {
                        building = map.building_manager.buildings[building_map_index],
                        index = building_map_index,
                        is_adding = false
                    });

                    map.building_manager.RemoveBuilding(building_map_index);
                    ((map_controls.MapBuildingInspector)MainForm.mapedittool.selected_inspector).RemoveBuilding(building_map_index);

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
                if (selected_building != -1)
                {
                    // undo/redo
                    if (op_change_pos != null)
                    {
                        op_change_pos.PostChangeProperty = map.building_manager.buildings[selected_building].grid_position;
                        if ((SFCoord)op_change_pos.PreChangeProperty != (SFCoord)op_change_pos.PostChangeProperty)
                        {
                            op_change_pos.Finish(map);
                            MainForm.mapedittool.op_queue.Push(op_change_pos);

                            map.heightmap.RefreshOverlay();
                            MainForm.mapedittool.ui.RedrawMinimapIcons();
                        }
                    }
                    op_change_pos = null;

                    MainForm.mapedittool.InspectorSelect(map.building_manager.buildings[selected_building]);
                }
            }
        }
    }
}
