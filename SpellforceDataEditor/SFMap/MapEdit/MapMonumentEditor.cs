using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public enum MonumentType { HUMAN = 0, ELF, DWARF, ORC, TROLL, DARKELF, HERO }

    public class MapMonumentEditor: MapEditor
    {
        bool drag_enabled = false;
        public MonumentType selected_type = MonumentType.HERO;
        public int selected_intobj { get; private set; } = -1;       // interactive object index
        public int selected_monument { get; private set; } = -1;    // spawn index

        public override void OnMousePress(SFCoord pos, MouseButtons b)
        {
            // 1. find clicked bindstone if it exists
            int intobj_index = -1;
            int monument_index = -1;
            SFMapInteractiveObject int_obj = null;

            foreach (SFMapInteractiveObject io in map.int_object_manager.int_objects)
            {
                intobj_index += 1;
                if ((io.game_id >= 771)&&(io.game_id <= 777))
                {
                    monument_index += 1;
                    if (SFCoord.Distance(io.grid_position, pos) <= 5)
                    {
                        int_obj = io;
                        break;
                    }
                }
            }

            // 2. if not clicked, create new bindstone
            if(int_obj == null)
            {
                if(b == MouseButtons.Left)
                {
                    if(drag_enabled == true)
                    {
                        if(map.heightmap.CanMoveToPosition(pos))
                            map.MoveInteractiveObject(intobj_index, pos);
                    }
                    else
                    {
                        int new_object_id = 771 + (int)selected_type;

                        byte unk_byte = 1;
                        if (new_object_id == 777)
                            unk_byte = 5;

                        map.AddInteractiveObject(new_object_id, pos, 0, unk_byte);

                        ((map_controls.MapMonumentInspector)MainForm.mapedittool.selected_inspector).LoadNextMonument();
                        selected_intobj = intobj_index;
                        MainForm.mapedittool.InspectorSelect(
                            map.int_object_manager.int_objects[map.int_object_manager.int_objects.Count - 1]);

                        drag_enabled = true;
                    }
                }
            }
            else
            {
                if (b == MouseButtons.Left)
                {
                    if (drag_enabled == true)
                    {
                        if (map.heightmap.CanMoveToPosition(pos))
                            map.MoveInteractiveObject(intobj_index, pos);
                    }
                    else
                    {
                        selected_monument = monument_index;
                        selected_intobj = intobj_index;

                        MainForm.mapedittool.InspectorSelect(
                            map.int_object_manager.int_objects[selected_intobj]);

                        drag_enabled = true;
                    }
                }
                else if (b == MouseButtons.Right)
                {
                   if (selected_intobj == intobj_index)
                        MainForm.mapedittool.InspectorSelect(null);

                    map.DeleteInteractiveObject(intobj_index);
                    ((map_controls.MapMonumentInspector)MainForm.mapedittool.selected_inspector).RemoveMonument(monument_index);
                }
            }
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (b == MouseButtons.Left)                                                                                
            {
                drag_enabled = false;
                if (selected_monument != -1)
                    MainForm.mapedittool.InspectorSelect(map.int_object_manager.int_objects[selected_intobj]);
            }
        }
    }
}
