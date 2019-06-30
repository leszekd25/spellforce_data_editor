using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapInspectorUnitControl : SpellforceDataEditor.SFMap.map_controls.MapInspectorBaseControl
    {
        bool drag_enabled = false;
        int selected_unit = -1;

        public MapInspectorUnitControl()
        {
            InitializeComponent();
        }

        private void MapInspectorUnitControl_VisibleChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;
            
            if (Visible)
            {
                ReloadUnitList();

                SelectUnit(selected_unit, false);
            }

            MainForm.mapedittool.update_render = true;
        }

        private void ReloadUnitList()
        {
            ListUnits.Items.Clear();

            foreach(SFMapUnit u in map.unit_manager.units)
                ListUnits.Items.Add(GetUnitString(u));
        }

        private string GetUnitString(SFMapUnit u)
        {
            string ret = SFCFF.SFCategoryManager.get_unit_name((ushort)u.game_id, true);
            ret += " " + u.grid_position.ToString();
            return ret;
        }

        private void ListUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectUnit(ListUnits.SelectedIndex, true);
        }

        private void UnitToPlaceID_TextChanged(object sender, EventArgs e)
        {
            ushort unit_id = Utility.TryParseUInt16(UnitToPlaceID.Text);

            UnitToPlaceNameAndLevel.Text = SFCFF.SFCategoryManager.get_unit_name(unit_id, true);
        }

        private void UnitToPlaceID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (MainForm.data == null)
                    return;
                if (!SFCFF.SFCategoryManager.ready)
                    return;

                ushort unit_id = Utility.TryParseUInt16(UnitToPlaceID.Text);

                MainForm.data.Tracer_StepForward(17, unit_id, false);
            }
        }

        public void SelectUnit(int unit_map_index, bool move_vision)
        {
            if (unit_map_index < 0)
                unit_map_index = -1;
            if (unit_map_index >= map.unit_manager.units.Count)
                unit_map_index = -1;

            selected_unit = unit_map_index;

            if(unit_map_index == -1)
            {
                SelectedUnitID.Text = "";
                SelectedUnitNameAndLevel.Text = "";
                SelectedUnitNPCID.Text = "";
                SelectedUnitX.Text = "";
                SelectedUnitY.Text = "";
                SelectedUnitAngle.Text = "";
                SelectedUnitUnk1.Text = "";
                SelectedUnitGroup.Text = "";
                SelectedUnitUnk2.Text = "";
                map.selection_helper.CancelSelection();
            }
            else
            {
                SFMapUnit unit = map.unit_manager.units[unit_map_index];
                SelectedUnitID.Text = unit.game_id.ToString();
                SelectedUnitNameAndLevel.Text = SFCFF.SFCategoryManager.get_unit_name((ushort)unit.game_id, true);
                SelectedUnitNPCID.Text = unit.npc_id.ToString();
                SelectedUnitX.Text = unit.grid_position.x.ToString();
                SelectedUnitY.Text = unit.grid_position.y.ToString();
                SelectedUnitAngle.Text = unit.angle.ToString();
                SelectedUnitUnk1.Text = unit.unknown.ToString();
                SelectedUnitGroup.Text = unit.group.ToString();
                SelectedUnitUnk2.Text = unit.unknown2.ToString();
                map.selection_helper.SelectUnit(unit);
                if(move_vision)
                    MainForm.mapedittool.SetCameraViewPoint(unit.grid_position);
            }
        }

        private void SelectedUnitID_Validated(object sender, EventArgs e)
        {
            if (selected_unit == -1)
                return;

            ushort new_unit_id = Utility.TryParseUInt16(SelectedUnitID.Text);
            
            SFMapUnit unit = map.unit_manager.units[selected_unit];
            if (unit.game_id == new_unit_id)
                return;

            // check if new unit exists
            if (map.gamedata.categories[17].get_element_index(new_unit_id) == -1)
                return;

            map.ReplaceUnit(selected_unit, new_unit_id);

            SelectedUnitNameAndLevel.Text = SFCFF.SFCategoryManager.get_unit_name(new_unit_id, true);
            ListUnits.Items[selected_unit] = GetUnitString(unit);
            MainForm.mapedittool.update_render = true;
        }

        private void SelectedUnitID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (MainForm.data == null)
                    return;
                if (!SFCFF.SFCategoryManager.ready)
                    return;

                ushort unit_id = Utility.TryParseUInt16(SelectedUnitID.Text);

                MainForm.data.Tracer_StepForward(17, unit_id, false);
            }
        }

        private void SelectedUnitAngleTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (selected_unit == -1)
                return;

            SFMapUnit unit = map.unit_manager.units[selected_unit];
            SelectedUnitAngle.Text = SelectedUnitAngleTrackBar.Value.ToString();
            unit.angle = SelectedUnitAngleTrackBar.Value;
            map.RotateUnit(selected_unit, unit.angle);

            MainForm.mapedittool.update_render = true;
        }

        private void SelectedUnitAngle_Validated(object sender, EventArgs e)
        {
            SelectedUnitAngleTrackBar.Value = (int)(Math.Max((ushort)0, Math.Min((ushort)359, Utility.TryParseUInt16(SelectedUnitAngle.Text))));
        }

        private void SelectedUnitUnk1_Validated(object sender, EventArgs e)
        {
            if (selected_unit == -1)
                return;

            SFMapUnit unit = map.unit_manager.units[selected_unit];
            unit.unknown = Utility.TryParseUInt16(SelectedUnitUnk1.Text);
        }

        private void SelectedUnitGroup_Validated(object sender, EventArgs e)
        {
            if (selected_unit == -1)
                return;

            SFMapUnit unit = map.unit_manager.units[selected_unit];
            unit.group = Utility.TryParseUInt8(SelectedUnitGroup.Text);
        }

        private void SelectedUnitUnk2_Validated(object sender, EventArgs e)
        {
            if (selected_unit == -1)
                return;

            SFMapUnit unit = map.unit_manager.units[selected_unit];
            unit.unknown2 = Utility.TryParseUInt8(SelectedUnitUnk2.Text);
        }

        public override void OnMouseDown(SFCoord clicked_pos, MouseButtons button)
        {
            if (map == null)
                return;

            SFCoord fixed_pos = new SFCoord(clicked_pos.x, map.height - clicked_pos.y - 1);
            // get unit under position
            SFMapUnit unit = null;
            SFMapHeightMapChunk chunk = map.heightmap.GetChunk(fixed_pos);
            foreach (SFMapUnit u in chunk.units)
            {
                if (u.grid_position == fixed_pos)
                {
                    unit = u;
                    break;
                }
            }

            // if no unit under the cursor and left mouse clicked, create new unit
            if (unit == null)
            {
                if (button == MouseButtons.Left)
                {
                    // if dragging unit, just move selected unit, dont create a new one
                    if (drag_enabled)
                    {
                        if (map.heightmap.CanMoveToPosition(fixed_pos))
                            map.MoveUnit(selected_unit, fixed_pos);
                    }
                    else
                    {
                        // check if can place
                        if (map.heightmap.CanMoveToPosition(fixed_pos))
                        {
                            ushort new_unit_id = Utility.TryParseUInt16(UnitToPlaceID.Text);
                            if (map.gamedata.categories[17].get_element_index(new_unit_id) == -1)
                                return;
                            // create new unit and drag it until mouse released
                            map.AddUnit(new_unit_id, fixed_pos, 0, 0, 0, 0, 0);
                            ListUnits.Items.Add(GetUnitString(map.unit_manager.units[map.unit_manager.units.Count - 1]));
                            SelectUnit(map.unit_manager.units.Count - 1, false);
                            drag_enabled = true;
                        }
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
                        if (map.heightmap.CanMoveToPosition(fixed_pos))
                            map.MoveUnit(selected_unit, fixed_pos);
                    }
                    else
                    {
                        // find selected unit id
                        int unit_map_index = map.unit_manager.units.IndexOf(unit);
                        if (unit_map_index == -1)
                            return;

                        SelectUnit(unit_map_index, false);
                        drag_enabled = true;
                    }
                }
                // delete unit
                else if (button == MouseButtons.Right)
                {
                    int unit_map_index = map.unit_manager.units.IndexOf(unit);
                    if (unit_map_index == -1)
                        return;

                    if (unit_map_index == selected_unit)
                        SelectUnit(-1, false);

                    map.DeleteUnit(unit_map_index);
                    ListUnits.Items.RemoveAt(unit_map_index);
                }
            }
        }

        public override void OnMouseUp()
        {
            drag_enabled = false;
        }

        private void SelectedUnitNPCID_MouseDown(object sender, MouseEventArgs e)
        {
            if (selected_unit == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                MapInspectorNPCControl npc_control = (MapInspectorNPCControl)(MainForm.mapedittool.GetEditorControl(8));
                int npc_id = (int)Utility.TryParseUInt32(SelectedUnitNPCID.Text);
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

                map.npc_manager.AddNPCRef(npc_id, map.unit_manager.units[selected_unit]);
                map.unit_manager.units[selected_unit].npc_id = npc_id;
                SelectedUnitNPCID.Text = npc_id.ToString();
            }
        }

        private void UnitListFindNext_Click(object sender, EventArgs e)
        {
            if (map == null)
                return;

            string search_phrase = UnitListSearchPhrase.Text.ToLower();
            if (search_phrase == "")
                return;

            int search_start = ListUnits.SelectedIndex;

            for(int i = search_start+1; i < map.unit_manager.units.Count; i++)
                if(ListUnits.Items[i].ToString().ToLower().Contains(search_phrase))
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

        private void UnitListFindPrevious_Click(object sender, EventArgs e)
        {
            if (map == null)
                return;

            string search_phrase = UnitListSearchPhrase.Text.ToLower();
            if (search_phrase == "")
                return;

            int search_start = ListUnits.SelectedIndex;

            for (int i = search_start-1; i >= 0; i--)
                if (ListUnits.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListUnits.SelectedIndex = i;
                    return;
                }

            if (search_start == -1)
                search_start = 0;

            for (int i = map.unit_manager.units.Count-1; i >= search_start; i--)
                if (ListUnits.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListUnits.SelectedIndex = i;
                    return;
                }
        }
    }
}
