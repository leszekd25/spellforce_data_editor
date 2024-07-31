using SFEngine;
using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control19 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        struct ItemSlotUI
        {
            public int slot_id;
            public CheckBox box;
            public TextBox text;
            public Label label;

            public ItemSlotUI(int id, CheckBox b, TextBox t, Label l)
            {
                slot_id = id;
                box = b;
                text = t;
                label = l;
            }

            public void set_checked(bool b)
            {
                box.Checked = b;
                text.Enabled = b;
            }

            public void set_text(int id, string txt)
            {
                text.Text = id.ToString();
                label.Text = txt;
            }
        }

        private ItemSlotUI[] item_slots;

        private bool edit_ready = false;

        Category2025 c2025;

        public Control19()
        {
            InitializeComponent();

            c2025 = SFCategoryManager.gamedata.c2025;
            category = c2025;

            column_dict.Add("Unit ID", "UnitID");
            column_dict.Add("Equipment slot", "EquipmentIndex");
            column_dict.Add("Item ID", "ItemID");

            item_slots = new ItemSlotUI[]
            {
                new ItemSlotUI(0, CheckHelmet, HelmetID, HelmetName),
                new ItemSlotUI(1, CheckRightHand, RightHandID, RightHandName),
                new ItemSlotUI(2, CheckChest, ChestID, ChestName),
                new ItemSlotUI(3, CheckLeftHand, LeftHandID, LeftHandName),
                new ItemSlotUI(4, CheckRightRing, RightRingID, RightRingName),
                new ItemSlotUI(5, CheckLegs, LegsID, LegsName),
                new ItemSlotUI(6, CheckLeftRing, LeftRingID, LeftRingName),
            };
        }

        private int GetItemsChecked()
        {
            int n = 0;
            for (int i = 0; i < 7; i++)
            {
                n += (item_slots[i].box.Checked ? 1 : 0);
            }
            return n;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2025.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }


        private void CheckItem_Click(object sender, EventArgs e)
        {
            if (!edit_ready)
            {
                return;
            }

            CheckBox ch = (CheckBox)sender;

            byte flag = byte.Parse((string)(ch.Tag));

            //if it's the last checked flag, disallow unchecking it
            if (ch.Checked)
            {
                if (GetItemsChecked() == 1)
                {
                    return;
                }

                int subelem_index = SFEngine.Utility.NO_INDEX;
                for (int i = 0; i < c2025.GetItemSubItemNum(current_element); i++)
                {
                    Byte item_slot = c2025[current_element, i].EquipmentIndex;
                    if (flag == item_slot)
                    {
                        subelem_index = i;
                        break;
                    }
                }
                if (subelem_index == SFEngine.Utility.NO_INDEX)
                {
                    return;
                }

                c2025.RemoveSub(current_element, subelem_index);
                item_slots[flag].set_checked(false);
            }
            else
            {
                //add checked element
                int count = c2025.GetItemSubItemNum(current_element);
                c2025.GetID(current_element, out int cur_id);
                ushort item_id = SFEngine.Utility.TryParseUInt16(item_slots[flag].text.Text);
                c2025.AddSubItem(current_element, count, new()
                {
                    UnitID = (ushort)cur_id,
                    EquipmentIndex = flag,
                    ItemID = item_id
                });
                item_slots[flag].set_checked(true);
                item_slots[flag].set_text(item_id, SFCategoryManager.GetItemName(item_id));
            }
        }

        private void TextBoxItem_Validated(object sender, EventArgs e)
        {
            if (!edit_ready)
            {
                return;
            }

            TextBox item_id = (TextBox)sender;
            Byte item_slot = byte.Parse((string)(((TextBox)sender).Tag));

            for (int i = 0; i < c2025.GetItemSubItemNum(current_element); i++)
            {
                if (c2025[current_element, i].EquipmentIndex == item_slot)
                {
                    c2025.SetField(current_element, i, "ItemID", SFEngine.Utility.TryParseUInt16(item_id.Text));

                    return;
                }
            }
        }

        public override void set_element(int index)
        {
            current_element = index;
            edit_ready = false;

            for (int i = 0; i < 7; i++)
            {
                item_slots[i].set_checked(false);
                item_slots[i].set_text(0, Utility.S_ITEM_MISSING);
            }

            for (int i = 0; i < c2025.GetItemSubItemNum(current_element); i++)
            {
                Byte item_slot = c2025[current_element, i].EquipmentIndex;
                UInt16 item_id = c2025[current_element, i].ItemID;

                item_slots[item_slot].set_checked(true);
                item_slots[item_slot].set_text(item_id, SFCategoryManager.GetItemName(item_id));
            }

            show_element();
            edit_ready = true;
        }

        public override void show_element()
        {
            c2025.GetID(current_element, out int id);
            textBox1.Text = id.ToString();
        }

        public override string get_element_string(int index)
        {
            return $"{c2025[index, 0].UnitID} {SFCategoryManager.GetUnitName(c2025[index, 0].UnitID)}";
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
            set_element(current_element);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2024, textBox1.Text);
        }

        private void LeftRingID_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2003, ((TextBox)sender).Text);
        }
    }
}
