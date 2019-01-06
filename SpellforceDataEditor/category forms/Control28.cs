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
    public partial class Control28 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control28()
        {
            InitializeComponent();
            column_dict.Add("Skill ID", new int[1] { 0 });
            column_dict.Add("Skill level", new int[1] { 1 });
            column_dict.Add("Strength", new int[1] { 2 });
            column_dict.Add("Stamina", new int[1] { 3 });
            column_dict.Add("Agility", new int[1] { 4 });
            column_dict.Add("Dexterity", new int[1] { 5 });
            column_dict.Add("Chrisma", new int[1] { 6 });
            column_dict.Add("Intelligence", new int[1] { 7 });
            column_dict.Add("Wisdom", new int[1] { 8 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 9;

            for (int i = 0; i < elem_count; i++)
                set_element_variant(current_element, i * 9 + 0, Utility.TryParseUInt8(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex * 9 + 2, Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex * 9 + 3, Utility.TryParseUInt8(textBox5.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex * 9 + 4, Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex * 9 + 5, Utility.TryParseUInt8(textBox7.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex * 9 + 6, Utility.TryParseUInt8(textBox6.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex * 9 + 7, Utility.TryParseUInt8(textBox9.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex * 9 + 8, Utility.TryParseUInt8(textBox8.Text));
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListLevels.Items.Clear();

            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 9;

            for(int i = 0; i < elem_count; i++)
            {
                ListLevels.Items.Add("Level " + (i + 1).ToString());
            }

            ListLevels.SelectedIndex = 0;
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
        }

        private void ListLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListLevels.SelectedIndex == -1)
                return;

            int index = ListLevels.SelectedIndex;
            textBox3.Text = variant_repr(index * 9 + 2);
            textBox5.Text = variant_repr(index * 9 + 3);
            textBox4.Text = variant_repr(index * 9 + 4);
            textBox7.Text = variant_repr(index * 9 + 5);
            textBox6.Text = variant_repr(index * 9 + 6);
            textBox9.Text = variant_repr(index * 9 + 7);
            textBox8.Text = variant_repr(index * 9 + 8);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 9;
            int index = ListLevels.Items.Count;

            object[] paste_data = new object[9];
            paste_data[0] = (Byte)elem.get_single_variant(0).value;
            paste_data[1] = (Byte)(index+1);
            paste_data[2] = (Byte)0;
            paste_data[3] = (Byte)0;
            paste_data[4] = (Byte)0;
            paste_data[5] = (Byte)0;
            paste_data[6] = (Byte)0;
            paste_data[7] = (Byte)0;
            paste_data[8] = (Byte)0;

            elem.paste_raw(paste_data, index * 9);
            ListLevels.Items.Add("Level " + (index + 1).ToString());
            ListLevels.SelectedIndex = index;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 9;
            if (elem_count == 1)
                return;

            int index = ListLevels.Items.Count - 1;

            elem.remove_raw(index * 9, 9);
            ListLevels.Items.RemoveAt(index);
            ListLevels.SelectedIndex = Math.Min(index, ListLevels.Items.Count - 1);
        }
    }
}
