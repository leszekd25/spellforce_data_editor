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
            string ret = SFCFF.SFCategoryManager.GetBuildingName((ushort)b.game_id);
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

            BuildingToPlaceName.Text = SFCFF.SFCategoryManager.GetBuildingName(building_id);
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
                SelectedBuildingName.Text = SFCFF.SFCategoryManager.GetBuildingName((ushort)building.game_id);
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
            if (map.gamedata[23].GetElementIndex(new_building_id) == -1)
                return;

            map.ReplaceBuilding(selected_building, new_building_id);

            SelectedBuildingName.Text = SFCFF.SFCategoryManager.GetBuildingName(new_building_id);
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

            // clear overlay at current building
            /*HashSet<SFCoord> pot_cells_taken = new HashSet<SFCoord>();
            map.building_manager.BoundaryFits(building.game_id, building.grid_position, building.angle, pot_cells_taken);
            foreach (SFCoord p in pot_cells_taken)
                map.heightmap.OverlayRemove("BuildingBlock", p);*/

            building.angle = SelectedBuildingAngleTrackBar.Value;
            map.RotateBuilding(selected_building, building.angle);

            /*map.building_manager.BoundaryFits(building.game_id, building.grid_position, building.angle, pot_cells_taken);
            foreach (SFCoord p in pot_cells_taken)
                map.heightmap.OverlayAdd("BuildingBlock", p);

            foreach (SFMapHeightMapChunk chunk in map.heightmap.chunks)
                chunk.OverlayUpdate("BuildingBlock");*/

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
                if (SFCFF.SFCategoryManager.gamedata[36].GetElementIndex(new_npc_id) == -1)
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

        public override void OnMouseDown(SFCoord clicked_pos, MouseButtons button)
        {
            if (map == null)
                return;

            SFCoord fixed_pos = new SFCoord(clicked_pos.x, map.height - clicked_pos.y - 1);
            // get unit under position
            SFMapBuilding building = null;
            foreach(SFMapBuilding b in map.building_manager.buildings)
            {
                float sel_scale = 0.0f;
                SFLua.lua_sql.SFLuaSQLBuildingData bld_data = SFLua.SFLuaEnvironment.buildings[b.game_id];
                if (bld_data != null)
                    sel_scale = (float)(bld_data.SelectionScaling / 2);

                OpenTK.Vector2 off = map.building_manager.building_collision[(ushort)b.game_id].collision_mesh.origin;
                float angle = (float)(b.angle * Math.PI / 180);
                OpenTK.Vector2 r_off = new OpenTK.Vector2(off.X, off.Y);
                r_off.X = (float)((Math.Cos(angle) * off.X) - (Math.Sin(angle) * off.Y));
                r_off.Y = (float)((Math.Sin(angle) * off.X) + (Math.Cos(angle) * off.Y));
                SFCoord offset_pos = new SFCoord((int)r_off.X, (int)r_off.Y);

                if (SFCoord.Distance(b.grid_position-offset_pos, fixed_pos) <= sel_scale)
                {
                    building = b;
                    break;
                }
            }

            // if no unit under the cursor and left mouse clicked, create new unit
            if (building == null)
            {
                if (button == MouseButtons.Left)
                {
                    // if dragging unit, just move selected unit, dont create a new one
                    if (drag_enabled)
                    {
                        //if (map.heightmap.CanMoveToPosition(fixed_pos))
                            map.MoveBuilding(selected_building, fixed_pos);
                    }
                    else
                    {
                        // check if can place
                        //if (map.heightmap.CanMoveToPosition(fixed_pos))
                        //{
                        ushort new_building_id = Utility.TryParseUInt16(BuildingToPlaceID.Text);
                        if (map.gamedata[23].GetElementIndex(new_building_id) == -1)
                            return;
                        // create new unit and drag it until mouse released
                        map.AddBuilding(new_building_id, fixed_pos, 0, 0, 1, -1);
                        ListBuildings.Items.Add(GetBuildingString(map.building_manager.buildings[map.building_manager.buildings.Count - 1]));
                        SelectBuilding(map.building_manager.buildings.Count - 1, false);
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
                            map.MoveBuilding(selected_building, fixed_pos);
                    }
                    else
                    {
                        // find selected unit id
                        int building_map_index = map.building_manager.buildings.IndexOf(building);
                        if (building_map_index == -1)
                            return;

                        SelectBuilding(building_map_index, false);
                        drag_enabled = true;
                    }
                }
                // delete unit
                else if (button == MouseButtons.Right)
                {
                    int building_map_index = map.building_manager.buildings.IndexOf(building);
                    if (building_map_index == -1)
                        return;

                    if (building_map_index == selected_building)
                        SelectBuilding(-1, false);

                    map.DeleteBuilding(building_map_index);
                    ListBuildings.Items.RemoveAt(building_map_index);
                }
            }
        }

        public override void OnMouseUp()
        {
            drag_enabled = false;
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
                    if (npc_index == -1)
                    {
                        map.npc_manager.AddNPCRef(npc_id, map.building_manager.buildings[selected_building]);
                        npc_control.AddNewNPCID(npc_id);
                        map.building_manager.buildings[selected_building].npc_id = npc_id;
                        SelectedBuildingNPCID.Text = npc_id.ToString();
                        npc_control.ReloadNPCList();
                        npc_index = npc_control.indices_to_keys.IndexOf(npc_id);
                    }
                    npc_control.SelectNPC(npc_index, false);
                }
                else
                {
                    npc_id = npc_control.FindLastUnusedNPCID();
                    if (npc_id == -1)
                        return;

                    map.npc_manager.AddNPCRef(npc_id, map.building_manager.buildings[selected_building]);
                    npc_control.AddNewNPCID(npc_id);
                    map.building_manager.buildings[selected_building].npc_id = npc_id;
                    SelectedBuildingNPCID.Text = npc_id.ToString();
                }
            }
        }

        private void BuildingListFindNext_Click(object sender, EventArgs e)
        {
            if (map == null)
                return;

            string search_phrase = BuildingListSearchPhrase.Text.ToLower();
            if (search_phrase == "")
                return;

            int search_start = ListBuildings.SelectedIndex;

            for (int i = search_start + 1; i < map.building_manager.buildings.Count; i++)
                if (ListBuildings.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListBuildings.SelectedIndex = i;
                    return;
                }

            for (int i = 0; i <= search_start; i++)
                if (ListBuildings.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListBuildings.SelectedIndex = i;
                    return;
                }
        }

        private void BuildingListFindPrevious_Click(object sender, EventArgs e)
        {
            if (map == null)
                return;

            string search_phrase = BuildingListSearchPhrase.Text.ToLower();
            if (search_phrase == "")
                return;

            int search_start = ListBuildings.SelectedIndex;

            for (int i = search_start - 1; i >= 0; i--)
                if (ListBuildings.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListBuildings.SelectedIndex = i;
                    return;
                }

            if (search_start == -1)
                search_start = 0;

            for (int i = map.building_manager.buildings.Count - 1; i >= search_start; i--)
                if (ListBuildings.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListBuildings.SelectedIndex = i;
                    return;
                }
        }
    }
}
