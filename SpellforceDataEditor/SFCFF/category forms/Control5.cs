using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control5 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2006 c2006;

        public Control5()
        {
            InitializeComponent();

            c2006 = SFCategoryManager.gamedata.c2006;
            category = c2006;

            column_dict.Add("Unit stats ID", "UnitStatsID");
            column_dict.Add("Unit major skill", "SkillMajorID");
            column_dict.Add("Unit minor skill", "SkillMinorID");
            column_dict.Add("Unit skill level", "SkillLevel");
        }

        private void set_list_text(int i)
        {
            Category2006Item item = c2006[current_element, i];
            ListSkills.Items[i] = SFCategoryManager.GetSkillName(item.SkillMajorID, item.SkillMinorID, item.SkillLevel);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2006.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSkills.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            c2006.SetField(current_element, cur_selected, "SkillMajorID", SFEngine.Utility.TryParseUInt8(textBox3.Text));
            set_list_text(cur_selected);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSkills.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            c2006.SetField(current_element, cur_selected, "SkillMinorID", SFEngine.Utility.TryParseUInt8(textBox4.Text));
            set_list_text(cur_selected);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSkills.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            c2006.SetField(current_element, cur_selected, "SkillLevel", SFEngine.Utility.TryParseUInt8(textBox2.Text));
            set_list_text(cur_selected);
        }

        private void ListSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSkills.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            Category2006Item item = c2006[current_element, cur_selected];

            textBox3.Text = item.SkillMajorID.ToString();
            textBox4.Text = item.SkillMinorID.ToString();
            textBox2.Text = item.SkillLevel.ToString();
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListSkills.Items.Clear();

            int start_index = c2006.GetSubItemIndex(current_element, 0);
            int num = c2006.GetItemSubItemNum(current_element);
            for (int i = 0; i < num; i++)
            {
                ListSkills.Items.Add("");
                set_list_text(i);
            }

            show_element();
        }

        public override void show_element()
        {
            Category2006Item item = c2006[current_element, 0];
            textBox1.Text = item.UnitStatsID.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Category2006Item item = new Category2006Item();
            c2006.GetID(current_element, out int id);
            item.SetID(id);

            int new_index;
            if (ListSkills.SelectedIndex == -1)
            {
                new_index = ListSkills.Items.Count - 1;
            }
            else
            {
                new_index = ListSkills.SelectedIndex;
            }

            c2006.AddSubItem(current_element, new_index, item);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListSkills.SelectedIndex == -1)
            {
                return;
            }

            if (ListSkills.Items.Count == 1)
            {
                return;
            }

            int new_index = ListSkills.SelectedIndex;

            c2006.RemoveSub(current_element, new_index);
        }


        public override string get_element_string(int index)
        {
            c2006.GetID(index, out int stats_id);
            if (!SFCategoryManager.gamedata.c2005.GetItemIndex(stats_id, out int stats_index))
            {
                return $"{stats_id} {SFEngine.Utility.S_ITEM_MISSING}";
            }
            Category2005Item item = SFCategoryManager.gamedata.c2005[stats_index];

            if (SFCategoryManager.hero_cache.GetItemIndex(item.StatsID, out int hero_index))
            {
                return $"{item.StatsID} {SFCategoryManager.GetRuneheroName(item.StatsID)} (lvl {item.UnitLevel})";
            }

            for (int i = 0; i < SFCategoryManager.gamedata.c2024.GetNumOfItems(); i++)
            {
                if (SFCategoryManager.gamedata.c2024[i].StatsID == item.StatsID)
                {
                    return $"{item.StatsID} {SFCategoryManager.GetTextByLanguage(SFCategoryManager.gamedata.c2024[i].NameID, SFEngine.Settings.LanguageID)} (lvl {item.UnitLevel})";
                }
            }

            return $"{item.StatsID} {SFEngine.Utility.S_ITEM_MISSING} (lvl {item.UnitLevel})";
        }

        public override void on_add_subelement(int subelem_index)
        {
            ListSkills.Items.Insert(subelem_index, "");
            set_list_text(subelem_index);
        }

        public override void on_remove_subelement(int subelem_index)
        {
            ListSkills.Items.RemoveAt(subelem_index);
        }

        public override void on_update_subelement(int subelem_index)
        {
            set_list_text(subelem_index);
            if (ListSkills.SelectedIndex == subelem_index)
            {
                Category2006Item item = c2006[current_element, subelem_index];

                textBox3.Text = item.SkillMajorID.ToString();
                textBox4.Text = item.SkillMinorID.ToString();
                textBox2.Text = item.SkillLevel.ToString();
            }
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2005, textBox1.Text);
        }
    }
}
