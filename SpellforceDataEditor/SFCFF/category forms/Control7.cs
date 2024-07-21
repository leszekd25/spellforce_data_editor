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

            button_repr(ButtonGoto8, SFCategoryManager.gamedata.c2004, "Armor stats", "Item");
            button_repr(ButtonGoto9, SFCategoryManager.gamedata.c2013, "Scroll link", "Item");
            button_repr(ButtonGoto10, SFCategoryManager.gamedata.c2015, "Weapon data", "Item");
            button_repr(ButtonGoto11, SFCategoryManager.gamedata.c2017, "Requirements", "Item");
            button_repr(ButtonGoto12, SFCategoryManager.gamedata.c2014, "Spell effects", "Item");
            button_repr(ButtonGoto13, SFCategoryManager.gamedata.c2012, "UI data", "Item");
            button_repr(ButtonGoto14, SFCategoryManager.gamedata.c2018, "Spell link", "Item");
        }

        private void ButtonGoto8_Click(object sender, EventArgs e)
        {
            button_repr(ButtonGoto8, SFCategoryManager.gamedata.c2004, "Armor stats", "Item");
        }

        private void ButtonGoto9_Click(object sender, EventArgs e)
        {
            button_repr(ButtonGoto9, SFCategoryManager.gamedata.c2013, "Scroll link", "Item");
        }

        private void ButtonGoto10_Click(object sender, EventArgs e)
        {
            button_repr(ButtonGoto10, SFCategoryManager.gamedata.c2015, "Weapon data", "Item");
        }

        private void ButtonGoto11_Click(object sender, EventArgs e)
        {
            button_repr(ButtonGoto11, SFCategoryManager.gamedata.c2017, "Requirements", "Item");
        }

        private void ButtonGoto12_Click(object sender, EventArgs e)
        {
            button_repr(ButtonGoto12, SFCategoryManager.gamedata.c2014, "Spell effects", "Item");
        }

        private void ButtonGoto13_Click(object sender, EventArgs e)
        {
            button_repr(ButtonGoto13, SFCategoryManager.gamedata.c2012, "UI data", "Item");
        }

        private void ButtonGoto14_Click(object sender, EventArgs e)
        {
            button_repr(ButtonGoto14, SFCategoryManager.gamedata.c2018, "Spell link", "Item");
        }


        public override string get_element_string(int index)
        {
            return $"{c2003[index].ItemID} {SFCategoryManager.GetTextByLanguage(c2003[index].NameID, 1)}";
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

            if(item.ItemSetID != 0)
            {
                bool set_found = SFCategoryManager.gamedata.c2072.GetItemIndex(item.ItemSetID, out int set_index);
                if (set_found)
                {
                    sw.WriteLine($"Part of set: {SFCategoryManager.GetTextByLanguage(SFCategoryManager.gamedata.c2072[set_index].DescriptionID, 1)}");
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
    }
}
