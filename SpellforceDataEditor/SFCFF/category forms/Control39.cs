using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control39 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2053 c2053;

        public Control39()
        {
            InitializeComponent();

            c2053 = SFCategoryManager.gamedata.c2053;
            category = c2053;

            column_dict.Add("Portal ID", "PortalID");
            column_dict.Add("Map ID", "MapID");
            column_dict.Add("Position X", "PosX");
            column_dict.Add("Position Y", "PosY");
            column_dict.Add("Is default", "IsDefault");
            column_dict.Add("Name ID", "NameID");
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            c2053.SetID(current_element, SFEngine.Utility.TryParseUInt16(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2053.SetField(current_element, "MapID", SFEngine.Utility.TryParseUInt32(textBox1.Text));
        }

        private void tb_rng_min_TextChanged(object sender, EventArgs e)
        {
            c2053.SetField(current_element, "PosX", SFEngine.Utility.TryParseUInt16(tb_rng_min.Text));
        }

        private void tb_rng_max_TextChanged(object sender, EventArgs e)
        {
            c2053.SetField(current_element, "PosY", SFEngine.Utility.TryParseUInt16(tb_rng_max.Text));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            c2053.SetField(current_element, "IsDefault", (Byte)(checkBox1.Checked ? 1 : 0));
        }

        private void tb_req4_1_TextChanged(object sender, EventArgs e)
        {
            c2053.SetField(current_element, "NameID", SFEngine.Utility.TryParseUInt16(tb_req4_1.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = c2053[current_element].PortalID.ToString();
            textBox1.Text = c2053[current_element].MapID.ToString();
            tb_rng_min.Text = c2053[current_element].PosX.ToString();
            tb_rng_max.Text = c2053[current_element].PosY.ToString();
            checkBox1.Checked = (c2053[current_element].IsDefault != 0);
            tb_req4_1.Text = c2053[current_element].NameID.ToString();
        }


        public override string get_element_string(int index)
        {
            return $"{c2053[index].PortalID} {SFCategoryManager.GetTextByLanguage(c2053[index].NameID, SFEngine.Settings.LanguageID)}";
        }

        public override string get_description_string(int index)
        {
            string map_handle = "";
            UInt32 map_id = c2053[index].MapID;
            if (SFCategoryManager.gamedata.c2052.GetItemIndex((int)map_id, out int map_index))
            {
                map_handle = SFCategoryManager.gamedata.c2052[map_index].GetHandleString();
            }
            else
            {
                map_handle = SFEngine.Utility.S_ITEM_MISSING;
            }
            return $"Map handle: {map_handle}";
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2052, textBox1.Text);
        }

        private void tb_req4_1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, tb_req4_1.Text);
        }
    }
}
