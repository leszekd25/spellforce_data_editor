using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control42 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2059 c2059;

        public Control42()
        {
            InitializeComponent();

            c2059 = SFCategoryManager.gamedata.c2059;
            category = c2059;

            column_dict.Add("Description ID", "ExtDescriptionID");
            column_dict.Add("Text ID", "TextID");
            column_dict.Add("Advanced text ID", "ExtTextID");
        }

        private void tb_sd3_TextChanged(object sender, EventArgs e)
        {
            c2059.SetID(current_element, SFEngine.Utility.TryParseUInt16(tb_sd3.Text));
        }

        private void tb_sd4_TextChanged(object sender, EventArgs e)
        {
            c2059.SetField(current_element, "TextID", SFEngine.Utility.TryParseUInt16(tb_sd4.Text));
        }

        private void sb_sd5_TextChanged(object sender, EventArgs e)
        {
            c2059.SetField(current_element, "ExtTextID", SFEngine.Utility.TryParseUInt16(sb_sd5.Text));
        }

        public override void show_element()
        {
            tb_sd3.Text = c2059[current_element].ExtDescriptionID.ToString();
            tb_sd4.Text = c2059[current_element].TextID.ToString();
            sb_sd5.Text = c2059[current_element].ExtTextID.ToString();
        }


        public override string get_element_string(int index)
        {
            return $"{c2059[index].ExtDescriptionID} {SFCategoryManager.GetTextByLanguage(c2059[index].ExtTextID, SFEngine.Settings.LanguageID)}";
        }

        public override string get_description_string(int index)
        {
            return $"Text ID: {SFCategoryManager.GetTextByLanguage(c2059[index].TextID, SFEngine.Settings.LanguageID)}";
        }

        private void tb_sd4_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, tb_sd4.Text);
        }

        private void sb_sd5_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, sb_sd5.Text);
        }
    }
}
