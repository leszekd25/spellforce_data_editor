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
    public partial class Control43 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control43()
        {
            InitializeComponent();
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(textBox1.Text, 15);
            category.set_element_variant(current_element, 1, data_array[0]);
            category.set_element_variant(current_element, 2, data_array[1]);
            category.set_element_variant(current_element, 3, data_array[2]);
            category.set_element_variant(current_element, 4, data_array[3]);
            category.set_element_variant(current_element, 5, data_array[4]);
            category.set_element_variant(current_element, 6, data_array[5]);
            category.set_element_variant(current_element, 7, data_array[6]);
            category.set_element_variant(current_element, 8, data_array[7]);
            category.set_element_variant(current_element, 9, data_array[8]);
            category.set_element_variant(current_element, 10, data_array[9]);
            category.set_element_variant(current_element, 11, data_array[10]);
            category.set_element_variant(current_element, 12, data_array[11]);
            category.set_element_variant(current_element, 13, data_array[12]);
            category.set_element_variant(current_element, 14, data_array[13]);
            category.set_element_variant(current_element, 15, data_array[14]);
        }
    }
}
