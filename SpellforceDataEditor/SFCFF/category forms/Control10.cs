using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.IO;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control10 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2015 c2015;

        public Control10()
        {
            InitializeComponent();

            c2015 = SFCategoryManager.gamedata.c2015;
            category = c2015;

            column_dict.Add("Item ID", "ItemID");
            column_dict.Add("Min damage", "MinDamage");
            column_dict.Add("Max damage", "MaxDamage");
            column_dict.Add("Min range", "MinRange");
            column_dict.Add("Max range", "MaxRange");
            column_dict.Add("Weapon speed", "WeaponSpeed");
            column_dict.Add("Weapon type", "WeaponType");
            column_dict.Add("Weapon material", "WeaponMaterial");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2015.SetField(current_element, "ItemID", SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            c2015.SetField(current_element, "MinDamage", SFEngine.Utility.TryParseUInt16(textBox6.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            c2015.SetField(current_element, "MaxDamage", SFEngine.Utility.TryParseUInt16(textBox8.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            c2015.SetField(current_element, "MinRange", SFEngine.Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            c2015.SetField(current_element, "MaxRange", SFEngine.Utility.TryParseUInt16(textBox7.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            c2015.SetField(current_element, "WeaponSpeed", SFEngine.Utility.TryParseUInt16(textBox4.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2015.SetField(current_element, "WeaponType", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2015.SetField(current_element, "WeaponMaterial", SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        public override void show_element()
        {
            Category2015Item item = c2015[current_element];

            textBox1.Text = item.ItemID.ToString();
            textBox6.Text = item.MinDamage.ToString();
            textBox8.Text = item.MaxDamage.ToString();
            textBox5.Text = item.MinRange.ToString();
            textBox7.Text = item.MaxRange.ToString();
            textBox4.Text = item.WeaponSpeed.ToString();
            textBox2.Text = item.WeaponType.ToString();
            textBox3.Text = item.WeaponMaterial.ToString();
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2003, textBox1.Text);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2063, textBox2.Text);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2064, textBox3.Text);
        }


        private float get_dmg(int min_dmg, int max_dmg, int sp)
        {
            Single mean = ((Single)min_dmg + (Single)max_dmg) / 2;
            Single ratio = ((Single)sp) / 100;
            return mean * ratio;
        }

        public override string get_element_string(int index)
        {
            c2015.GetID(index, out int id);
            return $"{id} {SFCategoryManager.GetItemName((ushort)id)}";
        }

        public override string get_description_string(int index)
        {
            Category2015Item item = c2015[index];
            bool wpntype_found = SFCategoryManager.gamedata.c2063.GetItemIndex(item.WeaponType, out int wpntype_index);
            bool wpnmat_found = SFCategoryManager.gamedata.c2064.GetItemIndex(item.WeaponMaterial, out int wpnmat_index);

            StringWriter sw = new StringWriter();
            sw.WriteLine($"Weapon type: {(wpntype_found ? SFCategoryManager.GetTextByLanguage(SFCategoryManager.gamedata.c2063[wpntype_index].NameID, SFEngine.Settings.LanguageID) : SFEngine.Utility.S_ITEM_MISSING)}");
            sw.WriteLine($"Weapon material: {(wpnmat_found ? SFCategoryManager.GetTextByLanguage(SFCategoryManager.gamedata.c2064[wpnmat_index].NameID, SFEngine.Settings.LanguageID) : SFEngine.Utility.S_ITEM_MISSING)}");
            sw.WriteLine($"Damage per second: {get_dmg(item.MinDamage, item.MaxRange, item.WeaponSpeed)}");

            return sw.ToString();
        }
    }
}
