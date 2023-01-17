using SFEngine.SFCFF;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control22 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control22()
        {
            InitializeComponent();
            column_dict.Add("Unit ID", new int[1] { 0 });
            column_dict.Add("Slot index", new int[1] { 1 });
            column_dict.Add("Item 1 ID", new int[1] { 2 });
            column_dict.Add("Item 1 chance", new int[1] { 3 });
            column_dict.Add("Item 2 ID", new int[1] { 4 });
            column_dict.Add("Item 2 chance", new int[1] { 5 });
            column_dict.Add("Item 3 ID", new int[1] { 6 });
        }

        int get_subelem_index_by_slot_id(int slot_id)
        {
            // get absolute index in element
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                if ((int)(byte)category[current_element, i][1] == slot_id)
                {
                    return i;
                }
            }
            return SFEngine.Utility.NO_INDEX;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            MainForm.data.op_queue.OpenCluster();
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                set_element_variant(current_element, i, 0, SFEngine.Utility.TryParseUInt16(textBox1.Text));
            }

            MainForm.data.op_queue.CloseCluster();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, index, 2, SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, index, 3, SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, index, 4, SFEngine.Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, index, 5, SFEngine.Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, index, 6, SFEngine.Utility.TryParseUInt16(textBox7.Text));
        }

        public override void set_element(int index)
        {
            current_element = index;
            ListSlots.ItemCheck -= new ItemCheckEventHandler(ListSlots_ItemCheck);

            for (int i = 0; i < 6; i++)
            {
                ListSlots.SetItemChecked(i, false);
            }

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                Byte b = (Byte)category[current_element, i][1];
                if ((b < 1) || (b > 6))
                {
                    continue;
                }

                ListSlots.SetItemChecked(((int)((Byte)category[current_element, i][1])) - 1, true);
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
            textBox1.Text = variant_repr(0, 0);
            for (int i = 1; i <= 6; i++)
            {
                if (get_subelem_index_by_slot_id(i) != SFEngine.Utility.NO_INDEX)
                {
                    ListSlots.SelectedIndex = SFEngine.Utility.NO_INDEX;
                    ListSlots.SelectedIndex = i - 1;
                    return;
                }
            }
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox1, 2024);
            }
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox2, 2003);
            }
        }

        private void textBox5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox5, 2003);
            }
        }

        private void textBox7_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox7, 2003);
            }
        }

        private void UpdateEffectiveChance()
        {
            textBox8.Text = "0"; textBox6.Text = "0"; textBox10.Text = "0";

            int slot_id = ListSlots.SelectedIndex + 1;
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            int item_num = 0;
            for (int i = 0; i < 3; i++)    // check for items
            {
                if ((UInt16)category[current_element, index][2 + i * 2] != 0)
                {
                    item_num++;
                }
            }

            Single[] chances = new Single[3];
            for (int i = 0; i < item_num; i++)
            {
                UInt16 item_id = (UInt16)category[current_element, index][2 + i * 2];
                Byte data_chance = 0;
                if (i != 2)
                {
                    data_chance = (Byte)category[current_element, index][3 + i * 2];
                }

                if (i == 0)
                {
                    chances[0] = (Single)(data_chance);
                    textBox8.Text = chances[0].ToString();
                }
                else if (i == 1)
                {
                    chances[1] = (Single)(data_chance) * (1 - chances[0] / 100);
                    textBox6.Text = chances[1].ToString();
                }
                else if (i == 2)
                {
                    chances[2] = 100 - chances[0] - chances[1];
                    textBox10.Text = chances[2].ToString();
                }
            }
        }

        private void ListSlots_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = ListSlots.SelectedIndex;
            if (index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            index = get_subelem_index_by_slot_id(index + 1);
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
            textBox2.Text = variant_repr(index, 2);
            textBox3.Text = variant_repr(index, 3);
            textBox5.Text = variant_repr(index, 4);
            textBox4.Text = variant_repr(index, 5);
            textBox7.Text = variant_repr(index, 6);

            item1_name.Text = SFCategoryManager.GetItemName(SFEngine.Utility.TryParseUInt16(textBox2.Text, 0));
            item2_name.Text = SFCategoryManager.GetItemName(SFEngine.Utility.TryParseUInt16(textBox5.Text, 0));
            item3_name.Text = SFCategoryManager.GetItemName(SFEngine.Utility.TryParseUInt16(textBox7.Text, 0));

            UpdateEffectiveChance();
        }

        private void ListSlots_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int index = e.Index;
            index = get_subelem_index_by_slot_id(index + 1);

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

                int tmp_index = category.element_lists[current_element].GetIndexByID(old_item_slot);
                if (tmp_index == SFEngine.Utility.NO_INDEX)
                {
                    SFEngine.LogUtils.Log.Error(SFEngine.LogUtils.LogSource.SFCFF, "ListSlots_ItemCheck(): Could not find item at given ID (ID: " + old_item_slot.ToString() + ")");
                    throw new Exception("Could not find item at given ID");
                }

                MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
                {
                    CategoryIndex = category.category_id,
                    ElementIndex = current_element,
                    SubElementIndex = tmp_index,
                    IsSubElement = true,
                    IsRemoving = true,
                });
            }
            else if (e.NewValue == CheckState.Checked)
            {
                int new_item_slot = e.Index + 1;

                int tmp_index;
                for (tmp_index = 0; tmp_index < category.element_lists[current_element].Elements.Count; tmp_index++)
                {
                    if ((Byte)category[current_element, tmp_index][1] > new_item_slot)
                    {
                        break;
                    }
                }

                SFCategoryElement new_elem = category.GetEmptyElement();
                new_elem[0] = (UInt16)(category.element_lists[current_element].GetID());
                new_elem[1] = (Byte)(e.Index + 1);

                MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
                {
                    CategoryIndex = category.category_id,
                    ElementIndex = current_element,
                    SubElementIndex = tmp_index,
                    Element = new_elem,
                    IsSubElement = true,
                });
            }

            ListSlots.SelectedIndex = SFEngine.Utility.NO_INDEX;
            ListSlots.SelectedIndex = e.Index;
        }


        public override string get_element_string(int index)
        {
            UInt16 unit_id = (UInt16)category[index, 0][0];
            int slot_count = category.element_lists[index].Elements.Count;
            string txt_unit = SFCategoryManager.GetUnitName(unit_id);
            return unit_id.ToString() + " " + txt_unit + " - " + slot_count.ToString() + ((slot_count == 1) ? " slot" : " slots");
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
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            if (index != subelem_index)
            {
                return;
            }

            textBox2.Text = variant_repr(index, 2);
            textBox3.Text = variant_repr(index, 3);
            textBox5.Text = variant_repr(index, 4);
            textBox4.Text = variant_repr(index, 5);
            textBox7.Text = variant_repr(index, 6);

            item1_name.Text = SFCategoryManager.GetItemName(SFEngine.Utility.TryParseUInt16(textBox2.Text, 0));
            item2_name.Text = SFCategoryManager.GetItemName(SFEngine.Utility.TryParseUInt16(textBox5.Text, 0));
            item3_name.Text = SFCategoryManager.GetItemName(SFEngine.Utility.TryParseUInt16(textBox7.Text, 0));

            UpdateEffectiveChance();
        }
    }
}
