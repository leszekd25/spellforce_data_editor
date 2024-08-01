using SFEngine;
using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control15 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2016 c2016;

        public Control15()
        {
            InitializeComponent();

            c2016 = SFCategoryManager.gamedata.c2016;
            category = c2016;
            
            column_dict.Add("Text ID", "TextID");
            column_dict.Add("Language ID", "LanguageID");
            column_dict.Add("Text mode", "Mode");
            column_dict.Add("Text handle", "Handle");
            column_dict.Add("Text content", "Content");

        }

        public override void set_element(int index)
        {
            current_element = index;

            ListLanguages.Items.Clear();

            for(int i = 0; i < c2016.GetItemSubItemNum(current_element); i++)
            {
                ListLanguages.Items.Add($"Language #{c2016[current_element, i].LanguageID}");
            }

            int safe_index = SFEngine.Utility.NO_INDEX;
            if (c2016.GetItemSubItemNum(current_element) != 0)
            {
                safe_index = 0;
            }

            int lang_index = SFEngine.Utility.NO_INDEX;
            for (int i = 0; i < c2016.GetItemSubItemNum(current_element); i++)
            {
                if (c2016[current_element, i].LanguageID == 1)
                {
                    lang_index = i;
                    break;
                }
                else if(c2016[current_element, i].LanguageID == 0)
                {
                    safe_index = i;
                }
            }

            if (lang_index == SFEngine.Utility.NO_INDEX)
            {
                lang_index = safe_index;
            }

            ListLanguages.SelectedIndex = lang_index;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2016.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < c2016.GetItemSubItemNum(current_element); i++)
            {
                c2016.SetField(current_element, i, "Mode", SFEngine.Utility.TryParseUInt8(textBox3.Text));
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < c2016.GetItemSubItemNum(current_element); i++)
            {
                c2016.SetField(current_element, i, "Handle", StringUtils.FromString(textBox4.Text, 0, 50));
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (ListLanguages.SelectedIndex == -1)
            {
                return;
            }

            c2016.SetField(current_element, ListLanguages.SelectedIndex, "Content", StringUtils.FromString(textBox5.Text, c2016[current_element, ListLanguages.SelectedIndex].LanguageID, 512));
        }

        public override void show_element()
        {
            if (ListLanguages.SelectedIndex == -1)
            {
                textBox1.Text = SFEngine.Utility.S_UNKNOWN;
                textBox3.Text = SFEngine.Utility.S_UNKNOWN;
                textBox4.Text = SFEngine.Utility.S_UNKNOWN;
                textBox5.Text = SFEngine.Utility.S_UNKNOWN;
                return;
            }
            textBox1.Text = c2016[current_element, ListLanguages.SelectedIndex].TextID.ToString();
            textBox3.Text = c2016[current_element, ListLanguages.SelectedIndex].Mode.ToString();
            textBox4.Text = c2016[current_element, ListLanguages.SelectedIndex].GetHandleString();
            textBox5.Text = c2016[current_element, ListLanguages.SelectedIndex].GetContentString();
        }

        private void DomainLanguages_SelectedItemChanged(object sender, EventArgs e)
        {
            show_element();
        }

        private void ButtonRemoveLang_Click(object sender, EventArgs e)
        {
            if (ListLanguages.SelectedIndex == -1)
            {
                return;
            }

            if (ListLanguages.Items.Count == 1)
            {
                return;
            }

            c2016.RemoveSub(current_element, ListLanguages.SelectedIndex);
        }

        private void ButtonAddLang_Click(object sender, EventArgs e)
        {
            if (ListLanguages.SelectedIndex == -1)
            {
                return;
            }

            byte new_lang_id = 0;
            while (true)
            {
                bool found_lang = false;
                for (int i = 0; i < c2016.GetItemSubItemNum(current_element); i++)
                {
                    if (c2016[current_element, i].LanguageID == new_lang_id)
                    {
                        new_lang_id += 1;
                        found_lang = true;
                        break;
                    }
                }
                if (!found_lang)
                {
                    break;
                }
            }

            int new_elem_index = c2016.GetItemSubItemNum(current_element);

            Category2016Item item = new Category2016Item();
            c2016.GetID(current_element, out int id);
            item.SetID(id);

            c2016.AddSubItem(current_element, new_elem_index, item);
            c2016.SetField(current_element, new_elem_index, "LanguageID", new_lang_id);
            c2016.SetField(current_element, new_elem_index, "Mode", c2016[current_element, 0].Mode);
            c2016.SetField(current_element, new_elem_index, "Handle", StringUtils.FromString(c2016[current_element, 0].GetHandleString(), 0, 50));
            c2016.SetField(current_element, new_elem_index, "Content", StringUtils.FromString(SFEngine.Utility.S_TEXT_MISSING, new_lang_id, 512));
        }

        private void ListLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListLanguages.SelectedIndex == -1)
            {
                return;
            }

            show_element();
        }

        public override string get_element_string(int index)
        {
            return $"{c2016[index, 0].TextID} {SFCategoryManager.GetTextByLanguage(c2016[index, 0].TextID, SFEngine.Settings.LanguageID)}";
        }

        public override void on_add_subelement(int subelem_index)
        {
            ListLanguages.Items.Insert(subelem_index, $"Language #{c2016[current_element, subelem_index].LanguageID}");
        }

        public override void on_remove_subelement(int subelem_index)
        {
            ListLanguages.Items.RemoveAt(subelem_index);
        }

        public override void on_update_subelement(int subelem_index)
        {
            ListLanguages.Items[subelem_index] = $"Language #{c2016[current_element, subelem_index].LanguageID}";
            if (ListLanguages.SelectedIndex == subelem_index)
            {
                textBox1.Text = c2016[current_element, subelem_index].TextID.ToString();
                textBox3.Text = c2016[current_element, subelem_index].Mode.ToString();
                textBox4.Text = c2016[current_element, subelem_index].GetHandleString();
                textBox5.Text = c2016[current_element, subelem_index].GetContentString();
            }
        }
    }
}
