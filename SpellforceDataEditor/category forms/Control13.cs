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
    public partial class Control13 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control13()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt8(textBox2.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.FixedLengthString(textBox4.Text, 64));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox3.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox2.Text = variant_repr(1);
            textBox4.Text = string_repr(2);
            textBox3.Text = variant_repr(3);
        }
    }
}
