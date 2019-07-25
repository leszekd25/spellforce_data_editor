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
            Byte skill_major = (Byte)(category[current_element][i * 4 + 1]);
            Byte skill_minor = (Byte)(category[current_element][i * 4 + 2]);
            Byte skill_level = (Byte)(category[current_element][i * 4 + 3]);

            string txt = SFCategoryManager.GetSkillName(skill_major, skill_minor, skill_level);
            ListSkills.Items[i] = txt;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 4;

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

            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 4;

            ListSkills.Items.Clear();

            for (int i = 0; i < elem_count; i++)
            {
                Byte skill_major = (Byte)(elem[i * 4 + 1]);
                Byte skill_minor = (Byte)(elem[i * 4 + 2]);
                Byte skill_level = (Byte)(elem[i * 4 + 3]);

                string txt = SFCategoryManager.GetSkillName(skill_major, skill_minor, skill_level);

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

            SFCategoryElement elem = category[current_element];

            object[] paste_data = new object[4];
            paste_data[0] = (UInt16)elem[0];
            paste_data[1] = (Byte)0;
            paste_data[2] = (Byte)0;
            paste_data[3] = (Byte)0;

            elem.PasteRaw(paste_data, new_index * 4);
            set_element(current_element);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListSkills.SelectedIndex == -1)
                return;
            if (ListSkills.Items.Count == 1)
                return;
            int new_index = ListSkills.SelectedIndex;

            SFCategoryElement elem = category[current_element];
            elem.RemoveRaw(new_index * 4, 4);

            set_element(current_element);
        }
    }
}
