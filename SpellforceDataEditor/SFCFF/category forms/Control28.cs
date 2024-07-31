using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control28 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2062 c2062;

        public Control28()
        {
            InitializeComponent();

            c2062 = SFCategoryManager.gamedata.c2062;
            category = c2062;

            column_dict.Add("Skill ID", "SkillMajorID");
            column_dict.Add("Skill level", "SkillLevel");
            column_dict.Add("Strength", "Strength");
            column_dict.Add("Stamina", "Stamina");
            column_dict.Add("Agility", "Agility");
            column_dict.Add("Dexterity", "Dexterity");
            column_dict.Add("Charisma", "Charisma");
            column_dict.Add("Intelligence", "Intelligence");
            column_dict.Add("Wisdom", "Wisdom");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2062.SetID(current_element, SFEngine.Utility.TryParseUInt8(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2062.SetField(current_element, ListLevels.SelectedIndex, "Strength", SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            c2062.SetField(current_element, ListLevels.SelectedIndex, "Stamina", SFEngine.Utility.TryParseUInt8(textBox5.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            c2062.SetField(current_element, ListLevels.SelectedIndex, "Agility", SFEngine.Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            c2062.SetField(current_element, ListLevels.SelectedIndex, "Dexterity", SFEngine.Utility.TryParseUInt8(textBox7.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            c2062.SetField(current_element, ListLevels.SelectedIndex, "Charisma", SFEngine.Utility.TryParseUInt8(textBox6.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            c2062.SetField(current_element, ListLevels.SelectedIndex, "Intelligence", SFEngine.Utility.TryParseUInt8(textBox9.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            c2062.SetField(current_element, ListLevels.SelectedIndex, "Wisdom", SFEngine.Utility.TryParseUInt8(textBox8.Text));
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListLevels.Items.Clear();

            for (int i = 0; i < c2062.GetItemSubItemNum(current_element); i++)
            {
                ListLevels.Items.Add("Level " + (i + 1).ToString());
            }

            ListLevels.SelectedIndex = 0;
        }

        public override void show_element()
        {
            textBox1.Text = c2062[current_element, 0].SkillMajorID.ToString();
        }

        private void ListLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListLevels.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int index = ListLevels.SelectedIndex;
            textBox3.Text = c2062[current_element, index].Strength.ToString();
            textBox5.Text = c2062[current_element, index].Stamina.ToString();
            textBox4.Text = c2062[current_element, index].Agility.ToString();
            textBox7.Text = c2062[current_element, index].Dexterity.ToString();
            textBox6.Text = c2062[current_element, index].Charisma.ToString();
            textBox9.Text = c2062[current_element, index].Intelligence.ToString();
            textBox8.Text = c2062[current_element, index].Wisdom.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int index = ListLevels.Items.Count;

            c2062.AddSubItem(current_element, index, new()
            {
                SkillMajorID = c2062[current_element, index].SkillMajorID,
                SkillLevel = (byte)(index + 1)
            });

            ListLevels.SelectedIndex = index;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (c2062.GetItemSubItemNum(current_element) == 1)
            {
                return;
            }

            int index = ListLevels.Items.Count - 1;
            c2062.RemoveSub(current_element, index);

            ListLevels.SelectedIndex = Math.Min(index, ListLevels.Items.Count - 1);
        }


        public override string get_element_string(int index)
        {
            Byte skill_major = c2062[index, 0].SkillMajorID;
            string txt_skill = SFCategoryManager.GetSkillName(skill_major, 101, 0);
            return txt_skill;
        }

        public override void on_add_subelement(int subelem_index)
        {
            ListLevels.Items.Insert(subelem_index, "Level " + (subelem_index + 1).ToString());
        }

        public override void on_remove_subelement(int subelem_index)
        {
            ListLevels.Items.RemoveAt(subelem_index);
        }

        public override void on_update_subelement(int subelem_index)
        {
            if (ListLevels.SelectedIndex != subelem_index)
            {
                return;
            }

            textBox1.Text = c2062[current_element, subelem_index].SkillMajorID.ToString();
            textBox3.Text = c2062[current_element, subelem_index].Strength.ToString();
            textBox5.Text = c2062[current_element, subelem_index].Stamina.ToString();
            textBox4.Text = c2062[current_element, subelem_index].Agility.ToString();
            textBox7.Text = c2062[current_element, subelem_index].Dexterity.ToString();
            textBox6.Text = c2062[current_element, subelem_index].Charisma.ToString();
            textBox9.Text = c2062[current_element, subelem_index].Intelligence.ToString();
            textBox8.Text = c2062[current_element, subelem_index].Wisdom.ToString();
        }
    }
}
