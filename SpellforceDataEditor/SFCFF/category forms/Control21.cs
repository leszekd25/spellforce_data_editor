using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control21 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        private List<Byte> combo_values = new List<Byte>();

        Category2028 c2028;

        public Control21()
        {
            InitializeComponent();

            c2028 = SFCategoryManager.gamedata.c2028;
            category = c2028;

            column_dict.Add("Unit ID", "ArmyUnitID");
            column_dict.Add("Resource type", "ResourceType");
            column_dict.Add("Resource amount", "ResourceValue");
        }

        private void load_resources()
        {
            comboRes.Items.Clear();
            combo_values.Clear();
            combo_values.Add(0);    //default null value

            Category2044 c2044 = SFCategoryManager.gamedata.c2044;
            for (int i = 0; i < c2044.GetNumOfItems(); i++)
            {
                comboRes.Items.Add($"{SFCategoryManager.GetTextByLanguage(c2044[i].TextID, SFEngine.Settings.LanguageID)}");
                combo_values.Add(c2044[i].ResourceID);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2028.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }


        private void RefreshListResources()
        {
            ListResources.Items.Clear();

            for (int i = 0; i < c2028.GetItemSubItemNum(current_element); i++)
            {
                int res_index = combo_values.IndexOf(c2028[current_element, i].ResourceType);
                string res_name = "";
                if ((res_index > comboRes.Items.Count) || (res_index <= 0))
                {
                    res_name = SFEngine.Utility.S_ITEM_MISSING;
                }
                else
                {
                    res_name = comboRes.Items[res_index - 1].ToString();    //-1 because of null value
                }

                ListResources.Items.Add($"{c2028[current_element, i].ResourceValue} {res_name}");
            }
        }

        public override void set_element(int index)
        {
            current_element = index;

            load_resources();

            RefreshListResources();

            ListResources.SelectedIndex = 0;
        }

        public override void show_element()
        {
            c2028.GetID(current_element, out int id);
            textBox1.Text = id.ToString();
        }

        private void ListResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListResources.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int index = ListResources.SelectedIndex;
            int found_index = combo_values.IndexOf(c2028[current_element, index].ResourceType);
            if ((found_index > comboRes.Items.Count) || (found_index <= 0))
            {

            }
            else
            {
                comboRes.SelectedIndex = found_index - 1;
            }
            textBox3.Text = c2028[current_element, index].ResourceValue.ToString();
        }

        private void comboRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboRes.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int cur_index = ListResources.SelectedIndex;
            Byte current_res = c2028[current_element, cur_index].ResourceType;
            Byte current_val = c2028[current_element, cur_index].ResourceValue;
            Byte new_res = combo_values[comboRes.SelectedIndex + 1];

            // if same resource was selected, exit early
            if (current_res == new_res)
            {
                return;
            }

            // check if resource like this already exists, exit early if so
            for (int i = 0; i < c2028.GetItemSubItemNum(current_element); i++)
            {
                Byte res_id = c2028[current_element, i].ResourceType;
                if (res_id == new_res)
                {
                    comboRes.SelectedIndex = combo_values.IndexOf(current_res) - 1;
                    return;
                }
            }

            // replace element, preserving acending resource type order
            int new_index = c2028.GetItemSubItemNum(current_element) - 1;
            for (int i = 0; i < new_index; i++)
            {
                if (c2028[current_element, i].ResourceType > new_res)
                {
                    new_index = i;
                    break;
                }
            }
            if(cur_index >= new_index)
            {
                cur_index += 1;
            }

            c2028.GetID(current_element, out int id);
            c2028.AddSubItem(current_element, new_index, new()
            {
                ArmyUnitID = (ushort)id,
                ResourceType = new_res,
                ResourceValue = current_val
            });
            c2028.RemoveSub(current_element, cur_index);

            ListResources.SelectedIndex = new_index;
        }


        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2028.SetField(current_element, ListResources.SelectedIndex, "ResourceValue", SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            c2028.AddSubItem(current_element, 0, new()
            {
                ArmyUnitID = c2028[current_element, 0].ArmyUnitID,
                ResourceType = 0,
                ResourceValue = 0
            });

            ListResources.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListResources.Items.Count == 1)
            {
                return;
            }

            int index = ListResources.SelectedIndex;
            if (index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            c2028.RemoveSub(current_element, index);
            ListResources.SelectedIndex = Math.Max(index, ListResources.Items.Count - 1);
        }


        public override string get_element_string(int index)
        {
            return $"{c2028[index, 0].ArmyUnitID} {SFCategoryManager.GetUnitName(c2028[index, 0].ArmyUnitID)}";
        }

        public override void on_add_subelement(int subelem_index)
        {
            RefreshListResources();
        }

        public override void on_remove_subelement(int subelem_index)
        {
            RefreshListResources();
        }

        public override void on_update_subelement(int subelem_index)
        {
            RefreshListResources();
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2024, textBox1.Text);
        }
    }
}
