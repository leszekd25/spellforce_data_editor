using SFEngine;
using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control31 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        static string[] item_types = { SFEngine.Utility.S_UNKNOWN, "Equipment", "Inventory rune", "Installed rune",
            "Spell scroll", "Equipped scroll", "Unit plan", "Building plan", "Equipped unit plan",
            "Equipped building plan", "Miscellaneous" };

        Category2047 c2047;

        public Control31()
        {
            InitializeComponent();

            c2047 = SFCategoryManager.gamedata.c2047;
            category = c2047;

            column_dict.Add("Merchant ID", "MerchantID");
            column_dict.Add("Item type", "ItemType");
            column_dict.Add("Price multiplier", "PriceMultiplier");
        }

        private void load_item_types()
        {
            comboItemType.Items.Clear();

            int elem_count = item_types.Length;
            for (int i = 1; i < elem_count; i++)
            {
                comboItemType.Items.Add(item_types[i]);
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            c2047.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox4_Validated(object sender, EventArgs e)
        {
            c2047.SetField(current_element, ListItemTypes.SelectedIndex, "PriceMultiplier", SFEngine.Utility.TryParseUInt16(textBox4.Text));
        }


        private void RefreshListItemTypes()
        {
            ListItemTypes.Items.Clear();

            for (int i = 0; i < c2047.GetItemSubItemNum(current_element); i++)
            {
                int res_index = c2047[current_element, i].ItemType;
                string res_name = "";
                if ((res_index > comboItemType.Items.Count) || (res_index == 0))
                {
                    res_name = SFEngine.Utility.S_ITEM_MISSING;
                }
                else
                {
                    res_name = comboItemType.Items[res_index - 1].ToString();    //-1 because of null value
                }

                ListItemTypes.Items.Add(res_name);
            }
        }

        public override void set_element(int index)
        {
            current_element = index;

            load_item_types();

            RefreshListItemTypes();

            ListItemTypes.SelectedIndex = 0;
        }

        public override void show_element()
        {
            textBox5.Text = c2047[current_element, 0].MerchantID.ToString();
        }

        private void ListItemTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListItemTypes.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int index = ListItemTypes.SelectedIndex;

            comboItemType.SelectedIndex = c2047[current_element, index].ItemType - 1;
            textBox4.Text = c2047[current_element, index].PriceMultiplier.ToString();
        }

        private void comboItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboItemType.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int cur_index = ListItemTypes.SelectedIndex;
            UInt16 current_id = c2047[current_element, 0].MerchantID;
            Byte current_res = c2047[current_element, cur_index].ItemType;
            UInt16 current_mul = c2047[current_element, cur_index].PriceMultiplier;
            Byte new_res = (Byte)(comboItemType.SelectedIndex + 1);
            if (current_res == new_res)
            {
                return;
            }

            // check if resource like this already exists
            for (int i = 0; i < c2047.GetItemSubItemNum(current_element); i++)
            {
                Byte res_id = c2047[current_element, i].ItemType;
                if (res_id == new_res)
                {
                    comboItemType.SelectedIndex = current_res - 1;
                    return;
                }
            }


            // generate new element with reordered resources by resource id, ascending order
            int new_index = c2047.GetItemSubItemNum(current_element) - 1;
            for (int i = 0; i < new_index; i++)
            {
                if (c2047[current_element, i].ItemType > new_res)
                {
                    new_index = i;
                    break;
                }
            }
            if (cur_index >= new_index)
            {
                cur_index += 1;
            }

            c2047.AddSubItem(current_element, new_index, new()
            {
                MerchantID = current_id,
                ItemType = new_res,
                PriceMultiplier = current_mul
            });
            c2047.RemoveSub(current_element, cur_index);

            ListItemTypes.SelectedIndex = new_index;
        }


        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (ListItemTypes.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            c2047.SetField(current_element, ListItemTypes.SelectedIndex, "PriceMultiplier", SFEngine.Utility.TryParseUInt16(textBox4.Text));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            c2047.AddSubItem(current_element, 0, new()
            {
                MerchantID = c2047[current_element, 0].MerchantID,
                ItemType = 0,
                PriceMultiplier = 100
            });

            ListItemTypes.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListItemTypes.Items.Count == 1)
            {
                return;
            }

            int index = ListItemTypes.SelectedIndex;
            if (index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            c2047.RemoveSub(current_element, index);
            ListItemTypes.SelectedIndex = Math.Min(index, ListItemTypes.Items.Count - 1);
        }


        public override string get_element_string(int index)
        {
            return $"{c2047[index, 0].MerchantID} {SFCategoryManager.GetMerchantName(c2047[index, 0].MerchantID)}";
        }

        public override void on_add_subelement(int subelem_index)
        {
            RefreshListItemTypes();
        }

        public override void on_remove_subelement(int subelem_index)
        {
            RefreshListItemTypes();
        }

        public override void on_update_subelement(int subelem_index)
        {
            RefreshListItemTypes();
        }

        private void textBox5_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2029, textBox5.Text);
        }
    }
}
