﻿using SFEngine.SFMap;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapDecorationInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        public int selected_dec_group = 0;

        public MapDecorationInspector()
        {
            InitializeComponent();
            for (int i = 0; i < 29; i++)
            {
                DecGroupData.Rows.Add();
            }
        }

        public override void OnSelect(object o)
        {
            if (o == null)
            {
                return;
            }

            int dec_group = (int)o;
            selected_dec_group = dec_group;
            DecGroupName.Text = dec_group.ToString();
            if (dec_group == 0)
            {
                DecGroupData.Visible = false;
                DecGroupName.Text = "None";
            }
            else
            {
                DecGroupData.Visible = true;
                SFMapDecorationGroup dg = map.decoration_manager.dec_groups[dec_group];
                for (int i = 0; i < 29; i++)
                {
                    DecGroupData.Rows[i].Cells["ObjID"].Value = dg.dec_id[i + 1];
                    DecGroupData.Rows[i].Cells["Weight"].Value = dg.weight[i + 1];
                }
            }

            map.heightmap.UpdateDecorationMap(selected_dec_group);
            map.heightmap.RefreshOverlay();
            MainForm.mapedittool.update_render = true;
        }

        public void external_EditRow(int row_index, ushort id, byte weight)
        {
            if (row_index < 0)
            {
                return;
            }

            DecGroupData.Rows[row_index].Cells[0].Value = id;
            DecGroupData.Rows[row_index].Cells[1].Value = weight;
        }

        public void external_AssignIDToSelectedRow(ushort id)
        {
            if (selected_dec_group == 0)
            {
                return;
            }

            if (DecGroupData.SelectedCells.Count == 0)
            {
                return;
            }

            int selected_row_index = DecGroupData.SelectedCells[0].RowIndex;
            for (int i = 1; i < DecGroupData.SelectedCells.Count; i++)
            {
                if (DecGroupData.SelectedCells[i].RowIndex != selected_row_index)
                {
                    return;
                }
            }

            DecGroupData.Rows[selected_row_index].Cells[0].Value = id;
            DecGroupData_CellEndEdit(null, new DataGridViewCellEventArgs(0, selected_row_index));
        }

        private void DecGroupData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            ushort new_id = SFEngine.Utility.TryParseUInt16(DecGroupData.Rows[e.RowIndex].Cells[0].Value.ToString());
            byte new_weight = SFEngine.Utility.TryParseUInt8(DecGroupData.Rows[e.RowIndex].Cells[1].Value.ToString());

            if ((new_id == map.decoration_manager.dec_groups[selected_dec_group].dec_id[e.RowIndex + 1])
                && (new_weight == map.decoration_manager.dec_groups[selected_dec_group].weight[e.RowIndex + 1]))
            {
                return;
            }

            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorDecorationModifyGroup()
            {
                group = selected_dec_group,
                index = e.RowIndex + 1,
                PreOperatorID = map.decoration_manager.dec_groups[selected_dec_group].dec_id[e.RowIndex + 1],
                PreOperatorWeight = map.decoration_manager.dec_groups[selected_dec_group].weight[e.RowIndex + 1],
                PostOperatorID = new_id,
                PostOperatorWeight = new_weight
            });

            map.decoration_manager.dec_groups[selected_dec_group].SetDecoration(e.RowIndex + 1, new_id, new_weight);

            map.decoration_manager.UpdateDecorationsOfGroup((byte)selected_dec_group);
            MainForm.mapedittool.update_render = true;

            MainForm.mapedittool.UpdateDecGroup(selected_dec_group);
        }
    }
}
