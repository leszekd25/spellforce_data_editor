using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control49 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2072 c2072;
        public Control49()
        {
            InitializeComponent();

            c2072 = SFCategoryManager.gamedata.c2072;
            category = c2072;

            column_dict.Add("Set ID", "ItemSetID");
            column_dict.Add("Description ID", "DescriptionID");
            column_dict.Add("Set type", "ItemSetType");
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            c2072.SetID(current_element, SFEngine.Utility.TryParseUInt8(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2072.SetField(current_element, "DescriptionID", SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2072.SetField(current_element, "ItemSetType", SFEngine.Utility.TryParseUInt8(textBox2.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = c2072[current_element].ItemSetID.ToString();
            textBox1.Text = c2072[current_element].DescriptionID.ToString();
            textBox2.Text = c2072[current_element].ItemSetType.ToString();
        }


        public override string get_element_string(int index)
        {
            return $"{c2072[index].ItemSetID} {SFCategoryManager.GetTextByLanguage(c2072[index].DescriptionID, SFEngine.Settings.LanguageID)}";
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox1.Text);
        }
    }
}
