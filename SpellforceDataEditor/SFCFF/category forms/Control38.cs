using SFEngine.SFCFF;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control38 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control38()
        {
            InitializeComponent();
            column_dict.Add("Map ID", new int[1] { 0 });
            column_dict.Add("Unknown", new int[1] { 1 });
            column_dict.Add("Map handle", new int[1] { 2 });
            column_dict.Add("Name ID", new int[1] { 3 });
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt32(tb_effID.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, SFString.FromString(textBox4.Text, 0, 64));// SFEngine.Utility.FixedLengthString(textBox4.Text, 64));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = variant_repr(0);
            textBox3.Text = variant_repr(1);
            textBox4.Text = string_repr(2);
            textBox2.Text = variant_repr(3);
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
            UInt32 map_id = (UInt32)category[index][0];
            string txt = SFCategoryManager.GetTextFromElement(category[index], 3);
            return map_id.ToString() + " " + txt;
        }
    }
}
