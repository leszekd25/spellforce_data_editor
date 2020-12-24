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
        bool first_click = false;
        public int selected_spawn { get; private set; } = -1;    // spawn index

        // undo/redo
        map_operators.MapOperatorEntityChangeProperty op_change_pos = null;

        public override void OnMousePress(SFCoord pos, MouseButtons b, ref special_forms.SpecialKeysPressed specials)
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
                    if (!map.heightmap.CanMoveToPosition(pos))
                        return;

                    // undo/redo
                    SFCoord previous_pos = new SFCoord(0, 0);

                    if ((specials.Shift) && (selected_spawn != Utility.NO_INDEX))
                    {
                        // undo/redo
                        previous_pos = map.metadata.coop_spawns[selected_spawn].spawn_obj.grid_position;

                        map.MoveObject(map.object_manager.objects.IndexOf(map.metadata.coop_spawns[selected_spawn].spawn_obj), pos);
                    }
                    else if (!first_click)
                    {
                        ushort new_object_id = 2541;
                        if (map.gamedata[33].GetElementIndex(new_object_id) == Utility.NO_INDEX)
                            return;
                        // create new spawn and drag it until mouse released
                        map.AddObject(new_object_id, pos, 0, 0, 0);
                        // undo/redo
                        previous_pos = pos;

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

                        // undo/redo
                        MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityAddOrRemove()
                        { type = map_operators.MapOperatorEntityType.COOPCAMP, id = 0, position = pos, is_adding = true });
                    }


                    // undo/redo
                    if ((selected_spawn != Utility.NO_INDEX) && (!first_click))
                    {
                        op_change_pos = new map_operators.MapOperatorEntityChangeProperty()
                        {
                            type = map_operators.MapOperatorEntityType.COOPCAMP,
                            index = selected_spawn,
                            property = map_operators.MapOperatorEntityProperty.POSITION,
                            PreChangeProperty = previous_pos
                        };
                    }

                    first_click = true;
                }
                else if(b == MouseButtons.Right)
                {
                    selected_spawn = -1;
                    MainForm.mapedittool.InspectorSelect(null);
                }
            }
            else
            {
                if (b == MouseButtons.Left)
                {
                    // find selected coop camp index
                    int spawn_map_index = map.metadata.coop_spawns.IndexOf(spawn);
                    if (spawn_map_index == -1)
                        return;

                    // if dragging unit, just move selected unit, dont create a new one
                    if ((specials.Shift) && (selected_spawn != -1))
                    {
                        if (map.heightmap.CanMoveToPosition(pos))
                            map.MoveObject(map.object_manager.objects.IndexOf(obj), pos);
                    }
                    else
                    {
                        selected_spawn = spawn_map_index;
                        MainForm.mapedittool.InspectorSelect(spawn);
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

                    // undo/redo
                    MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityAddOrRemove()
                    {
                        type = map_operators.MapOperatorEntityType.COOPCAMP,
                        id = spawn.spawn_id,
                        position = obj.grid_position,
                        is_adding = false
                    });

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
                first_click = false;
                if (selected_spawn != -1)
                {
                    // undo/redo
                    if (op_change_pos != null)
                    {
                        op_change_pos.PostChangeProperty = map.metadata.coop_spawns[selected_spawn].spawn_obj.grid_position;
                        if (!op_change_pos.PreChangeProperty.Equals(op_change_pos.PostChangeProperty))
                        {
                            op_change_pos.Finish(map);
                            MainForm.mapedittool.op_queue.Push(op_change_pos);
                        }
                    }
                    op_change_pos = null;
                    MainForm.mapedittool.InspectorSelect(map.metadata.coop_spawns[selected_spawn]);
                }
            }
        }
    }
}
