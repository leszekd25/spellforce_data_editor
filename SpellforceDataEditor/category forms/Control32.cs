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
    public partial class Control32 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control32()
        {
            InitializeComponent();
            column_dict.Add("Unknown", new int[3] { 0, 1, 2 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(textBox1.Text, 3);
            category.set_element_variant(current_element, 0, data_array[0]);
            category.set_element_variant(current_element, 1, data_array[1]);
            category.set_element_variant(current_element, 2, data_array[2]);
        }

        public override void show_element()
        {
            textBox1.Text = bytearray_repr(0, 3);
        }
    }
}
