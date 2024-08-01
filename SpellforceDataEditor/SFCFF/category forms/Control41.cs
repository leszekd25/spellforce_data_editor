using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control41 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2058 c2058;

        public Control41()
        {
            InitializeComponent();

            c2058 = SFCategoryManager.gamedata.c2058;
            category = c2058;

            column_dict.Add("Description ID", "DescriptionID");
            column_dict.Add("Text ID", "TextID");
        }

        private void tb_sd1_TextChanged(object sender, EventArgs e)
        {
            c2058.SetID(current_element, SFEngine.Utility.TryParseUInt16(tb_sd1.Text));
        }

        private void tb_sd2_TextChanged(object sender, EventArgs e)
        {
            c2058.SetField(current_element, "TextID", SFEngine.Utility.TryParseUInt16(tb_sd2.Text));
        }

        public override void show_element()
        {
            tb_sd1.Text = c2058[current_element].DescriptionID.ToString();
            tb_sd2.Text = c2058[current_element].TextID.ToString();
        }


        public override string get_element_string(int index)
        {
            return $"{c2058[index].DescriptionID} {SFCategoryManager.GetTextByLanguage(c2058[index].TextID, SFEngine.Settings.LanguageID)}";
        }

        private void tb_sd2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, tb_sd2.Text);
        }
    }
}
