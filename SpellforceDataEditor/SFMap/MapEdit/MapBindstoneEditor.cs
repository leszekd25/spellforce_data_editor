using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapBindstoneEditor: MapEditor
    {
        bool drag_enabled = false;
        public int selected_intobj { get; private set; } = -1;       // interactive object index
        public int selected_bindstone { get; private set; } = -1;    // spawn index

        public override void OnMousePress(SFCoord pos, MouseButtons b)
        {
            // 1. find clicked bindstone if it exists
            int intobj_index = -1;
            int bindstone_index = -1;
            SFMapInteractiveObject int_obj = null;

            foreach (SFMapInteractiveObject io in map.int_object_manager.int_objects)
            {
                intobj_index += 1;
                if (io.game_id == 769)
                {
                    bindstone_index += 1;
                    if (SFCoord.Distance(io.grid_position, pos) <= 2)
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
                        map.AddInteractiveObject(769, pos, 0, 1);
                        map.metadata.spawns.Add(new SFMapSpawn());
                        map.metadata.spawns[map.metadata.spawns.Count - 1].pos = pos;

                        ((map_controls.MapBindstoneInspector)MainForm.mapedittool.selected_inspector).LoadNextBindstone();
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
                        selected_bindstone = bindstone_index;
                        selected_intobj = intobj_index;

                        MainForm.mapedittool.InspectorSelect(
                            map.int_object_manager.int_objects[selected_intobj]);

                        drag_enabled = true;
                    }
                }
                else if (b == MouseButtons.Right)
                {
                    bool can_remove = true;
                    int player = map.metadata.FindPlayerBySpawnPos(int_obj.grid_position);
                    if (player != -1)
                    {
                        if (map.metadata.IsPlayerActive(player))
                            can_remove = false;
                    }

                    if(can_remove)
                    {
                        if (selected_intobj == intobj_index)
                            MainForm.mapedittool.InspectorSelect(null);

                        map.DeleteInteractiveObject(intobj_index);
                        ((map_controls.MapBindstoneInspector)MainForm.mapedittool.selected_inspector).RemoveBindstone(bindstone_index);
                        for (int i = 0; i < map.metadata.spawns.Count; i++)
                            if (int_obj.grid_position == map.metadata.spawns[i].pos)
                            {
                                map.metadata.spawns.RemoveAt(i);
                                break;
                            }
                    }
                }
            }
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                drag_enabled = false;
                if (selected_bindstone != -1)
                    MainForm.mapedittool.InspectorSelect(map.int_object_manager.int_objects[selected_intobj]);
            }
        }
    }
}
