using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control27 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2039 c2039;

        public Control27()
        {
            InitializeComponent();

            c2039 = SFCategoryManager.gamedata.c2039;
            category = c2039;

            column_dict.Add("Skill major type", "SkillMajorID");
            column_dict.Add("Skill minor type", "SkillMinorID");
            column_dict.Add("Text ID", "TextID");
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2039.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListSkills.Items.Clear();

            for (int i = 0; i < c2039.GetItemSubItemNum(current_element); i++)
            {
                string txt = SFCategoryManager.GetTextByLanguage(c2039[current_element, i].TextID, SFEngine.Settings.LanguageID);
                ListSkills.Items.Add(txt);
            }

            ListSkills.SelectedIndex = 0;
        }

        public override void show_element()
        {
            textBox3.Text = c2039[current_element, 0].SkillMajorID.ToString();
        }


        private void ListSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListSkills.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            textBox1.Text = c2039[current_element, ListSkills.SelectedIndex].TextID.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2039.SetField(current_element, ListSkills.SelectedIndex, "TextID", SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            c2039.AddSubItem(current_element, ListSkills.Items.Count, new()
            {
                SkillMajorID = c2039[current_element, 0].SkillMajorID,
                SkillMinorID = (byte)ListSkills.Items.Count,
                TextID = 0
            });

            ListSkills.SelectedIndex = ListSkills.Items.Count - 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (c2039.GetItemSubItemNum(current_element) == 1)
            {
                return;
            }

            c2039.RemoveSub(current_element, ListSkills.Items.Count - 1);

            ListSkills.SelectedIndex = ListSkills.Items.Count - 1;
        }


        public override string get_element_string(int index)
        {
            return $"{c2039[index, 0].SkillMajorID} {SFCategoryManager.GetTextByLanguage(c2039[index, 0].TextID, SFEngine.Settings.LanguageID)}";
        }

        public override void on_add_subelement(int subelem_index)
        {
            ListSkills.Items.Add(SFCategoryManager.GetTextByLanguage(c2039[current_element, subelem_index].TextID, SFEngine.Settings.LanguageID));
        }

        public override void on_remove_subelement(int subelem_index)
        {
            ListSkills.Items.RemoveAt(subelem_index);
        }

        public override void on_update_subelement(int subelem_index)
        {
            if (ListSkills.SelectedIndex != subelem_index)
            {
                return;
            }

            textBox1.Text = c2039[current_element, subelem_index].TextID.ToString();
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox1.Text);
        }
    }
}
