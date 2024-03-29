﻿using SFEngine.SFCFF;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control31 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        static string[] item_types = { SFEngine.Utility.S_UNKNOWN, "Equipment", "Inventory rune", "Installed rune",
            "Spell scroll", "Equipped scroll", "Unit plan", "Building plan", "Equipped unit plan",
            "Equipped building plan", "Miscellaneous" };

        public Control31()
        {
            InitializeComponent();
            column_dict.Add("Merchant ID", new int[1] { 0 });
            column_dict.Add("Item type", new int[1] { 1 });
            column_dict.Add("Price multiplier", new int[1] { 2 });
        }

        private void load_item_types()
        {
            comboItemType.Items.Clear();

            int elem_count = item_types.Length;
            for (int i = 1; i < elem_count; i++)
            {
                comboItemType.Items.Add(item_types[i]);
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            MainForm.data.op_queue.OpenCluster();
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                set_element_variant(current_element, i, 0, SFEngine.Utility.TryParseUInt16(textBox5.Text));
            }

            MainForm.data.op_queue.CloseCluster();
        }


        private void RefreshListItemTypes()
        {
            ListItemTypes.Items.Clear();

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                int res_index = (int)(Byte)category[current_element, i][1];
                string res_name = "";
                if (res_index == 0)
                {
                    res_name = SFEngine.Utility.S_NONE;
                }
                else
                {
                    res_name = comboItemType.Items[res_index - 1].ToString();    //-1 because of null value
                }

                ListItemTypes.Items.Add(res_name);
            }
        }

        public override void set_element(int index)
        {
            current_element = index;

            load_item_types();

            RefreshListItemTypes();

            ListItemTypes.SelectedIndex = 0;
        }

        public override void show_element()
        {
            textBox5.Text = variant_repr(0, 0);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox5, 2029);
            }
        }

        private void ListItemTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListItemTypes.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int index = ListItemTypes.SelectedIndex;

            comboItemType.SelectedIndex = (Byte)category[current_element, index][1] - 1;
            textBox4.Text = variant_repr(index, 2);
        }

        private void comboItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboItemType.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int cur_index = ListItemTypes.SelectedIndex;
            Byte current_res = (Byte)category[current_element, cur_index][1];
            Byte new_res = (Byte)(comboItemType.SelectedIndex + 1);
            if (current_res == new_res)
            {
                return;
            }

            // check if resource like this already exists
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                Byte res_id = (Byte)category[current_element, i][1];
                if (res_id == new_res)
                {
                    new_res = 0;
                    break;
                }
            }

            // generate new element with reordered resources by resource id, ascending orderMainForm.data.op_queue.OpenCluster();
            SFCategoryElement elem = category[current_element, cur_index];

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = cur_index,
                IsSubElement = true,
                IsRemoving = true,
            });

            int new_index = category.element_lists[current_element].Elements.Count - 1;
            for (int i = 0; i < category.element_lists[current_element].Elements.Count - 1; i++)
            {
                if ((Byte)category[current_element, i][1] > new_res)
                {
                    new_index = i;
                    break;
                }
            }

            SFCategoryElement new_elem = elem.GetCopy();
            new_elem[1] = new_res;

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = new_index,
                Element = new_elem,
                IsSubElement = true
            });
            MainForm.data.op_queue.CloseCluster();

            ListItemTypes.SelectedIndex = new_index;
        }


        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int index = ListItemTypes.SelectedIndex;
            set_element_variant(current_element, index, 2, SFEngine.Utility.TryParseUInt16(textBox4.Text));

            ListItemTypes.SelectedIndex = index;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SFCategoryElement new_elem = category.GetEmptyElement();
            new_elem[0] = (UInt16)category[current_element, 0][0];

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = 0,
                Element = new_elem,
                IsSubElement = true
            });

            ListItemTypes.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListItemTypes.Items.Count == 1)
            {
                return;
            }

            int index = ListItemTypes.SelectedIndex;
            if (index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = index,
                IsRemoving = true,
                IsSubElement = true
            });

            ListItemTypes.SelectedIndex = Math.Min(index, ListItemTypes.Items.Count - 1);
        }


        public override string get_element_string(int index)
        {
            UInt16 merchant_id = (UInt16)category[index, 0][0];
            string txt_merchant = SFCategoryManager.GetMerchantName(merchant_id);
            return merchant_id.ToString() + " " + txt_merchant;
        }

        public override void on_add_subelement(int subelem_index)
        {
            RefreshListItemTypes();
        }

        public override void on_remove_subelement(int subelem_index)
        {
            RefreshListItemTypes();
        }

        public override void on_update_subelement(int subelem_index)
        {
            RefreshListItemTypes();
        }
    }
}
