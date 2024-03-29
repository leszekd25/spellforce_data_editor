﻿using SFEngine.SFCFF;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control24 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control24()
        {
            InitializeComponent();
            column_dict.Add("Building ID", new int[1] { 0 });
            column_dict.Add("Race ID", new int[1] { 1 });
            column_dict.Add("Can enter", new int[1] { 2 });
            column_dict.Add("Slots", new int[1] { 3 });
            column_dict.Add("Health", new int[1] { 4 });
            column_dict.Add("Name ID", new int[1] { 5 });
            column_dict.Add("Center of rotation X", new int[1] { 6 });
            column_dict.Add("Center of rotation Y", new int[1] { 7 });
            column_dict.Add("Collision polygons", new int[1] { 8 });
            column_dict.Add("Worker cycle time", new int[1] { 9 });
            column_dict.Add("Required building ID", new int[1] { 10 });
            column_dict.Add("Initial angle", new int[1] { 11 });
            column_dict.Add("Extended description ID", new int[1] { 12 });
            column_dict.Add("Flags", new int[1] { 13 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseUInt8(textBox2.Text));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, (Byte)(checkBox1.Checked ? 1 : 0));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, SFEngine.Utility.TryParseUInt16(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, SFEngine.Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 6, SFEngine.Utility.TryParseInt16(textBox6.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 7, SFEngine.Utility.TryParseInt16(textBox7.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 8, SFEngine.Utility.TryParseUInt8(textBox9.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 9, SFEngine.Utility.TryParseUInt16(textBox10.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 10, SFEngine.Utility.TryParseUInt16(textBox11.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 11, SFEngine.Utility.TryParseUInt16(textBox8.Text));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 12, SFEngine.Utility.TryParseUInt16(textBox12.Text));
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 13, SFEngine.Utility.TryParseUInt8(textBox13.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox2.Text = variant_repr(1);
            checkBox1.Checked = (variant_repr(2) != "0");
            textBox3.Text = variant_repr(3);
            textBox4.Text = variant_repr(4);
            textBox5.Text = variant_repr(5);
            textBox6.Text = variant_repr(6);
            textBox7.Text = variant_repr(7);
            textBox9.Text = variant_repr(8);
            textBox10.Text = variant_repr(9);
            textBox11.Text = variant_repr(10);
            textBox8.Text = variant_repr(11);
            textBox12.Text = variant_repr(12);
            textBox13.Text = variant_repr(13);

            button_repr(ButtonGoto25, 2030, "Collision data", "Building");
            button_repr(ButtonGoto26, 2031, "Requirements", "Building");
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox2, 2022);
            }
        }

        private void textBox5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox5, 2016);
            }
        }

        private void textBox11_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox11, 2029);
            }
        }

        private void textBox12_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox12, 2059);
            }
        }

        private void ButtonGoto25_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto25, 2030);
            button_repr(ButtonGoto25, 2030, "Collision data", "Building");
        }

        private void ButtonGoto26_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto26, 2031);
            button_repr(ButtonGoto26, 2031, "Requirements", "Building");
        }


        public override string get_element_string(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(category[index], 5);
            return category[index][0].ToString() + " " + txt;
        }

        public override string get_description_string(int index)
        {
            Byte race_id = (Byte)category[index][1];
            string race_name = SFCategoryManager.GetRaceName(race_id);
            UInt16 other_building_id = (UInt16)category[index][10];
            string building_name = SFCategoryManager.GetBuildingName(other_building_id);
            return "Race: " + race_name + "\r\nRequires " + building_name;
        }
    }
}
