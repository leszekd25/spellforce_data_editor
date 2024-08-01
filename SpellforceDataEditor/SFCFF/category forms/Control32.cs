using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control32 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2044 c2044;

        public Control32()
        {
            InitializeComponent();

            c2044 = SFCategoryManager.gamedata.c2044;
            category = c2044;

            column_dict.Add("Resource ID", "ResourceID");
            column_dict.Add("Text ID", "TextID");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2044.SetID(current_element, SFEngine.Utility.TryParseUInt8(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2044.SetField(current_element, "TextID", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        public override void show_element()
        {
            textBox1.Text = c2044[current_element].ResourceID.ToString();
            textBox2.Text = c2044[current_element].TextID.ToString();
        }


        public override string get_element_string(int index)
        {
            return $"{c2044[index].ResourceID} {SFCategoryManager.GetTextByLanguage(c2044[index].TextID, SFEngine.Settings.LanguageID)}";
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox2.Text);
        }
    }
}
