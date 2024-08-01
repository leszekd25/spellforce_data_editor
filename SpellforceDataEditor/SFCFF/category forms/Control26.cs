using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control26 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        private List<Byte> combo_values = new List<Byte>();

        Category2031 c2031;

        public Control26()
        {
            InitializeComponent();

            c2031 = SFCategoryManager.gamedata.c2031;
            category = c2031;

            column_dict.Add("Building ID", "BuildingID");
            column_dict.Add("Resource type", "ResourceID");
            column_dict.Add("Resource amount", "ResourceRequirement");
        }

        private void load_resources()
        {
            comboRes.Items.Clear();
            combo_values.Clear();
            combo_values.Add(0);    //default null value

            for (int i = 0; i < SFCategoryManager.gamedata.c2044.GetNumOfItems(); i++)
            {
                comboRes.Items.Add(SFCategoryManager.GetTextByLanguage(SFCategoryManager.gamedata.c2044[i].TextID, SFEngine.Settings.LanguageID));
                combo_values.Add(SFCategoryManager.gamedata.c2044[i].ResourceID);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2031.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }


        private void RefreshListResources()
        {
            ListResources.Items.Clear();

            for (int i = 0; i < c2031.GetItemSubItemNum(current_element); i++)
            {
                int res_index = combo_values.IndexOf(c2031[current_element, i].ResourceID);
                string res_name = "";
                if ((res_index > comboRes.Items.Count) || (res_index == 0))
                {
                    res_name = SFEngine.Utility.S_ITEM_MISSING;
                }
                else
                {
                    res_name = comboRes.Items[res_index - 1].ToString();    //-1 because of null value
                }

                ListResources.Items.Add($"{c2031[current_element, i].ResourceRequirement} {res_name}");
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
            textBox1.Text = c2031[current_element, 0].BuildingID.ToString();
        }


        private void ListResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListResources.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int index = ListResources.SelectedIndex;
            int found_index = combo_values.IndexOf(c2031[current_element, index].ResourceID);
            if ((found_index > comboRes.Items.Count) || (found_index <= 0))
            {

            }
            else
            {
                comboRes.SelectedIndex = found_index - 1;
            }
            textBox3.Text = c2031[current_element, index].ResourceRequirement.ToString();
        }

        private void comboRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboRes.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int cur_index = ListResources.SelectedIndex;

            UInt16 current_id = c2031[current_element, 0].BuildingID;
            Byte current_res = c2031[current_element, cur_index].ResourceID;
            UInt16 current_req = c2031[current_element, cur_index].ResourceRequirement;
            Byte new_res = combo_values[comboRes.SelectedIndex + 1];
            if (current_res == new_res)
            {
                return;
            }

            // check if resource like this already exists
            for (int i = 0; i < c2031.GetItemSubItemNum(current_element); i++)
            {
                Byte res_id = c2031[current_element, i].ResourceID;
                if (res_id == new_res)
                {
                    comboRes.SelectedIndex = combo_values.IndexOf(current_res) - 1;
                    return;
                }
            }


            int new_index = c2031.GetItemSubItemNum(current_element) - 1;
            for (int i = 0; i < new_index; i++)
            {
                if (c2031[current_element, i].ResourceID > new_res)
                {
                    new_index = i;
                    break;
                }
            }
            if (cur_index >= new_index)
            {
                cur_index += 1;
            }

            c2031.AddSubItem(current_element, new_index, new()
            {
                BuildingID = current_id,
                ResourceID = new_res,
                ResourceRequirement = current_req
            });
            c2031.RemoveSub(current_element, cur_index);

            ListResources.SelectedIndex = new_index;
        }


        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2031.SetField(current_element, ListResources.SelectedIndex, "ResourceRequirement", SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            c2031.AddSubItem(current_element, 0, new()
            {
                BuildingID = c2031[current_element, 0].BuildingID,
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

            c2031.RemoveSub(current_element, index);

            ListResources.SelectedIndex = Math.Max(index, ListResources.Items.Count - 1);
        }


        public override string get_element_string(int index)
        {
            return $"{c2031[index, 0].BuildingID} {SFCategoryManager.GetBuildingName(c2031[index, 0].BuildingID)}";
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
            textbox_trace(e, 2029, textBox1.Text);
        }
    }
}
