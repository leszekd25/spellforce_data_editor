using SFEngine;
using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control23 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2001 c2001;

        public Control23()
        {
            InitializeComponent();

            c2001 = SFCategoryManager.gamedata.c2001;
            category = c2001;

            column_dict.Add("Unit ID", "ArmyUnitID");
            column_dict.Add("Requirement index", "BuildingIndex");
            column_dict.Add("Building ID", "BuildingID");
        }

        private void set_list_text(int i)
        {
            ListBuildings.Items[i] = SFCategoryManager.GetBuildingName(c2001[current_element, i].BuildingID);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2001.SetID(current_element, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListBuildings.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            c2001.SetField(current_element, cur_selected, "BuildingID", Utility.TryParseUInt16(textBox3.Text));
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListBuildings.Items.Clear();

            for (int i = 0; i < c2001.GetItemSubItemNum(current_element); i++)
            {
                ListBuildings.Items.Add(SFCategoryManager.GetBuildingName(c2001[current_element, i].BuildingID));
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = c2001[current_element, 0].ArmyUnitID.ToString();
        }

        private void ListBuildings_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListBuildings.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            textBox3.Text = c2001[current_element, cur_selected].BuildingID.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int new_index;
            if (ListBuildings.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                new_index = ListBuildings.Items.Count - 1;
            }
            else
            {
                new_index = ListBuildings.SelectedIndex;
            }

            Byte max_index = 0;
            for (int i = 0; i < c2001.GetItemSubItemNum(current_element); i++)
            {
                max_index = Math.Max(max_index, c2001[current_element, i].BuildingIndex);
            }
            max_index += 1;

            c2001.AddSubItem(current_element, new_index, new()
            {
                ArmyUnitID = c2001[current_element, 0].ArmyUnitID,
                BuildingIndex = max_index
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListBuildings.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            if (ListBuildings.Items.Count == 1)
            {
                return;
            }

            int new_index = ListBuildings.SelectedIndex;

            Byte cur_spell_index = c2001[current_element, new_index].BuildingIndex;

            c2001.RemoveSub(current_element, new_index);
            for (int i = 0; i < c2001.GetItemSubItemNum(current_element); i++)
            {
                if (c2001[current_element, i].BuildingIndex > cur_spell_index)
                {
                    c2001.SetField(current_element, i, "BuildingIndex", (byte)(c2001[current_element, i].BuildingIndex - 1));
                }
            }
        }


        public override string get_element_string(int index)
        {
            return $"{c2001[index, 0].ArmyUnitID} {SFCategoryManager.GetUnitName(c2001[index, 0].ArmyUnitID)}";
        }

        public override void on_add_subelement(int subelem_index)
        {
            ListBuildings.Items.Insert(subelem_index, "");
            set_list_text(subelem_index);
        }

        public override void on_remove_subelement(int subelem_index)
        {
            ListBuildings.Items.RemoveAt(subelem_index);
        }

        public override void on_update_subelement(int subelem_index)
        {
            set_list_text(subelem_index);
            if (ListBuildings.SelectedIndex == subelem_index)
            {
                textBox3.Text = c2001[current_element, subelem_index].BuildingID.ToString();
            }
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2024, textBox1.Text);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2029, textBox3.Text);
        }
    }
}
