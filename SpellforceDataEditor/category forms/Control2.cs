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
    public partial class Control2 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control2()
        {
            InitializeComponent();
            column_dict.Add("Spell type ID", new int[1] { 0 });
            column_dict.Add("Spell text ID", new int[1] { 1 });
            column_dict.Add("Unknown1 1", new int[1] { 2 });
            column_dict.Add("Unknown1 2", new int[1] { 3 });
            column_dict.Add("Sorting 1", new int[1] { 4 });
            column_dict.Add("Sorting 2", new int[1] { 5 });
            column_dict.Add("Sorting 3", new int[1] { 6 });
            column_dict.Add("Spell UI handle", new int[1] { 7 });
            column_dict.Add("Description ID", new int[1] { 8 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 3, Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 4, Utility.TryParseUInt8(textBox5.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 5, Utility.TryParseUInt8(textBox7.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 6, Utility.TryParseUInt8(textBox6.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 7, Utility.FixedLengthString(textBox8.Text, 64));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 8, Utility.TryParseUInt16(textBox9.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox2.Text = variant_repr(1);
            textBox3.Text = variant_repr(2);
            textBox4.Text = variant_repr(3);
            textBox5.Text = variant_repr(4);
            textBox7.Text = variant_repr(5);
            textBox6.Text = variant_repr(6);
            textBox8.Text = string_repr(7);
            textBox9.Text = variant_repr(8);
        }
    }
}
