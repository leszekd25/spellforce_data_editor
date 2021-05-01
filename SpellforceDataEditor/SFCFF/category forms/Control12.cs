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
    public partial class Control12 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control12()
        {
            InitializeComponent();
            column_dict.Add("Item ID", new int[1] { 0 });
            column_dict.Add("Item effect index", new int[1] { 1 });
            column_dict.Add("Effect ID", new int[1] { 2 });
        }

        private void set_list_text(int i)
        {
            UInt16 effect_id = (UInt16)(category[current_element, i][2]);

            string txt = SFCategoryManager.GetEffectName(effect_id, true);
            ListEffects.Items[i] = txt;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                set_element_variant(current_element, i, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListEffects.SelectedIndex;
            if (cur_selected < 0)
                return;
            set_element_variant(current_element, cur_selected, 2, Utility.TryParseUInt16(textBox3.Text));
            set_list_text(cur_selected);
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListEffects.Items.Clear();

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                ListEffects.Items.Add("");
                set_list_text(i);
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0, 0);
        }

        private void ListEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListEffects.SelectedIndex;
            if (cur_selected < 0)
                return;
            textBox3.Text = variant_repr(cur_selected, 2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int new_index;
            if (ListEffects.SelectedIndex == Utility.NO_INDEX)
                new_index = ListEffects.Items.Count - 1;
            else
                new_index = ListEffects.SelectedIndex;

            SFCategoryElement elem = category[current_element, 0];

            Byte max_index = 0;
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                max_index = Math.Max(max_index, (Byte)(category[current_element, i][1]));
            }
            max_index += 1;

            category.element_lists[current_element].Elements.Insert(new_index, category.GetEmptyElement());
            category[current_element, new_index][0] = (UInt16)elem[0];
            category[current_element, new_index][1] = (Byte)max_index;

            set_element(current_element);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListEffects.SelectedIndex == Utility.NO_INDEX)
                return;
            if (ListEffects.Items.Count == 1)
                return;
            int new_index = ListEffects.SelectedIndex;

            Byte cur_spell_index = (Byte)(category[current_element, new_index][1]);

            category.element_lists[current_element].Elements.RemoveAt(new_index);
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                if ((Byte)(category[current_element, i][1]) > cur_spell_index)
                    category[current_element, i][1] = (Byte)((Byte)(category[current_element, i][1]) - 1);

            set_element(current_element);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox3, 2002);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 2003);
        }


        public override string get_element_string(int index)
        {
            UInt16 item_id = (UInt16)category[index, 0][0];
            string txt_item = SFCategoryManager.GetItemName(item_id);

            UInt16 effect_id = (UInt16)category[index, 0][2];
            string txt_effect = SFCategoryManager.GetEffectName(effect_id, true);

            return item_id.ToString() + " " + txt_item + " | " + txt_effect;
        }
    }
}
