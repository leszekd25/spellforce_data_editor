﻿using SFEngine.SFCFF;
using SFEngine.SFLua;
using SFEngine.SFMap;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapBuildingInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        bool move_camera_on_select = false;
        bool building_selected_from_list = true;

        bool trackbar_clicked = false;
        int trackbar_initial_angle = -1;

        public MapBuildingInspector()
        {
            InitializeComponent();
        }

        private void MapBuildingInspector_Load(object sender, EventArgs e)
        {
            ReloadList();
            ResizeList();
        }

        private void ReloadList()
        {
            ListBuildings.Items.Clear();
            for (int i = 0; i < map.building_manager.buildings.Count; i++)
            {
                LoadNextBuilding(i);
            }
        }

        private void ShowList()
        {
            if (ButtonResizeList.Text == "-")
            {
                return;
            }

            ResizeList();

            ButtonResizeList.Text = "-";
        }

        private void ResizeList()
        {
            PanelBuildingList.Height = Height - PanelBuildingList.Location.Y - 3;
            ListBuildings.Height = PanelBuildingList.Height - 125;
            SearchBuildingText.Location = new Point(SearchBuildingText.Location.X, ListBuildings.Location.Y + ListBuildings.Height + 8);
            SearchBuildingNext.Location = new Point(SearchBuildingNext.Location.X, SearchBuildingText.Location.Y + 28);
            SearchBuildingPrevious.Location = new Point(SearchBuildingPrevious.Location.X, SearchBuildingText.Location.Y + 28);
        }

        private void HideList()
        {
            if (ButtonResizeList.Text == "+")
            {
                return;
            }

            PanelBuildingList.Height = 30;

            ButtonResizeList.Text = "+";
        }

        public void RemoveBuilding(int index)
        {
            if (ListBuildings.SelectedIndex == index)
            {
                PanelProperties.Enabled = false;
            }

            ListBuildings.Items.RemoveAt(index);
        }

        public void LoadNextBuilding(int index)
        {
            string building_name = SFCategoryManager.GetBuildingName((ushort)map.building_manager.buildings[index].game_id);
            building_name += " " + map.building_manager.buildings[index].grid_position.ToString();
            ListBuildings.Items.Insert(index, building_name);
        }

        private void MapBuildingInspector_Resize(object sender, EventArgs e)
        {
            if (ButtonResizeList.Text == "+")
            {
                return;
            }

            ResizeList();
        }

        public override void OnSelect(object o)
        {
            move_camera_on_select = false;
            building_selected_from_list = false;

            if (o == null)
            {
                map.selection_helper.CancelSelection();
                ((MapEdit.MapBuildingEditor)MainForm.mapedittool.selected_editor).selected_building = -1;
                PanelProperties.Enabled = false;
            }
            else
            {
                ListBuildings.SelectedIndex = map.building_manager.buildings.IndexOf((SFMapBuilding)o);
            }
        }

        private void ButtonResizeList_Click(object sender, EventArgs e)
        {
            if (ButtonResizeList.Text == "-")
            {
                HideList();
            }
            else
            {
                ShowList();
            }
        }

        private void ListBuildings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBuildings.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            } ((MapEdit.MapBuildingEditor)MainForm.mapedittool.selected_editor).selected_building = ListBuildings.SelectedIndex;

            PanelProperties.Enabled = true;
            SFMapBuilding building = map.building_manager.buildings[ListBuildings.SelectedIndex];
            BuildingID.Text = building.game_id.ToString();
            NPCID.Text = building.npc_id.ToString();
            PosX.Text = building.grid_position.x.ToString();
            PosY.Text = building.grid_position.y.ToString();
            AngleTrackbar.Value = building.angle;
            Level.Text = building.level.ToString();
            RaceID.Text = building.race_id.ToString();

            map.selection_helper.SelectBuilding(building);
            if ((move_camera_on_select) || (building_selected_from_list))
            {
                MainForm.mapedittool.SetCameraViewPoint(building.grid_position);
            }

            move_camera_on_select = false;
            building_selected_from_list = true;
        }

        private void BuildingID_Validated(object sender, EventArgs e)
        {
            if (ListBuildings.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            ushort new_building_id = SFEngine.Utility.TryParseUInt16(BuildingID.Text);

            SFMapBuilding building = map.building_manager.buildings[ListBuildings.SelectedIndex];
            if (building.game_id == new_building_id)
            {
                return;
            }

            // check if new building exists
            if (SFCategoryManager.gamedata[2029].GetElementIndex(new_building_id) == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.BUILDING,
                index = ListBuildings.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ID,
                PreChangeProperty = (ushort)building.game_id,
                PostChangeProperty = new_building_id
            });

            map.building_manager.ReplaceBuilding(ListBuildings.SelectedIndex, new_building_id);

            LabelBuildingName.Text = SFCategoryManager.GetBuildingName(new_building_id);
            ListBuildings.Items[ListBuildings.SelectedIndex] = LabelBuildingName.Text + " "
                + map.building_manager.buildings[ListBuildings.SelectedIndex].grid_position.ToString();

            map.heightmap.RefreshOverlay();
            MainForm.mapedittool.update_render = true;
        }

        private void NPCID_Validated(object sender, EventArgs e)
        {
            if (ListBuildings.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFMapBuilding building = map.building_manager.buildings[ListBuildings.SelectedIndex];

            int npc_id = SFEngine.Utility.TryParseInt32(NPCID.Text);

            // find if any npc exists
            SFMapEntity entity = map.FindNPCEntity(npc_id);
            if ((entity != null) && (!(entity is SFMapBuilding) || ((SFMapBuilding)entity) != building))
            {
                MessageBox.Show("Duplicate NPC ID " + npc_id + " found. Unable to change selected building ID.");
                NPCID.Text = building.npc_id.ToString();
            }

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.BUILDING,
                index = ListBuildings.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.NPCID,
                PreChangeProperty = building.npc_id,
                PostChangeProperty = npc_id
            });


            building.npc_id = npc_id;
        }

        private void NPCScript_Click(object sender, EventArgs e)
        {
            if (ListBuildings.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFMapBuilding building = map.building_manager.buildings[ListBuildings.SelectedIndex];
            if (building.npc_id == 0)
            {
                return;
            }

            string fname = "script\\p" + map.PlatformID.ToString() + "\\n" + building.npc_id.ToString() + ".lua";
            if (SFLuaEnvironment.OpenNPCScript((int)map.PlatformID, building.npc_id) != 0)
            {
                MessageBox.Show("Could not open " + fname);
            }
        }

        private void Angle_Validated(object sender, EventArgs e)
        {
            if (ListBuildings.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFMapBuilding building = map.building_manager.buildings[ListBuildings.SelectedIndex];

            int v = SFEngine.Utility.TryParseUInt16(Angle.Text, (ushort)building.angle);
            v = (v >= 0 ? (v <= 359 ? v : 359) : 0);

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.BUILDING,
                index = ListBuildings.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ANGLE,
                PreChangeProperty = building.angle,
                PostChangeProperty = v
            });

            AngleTrackbar.Value = v;
        }

        private void AngleTrackbar_ValueChanged(object sender, EventArgs e)
        {
            if (ListBuildings.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFMapBuilding building = map.building_manager.buildings[ListBuildings.SelectedIndex];
            Angle.Text = AngleTrackbar.Value.ToString();
            map.building_manager.RotateBuilding(ListBuildings.SelectedIndex, AngleTrackbar.Value);

            map.heightmap.RefreshOverlay();
            MainForm.mapedittool.update_render = true;
        }

        // this is to make sure the undo/redo queue only receives the latest angle changed as an action to perform
        private void AngleTrackbar_MouseDown(object sender, MouseEventArgs e)
        {
            if (ListBuildings.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            trackbar_clicked = true;

            if (trackbar_initial_angle == -1)
            {
                SFMapBuilding building = map.building_manager.buildings[ListBuildings.SelectedIndex];
                trackbar_initial_angle = building.angle;
            }
        }

        private void AngleTrackbar_MouseUp(object sender, MouseEventArgs e)
        {
            if (!trackbar_clicked)
            {
                return;
            }

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.BUILDING,
                index = ListBuildings.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ANGLE,
                PreChangeProperty = trackbar_initial_angle,
                PostChangeProperty = AngleTrackbar.Value
            });

            trackbar_clicked = false;
            trackbar_initial_angle = -1;
        }


        private void Level_Validated(object sender, EventArgs e)
        {
            if (ListBuildings.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFMapBuilding building = map.building_manager.buildings[ListBuildings.SelectedIndex];

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.BUILDING,
                index = ListBuildings.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.BUILDINGLEVEL,
                PreChangeProperty = building.level,
                PostChangeProperty = SFEngine.Utility.TryParseUInt16(Level.Text)
            });

            building.level = SFEngine.Utility.TryParseUInt16(Level.Text);
        }

        private void RaceID_Validated(object sender, EventArgs e)
        {
            if (ListBuildings.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFMapBuilding building = map.building_manager.buildings[ListBuildings.SelectedIndex];

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.BUILDING,
                index = ListBuildings.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.BUILDINGRACE,
                PreChangeProperty = building.race_id,
                PostChangeProperty = SFEngine.Utility.TryParseUInt16(RaceID.Text)
            });

            building.race_id = SFEngine.Utility.TryParseUInt16(RaceID.Text);
        }

        private void SearchBuildingNext_Click(object sender, EventArgs e)
        {
            string search_phrase = SearchBuildingText.Text.Trim().ToLower();
            if (search_phrase == "")
            {
                return;
            }

            int search_start = ListBuildings.SelectedIndex;

            move_camera_on_select = true;

            for (int i = search_start + 1; i < map.building_manager.buildings.Count; i++)
            {
                if (ListBuildings.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListBuildings.SelectedIndex = i;
                    return;
                }
            }

            for (int i = 0; i <= search_start; i++)
            {
                if (ListBuildings.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListBuildings.SelectedIndex = i;
                    return;
                }
            }
        }

        private void SearchBuildingPrevious_Click(object sender, EventArgs e)
        {
            string search_phrase = SearchBuildingText.Text.Trim().ToLower();
            if (search_phrase == "")
            {
                return;
            }

            int search_start = ListBuildings.SelectedIndex;

            move_camera_on_select = true;

            for (int i = search_start - 1; i >= 0; i--)
            {
                if (ListBuildings.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListBuildings.SelectedIndex = i;
                    return;
                }
            }

            if (search_start == -1)
            {
                search_start = 0;
            }

            for (int i = map.building_manager.buildings.Count - 1; i >= search_start; i--)
            {
                if (ListBuildings.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListBuildings.SelectedIndex = i;
                    return;
                }
            }
        }

        private void BuildingID_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
            {
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = SFEngine.Utility.TryParseUInt8(BuildingID.Text);
                int real_elem_id = SFCategoryManager.gamedata[2029].GetElementIndex(elem_id);
                if (real_elem_id != SFEngine.Utility.NO_INDEX)
                {
                    MainForm.data.Tracer_StepForward(23, real_elem_id);
                }
            }
        }
    }
}
