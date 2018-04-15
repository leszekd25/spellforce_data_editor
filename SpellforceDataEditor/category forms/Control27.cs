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
    public partial class Control27 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control27()
        {
            InitializeComponent();
            column_dict.Add("Skill major type", new int[1] { 0 });
            column_dict.Add("Skill minor type", new int[1] { 1 });
            column_dict.Add("Text ID", new int[1] { 2 });
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt8(textBox2.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.TryParseUInt16(textBox1.Text));
        }

        public override void show_element()
        {
            textBox3.Text = variant_repr(0);
            textBox2.Text = variant_repr(1);
            textBox1.Text = variant_repr(2);
        }
    }
}
