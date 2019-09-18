using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapPortalInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        bool move_camera_on_select = false;
        bool portal_selected_from_list = true;

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
            foreach (SFMapPortal portal in map.portal_manager.portals)
                ListPortals.Items.Add(GetPortalString(portal));
        }

        private string GetPortalString(SFMapPortal portal)
        {
            string ret = "";
            if (SFCFF.SFCategoryManager.ready)
            {
                int portal_id = portal.game_id;
                int portal_index = SFCFF.SFCategoryManager.gamedata[38].GetElementIndex(portal_id);
                if (portal_index != -1)
                {
                    SFCFF.SFCategoryElement portal_data = SFCFF.SFCategoryManager.gamedata[38][portal_index];
                    ret += SFCFF.SFCategoryManager.GetTextFromElement(portal_data, 5);
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

        public void LoadNextPortal()
        {
            if (ListPortals.Items.Count >= map.portal_manager.portals.Count)
                return;

            SFMapPortal portal = map.portal_manager.portals[ListPortals.Items.Count];

            ListPortals.Items.Add(GetPortalString(portal));
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
            if (ListPortals.SelectedIndex == -1)
                return;

            PanelProperties.Enabled = true;
            SFMapPortal portal = map.portal_manager.portals[ListPortals.SelectedIndex];
            PortalID.Text = portal.game_id.ToString();
            PosX.Text = portal.grid_position.x.ToString();
            PosY.Text = portal.grid_position.y.ToString();
            AngleTrackbar.Value = portal.angle;

            if ((move_camera_on_select) || (portal_selected_from_list))
                MainForm.mapedittool.SetCameraViewPoint(portal.grid_position);
            move_camera_on_select = false;
            portal_selected_from_list = true;
        }

        private void PortalID_Validated(object sender, EventArgs e)
        {
            if (ListPortals.SelectedIndex == -1)
                return;

            map.portal_manager.portals[ListPortals.SelectedIndex].game_id = Utility.TryParseUInt16(PortalID.Text);
        }

        private void Angle_Validated(object sender, EventArgs e)
        {
            if (ListPortals.SelectedIndex == -1)
                return;

            SFMapPortal portal = map.portal_manager.portals[ListPortals.SelectedIndex];

            int v = Utility.TryParseUInt16(Angle.Text, (ushort)portal.angle);
            AngleTrackbar.Value = (v >= 0 ? (v <= 359 ? v : 359) : 0);
        }

        private void AngleTrackbar_ValueChanged(object sender, EventArgs e)
        {
            if (ListPortals.SelectedIndex == -1)
                return;

            SFMapPortal portal = map.portal_manager.portals[ListPortals.SelectedIndex];
            Angle.Text = AngleTrackbar.Value.ToString();
            portal.angle = AngleTrackbar.Value;
            map.RotatePortal(ListPortals.SelectedIndex, AngleTrackbar.Value);

            MainForm.mapedittool.update_render = true;
        }

        private void ButtonResizeList_Click(object sender, EventArgs e)
        {
            if (ButtonResizeList.Text == "-")
                HideList();
            else
                ShowList();
        }
    }
}
