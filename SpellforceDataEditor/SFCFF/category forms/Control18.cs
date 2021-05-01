﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control18 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control18()
        {
            InitializeComponent();
            column_dict.Add("Unit ID", new int[1] { 0 });
            column_dict.Add("Unit name ID", new int[1] { 1 });
            column_dict.Add("Unit stats ID", new int[1] { 2 });
            column_dict.Add("Experience gain", new int[1] { 3 });
            column_dict.Add("Experience falloff", new int[1] { 4 });
            column_dict.Add("Money in copper", new int[1] { 5 });
            column_dict.Add("Gold variance", new int[1] { 6 });
            column_dict.Add("Unknown1 1", new int[1] { 7 });
            column_dict.Add("Unknown1 2", new int[1] { 8 });
            column_dict.Add("Armor", new int[1] { 9 });
            column_dict.Add("Unit handle", new int[1] { 10 });
            column_dict.Add("Placeable in map editor", new int[1] { 11 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, Utility.TryParseUInt32(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, Utility.TryParseUInt32(textBox6.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 6, Utility.TryParseUInt16(textBox7.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 7, Utility.TryParseUInt8(textBox8.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 8, Utility.TryParseUInt16(textBox9.Text));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 9, Utility.TryParseUInt16(textBox12.Text));
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 10, Utility.FixedLengthString(textBox13.Text, 40));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 11, Utility.TryParseUInt8(textBox10.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox2.Text = variant_repr(1);
            textBox3.Text = variant_repr(2);
            textBox4.Text = variant_repr(3);
            textBox5.Text = variant_repr(4);
            textBox6.Text = variant_repr(5);
            textBox7.Text = variant_repr(6);
            textBox8.Text = variant_repr(7);
            textBox9.Text = variant_repr(8);
            textBox12.Text = variant_repr(9);
            textBox13.Text = string_repr(10);
            textBox10.Text = variant_repr(11);

            button_repr(ButtonGoto19, 2025, "Equipment", "Unit");
            button_repr(ButtonGoto20, 2026, "Spells", "Unit");
            button_repr(ButtonGoto21, 2028, "Resource requirements", "Unit");
            button_repr(ButtonGoto22, 2040, "Loot", "Unit");
            button_repr(ButtonGoto23, 2001, "Upgrade data", "Unit");
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox3, 2005);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox2, 2016);
        }

        private void button_Trace_Click(object sender, EventArgs e)
        {
            step_into(textBox1, 2026);
        }

        private void ButtonGoto19_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto19, 2025);
            button_repr(ButtonGoto19, 2025, "Equipment", "Unit");
        }

        private void ButtonGoto20_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto20, 2026);
            button_repr(ButtonGoto20, 2026, "Spells", "Unit");
        }

        private void ButtonGoto21_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto21, 2028);
            button_repr(ButtonGoto21, 2028, "Resource requirements", "Unit");
        }

        private void ButtonGoto22_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto22, 2040);
            button_repr(ButtonGoto22, 2040, "Loot", "Unit");
        }

        private void ButtonGoto23_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto23, 2001);
            button_repr(ButtonGoto23, 2001, "Upgrade data", "Unit");
        }

        private int calculate_total_xp(UInt32 xp_gain, UInt16 xp_falloff)
        {
            if ((xp_gain == 0) || (xp_falloff == 0))
                return 0;
            int max_units = 500;
            int s = 0;
            for (int i = 0; i < max_units; i++)
            {
                s += (int)Math.Floor((Single)xp_gain * ((Single)(xp_falloff) / (Single)(xp_falloff + i)));
            }
            return s;
        }

        public override string get_element_string(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(category[index], 1);
            return category[index][0].ToString() + " " + txt;
        }

        public override string get_description_string(int index)
        {
            UInt32 xp_gain = (UInt32)category[index][3];
            UInt16 xp_falloff = (UInt16)category[index][4];
            return "Max XP gained from this unit: " + calculate_total_xp(xp_gain, xp_falloff).ToString();
        }
    }
}
