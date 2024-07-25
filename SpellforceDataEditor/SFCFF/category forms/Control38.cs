using SFEngine;
using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control38 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2052 c2052;

        public Control38()
        {
            InitializeComponent();

            c2052 = SFCategoryManager.gamedata.c2052;
            category = c2052;

            column_dict.Add("Map ID", new int[1] { 0 });
            column_dict.Add("Unknown", new int[1] { 1 });
            column_dict.Add("Map handle", new int[1] { 2 });
            column_dict.Add("Name ID", new int[1] { 3 });
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            c2052.SetID(current_element, (int)SFEngine.Utility.TryParseUInt32(tb_effID.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2052.SetField(current_element, "IsPersistent", SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            c2052.SetField(current_element, "Handle", StringUtils.FromString(textBox4.Text, 0, 64));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2052.SetField(current_element, "NameID", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = c2052[current_element].MapID.ToString();
            textBox3.Text = c2052[current_element].IsPersistent.ToString();
            textBox4.Text = c2052[current_element].GetHandleString();
            textBox2.Text = c2052[current_element].NameID.ToString();
        }


        public override string get_element_string(int index)
        {
            return $"{c2052[index].MapID} {SFCategoryManager.GetTextByLanguage(c2052[index].NameID, 1)}";
        }
    }
}
