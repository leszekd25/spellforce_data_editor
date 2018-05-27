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
    public partial class Control19 : SpellforceDataEditor.category_forms.SFControl
    {
        private Dictionary<CheckBox, TextBox> check_to_text;
        private Dictionary<CheckBox, int> check_to_flag;
        private Dictionary<int, CheckBox> flag_to_check;
        private Dictionary<TextBox, Label> text_to_name;

        private UInt32 item_flags = 0;
        private bool edit_ready = false;

        public Control19()
        {
            InitializeComponent();
            column_dict.Add("Unit ID", new int[1] { 0 });
            column_dict.Add("Equipment slot", new int[1] { 1 });
            column_dict.Add("Item ID", new int[1] { 2 });

            check_to_text = new Dictionary<CheckBox, TextBox>();
            check_to_text[CheckHelmet] = HelmetID;
            check_to_text[CheckRightHand] = RightHandID;
            check_to_text[CheckChest] = ChestID;
            check_to_text[CheckLeftHand] = LeftHandID;
            check_to_text[CheckRightRing] = RightRingID;
            check_to_text[CheckLegs] = LegsID;
            check_to_text[CheckLeftRing] = LeftRingID;

            check_to_flag = new Dictionary<CheckBox, int>();
            check_to_flag[CheckHelmet] = 0;
            check_to_flag[CheckRightHand] = 1;
            check_to_flag[CheckChest] = 2;
            check_to_flag[CheckLeftHand] = 3;
            check_to_flag[CheckRightRing] = 4;
            check_to_flag[CheckLegs] = 5;
            check_to_flag[CheckLeftRing] = 6;

            flag_to_check = new Dictionary<int, CheckBox>();
            flag_to_check[0] = CheckHelmet;
            flag_to_check[1] = CheckRightHand;
            flag_to_check[2] = CheckChest;
            flag_to_check[3] = CheckLeftHand;
            flag_to_check[4] = CheckRightRing;
            flag_to_check[5] = CheckLegs;
            flag_to_check[6] = CheckLeftRing;

            text_to_name = new Dictionary<TextBox, Label>();
            text_to_name[HelmetID] = HelmetName;
            text_to_name[RightHandID] = RightHandName;
            text_to_name[ChestID] = ChestName;
            text_to_name[LeftHandID] = LeftHandName;
            text_to_name[RightRingID] = RightRingName;
            text_to_name[LegsID] = LegsName;
            text_to_name[LeftRingID] = LeftRingName;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            for (int i = 0; i < elem_count; i++)
                category.set_element_variant(current_element, 0 + 3 * i, Utility.TryParseUInt16(textBox1.Text));
        }

        private void TryToFlip(CheckBox ch)
        {
            bool checkd = ch.Checked;
            int flag = check_to_flag[ch];

            //if it's the last checked flag, disallow unchecking it
            if (checkd == false)
            {
                if ((item_flags - (0x1 << flag)) == 0)
                {
                    ch.Checked = true;
                    return;
                }
            }

            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;
            SFCategoryElement new_elem = new SFCategoryElement();
            Object[] obj_array;

            if (checkd == true)
            {
                //find if element exists already, stop if true
                item_flags = item_flags | (uint)(0x1 << flag);
                check_to_text[ch].Enabled = true;
                for(int i = 0; i < elem_count; i++)
                {
                    Byte item_slot = (Byte)(elem.get_single_variant(i * 3 + 1)).value;
                    if(flag == item_slot)
                    {
                        return;
                    }
                }

                //add checked element

                //can be added at the end? need to test
                obj_array = new Object[(elem_count + 1) * 3];
                for (int i = 0; i < elem_count; i++)
                {
                    {
                        obj_array[i * 3 + 0] = (UInt16)elem.get_single_variant(i * 3 + 0).value;
                        obj_array[i * 3 + 1] = (Byte)elem.get_single_variant(i * 3 + 1).value;
                        obj_array[i * 3 + 2] = (UInt16)elem.get_single_variant(i * 3 + 2).value;
                    }
                }
                obj_array[elem_count * 3 + 0] = (UInt16)elem.get_single_variant(0).value;
                obj_array[elem_count * 3 + 1] = (Byte)flag;
                obj_array[elem_count * 3 + 2] = Utility.TryParseUInt16(check_to_text[ch].Text);
            }
            else
            {
                //find if element doesn't exist, stop if true
                if ((item_flags & (uint)(0x1 << flag)) != 0)
                {
                    item_flags -= (uint)(0x1 << flag);
                    check_to_text[ch].Enabled = false;
                }
                bool found = false;
                for (int i = 0; i < elem_count; i++)
                {
                    Byte item_slot = (Byte)(elem.get_single_variant(i * 3 + 1)).value;
                    if (flag == item_slot)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return;
                }

                //remove unchecked element
                int offset = 0;
                obj_array = new Object[(elem_count - 1) * 3];
                for (int i = 0; i < elem_count; i++)
                {
                    Byte item_slot = (Byte)(elem.get_single_variant(i * 3 + 1)).value;
                    if (item_slot == (Byte)flag)
                    {
                        offset = 1;
                        continue;
                    }
                    for (int j = 0; j < 3; j++)
                    {
                        obj_array[(i - offset) * 3 + j] = elem.get_single_variant(i * 3 + j).value;
                    }
                }
            }

            new_elem.set(obj_array);
            category.get_elements()[current_element] = new_elem;
            //set_element(current_element);
        }

        private void CheckHelmet_CheckedChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;
            TryToFlip(CheckHelmet);
        }

        private void CheckRightHand_CheckedChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;
            TryToFlip(CheckRightHand);
        }

        private void CheckChest_CheckedChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;
            TryToFlip(CheckChest);
        }

        private void CheckLeftHand_CheckedChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;
            TryToFlip(CheckLeftHand);
        }

        private void CheckRightRing_CheckedChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;
            TryToFlip(CheckRightRing);
        }

        private void CheckLegs_CheckedChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;
            TryToFlip(CheckLegs);
        }

        private void CheckLeftRing_CheckedChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;
            TryToFlip(CheckLeftRing);
        }

        private void set_single_variant(TextBox item_id, Byte item_slot)
        {
            //find element by item slot and (if exists) modify its item id
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;
            for (int i = 0; i < elem_count; i++)
            {
                if (((Byte)(elem.get_single_variant(i * 3 + 1)).value) == item_slot)
                {
                    UInt16 id = Utility.TryParseUInt16(item_id.Text);
                    elem.set_single_variant(i * 3 + 2, id);
                    text_to_name[item_id].Text = category.get_manager().get_item_name(id);
                    return;
                }
            }
        }

        private void HelmetID_TextChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;

            set_single_variant(HelmetID, (Byte)check_to_flag[CheckHelmet]);
        }

        private void RightHandID_TextChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;

            set_single_variant(RightHandID, (Byte)check_to_flag[CheckRightHand]);
        }

        private void ChestID_TextChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;

            set_single_variant(ChestID, (Byte)check_to_flag[CheckChest]);
        }

        private void LeftHandID_TextChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;

            set_single_variant(LeftHandID, (Byte)check_to_flag[CheckLeftHand]);
        }

        private void RightRingID_TextChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;

            set_single_variant(RightRingID, (Byte)check_to_flag[CheckRightRing]);
        }

        private void LegsID_TextChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;

            set_single_variant(LegsID, (Byte)check_to_flag[CheckLegs]);
        }

        private void LeftRingID_TextChanged(object sender, EventArgs e)
        {
            if (!edit_ready) return;

            set_single_variant(LeftRingID, (Byte)check_to_flag[CheckLeftRing]);
        }

        public override void set_element(int index)
        {
            current_element = index;
            item_flags = 0;
            edit_ready = false;

            foreach (CheckBox ch in check_to_text.Keys)
            {
                ch.Checked = false;
            }
            foreach (TextBox tb in check_to_text.Values)
            {
                tb.Enabled = false;
                tb.Text = "0";
            }
            foreach (Label lb in text_to_name.Values)
                lb.Text = "<no name>";

            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            for (int i = 0; i < elem_count; i++)
            {
                Byte item_slot = (Byte)(elem.get_single_variant(i * 3 + 1)).value;
                UInt16 item_id = (UInt16)(elem.get_single_variant(i * 3 + 2)).value;

                CheckBox ch = flag_to_check[(int)item_slot];
                check_to_text[ch].Enabled = true;
                check_to_text[ch].Text = item_id.ToString();
                text_to_name[check_to_text[ch]].Text = category.get_manager().get_item_name(item_id);
                ch.Checked = true;
                item_flags |= (uint)(0x1 << item_slot);
            }

            show_element();
            edit_ready = true;
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
        }
    }
}
