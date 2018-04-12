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
    public partial class Control48 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control48()
        {
            InitializeComponent();
        }

        private void tb_sd1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(tb_sd1.Text));
        }

        private void tb_sd6_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt16(tb_sd6.Text));
        }

        private void tb_sd2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.TryParseUInt16(tb_sd2.Text));
        }

        private void tb_sd7_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 3, Utility.TryParseUInt16(tb_sd7.Text));
        }

        private void tb_sd3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 4, Utility.TryParseUInt16(tb_sd3.Text));
        }

        private void tb_sd4_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 5, Utility.TryParseUInt16(tb_sd4.Text));
        }

        private void sb_sd5_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 6, Utility.TryParseUInt16(sb_sd5.Text));
        }

        private void tb_sd9_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 7, Utility.TryParseUInt16(tb_sd9.Text));
        }

        private void tb_sd10_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 8, Utility.TryParseUInt16(tb_sd10.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 9, Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 10, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 11, Utility.FixedLengthString(textBox3.Text, 64));
        }

        private void tb_sd8_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 12, Utility.TryParseUInt32(tb_sd8.Text));
        }
    }
}
