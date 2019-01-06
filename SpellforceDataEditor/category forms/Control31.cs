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
    public partial class Control31 : SpellforceDataEditor.category_forms.SFControl
    {
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

            SFCategory cat_res = SFCategoryManager.get_category(31);
            int elem_count = SFCategory7.item_types.Length;
            for (int i = 1; i < elem_count; i++)
            {
                comboItemType.Items.Add(SFCategory7.item_types[i]);
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            for (int i = 0; i < elem_count; i++)
                set_element_variant(current_element, i * 3 + 0, Utility.TryParseUInt16(textBox5.Text));
        }


        private void RefreshListItemTypes()
        {
            ListItemTypes.Items.Clear();

            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            for (int i = 0; i < elem_count; i++)
            {
                int res_index = (int)(Byte)elem.get_single_variant(i * 3 + 1).value;
                string res_name = "";
                if (res_index == 0)
                    res_name = Utility.S_NONE;
                else
                    res_name = comboItemType.Items[res_index - 1].ToString();    //-1 because of null value
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
            textBox5.Text = variant_repr(0);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox5, 23);
        }

        private void ListItemTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListItemTypes.SelectedIndex == -1)
                return;

            int index = ListItemTypes.SelectedIndex;

            comboItemType.SelectedIndex = (Byte)category.get_element_variant(current_element, index * 3 + 1).value - 1;
            textBox4.Text = variant_repr(index * 3 + 2);
        }

        private void comboItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboItemType.SelectedIndex == -1)
                return;

            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            int cur_index = ListItemTypes.SelectedIndex;
            Byte current_res = (Byte)elem.get_single_variant(cur_index * 3 + 1).value;
            Byte new_res = (Byte)(comboItemType.SelectedIndex + 1);
            if (current_res == new_res)
                return;

            // check if resource like this already exists
            for (int i = 0; i < elem_count; i++)
            {
                Byte res_id = (Byte)elem.get_single_variant(i * 3 + 1).value;
                if (res_id == new_res)
                {
                    new_res = 0;
                    break;
                }
            }

            // generate new element with reordered resources by resource id, ascending order
            object[] cur_data = elem.copy_raw(cur_index * 3, 3);
            elem.remove_raw(cur_index * 3, 3);
            int new_index = elem_count - 1;
            for (int i = 0; i < elem_count - 1; i++)
            {
                if ((Byte)elem.get_single_variant(i * 3 + 1).value > new_res)
                {
                    new_index = i;
                    break;
                }
            }
            elem.paste_raw(cur_data, new_index * 3);
            elem.set_single_variant(new_index * 3 + 1, new_res);
            RefreshListItemTypes();
            ListItemTypes.SelectedIndex = new_index;
        }


        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int index = ListItemTypes.SelectedIndex;
            set_element_variant(current_element, 3 * index + 2, Utility.TryParseUInt16(textBox4.Text));
            RefreshListItemTypes();
            ListItemTypes.SelectedIndex = index;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            object[] paste_data = new object[3];
            paste_data[0] = (UInt16)elem.get_single_variant(0).value;
            paste_data[1] = (Byte)0;
            paste_data[2] = (UInt16)0;

            elem.paste_raw(paste_data, 0);

            RefreshListItemTypes();
            ListItemTypes.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListItemTypes.Items.Count == 1)
                return;

            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            int index = ListItemTypes.SelectedIndex;

            elem.remove_raw(index * 3, 3);

            RefreshListItemTypes();
            ListItemTypes.SelectedIndex = Math.Max(index, ListItemTypes.Items.Count - 1);
        }
    }
}
