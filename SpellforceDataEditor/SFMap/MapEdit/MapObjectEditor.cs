using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapObjectEditor: MapEditor
    {
        bool first_click = false;
        public int selected_object { get; private set; } = -1;    // unit index
        public int placement_object { get; set; } = 0;

        public override void OnMousePress(SFCoord pos, MouseButtons button, ref special_forms.SpecialKeysPressed specials)
        {
            if (map == null)
                return;

            SFMapObject obj = map.FindObjectApprox(pos);

            // if no unit under the cursor and left mouse clicked, create new unit
            if (obj == null)
            {
                if (button == MouseButtons.Left)
                {
                    if (!map.heightmap.CanMoveToPosition(pos))
                        return;
                    // if dragging unit, just move selected unit, dont create a new one
                    if ((specials.Shift)&&(selected_object != -1))
                        map.MoveObject(selected_object, pos);
                    else if(!first_click)
                    {
                        ushort new_object_id = (ushort)placement_object;
                        if (map.gamedata[33].GetElementIndex(new_object_id) == -1)
                            return;
                        // create new unit and drag it until mouse released

                        map.AddObject(new_object_id, pos, 0, 0, 0);
                        ((map_controls.MapObjectInspector)MainForm.mapedittool.selected_inspector).LoadNextObject();
                        selected_object = map.object_manager.objects.Count - 1;
                        MainForm.mapedittool.InspectorSelect(map.object_manager.objects[selected_object]);

                        first_click = true;
                    }
                }
                else if(button == MouseButtons.Right)
                {
                    selected_object = -1;
                    MainForm.mapedittool.InspectorSelect(null);
                }
            }
            else
            {
                int object_map_index = map.object_manager.objects.IndexOf(obj);
                if (object_map_index == -1)
                    return;

                if (button == MouseButtons.Left)
                {
                    // if dragging unit, just move selected unit, dont create a new one
                    if ((specials.Shift) && (selected_object != -1))
                    {
                        if (map.heightmap.CanMoveToPosition(pos))
                            map.MoveObject(selected_object, pos);
                    }
                    else
                    {
                        selected_object = object_map_index;
                        MainForm.mapedittool.InspectorSelect(map.object_manager.objects[selected_object]);
                    }
                }
                // delete unit
                else if (button == MouseButtons.Right)
                {
                    if (object_map_index == selected_object)
                        MainForm.mapedittool.InspectorSelect(null);

                    map.DeleteObject(object_map_index);
                    ((map_controls.MapObjectInspector)MainForm.mapedittool.selected_inspector).RemoveObject(object_map_index);
                }
            }
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                first_click = false;
                if(selected_object != -1)
                    MainForm.mapedittool.InspectorSelect(map.object_manager.objects[selected_object]);
            }
        }
    }
}
