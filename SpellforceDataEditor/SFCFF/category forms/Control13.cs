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
            Byte ui_index = (Byte)category[current_element][i * 4 + 1];

            ListUI.Items[i] = ui_index.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 4;

            for (int i = 0; i < elem_count; i++)
                set_element_variant(current_element, i * 4 + 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListUI.SelectedIndex;
            if (cur_selected < 0)
                return;
            set_element_variant(current_element, cur_selected * 4 + 2, Utility.FixedLengthString(textBox4.Text, 64));
            set_list_text(cur_selected);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            int cur_selected = ListUI.SelectedIndex;
            if (cur_selected < 0)
                return;
            set_element_variant(current_element, cur_selected * 4 + 3, (checkBox1.Checked?(UInt16)1:(UInt16)0));
            set_list_text(cur_selected);
        }

        public override void set_element(int index)
        {
            current_element = index;

            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 4;

            ListUI.Items.Clear();

            for (int i = 0; i < elem_count; i++)
            {
                ListUI.Items.Add("");
                set_list_text(i);
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);

        }

        private void ListUI_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListUI.SelectedIndex;
            if (cur_selected < 0)
                return;
            textBox4.Text = string_repr(cur_selected * 4 + 2);
            checkBox1.Checked = ((UInt16)(category[current_element][cur_selected * 4 + 3])) == 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int new_index;
            if (ListUI.SelectedIndex == -1)
                new_index = ListUI.Items.Count - 1;
            else
                new_index = ListUI.SelectedIndex;

            SFCategoryElement elem = category[current_element];
            int cur_elem_count = elem.variants.Count / 4;

            Byte max_index = 0;
            for (int i = 0; i < cur_elem_count; i++)
            {
                max_index = Math.Max(max_index, (Byte)elem[i * 4 + 1]);
            }
            max_index += 1;

            object[] paste_data = new object[4];
            paste_data[0] = (UInt16)elem[0];
            paste_data[1] = (Byte)max_index;
            paste_data[2] = Utility.FixedLengthString("", 64);
            paste_data[3] = (UInt16)0;

            elem.PasteRaw(paste_data, new_index * 4);

            set_element(current_element);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListUI.SelectedIndex == -1)
                return;
            if (ListUI.Items.Count == 1)
                return;
            int new_index = ListUI.SelectedIndex;

            SFCategoryElement elem = category[current_element];
            Byte cur_spell_index = (Byte)(elem[new_index * 4 + 1]);

            elem.RemoveRaw(new_index * 4, 4);

            int cur_elem_count = elem.variants.Count / 4;
            for (int i = 0; i < cur_elem_count; i++)
                if ((Byte)(elem[i * 4 + 1]) > cur_spell_index)
                    elem[i * 4 + 1] = (Byte)((Byte)(elem[i * 4 + 1]) - (Byte)1);

            set_element(current_element);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 6);
        }
    }
}
