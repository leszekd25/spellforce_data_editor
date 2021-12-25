using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFEngine.SFCFF;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control48 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control48()
        {
            InitializeComponent();
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
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt16(tb_sd1.Text));
        }

        private void tb_sd6_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseUInt16(tb_sd6.Text));
        }

        private void tb_sd2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, SFEngine.Utility.TryParseUInt16(tb_sd2.Text));
        }

        private void tb_sd7_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, SFEngine.Utility.TryParseUInt16(tb_sd7.Text));
        }

        private void tb_sd3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, SFEngine.Utility.TryParseUInt16(tb_sd3.Text));
        }

        private void tb_sd4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, SFEngine.Utility.TryParseUInt16(tb_sd4.Text));
        }

        private void sb_sd5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 6, SFEngine.Utility.TryParseUInt16(sb_sd5.Text));
        }

        private void tb_sd9_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 7, SFEngine.Utility.TryParseUInt16(tb_sd9.Text));
        }

        private void tb_sd10_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 8, SFEngine.Utility.TryParseUInt16(tb_sd10.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 9, SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 10, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 11, SFString.FromString(textBox3.Text, 0, 64));// SFEngine.Utility.FixedLengthString(textBox3.Text, 64));
        }

        private void tb_sd8_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 12, SFEngine.Utility.TryParseUInt32(tb_sd8.Text));
        }

        public override void show_element()
        {
            tb_sd1.Text = variant_repr(0);
            tb_sd6.Text = variant_repr(1);
            tb_sd2.Text = variant_repr(2);
            tb_sd7.Text = variant_repr(3);
            tb_sd3.Text = variant_repr(4);
            tb_sd4.Text = variant_repr(5);
            sb_sd5.Text = variant_repr(6);
            tb_sd9.Text = variant_repr(7);
            tb_sd10.Text = variant_repr(8);
            textBox2.Text = variant_repr(9);
            textBox1.Text = variant_repr(10);
            textBox3.Text = string_repr(11);
            tb_sd8.Text = variant_repr(12);
        }

        private void tb_sd6_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(tb_sd6, 2029);
        }

        private void tb_sd2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(tb_sd2, 2016);
        }

        private void tb_sd7_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(tb_sd7, 2058);
        }


        public override string get_element_string(int index)
        {
            UInt16 elem_id = (UInt16)category[index][0];
            string txt = SFCategoryManager.GetTextFromElement(category[index], 2);
            return elem_id.ToString() + " " + txt;
        }

        public override string get_description_string(int index)
        {
            UInt16 elem_id = (UInt16)category[index][1];
            string building_name = SFCategoryManager.GetBuildingName(elem_id);
            UInt16 desc_id = (UInt16)category[index][3];
            string desc_name = SFCategoryManager.GetDescriptionName(desc_id);
            return desc_name + "\r\n\r\nUpgraded in building: " + building_name;
        }
    }
}
