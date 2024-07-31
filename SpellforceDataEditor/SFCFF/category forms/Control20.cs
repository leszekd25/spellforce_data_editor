using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control20 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2026 c2026;

        public Control20()
        {
            InitializeComponent();

            c2026 = SFCategoryManager.gamedata.c2026;
            category = c2026;

            column_dict.Add("Unit ID", "UnitID");
            column_dict.Add("Unit spell index", "SpellIndex");
            column_dict.Add("Spell ID", "SpellID");
        }

        private void set_list_text(int i)
        {
            UInt16 spell_id = c2026[current_element, i].SpellID;

            string txt = SFCategoryManager.GetEffectName(spell_id, true);
            ListSpells.Items[i] = txt;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2026.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSpells.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            c2026.SetField(current_element, cur_selected, "SpellID", SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListSpells.Items.Clear();

            for (int i = 0; i < c2026.GetItemSubItemNum(current_element); i++)
            {
                Byte spell_order = c2026[current_element, i].SpellIndex;
                UInt16 spell_id = c2026[current_element, i].SpellID;

                string txt = SFCategoryManager.GetEffectName(spell_id, true);

                ListSpells.Items.Add(txt);
            }

            show_element();
        }

        public override void show_element()
        {
            c2026.GetID(current_element, out int id);
            textBox1.Text = id.ToString();
        }

        private void ListSpells_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSpells.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            textBox3.Text = c2026[current_element, cur_selected].SpellID.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int new_index;
            if (ListSpells.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                new_index = ListSpells.Items.Count - 1;
            }
            else
            {
                new_index = ListSpells.SelectedIndex;
            }

            Byte max_index = 0;
            for (int i = 0; i < c2026.GetItemSubItemNum(current_element); i++)
            {
                max_index = Math.Max(max_index, c2026[current_element, i].SpellIndex);
            }
            max_index += 1;

            c2026.AddSubItem(current_element, new_index, new()
            {
                UnitID = c2026[current_element, 0].UnitID,
                SpellIndex = max_index,
                SpellID = 0
            });
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
            Byte cur_spell_index = c2026[current_element, new_index].SpellIndex;

            c2026.RemoveSub(current_element, new_index);
            for (int i = 0; i < c2026.GetItemSubItemNum(current_element); i++)
            {
                if (c2026[current_element, i].SpellIndex > cur_spell_index)
                {
                    c2026.SetField(current_element, i, "SpellIndex", (ushort)(c2026[current_element, i].SpellIndex - 1));
                }
            }
        }


        public override string get_element_string(int index)
        {
            return $"{c2026[index, 0].UnitID} {SFCategoryManager.GetUnitName(c2026[index, 0].UnitID)}";
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
                textBox3.Text = c2026[current_element, subelem_index].SpellID.ToString();
            }
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2024, textBox1.Text);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2002, textBox3.Text);
        }
    }
}
