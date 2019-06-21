using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapInspectorBuildingControl : SpellforceDataEditor.SFMap.map_controls.MapInspectorBaseControl
    {
        bool drag_enabled = false;
        int selected_building = -1;

        public MapInspectorBuildingControl()
        {
            InitializeComponent();
        }

        private void MapInspectorBuildingControl_VisibleChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (Visible)
            {
                ReloadBuildingList();
            }

            MainForm.mapedittool.update_render = true;
        }

        private void ReloadBuildingList()
        {
            ListBuildings.Items.Clear();

            foreach (SFMapBuilding b in map.building_manager.buildings)
                ListBuildings.Items.Add(GetBuildingString(b));
        }

        private string GetBuildingString(SFMapBuilding b)
        {
            string ret = SFCFF.SFCategoryManager.get_building_name((ushort)b.game_id);
            ret += " " + b.grid_position.ToString();
            return ret;
        }

        private void ListBuildings_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectBuilding(ListBuildings.SelectedIndex, true);
        }

        private void BuildingToPlaceID_TextChanged(object sender, EventArgs e)
        {
            ushort building_id = Utility.TryParseUInt16(BuildingToPlaceID.Text);

            BuildingToPlaceName.Text = SFCFF.SFCategoryManager.get_building_name(building_id);
        }

        private void BuildingToPlaceID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (MainForm.data == null)
                    return;
                if (!SFCFF.SFCategoryManager.ready)
                    return;

                ushort building_id = Utility.TryParseUInt16(BuildingToPlaceID.Text);

                MainForm.data.Tracer_StepForward(23, building_id, false);
            }
        }

        public void SelectBuilding(int building_map_index, bool move_vision)
        {
            if (building_map_index < 0)
                building_map_index = -1;
            if (building_map_index >= map.building_manager.buildings.Count)
                building_map_index = -1;

            selected_building = building_map_index;

            if (building_map_index == -1)
            {
                SelectedBuildingID.Text = "";
                SelectedBuildingName.Text = "";
                SelectedBuildingNPCID.Text = "";
                SelectedBuildingX.Text = "";
                SelectedBuildingY.Text = "";
                SelectedBuildingAngle.Text = "";
                SelectedBuildingLevel.Text = "";
                SelectedBuildingRace.Text = "";
                map.selection_helper.CancelSelection();
            }
            else
            {

                SFMapBuilding building = map.building_manager.buildings[building_map_index];
                SelectedBuildingID.Text = building.game_id.ToString();
                SelectedBuildingName.Text = SFCFF.SFCategoryManager.get_building_name((ushort)building.game_id);
                SelectedBuildingNPCID.Text = building.npc_id.ToString();
                SelectedBuildingX.Text = building.grid_position.x.ToString();
                SelectedBuildingY.Text = building.grid_position.y.ToString();
                SelectedBuildingAngle.Text = building.angle.ToString();
                SelectedBuildingLevel.Text = building.level.ToString();
                SelectedBuildingRace.Text = building.race_id.ToString();
                map.selection_helper.SelectBuilding(building);
                if (move_vision)
                    MainForm.mapedittool.SetCameraViewPoint(building.grid_position);
            }
        }

        private void SelectedBuildingID_Validated(object sender, EventArgs e)
        {
            if (selected_building == -1)
                return;

            ushort new_building_id = Utility.TryParseUInt16(SelectedBuildingID.Text);

            SFMapBuilding building = map.building_manager.buildings[selected_building];
            if (building.game_id == new_building_id)
                return;

            // check if new building exists
            if (map.gamedata.categories[23].get_element_index(new_building_id) == -1)
                return;

            map.ReplaceBuilding(selected_building, new_building_id);

            SelectedBuildingName.Text = SFCFF.SFCategoryManager.get_building_name(new_building_id);
            ListBuildings.Items[selected_building] = GetBuildingString(building);
            MainForm.mapedittool.update_render = true;
        }

        private void SelectedBuildingID_MouseDown(object sender, MouseEventArgs e)
        {
            if (selected_building == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                if (MainForm.data == null)
                    return;
                if (!SFCFF.SFCategoryManager.ready)
                    return;

                ushort building_id = Utility.TryParseUInt16(SelectedBuildingID.Text);

                MainForm.data.Tracer_StepForward(23, building_id, false);
            }
        }

        private void SelectedBuildingAngleTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (selected_building == -1)
                return;

            SFMapBuilding building = map.building_manager.buildings[selected_building];
            SelectedBuildingAngle.Text = SelectedBuildingAngleTrackBar.Value.ToString();
            building.angle = SelectedBuildingAngleTrackBar.Value;
            map.RotateBuilding(selected_building, building.angle);

            MainForm.mapedittool.update_render = true;
        }

        private void SelectedBuildingAngle_Validated(object sender, EventArgs e)
        {
            SelectedBuildingAngleTrackBar.Value = (int)(Math.Max((ushort)0, Math.Min((ushort)359, Utility.TryParseUInt16(SelectedBuildingAngle.Text))));
        }

        private void SelectedBuildingLevel_Validated(object sender, EventArgs e)
        {
            if (selected_building == -1)
                return;

            SFMapBuilding building = map.building_manager.buildings[selected_building];
            building.level = Utility.TryParseUInt8(SelectedBuildingLevel.Text);
        }

        private void SelectedBuildingRace_Validated(object sender, EventArgs e)
        {
            if (selected_building == -1)
                return;

            SFMapBuilding building = map.building_manager.buildings[selected_building];
            building.race_id = Utility.TryParseUInt8(SelectedBuildingRace.Text);
        }

        private void SelectedBuildingNPCID_Validated(object sender, EventArgs e)
        {
            if (selected_building == -1)
                return;

            int current_npc_id = map.building_manager.buildings[selected_building].npc_id;
            int new_npc_id = (int)Utility.TryParseUInt32(SelectedBuildingNPCID.Text);

            if (current_npc_id == new_npc_id)
                return;
            
            if(new_npc_id == 0)
            {
                map.npc_manager.RemoveNPCRef(current_npc_id);
                map.building_manager.buildings[selected_building].npc_id = 0;
            }
            else
            {
                // if new id does not exist in gamedata, return
                if (SFCFF.SFCategoryManager.gamedata.categories[36].get_element_index(new_npc_id) == -1)
                {
                    SelectedBuildingNPCID.Text = current_npc_id.ToString();
                    return;
                }
                // if id is used, remove it
                if(current_npc_id != 0)
                    map.npc_manager.RemoveNPCRef(current_npc_id);
                // add new id
                map.npc_manager.AddNPCRef(new_npc_id, map.building_manager.buildings[selected_building]);
                map.building_manager.buildings[selected_building].npc_id = new_npc_id;
            }
        }

        private void SelectedBuildingNPCID_MouseDown(object sender, MouseEventArgs e)
        {
            if (selected_building == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                MapInspectorNPCControl npc_control = (MapInspectorNPCControl)(MainForm.mapedittool.GetEditorControl(8));
                int npc_id = (int)Utility.TryParseUInt32(SelectedBuildingNPCID.Text);
                if (npc_id != 0)
                {
                    MainForm.mapedittool.SetEditMode(special_forms.MAPEDIT_MODE.NPC);
                    int npc_index = npc_control.indices_to_keys.IndexOf(npc_id);
                    npc_control.SelectNPC(npc_index, false);
                    return;
                }

                npc_id = npc_control.FindLastUnusedNPCID();
                if (npc_id == -1)
                    return;

                map.npc_manager.AddNPCRef(npc_id, map.building_manager.buildings[selected_building]);
                map.building_manager.buildings[selected_building].npc_id = npc_id;
                SelectedBuildingNPCID.Text = npc_id.ToString();
            }
        }
    }
}
