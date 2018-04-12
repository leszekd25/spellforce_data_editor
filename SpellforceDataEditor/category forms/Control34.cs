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
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt8(textBox1.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(textBox6.Text, 5);
            category.set_element_variant(current_element, 1, data_array[0]);
            category.set_element_variant(current_element, 2, data_array[1]);
            category.set_element_variant(current_element, 3, data_array[2]);
            category.set_element_variant(current_element, 4, data_array[3]);
            category.set_element_variant(current_element, 5, data_array[4]);
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 6, Utility.FixedLengthString(textBox7.Text, 47));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(textBox6.Text, 7);
            category.set_element_variant(current_element, 7, data_array[0]);
            category.set_element_variant(current_element, 8, data_array[1]);
            category.set_element_variant(current_element, 9, data_array[2]);
            category.set_element_variant(current_element, 10, data_array[3]);
            category.set_element_variant(current_element, 11, data_array[4]);
            category.set_element_variant(current_element, 12, data_array[5]);
            category.set_element_variant(current_element, 13, data_array[6]);
        }
    }
}
