using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.IO;
using System.Windows.Forms;
using Windows.Devices.PointOfService;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control7 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        static string[] item_types = { SFEngine.Utility.S_UNKNOWN, "Equipment", "Inventory rune", "Installed rune",
            "Spell scroll", "Equipped scroll", "Unit plan", "Building plan", "Equipped unit plan",
            "Equipped building plan", "Miscellaneous" };

        static string[] equipment_types = { SFEngine.Utility.S_UNKNOWN, "Headpiece", "Chestpiece", "Legpiece", "Unknown", "Unknown", "Ring",
            "1H Weapon", "2H Weapon", "Shield", "Robe", "ItemChestFake (monsters)", "Ranged Weapon", "ItemChestFake (playable)" };

        Category2003 c2003;

        public Control7()
        {
            InitializeComponent();

            c2003 = SFCategoryManager.gamedata.c2003;
            category = c2003;

            column_dict.Add("Item ID", "ItemID");
            column_dict.Add("Item type 1", "ItemType1");
            column_dict.Add("Item type 2", "ItemType2");
            column_dict.Add("Item name ID", "NameID");
            column_dict.Add("Unit stats ID", "UnitStatsI");
            column_dict.Add("Army unit ID", "ArmyUnitID");
            column_dict.Add("Building ID", "BuildingID");
            column_dict.Add("Flags", "Option");
            column_dict.Add("Selling price", "SellValue");
            column_dict.Add("Buying price", "BuyValue");
            column_dict.Add("Item set ID", "ItemSetID");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2003.SetField(current_element, "ItemID", SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            c2003.SetField(current_element, "ItemType1", SFEngine.Utility.TryParseUInt8(textBox10.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            c2003.SetField(current_element, "ItemType2", SFEngine.Utility.TryParseUInt8(textBox11.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2003.SetField(current_element, "NameID", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2003.SetField(current_element, "UnitStatsID", SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            c2003.SetField(current_element, "ArmyUnitID", SFEngine.Utility.TryParseUInt16(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            c2003.SetField(current_element, "BuildingID", SFEngine.Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            c2003.SetField(current_element, "Option", SFEngine.Utility.TryParseUInt8(textBox8.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            c2003.SetField(current_element, "SellValue", SFEngine.Utility.TryParseUInt32(textBox6.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            c2003.SetField(current_element, "BuyValue", SFEngine.Utility.TryParseUInt32(textBox7.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            c2003.SetField(current_element, "ItemSetID", SFEngine.Utility.TryParseUInt8(textBox9.Text));
        }

        public override void show_element()
        {
            Category2003Item item = c2003[current_element];
            textBox1.Text = item.ItemID.ToString();
            textBox10.Text = item.ItemType1.ToString();
            textBox11.Text = item.ItemType2.ToString();
            textBox2.Text = item.NameID.ToString();
            textBox3.Text = item.UnitStatsID.ToString();
            textBox4.Text = item.ArmyUnitID.ToString();
            textBox5.Text = item.BuildingID.ToString();
            textBox8.Text = item.Option.ToString();
            textBox6.Text = item.SellValue.ToString();
            textBox7.Text = item.BuyValue.ToString();
            textBox9.Text = item.ItemSetID.ToString();

            button_repr(ButtonGoto8, 2004);
            button_repr(ButtonGoto9, 2013);
            button_repr(ButtonGoto10, 2015);
            button_repr(ButtonGoto11, 2017);
            button_repr(ButtonGoto12, 2014);
            button_repr(ButtonGoto13, 2012);
            button_repr(ButtonGoto14, 2018);
        }

        private void ButtonGoto8_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto8, 2004);
        }

        private void ButtonGoto9_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto9, 2013);
        }

        private void ButtonGoto10_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto10, 2015);
        }

        private void ButtonGoto11_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto11, 2017);
        }

        private void ButtonGoto12_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto12, 2014);
        }

        private void ButtonGoto13_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto13, 2012);
        }

        private void ButtonGoto14_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto14, 2018);
        }


        public override string get_element_string(int index)
        {
            return $"{c2003[index].ItemID} {SFCategoryManager.GetTextByLanguage(c2003[index].NameID, SFEngine.Settings.LanguageID)}";
        }

        public override string get_description_string(int index)
        {
            Category2003Item item = c2003[index];
            StringWriter sw = new StringWriter();

            if ((item.ItemType1 > 0) && (item.ItemType1 < item_types.Length))
            {
                sw.WriteLine(item_types[item.ItemType1]);
            }
            else
            {
                sw.WriteLine("<INVALID ITEM TYPE>");
            }
            switch (item.ItemType1)
            {
                case 1:
                    if ((item.ItemType2 > 0) && (item.ItemType2 < equipment_types.Length))
                    {
                        sw.WriteLine(equipment_types[item.ItemType2]);
                    }
                    else
                    {
                        sw.WriteLine("<INVALID EQUIPMENT TYPE>");
                    }
                    break;
                case 2:
                case 3:
                    sw.WriteLine($"Contains {SFCategoryManager.GetRuneheroName(item.UnitStatsID)} ({SFCategoryManager.GetRaceName(item.ItemType2)})");
                    break;
                case 6:
                case 8:
                    sw.WriteLine($"Contains {SFCategoryManager.GetUnitName(item.ArmyUnitID)} ({SFCategoryManager.GetRaceName(item.ItemType2)})");
                    break;
                case 7:
                case 9:
                    sw.WriteLine($"Contains {SFCategoryManager.GetBuildingName(item.BuildingID)} ({SFCategoryManager.GetRaceName(item.ItemType2)})");
                    break;
            }

            if (item.ItemSetID != 0)
            {
                bool set_found = SFCategoryManager.gamedata.c2072.GetItemIndex(item.ItemSetID, out int set_index);
                if (set_found)
                {
                    sw.WriteLine($"Part of set: {SFCategoryManager.GetTextByLanguage(SFCategoryManager.gamedata.c2072[set_index].DescriptionID, SFEngine.Settings.LanguageID)}");
                }
                else
                {
                    sw.WriteLine("<INVALID SET ID>");
                }
            }

            if ((item.Option & 0b1) == 0b1)
            {
                sw.WriteLine("Stackable item");
            }
            if ((item.Option & 0b10) == 0b10)
            {
                sw.WriteLine("Lore item");
            }
            if ((item.Option & 0b100) == 0b100)
            {
                sw.WriteLine("Quest item (can not be sold)");
            }
            if ((item.Option & 0b1000) == 0b1000)
            {
                sw.WriteLine("Quest item (can be sold)");
            }
            if ((item.Option & 0b10000) == 0b10000)
            {
                sw.WriteLine("You need to meet all item requirements to use this item");
            }
            if ((item.Option & 0b11100000) != 0)
            {
                sw.WriteLine("Unknown optional data");
            }

            return sw.ToString();
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox2.Text);
        }

        private void textBox9_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2072, textBox9.Text);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2005, textBox3.Text);
        }

        private void textBox4_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2024, textBox4.Text);
        }

        private void textBox5_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2029, textBox5.Text);
        }
    }
}
