using SFEngine.SFCFF;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control13 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control13()
        {
            InitializeComponent();
            column_dict.Add("Item ID", new int[1] { 0 });
            column_dict.Add("Item UI index", new int[1] { 1 });
            column_dict.Add("Item UI handle", new int[1] { 2 });
            column_dict.Add("Scaled down?", new int[1] { 3 });
        }

        private void set_list_text(int i)
        {
            Byte ui_index = (Byte)category[current_element, i][1];

            ListUI.Items[i] = ui_index.ToString();
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

        private void checkBox1_Click(object sender, EventArgs e)
        {
            int cur_selected = ListUI.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            set_element_variant(current_element, cur_selected, 3, (checkBox1.Checked ? (UInt16)1 : (UInt16)0));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListUI.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            set_element_variant(current_element, cur_selected, 2, SFString.FromString(textBox4.Text, 0, 64));
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListUI.Items.Clear();

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                ListUI.Items.Add("");
                set_list_text(i);
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0, 0);
        }

        private void ListUI_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListUI.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            textBox4.Text = string_repr(cur_selected, 2);
            checkBox1.Checked = ((UInt16)(category[current_element, cur_selected][3])) == 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int new_index;
            if (ListUI.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                new_index = ListUI.Items.Count - 1;
            }
            else
            {
                new_index = ListUI.SelectedIndex;
            }

            Byte max_index = 1;
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                max_index = Math.Max(max_index, (Byte)(category[current_element, i][1]));
            }
            max_index += 1;

            SFCategoryElement new_elem = category.GetEmptyElement();
            new_elem[0] = (UInt16)(category[current_element, 0][0]);
            new_elem[1] = (Byte)max_index;
            new_elem[2] = SFString.FromString("", 0, 64);

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = new_index,
                Element = new_elem,
                IsSubElement = true
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListUI.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            if (ListUI.Items.Count == 1)
            {
                return;
            }

            int new_index = ListUI.SelectedIndex;

            Byte cur_spell_index = (Byte)(category[current_element, new_index][1]);

            MainForm.data.op_queue.OpenCluster();
            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = new_index,
                IsRemoving = true,
                IsSubElement = true
            });

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                if ((Byte)(category[current_element, i][1]) > cur_spell_index)
                {
                    MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorModifyCategoryElement()
                    {
                        CategoryIndex = category.category_id,
                        ElementIndex = current_element,
                        SubElementIndex = i,
                        VariantIndex = 1,
                        NewVariant = (Byte)((Byte)(category[current_element, i][1]) - 1),
                        IsSubElement = true
                    });
                }
            }

            MainForm.data.op_queue.CloseCluster();
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox1, 2003);
            }
        }


        public override string get_element_string(int index)
        {
            UInt16 item_id = (UInt16)category[index, 0][0];
            string txt_item = SFCategoryManager.GetItemName(item_id);
            return item_id.ToString() + " " + txt_item;
        }

        public override void on_add_subelement(int subelem_index)
        {
            ListUI.Items.Insert(subelem_index, "");
            set_list_text(subelem_index);
        }

        public override void on_remove_subelement(int subelem_index)
        {
            ListUI.Items.RemoveAt(subelem_index);
        }

        public override void on_update_subelement(int subelem_index)
        {
            set_list_text(subelem_index);
            if (ListUI.SelectedIndex == subelem_index)
            {
                textBox4.Text = string_repr(subelem_index, 2);
                checkBox1.Checked = ((UInt16)(category[current_element, subelem_index][3])) == 1;
            }
        }
    }
}
