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

            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count/5;
            DomainLanguages.Items.Clear();

            for (int i = 0; i < elem_count; i++)
                DomainLanguages.Items.Add(((Byte)elem.get_single_variant(5 * i + 1).value).ToString());

            int safe_index = -1;
            int lang_index = -1;
            for(int i = 0; i < elem_count; i++)
            {
                if (((Byte)elem.get_single_variant(5 * i + 1).value) == 1)
                {
                    lang_index = i;
                    break;
                }
                else if (((Byte)elem.get_single_variant(5 * i + 1).value) == 0)
                    safe_index = i;
            }

            if (lang_index == -1)
                lang_index = safe_index;
            DomainLanguages.SelectedIndex = lang_index;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 5;

            for(int i = 0; i < elem_count; i++)
                set_element_variant(current_element, 0 + 5 * i, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 5;

            for (int i = 0; i < elem_count; i++)
                set_element_variant(current_element, 2 + 5 * i, Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 5;

            for (int i = 0; i < elem_count; i++)
                set_element_variant(current_element, 3 + 5 * i, Utility.FixedLengthString(textBox4.Text, 50));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4 + 5 * DomainLanguages.SelectedIndex, Utility.FixedLengthString(textBox5.Text, 512));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0 + 5 * DomainLanguages.SelectedIndex);
            DomainLanguages.Text = variant_repr(1 + 5 * DomainLanguages.SelectedIndex);
            textBox3.Text = variant_repr(2 + 5 * DomainLanguages.SelectedIndex);
            textBox4.Text = string_repr(3 + 5 * DomainLanguages.SelectedIndex);
            textBox5.Text = string_repr(4 + 5 * DomainLanguages.SelectedIndex);
        }

        private void DomainLanguages_SelectedItemChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 5;

            if(DomainLanguages.SelectedIndex < 0)
            {
                DomainLanguages.SelectedIndex = 0;
                return;
            }

            if(DomainLanguages.SelectedIndex >= elem_count)
            {
                DomainLanguages.SelectedIndex = elem_count - 1;
                return;
            }

            show_element();
        }

        private void ButtonRemoveLang_Click(object sender, EventArgs e)
        {
            SFCategoryElement cur_elem = category.get_element(current_element);
            if ((Byte)(cur_elem.get_single_variant(1 + 5 * DomainLanguages.SelectedIndex).value) == 0)
                return;
            //example: aaaaa|bbbbb|ccccc|ddddd
            //remove language index 1
            //result: aaaaa|ccccc|ddddd
            int cur_elem_count = cur_elem.get().Count / 5;
            int offset = 0;
            SFCategoryElement new_elem = new SFCategoryElement();
            Object[] obj_array = new Object[(cur_elem_count-1)*5];
            for(int i = 0; i < cur_elem_count; i++)
            {
                if(i == DomainLanguages.SelectedIndex)
                {
                    offset = 1;
                    continue;
                }
                for(int j = 0; j < 5; j++)
                {
                    obj_array[(i - offset) * 5 + j] = cur_elem.get_single_variant(i * 5 + j).value;
                }
            }
            new_elem.set(obj_array);
            category.get_elements()[current_element] = new_elem;
            set_element(current_element);
            show_element();
        }

        private void ButtonAddLang_Click(object sender, EventArgs e)
        {
            SFCategoryElement cur_elem = category.get_element(current_element);
            
            //example: aaaaa|bbbbb|ccccc|ddddd
            //add language index 2
            //result: aaaaa|bbbbb|ccccc|eeeee|ddddd
            int cur_elem_count = cur_elem.get().Count / 5;

            HashSet<Byte> blist = new HashSet<Byte>();
            Byte new_lang_index = 0;
            bool found_index = false;
            for (int i = 0; i < cur_elem_count; i++)
                blist.Add((Byte)(cur_elem.get_single_variant(1 + 5 * i).value));
            for(int i = 0; i < blist.Count; i++)
                if(!blist.Contains((Byte)i))
                {
                    new_lang_index = (Byte)i;
                    found_index = true;
                    break;
                }
            if (!found_index)
                new_lang_index = (Byte)blist.Count;

            int offset = 0;
            SFCategoryElement new_elem = new SFCategoryElement();
            Object[] obj_array = new Object[(cur_elem_count + 1) * 5];
            for (int i = 0; i < cur_elem_count; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    obj_array[(i + offset) * 5 + j] = cur_elem.get_single_variant(i * 5 + j).value;
                }
                if (i == DomainLanguages.SelectedIndex)
                {
                    offset = 1;
                    obj_array[(i + offset) * 5 + 0] = (UInt16)cur_elem.get_single_variant(0).value;
                    obj_array[(i + offset) * 5 + 1] = new_lang_index;
                    obj_array[(i + offset) * 5 + 2] = (Byte)cur_elem.get_single_variant(2).value;
                    obj_array[(i + offset) * 5 + 3] = Utility.FixedLengthString(Utility.CleanString(cur_elem.get_single_variant(3)), 50);    
                    obj_array[(i + offset) * 5 + 4] = new char[512];    //todo: get this value from category
                }
            }
            new_elem.set(obj_array);
            category.get_elements()[current_element] = new_elem;
            set_element(current_element);
            show_element();
        }
    }
}
