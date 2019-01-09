using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control43 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control43()
        {
            InitializeComponent();
            column_dict.Add("Quest ID", new int[1] { 0 });
            column_dict.Add("Parent quest ID", new int[1] { 1 });
            column_dict.Add("Unknown", new int[1] { 2 });
            column_dict.Add("Quest name ID", new int[1] { 3 });
            column_dict.Add("Quest description ID", new int[1] { 4 });
            column_dict.Add("Quest order index", new int[1] { 5 });
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, Utility.TryParseUInt32(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, Utility.TryParseUInt32(textBox1.Text));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, (Byte)(checkBox1.Checked ? 1 : 0));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, Utility.TryParseUInt32(textBox4.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = variant_repr(0);
            textBox1.Text = variant_repr(1);
            checkBox1.Checked = ((Byte)category.get_element_variant(current_element, 2).value != 0);
            textBox2.Text = variant_repr(3);
            textBox3.Text = variant_repr(4);
            textBox4.Text = variant_repr(5);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 42);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox2, 14);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox3, 14);
        }
    }
}
