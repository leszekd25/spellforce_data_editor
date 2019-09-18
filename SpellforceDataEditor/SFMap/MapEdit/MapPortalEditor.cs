using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapPortalEditor: MapEditor
    {
        bool drag_enabled = false;
        public int selected_portal { get; private set; } = -1;    // spawn index

        public override void OnMousePress(SFCoord pos, MouseButtons b)
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
                    if (drag_enabled == true)
                    {
                        if(map.heightmap.CanMoveToPosition(pos))
                            map.MovePortal(selected_portal, pos);
                    }
                    else
                    {
                        // check if can place
                        if (map.heightmap.CanMoveToPosition(pos))
                        {
                            map.AddPortal(0, pos, 0);

                            ((map_controls.MapPortalInspector)MainForm.mapedittool.selected_inspector).LoadNextPortal();
                            selected_portal = map.portal_manager.portals.Count - 1;
                            MainForm.mapedittool.InspectorSelect(map.portal_manager.portals[selected_portal]);

                            drag_enabled = true;
                        }
                    }
                }
            }
            else
            {
                if (b == MouseButtons.Left)
                {
                    // if dragging unit, just move selected unit, dont create a new one
                    if (drag_enabled)
                    {
                        if (map.heightmap.CanMoveToPosition(pos))
                            map.MovePortal(selected_portal, pos);
                    }
                    else
                    {
                        // find selected coop camp index
                        int portal_map_index = map.portal_manager.portals.IndexOf(portal);
                        if (portal_map_index == -1)
                            return;

                        selected_portal = portal_map_index;
                        MainForm.mapedittool.InspectorSelect(portal);

                        drag_enabled = true;
                    }
                }
                // delete unit
                else if (b == MouseButtons.Right)
                {
                    int portal_map_index = map.portal_manager.portals.IndexOf(portal);
                    if (portal_map_index == -1)
                        return;

                    if (portal_map_index == selected_portal)
                        MainForm.mapedittool.InspectorSelect(null);

                    map.DeletePortal(portal_map_index);
                    ((map_controls.MapPortalInspector)MainForm.mapedittool.selected_inspector).RemovePortal(portal_map_index);
                }
            }
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                drag_enabled = false;
                if (selected_portal != -1)
                    MainForm.mapedittool.InspectorSelect(map.portal_manager.portals[selected_portal]);
            }
        }
    }
}
