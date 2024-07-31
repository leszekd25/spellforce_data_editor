using SFEngine;
using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control13 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2012 c2012;

        public Control13()
        {
            InitializeComponent();

            c2012 = SFCategoryManager.gamedata.c2012;
            category = c2012;

            column_dict.Add("Item ID", "ItemID");
            column_dict.Add("Item UI index", "UIIndex");
            column_dict.Add("Item UI handle", "UIHandle");
            column_dict.Add("Scaled down?", "IsScaledDown");
        }

        private void set_list_text(int i)
        {
            Byte ui_index = c2012[current_element, i].UIIndex;

            ListUI.Items[i] = ui_index.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2012.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            int cur_selected = ListUI.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            c2012.SetField(current_element, cur_selected, "IsScaledDown", (checkBox1.Checked ? (UInt16)1 : (UInt16)0));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListUI.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            c2012.SetField(current_element, cur_selected, "UIHandle", StringUtils.FromString(textBox4.Text, 0, 64));
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListUI.Items.Clear();

            for (int i = 0; i < c2012.GetItemSubItemNum(current_element); i++)
            {
                ListUI.Items.Add("");
                set_list_text(i);
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = c2012[current_element, 0].ItemID.ToString();
        }

        private void ListUI_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListUI.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            textBox4.Text = c2012[current_element, cur_selected].GetHandleString();
            checkBox1.Checked = (c2012[current_element, cur_selected].IsScaledDown == 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Category2012Item item = new Category2012Item();
            c2012.GetID(current_element, out int id);
            item.SetID(id);

            // add new subelement, id being the lowest not occuring subelement
            byte new_subid;
            for(new_subid = 1; ;new_subid++)
            {
                bool found = false;
                for(int i = 0; i < c2012.GetItemSubItemNum(current_element); i++)
                {
                    if (c2012[current_element, i].GetSubID() == new_subid)
                    {
                        found = true;
                        break;
                    }
                }
                if(!found)
                {
                    break;
                }
            }
            int new_index = new_subid - 1;
            item.UIIndex = new_subid;

            c2012.AddSubItem(current_element, new_index, item);
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
            byte cur_ui_index = c2012[current_element, new_index].UIIndex;
            c2012.RemoveSub(current_element, new_index);

            for (int i = 0; i < c2012.GetItemSubItemNum(current_element); i++)
            {
                if (c2012[current_element, i].UIIndex > cur_ui_index)
                {
                    c2012.SetField(current_element, i, "UIIndex", (byte)(c2012[current_element, i].UIIndex - 1));
                }
            }
        }


        public override string get_element_string(int index)
        {
            UInt16 item_id = c2012[index, 0].ItemID;
            return $"{item_id} {SFCategoryManager.GetItemName(item_id)}";
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
                textBox4.Text = c2012[current_element, subelem_index].GetHandleString();
                checkBox1.Checked = (c2012[current_element, subelem_index].IsScaledDown == 1);
            }
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2003, textBox1.Text);
        }
    }
}
