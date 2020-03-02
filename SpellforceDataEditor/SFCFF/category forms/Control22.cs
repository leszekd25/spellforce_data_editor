using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 7;
            for (int i = 0; i < elem_count; i++)
            {
                if ((int)(byte)elem[i * 7 + 1] == slot_id)
                {
                    return i;
                }
            }
            return -1;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 7;
            for (int i = 0; i < elem_count; i++)
                set_element_variant(current_element, 0 + i * 7, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex+1);
            set_element_variant(current_element, 2 + index * 7, Utility.TryParseUInt16(textBox2.Text));

            item1_name.Text = SFCategoryManager.GetItemName(Utility.TryParseUInt16(textBox2.Text, 0));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, 3 + index * 7, Utility.TryParseUInt8(textBox3.Text));
            UpdateEffectiveChance();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, 4 + index * 7, Utility.TryParseUInt16(textBox5.Text));

            item2_name.Text = SFCategoryManager.GetItemName(Utility.TryParseUInt16(textBox5.Text, 0));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, 5 + index * 7, Utility.TryParseUInt8(textBox4.Text));
            UpdateEffectiveChance();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, 6 + index * 7, Utility.TryParseUInt16(textBox7.Text));

            item3_name.Text = SFCategoryManager.GetItemName(Utility.TryParseUInt16(textBox7.Text, 0));
        }

        public override void set_element(int index)
        {
            current_element = index;
            ListSlots.ItemCheck -= new ItemCheckEventHandler(this.ListSlots_ItemCheck);

            for (int i = 0; i < 6; i++)
            {
                ListSlots.SetItemChecked(i, false);
            }

            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 7;
            for (int i = 0; i < elem_count; i++)
            {
                Byte b = (Byte)elem[i * 7 + 1];
                if ((b < 1) || (b > 6))
                    continue;
                ListSlots.SetItemChecked(((int)((Byte)elem[i * 7 + 1])) - 1, true);
            }

            for (int i = 0; i < 6; i++)
            {
                if (ListSlots.GetItemChecked(i))
                {
                    ListSlots.SelectedIndex = i;
                    break;
                }
            }
            ListSlots.ItemCheck += new ItemCheckEventHandler(this.ListSlots_ItemCheck);
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            for(int i = 1; i <= 6; i++)
            {
                if(get_subelem_index_by_slot_id(i) != Utility.NO_INDEX)
                {
                    ListSlots.SelectedIndex = Utility.NO_INDEX;
                    ListSlots.SelectedIndex = i-1;
                    return;
                }
            }
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 17);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox2, 6);
        }

        private void textBox5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox5, 6);
        }

        private void textBox7_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox7, 6);
        }

        private void UpdateEffectiveChance()
        {
            textBox8.Text = "0"; textBox6.Text = "0"; textBox10.Text = "0"; 

            SFCategoryElement elem = category[current_element];

            int slot_id = ListSlots.SelectedIndex + 1;
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            int item_num = 0;
            for (int i = 0; i < 3; i++)    // check for items
            {
                if ((UInt16)elem[7 * index + 2 + i * 2] != 0)
                    item_num++;
            }

            Single[] chances = new Single[3];
            for (int i = 0; i < item_num; i++)
            {
                UInt16 item_id = (UInt16)elem[7 * index + 2 + i * 2];
                Byte data_chance = 0;
                if (i != 2)
                    data_chance = (Byte)elem[7 * index + 3 + i * 2];
                if (i == 0)
                {
                    chances[0] = (Single)(data_chance);
                    textBox8.Text = chances[0].ToString();
                }
                else if (i == 1)
                {
                    chances[1] = (Single)(data_chance) * (1 - chances[0]/100); 
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
            if (index == Utility.NO_INDEX)
                return;

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
            textBox2.Text = variant_repr(2 + index * 7);
            textBox3.Text = variant_repr(3 + index * 7);
            textBox5.Text = variant_repr(4 + index * 7);
            textBox4.Text = variant_repr(5 + index * 7);
            textBox7.Text = variant_repr(6 + index * 7);

            item1_name.Text = SFCategoryManager.GetItemName(Utility.TryParseUInt16(textBox2.Text, 0));
            item2_name.Text = SFCategoryManager.GetItemName(Utility.TryParseUInt16(textBox5.Text, 0));
            item3_name.Text = SFCategoryManager.GetItemName(Utility.TryParseUInt16(textBox7.Text, 0));

            UpdateEffectiveChance();
        }

        private void ListSlots_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int index = e.Index;
            index = get_subelem_index_by_slot_id(index + 1);

            // if last checkbox unchecked, prevent
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 7;
            int checked_slots = 0;
            for (int i = 0; i < 6; i++)
                checked_slots += (ListSlots.GetItemChecked(i)) ? 1 : 0;
            if ((checked_slots == 1) && (e.NewValue == CheckState.Unchecked))
            {
                e.NewValue = CheckState.Checked;
                return;
            }

            // if unchecked, remove element
            if (e.NewValue == CheckState.Unchecked)
            {
                int old_item_slot = e.Index + 1;

                int offset = 0;
                SFCategoryElement new_elem = new SFCategoryElement();
                Object[] obj_array = new Object[(elem_count - 1) * 7];
                for (int i = 0; i < elem_count; i++)
                {
                    if ((Byte)elem[i * 7 + 1] == old_item_slot)
                    {
                        offset = 1;
                        continue;
                    }
                    for (int j = 0; j < 7; j++)
                    {
                        obj_array[(i - offset) * 7 + j] = elem[i * 7 + j];
                    }
                }
                new_elem.AddVariants(obj_array);
                category[current_element] = new_elem;
                set_element(current_element);
            }
            else if(e.NewValue == CheckState.Checked)
            {
                int new_item_slot = e.Index + 1;
                
                int offset = 0;
                SFCategoryElement new_elem = new SFCategoryElement();
                Object[] obj_array = new Object[(elem_count + 1) * 7];
                for (int i = 0; i < elem_count; i++)
                {
                    if((offset == 0)&&((Byte)elem[i * 7 + 1] > new_item_slot))
                    {
                        obj_array[(i + offset) * 7 + 0] = (UInt16)elem[0];
                        obj_array[(i + offset) * 7 + 1] = (Byte)(e.Index + 1);
                        obj_array[(i + offset) * 7 + 2] = (UInt16)0;
                        obj_array[(i + offset) * 7 + 3] = (Byte)0;
                        obj_array[(i + offset) * 7 + 4] = (UInt16)0;
                        obj_array[(i + offset) * 7 + 5] = (Byte)0;
                        obj_array[(i + offset) * 7 + 6] = (UInt16)0;
                        offset = 1;
                    }
                    for (int j = 0; j < 7; j++)
                    {
                        obj_array[(i + offset) * 7 + j] = elem[i * 7 + j];
                    }
                }
                if(offset == 0)
                {
                    obj_array[elem_count * 7 + 0] = (UInt16)elem[0];
                    obj_array[elem_count * 7 + 1] = (Byte)(e.Index + 1);
                    obj_array[elem_count * 7 + 2] = (UInt16)0;
                    obj_array[elem_count * 7 + 3] = (Byte)0;
                    obj_array[elem_count * 7 + 4] = (UInt16)0;
                    obj_array[elem_count * 7 + 5] = (Byte)0;
                    obj_array[elem_count * 7 + 6] = (UInt16)0;
                }
                new_elem.AddVariants(obj_array);
                category[current_element] = new_elem;
                set_element(current_element);
            }

            ListSlots.SelectedIndex = Utility.NO_INDEX;
            ListSlots.SelectedIndex = e.Index;
        }
    }
}
