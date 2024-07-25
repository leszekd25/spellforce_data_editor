using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

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

            column_dict.Add("Portal ID", new int[1] { 0 });
            column_dict.Add("Map ID", new int[1] { 1 });
            column_dict.Add("Position X", new int[1] { 2 });
            column_dict.Add("Position Y", new int[1] { 3 });
            column_dict.Add("Unknown", new int[1] { 4 });
            column_dict.Add("Name ID", new int[1] { 5 });
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
            return $"{c2053[index].PortalID} {SFCategoryManager.GetTextByLanguage(c2053[index].NameID, 1)}";
        }

        public override string get_description_string(int index)
        {
            string map_handle = "";
            UInt32 map_id = c2053[index].MapID;
            if(SFCategoryManager.gamedata.c2052.GetItemIndex((int)map_id, out int map_index))
            {
                map_handle = SFCategoryManager.gamedata.c2052[map_index].GetHandleString();
            }
            else
            {
                map_handle = SFEngine.Utility.S_ITEM_MISSING;
            }
            return $"Map handle: {map_handle}";
        }
    }
}
