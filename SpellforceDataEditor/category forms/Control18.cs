﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.category_forms
{
    public partial class Control18 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control18()
        {
            InitializeComponent();
            column_dict.Add("Unit ID", new int[1] { 0 });
            column_dict.Add("Unit name ID", new int[1] { 1 });
            column_dict.Add("Unit stats ID", new int[1] { 2 });
            column_dict.Add("Experience gain", new int[1] { 3 });
            column_dict.Add("Experience falloff", new int[1] { 4 });
            column_dict.Add("Health factor", new int[1] { 5 });
            column_dict.Add("Unknown 1", new int[1] { 6 });
            column_dict.Add("Unknown 2", new int[1] { 7 });
            column_dict.Add("Unknown 3", new int[1] { 8 });
            column_dict.Add("Armor", new int[1] { 9 });
            column_dict.Add("Unit handle", new int[1] { 10 });
            column_dict.Add("Unknown2", new int[1] { 11 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 3, Utility.TryParseUInt32(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 4, Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 5, Utility.TryParseUInt32(textBox6.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 6, Utility.TryParseUInt16(textBox7.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 7, Utility.TryParseUInt8(textBox8.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 8, Utility.TryParseUInt16(textBox9.Text));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 9, Utility.TryParseUInt16(textBox12.Text));
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 10, Utility.FixedLengthString(textBox13.Text, 40));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 11, Utility.TryParseUInt8(textBox10.Text));
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
        }
    }
}
