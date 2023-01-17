using SFEngine.SFCFF;
using System;
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
            Byte skill_major = (Byte)(category[current_element, i][1]);
            Byte skill_minor = (Byte)(category[current_element, i][2]);
            Byte skill_level = (Byte)(category[current_element, i][3]);

            string txt = SFCategoryManager.GetSkillName(skill_major, skill_minor, skill_level);
            ListSkills.Items[i] = txt;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 4;

            MainForm.data.op_queue.OpenCluster();
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                set_element_variant(current_element, i, 0, SFEngine.Utility.TryParseUInt16(textBox1.Text));
            }

            MainForm.data.op_queue.CloseCluster();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSkills.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            set_element_variant(current_element, cur_selected, 1, SFEngine.Utility.TryParseUInt8(textBox3.Text));
            set_list_text(cur_selected);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSkills.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            set_element_variant(current_element, cur_selected, 2, SFEngine.Utility.TryParseUInt8(textBox4.Text));
            set_list_text(cur_selected);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSkills.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            set_element_variant(current_element, cur_selected, 3, SFEngine.Utility.TryParseUInt8(textBox2.Text));
            set_list_text(cur_selected);
        }

        private void ListSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSkills.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            textBox3.Text = variant_repr(cur_selected, 1);
            textBox4.Text = variant_repr(cur_selected, 2);
            textBox2.Text = variant_repr(cur_selected, 3);
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListSkills.Items.Clear();

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                ListSkills.Items.Add("");
                set_list_text(i);
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int new_index;
            if (ListSkills.SelectedIndex == -1)
            {
                new_index = ListSkills.Items.Count - 1;
            }
            else
            {
                new_index = ListSkills.SelectedIndex;
            }

            SFCategoryElement new_elem = category.GetEmptyElement();
            new_elem[0] = (UInt16)(category[current_element, 0][0]);

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = new_index,
                Element = new_elem,
                IsSubElement = true
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListSkills.SelectedIndex == -1)
            {
                return;
            }

            if (ListSkills.Items.Count == 1)
            {
                return;
            }

            int new_index = ListSkills.SelectedIndex;

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = new_index,
                IsRemoving = true,
                IsSubElement = true
            });
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox1, 2005);
            }
        }


        public override string get_element_string(int index)
        {
            UInt16 stats_id = (UInt16)category[index, 0][0];
            string unit_txt;
            if (SFCategoryManager.gamedata[2024] == null)
            {
                unit_txt = SFEngine.Utility.S_MISSING;
            }
            else
            {
                SFCategoryElement elem = SFCategoryManager.gamedata[2024].FindElement<UInt16>(2, stats_id);
                unit_txt = SFCategoryManager.GetTextFromElement(elem, 1);
                if (unit_txt == SFEngine.Utility.S_NONAME)
                {
                    unit_txt = SFCategoryManager.GetRuneheroName(stats_id);
                }
            }
            return stats_id.ToString() + " " + unit_txt;

        }

        public override void on_add_subelement(int subelem_index)
        {
            ListSkills.Items.Insert(subelem_index, "");
            set_list_text(subelem_index);
        }

        public override void on_remove_subelement(int subelem_index)
        {
            ListSkills.Items.RemoveAt(subelem_index);
        }

        public override void on_update_subelement(int subelem_index)
        {
            set_list_text(subelem_index);
            if (ListSkills.SelectedIndex == subelem_index)
            {
                textBox3.Text = variant_repr(subelem_index, 1);
                textBox4.Text = variant_repr(subelem_index, 2);
                textBox2.Text = variant_repr(subelem_index, 3);
            }
        }
    }
}
