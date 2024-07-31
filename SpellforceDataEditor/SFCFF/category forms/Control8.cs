using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control8 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2004 c2004;

        public Control8()
        {
            InitializeComponent();

            c2004 = SFCategoryManager.gamedata.c2004;
            category = c2004;

            column_dict.Add("Item ID", "ItemID");
            column_dict.Add("Strength", "Strength");
            column_dict.Add("Stamina", "Stamina");
            column_dict.Add("Agility", "Agility");
            column_dict.Add("Dexterity", "Dexterity");
            column_dict.Add("Health", "Health");
            column_dict.Add("Charisma", "Charisma");
            column_dict.Add("Intelligence", "Intelligence");
            column_dict.Add("Wisdom", "Wisdom");
            column_dict.Add("Mana", "Mana");
            column_dict.Add("Armor", "Armor");
            column_dict.Add("Fire resistance", "ResistFire");
            column_dict.Add("Ice resistance", "ResistIce");
            column_dict.Add("Black resistance", "ResistBlack");
            column_dict.Add("Mind resistance", "ResistMind");
            column_dict.Add("Walking speed", "SpeedWalk");
            column_dict.Add("Fighting speed", "SpeedFight");
            column_dict.Add("Casting speed", "SpeedCast");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "ItemID", SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "Strength", SFEngine.Utility.TryParseInt16(textBox4.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "Stamina", SFEngine.Utility.TryParseInt16(textBox6.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "Agility", SFEngine.Utility.TryParseInt16(textBox8.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "Dexterity", SFEngine.Utility.TryParseInt16(textBox10.Text));
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "Health", SFEngine.Utility.TryParseInt16(textBox18.Text));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "Charisma", SFEngine.Utility.TryParseInt16(textBox12.Text));
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "Intelligence", SFEngine.Utility.TryParseInt16(textBox14.Text));
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "Wisdom", SFEngine.Utility.TryParseInt16(textBox16.Text));
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "Mana", SFEngine.Utility.TryParseInt16(textBox17.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "Armor", SFEngine.Utility.TryParseInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "ResistFire", SFEngine.Utility.TryParseInt16(textBox3.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "ResistIce", SFEngine.Utility.TryParseInt16(textBox5.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "ResistBlack", SFEngine.Utility.TryParseInt16(textBox7.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "ResistMind", SFEngine.Utility.TryParseInt16(textBox9.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "SpeedWalk", SFEngine.Utility.TryParseInt16(textBox11.Text));
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "SpeedFight", SFEngine.Utility.TryParseInt16(textBox13.Text));
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            c2004.SetField(current_element, "SpeedCast", SFEngine.Utility.TryParseInt16(textBox15.Text));
        }

        public override void show_element()
        {
            Category2004Item item = c2004[current_element];

            textBox1.Text = item.ItemID.ToString();
            textBox4.Text = item.Strength.ToString();
            textBox6.Text = item.Stamina.ToString();
            textBox8.Text = item.Agility.ToString();
            textBox10.Text = item.Dexterity.ToString();
            textBox18.Text = item.Health.ToString();
            textBox12.Text = item.Charisma.ToString();
            textBox14.Text = item.Intelligence.ToString();
            textBox16.Text = item.Wisdom.ToString();
            textBox17.Text = item.Mana.ToString();
            textBox2.Text = item.Armor.ToString();
            textBox3.Text = item.ResistFire.ToString();
            textBox5.Text = item.ResistIce.ToString();
            textBox7.Text = item.ResistBlack.ToString();
            textBox9.Text = item.ResistMind.ToString();
            textBox11.Text = item.SpeedWalk.ToString();
            textBox13.Text = item.SpeedFight.ToString();
            textBox15.Text = item.SpeedCast.ToString();
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2003, textBox1.Text);
        }

        public override string get_element_string(int index)
        {
            c2004.GetID(index, out int id);
            return $"{id} {SFCategoryManager.GetItemName((ushort)id)}";
        }
    }
}
