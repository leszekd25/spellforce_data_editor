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
    public partial class Control36 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control36()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt8(textBox6.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 3, Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 4, Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 5, Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 6, Utility.TryParseUInt16(textBox7.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox6.Text = variant_repr(1);
            textBox2.Text = variant_repr(2);
            textBox3.Text = variant_repr(3);
            textBox5.Text = variant_repr(4);
            textBox4.Text = variant_repr(5);
            textBox7.Text = variant_repr(6);
        }
    }
}
