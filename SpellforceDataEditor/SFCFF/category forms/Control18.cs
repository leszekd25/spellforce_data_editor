using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control18 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2024 c2024;

        public Control18()
        {
            InitializeComponent();

            c2024 = SFCategoryManager.gamedata.c2024;
            category = c2024;

            column_dict.Add("Unit ID", "UnitID");
            column_dict.Add("Unit name ID", "NameID");
            column_dict.Add("Unit stats ID", "StatsID");
            column_dict.Add("Experience gain", "ExperienceGain");
            column_dict.Add("Experience falloff", "ExperienceFalloff");
            column_dict.Add("Money in copper", "CopperLoot");
            column_dict.Add("Gold variance", "CopperVariance");
            column_dict.Add("Rangedness", "Rangedness");
            column_dict.Add("Meat amount", "MeatValue");
            column_dict.Add("Armor", "Armor");
            column_dict.Add("Unit handle", "Handle");
            column_dict.Add("Placeable in map editor", "CanBePlaced");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2024.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2024.SetField(current_element, "NameID", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2024.SetField(current_element, "StatsID", SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            c2024.SetField(current_element, "ExperienceGain", SFEngine.Utility.TryParseUInt32(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            c2024.SetField(current_element, "ExperienceFalloff", SFEngine.Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            c2024.SetField(current_element, "CopperLoot", SFEngine.Utility.TryParseUInt32(textBox6.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            c2024.SetField(current_element, "CopperVariance", SFEngine.Utility.TryParseUInt16(textBox7.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            c2024.SetField(current_element, "Rangedness", SFEngine.Utility.TryParseUInt8(textBox8.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            c2024.SetField(current_element, "MeatValue", SFEngine.Utility.TryParseUInt16(textBox9.Text));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            c2024.SetField(current_element, "Armor", SFEngine.Utility.TryParseUInt16(textBox12.Text));
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            c2024.SetField(current_element, "Handle", SFEngine.StringUtils.FromString(textBox13.Text, 0, 40));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            c2024.SetField(current_element, "CanBePlaced", SFEngine.Utility.TryParseUInt8(textBox10.Text));
        }

        public override void show_element()
        {
            textBox1.Text = c2024[current_element].GetID().ToString();
            textBox2.Text = c2024[current_element].NameID.ToString();
            textBox3.Text = c2024[current_element].StatsID.ToString();
            textBox4.Text = c2024[current_element].ExperienceGain.ToString();
            textBox5.Text = c2024[current_element].ExperienceFalloff.ToString();
            textBox6.Text = c2024[current_element].CopperLoot.ToString();
            textBox7.Text = c2024[current_element].CopperVariance.ToString();
            textBox8.Text = c2024[current_element].Rangedness.ToString();
            textBox9.Text = c2024[current_element].MeatValue.ToString();
            textBox12.Text = c2024[current_element].Armor.ToString();
            textBox13.Text = c2024[current_element].GetHandleString();
            textBox10.Text = c2024[current_element].CanBePlaced.ToString();

            button_repr(ButtonGoto19, 2025);
            button_repr(ButtonGoto20, 2026);
            button_repr(ButtonGoto21, 2028);
            button_repr(ButtonGoto22, 2040);
            button_repr(ButtonGoto23, 2001);
        }

        private int calculate_total_xp(UInt32 xp_gain, UInt16 xp_falloff)
        {
            if ((xp_gain == 0) || (xp_falloff == 0))
            {
                return 0;
            }

            int max_units = 500;
            int s = 0;
            for (int i = 0; i < max_units; i++)
            {
                s += (int)Math.Floor((Single)xp_gain * ((Single)(xp_falloff) / (Single)(xp_falloff + i)));
            }
            return s;
        }

        public override string get_element_string(int index)
        {
            return $"{c2024[index].GetID()} {SFCategoryManager.GetTextByLanguage(c2024[index].NameID, SFEngine.Settings.LanguageID)}";
        }

        public override string get_description_string(int index)
        {
            UInt32 xp_gain = c2024[index].ExperienceGain;
            UInt16 xp_falloff = c2024[index].ExperienceFalloff;
            return "Max XP gained from this unit: " + calculate_total_xp(xp_gain, xp_falloff).ToString();
        }

        private void ButtonGoto19_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto19, 2025);
        }

        private void ButtonGoto20_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto20, 2026);
        }

        private void ButtonGoto21_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto21, 2028);
        }

        private void ButtonGoto22_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto22, 2040);
        }

        private void ButtonGoto23_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto23, 2001);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2005, textBox3.Text);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox2.Text);
        }
    }
}
