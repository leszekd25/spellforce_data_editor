using SFEngine.SFCFF;
using System;
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
            MainForm.data.op_queue.OpenCluster();
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                set_element_variant(current_element, i, 0, SFEngine.Utility.TryParseUInt16(textBox1.Text));
            }

            MainForm.data.op_queue.CloseCluster();
        }

        public override void set_element(int index)
        {
            is_clearing_table = true;
            MerchantGrid.Rows.Clear();
            MerchantGrid.ClearSelection();
            MerchantGrid.Refresh();
            is_clearing_table = false;

            current_element = index;

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                UInt16 item_id = (UInt16)(category[current_element, i][1]);
                UInt16 item_count = (UInt16)(category[current_element, i][2]);

                MerchantGrid.Rows.Add();
                MerchantGrid.Rows[i].Cells[0].Value = item_id;
                MerchantGrid.Rows[i].Cells[1].Value = item_count;

                if (SFCategoryManager.gamedata[2003] == null)
                {
                    MerchantGrid.Rows[i].Cells[2].Value = SFEngine.Utility.S_MISSING;
                }
                else
                {
                    SFCategoryElement item_elem = SFCategoryManager.gamedata[2003].FindElementBinary<UInt16>(0, item_id);
                    if (item_elem == null)
                    {
                        MerchantGrid.Rows[i].Cells[2].Value = SFEngine.Utility.S_NONAME;
                    }
                    else
                    {
                        MerchantGrid.Rows[i].Cells[2].Value = SFCategoryManager.GetItemName(item_id);
                    }
                }
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0, 0);
        }

        private void OnCellValueChange(object sender, DataGridViewCellEventArgs e)
        {
            if (is_clearing_table)
            {
                return;
            }

            if (MerchantGrid.CurrentCell == null)
            {
                return;
            }

            DataGridViewCell cell = (DataGridViewCell)MerchantGrid.CurrentCell;

            if (cell == null)
            {
                return;
            }

            int i = cell.RowIndex;

            if (cell.ColumnIndex == 0)
            {
                UInt16 item_id = SFEngine.Utility.TryParseUInt16(cell.Value.ToString());
                if (SFCategoryManager.gamedata[2003] == null)
                {
                    cell.Value = variant_repr(i, 1);
                }
                else
                {
                    SFCategoryElement item_elem = SFCategoryManager.gamedata[2003].FindElementBinary(0, item_id);
                    if (item_elem == null)
                    {
                        cell.Value = variant_repr(i, 1);
                    }
                    else
                    {
                        if (item_id == (short)(category[current_element, i][1]))
                        {
                            cell.Value = variant_repr(i, 1);
                        }
                        else
                        {
                            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorModifyCategoryElement()
                            {
                                CategoryIndex = category.category_id,
                                ElementIndex = current_element,
                                SubElementIndex = i,
                                VariantIndex = 1,
                                NewVariant = item_id,
                                IsSubElement = true
                            });
                        }
                    }
                }
            }
            else if (cell.ColumnIndex == 1)
            {
                UInt16 item_count = SFEngine.Utility.TryParseUInt16(cell.Value.ToString());
                if (item_count == (short)(category[current_element, i][2]))
                {
                    cell.Value = variant_repr(i, 2);
                }
                else
                {
                    MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorModifyCategoryElement()
                    {
                        CategoryIndex = category.category_id,
                        ElementIndex = current_element,
                        SubElementIndex = i,
                        VariantIndex = 2,
                        NewVariant = item_count,
                        IsSubElement = true
                    });
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MerchantGrid.ClearSelection();
            SFCategoryElement elem = category[current_element, 0];

            int new_index = MerchantGrid.Rows.Count;
            UInt16 item_id = SFEngine.Utility.TryParseUInt16(textBox2.Text);

            // if item already exists, add count to that item instead
            for (int i = 0; i < new_index; i++)
            {
                UInt16 current_item_id = (UInt16)(category[current_element, i][1]);
                if (item_id == current_item_id)
                {
                    MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorModifyCategoryElement()
                    {
                        CategoryIndex = category.category_id,
                        ElementIndex = current_element,
                        SubElementIndex = i,
                        VariantIndex = 2,
                        NewVariant = (UInt16)((UInt16)(category[current_element, i][2]) + 1),
                        IsSubElement = true
                    });
                    return;
                }
            }

            // add new element
            SFCategoryElement new_elem = category.GetEmptyElement();
            new_elem[0] = (UInt16)(category[current_element, 0][0]);
            new_elem[1] = item_id;
            new_elem[2] = (UInt16)1;
            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = category.element_lists[current_element].Elements.Count,
                Element = new_elem,
                IsSubElement = true
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MerchantGrid.SelectedCells.Count == 0)
            {
                return;
            }

            if (MerchantGrid.Rows.Count == 1)
            {
                return;
            }

            int selected = MerchantGrid.SelectedCells[0].RowIndex;

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = selected,
                IsRemoving = true,
                IsSubElement = true
            });
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox1, 2041);
            }
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox2, 2024);
            }
        }

        private void MerchantGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int CurrentColumnMouseOver = MerchantGrid.HitTest(e.X, e.Y).ColumnIndex;
                if (CurrentColumnMouseOver != 0)
                {
                    return;
                }

                int CurrentRowMouseOver = MerchantGrid.HitTest(e.X, e.Y).RowIndex;

                DataGridViewCell cell = MerchantGrid[CurrentColumnMouseOver, CurrentRowMouseOver];
                int item_id = SFEngine.Utility.TryParseInt32(cell.Value.ToString());

                step_into(2003, item_id);
            }
        }


        public override string get_element_string(int index)
        {
            UInt16 merchant_id = (UInt16)category[index, 0][0];
            string txt_merchant = SFCategoryManager.GetMerchantName(merchant_id);
            return merchant_id.ToString() + " " + txt_merchant;
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
            if (MerchantGrid.SelectedCells.Count != 0)
            {
                int selected = MerchantGrid.SelectedCells[0].RowIndex;

                if (selected == subelem_index)
                {
                    textBox1.Text = variant_repr(subelem_index, 0);
                }
            }

            MerchantGrid.Rows[subelem_index].Cells[0].Value = (UInt16)(category[current_element, subelem_index][1]);
            MerchantGrid.Rows[subelem_index].Cells[1].Value = (UInt16)(category[current_element, subelem_index][2]);
            SFCategoryElement item_elem = SFCategoryManager.gamedata[2003].FindElementBinary<UInt16>(0, (UInt16)(category[current_element, subelem_index][1]));
            if (item_elem == null)
            {
                MerchantGrid.Rows[subelem_index].Cells[2].Value = SFEngine.Utility.S_NONAME;
            }
            else
            {
                MerchantGrid.Rows[subelem_index].Cells[2].Value = SFCategoryManager.GetItemName((UInt16)(category[current_element, subelem_index][1]));
            }
        }
    }
}
