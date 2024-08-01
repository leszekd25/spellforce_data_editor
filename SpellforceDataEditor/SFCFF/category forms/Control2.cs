using SFEngine;
using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control2 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2054 c2054;

        public Control2()
        {
            InitializeComponent();

            c2054 = SFCategoryManager.gamedata.c2054;
            category = c2054;

            column_dict.Add("Spell type ID", "SpellLineID");
            column_dict.Add("Spell text ID", "TextID");
            column_dict.Add("Spell flags", "Flags");
            column_dict.Add("Spell magic type", "MagicType");
            column_dict.Add("Minimum level", "MinLevel");
            column_dict.Add("Maximum level", "MaxLevel");
            column_dict.Add("Availability", "Availability");
            column_dict.Add("Spell UI handle", "UIHandle");
            column_dict.Add("Description ID", "DescriptionID");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2054.SetField(current_element, "SpellLineID", SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2054.SetField(current_element, "TextID", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2054.SetField(current_element, "Flags", SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            c2054.SetField(current_element, "MagicType", SFEngine.Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            c2054.SetField(current_element, "MinLevel", SFEngine.Utility.TryParseUInt8(textBox5.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            c2054.SetField(current_element, "MaxLevel", SFEngine.Utility.TryParseUInt8(textBox7.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            c2054.SetField(current_element, "Availability", SFEngine.Utility.TryParseUInt8(textBox6.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            c2054.SetField(current_element, "UIHandle", StringUtils.FromString(textBox8.Text, 0, 64));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            c2054.SetField(current_element, "DescriptionID", SFEngine.Utility.TryParseUInt16(textBox9.Text));
            textbox_repr(textBox9, 2058);
        }

        public override void show_element()
        {
            Category2054Item item = c2054.Items[current_element];
            textBox1.Text = item.SpellLineID.ToString();
            textBox2.Text = item.TextID.ToString();
            textBox3.Text = item.Flags.ToString();
            textBox4.Text = item.MagicType.ToString();
            textBox5.Text = item.MinLevel.ToString();
            textBox7.Text = item.MaxLevel.ToString();
            textBox6.Text = item.Availability.ToString();
            textBox8.Text = item.GetHandleString();
            textBox9.Text = item.DescriptionID.ToString();

            textbox_repr(textBox9, 2058);
        }

        public override string get_element_string(int index)
        {
            Category2054Item item = c2054.Items[index];
            return $"{item.SpellLineID} {SFCategoryManager.GetTextByLanguage(item.TextID, SFEngine.Settings.LanguageID)}";
        }

        public override string get_description_string(int index)
        {
            Category2054Item item = c2054.Items[index];
            return $"{SFCategoryManager.GetTextByLanguage(item.TextID, SFEngine.Settings.LanguageID)}\r\n{SFCategoryManager.GetDescriptionName(item.DescriptionID)}";
        }


        // trace
        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox2.Text);
        }

        private void textBox9_MouseDown(object sender, MouseEventArgs e)
        {
            if(!textbox_trace(e, 2058, textBox9.Text))
            {
                textbox_gen_elem(textBox9, 2058);
                c2054.SetField(current_element, "DescriptionID", SFEngine.Utility.TryParseUInt16(textBox9.Text));
                textbox_repr(textBox9, 2058);
            }
        }
    }
}
