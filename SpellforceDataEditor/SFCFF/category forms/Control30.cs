using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control30 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        bool is_clearing_table = false;
        Category2042 c2042;

        public Control30()
        {
            InitializeComponent();

            c2042 = SFCategoryManager.gamedata.c2042;
            category = c2042;

            column_dict.Add("Merchant ID", "MerchantID");
            column_dict.Add("Item ID", "ItemID");
            column_dict.Add("Item quantity", "Stock");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2042.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        public override void set_element(int index)
        {
            is_clearing_table = true;
            MerchantGrid.Rows.Clear();
            MerchantGrid.ClearSelection();
            MerchantGrid.Refresh();
            is_clearing_table = false;

            current_element = index;

            for (int i = 0; i < c2042.GetItemSubItemNum(current_element); i++)
            {
                UInt16 item_id = c2042[current_element, i].ItemID;
                UInt16 item_count = c2042[current_element, i].Stock;

                MerchantGrid.Rows.Add();
                MerchantGrid.Rows[i].Cells[0].Value = item_id;
                MerchantGrid.Rows[i].Cells[1].Value = item_count;
                MerchantGrid.Rows[i].Cells[2].Value = SFCategoryManager.GetItemName(item_id);
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = c2042[current_element, 0].MerchantID.ToString();
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
                if (!SFCategoryManager.gamedata.c2003.GetItemIndex(item_id, out int item_index))
                {
                    cell.Value = c2042[current_element, i].ItemID;
                }
                else
                {
                    if (item_id == c2042[current_element, i].ItemID)
                    {
                        cell.Value = c2042[current_element, i].ItemID;
                    }
                    else
                    {
                        c2042.SetField(current_element, i, "ItemID", item_id);
                    }
                }

            }
            else if (cell.ColumnIndex == 1)
            {
                UInt16 item_count = SFEngine.Utility.TryParseUInt16(cell.Value.ToString());
                if (item_count == c2042[current_element, i].Stock)
                {
                    cell.Value = c2042[current_element, i].Stock.ToString();
                }
                else
                {
                    c2042.SetField(current_element, i, "Stock", item_count);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MerchantGrid.ClearSelection();

            int new_index = MerchantGrid.Rows.Count;
            UInt16 item_id = SFEngine.Utility.TryParseUInt16(textBox2.Text);

            // if item already exists, add count to that item instead
            for (int i = 0; i < new_index; i++)
            {
                UInt16 current_item_id = c2042[current_element, i].ItemID;
                if (item_id == current_item_id)
                {
                    c2042.SetField(current_element, i, "Stock", (ushort)(c2042[current_element, i].Stock + 1));
                    return;
                }
            }

            c2042.AddSubItem(current_element, c2042.GetItemSubItemNum(current_element), new()
            {
                MerchantID = c2042[current_element, 0].MerchantID,
                ItemID = item_id,
                Stock = 1
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
            c2042.RemoveSub(current_element, selected);
        }


        public override string get_element_string(int index)
        {
            return $"{c2042[index, 0].MerchantID} {SFCategoryManager.GetMerchantName(c2042[index, 0].MerchantID)}";
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
                    textBox1.Text = c2042[current_element, 0].MerchantID.ToString();
                }
            }

            MerchantGrid.Rows[subelem_index].Cells[0].Value = c2042[current_element, subelem_index].ItemID;
            MerchantGrid.Rows[subelem_index].Cells[1].Value = c2042[current_element, subelem_index].Stock;
            MerchantGrid.Rows[subelem_index].Cells[2].Value = SFCategoryManager.GetItemName(c2042[current_element, subelem_index].ItemID);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2041, textBox1.Text);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2003, textBox2.Text);
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

                trace(2003, item_id);
            }
        }
    }
}
