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
                    SFCFF.SFCategoryElement text_data = SFCFF.SFCategoryManager.find_element_text(text_id, 1);
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
                ret += SFCFF.SFCategoryManager.get_object_name((ushort)io.game_id)+" ";
            ret += io.grid_position.ToString();
            return ret;
        }

        // testing purpose for now

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
            MainForm.mapedittool.SetCameraViewPoint(map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_obj.grid_position);
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
            MainForm.mapedittool.SetCameraViewPoint(io.grid_position);
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
            MainForm.mapedittool.SetCameraViewPoint(map.portal_manager.portals[ListPortals.SelectedIndex].grid_position);
        }

        private void ListMonuments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListMonuments.SelectedIndex == -1)
                return;

            List<int> monument_indexes = new List<int>();
            for (int i = 0; i < map.int_object_manager.int_objects.Count; i++)
                if ((map.int_object_manager.int_objects[i].game_id >= 771)&& (map.int_object_manager.int_objects[i].game_id <= 777))
                    monument_indexes.Add(i);

            SFMapInteractiveObject io = map.int_object_manager.int_objects[monument_indexes[ListMonuments.SelectedIndex]];

            SelectedMonumentX.Text = io.grid_position.x.ToString();
            SelectedMonumentY.Text = io.grid_position.y.ToString();
            SelectedMonumentAngle.Text = io.angle.ToString();
            SelectedMonumentType.SelectedIndex = io.game_id - 771;

            map.selection_helper.SelectInteractiveObject(io);
            MainForm.mapedittool.SetCameraViewPoint(io.grid_position);
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
    }
}
