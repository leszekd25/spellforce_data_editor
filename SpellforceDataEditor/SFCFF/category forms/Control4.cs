using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFEngine.SFCFF;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control4 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control4()
        {
            InitializeComponent();
            column_dict.Add("Unit stats ID", new int[1] { 0 });
            column_dict.Add("Unit level", new int[1] { 1 });
            column_dict.Add("Unit race ID", new int[1] { 2 });
            column_dict.Add("Agility", new int[1] { 3 });
            column_dict.Add("Dexterity", new int[1] { 4 });
            column_dict.Add("Charisma", new int[1] { 5 });
            column_dict.Add("Intelligence", new int[1] { 6 });
            column_dict.Add("Stamina", new int[1] { 7 });
            column_dict.Add("Strength", new int[1] { 8 });
            column_dict.Add("Wisdom", new int[1] { 9 });
            column_dict.Add("Random init", new int[1] { 10 });
            column_dict.Add("Fire resistance", new int[1] { 11 });
            column_dict.Add("Ice resistance", new int[1] { 12 });
            column_dict.Add("Black resistance", new int[1] { 13 });
            column_dict.Add("Mind resistance", new int[1] { 14 });
            column_dict.Add("Walk speed", new int[1] { 15 });
            column_dict.Add("Fight speed", new int[1] { 16 });
            column_dict.Add("Cast speed", new int[1] { 17 });
            column_dict.Add("Unit size", new int[1] { 18 });
            column_dict.Add("Mana usage", new int[1] { 19 });
            column_dict.Add("Spawn base time", new int[1] { 20 });
            column_dict.Add("Unit flags", new int[1] { 21 });
            column_dict.Add("Head ID", new int[1] { 22 });
            column_dict.Add("Equipment mode", new int[1] { 23 });

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseUInt16(textBox6.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, SFEngine.Utility.TryParseUInt8(textBox2.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, SFEngine.Utility.TryParseUInt16(textBox7.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, SFEngine.Utility.TryParseUInt16(textBox9.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, SFEngine.Utility.TryParseUInt16(textBox8.Text));
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 6, SFEngine.Utility.TryParseUInt16(textBox17.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 7, SFEngine.Utility.TryParseUInt16(textBox4.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 8, SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 9, SFEngine.Utility.TryParseUInt16(textBox18.Text));
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 10, SFEngine.Utility.TryParseUInt16(textBox22.Text));
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 11, SFEngine.Utility.TryParseUInt16(textBox16.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 12, SFEngine.Utility.TryParseUInt16(textBox10.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 13, SFEngine.Utility.TryParseUInt16(textBox11.Text));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 14, SFEngine.Utility.TryParseUInt16(textBox12.Text));
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 15, SFEngine.Utility.TryParseUInt16(textBox13.Text));
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 16, SFEngine.Utility.TryParseUInt16(textBox14.Text));
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 17, SFEngine.Utility.TryParseUInt16(textBox15.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 18, SFEngine.Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox25_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 19, SFEngine.Utility.TryParseUInt16(textBox25.Text));
        }

        private void textBox27_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 20, SFEngine.Utility.TryParseUInt32(textBox27.Text));
        }

        private void textBox26_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 21, SFEngine.Utility.TryParseUInt8(textBox26.Text));
        }

        private void textBox21_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 22, SFEngine.Utility.TryParseUInt16(textBox21.Text));
        }

        private void textBox24_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 23, SFEngine.Utility.TryParseUInt8(textBox24.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox6.Text = variant_repr(1);
            textBox2.Text = variant_repr(2);
            textBox7.Text = variant_repr(3);
            textBox9.Text = variant_repr(4);
            textBox8.Text = variant_repr(5);
            textBox17.Text = variant_repr(6);
            textBox4.Text = variant_repr(7);
            textBox3.Text = variant_repr(8);
            textBox18.Text = variant_repr(9);
            textBox22.Text = variant_repr(10);
            textBox16.Text = variant_repr(11);
            textBox10.Text = variant_repr(12);
            textBox11.Text = variant_repr(13);
            textBox12.Text = variant_repr(14);
            textBox13.Text = variant_repr(15);
            textBox14.Text = variant_repr(16);
            textBox15.Text = variant_repr(17);
            textBox5.Text = variant_repr(18);
            textBox25.Text = variant_repr(19);
            textBox27.Text = variant_repr(20);
            textBox26.Text = variant_repr(21);
            textBox21.Text = variant_repr(22);
            textBox24.Text = variant_repr(23);

            // button repr
            button_repr(ButtonGoto5, 2006, "Hero/Worker skills", "Unit/Hero data");
            button_repr(ButtonGoto6, 2067, "Hero spells", "Unit/Hero data");
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox2, 2022);
        }

        private void ButtonGoto5_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto5, 2006);
            button_repr(ButtonGoto5, 2006, "Hero/Worker skills", "Unit/Hero data");
        }

        private void ButtonGoto6_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto6, 2067);
            button_repr(ButtonGoto6, 2067, "Hero spells", "Unit/Hero data");
        }


        public override string get_element_string(int index)
        {
            UInt16 stats_id = (UInt16)category[index][0];
            UInt16 stats_level = (UInt16)category[index][1];

            string unit_txt;
            if (SFCategoryManager.gamedata[2024] != null)
            {
                SFCategoryElement elem = SFCategoryManager.gamedata[2024].FindElement<UInt16>(2, stats_id);
                unit_txt = SFCategoryManager.GetTextFromElement(elem, 1);
                if (unit_txt == SFEngine.Utility.S_NONAME)
                    unit_txt = SFCategoryManager.GetRuneheroName(stats_id);
            }
            else
                unit_txt = SFEngine.Utility.S_UNKNOWN;

            return stats_id.ToString() + " " + unit_txt + " (lvl " + stats_level.ToString() + ")";
        }

        public override string get_description_string(int index)
        {
            string race_name = "";
            int hp = (int)(UInt16)category[index][7];
            int mana = (int)(UInt16)category[index][9];
            int lvl = ((int)(UInt16)category[index][1]) - 1;
            string stat_txt = "";
            if (SFCategoryManager.gamedata[2048] != null)
            {
                if ((lvl >= 0) && (lvl < SFCategoryManager.gamedata[2048].GetElementCount()))
                {
                    SFCategoryElement lvl_elem = SFCategoryManager.gamedata[2048][lvl];
                    if (lvl_elem != null)
                    {
                        hp *= (int)(UInt16)lvl_elem[1];
                        mana *= (int)(UInt16)lvl_elem[2];
                        hp /= 100;
                        mana /= 100;
                        stat_txt = "\r\nHealth: " + hp.ToString() + "\r\nMana: " + mana.ToString();
                    }
                }
            }
            Byte race_id = (Byte)category[index][2];
            race_name = SFCategoryManager.GetRaceName(race_id);
            Byte flags = (Byte)category[index][23];
            bool isMale = (flags & 1) == 0;
            bool isUnkillable = (flags & 2) == 2;
            string textFlags = "\r\nUnit gender: ";
            if (isMale)
                textFlags += "male";
            else
                textFlags += "female";
            if (isUnkillable)
                textFlags += "\r\nThis unit is unkillable";
            return "This unit race: " + race_name + stat_txt + textFlags;
        }
    }
}
