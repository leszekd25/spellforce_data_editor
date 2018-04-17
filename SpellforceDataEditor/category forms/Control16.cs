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
    public partial class Control16 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control16()
        {
            InitializeComponent();
            column_dict.Add("Race ID", new int[1] { 0 });
            column_dict.Add("Range 1", new int[1] { 1 });
            column_dict.Add("Range 2", new int[1] { 2 });
            column_dict.Add("Range 3", new int[1] { 3 });
            column_dict.Add("Percentage 1", new int[1] { 4 });
            column_dict.Add("Percentage 2", new int[1] { 5 });
            column_dict.Add("Percentage 3", new int[1] { 6 });
            column_dict.Add("Race text ID", new int[1] { 7 });
            column_dict.Add("Unknown2", new int[7] { 8, 9, 10, 11 ,12, 13, 14 });
            column_dict.Add("Lua 1", new int[1] { 15 });
            column_dict.Add("Lua 2", new int[1] { 16 });
            column_dict.Add("Lua 3", new int[1] { 17 });
            column_dict.Add("Unknown3", new int[8] { 18, 19, 20, 21, 22, 23, 24, 25 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt8(textBox1.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 3, Utility.TryParseUInt8(textBox2.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 4, Utility.TryParseUInt8(textBox7.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 5, Utility.TryParseUInt8(textBox6.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 6, Utility.TryParseUInt8(textBox5.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 7, Utility.TryParseUInt16(textBox9.Text));
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(textBox15.Text, 7);
            category.set_element_variant(current_element, 8, data_array[0]);
            category.set_element_variant(current_element, 9, data_array[1]);
            category.set_element_variant(current_element, 10, data_array[2]);
            category.set_element_variant(current_element, 11, data_array[3]);
            category.set_element_variant(current_element, 12, data_array[4]);
            category.set_element_variant(current_element, 13, data_array[5]);
            category.set_element_variant(current_element, 14, data_array[6]);
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 15, Utility.TryParseUInt8(textBox19.Text));
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 16, Utility.TryParseUInt8(textBox18.Text));
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 17, Utility.TryParseUInt8(textBox17.Text));
        }

        private void textBox26_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(textBox26.Text, 8);
            category.set_element_variant(current_element, 18, data_array[0]);
            category.set_element_variant(current_element, 19, data_array[1]);
            category.set_element_variant(current_element, 20, data_array[2]);
            category.set_element_variant(current_element, 21, data_array[3]);
            category.set_element_variant(current_element, 22, data_array[4]);
            category.set_element_variant(current_element, 23, data_array[5]);
            category.set_element_variant(current_element, 24, data_array[6]);
            category.set_element_variant(current_element, 25, data_array[7]);
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox4.Text = variant_repr(1);
            textBox3.Text = variant_repr(2);
            textBox2.Text = variant_repr(3);
            textBox7.Text = variant_repr(4);
            textBox6.Text = variant_repr(5);
            textBox5.Text = variant_repr(6);
            textBox9.Text = variant_repr(7);
            textBox15.Text = bytearray_repr(8, 7);
            textBox19.Text = variant_repr(15);
            textBox18.Text = variant_repr(16);
            textBox17.Text = variant_repr(17);
            textBox26.Text = bytearray_repr(18, 8);
        }
    }
}
