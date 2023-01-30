using SFEngine.SFCFF;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control19 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        struct ItemSlotUI
        {
            public bool active;
            public int slot_id;
            public CheckBox box;
            public TextBox text;
            public Label label;

            public ItemSlotUI(int id, CheckBox b, TextBox t, Label l)
            {
                active = false;
                slot_id = id;
                box = b;
                text = t;
                label = l;
            }

            public void set_checked(bool b)
            {
                active = b;
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

        public Control19()
        {
            InitializeComponent();
            column_dict.Add("Unit ID", new int[1] { 0 });
            column_dict.Add("Equipment slot", new int[1] { 1 });
            column_dict.Add("Item ID", new int[1] { 2 });

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
            for(int i = 0; i < 7; i++)
            {
                n += (item_slots[i].active ? 1 : 0);
            }
            return n;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;

            MainForm.data.op_queue.OpenCluster();
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                set_element_variant(current_element, i, 0, SFEngine.Utility.TryParseUInt16(textBox1.Text));
            }

            MainForm.data.op_queue.CloseCluster();
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
                if(GetItemsChecked() == 1)
                {
                    return;
                }

                int subelem_index = SFEngine.Utility.NO_INDEX;
                for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                {
                    Byte item_slot = (Byte)(category[current_element, i][1]);
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

                //remove unchecked element
                MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
                {
                    CategoryIndex = category.category_id,
                    ElementIndex = current_element,
                    SubElementIndex = subelem_index,
                    IsRemoving = true,
                    IsSubElement = true
                });
            }
            else
            {
                //add checked element
                int count = category.element_lists[current_element].Elements.Count;
                SFCategoryElement new_elem = category.GetEmptyElement();
                new_elem[0] = (UInt16)(category.element_lists[current_element].GetID());
                new_elem[1] = (Byte)flag;
                new_elem[2] = SFEngine.Utility.TryParseUInt16(item_slots[flag].text.Text);

                MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
                {
                    CategoryIndex = category.category_id,
                    ElementIndex = current_element,
                    SubElementIndex = count,
                    Element = new_elem,
                    IsSubElement = true
                });
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

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                if ((Byte)(category[current_element, i][1]) == item_slot)
                {
                    UInt16 id = SFEngine.Utility.TryParseUInt16(item_id.Text);

                    MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorModifyCategoryElement()
                    {
                        CategoryIndex = category.category_id,
                        ElementIndex = current_element,
                        SubElementIndex = i,
                        VariantIndex = 2,
                        NewVariant = id,
                        IsSubElement = true
                    });

                    return;
                }
            }
        }

        public override void set_element(int index)
        {
            current_element = index;
            edit_ready = false;

            for(int i = 0; i < 7; i++)
            {
                item_slots[i].set_checked(false);
            }

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                Byte item_slot = (Byte)(category[current_element, i][1]);
                UInt16 item_id = (UInt16)(category[current_element, i][2]);

                item_slots[item_slot].set_checked(true);
                item_slots[item_slot].set_text(item_id, SFCategoryManager.GetItemName(item_id));
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
            {
                step_into(textBox1, 2024);
            }
        }

        private void TextboxItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into((TextBox)sender, 2003);
            }
        }

        public override string get_element_string(int index)
        {
            UInt16 unit_id = (UInt16)category[index, 0][0];
            string txt_unit = SFCategoryManager.GetUnitName(unit_id);
            return unit_id.ToString() + " " + txt_unit;
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
    }
}
