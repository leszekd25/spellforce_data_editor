using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;


namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control43 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2061 c2061;

        public Control43()
        {
            InitializeComponent();

            c2061 = SFCategoryManager.gamedata.c2061;
            category = c2061;

            column_dict.Add("Quest ID", "QuestID");
            column_dict.Add("Parent quest ID", "ParentQuestID");
            column_dict.Add("Is main quest", "IsMainQuest");
            column_dict.Add("Quest name ID", "NameID");
            column_dict.Add("Quest description ID", "DescriptionID");
            column_dict.Add("Quest order index", "OrderIndex");
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            c2061.SetID(current_element, (int)SFEngine.Utility.TryParseUInt32(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2061.SetField(current_element, "ParentQuestID", SFEngine.Utility.TryParseUInt32(textBox1.Text));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            c2061.SetField(current_element, "IsMainQuest", (Byte)(checkBox1.Checked ? 1 : 0));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2061.SetField(current_element, "NameID", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2061.SetField(current_element, "DescriptionID", SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            c2061.SetField(current_element, "OrderIndex", SFEngine.Utility.TryParseUInt32(textBox4.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = c2061[current_element].QuestID.ToString();
            textBox1.Text = c2061[current_element].ParentQuestID.ToString();
            checkBox1.Checked = (c2061[current_element].IsMainQuest != 0);
            textBox2.Text = c2061[current_element].NameID.ToString();
            textBox3.Text = c2061[current_element].DescriptionID.ToString();
            textBox4.Text = c2061[current_element].OrderIndex.ToString();
        }


        public override string get_element_string(int index)
        {
            return $"{c2061[index].QuestID} {SFCategoryManager.GetTextByLanguage(c2061[index].NameID, SFEngine.Settings.LanguageID)}";
        }

        public override string get_description_string(int index)
        {
            if (c2061.GetItemIndex((int)c2061[index].ParentQuestID, out int pqindex))
            {
                return $"{SFCategoryManager.GetTextByLanguage(c2061[index].DescriptionID, SFEngine.Settings.LanguageID)}\r\n\r\nPart of quest {SFCategoryManager.GetTextByLanguage(c2061[pqindex].NameID, SFEngine.Settings.LanguageID)}";
            }
            else
            {
                return $"{SFCategoryManager.GetTextByLanguage(c2061[index].DescriptionID, SFEngine.Settings.LanguageID)}";
            }
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2061, textBox1.Text);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox2.Text);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox3.Text);
        }
    }
}
