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
    public partial class Control10 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control10()
        {
            InitializeComponent();
            column_dict.Add("Item ID", new int[1] { 0 });
            column_dict.Add("Min damage", new int[1] { 1 });
            column_dict.Add("Max damage", new int[1] { 2 });
            column_dict.Add("Min range", new int[1] { 3 });
            column_dict.Add("Max range", new int[1] { 4 });
            column_dict.Add("Weapon speed", new int[1] { 5 });
            column_dict.Add("Weapon type", new int[1] { 6 });
            column_dict.Add("Weapon material", new int[1] { 7 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseUInt16(textBox6.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, SFEngine.Utility.TryParseUInt16(textBox8.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, SFEngine.Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, SFEngine.Utility.TryParseUInt16(textBox7.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, SFEngine.Utility.TryParseUInt16(textBox4.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 6, SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 7, SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox6.Text = variant_repr(1);
            textBox8.Text = variant_repr(2);
            textBox5.Text = variant_repr(3);
            textBox7.Text = variant_repr(4);
            textBox4.Text = variant_repr(5);
            textBox2.Text = variant_repr(6);
            textBox3.Text = variant_repr(7);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 2003);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox2, 2063);
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox3, 2064);
        }


        private float get_dmg(int min_dmg, int max_dmg, int sp)
        {
            Single mean = ((Single)min_dmg + (Single)max_dmg) / 2;
            Single ratio = ((Single)sp) / 100;
            return mean * ratio;
        }

        public override string get_element_string(int index)
        {
            UInt16 item_id = (UInt16)category[index][0];
            string txt = SFCategoryManager.GetItemName(item_id);
            return category[index][0].ToString() + " " + txt;
        }

        public override string get_description_string(int index)
        {
            UInt16 type_id = (UInt16)category[index][6];
            UInt16 material_id = (UInt16)category[index][7];

            string type_name;
            string material_name;

            if (SFCategoryManager.gamedata[2063] == null)
                type_name = SFEngine.Utility.S_UNKNOWN;
            else
            {
                SFCategoryElement type_elem = SFCategoryManager.gamedata[2063].FindElementBinary<UInt16>(0, type_id);
                type_name = SFCategoryManager.GetTextFromElement(type_elem, 1);
            }

            if (SFCategoryManager.gamedata[2064] == null)
                material_name = SFEngine.Utility.S_UNKNOWN;
            else
            {
                SFCategoryElement material_elem = SFCategoryManager.gamedata[2064].FindElementBinary<UInt16>(0, material_id);
                material_name = SFCategoryManager.GetTextFromElement(material_elem, 1);
            }

            UInt16 min_dmg = (UInt16)category[index][1];
            UInt16 max_dmg = (UInt16)category[index][2];
            UInt16 spd = (UInt16)category[index][5];

            return "Weapon type: " + type_name
                + "\r\nWeapon material: " + material_name
                + "\r\nDamage per second: " + get_dmg((int)min_dmg, (int)max_dmg, (int)spd).ToString();
        }
    }
}
