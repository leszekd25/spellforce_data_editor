using SFEngine.SFCFF;
using System;
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
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt32(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseUInt32(textBox1.Text));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, (Byte)(checkBox1.Checked ? 1 : 0));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, SFEngine.Utility.TryParseUInt32(textBox4.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = variant_repr(0);
            textBox1.Text = variant_repr(1);
            checkBox1.Checked = ((Byte)category[current_element][2] != 0);
            textBox2.Text = variant_repr(3);
            textBox3.Text = variant_repr(4);
            textBox4.Text = variant_repr(5);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox1, 2061);
            }
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox2, 2016);
            }
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox3, 2016);
            }
        }


        public override string get_element_string(int index)
        {
            UInt32 elem_id = (UInt32)category[index][0];
            string txt = SFCategoryManager.GetTextFromElement(category[index], 3);
            return elem_id.ToString() + " " + txt;
        }

        public override string get_description_string(int index)
        {
            UInt32 quest_id = (UInt32)category[index][1];
            SFCategoryElement quest_elem = category.FindElementBinary<UInt32>(0, quest_id);
            string quest_name = SFCategoryManager.GetTextFromElement(quest_elem, 3);
            string desc_text = SFCategoryManager.GetTextFromElement(category[index], 4);
            return desc_text + "\r\n\r\nPart of quest " + quest_name;
        }
    }
}
