using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFEngine.SFCFF;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control6 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control6()
        {
            InitializeComponent();
            column_dict.Add("Unit stats ID", new int[1] { 0 });
            column_dict.Add("Unit spell index", new int[1] { 1 });
            column_dict.Add("Unit spell ID", new int[1] { 2 });
        }

        private void set_list_text(int i)
        {
            UInt16 spell_id = (UInt16)(category[current_element, i][2]);

            string txt = SFCategoryManager.GetEffectName(spell_id, true);
            ListSpells.Items[i] = txt;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                set_element_variant(current_element, i, 0, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSpells.SelectedIndex;
            if (cur_selected < 0)
                return;
            set_element_variant(current_element, cur_selected, 2, SFEngine.Utility.TryParseUInt16(textBox3.Text));
            set_list_text(cur_selected);
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListSpells.Items.Clear();

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                Byte spell_order = (Byte)(category[current_element, i][1]);
                UInt16 spell_id = (UInt16)(category[current_element, i][2]);

                string txt = SFCategoryManager.GetEffectName(spell_id, true);

                ListSpells.Items.Add(txt);
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0, 0);
        }

        private void ListSpells_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListSpells.SelectedIndex;
            if (cur_selected < 0)
                return;
            textBox3.Text = variant_repr(cur_selected, 2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int new_index;
            if (ListSpells.SelectedIndex == SFEngine.Utility.NO_INDEX)
                new_index = ListSpells.Items.Count - 1;
            else
                new_index = ListSpells.SelectedIndex;

            SFCategoryElement elem = category[current_element, 0];

            Byte max_index = 0;
            for(int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
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
            if (ListSpells.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;
            if (ListSpells.Items.Count == 1)
                return;
            int new_index = ListSpells.SelectedIndex;

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
                step_into(textBox1, 2005);
        }


        public override string get_element_string(int index)
        {
            UInt16 hero_id = (UInt16)category[index, 0][0];
            string txt = SFCategoryManager.GetRuneheroName(hero_id);
            return hero_id.ToString() + " " + txt;
        }

        public override void on_add_subelement(int subelem_index)
        {
            base.on_add_subelement(subelem_index);

            ListSpells.Items.Insert(subelem_index, "");
            set_list_text(subelem_index);
        }

        public override void on_remove_subelement(int subelem_index)
        {
            base.on_remove_subelement(subelem_index);

            ListSpells.Items.RemoveAt(subelem_index);
        }

        public override void on_update_subelement(int subelem_index)
        {
            base.on_update_subelement(subelem_index);

            set_list_text(subelem_index);

            if (ListSpells.SelectedIndex == subelem_index)
            {
                textBox3.Text = variant_repr(subelem_index, 2);
            }
        }
    }
}
