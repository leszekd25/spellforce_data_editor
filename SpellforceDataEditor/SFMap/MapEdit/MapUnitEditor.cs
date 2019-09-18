using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapUnitEditor: MapEditor
    {
        bool drag_enabled = false;
        public int selected_unit { get; private set; } = -1;    // unit index
        public int placement_unit { get; set; } = 0;

        public override void OnMousePress(SFCoord pos, MouseButtons b)
        {
            if (map == null)
                return;

            // get unit under position
            SFMapUnit unit = map.FindUnit(pos);

            // if no unit under the cursor and left mouse clicked, create new unit
            if (unit == null)
            {
                if (b == MouseButtons.Left)
                {
                    // if dragging unit, just move selected unit, dont create a new one
                    if (drag_enabled)
                    {
                        if (map.heightmap.CanMoveToPosition(pos))
                            map.MoveUnit(selected_unit, pos);
                    }
                    else
                    {
                        // check if can place
                        if (map.heightmap.CanMoveToPosition(pos))
                        {
                            ushort new_unit_id = (ushort)placement_unit;
                            if (map.gamedata[17].GetElementIndex(new_unit_id) == -1)
                                return;
                            // create new unit and drag it until mouse released
                            map.AddUnit(new_unit_id, pos, 0, 0, 0, 0, 0);

                            ((map_controls.MapUnitInspector)MainForm.mapedittool.selected_inspector).LoadNextUnit();
                            selected_unit = map.unit_manager.units.Count - 1;
                            MainForm.mapedittool.InspectorSelect(map.unit_manager.units[selected_unit]);

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
                            map.MoveUnit(selected_unit, pos);
                    }
                    else
                    {
                        // find selected unit id
                        int unit_map_index = map.unit_manager.units.IndexOf(unit);
                        if (unit_map_index == -1)
                            return;

                        selected_unit = unit_map_index;
                        MainForm.mapedittool.InspectorSelect(unit);

                        drag_enabled = true;
                    }
                }
                // delete unit
                else if (b == MouseButtons.Right)
                {
                    int unit_map_index = map.unit_manager.units.IndexOf(unit);
                    if (unit_map_index == -1)
                        return;

                    if (unit_map_index == selected_unit)
                        MainForm.mapedittool.InspectorSelect(null);

                    map.DeleteUnit(unit_map_index);
                    ((map_controls.MapUnitInspector)MainForm.mapedittool.selected_inspector).RemoveUnit(unit_map_index);
                }
            }
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                drag_enabled = false;
                if(selected_unit != -1)
                    MainForm.mapedittool.InspectorSelect(map.unit_manager.units[selected_unit]);
            }
        }
    }
}
