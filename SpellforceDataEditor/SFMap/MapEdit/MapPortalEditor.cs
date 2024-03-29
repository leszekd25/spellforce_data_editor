﻿using SFEngine.SFMap;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapPortalEditor : MapEditor
    {
        bool first_click = false;
        public int selected_portal { get; private set; } = -1;    // spawn index

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

            selected_portal = index;
        }

        public override void OnMousePress(SFCoord pos, MouseButtons b, ref special_forms.SpecialKeysPressed specials)
        {
            SFMapPortal portal = null;

            foreach (SFMapPortal _portal in map.portal_manager.portals)
            {
                if (SFCoord.Distance(_portal.grid_position, pos) <= 3)   // since spawn size is 16
                {
                    portal = _portal;
                    break;
                }
            }

            // if no unit under the cursor and left mouse clicked, create new unit
            if (portal == null)
            {
                if (b == MouseButtons.Left)
                {
                    if (!map.heightmap.CanMoveToPosition(pos))
                    {
                        return;
                    }

                    // undo/redo
                    SFCoord previous_pos = new SFCoord(0, 0);

                    if ((specials.Shift) && (selected_portal != SFEngine.Utility.NO_INDEX))
                    {
                        // undo/redo
                        previous_pos = map.portal_manager.portals[selected_portal].grid_position;

                        map.portal_manager.MovePortal(selected_portal, pos);
                    }
                    else if (!first_click)
                    {
                        int ptl_index = map.portal_manager.AddPortal(0, pos, 0);
                        // undo/redo
                        previous_pos = pos;

                        ((map_controls.MapPortalInspector)MainForm.mapedittool.selected_inspector).LoadNextPortal(ptl_index);
                        Select(ptl_index);
                        MainForm.mapedittool.InspectorSelect(map.portal_manager.portals[selected_portal]);

                        map.heightmap.RefreshOverlay();
                        MainForm.mapedittool.ui.RedrawMinimapIcons();

                        // undo/redo
                        MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorPortalAddOrRemove()
                        { portal = map.portal_manager.portals[ptl_index], index = ptl_index, is_adding = true });
                    }

                    // undo/redo
                    if ((selected_portal != SFEngine.Utility.NO_INDEX) && (!first_click))
                    {
                        op_change_pos = new map_operators.MapOperatorEntityChangeProperty()
                        {
                            type = map_operators.MapOperatorEntityType.PORTAL,
                            index = selected_portal,
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
                // find selected coop camp index
                int portal_map_index = map.portal_manager.portals.IndexOf(portal);
                if (portal_map_index == -1)
                {
                    return;
                }

                if (b == MouseButtons.Left)
                {
                    // if dragging unit, just move selected unit, dont create a new one
                    if ((specials.Shift) && (selected_portal != -1))
                    {
                        if (map.heightmap.CanMoveToPosition(pos))
                        {
                            map.portal_manager.MovePortal(selected_portal, pos);
                        }
                    }
                    else
                    {
                        Select(portal_map_index);
                        MainForm.mapedittool.InspectorSelect(portal);
                    }
                }
                // delete unit
                else if (b == MouseButtons.Right)
                {
                    if (portal_map_index == selected_portal)
                    {
                        Select(SFEngine.Utility.NO_INDEX);
                        MainForm.mapedittool.InspectorSelect(null);
                    }

                    // undo/redo
                    MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorPortalAddOrRemove()
                    {
                        portal = map.portal_manager.portals[portal_map_index],
                        index = portal_map_index,
                        is_adding = false
                    });

                    map.portal_manager.RemovePortal(portal_map_index);
                    ((map_controls.MapPortalInspector)MainForm.mapedittool.selected_inspector).RemovePortal(portal_map_index);

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
                if (selected_portal != -1)
                {
                    // undo/redo
                    if (op_change_pos != null)
                    {
                        op_change_pos.PostChangeProperty = map.portal_manager.portals[selected_portal].grid_position;
                        if ((SFCoord)op_change_pos.PreChangeProperty != (SFCoord)op_change_pos.PostChangeProperty)
                        {
                            op_change_pos.Finish(map);
                            MainForm.mapedittool.op_queue.Push(op_change_pos);

                            map.heightmap.RefreshOverlay();
                            MainForm.mapedittool.ui.RedrawMinimapIcons();
                        }
                    }
                    op_change_pos = null;

                    MainForm.mapedittool.InspectorSelect(map.portal_manager.portals[selected_portal]);
                }
            }
        }
    }
}
