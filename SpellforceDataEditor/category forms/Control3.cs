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
    public partial class Control3 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control3()
        {
            InitializeComponent();
            column_dict.Add("Unknown", new int[6] { 0, 1, 2, 3, 4, 5 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(textBox1.Text, 6);
            set_element_variant(current_element, 0, data_array[0]);
            set_element_variant(current_element, 1, data_array[1]);
            set_element_variant(current_element, 2, data_array[2]);
            set_element_variant(current_element, 3, data_array[3]);
            set_element_variant(current_element, 4, data_array[4]);
            set_element_variant(current_element, 5, data_array[5]);
        }

        public override void show_element()
        {
            textBox1.Text = bytearray_repr(0, 6);
        }
    }
}
