using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control33 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2048 c2048;

        public Control33()
        {
            InitializeComponent();

            c2048 = SFCategoryManager.gamedata.c2048;
            category = c2048;
            
            column_dict.Add("Level", "Level");
            column_dict.Add("Health factor", "HealthFactor");
            column_dict.Add("Mana factor", "ManaFactor");
            column_dict.Add("Experience required", "ExperienceRequired");
            column_dict.Add("Attribute point limit", "AttributePointLimit");
            column_dict.Add("Skill point limit", "SkillPointLimit");
            column_dict.Add("Damage factor", "DamageFactor");
            column_dict.Add("Armor class factor", "ArmorClassFactor");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2048.SetID(current_element, SFEngine.Utility.TryParseUInt8(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2048.SetField(current_element, "HealthFactor", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            c2048.SetField(current_element, "ManaFactor", SFEngine.Utility.TryParseUInt16(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            c2048.SetField(current_element, "ExperienceRequired", SFEngine.Utility.TryParseUInt32(textBox5.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2048.SetField(current_element, "AttributePointLimit", SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            c2048.SetField(current_element, "SkillPointLimit", SFEngine.Utility.TryParseUInt8(textBox9.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            c2048.SetField(current_element, "DamageFactor", SFEngine.Utility.TryParseUInt16(textBox6.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            c2048.SetField(current_element, "ArmorClassFactor", SFEngine.Utility.TryParseUInt16(textBox8.Text));
        }

        public override void show_element()
        {
            textBox1.Text = c2048[current_element].Level.ToString();
            textBox2.Text = c2048[current_element].HealthFactor.ToString();
            textBox4.Text = c2048[current_element].ManaFactor.ToString();
            textBox5.Text = c2048[current_element].ExperienceRequired.ToString();
            textBox3.Text = c2048[current_element].AttributePointLimit.ToString();
            textBox9.Text = c2048[current_element].SkillPointLimit.ToString();
            textBox6.Text = c2048[current_element].DamageFactor.ToString();
            textBox8.Text = c2048[current_element].ArmorClassFactor.ToString();
        }


        public override string get_element_string(int index)
        {
            return $"Level {c2048[index].Level}";
        }
    }
}
