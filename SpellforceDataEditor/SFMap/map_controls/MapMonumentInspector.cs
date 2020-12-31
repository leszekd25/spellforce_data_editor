﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapMonumentInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        bool move_camera_on_select = false;
        bool monument_selected_from_list = true;

        bool trackbar_clicked = false;

        public MapMonumentInspector()
        {
            InitializeComponent();
        }

        private void MapMonumentInspector_Load(object sender, EventArgs e)
        {
            ReloadList();
            ResizeList();
        }

        private void ReloadList()
        {
            ListMonuments.Items.Clear();
            foreach (int i in map.int_object_manager.monuments_index)
                ListMonuments.Items.Add(GetMonumentString(map.int_object_manager.int_objects[i]));
        }

        private int GetMonumentIndex(SFMapInteractiveObject o)
        {
            int i = map.int_object_manager.int_objects.IndexOf(o);
            if(i != Utility.NO_INDEX)
                return map.int_object_manager.monuments_index.IndexOf(i);

            return Utility.NO_INDEX;
        }

        private string GetMonumentString(SFMapInteractiveObject io)
        {
            string ret = SFCFF.SFCategoryManager.GetObjectName((ushort)io.game_id)+" ";
            ret += io.grid_position.ToString();
            return ret;
        }

        private void ShowList()
        {
            if (ButtonResizeList.Text == "-")
                return;

            ResizeList();

            ButtonResizeList.Text = "-";
        }

        private void ResizeList()
        {
            PanelMonumentList.Height = this.Height - PanelMonumentList.Location.Y - 3;
            ListMonuments.Height = PanelMonumentList.Height - 75;
        }

        public void RemoveMonument(int index)
        {
            if (ListMonuments.SelectedIndex == index)
                PanelProperties.Enabled = false;
            ListMonuments.Items.RemoveAt(index);
        }

        public void LoadNextMonument()
        {
            if (ListMonuments.Items.Count >= map.int_object_manager.monuments_index.Count)
                return;

            SFMapInteractiveObject io = map.int_object_manager.int_objects[map.int_object_manager.monuments_index[ListMonuments.Items.Count]];
            ListMonuments.Items.Add(GetMonumentString(io));
        }

        private void HideList()
        {
            if (ButtonResizeList.Text == "+")
                return;

            PanelMonumentList.Height = 30;

            ButtonResizeList.Text = "+";
        }

        public override void OnSelect(object o)
        {
            move_camera_on_select = false;
            monument_selected_from_list = false;

            if (o == null)
            {
                map.selection_helper.CancelSelection();
                PanelProperties.Enabled = false;
            }
            else
                ListMonuments.SelectedIndex = GetMonumentIndex((SFMapInteractiveObject)o);
        }

        private void ListMonuments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListMonuments.SelectedIndex == Utility.NO_INDEX)
                return;

            PanelProperties.Enabled = true;
            SFMapInteractiveObject monument = map.int_object_manager.int_objects[map.int_object_manager.monuments_index[ListMonuments.SelectedIndex]];
            PosX.Text = monument.grid_position.x.ToString();
            PosY.Text = monument.grid_position.y.ToString();
            AngleTrackbar.Value = monument.angle;
            // angle, angletrackbar

            map.selection_helper.SelectInteractiveObject(monument);
            if ((move_camera_on_select) || (monument_selected_from_list))
                MainForm.mapedittool.SetCameraViewPoint(monument.grid_position);
            move_camera_on_select = false;
            monument_selected_from_list = true;
        }

        private void ButtonResizeList_Click(object sender, EventArgs e)
        {
            if (ButtonResizeList.Text == "-")
                HideList();
            else
                ShowList();
        }

        private void Angle_Validated(object sender, EventArgs e)
        {
            if (ListMonuments.SelectedIndex == Utility.NO_INDEX)
                return;

            SFMapInteractiveObject monument = map.int_object_manager.int_objects[map.int_object_manager.monuments_index[ListMonuments.SelectedIndex]];

            int v = Utility.TryParseUInt16(Angle.Text, (ushort)monument.angle);
            v = (v >= 0 ? (v <= 359 ? v : 359) : 0);

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.MONUMENT,
                index = ListMonuments.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ANGLE,
                PreChangeProperty = monument.angle,
                PostChangeProperty = v
            });

            AngleTrackbar.Value = v;
        }

        private void AngleTrackbar_ValueChanged(object sender, EventArgs e)
        {
            if (ListMonuments.SelectedIndex == Utility.NO_INDEX)
                return;

            SFMapInteractiveObject monument = map.int_object_manager.int_objects[map.int_object_manager.monuments_index[ListMonuments.SelectedIndex]];
            Angle.Text = AngleTrackbar.Value.ToString();
            monument.angle = AngleTrackbar.Value;
            map.RotateInteractiveObject(map.int_object_manager.monuments_index[ListMonuments.SelectedIndex], monument.angle);

            MainForm.mapedittool.update_render = true;
        }

        // this is to make sure the undo/redo queue only receives the latest angle changed as an action to perform
        private void AngleTrackbar_MouseDown(object sender, MouseEventArgs e)
        {
            trackbar_clicked = true;
        }

        private void AngleTrackbar_MouseUp(object sender, MouseEventArgs e)
        {
            if (!trackbar_clicked)
                return;

            SFMapInteractiveObject monument = map.int_object_manager.int_objects[map.int_object_manager.monuments_index[ListMonuments.SelectedIndex]];
            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.MONUMENT,
                index = ListMonuments.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ANGLE,
                PreChangeProperty = monument.angle,
                PostChangeProperty = AngleTrackbar.Value
            });

            trackbar_clicked = false;
        }
    }
}
