using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapDecorationInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        int selected_dec_group = 0;

        public MapDecorationInspector()
        {
            InitializeComponent();
            for (int i = 0; i < 29; i++)
                DecGroupData.Rows.Add();
        }

        public override void OnSelect(object o)
        {
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
                    DecGroupData.Rows[i].Cells["ObjID"].Value = dg.dec_id[i+1];
                    DecGroupData.Rows[i].Cells["Weight"].Value = dg.weight[i+1];
                }
            }

            for (int i = 0; i < map.width; i++)
                for (int j = 0; j < map.height; j++)
                    map.heightmap.overlay_data_decals[j * map.width + i] = 0;
            if (selected_dec_group != 0)
            {
                for (int i = 0; i < 1048576; i++)
                {
                    if (map.decoration_manager.dec_assignment[i] == selected_dec_group)
                    {
                        SFCoord p = map.decoration_manager.GetFixedDecPosition(i);
                        map.heightmap.overlay_data_decals[p.y * map.width + p.x] = 6;
                    }
                }
                map.heightmap.RefreshOverlay();
            }
            MainForm.mapedittool.update_render = true;
        }

        private void DecGroupData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == Utility.NO_INDEX)
                return;
            
            ushort new_id = Utility.TryParseUInt16(DecGroupData.Rows[e.RowIndex].Cells[0].Value.ToString());
            byte new_weight = Utility.TryParseUInt8(DecGroupData.Rows[e.RowIndex].Cells[1].Value.ToString());
            map.decoration_manager.dec_groups[selected_dec_group].SetDecoration(e.RowIndex + 1, new_id, new_weight);
            if (e.ColumnIndex == 0)
            {
                map.decoration_manager.ModifyDecorations((byte)selected_dec_group);
                MainForm.mapedittool.update_render = true;
            }
            MainForm.mapedittool.UpdateDecGroup(selected_dec_group);
        }
    }
}
