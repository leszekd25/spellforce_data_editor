using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapUnitInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        bool move_camera_on_select = false;
        bool unit_selected_from_list = true;

        public MapUnitInspector()
        {
            InitializeComponent();
        }

        private void MapUnitInspector_Load(object sender, EventArgs e)
        {
            ReloadList();
            ResizeList();
        }

        private void ReloadList()
        {
            ListUnits.Items.Clear();
            for (int i = 0; i < map.unit_manager.units.Count; i++)
                LoadNextUnit();
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
            PanelUnitList.Height = this.Height - PanelUnitList.Location.Y - 3;
            ListUnits.Height = PanelUnitList.Height - 125;
            SearchUnitText.Location = new Point(SearchUnitText.Location.X, ListUnits.Location.Y + ListUnits.Height + 8);
            SearchUnitNext.Location = new Point(SearchUnitNext.Location.X, SearchUnitText.Location.Y + 28);
            SearchUnitPrevious.Location = new Point(SearchUnitPrevious.Location.X, SearchUnitText.Location.Y + 28);
        }

        private void HideList()
        {
            if (ButtonResizeList.Text == "+")
                return;

            PanelUnitList.Height = 30;

            ButtonResizeList.Text = "+";
        }

        public void RemoveUnit(int index)
        {
            if (ListUnits.SelectedIndex == index)
                PanelProperties.Enabled = false;
            ListUnits.Items.RemoveAt(index);
        }

        public void LoadNextUnit()
        {
            if (ListUnits.Items.Count >= map.unit_manager.units.Count)
                return;

            string unit_name = SFCFF.SFCategoryManager.GetUnitName((ushort)map.unit_manager.units[ListUnits.Items.Count].game_id, true);
            unit_name += " " + map.unit_manager.units[ListUnits.Items.Count].grid_position.ToString();
            ListUnits.Items.Add(unit_name);
        }

        private void MapUnitInspector_Resize(object sender, EventArgs e)
        {
            if (ButtonResizeList.Text == "+")
                return;

            ResizeList();
        }

        public override void OnSelect(object o)
        {
            move_camera_on_select = false;
            unit_selected_from_list = false;

            if (o == null)
            {
                map.selection_helper.CancelSelection();
                PanelProperties.Enabled = false;
            }
            else
                ListUnits.SelectedIndex = map.unit_manager.units.IndexOf((SFMapUnit)o);
        }

        private void ButtonResizeList_Click(object sender, EventArgs e)
        {
            if (ButtonResizeList.Text == "-")
                HideList();
            else
                ShowList();
        }

        private void ListUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListUnits.SelectedIndex == Utility.NO_INDEX)
                return;

            PanelProperties.Enabled = true;
            SFMapUnit unit = map.unit_manager.units[ListUnits.SelectedIndex];
            UnitID.Text = unit.game_id.ToString();
            NPCID.Text = unit.npc_id.ToString();
            PosX.Text = unit.grid_position.x.ToString();
            PosY.Text = unit.grid_position.y.ToString();
            AngleTrackbar.Value = unit.angle;
            Unknown1.Text = unit.unknown.ToString();
            Group.Text = unit.group.ToString();
            Unknown2.Text = unit.unknown2.ToString();
            
            map.selection_helper.SelectUnit(unit);
            if ((move_camera_on_select)||(unit_selected_from_list))
                MainForm.mapedittool.SetCameraViewPoint(unit.grid_position);
            move_camera_on_select = false;
            unit_selected_from_list = true;
        }

        private void UnitID_Validated(object sender, EventArgs e)
        {
            if (ListUnits.SelectedIndex == Utility.NO_INDEX)
                return;

            int new_unit_id = Utility.TryParseUInt16(UnitID.Text);
            SFMapUnit unit = map.unit_manager.units[ListUnits.SelectedIndex];
            if (unit.game_id == new_unit_id)
                return;

            if (SFCFF.SFCategoryManager.gamedata[17].GetElementIndex(new_unit_id) == -1)
                return;

            map.ReplaceUnit(ListUnits.SelectedIndex, (ushort)new_unit_id);

            LabelUnitName.Text = SFCFF.SFCategoryManager.GetUnitName((ushort)new_unit_id, true);
            ListUnits.Items[ListUnits.SelectedIndex] = LabelUnitName.Text + " " +
                map.unit_manager.units[ListUnits.SelectedIndex].grid_position.ToString();

            MainForm.mapedittool.update_render = true;
        }

        private void NPCID_Validated(object sender, EventArgs e)
        {
            if (ListUnits.SelectedIndex == Utility.NO_INDEX)
                return;

            SFMapUnit unit = map.unit_manager.units[ListUnits.SelectedIndex];

            int npc_id = Utility.TryParseInt32(NPCID.Text);

            // find if any npc exists
            object entity = map.FindNPCEntity(npc_id);
            if(entity != null)
            {
                MessageBox.Show("Duplicate NPC ID " + npc_id + " found. Unable to change selected unit ID.");
                NPCID.Text = unit.npc_id.ToString();
            }

            unit.npc_id = npc_id;
        }

        private void NPCScript_Click(object sender, EventArgs e)
        {
            if (ListUnits.SelectedIndex == Utility.NO_INDEX)
                return;

            SFMapUnit unit = map.unit_manager.units[ListUnits.SelectedIndex];
            if (unit.npc_id == 0)
                return;

            string fname = "script\\p" + map.PlatformID.ToString() + "\\n" + unit.npc_id.ToString() + ".lua";
            if (SFLua.SFLuaEnvironment.OpenNPCScript((int)map.PlatformID, unit.npc_id) != 0)
                MessageBox.Show("Could not open " + fname);
        }

        private void Angle_Validated(object sender, EventArgs e)
        {
            if (ListUnits.SelectedIndex == Utility.NO_INDEX)
                return;

            SFMapUnit unit = map.unit_manager.units[ListUnits.SelectedIndex];

            int v = Utility.TryParseUInt16(Angle.Text, (ushort)unit.angle);
            AngleTrackbar.Value = (v >= 0 ? (v <= 359 ? v : 359) : 0);
        }

        private void AngleTrackbar_ValueChanged(object sender, EventArgs e)
        {
            if (ListUnits.SelectedIndex == Utility.NO_INDEX)
                return;

            SFMapUnit unit = map.unit_manager.units[ListUnits.SelectedIndex];
            Angle.Text = AngleTrackbar.Value.ToString();
            unit.angle = AngleTrackbar.Value;
            map.RotateUnit(ListUnits.SelectedIndex, unit.angle);

            MainForm.mapedittool.update_render = true;
        }


        private void Unknown1_Validated(object sender, EventArgs e)
        {
            if (ListUnits.SelectedIndex == Utility.NO_INDEX)
                return;

            SFMapUnit unit = map.unit_manager.units[ListUnits.SelectedIndex];
            unit.unknown = Utility.TryParseUInt16(Unknown1.Text);
        }

        private void Group_Validated(object sender, EventArgs e)
        {
            if (ListUnits.SelectedIndex == Utility.NO_INDEX)
                return;

            SFMapUnit unit = map.unit_manager.units[ListUnits.SelectedIndex];
            unit.group = Utility.TryParseUInt16(Group.Text);
        }

        private void Unknown2_Validated(object sender, EventArgs e)
        {
            if (ListUnits.SelectedIndex == Utility.NO_INDEX)
                return;

            SFMapUnit unit = map.unit_manager.units[ListUnits.SelectedIndex];
            unit.unknown2 = Utility.TryParseUInt16(Unknown2.Text);
        }

        private void SearchUnitNext_Click(object sender, EventArgs e)
        {
            string search_phrase = SearchUnitText.Text.Trim().ToLower();
            if (search_phrase == "")
                return;

            int search_start = ListUnits.SelectedIndex;

            move_camera_on_select = true;

            for (int i = search_start + 1; i < map.unit_manager.units.Count; i++)
                if (ListUnits.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListUnits.SelectedIndex = i;
                    return;
                }

            for (int i = 0; i <= search_start; i++)
                if (ListUnits.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListUnits.SelectedIndex = i;
                    return;
                }
        }

        private void SearchUnitPrevious_Click(object sender, EventArgs e)
        {
            string search_phrase = SearchUnitText.Text.Trim().ToLower();
            if (search_phrase == "")
                return;

            int search_start = ListUnits.SelectedIndex;

            move_camera_on_select = true;

            for (int i = search_start - 1; i >= 0; i--)
                if (ListUnits.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListUnits.SelectedIndex = i;
                    return;
                }

            if (search_start == -1)
                search_start = 0;

            for (int i = map.unit_manager.units.Count - 1; i >= search_start; i--)
                if (ListUnits.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListUnits.SelectedIndex = i;
                    return;
                }
        }

        private void UnitID_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(UnitID.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[17].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(17, real_elem_id);
            }
        }
    }
}
