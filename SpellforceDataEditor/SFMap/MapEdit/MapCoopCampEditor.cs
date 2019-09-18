using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapCoopCampEditor: MapEditor
    {
        bool drag_enabled = false;
        public int selected_spawn { get; private set; } = -1;    // spawn index

        public override void OnMousePress(SFCoord pos, MouseButtons b)
        {
            SFMapObject obj = null;
            SFMapCoopAISpawn spawn = null;

            foreach (SFMapCoopAISpawn _spawn in map.metadata.coop_spawns)
            {
                if (SFCoord.Distance(_spawn.spawn_obj.grid_position, pos) <= 8)   // since spawn size is 16
                {
                    obj = _spawn.spawn_obj;
                    spawn = _spawn;
                    break;
                }
            }

            // if no unit under the cursor and left mouse clicked, create new unit
            if (obj == null)
            {
                if (b == MouseButtons.Left)
                {
                    if (drag_enabled == true)
                        map.MoveObject(map.object_manager.objects.IndexOf(map.metadata.coop_spawns[selected_spawn].spawn_obj), pos);
                    else
                    {
                        // check if can place
                        //if (map.heightmap.CanMoveToPosition(fixed_pos))
                        //{
                        ushort new_object_id = 2541;
                        if (map.gamedata[33].GetElementIndex(new_object_id) == -1)
                            return;
                        // create new spawn and drag it until mouse released
                        map.AddObject(new_object_id, pos, 0, 0, 0);
                        map.metadata.coop_spawns.Add(
                            new SFMapCoopAISpawn(map.object_manager.objects[map.object_manager.objects.Count - 1], 0, 0));
                        
                        // add mesh to the object
                        SF3D.SceneSynchro.SceneNode obj_node =
                            map.heightmap.GetChunkNode(pos)
                            .FindNode<SF3D.SceneSynchro.SceneNode>(
                                map.object_manager.objects[map.object_manager.objects.Count - 1].GetObjectName());

                        string m = "editor_dummy_spawnpoint";
                        SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeSimple(obj_node, m, obj_node.Name + "_SPAWNCIRCLE");

                        ((map_controls.MapCoopCampInspector)MainForm.mapedittool.selected_inspector).LoadNextCoopCamp();
                        selected_spawn = map.metadata.coop_spawns.Count - 1;
                        MainForm.mapedittool.InspectorSelect(map.metadata.coop_spawns[selected_spawn]);

                        drag_enabled = true;
                        //}
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
                            map.MoveObject(map.object_manager.objects.IndexOf(obj), pos);
                    }
                    else
                    {
                        // find selected coop camp index
                        int spawn_map_index = map.metadata.coop_spawns.IndexOf(spawn);
                        if (spawn_map_index == -1)
                            return;

                        selected_spawn = spawn_map_index;
                        MainForm.mapedittool.InspectorSelect(spawn);

                        drag_enabled = true;
                    }
                }
                // delete unit
                else if (b == MouseButtons.Right)
                {
                    int object_map_index = map.object_manager.objects.IndexOf(obj);
                    if (object_map_index == -1)
                        return;

                    if (map.metadata.coop_spawns.IndexOf(spawn) == selected_spawn)
                        MainForm.mapedittool.InspectorSelect(null);

                    map.DeleteObject(object_map_index);
                    ((map_controls.MapCoopCampInspector)MainForm.mapedittool.selected_inspector)
                        .RemoveCoopCamp(map.metadata.coop_spawns.IndexOf(spawn));
                    map.metadata.coop_spawns.Remove(spawn);
                }
            }
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                drag_enabled = false;
                if (selected_spawn != -1)
                    MainForm.mapedittool.InspectorSelect(map.metadata.coop_spawns[selected_spawn]);
            }
        }
    }
}
