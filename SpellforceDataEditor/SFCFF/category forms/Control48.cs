using SFEngine;
using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control48 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2036 c2036;

        public Control48()
        {
            InitializeComponent();

            c2036 = SFCategoryManager.gamedata.c2036;
            category = c2036;

            column_dict.Add("Button ID", new int[1] { 0 });
            column_dict.Add("Building ID", new int[1] { 1 });
            column_dict.Add("Button name ID", new int[1] { 2 });
            column_dict.Add("Button description ID", new int[1] { 3 });
            column_dict.Add("Wood", new int[1] { 4 });
            column_dict.Add("Stone", new int[1] { 5 });
            column_dict.Add("Iron", new int[1] { 6 });
            column_dict.Add("Lenya", new int[1] { 7 });
            column_dict.Add("Aria", new int[1] { 8 });
            column_dict.Add("Moonsilver", new int[1] { 9 });
            column_dict.Add("Food", new int[1] { 10 });
            column_dict.Add("Button handle", new int[1] { 11 });
            column_dict.Add("Research time", new int[1] { 12 });
        }

        private void tb_sd1_TextChanged(object sender, EventArgs e)
        {
            c2036.SetID(current_element, SFEngine.Utility.TryParseUInt16(tb_sd1.Text));
        }

        private void tb_sd6_TextChanged(object sender, EventArgs e)
        {
            c2036.SetField(current_element, "BuildingID", SFEngine.Utility.TryParseUInt16(tb_sd6.Text));
        }

        private void tb_sd2_TextChanged(object sender, EventArgs e)
        {
            c2036.SetField(current_element, "ButtonNameID", SFEngine.Utility.TryParseUInt16(tb_sd2.Text));
        }

        private void tb_sd7_TextChanged(object sender, EventArgs e)
        {
            c2036.SetField(current_element, "ButtonDescriptionID", SFEngine.Utility.TryParseUInt16(tb_sd7.Text));
        }

        private void tb_sd3_TextChanged(object sender, EventArgs e)
        {
            c2036.SetField(current_element, "Wood", SFEngine.Utility.TryParseUInt16(tb_sd3.Text));
        }

        private void tb_sd4_TextChanged(object sender, EventArgs e)
        {
            c2036.SetField(current_element, "Stone", SFEngine.Utility.TryParseUInt16(tb_sd4.Text));
        }

        private void sb_sd5_TextChanged(object sender, EventArgs e)
        {
            c2036.SetField(current_element, "Iron", SFEngine.Utility.TryParseUInt16(sb_sd5.Text));
        }

        private void tb_sd9_TextChanged(object sender, EventArgs e)
        {
            c2036.SetField(current_element, "Lenya", SFEngine.Utility.TryParseUInt16(tb_sd9.Text));
        }

        private void tb_sd10_TextChanged(object sender, EventArgs e)
        {
            c2036.SetField(current_element, "Aria", SFEngine.Utility.TryParseUInt16(tb_sd10.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2036.SetField(current_element, "Moonsilver", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2036.SetField(current_element, "Food", SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2036.SetField(current_element, "Handle", StringUtils.FromString(textBox3.Text, 0, 64));
        }

        private void tb_sd8_TextChanged(object sender, EventArgs e)
        {
            c2036.SetField(current_element, "ResearchTime", SFEngine.Utility.TryParseUInt32(tb_sd8.Text));
        }

        public override void show_element()
        {
            tb_sd1.Text = c2036[current_element].ButtonID.ToString();
            tb_sd6.Text = c2036[current_element].BuildingID.ToString();
            tb_sd2.Text = c2036[current_element].ButtonNameID.ToString();
            tb_sd7.Text = c2036[current_element].ButtonDescriptionID.ToString();
            tb_sd3.Text = c2036[current_element].Wood.ToString();
            tb_sd4.Text = c2036[current_element].Stone.ToString();
            sb_sd5.Text = c2036[current_element].Iron.ToString();
            tb_sd9.Text = c2036[current_element].Lenya.ToString();
            tb_sd10.Text = c2036[current_element].Aria.ToString();
            textBox2.Text = c2036[current_element].Moonsilver.ToString();
            textBox1.Text = c2036[current_element].Food.ToString();
            textBox3.Text = c2036[current_element].GetHandleString();
            tb_sd8.Text = c2036[current_element].ResearchTime.ToString();
        }


        public override string get_element_string(int index)
        {
            return $"{c2036[index].ButtonID} {SFCategoryManager.GetTextByLanguage(c2036[index].ButtonNameID, 1)}";
        }

        public override string get_description_string(int index)
        {
            return $"{SFCategoryManager.GetDescriptionName(c2036[index].ButtonDescriptionID)}\r\n\r\nUpgraded in building: {SFCategoryManager.GetBuildingName(c2036[index].BuildingID)}";
        }
    }
}
