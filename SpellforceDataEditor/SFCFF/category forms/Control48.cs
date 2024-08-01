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

            column_dict.Add("Button ID", "ButtonID");
            column_dict.Add("Building ID", "BuildingID");
            column_dict.Add("Button name ID", "ButtonNameID");
            column_dict.Add("Button description ID", "ButtonDescriptionID");
            column_dict.Add("Wood", "Wood");
            column_dict.Add("Stone", "Stone");
            column_dict.Add("Iron", "Iron");
            column_dict.Add("Lenya", "Lenya");
            column_dict.Add("Aria", "Aria");
            column_dict.Add("Moonsilver", "Moonsilver");
            column_dict.Add("Food", "Food");
            column_dict.Add("Button handle", "Handle");
            column_dict.Add("Research time", "ResearchTime");
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
            return $"{c2036[index].ButtonID} {SFCategoryManager.GetTextByLanguage(c2036[index].ButtonNameID, SFEngine.Settings.LanguageID)}";
        }

        public override string get_description_string(int index)
        {
            return $"{SFCategoryManager.GetDescriptionName(c2036[index].ButtonDescriptionID)}\r\n\r\nUpgraded in building: {SFCategoryManager.GetBuildingName(c2036[index].BuildingID)}";
        }

        private void tb_sd6_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2029, tb_sd6.Text);
        }

        private void tb_sd2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, tb_sd2.Text);
        }

        private void tb_sd7_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2058, tb_sd7.Text);
        }
    }
}
