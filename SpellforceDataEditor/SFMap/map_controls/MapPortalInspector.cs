using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SFEngine.SFMap;
using SFEngine.SFCFF;
using SFEngine.SFLua;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapPortalInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        bool move_camera_on_select = false;
        bool portal_selected_from_list = true;

        bool trackbar_clicked = false;
        int trackbar_initial_angle = -1;

        public MapPortalInspector()
        {
            InitializeComponent();
        }

        private void MapPortalInspector_Load(object sender, EventArgs e)
        {
            ReloadList();
            ResizeList();
        }

        private void ReloadList()
        {
            for (int i = 0; i < map.portal_manager.portals.Count; i++)
                LoadNextPortal(i);
        }

        private string GetPortalString(SFMapPortal portal)
        {
            string ret = "";
            if (SFCategoryManager.ready)
            {
                int portal_id = portal.game_id;
                int portal_index = SFCategoryManager.gamedata[2053].GetElementIndex(portal_id);
                if (portal_index != SFEngine.Utility.NO_INDEX)
                {
                    SFCategoryElement portal_data = SFCategoryManager.gamedata[2053][portal_index];
                    ret += SFCategoryManager.GetTextFromElement(portal_data, 5);
                }
            }
            ret += portal.grid_position.ToString();
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
            PanelPortalList.Height = this.Height - PanelPortalList.Location.Y - 3;
            ListPortals.Height = PanelPortalList.Height - 75;
        }

        public void RemovePortal(int index)
        {
            if (ListPortals.SelectedIndex == index)
                PanelProperties.Enabled = false;
            ListPortals.Items.RemoveAt(index);
        }

        public void LoadNextPortal(int index)
        {
            SFMapPortal portal = map.portal_manager.portals[index];

            ListPortals.Items.Insert(index, GetPortalString(portal));
        }

        private void HideList()
        {
            if (ButtonResizeList.Text == "+")
                return;

            PanelPortalList.Height = 30;

            ButtonResizeList.Text = "+";
        }

        public override void OnSelect(object o)
        {
            move_camera_on_select = false;
            portal_selected_from_list = false;

            if (o == null)
            {
                map.selection_helper.CancelSelection();
                PanelProperties.Enabled = false;
            }
            else
                ListPortals.SelectedIndex = map.portal_manager.portals.IndexOf((SFMapPortal)o);
        }

        private void ListPortals_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListPortals.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;

            PanelProperties.Enabled = true;
            SFMapPortal portal = map.portal_manager.portals[ListPortals.SelectedIndex];
            PortalID.Text = portal.game_id.ToString();
            PosX.Text = portal.grid_position.x.ToString();
            PosY.Text = portal.grid_position.y.ToString();
            AngleTrackbar.Value = portal.angle;

            map.selection_helper.SelectPortal(portal);
            if ((move_camera_on_select) || (portal_selected_from_list))
                MainForm.mapedittool.SetCameraViewPoint(portal.grid_position);
            move_camera_on_select = false;
            portal_selected_from_list = true;
        }

        private void PortalID_Validated(object sender, EventArgs e)
        {
            if (ListPortals.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.PORTAL,
                index = ListPortals.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ID,
                PreChangeProperty = map.portal_manager.portals[ListPortals.SelectedIndex].game_id,
                PostChangeProperty = SFEngine.Utility.TryParseUInt16(PortalID.Text)
            });

            map.portal_manager.portals[ListPortals.SelectedIndex].game_id = SFEngine.Utility.TryParseUInt16(PortalID.Text);
        }

        private void Angle_Validated(object sender, EventArgs e)
        {
            if (ListPortals.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;

            SFMapPortal portal = map.portal_manager.portals[ListPortals.SelectedIndex];

            int v = SFEngine.Utility.TryParseUInt16(Angle.Text, (ushort)portal.angle);
            v = (v >= 0 ? (v <= 359 ? v : 359) : 0);

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.PORTAL,
                index = ListPortals.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ANGLE,
                PreChangeProperty = portal.angle,
                PostChangeProperty = v
            });

            AngleTrackbar.Value = v;
        }

        private void AngleTrackbar_ValueChanged(object sender, EventArgs e)
        {
            if (ListPortals.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;

            SFMapPortal portal = map.portal_manager.portals[ListPortals.SelectedIndex];
            Angle.Text = AngleTrackbar.Value.ToString();
            portal.angle = AngleTrackbar.Value;
            map.RotatePortal(ListPortals.SelectedIndex, AngleTrackbar.Value);

            MainForm.mapedittool.update_render = true;
        }

        // this is to make sure the undo/redo queue only receives the latest angle changed as an action to perform
        private void AngleTrackbar_MouseDown(object sender, MouseEventArgs e)
        {
            if (ListPortals.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;

            trackbar_clicked = true;

            if (trackbar_initial_angle == -1)
            {
                SFMapPortal portal = map.portal_manager.portals[ListPortals.SelectedIndex];
                trackbar_initial_angle = portal.angle;
            }
        }

        private void AngleTrackbar_MouseUp(object sender, MouseEventArgs e)
        {
            if (!trackbar_clicked)
                return;

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.PORTAL,
                index = ListPortals.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ANGLE,
                PreChangeProperty = trackbar_initial_angle,
                PostChangeProperty = AngleTrackbar.Value
            });

            trackbar_clicked = false;
        }

        private void ButtonResizeList_Click(object sender, EventArgs e)
        {
            if (ButtonResizeList.Text == "-")
                HideList();
            else
                ShowList();
        }

        private void PortalID_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = SFEngine.Utility.TryParseUInt16(PortalID.Text);
                int real_elem_id = SFCategoryManager.gamedata[2053].GetElementIndex(elem_id);
                if (real_elem_id != SFEngine.Utility.NO_INDEX)
                    MainForm.data.Tracer_StepForward(38, real_elem_id);
            }
        }
    }
}
