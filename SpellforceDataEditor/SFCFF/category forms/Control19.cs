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
    public partial class Control19 : SpellforceDataEditor.SFCFF.category_forms.SFControl
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
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                set_element_variant(current_element, i, 0, Utility.TryParseUInt16(textBox1.Text));
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

            if (checkd == true)
            {
                //find if element exists already, stop if true
                item_flags = item_flags | (uint)(0x1 << flag);
                check_to_text[ch].Enabled = true;
                for(int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                {
                    Byte item_slot = (Byte)(category[current_element, i][1]);
                    if(flag == item_slot)
                    {
                        return;
                    }
                }

                //add checked element

                //can be added at the end? need to test
                int count = category.element_lists[current_element].Elements.Count;
                category.element_lists[current_element].Elements.Add(category.GetEmptyElement());
                category[current_element, count][0] = (UInt16)(category.element_lists[current_element].GetID());
                category[current_element, count][1] = (Byte)flag;
                category[current_element, count][2] = Utility.TryParseUInt16(check_to_text[ch].Text);
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
                for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                {
                    Byte item_slot = (Byte)(category[current_element, i][1]);
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
                category.element_lists[current_element].Elements.RemoveAt(flag);
            }

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
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                if((Byte)(category[current_element, i][1]) == item_slot)
                {
                    UInt16 id = Utility.TryParseUInt16(item_id.Text);
                    category[current_element, i][2] = id;
                    text_to_name[item_id].Text = SFCategoryManager.GetItemName(id);
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

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                Byte item_slot = (Byte)(category[current_element, i][1]);
                UInt16 item_id = (UInt16)(category[current_element, i][2]);

                CheckBox ch = flag_to_check[(int)item_slot];
                check_to_text[ch].Enabled = true;
                check_to_text[ch].Text = item_id.ToString();
                text_to_name[check_to_text[ch]].Text = SFCategoryManager.GetItemName(item_id);
                ch.Checked = true;
                item_flags |= (uint)(0x1 << item_slot);
            }

            show_element();
            edit_ready = true;
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0, 0);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 2024);
        }

        private void HelmetID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(HelmetID, 2003);
        }

        private void RightHandID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(RightHandID, 2003);
        }

        private void ChestID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(ChestID, 2003);
        }

        private void LeftHandID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(LeftHandID, 2003);
        }

        private void RightRingID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(RightRingID, 2003);
        }

        private void LegsID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(LegsID, 2003);
        }

        private void LeftRingID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(LeftRingID, 2003);
        }

        public override string get_element_string(int index)
        {
            UInt16 unit_id = (UInt16)category[index, 0][0];
            string txt_unit = SFCategoryManager.GetUnitName(unit_id);
            return unit_id.ToString() + " " + txt_unit;
        }
    }
}
