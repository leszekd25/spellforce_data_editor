using SFEngine.SFCFF;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control37 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control37()
        {
            InitializeComponent();
            column_dict.Add("NPC ID", new int[1] { 0 });
            column_dict.Add("Name ID", new int[1] { 1 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt32(textBox1.Text));
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
            UInt32 object_id = (UInt32)category[index][0];
            string txt = SFCategoryManager.GetTextFromElement(category[index], 1);
            return object_id.ToString() + " " + txt;
        }
    }
}
