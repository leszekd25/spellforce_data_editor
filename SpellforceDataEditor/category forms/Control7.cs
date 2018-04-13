using System;
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
    public partial class Control7 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control7()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt8(textBox10.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.TryParseUInt8(textBox11.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 3, Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 4, Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 5, Utility.TryParseUInt16(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 6, Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 7, Utility.TryParseUInt8(textBox8.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 8, Utility.TryParseUInt32(textBox6.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 9, Utility.TryParseUInt32(textBox7.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 10, Utility.TryParseUInt8(textBox9.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox10.Text = variant_repr(1);
            textBox11.Text = variant_repr(2);
            textBox2.Text = variant_repr(3);
            textBox3.Text = variant_repr(4);
            textBox4.Text = variant_repr(5);
            textBox5.Text = variant_repr(6);
            textBox8.Text = variant_repr(7);
            textBox6.Text = variant_repr(8);
            textBox7.Text = variant_repr(9);
            textBox9.Text = variant_repr(10);
        }
    }
}
