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
    public partial class Control38 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control38()
        {
            InitializeComponent();
            column_dict.Add("Map ID", new int[1] { 0 });
            column_dict.Add("Unknown", new int[1] { 1 });
            column_dict.Add("Map handle", new int[1] { 2 });
            column_dict.Add("Name ID", new int[1] { 3 });
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt32(tb_effID.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.FixedLengthString(textBox4.Text, 64));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 3, Utility.TryParseUInt16(textBox2.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = variant_repr(0);
            textBox3.Text = variant_repr(1);
            textBox4.Text = string_repr(2);
            textBox2.Text = variant_repr(3);
        }
    }
}
