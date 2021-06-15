using System;
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
    public partial class Control21 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        private List<Byte> combo_values = new List<Byte>();

        public Control21()
        {
            InitializeComponent();
            column_dict.Add("Unit ID", new int[1] { 0 });
            column_dict.Add("Resource type", new int[1] { 1 });
            column_dict.Add("Resource amount", new int[1] { 2 });
        }

        private void load_resources()
        {
            comboRes.Items.Clear();
            combo_values.Clear();
            combo_values.Add(0);    //default null value

            SFCategory cat_res = SFCategoryManager.gamedata[2044];
            int elem_count = cat_res.GetElementCount();
            for (int i = 0; i < elem_count; i++)
            {
                SFCategoryElement elem = cat_res[i];
                string txt = SFCategoryManager.GetTextFromElement(elem, 1);
                comboRes.Items.Add(txt);
                combo_values.Add((Byte)elem[0]);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                set_element_variant(current_element, i, 0, Utility.TryParseUInt16(textBox1.Text));
        }


        private void RefreshListResources()
        {
            ListResources.Items.Clear();

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                int res_index = combo_values.IndexOf((Byte)category[current_element, i][1]);
                string res_name = "";
                if (res_index == 0)
                    res_name = Utility.S_NONE;
                else
                    res_name = comboRes.Items[res_index - 1].ToString();    //-1 because of null value
                string elem_name = ((Byte)category[current_element, i][2]).ToString() + " " + res_name;
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
            textBox1.Text = variant_repr(0, 0);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 2024);
        }

        private void ListResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListResources.SelectedIndex == Utility.NO_INDEX)
                return;

            int index = ListResources.SelectedIndex;

            comboRes.SelectedIndex = combo_values.IndexOf((Byte)category[current_element, index][1]) - 1;
            textBox3.Text = variant_repr(index, 2);
        }

        private void comboRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboRes.SelectedIndex == Utility.NO_INDEX)
                return;


            int cur_index = ListResources.SelectedIndex;
            Byte current_res = (Byte)category[current_element, cur_index][1];
            Byte new_res = combo_values[comboRes.SelectedIndex + 1];
            if (current_res == new_res)
                return;

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

            // generate new element with reordered resources by resource id, ascending order
            SFCategoryElement elem = category[current_element, cur_index];
            category.element_lists[current_element].Elements.RemoveAt(cur_index);

            int new_index = category.element_lists[current_element].Elements.Count - 1;
            for(int i = 0; i < category.element_lists[current_element].Elements.Count - 1; i++)
            {
                if((Byte)category[current_element, i][1] > new_res)
                {
                    new_index = i;
                    break;
                }
            }

            category.element_lists[current_element].Elements.Insert(new_index, elem);
            elem[1] = new_res;

            RefreshListResources();
            ListResources.SelectedIndex = new_index;
        }


        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int index = ListResources.SelectedIndex;
            set_element_variant(current_element, index, 2, Utility.TryParseUInt8(textBox3.Text));
            RefreshListResources();
            ListResources.SelectedIndex = index;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            category.element_lists[current_element].Elements.Insert(0, category.GetEmptyElement());
            category[current_element, 0][0] = (UInt16)category[current_element, 0][1];
            category[current_element, 0][1] = (Byte)0;
            category[current_element, 0][2] = (Byte)0;

            RefreshListResources();
            ListResources.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListResources.Items.Count == 1)
                return;

            int index = ListResources.SelectedIndex;
            category.element_lists[current_element].Elements.RemoveAt(index);

            RefreshListResources();
            ListResources.SelectedIndex = Math.Max(index, ListResources.Items.Count - 1);
        }


        public override string get_element_string(int index)
        {
            UInt16 unit_id = (UInt16)category[index, 0][0];
            string txt_unit = SFCategoryManager.GetUnitName(unit_id);
            return unit_id.ToString() + " " + txt_unit;
        }
    }
}
