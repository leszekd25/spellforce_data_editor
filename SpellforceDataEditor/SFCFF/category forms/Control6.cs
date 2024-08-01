using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control6 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2067 c2067;

        public Control6()
        {
            InitializeComponent();

            c2067 = SFCategoryManager.gamedata.c2067;
            category = c2067;

            column_dict.Add("Unit stats ID", "UnitStatsID");
            column_dict.Add("Unit spell index", "SpellIndex");
            column_dict.Add("Unit spell ID", "SpellID");
        }

        private void set_list_text(int i)
        {
            ushort spell_id = c2067[current_element, i].SpellID;

            string txt = SFCategoryManager.GetEffectName(spell_id, true);
            ListSpells.Items[i] = txt;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2067.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSpells.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            c2067.SetField(current_element, cur_selected, "SpellID", SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListSpells.Items.Clear();

            for (int i = 0; i < c2067.GetItemSubItemNum(current_element); i++)
            {
                ListSpells.Items.Add(SFCategoryManager.GetEffectName(c2067[current_element, i].SpellID, true));
            }

            show_element();
        }

        public override void show_element()
        {
            c2067.GetID(current_element, out int id);
            textBox1.Text = id.ToString();
        }

        private void ListSpells_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSpells.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            textBox3.Text = c2067[current_element, cur_selected].SpellID.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Category2067Item item = new Category2067Item();
            c2067.GetID(current_element, out int id);
            item.SetID(id);

            int new_index;
            if (ListSpells.SelectedIndex == -1)
            {
                new_index = ListSpells.Items.Count - 1;
            }
            else
            {
                new_index = ListSpells.SelectedIndex;
            }

            byte max_index = 0;
            for (int i = 0; i < c2067.GetItemSubItemNum(current_element); i++)
            {
                max_index = Math.Max(max_index, c2067[current_element, i].SpellIndex);
            }
            item.SpellIndex = (byte)(max_index + 1);

            c2067.AddSubItem(current_element, new_index, item);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListSpells.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            if (ListSpells.Items.Count == 1)
            {
                return;
            }

            int new_index = ListSpells.SelectedIndex;
            byte cur_spell_index = c2067[current_element, new_index].SpellIndex;
            c2067.RemoveSub(current_element, new_index);

            for (int i = 0; i < c2067.GetItemSubItemNum(current_element); i++)
            {
                if (c2067[current_element, i].SpellIndex > cur_spell_index)
                {
                    c2067.SetField(current_element, i, "SpellIndex", (byte)(c2067[current_element, i].SpellIndex - 1));
                }
            }
        }


        public override string get_element_string(int index)
        {
            c2067.GetID(index, out int stats_id);
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
            ListSpells.Items.Insert(subelem_index, "");
            set_list_text(subelem_index);
        }

        public override void on_remove_subelement(int subelem_index)
        {
            ListSpells.Items.RemoveAt(subelem_index);
        }

        public override void on_update_subelement(int subelem_index)
        {
            set_list_text(subelem_index);

            if (ListSpells.SelectedIndex == subelem_index)
            {
                textBox3.Text = c2067[current_element, subelem_index].SpellID.ToString();
            }
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2005, textBox1.Text);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2002, textBox3.Text);
        }
    }
}
