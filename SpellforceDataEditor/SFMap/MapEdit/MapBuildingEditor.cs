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
        bool first_click = false;
        public int selected_building { get; set; } = -1;    // unit index
        public int placement_building { get; set; } = 0;

        public override void OnMousePress(SFCoord pos, MouseButtons button, ref special_forms.SpecialKeysPressed specials)
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
                    if (!map.heightmap.CanMoveToPosition(pos))
                        return;
                    // if dragging unit, just move selected unit, dont create a new one
                    if ((specials.Shift)&&(selected_building != -1))
                        map.MoveBuilding(selected_building, pos);
                    else if(!first_click)
                    {
                        ushort new_building_id = (ushort)placement_building;
                        if (map.gamedata[23].GetElementIndex(new_building_id) == -1)
                            return;
                        // create new building and drag it until mouse released
                        map.AddBuilding(new_building_id, pos, 0, 0, 1, -1);

                        ((map_controls.MapBuildingInspector)MainForm.mapedittool.selected_inspector).LoadNextBuilding();
                        selected_building = map.building_manager.buildings.Count - 1;
                        MainForm.mapedittool.InspectorSelect(map.building_manager.buildings[selected_building]);

                        first_click = true;
                    }
                }
                else if(button == MouseButtons.Right)
                {
                    selected_building = -1;
                    MainForm.mapedittool.InspectorSelect(null);
                }
            }
            else
            {
                // find selected unit id
                int building_map_index = map.building_manager.buildings.IndexOf(building);
                if (building_map_index == -1)
                    return;

                if (button == MouseButtons.Left)
                {
                    // if dragging unit, just move selected unit, dont create a new one
                    if ((specials.Shift)&&(selected_building != -1))
                    {
                        if (map.heightmap.CanMoveToPosition(pos))
                            map.MoveBuilding(selected_building, pos);
                    }
                    else
                    {
                        selected_building = building_map_index;
                        MainForm.mapedittool.InspectorSelect(map.building_manager.buildings[selected_building]);
                    }
                }
                // delete unit
                else if (button == MouseButtons.Right)
                {
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
                first_click = false;
                if(selected_building != -1)
                    MainForm.mapedittool.InspectorSelect(map.building_manager.buildings[selected_building]);
            }
        }
    }
}
