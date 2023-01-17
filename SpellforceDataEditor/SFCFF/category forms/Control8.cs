using SFEngine.SFCFF;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control8 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control8()
        {
            InitializeComponent();
            column_dict.Add("Item ID", new int[1] { 0 });
            column_dict.Add("Strength", new int[1] { 1 });
            column_dict.Add("Stamina", new int[1] { 2 });
            column_dict.Add("Agility", new int[1] { 3 });
            column_dict.Add("Dexterity", new int[1] { 4 });
            column_dict.Add("Health", new int[1] { 5 });
            column_dict.Add("Charisma", new int[1] { 6 });
            column_dict.Add("Intelligence", new int[1] { 7 });
            column_dict.Add("Wisdom", new int[1] { 8 });
            column_dict.Add("Mana", new int[1] { 9 });
            column_dict.Add("Armor", new int[1] { 10 });
            column_dict.Add("Fire resistance", new int[1] { 11 });
            column_dict.Add("Ice resistance", new int[1] { 12 });
            column_dict.Add("Black resistance", new int[1] { 13 });
            column_dict.Add("Mind resistance", new int[1] { 14 });
            column_dict.Add("Walking speed", new int[1] { 15 });
            column_dict.Add("Fighting speed", new int[1] { 16 });
            column_dict.Add("Casting speed", new int[1] { 17 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseInt16(textBox4.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, SFEngine.Utility.TryParseInt16(textBox6.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, SFEngine.Utility.TryParseInt16(textBox8.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, SFEngine.Utility.TryParseInt16(textBox10.Text));
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, SFEngine.Utility.TryParseInt16(textBox18.Text));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 6, SFEngine.Utility.TryParseInt16(textBox12.Text));
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 7, SFEngine.Utility.TryParseInt16(textBox14.Text));
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 8, SFEngine.Utility.TryParseInt16(textBox16.Text));
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 9, SFEngine.Utility.TryParseInt16(textBox17.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 10, SFEngine.Utility.TryParseInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 11, SFEngine.Utility.TryParseInt16(textBox3.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 12, SFEngine.Utility.TryParseInt16(textBox5.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 13, SFEngine.Utility.TryParseInt16(textBox7.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 14, SFEngine.Utility.TryParseInt16(textBox9.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 15, SFEngine.Utility.TryParseInt16(textBox11.Text));
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 16, SFEngine.Utility.TryParseInt16(textBox13.Text));
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 17, SFEngine.Utility.TryParseInt16(textBox15.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox4.Text = variant_repr(1);
            textBox6.Text = variant_repr(2);
            textBox8.Text = variant_repr(3);
            textBox10.Text = variant_repr(4);
            textBox18.Text = variant_repr(5);
            textBox12.Text = variant_repr(6);
            textBox14.Text = variant_repr(7);
            textBox16.Text = variant_repr(8);
            textBox17.Text = variant_repr(9);
            textBox2.Text = variant_repr(10);
            textBox3.Text = variant_repr(11);
            textBox5.Text = variant_repr(12);
            textBox7.Text = variant_repr(13);
            textBox9.Text = variant_repr(14);
            textBox11.Text = variant_repr(15);
            textBox13.Text = variant_repr(16);
            textBox15.Text = variant_repr(17);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox1, 2003);
            }
        }



        public override string get_element_string(int index)
        {
            UInt16 item_id = (UInt16)category[index][0];
            string txt = SFCategoryManager.GetItemName(item_id);
            return category[index][0].ToString() + " " + txt;
        }
    }
}
