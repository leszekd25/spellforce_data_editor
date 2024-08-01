using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control4 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        SFEngine.SFCFF.CTG.Category2005 c2005;

        public Control4()
        {
            InitializeComponent();

            c2005 = SFCategoryManager.gamedata.c2005;
            category = c2005;

            column_dict.Add("Unit stats ID", "StatsID");
            column_dict.Add("Unit level", "UnitLevel");
            column_dict.Add("Unit race ID", "UnitRace");
            column_dict.Add("Agility", "Agility");
            column_dict.Add("Dexterity", "Dexterity");
            column_dict.Add("Charisma", "Charisma");
            column_dict.Add("Intelligence", "Intelligence");
            column_dict.Add("Stamina", "Stamina");
            column_dict.Add("Strength", "Strength");
            column_dict.Add("Wisdom", "Wisdom");
            column_dict.Add("Random init", "RandomInit");
            column_dict.Add("Fire resistance", "ResistanceFire");
            column_dict.Add("Ice resistance", "ResistanceIce");
            column_dict.Add("Black resistance", "ResistanceBlack");
            column_dict.Add("Mind resistance", "ResistanceMind");
            column_dict.Add("Walk speed", "SpeedWalk");
            column_dict.Add("Fight speed", "SpeedFight");
            column_dict.Add("Cast speed", "SpeedCast");
            column_dict.Add("Unit size", "UnitSize");
            column_dict.Add("Mana usage", "ManaUsage");
            column_dict.Add("Spawn base time", "SpawnBaseTime");
            column_dict.Add("Unit flags", "UnitFlags");
            column_dict.Add("Head ID", "HeadID");
            column_dict.Add("Equipment mode", "EquipmentMode");

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "StatsID", SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "UnitLevel", SFEngine.Utility.TryParseUInt16(textBox6.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "UnitRace", SFEngine.Utility.TryParseUInt8(textBox2.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "Agility", SFEngine.Utility.TryParseUInt16(textBox7.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "Dexterity", SFEngine.Utility.TryParseUInt16(textBox9.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "Charisma", SFEngine.Utility.TryParseUInt16(textBox8.Text));
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "Intelligence", SFEngine.Utility.TryParseUInt16(textBox17.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "Stamina", SFEngine.Utility.TryParseUInt16(textBox4.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "Strength", SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "Wisdom", SFEngine.Utility.TryParseUInt16(textBox18.Text));
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "RandomInit", SFEngine.Utility.TryParseUInt16(textBox22.Text));
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "ResistanceFire", SFEngine.Utility.TryParseUInt16(textBox16.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "ResistanceIce", SFEngine.Utility.TryParseUInt16(textBox10.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "ResistanceBlack", SFEngine.Utility.TryParseUInt16(textBox11.Text));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "ResistanceMind", SFEngine.Utility.TryParseUInt16(textBox12.Text));
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "SpeedWalk", SFEngine.Utility.TryParseUInt16(textBox13.Text));
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "SpeedFight", SFEngine.Utility.TryParseUInt16(textBox14.Text));
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "SpeedCast", SFEngine.Utility.TryParseUInt16(textBox15.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "UnitSize", SFEngine.Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox25_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "ManaUsage", SFEngine.Utility.TryParseUInt16(textBox25.Text));
        }

        private void textBox27_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "SpawnBaseTime", SFEngine.Utility.TryParseUInt32(textBox27.Text));
        }

        private void textBox26_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "UnitFlags", SFEngine.Utility.TryParseUInt8(textBox26.Text));
        }

        private void textBox21_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "HeadID", SFEngine.Utility.TryParseUInt16(textBox21.Text));
        }

        private void textBox24_TextChanged(object sender, EventArgs e)
        {
            c2005.SetField(current_element, "EquipmentMode", SFEngine.Utility.TryParseUInt8(textBox24.Text));
        }

        public override void show_element()
        {
            Category2005Item item = c2005[current_element];
            textBox1.Text = item.StatsID.ToString();
            textBox6.Text = item.UnitLevel.ToString();
            textBox2.Text = item.UnitRace.ToString();
            textBox7.Text = item.Agility.ToString();
            textBox9.Text = item.Dexterity.ToString();
            textBox8.Text = item.Charisma.ToString();
            textBox17.Text = item.Intelligence.ToString();
            textBox4.Text = item.Stamina.ToString();
            textBox3.Text = item.Strength.ToString();
            textBox18.Text = item.Wisdom.ToString();
            textBox22.Text = item.RandomInit.ToString();
            textBox16.Text = item.ResistanceFire.ToString();
            textBox10.Text = item.ResistanceIce.ToString();
            textBox11.Text = item.ResistanceBlack.ToString();
            textBox12.Text = item.ResistanceMind.ToString();
            textBox13.Text = item.SpeedWalk.ToString();
            textBox14.Text = item.SpeedFight.ToString();
            textBox15.Text = item.SpeedCast.ToString();
            textBox5.Text = item.UnitSize.ToString();
            textBox25.Text = item.ManaUsage.ToString();
            textBox27.Text = item.SpawnBaseTime.ToString();
            textBox26.Text = item.UnitFlags.ToString();
            textBox21.Text = item.HeadID.ToString();
            textBox24.Text = item.EquipmentMode.ToString();

            // button repr
            button_repr(ButtonGoto5, 2006);
            button_repr(ButtonGoto6, 2067);
        }

        private void ButtonGoto5_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto5, 2006);
        }

        private void ButtonGoto6_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto6, 2067);
        }


        public override string get_element_string(int index)
        {
            Category2005Item item = c2005[index];

            if (SFCategoryManager.hero_cache.GetItemIndex(item.StatsID, out int hero_index))
            {
                return $"{item.StatsID} {SFCategoryManager.GetRuneheroName(item.StatsID)} (lvl {item.UnitLevel})";
            }

            for (int i = 0; i < SFCategoryManager.gamedata.c2024.GetNumOfItems(); i++)
            {
                if (SFCategoryManager.gamedata.c2024[i].StatsID == item.StatsID)
                {
                    return $"{item.StatsID} {SFCategoryManager.GetTextByLanguage(SFCategoryManager.gamedata.c2024[i].NameID, SFEngine.Settings.LanguageID)} (lvl {item.UnitLevel})";
                }
            }

            return $"{item.StatsID} {SFEngine.Utility.S_ITEM_MISSING} (lvl {item.UnitLevel})";
        }

        public override string get_description_string(int index)
        {
            Category2005Item item = c2005[current_element];
            StringWriter sw = new StringWriter();

            sw.WriteLine($"This unit race: {SFCategoryManager.GetRaceName(item.UnitRace)}");

            int hp = item.Stamina;
            int mana = item.Wisdom;
            int lvl = item.UnitLevel - 1;

            if ((lvl >= 0) && (lvl < SFCategoryManager.gamedata.c2048.GetNumOfItems()))
            {
                hp *= SFCategoryManager.gamedata.c2048[lvl].HealthFactor;
                mana *= SFCategoryManager.gamedata.c2048[lvl].ManaFactor;
                sw.WriteLine($"Health: {hp / 100}");
                sw.WriteLine($"Mana: {mana / 100}");
            }
            else
            {
                sw.WriteLine($"WARNING: Invalid stats unit level {lvl + 1}");
            }
            sw.WriteLine($"Unit gender: {((item.UnitFlags & 0b1) == 0b1 ? "female" : "male")}");
            if ((item.UnitFlags & 0b10) == 0b10)
            {
                sw.WriteLine("This unit is unkillable");
            }
            return sw.ToString();
        }

        // trace
        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2022, textBox2.Text);
        }
    }
}
