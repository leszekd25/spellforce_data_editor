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
            column_dict.Add("Item ID", new int[1] { 0 });
            column_dict.Add("Item type 1", new int[1] { 1 });
            column_dict.Add("Item type 2", new int[1] { 2 });
            column_dict.Add("Item name ID", new int[1] { 3 });
            column_dict.Add("Unit stats ID", new int[1] { 4 });
            column_dict.Add("Army unit ID", new int[1] { 5 });
            column_dict.Add("Building ID", new int[1] { 6 });
            column_dict.Add("Unknown", new int[1] { 7 });
            column_dict.Add("Selling price", new int[1] { 8 });
            column_dict.Add("Buying price", new int[1] { 9 });
            column_dict.Add("Item set ID", new int[1] { 10 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, Utility.TryParseUInt8(textBox10.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, Utility.TryParseUInt8(textBox11.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, Utility.TryParseUInt16(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 6, Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 7, Utility.TryParseUInt8(textBox8.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 8, Utility.TryParseUInt32(textBox6.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 9, Utility.TryParseUInt32(textBox7.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 10, Utility.TryParseUInt8(textBox9.Text));
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

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox2, 14);
        }

        private void textBox9_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox9, 48);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox3, 3);
        }

        private void textBox4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox4, 17);
        }

        private void textBox5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox5, 23);
        }
    }
}
