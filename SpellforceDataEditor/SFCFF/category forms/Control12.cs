using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control12 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2014 c2014;

        public Control12()
        {
            InitializeComponent();

            c2014 = SFCategoryManager.gamedata.c2014;
            category = c2014;

            column_dict.Add("Item ID", "ItemID");
            column_dict.Add("Item effect index", "EffectIndex");
            column_dict.Add("Effect ID", "EffectID");
        }

        private void set_list_text(int i)
        {
            UInt16 effect_id = c2014[current_element, i].EffectID;

            string txt = SFCategoryManager.GetEffectName(effect_id, true);
            ListEffects.Items[i] = txt;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2014.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListEffects.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            c2014.SetField(current_element, cur_selected, "EffectID", SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListEffects.Items.Clear();

            for (int i = 0; i < c2014.GetItemSubItemNum(current_element); i++)
            {
                ListEffects.Items.Add("");
                set_list_text(i);
            }

            show_element();
        }

        public override void show_element()
        {
            c2014.GetID(current_element, out int id);
            textBox1.Text = id.ToString();
        }

        private void ListEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListEffects.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            textBox3.Text = c2014[current_element, cur_selected].EffectID.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Category2014Item item = new Category2014Item();
            c2014.GetID(current_element, out int id);
            item.SetID(id);

            int new_index;
            if (ListEffects.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                new_index = ListEffects.Items.Count - 1;
            }
            else
            {
                new_index = ListEffects.SelectedIndex;
            }

            Byte max_index = 0;
            for (int i = 0; i < c2014.GetItemSubItemNum(current_element); i++)
            {
                max_index = Math.Max(max_index, c2014[current_element, i].EffectIndex);
            }
            item.EffectIndex = (byte)(max_index + 1);

            c2014.AddSubItem(current_element, new_index, item);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListEffects.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            if (ListEffects.Items.Count == 1)
            {
                return;
            }

            int new_index = ListEffects.SelectedIndex;

            byte cur_effect_index = c2014[current_element, new_index].EffectIndex;
            c2014.RemoveSub(current_element, new_index);

            for (int i = 0; i < c2014.GetItemSubItemNum(current_element); i++)
            {
                if (c2014[current_element, i].EffectIndex > cur_effect_index)
                {
                    c2014.SetField(current_element, i, "EffectIndex", (byte)(c2014[current_element, i].EffectIndex - 1));
                }
            }
        }


        public override string get_element_string(int index)
        {
            UInt16 item_id = c2014[index, 0].ItemID;
            UInt16 effect_id = c2014[index, 0].EffectID;
            return $"{item_id} {SFCategoryManager.GetItemName(item_id)} | {SFCategoryManager.GetEffectName(effect_id, true)}";
        }

        public override void on_add_subelement(int subelem_index)
        {
            ListEffects.Items.Insert(subelem_index, "");
            set_list_text(subelem_index);
        }

        public override void on_remove_subelement(int subelem_index)
        {
            ListEffects.Items.RemoveAt(subelem_index);
        }

        public override void on_update_subelement(int subelem_index)
        {
            set_list_text(subelem_index);
            if (ListEffects.SelectedIndex == subelem_index)
            {
                textBox3.Text = c2014[current_element, subelem_index].EffectID.ToString();
            }
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2003, textBox1.Text);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2002, textBox3.Text);
        }
    }
}
