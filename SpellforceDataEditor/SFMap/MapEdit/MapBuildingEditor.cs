using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapBuildingEditor: MapEditor
    {
        bool drag_enabled = false;
        public int selected_building { get; set; } = -1;    // unit index
        public int placement_building { get; set; } = 0;

        public override void OnMousePress(SFCoord pos, MouseButtons button)
        {
            if (map == null)
                return;

            // get unit under position
            SFMapBuilding building = map.FindBuildingApprox(pos);

            // if no unit under the cursor and left mouse clicked, create new unit
            if (building == null)
            {
                if (button == MouseButtons.Left)
                {
                    // if dragging unit, just move selected unit, dont create a new one
                    if (drag_enabled)
                    {
                        //if (map.heightmap.CanMoveToPosition(fixed_pos))
                        map.MoveBuilding(selected_building, pos);
                    }
                    else
                    {
                        // check if can place
                        //if (map.heightmap.CanMoveToPosition(fixed_pos))
                        //{
                        ushort new_building_id = (ushort)placement_building;
                        if (map.gamedata[23].GetElementIndex(new_building_id) == -1)
                            return;
                        // create new building and drag it until mouse released
                        map.AddBuilding(new_building_id, pos, 0, 0, 1, -1);
                        ((map_controls.MapBuildingInspector)MainForm.mapedittool.selected_inspector).LoadNextBuilding();
                        selected_building = map.building_manager.buildings.Count - 1;
                        MainForm.mapedittool.InspectorSelect(map.building_manager.buildings[selected_building]);

                        drag_enabled = true;
                        //}
                    }
                }
            }
            else
            {
                if (button == MouseButtons.Left)
                {
                    // if dragging unit, just move selected unit, dont create a new one
                    if (drag_enabled)
                    {
                        //if (map.heightmap.CanMoveToPosition(fixed_pos))
                        map.MoveBuilding(selected_building, pos);
                    }
                    else
                    {
                        // find selected unit id
                        int building_map_index = map.building_manager.buildings.IndexOf(building);
                        if (building_map_index == -1)
                            return;

                        selected_building = building_map_index;
                        MainForm.mapedittool.InspectorSelect(map.building_manager.buildings[selected_building]);
                        
                        drag_enabled = true;
                    }
                }
                // delete unit
                else if (button == MouseButtons.Right)
                {
                    int building_map_index = map.building_manager.buildings.IndexOf(building);
                    if (building_map_index == -1)
                        return;

                    if (building_map_index == selected_building)
                        MainForm.mapedittool.InspectorSelect(null);

                    map.DeleteBuilding(building_map_index);
                    ((map_controls.MapBuildingInspector)MainForm.mapedittool.selected_inspector).RemoveBuilding(building_map_index);
                }
            }
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                drag_enabled = false;
                if(selected_building != -1)
                    MainForm.mapedittool.InspectorSelect(map.building_manager.buildings[selected_building]);
            }
        }
    }
}
