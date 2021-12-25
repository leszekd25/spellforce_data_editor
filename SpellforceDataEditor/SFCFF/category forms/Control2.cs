using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFEngine.SFCFF;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control2 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control2()
        {
            InitializeComponent();
            column_dict.Add("Spell type ID", new int[1] { 0 });
            column_dict.Add("Spell text ID", new int[1] { 1 });
            column_dict.Add("Spell flags", new int[1] { 2 });
            column_dict.Add("Spell magic type", new int[1] { 3 });
            column_dict.Add("Minimum level", new int[1] { 4 });
            column_dict.Add("Maximum level", new int[1] { 5 });
            column_dict.Add("Availability", new int[1] { 6 });
            column_dict.Add("Spell UI handle", new int[1] { 7 });
            column_dict.Add("Description ID", new int[1] { 8 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, SFEngine.Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, SFEngine.Utility.TryParseUInt8(textBox5.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, SFEngine.Utility.TryParseUInt8(textBox7.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 6, SFEngine.Utility.TryParseUInt8(textBox6.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 7, SFString.FromString(textBox8.Text, 0, 64));// SFEngine.Utility.FixedLengthString(textBox8.Text, 64));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 8, SFEngine.Utility.TryParseUInt16(textBox9.Text));
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

            textbox_repr(textBox9, 2058);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox2, 2016);
        }

        private void textBox9_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (SFCategoryManager.gamedata[2058] == null)
                    return;

                int cur_id = SFEngine.Utility.TryParseInt32(textBox9.Text);
                int ind = SFCategoryManager.gamedata[2058].GetElementIndex(cur_id);

                if((ind == SFEngine.Utility.NO_INDEX)||(ind == 0))
                {
                    if(ind == 0)
                        cur_id = 2000;
                    // create new description
                    int new_id;
                    int new_ind;
                    new_ind = SFCategoryManager.gamedata[2058].GetNextNewElementIndex(cur_id, out new_id);
                    if (new_id > 4000)
                        return;

                    SFCategoryElement new_elem = new SFCategoryElement();
                    new_elem.AddVariant((ushort)new_id);
                    new_elem.AddVariant((ushort)0);
                    SFCategoryManager.gamedata[2058].elements.Insert(new_ind, new_elem);
                    SFCategoryManager.gamedata[2058].element_status.Insert(new_ind, SFCategoryElementStatus.ADDED);
                    textBox9.Text = new_id.ToString();
                    textBox9.BackColor = Color.DarkOrange;
                }
                else
                    step_into(textBox9, 2058);
            }
        }


        public override string get_element_string(int index)
        {
            string stype_txt = SFCategoryManager.GetTextFromElement(category[index], 1);
            return category[index][0].ToString() + " " + stype_txt;
        }

        public override string get_description_string(int index)
        {
            string spell_name = SFCategoryManager.GetTextFromElement(category[index], 1);
            string spell_desc = SFCategoryManager.GetDescriptionName((UInt16)category[index][8]);
            return spell_name + "\r\n" + spell_desc;
        }
    }
}
