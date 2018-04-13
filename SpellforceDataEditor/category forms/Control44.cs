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
    public partial class Control44 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control44()
        {
            InitializeComponent();
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(textBox1.Text, 3);
            category.set_element_variant(current_element, 1, data_array[0]);
            category.set_element_variant(current_element, 2, data_array[1]);
            category.set_element_variant(current_element, 3, data_array[2]);
        }

        public override void show_element()
        {
            tb_effID.Text = variant_repr(0);
            textBox1.Text = bytearray_repr(1, 3);
        }
    }
}
