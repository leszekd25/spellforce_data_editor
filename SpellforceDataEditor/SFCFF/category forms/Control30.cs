﻿using System;
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
    public partial class Control30 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        bool is_clearing_table = false;

        public Control30()
        {
            InitializeComponent();
            //how to deal with this one?
            column_dict.Add("Merchant ID", new int[1] { 0 });
            column_dict.Add("Item ID", new int[1] { 1 });
            column_dict.Add("Item quantity", new int[1] { 2 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;

            for (int i = 0; i < elem_count; i++)
                set_element_variant(current_element, 0 + 3 * i, Utility.TryParseUInt16(textBox1.Text));
        }

        public override void set_element(int index)
        {
            is_clearing_table = true;
            MerchantGrid.Rows.Clear();
            MerchantGrid.ClearSelection();
            MerchantGrid.Refresh();
            is_clearing_table = false;

            current_element = index;

            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;

            for (int i = 0; i < elem_count; i++)
            {
                UInt16 item_id = (UInt16)(elem[i * 3 + 1]);
                UInt16 item_count = (UInt16)(elem[i * 3 + 2]);

                MerchantGrid.Rows.Add();
                MerchantGrid.Rows[i].Cells[0].Value = item_id;
                MerchantGrid.Rows[i].Cells[1].Value = item_count;

                SFCategoryElement item_elem = SFCategoryManager.gamedata[2003].FindElementBinary<UInt16>(0, item_id);
                if (item_elem == null)
                {
                    MerchantGrid.Rows[i].Cells[2].Value = "<no name>";
                }
                else
                {
                    MerchantGrid.Rows[i].Cells[2].Value = SFCategoryManager.GetItemName(item_id);
                }
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
        }

        private void OnCellValueChange(object sender, DataGridViewCellEventArgs e)
        {
            if (is_clearing_table)
                return;

            if (MerchantGrid.CurrentCell == null)
                return;

            DataGridViewCell cell = (DataGridViewCell)MerchantGrid.CurrentCell;

            if (cell == null)
                return;


            int i = cell.RowIndex;
            
            if(cell.ColumnIndex == 0)
            {
                UInt16 item_id = Utility.TryParseUInt16(cell.Value.ToString());
                SFCategoryElement item_elem = SFCategoryManager.gamedata[2003].FindElementBinary(0, item_id);
                if (item_elem == null)
                {
                    cell.Value = variant_repr(i * 3 + 1);
                }
                else
                {
                    set_element_variant(current_element, i * 3 + 1, item_id);
                    MerchantGrid.Rows[i].Cells[2].Value = SFCategoryManager.GetItemName(item_id);
                }
            }
            else if(cell.ColumnIndex == 1)
            {
                UInt16 item_count = Utility.TryParseUInt16(cell.Value.ToString());
                set_element_variant(current_element, i * 3 + 2, item_count);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MerchantGrid.ClearSelection();
            SFCategoryElement elem = category[current_element];

            int new_index = MerchantGrid.Rows.Count;
            UInt16 item_id = Utility.TryParseUInt16(textBox2.Text);

            for(int i = 0; i < new_index; i++)
            {
                UInt16 current_item_id = (UInt16)(elem[i * 3 + 1]);
                if(item_id == current_item_id)
                {
                    elem[i * 3 + 2] = (UInt16)((UInt16)(elem[i * 3 + 2]) + 1);
                    MerchantGrid.Rows[i].Cells[1].Value = (UInt16)(elem[i * 3 + 2]);
                    return;
                }
            }

            //add new element
            SFCategoryElement new_elem = new SFCategoryElement();
            Object[] obj_array = new Object[(new_index + 1) * 3];
            for (int i = 0; i < new_index; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    obj_array[i *3 + j] = elem[i * 3 + j];
                }
            }
            obj_array[new_index * 3 + 0] = (UInt16)elem[0];
            obj_array[new_index * 3 + 1] = Utility.TryParseUInt16(textBox2.Text);
            obj_array[new_index * 3 + 2] = (UInt16)1;

            new_elem.AddVariants(obj_array);
            category[current_element] = new_elem;
            set_element(current_element);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MerchantGrid.SelectedCells.Count == 0)
                return;
            if (MerchantGrid.Rows.Count == 1)
                return;
            int selected = MerchantGrid.SelectedCells[0].RowIndex;

            SFCategoryElement elem = category[current_element];
            int cur_elem_count = elem.variants.Count / 3;
            int offset = 0;
            SFCategoryElement new_elem = new SFCategoryElement();
            Object[] obj_array = new Object[(cur_elem_count - 1) * 3];
            for (int i = 0; i < cur_elem_count; i++)
            {
                if (i == selected)
                {
                    offset = 1;
                    continue;
                }
                for (int j = 0; j < 3; j++)
                {
                    obj_array[(i - offset) * 3 + j] = elem[i * 3 + j];
                }
            }
            new_elem.AddVariants(obj_array);
            category[current_element] = new_elem;
            set_element(current_element);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 2041);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox2, 2024);
        }

        private void MerchantGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int CurrentColumnMouseOver = MerchantGrid.HitTest(e.X, e.Y).ColumnIndex;
                if (CurrentColumnMouseOver != 0)
                    return;

                int CurrentRowMouseOver = MerchantGrid.HitTest(e.X, e.Y).RowIndex;

                DataGridViewCell cell = MerchantGrid[CurrentColumnMouseOver, CurrentRowMouseOver];
                int item_id = Utility.TryParseInt32(cell.Value.ToString());

                step_into(2003, item_id);
            }
        }


        public override string get_element_string(int index)
        {
            UInt16 merchant_id = (UInt16)category[index][0];
            string txt_merchant = SFCategoryManager.GetMerchantName(merchant_id);
            return merchant_id.ToString() + " " + txt_merchant;
        }
    }
}
