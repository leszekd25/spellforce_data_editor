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
    public partial class Control23 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control23()
        {
            InitializeComponent();
            column_dict.Add("Unit ID", new int[1] { 0 });
            column_dict.Add("Requirement index", new int[1] { 1 });
            column_dict.Add("Building ID", new int[1] { 2 });
        }

        private void set_list_text(int i)
        {
            UInt16 spell_id = (UInt16)(category.get_element_variant(current_element, i * 3 + 2)).value;

            string txt = SFCategoryManager.get_effect_name(spell_id, true);
            ListBuildings.Items[i] = txt;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            for (int i = 0; i < elem_count; i++)
                set_element_variant(current_element, i * 3 + 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListBuildings.SelectedIndex;
            if (cur_selected < 0)
                return;
            set_element_variant(current_element, cur_selected * 3 + 2, Utility.TryParseUInt16(textBox3.Text));
            set_list_text(cur_selected);
        }

        public override void set_element(int index)
        {
            current_element = index;

            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            ListBuildings.Items.Clear();

            for (int i = 0; i < elem_count; i++)
            {
                Byte spell_order = (Byte)(elem.get_single_variant(i * 3 + 1)).value;
                UInt16 building_id = (UInt16)(elem.get_single_variant(i * 3 + 2)).value;

                string txt = SFCategoryManager.get_building_name(building_id);

                ListBuildings.Items.Add(txt);
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
        }

        private void ListBuildings_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListBuildings.SelectedIndex;
            if (cur_selected < 0)
                return;
            textBox3.Text = variant_repr(cur_selected * 3 + 2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int new_index;
            if (ListBuildings.SelectedIndex == -1)
                new_index = ListBuildings.Items.Count - 1;
            else
                new_index = ListBuildings.SelectedIndex;

            SFCategoryElement elem = category.get_element(current_element);
            int cur_elem_count = elem.get().Count / 3;

            Byte max_index = 0;
            for (int i = 0; i < cur_elem_count; i++)
            {
                max_index = Math.Max(max_index, (Byte)(elem.get_single_variant(i * 3 + 1).value));
            }

            int offset = 0;
            SFCategoryElement new_elem = new SFCategoryElement();
            Object[] obj_array = new Object[(cur_elem_count + 1) * 3];
            for (int i = 0; i < cur_elem_count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    obj_array[(i + offset) * 3 + j] = elem.get_single_variant(i * 3 + j).value;
                }
                if (i == new_index)
                {
                    offset = 1;
                    obj_array[(i + offset) * 3 + 0] = (UInt16)elem.get_single_variant(0).value;
                    obj_array[(i + offset) * 3 + 1] = max_index;
                    obj_array[(i + offset) * 3 + 2] = (UInt16)0;
                }
            }
            new_elem.set(obj_array);
            category.get_elements()[current_element] = new_elem;
            set_element(current_element);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListBuildings.SelectedIndex == -1)
                return;
            if (ListBuildings.Items.Count == 1)
                return;
            int new_index = ListBuildings.SelectedIndex;

            SFCategoryElement elem = category.get_element(current_element);
            int cur_elem_count = elem.get().Count / 3;

            Byte cur_spell_index = (Byte)(elem.get_single_variant(new_index * 3 + 1).value);

            int offset = 0;
            SFCategoryElement new_elem = new SFCategoryElement();
            Object[] obj_array = new Object[(cur_elem_count - 1) * 3];
            for (int i = 0; i < cur_elem_count; i++)
            {
                if (i == new_index)
                {
                    offset = 1;
                    continue;
                }
                for (int j = 0; j < 3; j++)
                {
                    obj_array[(i - offset) * 3 + j] = elem.get_single_variant(i * 3 + j).value;
                }
                if ((Byte)(elem.get_single_variant(i * 3 + 1).value) > cur_spell_index)
                    elem.set_single_variant(i * 3 + 1, (Byte)(elem.get_single_variant(i * 3 + 1).value) - (Byte)1);
            }
            new_elem.set(obj_array);
            category.get_elements()[current_element] = new_elem;
            set_element(current_element);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox3, 23);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 17);
        }
    }
}
