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
    public partial class Control4 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control4()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt16(textBox6.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.TryParseUInt8(textBox2.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 3, Utility.TryParseUInt16(textBox7.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 4, Utility.TryParseUInt16(textBox9.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 5, Utility.TryParseUInt16(textBox8.Text));
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 6, Utility.TryParseUInt16(textBox17.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 7, Utility.TryParseUInt16(textBox4.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 8, Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 9, Utility.TryParseUInt16(textBox18.Text));
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 10, Utility.TryParseUInt8(textBox22.Text));
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 11, Utility.TryParseUInt16(textBox19.Text));
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 12, Utility.TryParseUInt16(textBox16.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 13, Utility.TryParseUInt16(textBox10.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 14, Utility.TryParseUInt16(textBox11.Text));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 15, Utility.TryParseUInt16(textBox12.Text));
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 16, Utility.TryParseUInt16(textBox13.Text));
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 17, Utility.TryParseUInt16(textBox14.Text));
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 18, Utility.TryParseUInt16(textBox15.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 19, Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox25_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 20, Utility.TryParseUInt8(textBox25.Text));
        }

        private void textBox23_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 21, Utility.TryParseUInt8(textBox23.Text));
        }

        private void textBox27_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 22, Utility.TryParseUInt32(textBox27.Text));
        }

        private void textBox26_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 23, Utility.TryParseUInt8(textBox26.Text));
        }

        private void textBox21_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 24, Utility.TryParseUInt16(textBox21.Text));
        }

        private void textBox24_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 25, Utility.TryParseUInt8(textBox24.Text));
        }
    }
}
