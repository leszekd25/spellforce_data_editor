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
    public partial class Control7 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        static string[] item_types = { Utility.S_UNKNOWN, "Equipment", "Inventory rune", "Installed rune",
            "Spell scroll", "Equipped scroll", "Unit plan", "Building plan", "Equipped unit plan",
            "Equipped building plan", "Miscellaneous" };

        static string[] equipment_types = { Utility.S_UNKNOWN, "Headpiece", "Chestpiece", "Legpiece", "Unknown", "Unknown", "Ring",
            "1H Weapon", "2H Weapon", "Shield", "Robe", "ItemChestFake (monsters)", "Ranged Weapon", "ItemChestFake (playable)" };

        public Control7()
        {
            InitializeComponent();
            column_dict.Add("Item ID", new int[1] { 0 });
            column_dict.Add("Item type 1", new int[1] { 1 });
            column_dict.Add("Item type 2", new int[1] { 2 });
            column_dict.Add("Item name ID", new int[1] { 3 });
            column_dict.Add("Unit stats ID", new int[1] { 4 });
            column_dict.Add("Army unit ID", new int[1] { 5 });
            column_dict.Add("Building ID", new int[1] { 6 });
            column_dict.Add("Unknown", new int[1] { 7 });
            column_dict.Add("Selling price", new int[1] { 8 });
            column_dict.Add("Buying price", new int[1] { 9 });
            column_dict.Add("Item set ID", new int[1] { 10 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, Utility.TryParseUInt8(textBox10.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, Utility.TryParseUInt8(textBox11.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, Utility.TryParseUInt16(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 6, Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 7, Utility.TryParseUInt8(textBox8.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 8, Utility.TryParseUInt32(textBox6.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 9, Utility.TryParseUInt32(textBox7.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 10, Utility.TryParseUInt8(textBox9.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox10.Text = variant_repr(1);
            textBox11.Text = variant_repr(2);
            textBox2.Text = variant_repr(3);
            textBox3.Text = variant_repr(4);
            textBox4.Text = variant_repr(5);
            textBox5.Text = variant_repr(6);
            textBox8.Text = variant_repr(7);
            textBox6.Text = variant_repr(8);
            textBox7.Text = variant_repr(9);
            textBox9.Text = variant_repr(10);

            button_repr(ButtonGoto8, 2004, "Armor stats", "Item");
            button_repr(ButtonGoto9, 2013, "Scroll link", "Item");
            button_repr(ButtonGoto10, 2015, "Weapon data", "Item");
            button_repr(ButtonGoto11, 2017, "Requirements", "Item");
            button_repr(ButtonGoto12, 2014, "Spell effects", "Item");
            button_repr(ButtonGoto13, 2012, "UI data", "Item");
            button_repr(ButtonGoto14, 2018, "Spell link", "Item");
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox2, 2016);
        }

        private void textBox9_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox9, 2072);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox3, 2005);
        }

        private void textBox4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox4, 2024);
        }

        private void textBox5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox5, 2029);
        }

        private void ButtonGoto8_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto8, 2004);
            button_repr(ButtonGoto8, 2004, "Armor stats", "Item");
        }

        private void ButtonGoto9_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto9, 2013);
            button_repr(ButtonGoto9, 2013, "Scroll link", "Item");
        }

        private void ButtonGoto10_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto10, 2015);
            button_repr(ButtonGoto10, 2015, "Weapon data", "Item");
        }

        private void ButtonGoto11_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto11, 2017);
            button_repr(ButtonGoto11, 2017, "Requirements", "Item");
        }

        private void ButtonGoto12_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto12, 2014);
            button_repr(ButtonGoto12, 2014, "Spell effects", "Item");
        }

        private void ButtonGoto13_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto13, 2012);
            button_repr(ButtonGoto13, 2012, "UI data", "Item");
        }

        private void ButtonGoto14_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto14, 2018);
            button_repr(ButtonGoto14, 2018, "Spell link", "Item");
        }


        public override string get_element_string(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(category[index], 3);
            return category[index][0].ToString() + " " + txt;
        }

        public override string get_description_string(int index)
        {
            string contains_text;
            string item_type_text = "";
            Byte item_type = (Byte)category[index][1];
            Byte bonus_type = (Byte)category[index][2];
            Byte special = (Byte)category[index][7];
            Byte set_type = (Byte)category[index][10];

            if ((item_type > 0) && (item_type < item_types.Length))
                item_type_text += item_types[item_type];
            /*switch (item_type)
            { }*/
            switch (item_type)
            {
                case 2:
                case 3:
                    UInt16 rune_id = (UInt16)category[index][4];
                    contains_text = SFCategoryManager.GetRuneheroName(rune_id);
                    break;
                case 6:
                case 8:
                    UInt16 army_id = (UInt16)category[index][5];
                    contains_text = SFCategoryManager.GetUnitName(army_id);
                    break;
                case 7:
                case 9:
                    UInt16 building_id = (UInt16)category[index][6];
                    contains_text = SFCategoryManager.GetBuildingName(building_id);
                    break;
                default:
                    contains_text = "";
                    break;
            }

            if (item_type == 1)
            {
                string bonus_type_text = String.Copy(Utility.S_UNKNOWN);
                if ((bonus_type > 0) && (bonus_type < (Byte)equipment_types.Length))
                    bonus_type_text = equipment_types[(int)bonus_type];
                item_type_text += " (" + bonus_type_text + ")";
            }

            string total_text = item_type_text;
            if (contains_text != "")
            {
                contains_text += " (" + SFCategoryManager.GetRaceName(bonus_type) + ")";
                //SFCategoryElement race_elem = SFCategoryManager.get_category
                total_text += "\r\nContains " + contains_text;
            }

            if (set_type != 0)
            {
                Byte elem_id = set_type;

                string txt;
                if (SFCategoryManager.gamedata[2072] != null)
                {
                    SFCategoryElement set_elem = SFCategoryManager.gamedata[2072].FindElementBinary<Byte>(0, elem_id);
                    if (set_elem == null)
                        txt = Utility.S_MISSING;
                    else
                        txt = SFCategoryManager.GetTextFromElement(set_elem, 1);
                }
                else
                    txt = Utility.S_UNKNOWN;

                total_text += "\r\nPart of set: " + txt;
            }

            if ((special & 1) == 1)
                total_text += "\r\nStackable item";
            if ((special & 2) == 2)
                total_text += "\r\nLore item";
            if ((special & 4) == 4)
                total_text += "\r\nQuest item (can not be sold)";
            if ((special & 8) == 8)
                total_text += "\r\nQuest item (can be sold)";
            if ((special & 16) == 16)
                total_text += "\r\nYou need to meet all item requirements to use this item";
            if ((special & (0b11100000)) != 0)
                total_text += "\r\nUnknown optional data";
            return total_text;
        }
    }
}
