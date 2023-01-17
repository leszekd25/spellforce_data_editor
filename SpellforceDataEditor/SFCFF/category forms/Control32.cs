using SFEngine.SFCFF;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control32 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control32()
        {
            InitializeComponent();
            column_dict.Add("Resource ID", new int[1] { 0 });
            column_dict.Add("Text ID", new int[1] { 1 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt8(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox2.Text = variant_repr(1);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox2, 2016);
            }
        }

        public override string get_element_string(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(category[index], 1);
            return category[index][0].ToString() + " " + txt;
        }
    }
}
