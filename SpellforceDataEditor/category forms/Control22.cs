using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.category_forms
{
    public partial class Control22 : SpellforceDataEditor.category_forms.SFControl
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
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 7;
            bool found = false;
            for (int i = 0; i < elem_count; i++)
            {
                if ((int)(byte)elem.get_single_variant(i * 7 + 1).value == slot_id)
                {
                    return i;
                }
            }
            return -1;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 7;
            for (int i = 0; i < elem_count; i++)
                set_element_variant(current_element, 0 + i * 7, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, 1 + index * 7, Utility.TryParseUInt8(textBox6.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex+1);
            set_element_variant(current_element, 2 + index * 7, Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, 3 + index * 7, Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, 4 + index * 7, Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, 5 + index * 7, Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            int index = get_subelem_index_by_slot_id(ListSlots.SelectedIndex + 1);
            set_element_variant(current_element, 6 + index * 7, Utility.TryParseUInt16(textBox7.Text));
        }

        public override void set_element(int index)
        {
            current_element = index;
            ListSlots.ItemCheck -= new ItemCheckEventHandler(this.ListSlots_ItemCheck);

            for (int i = 0; i < 6; i++)
            {
                ListSlots.SetItemChecked(i, false);
            }

            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 7;
            for (int i = 0; i < elem_count; i++)
            {
                Byte b = (Byte)elem.get_single_variant(i * 7 + 1).value;
                if ((b < 1) || (b > 6))
                    continue;
                ListSlots.SetItemChecked(((int)((Byte)elem.get_single_variant(i * 7 + 1).value)) - 1, true);
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
            for(int i = 0; i < 6; i++)
            {
                if(get_subelem_index_by_slot_id(i) != -1)
                {
                    ListSlots.SelectedIndex = i;
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

        private void ListSlots_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = ListSlots.SelectedIndex;
            if (index == -1)
                return;

            index = get_subelem_index_by_slot_id(index + 1);
            bool enable = ListSlots.GetItemChecked(ListSlots.SelectedIndex);
            textBox2.Enabled = enable;
            textBox3.Enabled = enable;
            textBox5.Enabled = enable;
            textBox4.Enabled = enable;
            textBox7.Enabled = enable;

            if (!enable)
                return;
            textBox6.Text = variant_repr(1 + index * 7);
            textBox2.Text = variant_repr(2 + index * 7);
            textBox3.Text = variant_repr(3 + index * 7);
            textBox5.Text = variant_repr(4 + index * 7);
            textBox4.Text = variant_repr(5 + index * 7);
            textBox7.Text = variant_repr(6 + index * 7);
        }

        private void ListSlots_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int index = e.Index;
            index = get_subelem_index_by_slot_id(index + 1);

            // if last checkbox unchecked, prevent
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 7;
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
                    if ((Byte)elem.get_single_variant(i * 7 + 1).value == old_item_slot)
                    {
                        offset = 1;
                        continue;
                    }
                    for (int j = 0; j < 7; j++)
                    {
                        obj_array[(i - offset) * 7 + j] = elem.get_single_variant(i * 7 + j).value;
                    }
                }
                new_elem.set(obj_array);
                category.get_elements()[current_element] = new_elem;
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
                    if((offset == 0)&&((Byte)elem.get_single_variant(i * 7 + 1).value > new_item_slot))
                    {
                        obj_array[(i + offset) * 7 + 0] = (UInt16)elem.get_single_variant(0).value;
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
                        obj_array[(i + offset) * 7 + j] = elem.get_single_variant(i * 7 + j).value;
                    }
                }
                if(offset == 0)
                {
                    obj_array[elem_count * 7 + 0] = (UInt16)elem.get_single_variant(0).value;
                    obj_array[elem_count * 7 + 1] = (Byte)(e.Index + 1);
                    obj_array[elem_count * 7 + 2] = (UInt16)0;
                    obj_array[elem_count * 7 + 3] = (Byte)0;
                    obj_array[elem_count * 7 + 4] = (UInt16)0;
                    obj_array[elem_count * 7 + 5] = (Byte)0;
                    obj_array[elem_count * 7 + 6] = (UInt16)0;
                }
                new_elem.set(obj_array);
                category.get_elements()[current_element] = new_elem;
                set_element(current_element);
            }

            ListSlots.SelectedIndex = -1;
            ListSlots.SelectedIndex = e.Index;
        }
    }
}
