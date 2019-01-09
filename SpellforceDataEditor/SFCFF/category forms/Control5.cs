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
    public partial class Control5 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control5()
        {
            InitializeComponent();
            column_dict.Add("Unit stats ID", new int[1] { 0 });
            column_dict.Add("Unit major skill", new int[1] { 1 });
            column_dict.Add("Unit minor skill", new int[1] { 2 });
            column_dict.Add("Unit skill level", new int[1] { 3 });
        }

        private void set_list_text(int i)
        {
            Byte skill_major = (Byte)(category.get_element_variant(current_element, i * 4 + 1)).value;
            Byte skill_minor = (Byte)(category.get_element_variant(current_element, i * 4 + 2)).value;
            Byte skill_level = (Byte)(category.get_element_variant(current_element, i * 4 + 3)).value;

            string txt = SFCategoryManager.get_skill_name(skill_major, skill_minor, skill_level);
            ListSkills.Items[i] = txt;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 4;

            for (int i = 0; i < elem_count; i++)
                set_element_variant(current_element, i * 4 + 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSkills.SelectedIndex;
            if (cur_selected < 0)
                return;
            set_element_variant(current_element, cur_selected*4+1, Utility.TryParseUInt8(textBox3.Text));
            set_list_text(cur_selected);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSkills.SelectedIndex;
            if (cur_selected < 0)
                return;
            set_element_variant(current_element, cur_selected*4+2, Utility.TryParseUInt8(textBox4.Text));
            set_list_text(cur_selected);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSkills.SelectedIndex;
            if (cur_selected < 0)
                return;
            set_element_variant(current_element, cur_selected*4+3, Utility.TryParseUInt8(textBox2.Text));
            set_list_text(cur_selected);
        }

        private void ListSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSkills.SelectedIndex;
            if (cur_selected < 0)
                return;
            textBox3.Text = variant_repr(cur_selected*4+1);
            textBox4.Text = variant_repr(cur_selected*4+2);
            textBox2.Text = variant_repr(cur_selected*4+3);
        }

        public override void set_element(int index)
        {
            current_element = index;

            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 4;

            ListSkills.Items.Clear();

            for (int i = 0; i < elem_count; i++)
            {
                Byte skill_major = (Byte)(elem.get_single_variant(i * 4 + 1)).value;
                Byte skill_minor = (Byte)(elem.get_single_variant(i * 4 + 2)).value;
                Byte skill_level = (Byte)(elem.get_single_variant(i * 4 + 3)).value;

                string txt = SFCategoryManager.get_skill_name(skill_major, skill_minor, skill_level);

                ListSkills.Items.Add(txt);
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int new_index;
            if (ListSkills.SelectedIndex == -1)
                new_index = ListSkills.Items.Count-1;
            else
                new_index = ListSkills.SelectedIndex;

            SFCategoryElement elem = category.get_element(current_element);
            int cur_elem_count = elem.get().Count / 4;

            int offset = 0;
            SFCategoryElement new_elem = new SFCategoryElement();
            Object[] obj_array = new Object[(cur_elem_count + 1) * 4];
            for (int i = 0; i < cur_elem_count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    obj_array[(i + offset) * 4 + j] = elem.get_single_variant(i * 4 + j).value;
                }
                if (i == new_index)
                {
                    offset = 1;
                    obj_array[(i + offset) * 4 + 0] = (UInt16)elem.get_single_variant(0).value;
                    obj_array[(i + offset) * 4 + 1] = (Byte)0;
                    obj_array[(i + offset) * 4 + 2] = (Byte)0;
                    obj_array[(i + offset) * 4 + 3] = (Byte)0;
                }
            }
            new_elem.set(obj_array);
            category.get_elements()[current_element] = new_elem;
            set_element(current_element);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListSkills.SelectedIndex == -1)
                return;
            if (ListSkills.Items.Count == 1)
                return;
            int new_index = ListSkills.SelectedIndex;

            SFCategoryElement elem = category.get_element(current_element);
            int cur_elem_count = elem.get().Count / 4;

            int offset = 0;
            SFCategoryElement new_elem = new SFCategoryElement();
            Object[] obj_array = new Object[(cur_elem_count - 1) * 4];
            for (int i = 0; i < cur_elem_count; i++)
            {
                if (i == new_index)
                {
                    offset = 1;
                    continue;
                }
                for (int j = 0; j < 4; j++)
                {
                    obj_array[(i - offset) * 4 + j] = elem.get_single_variant(i * 4 + j).value;
                }
            }
            new_elem.set(obj_array);
            category.get_elements()[current_element] = new_elem;
            set_element(current_element);
        }
    }
}
