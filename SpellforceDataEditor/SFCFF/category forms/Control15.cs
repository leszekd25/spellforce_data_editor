﻿using System;
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
    public partial class Control15 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control15()
        {
            InitializeComponent();
            column_dict.Add("Text ID", new int[1] { 0 });
            column_dict.Add("Language ID", new int[1] { 1 });
            column_dict.Add("Text mode", new int[1] { 2 });
            column_dict.Add("Text handle", new int[1] { 3 });
            column_dict.Add("Text content", new int[1] { 4 });

        }

        public override void set_element(int index)
        {
            current_element = index;

            ListLanguages.Items.Clear();

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                ListLanguages.Items.Add("Language #"+((Byte)category[current_element, i][1]).ToString());

            int safe_index = Utility.NO_INDEX;
            int lang_index = Utility.NO_INDEX;
            for(int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                if (((Byte)category[current_element, i][1]) == 1)
                {
                    lang_index = i;
                    break;
                }
                else if (((Byte)category[current_element, i][1]) == 0)
                    safe_index = i;
            }

            if (lang_index == Utility.NO_INDEX)
                lang_index = safe_index;

            ListLanguages.SelectedIndex = lang_index;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (ListLanguages.SelectedIndex == -1)
                return;

            for(int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                set_element_variant(current_element, i, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (ListLanguages.SelectedIndex == -1)
                return;

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                set_element_variant(current_element, i, 2, Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (ListLanguages.SelectedIndex == -1)
                return;

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                set_element_variant(current_element, i, 3, Utility.FixedLengthString(textBox4.Text, 50));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (ListLanguages.SelectedIndex == -1)
                return;

            set_element_variant(current_element, ListLanguages.SelectedIndex, 4, Utility.FixedLengthString(textBox5.Text, 512));
        }

        public override void show_element()
        {
            if(ListLanguages.SelectedIndex == -1)
            {
                textBox1.Text = Utility.S_NONE;
                textBox3.Text = Utility.S_NONE;
                textBox4.Text = Utility.S_NONE;
                textBox5.Text = Utility.S_NONE;
                return;
            }
            textBox1.Text = variant_repr(ListLanguages.SelectedIndex, 0);
            textBox3.Text = variant_repr(ListLanguages.SelectedIndex, 2);
            textBox4.Text = string_repr(ListLanguages.SelectedIndex, 3);
            textBox5.Text = string_repr(ListLanguages.SelectedIndex, 4);
        }

        private void DomainLanguages_SelectedItemChanged(object sender, EventArgs e)
        {
            show_element();
        }

        private void ButtonRemoveLang_Click(object sender, EventArgs e)
        {
            if (ListLanguages.SelectedIndex == -1)
                return;
            if (ListLanguages.Items.Count == 1)
                return;

            category.element_lists[current_element].Elements.RemoveAt(ListLanguages.SelectedIndex);

            set_element(current_element);
            show_element();
        }

        private void ButtonAddLang_Click(object sender, EventArgs e)
        {
            if (ListLanguages.SelectedIndex == -1)
                return;

            SFCategoryElement cur_elem = category[current_element, 0];

            byte new_lang_id = 0;
            while (true)
            {
                bool found_lang = false;
                for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                {
                    if ((byte)category[current_element, i][1] == new_lang_id)
                    {
                        new_lang_id += 1;
                        found_lang = true;
                        break;
                    }
                }
                if (!found_lang)
                    break;
            }

            int new_elem_index = category.element_lists[current_element].Elements.Count;
            category.element_lists[current_element].Elements.Add(category.GetEmptyElement());
            category[current_element, new_elem_index][0] = (UInt16)cur_elem[0];
            category[current_element, new_elem_index][1] = (Byte)new_lang_id;
            category[current_element, new_elem_index][2] = (Byte)cur_elem[2];
            category[current_element, new_elem_index][3] = Utility.FixedLengthString(Utility.CleanString(cur_elem[3]), 50);
            category[current_element, new_elem_index][4] = Utility.FixedLengthString("", 512);

            set_element(current_element);
            show_element();
        }

        private void ListLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListLanguages.SelectedIndex == -1)
                return;

            show_element();
        }



        public override string get_element_string(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(category[index, 0], 0);
            return category[index, 0][0].ToString() + " " + txt;
        }
    }
}
