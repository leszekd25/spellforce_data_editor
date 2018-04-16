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
    public partial class Control34 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control34()
        {
            InitializeComponent();
            column_dict.Add("Object ID", new int[1] { 0 });
            column_dict.Add("Name ID", new int[1] { 1 });
            column_dict.Add("Unknown", new int[3] { 2, 3, 4 });
            column_dict.Add("Object handle", new int[1] { 5 });
            column_dict.Add("Unknown2", new int[7] { 6, 7, 8, 9, 10, 11, 12 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(textBox6.Text, 3);
            category.set_element_variant(current_element, 2, data_array[0]);
            category.set_element_variant(current_element, 3, data_array[1]);
            category.set_element_variant(current_element, 4, data_array[2]);
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 5, Utility.FixedLengthString(textBox7.Text, 40));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(textBox2.Text, 7);
            category.set_element_variant(current_element, 6, data_array[0]);
            category.set_element_variant(current_element, 7, data_array[1]);
            category.set_element_variant(current_element, 8, data_array[2]);
            category.set_element_variant(current_element, 9, data_array[3]);
            category.set_element_variant(current_element, 10, data_array[4]);
            category.set_element_variant(current_element, 11, data_array[5]);
            category.set_element_variant(current_element, 12, data_array[6]);
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox3.Text = variant_repr(1);
            textBox6.Text = bytearray_repr(2, 3);
            textBox7.Text = string_repr(5);
            textBox2.Text = bytearray_repr(6, 7);
        }
    }
}
