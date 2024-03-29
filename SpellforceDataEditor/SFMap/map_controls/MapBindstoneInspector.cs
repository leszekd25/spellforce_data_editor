﻿using SFEngine.SFCFF;
using SFEngine.SFMap;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapBindstoneInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        bool move_camera_on_select = false;
        bool bindstone_selected_from_list = true;

        bool trackbar_clicked = false;
        int trackbar_initial_angle = -1;

        public MapBindstoneInspector()
        {
            InitializeComponent();
        }

        private void MapBindstoneInspector_Load(object sender, EventArgs e)
        {
            ReloadList();
            ResizeList();
        }

        private void ReloadList()
        {
            ListBindstones.Items.Clear();
            for (int i = 0; i < map.int_object_manager.bindstones_index.Count; i++)
            {
                LoadNextBindstone(i);
            }
        }

        private int GetBindstoneIndex(SFMapInteractiveObject o)
        {
            int i = map.int_object_manager.int_objects.IndexOf(o);
            if (i != SFEngine.Utility.NO_INDEX)
            {
                return map.int_object_manager.bindstones_index.IndexOf(i);
            }

            return SFEngine.Utility.NO_INDEX;
        }

        private int GetPlayerIndexByBindstoneIndex(int i)
        {
            return map.metadata.FindPlayerByBindstoneIndex(i);
        }

        private string GetBindstoneString(SFMapInteractiveObject io)
        {
            int player = map.metadata.FindPlayerBySpawnPos(io.grid_position);
            if (player == SFEngine.Utility.NO_INDEX)
            {
                return "Bindstone at " + io.grid_position.ToString();
            }

            if (map.metadata.spawns[player].text_id == 0)
            {
                return "Bindstone at " + io.grid_position.ToString();
            }

            SFCategoryElement elem = SFCategoryManager.FindElementText(
                map.metadata.spawns[player].text_id, SFEngine.Settings.LanguageID);
            if (elem == null)
            {
                return "Bindstone at " + io.grid_position.ToString();
            }

            string ret = elem.variants[4].ToString();
            return ret + " " + io.grid_position.ToString();

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
            PanelBindstonesList.Height = Height - PanelBindstonesList.Location.Y - 3;
            ListBindstones.Height = PanelBindstonesList.Height - 75;
        }

        public void RemoveBindstone(int index)
        {
            if (ListBindstones.SelectedIndex == index)
            {
                PanelProperties.Enabled = false;
            }

            ListBindstones.Items.RemoveAt(index);
        }

        public void LoadNextBindstone(int index)
        {
            SFMapInteractiveObject io = map.int_object_manager.int_objects[map.int_object_manager.bindstones_index[index]];
            ListBindstones.Items.Insert(index, GetBindstoneString(io));
        }

        private void HideList()
        {
            if (ButtonResizeList.Text == "+")
            {
                return;
            }

            PanelBindstonesList.Height = 30;

            ButtonResizeList.Text = "+";
        }

        public override void OnSelect(object o)
        {
            move_camera_on_select = false;
            bindstone_selected_from_list = false;

            if (o == null)
            {
                map.selection_helper.CancelSelection();
                PanelProperties.Enabled = false;
            }
            else
            {
                ListBindstones.SelectedIndex = GetBindstoneIndex((SFMapInteractiveObject)o);
            }
        }

        private void ListBindstones_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBindstones.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            PanelProperties.Enabled = true;
            SFMapInteractiveObject bindstone = map.int_object_manager.int_objects[map.int_object_manager.bindstones_index[ListBindstones.SelectedIndex]];
            int player = GetPlayerIndexByBindstoneIndex(ListBindstones.SelectedIndex);
            if (player == -1)
            {
                SFEngine.LogUtils.Log.Warning(SFEngine.LogUtils.LogSource.SFMap,
                    "MapBindstoneInspector.ListBindstones_SelectedIndexChanged(): Can't find player at position "
                    + bindstone.grid_position.ToString());
            }
            else
            {
                TextID.Text = map.metadata.spawns[player].text_id.ToString();
                Unknown.Text = map.metadata.spawns[player].unknown.ToString();
            }
            PosX.Text = bindstone.grid_position.x.ToString();
            PosY.Text = bindstone.grid_position.y.ToString();
            AngleTrackbar.Value = bindstone.angle;
            // angle, angletrackbar

            map.selection_helper.SelectInteractiveObject(bindstone);
            if ((move_camera_on_select) || (bindstone_selected_from_list))
            {
                MainForm.mapedittool.SetCameraViewPoint(bindstone.grid_position);
            }

            move_camera_on_select = false;
            bindstone_selected_from_list = true;
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

        private void Angle_Validated(object sender, EventArgs e)
        {
            if (ListBindstones.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFMapInteractiveObject bindstone = map.int_object_manager.int_objects[map.int_object_manager.bindstones_index[ListBindstones.SelectedIndex]];

            int v = SFEngine.Utility.TryParseUInt16(Angle.Text, (ushort)bindstone.angle);
            v = (v >= 0 ? (v <= 359 ? v : 359) : 0);

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.BINDSTONE,
                index = ListBindstones.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ANGLE,
                PreChangeProperty = bindstone.angle,
                PostChangeProperty = v
            });

            AngleTrackbar.Value = v;
        }

        private void AngleTrackbar_ValueChanged(object sender, EventArgs e)
        {
            if (ListBindstones.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFMapInteractiveObject bindstone = map.int_object_manager.int_objects[map.int_object_manager.bindstones_index[ListBindstones.SelectedIndex]];
            Angle.Text = AngleTrackbar.Value.ToString();
            map.int_object_manager.RotateInteractiveObject(map.int_object_manager.bindstones_index[ListBindstones.SelectedIndex], AngleTrackbar.Value);

            MainForm.mapedittool.update_render = true;
        }

        // this is to make sure the undo/redo queue only receives the latest angle changed as an action to perform
        private void AngleTrackbar_MouseDown(object sender, MouseEventArgs e)
        {
            if (ListBindstones.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            trackbar_clicked = true;

            if (trackbar_initial_angle == -1)
            {
                SFMapInteractiveObject bindstone = map.int_object_manager.int_objects[map.int_object_manager.bindstones_index[ListBindstones.SelectedIndex]];
                trackbar_initial_angle = bindstone.angle;
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
                type = map_operators.MapOperatorEntityType.BINDSTONE,
                index = ListBindstones.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ANGLE,
                PreChangeProperty = trackbar_initial_angle,
                PostChangeProperty = AngleTrackbar.Value
            });

            trackbar_clicked = false;
            trackbar_initial_angle = -1;
        }

        private void TextID_Validated(object sender, EventArgs e)
        {
            if (ListBindstones.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFMapInteractiveObject bindstone = map.int_object_manager.int_objects[map.int_object_manager.bindstones_index[ListBindstones.SelectedIndex]];
            int player = GetPlayerIndexByBindstoneIndex(ListBindstones.SelectedIndex);
            if (player == SFEngine.Utility.NO_INDEX)
            {
                SFEngine.LogUtils.Log.Warning(SFEngine.LogUtils.LogSource.SFMap,
                    "MapBindstoneInspector.TextID_Validated(): Can't find player at position "
                    + bindstone.grid_position.ToString());
                return;
            }

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.BINDSTONE,
                index = ListBindstones.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ID,
                PreChangeProperty = map.metadata.spawns[player].text_id,
                PostChangeProperty = SFEngine.Utility.TryParseUInt16(TextID.Text, map.metadata.spawns[player].text_id)
            });

            map.metadata.spawns[player].text_id = SFEngine.Utility.TryParseUInt16(TextID.Text, map.metadata.spawns[player].text_id);
            ListBindstones.Items[ListBindstones.SelectedIndex] = GetBindstoneString(bindstone);
        }

        private void Unknown_Validated(object sender, EventArgs e)
        {
            if (ListBindstones.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFMapInteractiveObject bindstone = map.int_object_manager.int_objects[map.int_object_manager.bindstones_index[ListBindstones.SelectedIndex]];
            int player = GetPlayerIndexByBindstoneIndex(ListBindstones.SelectedIndex);
            if (player == SFEngine.Utility.NO_INDEX)
            {
                SFEngine.LogUtils.Log.Warning(SFEngine.LogUtils.LogSource.SFMap,
                    "MapBindstoneInspector.Unknown_Validated(): Can't find player at position "
                    + bindstone.grid_position.ToString());
                return;
            }

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.BINDSTONE,
                index = ListBindstones.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.BINDSTONEUNKNOWN,
                PreChangeProperty = map.metadata.spawns[player].unknown,
                PostChangeProperty = SFEngine.Utility.TryParseInt16(Unknown.Text, map.metadata.spawns[player].unknown)
            });

            map.metadata.spawns[player].unknown = SFEngine.Utility.TryParseInt16(Unknown.Text, map.metadata.spawns[player].unknown);
        }

        private void TextID_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
            {
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = SFEngine.Utility.TryParseUInt16(TextID.Text);
                int real_elem_id = SFCategoryManager.gamedata[2016].GetElementIndex(elem_id);
                if (real_elem_id != SFEngine.Utility.NO_INDEX)
                {
                    MainForm.data.Tracer_StepForward(14, real_elem_id);
                }
            }
        }
    }
}
