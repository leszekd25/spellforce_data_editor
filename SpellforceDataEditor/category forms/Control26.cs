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
    public partial class Control26 : SpellforceDataEditor.category_forms.SFControl
    {
        private List<Byte> combo_values = new List<Byte>();

        public Control26()
        {
            InitializeComponent();
            column_dict.Add("Building ID", new int[1] { 0 });
            column_dict.Add("Resource type", new int[1] { 1 });
            column_dict.Add("Resource amount", new int[1] { 2 });
        }

        private void load_resources()
        {
            comboRes.Items.Clear();
            combo_values.Clear();
            combo_values.Add(0);    //default null value

            SFCategory cat_res = category.get_manager().get_category(31);
            int elem_count = cat_res.get_element_count();
            for(int i = 0; i < elem_count; i++)
            {
                SFCategoryElement elem = cat_res.get_element(i);
                string txt = category.get_text_from_element(elem, 1);
                comboRes.Items.Add(txt);
                combo_values.Add((Byte)elem.get_single_variant(0).value);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            for (int i = 0; i < elem_count; i++)
                set_element_variant(current_element, i * 3 + 0, Utility.TryParseUInt16(textBox1.Text));
        }


        private void RefreshListResources()
        {
            ListResources.Items.Clear();

            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            for (int i = 0; i < elem_count; i++)
            {
                int res_index = combo_values.IndexOf((Byte)elem.get_single_variant(i * 3 + 1).value);
                string res_name = "";
                if (res_index == 0)
                    res_name = Utility.S_NONE;
                else
                    res_name = comboRes.Items[res_index-1].ToString();    //-1 because of null value
                string elem_name = ((UInt16)elem.get_single_variant(i * 3 + 2).value).ToString() + " " + res_name;
                ListResources.Items.Add(elem_name);
            }
        }

        public override void set_element(int index)
        {
            current_element = index;

            load_resources();

            RefreshListResources();

            ListResources.SelectedIndex = 0;
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 23);
        }

        private void ListResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListResources.SelectedIndex == -1)
                return;

            int index = ListResources.SelectedIndex;

            comboRes.SelectedIndex = combo_values.IndexOf((Byte)category.get_element_variant(current_element, index * 3 + 1).value)-1;
            textBox3.Text = variant_repr(index * 3 + 2);
        }

        private void comboRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboRes.SelectedIndex == -1)
                return;

            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            int cur_index = ListResources.SelectedIndex;
            Byte current_res = (Byte)elem.get_single_variant(cur_index * 3 + 1).value;
            Byte new_res = combo_values[comboRes.SelectedIndex+1];
            if (current_res == new_res)
                return;

            // check if resource like this already exists
            for(int i = 0; i < elem_count; i++)
            {
                Byte res_id = (Byte)elem.get_single_variant(i * 3 + 1).value;
                if(res_id == new_res)
                {
                    new_res = 0;
                    break;
                }
            }

            // generate new element with reordered resources by resource id, ascending order
            object[] cur_data = elem.copy_raw(cur_index * 3, 3);
            elem.remove_raw(cur_index*3, 3);
            int new_index = elem_count-1;
            for (int i = 0; i < elem_count-1; i++)
            {
                if((Byte)elem.get_single_variant(i*3+1).value > new_res)
                {
                    new_index = i;
                    break;
                }
            }
            elem.paste_raw(cur_data, new_index * 3);
            elem.set_single_variant(new_index * 3 + 1, new_res);
            RefreshListResources();
            ListResources.SelectedIndex = new_index;
        }


        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int index = ListResources.SelectedIndex;
            set_element_variant(current_element, 3*index+2, Utility.TryParseUInt16(textBox3.Text));
            RefreshListResources();
            ListResources.SelectedIndex = index;
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

            RefreshListResources();
            ListResources.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListResources.Items.Count == 1)
                return;

            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            int index = ListResources.SelectedIndex;

            elem.remove_raw(index * 3, 3);

            RefreshListResources();
            ListResources.SelectedIndex = Math.Max(index, ListResources.Items.Count - 1);
        }
    }
}
