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
    public partial class Control37 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control37()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(textBox6.Text, 4);
            category.set_element_variant(current_element, 1, data_array[0]);
            category.set_element_variant(current_element, 2, data_array[1]);
            category.set_element_variant(current_element, 3, data_array[2]);
            category.set_element_variant(current_element, 4, data_array[3]);
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox6.Text = bytearray_repr(1, 4);
        }
    }
}
