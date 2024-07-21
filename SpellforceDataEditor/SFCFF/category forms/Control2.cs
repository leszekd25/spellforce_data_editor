using SFEngine;
using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Drawing;
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

            textbox_repr(textBox9, SFCategoryManager.gamedata.c2058);
        }

        public override string get_element_string(int index)
        {
            Category2054Item item = c2054.Items[index];
            return $"{item.SpellLineID} {SFCategoryManager.GetTextByLanguage(item.TextID, 1)}";
        }

        public override string get_description_string(int index)
        {
            Category2054Item item = c2054.Items[index];
            return $"{SFCategoryManager.GetTextByLanguage(item.TextID, 1)}\r\n{SFCategoryManager.GetDescriptionName(item.DescriptionID)}";
        }
    }
}
