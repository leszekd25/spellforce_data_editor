using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control16 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        static public string[] race_flags = new string[] { "Undead", "Breathing", "Huntable", "Animal", "Has soul", "Attacks buildings", "Bleeds", "(not used)" };
        static public string[] race_ai_flags = new string[] { "Default", "Idle", "Stroll along", "Nomadic", "Aggressive", "Defensive", "Script", "(not used)" };

        Category2022 c2022;

        public Control16()
        {
            InitializeComponent();

            c2022 = SFCategoryManager.gamedata.c2022;
            category = c2022;

            column_dict.Add("Race ID", "RaceID");
            column_dict.Add("Visibility range (day)", "VisRangeDay");
            column_dict.Add("Visibility range (night)", "VisRangeNight");
            column_dict.Add("Hearing range", "HearRange");
            column_dict.Add("Aggro range factor", "AggroRangeFactor");
            column_dict.Add("Moral", "Moral");
            column_dict.Add("Aggresiveness", "Aggresiveness");
            column_dict.Add("Race text ID", "TextID");
            column_dict.Add("Race flags", "Flags");
            column_dict.Add("Clan ID", "FactionID");
            column_dict.Add("Damage taken (blunt)", "DmgTakenBlunt");
            column_dict.Add("Damage taken (slash)", "DmgTakenSlash");
            column_dict.Add("AI flags", "AIFlags");
            column_dict.Add("Group size (min)", "GroupSizeMin");
            column_dict.Add("Group size (max)", "GroupSizeMax");
            column_dict.Add("Group chance", "GroupChance");
            column_dict.Add("Group formation", "GroupFormation");
            column_dict.Add("Flee", "Flee");
            column_dict.Add("Retreat on damage", "RetreatOnDmg");
            column_dict.Add("Retreat follow", "RetreatFollow");
            column_dict.Add("Attack time factor", "AttackSpeedFactor");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2022.SetID(current_element, SFEngine.Utility.TryParseUInt8(textBox1.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "VisRangeDay", SFEngine.Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "VisRangeNight", SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "HearRange", SFEngine.Utility.TryParseUInt8(textBox2.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "AggroRangeFactor", SFEngine.Utility.TryParseUInt8(textBox7.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "Moral", SFEngine.Utility.TryParseUInt8(textBox6.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "Aggresiveness", SFEngine.Utility.TryParseUInt8(textBox5.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "TextID", SFEngine.Utility.TryParseUInt16(textBox9.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "Flags", SFEngine.Utility.TryParseUInt8(textBox11.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "FactionID", SFEngine.Utility.TryParseUInt16(textBox10.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "DmgTakenBlunt", SFEngine.Utility.TryParseUInt8(textBox8.Text));
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "DmgTakenSlash", SFEngine.Utility.TryParseUInt8(textBox13.Text));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "AIFlags", SFEngine.Utility.TryParseUInt16(textBox12.Text));
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "GroupSizeMin", SFEngine.Utility.TryParseUInt8(textBox19.Text));
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "GroupSizeMax", SFEngine.Utility.TryParseUInt8(textBox18.Text));
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "GroupChance", SFEngine.Utility.TryParseUInt8(textBox17.Text));
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "GroupFormation", SFEngine.Utility.TryParseUInt8(textBox22.Text));
        }

        private void textBox21_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "Flee", SFEngine.Utility.TryParseUInt16(textBox21.Text));
        }

        private void textBox20_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "RetreatOnDmg", SFEngine.Utility.TryParseUInt16(textBox20.Text));
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "RetreatFollow", SFEngine.Utility.TryParseUInt16(textBox16.Text));
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            c2022.SetField(current_element, "AttackSpeedFactor", SFEngine.Utility.TryParseUInt8(textBox14.Text));
        }

        public override void show_element()
        {
            textBox1.Text = c2022[current_element].RaceID.ToString();
            textBox4.Text = c2022[current_element].VisRangeDay.ToString();
            textBox3.Text = c2022[current_element].VisRangeNight.ToString();
            textBox2.Text = c2022[current_element].HearRange.ToString();
            textBox7.Text = c2022[current_element].AggroRangeFactor.ToString();
            textBox6.Text = c2022[current_element].Moral.ToString();
            textBox5.Text = c2022[current_element].Aggresiveness.ToString();
            textBox9.Text = c2022[current_element].TextID.ToString();
            textBox11.Text = c2022[current_element].Flags.ToString();
            textBox10.Text = c2022[current_element].FactionID.ToString();
            textBox8.Text = c2022[current_element].DmgTakenBlunt.ToString();
            textBox13.Text = c2022[current_element].DmgTakenSlash.ToString();
            textBox12.Text = c2022[current_element].AIFlags.ToString();
            textBox19.Text = c2022[current_element].GroupSizeMin.ToString();
            textBox18.Text = c2022[current_element].GroupSizeMax.ToString();
            textBox17.Text = c2022[current_element].GroupChance.ToString();
            textBox22.Text = c2022[current_element].GroupFormation.ToString();
            textBox21.Text = c2022[current_element].Flee.ToString();
            textBox20.Text = c2022[current_element].RetreatOnDmg.ToString();
            textBox16.Text = c2022[current_element].RetreatFollow.ToString();
            textBox14.Text = c2022[current_element].AttackSpeedFactor.ToString();
        }

        public override string get_element_string(int index)
        {
            return $"{c2022[index].RaceID} {SFCategoryManager.GetTextByLanguage(c2022[index].TextID, SFEngine.Settings.LanguageID)}";
        }

        public override string get_description_string(int index)
        {
            Byte flags = c2022[index].Flags;
            UInt16 ai_flags = c2022[index].AIFlags;

            string flag_text = "Race flags: ";
            bool first_flag_set = false;
            for (int i = 0; i < 8; i++)
            {
                if (((flags >> i) & 0x1) == 0x1)
                {
                    if (first_flag_set)
                    {
                        flag_text += " | ";
                    }

                    flag_text += race_flags[i];
                    first_flag_set = true;
                }
            }

            string ai_flag_text = "\r\nAI flags: ";
            first_flag_set = false;
            for (int i = 0; i < 8; i++)
            {
                if (((ai_flags >> i) & 0x1) == 0x1)
                {
                    if (first_flag_set)
                    {
                        ai_flag_text += " | ";
                    }

                    ai_flag_text += race_ai_flags[i];
                    first_flag_set = true;
                }
            }

            return flag_text + ai_flag_text;
        }

        private void textBox9_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox9.Text);
        }

        private void textBox10_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2023, textBox10.Text);
        }
    }
}
