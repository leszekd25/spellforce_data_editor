using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control11 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2017 c2017;

        public Control11()
        {
            InitializeComponent();

            c2017 = SFCategoryManager.gamedata.c2017;
            category = c2017;

            column_dict.Add("Item ID", "ItemID");
            column_dict.Add("Requirement index", "ReqIndex");
            column_dict.Add("Requirement 1", "SkillMajorID");
            column_dict.Add("Requirement 2", "SkillMinorID");
            column_dict.Add("Requirement 3", "SkillLevel");
        }

        private void set_list_text(int i)
        {
            Category2017Item item = c2017[current_element, i];
            ListRequirements.Items[i] = SFCategoryManager.GetSkillName(item.SkillMajorID, item.SkillMinorID, item.SkillLevel);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2017.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListRequirements.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            c2017.SetField(current_element, cur_selected, "SkillMajorID", SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListRequirements.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            c2017.SetField(current_element, cur_selected, "SkillMinorID", SFEngine.Utility.TryParseUInt8(textBox5.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int cur_selected = ListRequirements.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            c2017.SetField(current_element, cur_selected, "SkillLevel", SFEngine.Utility.TryParseUInt8(textBox4.Text));
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListRequirements.Items.Clear();

            for (int i = 0; i < c2017.GetItemSubItemNum(current_element); i++)
            {
                ListRequirements.Items.Add("");
                set_list_text(i);
            }

            show_element();
        }

        public override void show_element()
        {
            c2017.GetID(current_element, out int id);
            textBox1.Text = id.ToString();
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2003, textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Category2017Item item = new Category2017Item();
            c2017.GetID(current_element, out int id);
            item.SetID(id);

            int new_index;
            if (ListRequirements.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                new_index = ListRequirements.Items.Count - 1;
            }
            else
            {
                new_index = ListRequirements.SelectedIndex;
            }

            byte max_index = 0;
            for (int i = 0; i < c2017.GetItemSubItemNum(current_element); i++)
            {
                max_index = Math.Max(max_index, c2017[current_element, i].ReqIndex);
            }
            item.ReqIndex = (byte)(max_index + 1);

            c2017.AddSubItem(current_element, new_index, item);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListRequirements.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            if (ListRequirements.Items.Count == 1)
            {
                return;
            }

            int new_index = ListRequirements.SelectedIndex;
            byte cur_req_index = c2017[current_element, new_index].ReqIndex;
            c2017.RemoveSub(current_element, new_index);

            for (int i = 0; i < c2017.GetItemSubItemNum(current_element); i++)
            {
                if (c2017[current_element, i].ReqIndex > cur_req_index)
                {
                    c2017.SetField(current_element, i, "ReqIndex", (byte)(c2017[current_element, i].ReqIndex - 1));
                }
            }
        }

        private void ListRequirements_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cur_selected = ListRequirements.SelectedIndex;
            if (cur_selected < 0)
            {
                return;
            }

            Category2017Item item = c2017[current_element, cur_selected];

            textBox3.Text = item.SkillMajorID.ToString();
            textBox5.Text = item.SkillMinorID.ToString();
            textBox4.Text = item.SkillLevel.ToString();
        }


        public override string get_element_string(int index)
        {
            c2017.GetID(index, out int item_id);
            return $"{item_id} {SFCategoryManager.GetItemName((ushort)item_id)}";
        }

        public override void on_add_subelement(int subelem_index)
        {
            ListRequirements.Items.Insert(subelem_index, "");
            set_list_text(subelem_index);
        }

        public override void on_remove_subelement(int subelem_index)
        {
            ListRequirements.Items.RemoveAt(subelem_index);
        }

        public override void on_update_subelement(int subelem_index)
        {
            set_list_text(subelem_index);
            if (ListRequirements.SelectedIndex == subelem_index)
            {
                Category2017Item item = c2017[current_element, subelem_index];

                textBox3.Text = item.SkillMajorID.ToString();
                textBox5.Text = item.SkillMinorID.ToString();
                textBox4.Text = item.SkillLevel.ToString();
            }
        }
    }
}
