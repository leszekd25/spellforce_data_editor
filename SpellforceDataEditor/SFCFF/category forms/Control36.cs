using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control36 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2065 c2065;

        public Control36()
        {
            InitializeComponent();

            c2065 = SFCategoryManager.gamedata.c2065;
            category = c2065;

            column_dict.Add("Object ID", "ObjectID");
            column_dict.Add("Slot index", "LootIndex");
            column_dict.Add("Item 1 ID", "ItemID1");
            column_dict.Add("Item 1 chance", "ItemChance1");
            column_dict.Add("Item 2 ID", "ItemID2");
            column_dict.Add("Item 2 chance", "ItemChance2");
            column_dict.Add("Item 3 ID", "ItemID3");
        }

        int get_subelem_index_by_slot_id(int slot_id)
        {
            // get absolute index in element
            for (int i = 0; i < c2065.GetItemSubItemNum(current_element); i++)
            {
                if (c2065[current_element, i].LootIndex == slot_id + 1)
                {
                    return i;
                }
            }
            return SFEngine.Utility.NO_INDEX;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2065.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex);
            c2065.SetField(current_element, index, "ItemID1", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex);
            c2065.SetField(current_element, index, "ItemChance1", SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex);
            c2065.SetField(current_element, index, "ItemID2", SFEngine.Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex);
            c2065.SetField(current_element, index, "ItemChance2", SFEngine.Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex);
            c2065.SetField(current_element, index, "ItemID3", SFEngine.Utility.TryParseUInt16(textBox7.Text));
        }

        public override void set_element(int index)
        {
            current_element = index;
            ListSlots.ItemCheck -= new ItemCheckEventHandler(ListSlots_ItemCheck);

            for (int i = 0; i < 6; i++)
            {
                ListSlots.SetItemChecked(i, false);
            }

            for (int i = 0; i < c2065.GetItemSubItemNum(current_element); i++)
            {
                Byte b = c2065[current_element, i].LootIndex;
                if ((b < 1) || (b > 6))
                {
                    continue;
                }

                ListSlots.SetItemChecked(c2065[current_element, i].LootIndex - 1, true);
            }

            for (int i = 0; i < 6; i++)
            {
                if (ListSlots.GetItemChecked(i))
                {
                    ListSlots.SelectedIndex = i;
                    break;
                }
            }
            ListSlots.ItemCheck += new ItemCheckEventHandler(ListSlots_ItemCheck);
        }

        public override void show_element()
        {
            textBox1.Text = c2065[current_element, 0].ObjectID.ToString();
            for (int i = 0; i < 6; i++)
            {
                if (get_subelem_index_by_slot_id(i) != SFEngine.Utility.NO_INDEX)
                {
                    ListSlots.SelectedIndex = SFEngine.Utility.NO_INDEX;
                    ListSlots.SelectedIndex = i;
                    return;
                }
            }
        }

        private void UpdateEffectiveChance()
        {
            textBox8.Text = "0"; textBox6.Text = "0"; textBox10.Text = "0";

            int slot_id = ListSlots.SelectedIndex;
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex);

            Single[] chances = new Single[3];
            if (c2065[current_element, index].ItemID1 != 0)
            {
                Byte data_chance = c2065[current_element, index].ItemChance1;
                chances[0] = (Single)(data_chance);
                textBox8.Text = chances[0].ToString();
            }
            if (c2065[current_element, index].ItemID2 != 0)
            {
                Byte data_chance = c2065[current_element, index].ItemChance2;
                chances[1] = (Single)(data_chance) * (1 - chances[0] / 100);
                textBox6.Text = chances[1].ToString();
            }
            if (c2065[current_element, index].ItemID3 != 0)
            {
                chances[2] = 100 - chances[0] - chances[1];
                textBox10.Text = chances[2].ToString();
            }
        }

        private void ListSlots_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = ListSlots.SelectedIndex;
            if (index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            bool enable = ListSlots.GetItemChecked(ListSlots.SelectedIndex);
            textBox2.Enabled = enable;
            textBox3.Enabled = enable;
            textBox5.Enabled = enable;
            textBox4.Enabled = enable;
            textBox7.Enabled = enable;

            if (!enable)
            {
                item1_name.Text = "";
                item2_name.Text = "";
                item3_name.Text = "";
                return;
            }

            index = get_subelem_index_by_slot_id(index);
            if (index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            textBox2.Text = c2065[current_element, index].ItemID1.ToString();
            textBox3.Text = c2065[current_element, index].ItemChance1.ToString();
            textBox5.Text = c2065[current_element, index].ItemID2.ToString();
            textBox4.Text = c2065[current_element, index].ItemChance2.ToString();
            textBox7.Text = c2065[current_element, index].ItemID3.ToString();

            item1_name.Text = SFCategoryManager.GetItemName(SFEngine.Utility.TryParseUInt16(textBox2.Text, 0));
            item2_name.Text = SFCategoryManager.GetItemName(SFEngine.Utility.TryParseUInt16(textBox5.Text, 0));
            item3_name.Text = SFCategoryManager.GetItemName(SFEngine.Utility.TryParseUInt16(textBox7.Text, 0));

            UpdateEffectiveChance();
        }

        private void ListSlots_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int index = e.Index;
            index = get_subelem_index_by_slot_id(index);

            // if last checkbox unchecked, prevent
            int checked_slots = 0;
            for (int i = 0; i < 6; i++)
            {
                checked_slots += (ListSlots.GetItemChecked(i)) ? 1 : 0;
            }

            if ((checked_slots == 1) && (e.NewValue == CheckState.Unchecked))
            {
                e.NewValue = CheckState.Checked;
                return;
            }

            // if unchecked, remove element
            if (e.NewValue == CheckState.Unchecked)
            {
                int old_item_slot = e.Index + 1;

                int tmp_index = SFEngine.Utility.NO_INDEX;
                for (int i = 0; i < c2065.GetItemSubItemNum(current_element); i++)
                {
                    if (c2065[current_element, i].LootIndex == old_item_slot)
                    {
                        tmp_index = i;
                        break;
                    }
                }

                if (tmp_index == SFEngine.Utility.NO_INDEX)
                {
                    SFEngine.LogUtils.Log.Error(SFEngine.LogUtils.LogSource.SFCFF, "ListSlots_ItemCheck(): Could not find item at given ID (ID: " + old_item_slot.ToString() + ")");
                    throw new Exception("Could not find item at given ID");
                }

                c2065.RemoveSub(current_element, tmp_index);
            }
            else if (e.NewValue == CheckState.Checked)
            {
                int new_item_slot = e.Index + 1;

                int tmp_index;
                for (tmp_index = 0; tmp_index < c2065.GetItemSubItemNum(current_element); tmp_index++)
                {
                    if (c2065[current_element, tmp_index].LootIndex > new_item_slot)
                    {
                        break;
                    }
                }

                c2065.AddSubItem(current_element, tmp_index, new()
                {
                    ObjectID = c2065[current_element, 0].ObjectID,
                    LootIndex = (byte)(e.Index + 1),
                });
            }

            ListSlots.SelectedIndex = SFEngine.Utility.NO_INDEX;
            ListSlots.SelectedIndex = e.Index;
        }


        public override string get_element_string(int index)
        {
            return $"{c2065[index, 0].ObjectID} {SFCategoryManager.GetObjectName(c2065[index, 0].ObjectID)} - {c2065.GetItemSubItemNum(index)} {((c2065.GetItemSubItemNum(index) == 1) ? "slot" : "slots")}";
        }

        public override void on_add_subelement(int subelem_index)
        {
            set_element(current_element);
        }

        public override void on_remove_subelement(int subelem_index)
        {
            set_element(current_element);
        }

        public override void on_update_subelement(int subelem_index)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex);
            if (index != subelem_index)
            {
                return;
            }

            textBox2.Text = c2065[current_element, index].ItemID1.ToString();
            textBox3.Text = c2065[current_element, index].ItemChance1.ToString();
            textBox5.Text = c2065[current_element, index].ItemID2.ToString();
            textBox4.Text = c2065[current_element, index].ItemChance2.ToString();
            textBox7.Text = c2065[current_element, index].ItemID3.ToString();

            item1_name.Text = SFCategoryManager.GetItemName(SFEngine.Utility.TryParseUInt16(textBox2.Text, 0));
            item2_name.Text = SFCategoryManager.GetItemName(SFEngine.Utility.TryParseUInt16(textBox5.Text, 0));
            item3_name.Text = SFCategoryManager.GetItemName(SFEngine.Utility.TryParseUInt16(textBox7.Text, 0));

            UpdateEffectiveChance();
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2050, textBox1.Text);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2003, ((TextBox)sender).Text);
        }
    }
}
