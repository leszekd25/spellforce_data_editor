﻿using SFEngine.SFCFF;
using SFEngine.SFMap;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapObjectEditor : MapEditor
    {
        bool first_click = false;
        public int selected_object { get; private set; } = -1;    // unit index
        public int placement_object { get; set; } = 0;

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

            selected_object = index;
        }

        public override void OnMousePress(SFCoord pos, MouseButtons button, ref special_forms.SpecialKeysPressed specials)
        {
            if (map == null)
            {
                return;
            }

            SFMapObject obj = map.FindObjectApprox(pos);

            // if no unit under the cursor and left mouse clicked, create new unit
            if (obj == null)
            {
                if (button == MouseButtons.Left)
                {
                    if (map.heightmap.IsFlagSet(pos, SFMapHeightMapFlag.ENTITY_OBJECT))
                    {
                        return;
                    }

                    // undo/redo
                    SFCoord previous_pos = new SFCoord(0, 0);

                    // if dragging unit, just move selected unit, dont create a new one
                    if ((specials.Shift) && (selected_object != SFEngine.Utility.NO_INDEX))
                    {
                        // undo/redo
                        previous_pos = map.object_manager.objects[selected_object].grid_position;

                        map.object_manager.MoveObject(selected_object, pos);
                    }
                    else if (!first_click)
                    {
                        ushort new_object_id = (ushort)placement_object;
                        if (SFCategoryManager.gamedata[2050].GetElementIndex(new_object_id) == SFEngine.Utility.NO_INDEX)
                        {
                            return;
                        }
                        // create new unit and drag it until mouse released

                        int obj_index = map.object_manager.AddObject(new_object_id, pos, 0, 0, 0);
                        // undo/redo
                        previous_pos = pos;

                        ((map_controls.MapObjectInspector)MainForm.mapedittool.selected_inspector).LoadNextObject(obj_index);
                        Select(obj_index);

                        special_forms.MapEditorForm.AngleInfo angle_info = MainForm.mapedittool.GetAngleInfo();
                        UInt16 angle = angle_info.angle;
                        if (angle_info.random)
                        {
                            angle = (UInt16)(SFEngine.MathUtils.Rand() % 360);
                        }

                        map.object_manager.RotateObject(selected_object, angle);

                        MainForm.mapedittool.InspectorSelect(map.object_manager.objects[selected_object]);


                        MainForm.mapedittool.ui.RedrawMinimapIcons();

                        // undo/redo
                        MainForm.mapedittool.op_queue.OpenCluster();
                        MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorObjectAddOrRemove()
                        { obj = map.object_manager.objects[selected_object], index = selected_object, is_adding = true });
                        MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
                        { type = map_operators.MapOperatorEntityType.OBJECT, index = selected_object, property = map_operators.MapOperatorEntityProperty.ANGLE, PreChangeProperty = 0, PostChangeProperty = (int)angle });
                        MainForm.mapedittool.op_queue.CloseCluster();

                        map.heightmap.RefreshOverlay();
                    }

                    // undo/redo
                    if ((selected_object != SFEngine.Utility.NO_INDEX) && (!first_click))
                    {
                        op_change_pos = new map_operators.MapOperatorEntityChangeProperty()
                        {
                            type = map_operators.MapOperatorEntityType.OBJECT,
                            index = selected_object,
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
                int object_map_index = map.object_manager.objects.IndexOf(obj);
                if (object_map_index == -1)
                {
                    return;
                }

                if (button == MouseButtons.Left)
                {
                    // if dragging unit, just move selected unit, dont create a new one
                    if ((specials.Shift) && (selected_object != -1))
                    {
                        if (!map.heightmap.IsFlagSet(pos, SFMapHeightMapFlag.ENTITY_OBJECT))
                        {
                            map.object_manager.MoveObject(selected_object, pos);
                        }
                    }
                    else
                    {
                        Select(object_map_index);
                        MainForm.mapedittool.InspectorSelect(map.object_manager.objects[selected_object]);
                    }
                }
                // delete unit
                else if (button == MouseButtons.Right)
                {
                    if (object_map_index == selected_object)
                    {
                        Select(SFEngine.Utility.NO_INDEX);
                        MainForm.mapedittool.InspectorSelect(null);
                    }

                    // undo/redo
                    MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorObjectAddOrRemove()
                    {
                        obj = map.object_manager.objects[object_map_index],
                        index = object_map_index,
                        is_adding = false
                    });

                    map.object_manager.RemoveObject(object_map_index);
                    ((map_controls.MapObjectInspector)MainForm.mapedittool.selected_inspector).RemoveObject(object_map_index);

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
                if (selected_object != -1)
                {
                    // undo/redo
                    if (op_change_pos != null)
                    {
                        op_change_pos.PostChangeProperty = map.object_manager.objects[selected_object].grid_position;
                        if ((SFCoord)op_change_pos.PreChangeProperty != (SFCoord)op_change_pos.PostChangeProperty)
                        {
                            op_change_pos.Finish(map);
                            MainForm.mapedittool.op_queue.Push(op_change_pos);

                            map.heightmap.RefreshOverlay();
                            MainForm.mapedittool.ui.RedrawMinimapIcons();
                        }
                    }
                    op_change_pos = null;

                    MainForm.mapedittool.InspectorSelect(map.object_manager.objects[selected_object]);
                }
            }
        }
    }
}
