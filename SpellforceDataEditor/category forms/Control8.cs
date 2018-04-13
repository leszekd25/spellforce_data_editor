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
    public partial class Control8 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control8()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseInt16(textBox4.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.TryParseInt16(textBox6.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 3, Utility.TryParseInt16(textBox8.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 4, Utility.TryParseInt16(textBox10.Text));
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 5, Utility.TryParseInt16(textBox18.Text));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 6, Utility.TryParseInt16(textBox12.Text));
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 7, Utility.TryParseInt16(textBox14.Text));
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 8, Utility.TryParseInt16(textBox16.Text));
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 9, Utility.TryParseInt16(textBox17.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 10, Utility.TryParseInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 11, Utility.TryParseInt16(textBox3.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 12, Utility.TryParseInt16(textBox5.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 13, Utility.TryParseInt16(textBox7.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 14, Utility.TryParseInt16(textBox9.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 15, Utility.TryParseInt16(textBox11.Text));
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 16, Utility.TryParseInt16(textBox13.Text));
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 17, Utility.TryParseInt16(textBox15.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox4.Text = variant_repr(1);
            textBox6.Text = variant_repr(2);
            textBox8.Text = variant_repr(3);
            textBox10.Text = variant_repr(4);
            textBox18.Text = variant_repr(5);
            textBox12.Text = variant_repr(6);
            textBox14.Text = variant_repr(7);
            textBox16.Text = variant_repr(8);
            textBox17.Text = variant_repr(9);
            textBox2.Text = variant_repr(10);
            textBox3.Text = variant_repr(11);
            textBox5.Text = variant_repr(12);
            textBox7.Text = variant_repr(13);
            textBox9.Text = variant_repr(14);
            textBox11.Text = variant_repr(15);
            textBox13.Text = variant_repr(16);
            textBox15.Text = variant_repr(17);
        }
    }
}
