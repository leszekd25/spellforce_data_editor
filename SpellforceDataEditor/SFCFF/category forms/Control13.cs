using System;
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
    public partial class Control13 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control13()
        {
            InitializeComponent();
            column_dict.Add("Item ID", new int[1] { 0 });
            column_dict.Add("Item UI index", new int[1] { 1 });
            column_dict.Add("Item UI handle", new int[1] { 2 });
            column_dict.Add("Scaled down?", new int[1] { 3 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, Utility.TryParseUInt8(textBox2.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, Utility.FixedLengthString(textBox4.Text, 64));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, (checkBox1.Checked?(UInt16)1:(UInt16)0));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox2.Text = variant_repr(1);
            textBox4.Text = string_repr(2);
            checkBox1.Checked = ((UInt16)category.get_element_variant(current_element, 3).value == 1 ? true : false);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 6);
        }
    }
}
