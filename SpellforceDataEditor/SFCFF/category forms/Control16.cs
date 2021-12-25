﻿using System;
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
    public partial class Control16 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        static public string[] race_flags = new string[] { "Undead", "Breathing", "Huntable", "Animal", "Has soul", "Attacks buildings", "Bleeds", "(not used)" };
        static public string[] race_ai_flags = new string[] { "Default", "Idle", "Stroll along", "Nomadic", "Aggressive", "Defensive", "Script", "(not used)" };



        public Control16()
        {
            InitializeComponent();
            column_dict.Add("Race ID", new int[1] { 0 });
            column_dict.Add("Range 1", new int[1] { 1 });
            column_dict.Add("Range 2", new int[1] { 2 });
            column_dict.Add("Range 3", new int[1] { 3 });
            column_dict.Add("Percentage 1", new int[1] { 4 });
            column_dict.Add("Percentage 2", new int[1] { 5 });
            column_dict.Add("Percentage 3", new int[1] { 6 });
            column_dict.Add("Race text ID", new int[1] { 7 });
            column_dict.Add("Race flags", new int[1] { 8 });
            column_dict.Add("Clan ID", new int[1] { 9 });
            column_dict.Add("Damage taken (melee)", new int[1] { 10 });
            column_dict.Add("Damage taken (ranged)", new int[1] { 11 });
            column_dict.Add("Unknown2", new int[1] { 12 });
            column_dict.Add("Lua 1", new int[1] { 13 });
            column_dict.Add("Lua 2", new int[1] { 14 });
            column_dict.Add("Lua 3", new int[1] { 15 });
            column_dict.Add("Unknown3 1", new int[1] { 16 });
            column_dict.Add("Unknown3 2", new int[1] { 17 });
            column_dict.Add("Retreat chance 1", new int[1] { 18 });
            column_dict.Add("Retreat chance 2", new int[1] { 19 });
            column_dict.Add("Attack time factor", new int[1] { 20 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt8(textBox1.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, SFEngine.Utility.TryParseUInt8(textBox2.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, SFEngine.Utility.TryParseUInt8(textBox7.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, SFEngine.Utility.TryParseUInt8(textBox6.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 6, SFEngine.Utility.TryParseUInt8(textBox5.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 7, SFEngine.Utility.TryParseUInt16(textBox9.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 8, SFEngine.Utility.TryParseUInt8(textBox11.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 9, SFEngine.Utility.TryParseUInt16(textBox10.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 10, SFEngine.Utility.TryParseUInt8(textBox8.Text));
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 11, SFEngine.Utility.TryParseUInt8(textBox13.Text));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 12, SFEngine.Utility.TryParseUInt16(textBox12.Text));
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 13, SFEngine.Utility.TryParseUInt8(textBox19.Text));
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 14, SFEngine.Utility.TryParseUInt8(textBox18.Text));
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 15, SFEngine.Utility.TryParseUInt8(textBox17.Text));
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 16, SFEngine.Utility.TryParseUInt8(textBox22.Text));
        }

        private void textBox21_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 17, SFEngine.Utility.TryParseUInt16(textBox21.Text));
        }

        private void textBox20_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 18, SFEngine.Utility.TryParseUInt16(textBox20.Text));
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 19, SFEngine.Utility.TryParseUInt16(textBox16.Text));
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 20, SFEngine.Utility.TryParseUInt8(textBox14.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox4.Text = variant_repr(1);
            textBox3.Text = variant_repr(2);
            textBox2.Text = variant_repr(3);
            textBox7.Text = variant_repr(4);
            textBox6.Text = variant_repr(5);
            textBox5.Text = variant_repr(6);
            textBox9.Text = variant_repr(7);
            textBox11.Text = variant_repr(8);
            textBox10.Text = variant_repr(9);
            textBox8.Text = variant_repr(10);
            textBox13.Text = variant_repr(11);
            textBox12.Text = variant_repr(12);
            textBox19.Text = variant_repr(13);
            textBox18.Text = variant_repr(14);
            textBox17.Text = variant_repr(15);
            textBox22.Text = variant_repr(16);
            textBox21.Text = variant_repr(17);
            textBox20.Text = variant_repr(18);
            textBox16.Text = variant_repr(19);
            textBox14.Text = variant_repr(20);
        }

        private void textBox9_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox9, 2016);
        }

        private void textBox10_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox10, 2023);
        }



        public override string get_element_string(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(category[index], 7);
            return category[index][0].ToString() + " " + txt;
        }

        public override string get_description_string(int index)
        {
            Byte flags = (Byte)category[index][8];
            UInt16 ai_flags = (UInt16)category[index][12];

            string flag_text = "Race flags: ";
            bool first_flag_set = false;
            for (int i = 0; i < 8; i++)
            {
                if (((flags >> i) & 0x1) == 0x1)
                {
                    if (first_flag_set)
                        flag_text += " | ";
                    flag_text += race_flags[i];
                    first_flag_set = true;
                }
            }

            string ai_flag_text = "\r\nAI flags: ";
            first_flag_set = false;
            for (int i = 0; i < 8; i++)
            {
                if (((ai_flags >> i) & 0x1) == 0x1)
                {
                    if (first_flag_set)
                        ai_flag_text += " | ";
                    ai_flag_text += race_ai_flags[i];
                    first_flag_set = true;
                }
            }

            return flag_text + ai_flag_text;
        }
    }
}
