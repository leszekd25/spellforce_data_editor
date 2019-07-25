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
    public partial class Control11 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control11()
        {
            InitializeComponent();
            column_dict.Add("Item ID", new int[1] { 0 });
            column_dict.Add("Requirement index", new int[1] { 1 });
            column_dict.Add("Requirement 1", new int[1] { 2 });
            column_dict.Add("Requirement 2", new int[1] { 3 });
            column_dict.Add("Requirement 3", new int[1] { 4 });
        }

        private void set_list_text(int i)
        {
            Byte skill_major = (Byte)(category[current_element][i * 5 + 2]);
            Byte skill_minor = (Byte)(category[current_element][i * 5 + 3]);
            Byte skill_level = (Byte)(category[current_element][i * 5 + 4]);

            string txt = SFCategoryManager.GetSkillName(skill_major, skill_minor, skill_level);
            ListRequirements.Items[i] = txt;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 5;

            for (int i = 0; i < elem_count; i++)
                set_element_variant(current_element, i * 5 + 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListRequirements.SelectedIndex;
            if (cur_selected < 0)
                return;
            set_element_variant(current_element, cur_selected * 5 + 2, Utility.TryParseUInt8(textBox3.Text));
            set_list_text(cur_selected);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListRequirements.SelectedIndex;
            if (cur_selected < 0)
                return;
            set_element_variant(current_element, cur_selected * 5 + 3, Utility.TryParseUInt8(textBox5.Text));
            set_list_text(cur_selected);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListRequirements.SelectedIndex;
            if (cur_selected < 0)
                return;
            set_element_variant(current_element, cur_selected * 5 + 4, Utility.TryParseUInt8(textBox4.Text));
            set_list_text(cur_selected);
        }

        public override void set_element(int index)
        {
            current_element = index;

            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 5;

            ListRequirements.Items.Clear();

            for (int i = 0; i < elem_count; i++)
            {
                ListRequirements.Items.Add("");
                set_list_text(i);
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 6);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int new_index;
            if (ListRequirements.SelectedIndex == -1)
                new_index = ListRequirements.Items.Count - 1;
            else
                new_index = ListRequirements.SelectedIndex;

            SFCategoryElement elem = category[current_element];
            int cur_elem_count = elem.variants.Count / 5;

            Byte max_index = 0;
            for (int i = 0; i < cur_elem_count; i++)
            {
                max_index = Math.Max(max_index, (Byte)(elem[i * 5 + 1]));
            }
            max_index += 1;

            object[] paste_data = new object[5];
            paste_data[0] = (UInt16)elem[0];
            paste_data[1] = (Byte)max_index;
            paste_data[2] = (Byte)0;
            paste_data[3] = (Byte)0;
            paste_data[4] = (Byte)0;

            elem.PasteRaw(paste_data, new_index * 5);

            set_element(current_element);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListRequirements.SelectedIndex == -1)
                return;
            if (ListRequirements.Items.Count == 1)
                return;
            int new_index = ListRequirements.SelectedIndex;

            SFCategoryElement elem = category[current_element];
            Byte cur_spell_index = (Byte)(elem[new_index * 5 + 1]);

            elem.RemoveRaw(new_index * 5, 5);

            int cur_elem_count = elem.variants.Count / 5;
            for (int i = 0; i < cur_elem_count; i++)
                if ((Byte)(elem[i * 5 + 1]) > cur_spell_index)
                    elem[i * 5 + 1] = (Byte)((Byte)(elem[i * 5 + 1]) - 1);

            set_element(current_element);
        }

        private void ListRequirements_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListRequirements.SelectedIndex;
            if (cur_selected < 0)
                return;
            textBox3.Text = variant_repr(cur_selected * 5 + 2);
            textBox5.Text = variant_repr(cur_selected * 5 + 3);
            textBox4.Text = variant_repr(cur_selected * 5 + 4);
        }
    }
}
