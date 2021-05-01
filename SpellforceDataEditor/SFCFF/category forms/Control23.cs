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
    public partial class Control23 : SpellforceDataEditor.SFCFF.category_forms.SFControl
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
            UInt16 building_id = (UInt16)(category[current_element][i * 3 + 2]);

            string txt = SFCategoryManager.GetBuildingName(building_id);
            ListBuildings.Items[i] = txt;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;

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

            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;

            ListBuildings.Items.Clear();

            for (int i = 0; i < elem_count; i++)
            {
                Byte spell_order = (Byte)(elem[i * 3 + 1]);
                UInt16 building_id = (UInt16)(elem[i * 3 + 2]);

                string txt = SFCategoryManager.GetBuildingName(building_id);

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
            if (ListBuildings.SelectedIndex == Utility.NO_INDEX)
                new_index = ListBuildings.Items.Count - 1;
            else
                new_index = ListBuildings.SelectedIndex;

            SFCategoryElement elem = category[current_element];
            int cur_elem_count = elem.variants.Count / 3;

            Byte max_index = 0;
            for (int i = 0; i < cur_elem_count; i++)
            {
                max_index = Math.Max(max_index, (Byte)(elem[i * 3 + 1]));
            }
            max_index += 1;

            object[] paste_data = new object[3];
            paste_data[0] = (UInt16)elem[0];
            paste_data[1] = (Byte)max_index;
            paste_data[2] = (UInt16)0;

            elem.PasteRaw(paste_data, new_index * 3);

            set_element(current_element);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListBuildings.SelectedIndex == Utility.NO_INDEX)
                return;
            if (ListBuildings.Items.Count == 1)
                return;
            int new_index = ListBuildings.SelectedIndex;

            SFCategoryElement elem = category[current_element];
            Byte cur_spell_index = (Byte)(elem[new_index * 3 + 1]);

            elem.RemoveRaw(new_index * 3, 3);

            int cur_elem_count = elem.variants.Count / 3;
            for (int i = 0; i < cur_elem_count; i++)
                if ((Byte)(elem[i * 3 + 1]) > cur_spell_index)
                    elem[i * 3 + 1] = (Byte)((Byte)(elem[i * 3 + 1]) - 1);

            set_element(current_element);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox3, 2029);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 2024);
        }


        public override string get_element_string(int index)
        {
            UInt16 unit_id = (UInt16)category[index][0];
            string txt_unit = SFCategoryManager.GetUnitName(unit_id);
            return unit_id.ToString() + " " + txt_unit;
        }
    }
}
