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
    public partial class Control12 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control12()
        {
            InitializeComponent();
            column_dict.Add("Item ID", new int[1] { 0 });
            column_dict.Add("Item effect index", new int[1] { 1 });
            column_dict.Add("Effect ID", new int[1] { 2 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, Utility.TryParseUInt8(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, Utility.TryParseUInt16(textBox3.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox2.Text = variant_repr(1);
            textBox3.Text = variant_repr(2);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 6);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox3, 0);
        }
    }
}
