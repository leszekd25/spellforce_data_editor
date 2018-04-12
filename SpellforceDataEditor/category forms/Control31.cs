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
    public partial class Control31 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control31()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(textBox1.Text, 5);
            category.set_element_variant(current_element, 0, data_array[0]);
            category.set_element_variant(current_element, 1, data_array[1]);
            category.set_element_variant(current_element, 2, data_array[2]);
            category.set_element_variant(current_element, 3, data_array[3]);
            category.set_element_variant(current_element, 4, data_array[4]);
        }
    }
}
