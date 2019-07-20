using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public enum MISC_EDITMODE { COOP_CAMPS = 0, BINDSTONES, PORTALS, MONUMENTS }

    public partial class MapInspectorMiscellaneuosObjectsControl : SpellforceDataEditor.SFMap.map_controls.MapInspectorBaseControl
    {
        bool drag_enabled = false;
        bool no_camera_jump = false;     // forgive me lord for i have sinned (todo: remove this and add proper Select_ methods)
        MISC_EDITMODE mode = MISC_EDITMODE.COOP_CAMPS;

        public MapInspectorMiscellaneuosObjectsControl()
        {
            InitializeComponent();
        }

        private void MapInspectorMiscellaneuosObjectsControl_VisibleChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (Visible)
            {
                SFLua.SFLuaEnvironment.coop_spawns.Load();
                ReloadCoopCampList();
                ReloadBindstoneList();
                ReloadPortalList();
                ReloadMonumentList();
            }

            MainForm.mapedittool.update_render = true;
        }

        private void ReloadCoopCampList()
        {
            ListCoopCamps.Items.Clear();
            if (map.metadata.coop_spawns == null)
                return;

            foreach (SFMapCoopAISpawn spawn in map.metadata.coop_spawns)
                ListCoopCamps.Items.Add(GetCoopSpawnString(spawn));
        }

        private string GetCoopSpawnString(SFMapCoopAISpawn spawn)
        {
            string ret = "";
            if (SFLua.SFLuaEnvironment.coop_spawns.coop_spawn_types != null)
                if (SFLua.SFLuaEnvironment.coop_spawns.coop_spawn_types.ContainsKey(spawn.spawn_id))
                    ret += SFLua.SFLuaEnvironment.coop_spawns.coop_spawn_types[spawn.spawn_id].name + " ";
            ret += spawn.spawn_obj.grid_position.ToString();
            return ret;
        }

        private void ReloadBindstoneList()
        {
            ListBindstones.Items.Clear();
            foreach (SFMapInteractiveObject io in map.int_object_manager.int_objects)
                if (io.game_id == 769)
                    ListBindstones.Items.Add(GetBindstoneString(io));
        }

        private string GetBindstoneString(SFMapInteractiveObject io)
        {
            return " Bindstone at " + io.grid_position.ToString();
        }

        private void ReloadPortalList()
        {
            ListPortals.Items.Clear();
            foreach (SFMapPortal p in map.portal_manager.portals)
                ListPortals.Items.Add(GetPortalString(p));
        }

        private string GetPortalString(SFMapPortal p)
        {
            string ret = "";
            if (SFCFF.SFCategoryManager.ready)
            {
                int portal_id = p.game_id;
                int portal_index = SFCFF.SFCategoryManager.gamedata.categories[38].get_element_index(portal_id);
                if (portal_index != -1)
                {
                    SFCFF.SFCategoryElement portal_data = SFCFF.SFCategoryManager.gamedata.categories[38].get_element(portal_index);
                    ushort text_id = (ushort)portal_data.get_single_variant(5).value;
                    SFCFF.SFCategoryElement text_data = SFCFF.SFCategoryManager.find_element_text(text_id, Settings.LanguageID);
                    if (text_data != null)
                        ret += Utility.CleanString(text_data.get_single_variant(4)) + " ";
                    else
                        ret += Utility.S_MISSING + " ";
                }
            }
            ret += p.grid_position.ToString();
            return ret;
        }

        private void ReloadMonumentList()
        {
            ListMonuments.Items.Clear();
            foreach (SFMapInteractiveObject io in map.int_object_manager.int_objects)
                if ((io.game_id >= 771) && (io.game_id <= 777))
                    ListMonuments.Items.Add(GetMonumentString(io));
        }

        private string GetMonumentString(SFMapInteractiveObject io)
        {
            string ret = "";
            if (SFCFF.SFCategoryManager.ready)
                ret += SFCFF.SFCategoryManager.get_object_name((ushort)io.game_id) + " ";
            ret += io.grid_position.ToString();
            return ret;
        }

        private void ButtonChangeSelectedCampType_Click(object sender, EventArgs e)
        {
            if (!Settings.AllowLua)
            {
                MessageBox.Show("Lua interpreter is disabled. Close editor, set 'AllowLua' in settings to 'YES' and run editor again to enable Lua interpreter.");
                return;
            }
            SFLua.SFLuaEnvironment.ShowRtsCoopSpawnGroupsForm();
            ReloadCoopCampList();
        }

        private void ListCoopCamps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListCoopCamps.SelectedIndex == -1)
                return;

            SelectedCampX.Text = map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_obj.grid_position.x.ToString();
            SelectedCampY.Text = map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_obj.grid_position.y.ToString();
            SelectedCampType.Text = map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_id.ToString();
            SelectedCampUnknown.Text = map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_certain.ToString();

            map.selection_helper.SelectObject(map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_obj);
            if(!no_camera_jump)
                MainForm.mapedittool.SetCameraViewPoint(map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_obj.grid_position);
            no_camera_jump = false;

            ComboEditMode.SelectedIndex = 0;
        }

        private void ListBindstones_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBindstones.SelectedIndex == -1)
                return;

            List<int> bindstone_indexes = new List<int>();
            for (int i = 0; i < map.int_object_manager.int_objects.Count; i++)
                if (map.int_object_manager.int_objects[i].game_id == 769)
                    bindstone_indexes.Add(i);

            SFMapInteractiveObject io = map.int_object_manager.int_objects[bindstone_indexes[ListBindstones.SelectedIndex]];

            SelectedBindstoneX.Text = io.grid_position.x.ToString();
            SelectedBindstoneY.Text = io.grid_position.y.ToString();
            SelectedBindstoneAngle.Text = io.angle.ToString();

            map.selection_helper.SelectInteractiveObject(io);
            if(!no_camera_jump)
                MainForm.mapedittool.SetCameraViewPoint(io.grid_position);
            no_camera_jump = false;

            ComboEditMode.SelectedIndex = 1;
        }

        private void ListPortals_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListPortals.SelectedIndex == -1)
                return;

            SelectedPortalX.Text = map.portal_manager.portals[ListPortals.SelectedIndex].grid_position.x.ToString();
            SelectedPortalY.Text = map.portal_manager.portals[ListPortals.SelectedIndex].grid_position.y.ToString();
            SelectedPortalAngle.Text = map.portal_manager.portals[ListPortals.SelectedIndex].angle.ToString();
            SelectedPortalID.Text = map.portal_manager.portals[ListPortals.SelectedIndex].game_id.ToString();

            map.selection_helper.SelectPortal(map.portal_manager.portals[ListPortals.SelectedIndex]);
            if (!no_camera_jump)
                MainForm.mapedittool.SetCameraViewPoint(map.portal_manager.portals[ListPortals.SelectedIndex].grid_position);
            no_camera_jump = false;

            ComboEditMode.SelectedIndex = 2;
        }

        private void ListMonuments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListMonuments.SelectedIndex == -1)
                return;

            List<int> monument_indexes = new List<int>();
            for (int i = 0; i < map.int_object_manager.int_objects.Count; i++)
                if ((map.int_object_manager.int_objects[i].game_id >= 771) && (map.int_object_manager.int_objects[i].game_id <= 777))
                    monument_indexes.Add(i);

            SFMapInteractiveObject io = map.int_object_manager.int_objects[monument_indexes[ListMonuments.SelectedIndex]];

            SelectedMonumentX.Text = io.grid_position.x.ToString();
            SelectedMonumentY.Text = io.grid_position.y.ToString();
            SelectedMonumentAngle.Text = io.angle.ToString();
            SelectedMonumentType.SelectedIndex = io.game_id - 771;

            map.selection_helper.SelectInteractiveObject(io);
            if (!no_camera_jump)
                MainForm.mapedittool.SetCameraViewPoint(io.grid_position);
            no_camera_jump = false;

            ComboEditMode.SelectedIndex = 3;
        }

        private void SelectedCampType_Validated(object sender, EventArgs e)
        {
            if (ListCoopCamps.SelectedIndex == -1)
                return;

            map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_id = Utility.TryParseUInt8(SelectedCampType.Text);
        }

        private void SelectedCampUnknown_Validated(object sender, EventArgs e)
        {
            if (ListCoopCamps.SelectedIndex == -1)
                return;

            map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_certain = Utility.TryParseUInt8(SelectedCampUnknown.Text);
        }

        private void SelectedPortalID_Validated(object sender, EventArgs e)
        {
            if (ListPortals.SelectedIndex == -1)
                return;

            map.portal_manager.portals[ListPortals.SelectedIndex].game_id = Utility.TryParseUInt16(SelectedPortalID.Text);
        }

        private void SelectedMonumentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListMonuments.SelectedIndex == -1)
                return;

            map.ReplaceMonument(ListMonuments.SelectedIndex, SelectedMonumentType.SelectedIndex);
            MainForm.mapedittool.update_render = true;
        }

        public override void OnMouseDown(SFCoord clicked_pos, MouseButtons button)
        {
            if (map == null)
                return;

            SFCoord fixed_pos = new SFCoord(clicked_pos.x, map.height - clicked_pos.y - 1);
            SFMapObject obj = null;
            SFMapInteractiveObject int_obj = null;
            SFMapPortal portal = null;
            SFMapCoopAISpawn spawn = null;

            int io_index = -1;
            int iter = -1;

            // todo: move this to respective methods
            switch (mode)
            {
                case MISC_EDITMODE.COOP_CAMPS:
                    foreach (SFMapCoopAISpawn _spawn in map.metadata.coop_spawns)
                    {
                        if (SFCoord.Distance(_spawn.spawn_obj.grid_position, fixed_pos) <= 8)   // since spawn size is 16
                        {
                            obj = _spawn.spawn_obj;
                            spawn = _spawn;
                            break;
                        }
                    }

                    // if no unit under the cursor and left mouse clicked, create new unit
                    if (obj == null)
                    {
                        if (button == MouseButtons.Left)
                        {
                            if (drag_enabled == true)
                            {
                                map.MoveObject(map.object_manager.objects.IndexOf(map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_obj), fixed_pos);
                            }
                            else
                            {
                                // check if can place
                                //if (map.heightmap.CanMoveToPosition(fixed_pos))
                                //{
                                ushort new_object_id = 2541;
                                if (map.gamedata.categories[33].get_element_index(new_object_id) == -1)
                                    return;
                                // create new spawn and drag it until mouse released
                                map.AddObject(new_object_id, fixed_pos, 0, 0, 0);
                                map.metadata.coop_spawns.Add(new SFMapCoopAISpawn(map.object_manager.objects[map.object_manager.objects.Count - 1], 0, 0));


                                ListCoopCamps.Items.Add(GetCoopSpawnString(map.metadata.coop_spawns[map.metadata.coop_spawns.Count - 1]));
                                no_camera_jump = true;
                                ListCoopCamps.SelectedIndex = map.metadata.coop_spawns.Count - 1;
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
                                map.MoveObject(map.object_manager.objects.IndexOf(obj), fixed_pos);
                            }
                            else
                            {
                                no_camera_jump = true;
                                ListCoopCamps.SelectedIndex = map.metadata.coop_spawns.IndexOf(spawn);
                                drag_enabled = true;
                            }
                        }
                        // delete unit
                        else if (button == MouseButtons.Right)
                        {
                            int object_map_index = map.object_manager.objects.IndexOf(obj);
                            if (object_map_index == -1)
                                return;

                            if (map.metadata.coop_spawns.IndexOf(spawn) == ListCoopCamps.SelectedIndex)
                                ListCoopCamps.SelectedIndex = -1;

                            map.DeleteObject(object_map_index);
                            ListCoopCamps.Items.RemoveAt(map.metadata.coop_spawns.IndexOf(spawn));
                        }
                    }
                    break;

                case MISC_EDITMODE.BINDSTONES:
                    foreach (SFMapInteractiveObject io in map.int_object_manager.int_objects)
                    {
                        if (io.game_id != 769)
                            continue;

                        iter += 1;
                        if (SFCoord.Distance(io.grid_position, fixed_pos) <= 2)   // bindstone size selection...
                        {
                            int_obj = io;
                            io_index = iter;
                            break;
                        }
                    }

                    // if no unit under the cursor and left mouse clicked, create new unit
                    if (int_obj == null)
                    {
                        if (button == MouseButtons.Left)
                        {
                            if (drag_enabled == true)
                            {
                                iter = -1;
                                io_index = ListBindstones.SelectedIndex;
                                //if (map.heightmap.CanMoveToPosition(fixed_pos))
                                for(int i = 0; i < map.int_object_manager.int_objects.Count; i++)
                                {
                                    if (map.int_object_manager.int_objects[i].game_id == 769)
                                    {
                                        iter += 1;
                                        if(iter == io_index)
                                        map.MoveInteractiveObject(i, fixed_pos);
                                    }
                                }
                            }
                            else
                            {
                                // check if can place
                                //if (map.heightmap.CanMoveToPosition(fixed_pos))
                                //{
                                ushort new_object_id = 769;
                                // create new spawn and drag it until mouse released
                                map.AddInteractiveObject(new_object_id, fixed_pos, 0, 1);
                                map.metadata.spawns.Add(new SFMapSpawn());
                                map.metadata.spawns[map.metadata.spawns.Count - 1].pos = fixed_pos;
                                ListBindstones.Items.Add(GetBindstoneString(map.int_object_manager.int_objects[map.int_object_manager.int_objects.Count - 1]));
                                no_camera_jump = true;
                                ListBindstones.SelectedIndex = ListBindstones.Items.Count - 1;
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
                                map.MoveInteractiveObject(map.int_object_manager.int_objects.IndexOf(int_obj), fixed_pos);
                            }
                            else
                            {
                                no_camera_jump = true;
                                ListBindstones.SelectedIndex = io_index;
                                drag_enabled = true;
                            }
                        }
                        // delete unit
                        else if (button == MouseButtons.Right)
                        {
                            bool can_remove = true;
                            int player = map.metadata.FindPlayerBySpawnPos(int_obj.grid_position);
                            if (player != -1)
                            {
                                if (map.metadata.IsPlayerActive(player))
                                    can_remove = false;
                            }

                            if (can_remove)
                            {
                                if (ListBindstones.SelectedIndex == io_index)
                                    ListBindstones.SelectedIndex = -1;

                                map.DeleteInteractiveObject(map.int_object_manager.int_objects.IndexOf(int_obj));
                                for (int i = 0; i < map.metadata.spawns.Count; i++)
                                    if (int_obj.grid_position == map.metadata.spawns[i].pos)
                                    {
                                        map.metadata.spawns.RemoveAt(i);
                                        break;
                                    }
                                ListBindstones.Items.RemoveAt(io_index);
                            }
                        }
                    }
                    break;
                case MISC_EDITMODE.PORTALS:
                    foreach (SFMapPortal _portal in map.portal_manager.portals)
                    {
                        if (SFCoord.Distance(_portal.grid_position, fixed_pos) <= 3)   // since spawn size is 16
                        {
                            portal = _portal;
                            break;
                        }
                    }

                    // if no unit under the cursor and left mouse clicked, create new unit
                    if (portal == null)
                    {
                        if (button == MouseButtons.Left)
                        {
                            if (drag_enabled == true)
                            {
                                map.MovePortal(ListPortals.SelectedIndex, fixed_pos);
                            }
                            else
                            {
                                // check if can place
                                //if (map.heightmap.CanMoveToPosition(fixed_pos))
                                //{
                                // create new spawn and drag it until mouse released
                                map.AddPortal(0, fixed_pos, 0);

                                ListPortals.Items.Add(GetPortalString(map.portal_manager.portals[map.portal_manager.portals.Count - 1]));
                                no_camera_jump = true;
                                ListPortals.SelectedIndex = map.portal_manager.portals.Count - 1;
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
                                map.MovePortal(ListPortals.SelectedIndex, fixed_pos);
                            }
                            else
                            {
                                no_camera_jump = true;
                                ListPortals.SelectedIndex = map.portal_manager.portals.IndexOf(portal);
                                drag_enabled = true;
                            }
                        }
                        // delete unit
                        else if (button == MouseButtons.Right)
                        {
                            int portal_map_index = map.portal_manager.portals.IndexOf(portal);
                            if (portal_map_index == -1)
                                return;

                            if (portal_map_index == ListPortals.SelectedIndex)
                                ListPortals.SelectedIndex = -1;

                            map.DeletePortal(portal_map_index);
                            ListPortals.Items.RemoveAt(portal_map_index);
                        }
                    }
                    break;
                case MISC_EDITMODE.MONUMENTS:
                    foreach (SFMapInteractiveObject io in map.int_object_manager.int_objects)
                    {
                        if ((io.game_id < 771)||(io.game_id > 777))
                            continue;

                        iter += 1;
                        if (SFCoord.Distance(io.grid_position, fixed_pos) <= 5)   // monument size selection...
                        {
                            int_obj = io;
                            io_index = iter;
                            break;
                        }
                    }

                    // if no unit under the cursor and left mouse clicked, create new unit
                    if (int_obj == null)
                    {
                        if (button == MouseButtons.Left)
                        {
                            if (drag_enabled == true)
                            {
                                iter = -1;
                                io_index = ListMonuments.SelectedIndex;
                                //if (map.heightmap.CanMoveToPosition(fixed_pos))
                                for (int i = 0; i < map.int_object_manager.int_objects.Count; i++)
                                {
                                    if ((map.int_object_manager.int_objects[i].game_id >= 771) && (map.int_object_manager.int_objects[i].game_id <= 777))
                                    {
                                        iter += 1;
                                        if (iter == io_index)
                                            map.MoveInteractiveObject(i, fixed_pos);
                                    }
                                }
                            }
                            else
                            {
                                // check if can place
                                //if (map.heightmap.CanMoveToPosition(fixed_pos))
                                //{
                                ushort new_object_id = (ushort)(771 + SelectedMonumentType.SelectedIndex);
                                if (new_object_id == 770)
                                    new_object_id = 777;
                                // create new spawn and drag it until mouse released
                                byte unk_byte = 1;
                                if (new_object_id == 777)
                                    unk_byte = 5;
                                map.AddInteractiveObject(new_object_id, fixed_pos, 0, unk_byte);
                                ListMonuments.Items.Add(GetMonumentString(map.int_object_manager.int_objects[map.int_object_manager.int_objects.Count - 1]));
                                no_camera_jump = true;
                                ListMonuments.SelectedIndex = ListBindstones.Items.Count - 1;
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

                                map.MoveInteractiveObject(map.int_object_manager.int_objects.IndexOf(int_obj), fixed_pos);
                            }
                            else
                            {
                                no_camera_jump = true;
                                ListMonuments.SelectedIndex = io_index;
                                drag_enabled = true;
                            }
                        }
                        // delete unit
                        else if (button == MouseButtons.Right)
                        {
                            if (ListMonuments.SelectedIndex == io_index)
                                ListMonuments.SelectedIndex = -1;

                            map.DeleteInteractiveObject(map.int_object_manager.int_objects.IndexOf(int_obj));
                            ListMonuments.Items.RemoveAt(io_index);
                        }
                    }
                    break;
            }
        }

        public override void OnMouseUp()
        {
            drag_enabled = false;
        }

        private void ComboEditMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboEditMode.SelectedIndex == -1)
                return;

            mode = (MISC_EDITMODE)ComboEditMode.SelectedIndex;
        }

        private void SelectedBindstoneAngleTrackBar_Scroll(object sender, EventArgs e)
        {
            if (ListBindstones.SelectedIndex == -1)
                return;

            List<int> bindstone_indexes = new List<int>();
            for (int i = 0; i < map.int_object_manager.int_objects.Count; i++)
                if (map.int_object_manager.int_objects[i].game_id == 769)
                    bindstone_indexes.Add(i);

            SFMapInteractiveObject io = map.int_object_manager.int_objects[bindstone_indexes[ListBindstones.SelectedIndex]];
            SelectedBindstoneAngle.Text = SelectedBindstoneAngleTrackBar.Value.ToString();
            io.angle = SelectedBindstoneAngleTrackBar.Value;
            map.RotateInteractiveObject(bindstone_indexes[ListBindstones.SelectedIndex], io.angle);

            MainForm.mapedittool.update_render = true;
        }

        private void SelectedBindstoneAngle_Validated(object sender, EventArgs e)
        {
            SelectedBindstoneAngleTrackBar.Value = (int)(Math.Max((ushort)0, Math.Min((ushort)359, Utility.TryParseUInt16(SelectedBindstoneAngle.Text))));
        }

        private void SelectedPortalAngleTrackbar_Scroll(object sender, EventArgs e)
        {
            if (ListPortals.SelectedIndex == -1)
                return;

            SFMapPortal p = map.portal_manager.portals[ListPortals.SelectedIndex];
            SelectedPortalAngle.Text = SelectedPortalAngleTrackbar.Value.ToString();
            p.angle = SelectedPortalAngleTrackbar.Value;
            map.RotatePortal(ListPortals.SelectedIndex, p.angle);

            MainForm.mapedittool.update_render = true;
        }

        private void SelectedPortalAngle_Validated(object sender, EventArgs e)
        {
            SelectedPortalAngleTrackbar.Value = (int)(Math.Max((ushort)0, Math.Min((ushort)359, Utility.TryParseUInt16(SelectedPortalAngle.Text))));
        }

        private void SelectedMonumentAngleTrackbar_Scroll(object sender, EventArgs e)
        {
            if (ListMonuments.SelectedIndex == -1)
                return;

            List<int> monument_indexes = new List<int>();
            for (int i = 0; i < map.int_object_manager.int_objects.Count; i++)
                if ((map.int_object_manager.int_objects[i].game_id >= 771) && (map.int_object_manager.int_objects[i].game_id <= 777))
                    monument_indexes.Add(i);

            SFMapInteractiveObject io = map.int_object_manager.int_objects[monument_indexes[ListMonuments.SelectedIndex]];
            SelectedMonumentAngle.Text = SelectedMonumentAngleTrackbar.Value.ToString();
            io.angle = SelectedMonumentAngleTrackbar.Value;
            map.RotateInteractiveObject(monument_indexes[ListMonuments.SelectedIndex], io.angle);

            MainForm.mapedittool.update_render = true;

        }
    }
}
