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
    public partial class Control33 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control33()
        {
            InitializeComponent();
            column_dict.Add("Level", new int[1] { 0 });
            column_dict.Add("Health factor", new int[1] { 1 });
            column_dict.Add("Mana factor", new int[1] { 2 });
            column_dict.Add("Experience required", new int[1] { 3 });
            column_dict.Add("Attribute point limit", new int[1] { 4 });
            column_dict.Add("Skill point limit", new int[1] { 5 });
            column_dict.Add("Damage factor", new int[1] { 6 });
            column_dict.Add("Armor class factor", new int[1] { 7 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt8(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.TryParseUInt16(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 3, Utility.TryParseUInt32(textBox5.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 4, Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 5, Utility.TryParseUInt8(textBox9.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 6, Utility.TryParseUInt16(textBox6.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 7, Utility.TryParseUInt16(textBox8.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox2.Text = variant_repr(1);
            textBox4.Text = variant_repr(2);
            textBox5.Text = variant_repr(3);
            textBox3.Text = variant_repr(4);
            textBox9.Text = variant_repr(5);
            textBox6.Text = variant_repr(6);
            textBox8.Text = variant_repr(7);
        }
    }
}
