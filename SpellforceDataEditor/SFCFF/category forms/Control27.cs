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
            set_element_variant(current_element, 0, Utility.TryParseUInt8(textBox3.Text));
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListSkills.Items.Clear();

            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;

            for (int i = 0; i < elem_count; i++)
            {
                string txt = SFCategoryManager.GetTextFromElement(elem, i * 3 + 2);
                ListSkills.Items.Add(txt);
            }

            ListSkills.SelectedIndex = 0;
        }

        public override void show_element()
        {
            textBox3.Text = variant_repr(0);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 14);
        }

        private void ListSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListSkills.SelectedIndex == Utility.NO_INDEX)
                return;

            textBox1.Text = variant_repr(ListSkills.SelectedIndex * 3 + 2);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListSkills.SelectedIndex * 3 + 2, Utility.TryParseUInt16(textBox1.Text));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;
            int index = ListSkills.Items.Count;

            object[] paste_data = new object[3];
            paste_data[0] = (Byte)elem[0];
            paste_data[1] = (Byte)index;
            paste_data[2] = (UInt16)0;

            elem.PasteRaw(paste_data, index * 3);
            ListSkills.Items.Add(SFCategoryManager.GetTextFromElement(elem, index*3+2));
            ListSkills.SelectedIndex = index;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;
            if (elem_count == 1)
                return;

            int index = ListSkills.Items.Count -1;

            elem.RemoveRaw(index * 3, 3);
            ListSkills.Items.RemoveAt(index);
            ListSkills.SelectedIndex = Math.Min(index, ListSkills.Items.Count-1);
        }
    }
}
