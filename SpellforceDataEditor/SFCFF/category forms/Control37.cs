using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control37 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2051 c2051;

        public Control37()
        {
            InitializeComponent();

            c2051 = SFCategoryManager.gamedata.c2051;
            category = c2051;

            column_dict.Add("NPC ID", "NPCID");
            column_dict.Add("Name ID", "TextID");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2051.SetID(current_element, (int)SFEngine.Utility.TryParseUInt32(textBox1.Text));
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2051.SetField(current_element, "TextID", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        public override void show_element()
        {
            textBox1.Text = c2051[current_element].NPCID.ToString();
            textBox2.Text = c2051[current_element].TextID.ToString();
        }



        public override string get_element_string(int index)
        {
            return $"{c2051[index].NPCID} {SFCategoryManager.GetTextByLanguage(c2051[index].TextID, SFEngine.Settings.LanguageID)}";
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox2.Text);
        }
    }
}
