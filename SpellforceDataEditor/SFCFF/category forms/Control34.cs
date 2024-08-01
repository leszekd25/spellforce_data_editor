using SFEngine;
using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control34 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2050 c2050;

        public Control34()
        {
            InitializeComponent();

            c2050 = SFCategoryManager.gamedata.c2050;
            category = c2050;

            column_dict.Add("Object ID", "ObjectID");
            column_dict.Add("Name ID", "NameID");
            column_dict.Add("Flags", "Flags");
            column_dict.Add("Flatten mode", "FlattenMode");
            column_dict.Add("Collision polygons", "PolygonNum");
            column_dict.Add("Object handle", "Handle");
            column_dict.Add("Resource amount", "ResourceAmount");
            column_dict.Add("Width", "Width");
            column_dict.Add("Height", "Height");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2050.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2050.SetField(current_element, "NameID", SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            c2050.SetField(current_element, "Flags", SFEngine.Utility.TryParseUInt8(textBox6.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            c2050.SetField(current_element, "FlattenMode", SFEngine.Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            c2050.SetField(current_element, "PolygonNum", SFEngine.Utility.TryParseUInt8(textBox5.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            c2050.SetField(current_element, "Handle", StringUtils.FromString(textBox7.Text, 0, 41));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2050.SetField(current_element, "ResourceAmount", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            c2050.SetField(current_element, "Width", SFEngine.Utility.TryParseUInt16(textBox9.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            c2050.SetField(current_element, "Height", SFEngine.Utility.TryParseUInt16(textBox10.Text));
        }

        public override void show_element()
        {
            textBox1.Text = c2050[current_element].ObjectID.ToString();
            textBox3.Text = c2050[current_element].NameID.ToString();
            textBox6.Text = c2050[current_element].Flags.ToString();
            textBox4.Text = c2050[current_element].FlattenMode.ToString();
            textBox5.Text = c2050[current_element].PolygonNum.ToString();
            textBox7.Text = c2050[current_element].GetHandleString();
            textBox2.Text = c2050[current_element].ResourceAmount.ToString();
            textBox9.Text = c2050[current_element].Width.ToString();
            textBox10.Text = c2050[current_element].Height.ToString();

            button_repr(ButtonGoto35, 2057);
            button_repr(ButtonGoto36, 2065);
        }


        public override string get_element_string(int index)
        {
            return $"{c2050[index].ObjectID} {c2050[index].GetHandleString()}/{SFCategoryManager.GetTextByLanguage(c2050[index].NameID, SFEngine.Settings.LanguageID)}";
        }

        public override string get_description_string(int elem_key)
        {
            Byte flags = c2050[elem_key].Flags;
            string txt = "";

            if ((flags & 0x1) == 0x1)
            {
                txt += "Blocks terrain at its position\r\n";
            }

            if ((flags & 0x2) == 0x2)
            {
                txt += "Adjusts height\r\n";
            }

            if ((flags & 0x4) == 0x4)
            {
                txt += "Can place in original editor\r\n";
            }

            if ((flags & 0x80) == 0x80)
            {
                txt += "Contains loot\r\n";
            }

            if ((flags & (0xFF - 0x87)) != 0)
            {
                txt += "Unknown flags\r\n";
            }

            return txt;
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox3.Text);
        }

        private void ButtonGoto35_Click(object sender, EventArgs e)
        {
            if(button_gen_elem(ButtonGoto35, 2057))
            {
                SFCategoryManager.gamedata.c2057.GetItemIndex(c2050[current_element].GetID(), out int object_index);
                c2050.SetField(current_element, "PolygonNum", (byte)SFCategoryManager.gamedata.c2057.GetItemSubItemNum(object_index));
            }
        }

        private void ButtonGoto36_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto36, 2065);
        }
    }
}
