using SFEngine.SFCFF;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control27 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control27()
        {
            InitializeComponent();
            column_dict.Add("Skill major type", new int[1] { 0 });
            column_dict.Add("Skill minor type", new int[1] { 1 });
            column_dict.Add("Text ID", new int[1] { 2 });
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            MainForm.data.op_queue.OpenCluster();
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                set_element_variant(current_element, i, 0, SFEngine.Utility.TryParseUInt16(textBox3.Text));
            }

            MainForm.data.op_queue.CloseCluster();
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListSkills.Items.Clear();

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                string txt = SFCategoryManager.GetTextFromElement(category[current_element, i], 2);
                ListSkills.Items.Add(txt);
            }

            ListSkills.SelectedIndex = 0;
        }

        public override void show_element()
        {
            textBox3.Text = variant_repr(0, 0);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox1, 2016);
            }
        }

        private void ListSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListSkills.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            textBox1.Text = variant_repr(ListSkills.SelectedIndex, 2);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListSkills.SelectedIndex, 2, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element, 0];
            int index = ListSkills.Items.Count;

            SFCategoryElement new_elem = category.GetEmptyElement();
            new_elem[0] = (Byte)(category[current_element, 0][0]);
            new_elem[1] = (Byte)index;

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = index,
                Element = new_elem,
                IsSubElement = true,
            });

            ListSkills.SelectedIndex = index;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (category.element_lists[current_element].Elements.Count == 1)
            {
                return;
            }

            int index = ListSkills.Items.Count - 1;

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = index,
                IsRemoving = true,
                IsSubElement = true,
            });

            ListSkills.SelectedIndex = Math.Min(index, ListSkills.Items.Count - 1);
        }


        public override string get_element_string(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(category[index, 0], 2);
            return category[index, 0][0].ToString() + " " + txt;
        }

        public override void on_add_subelement(int subelem_index)
        {
            ListSkills.Items.Add(SFCategoryManager.GetTextFromElement(category[current_element, subelem_index], 2));
        }

        public override void on_remove_subelement(int subelem_index)
        {
            ListSkills.Items.RemoveAt(subelem_index);
        }

        public override void on_update_subelement(int subelem_index)
        {
            if (ListSkills.SelectedIndex != subelem_index)
            {
                return;
            }

            textBox1.Text = variant_repr(subelem_index, 2);
        }
    }
}
