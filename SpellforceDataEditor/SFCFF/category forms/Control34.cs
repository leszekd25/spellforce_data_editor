using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control34 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control34()
        {
            InitializeComponent();
            column_dict.Add("Object ID", new int[1] { 0 });
            column_dict.Add("Name ID", new int[1] { 1 });
            column_dict.Add("Flags", new int[1] { 2 });
            column_dict.Add("Flatten mode", new int[1] { 3 });
            column_dict.Add("Collision polygons", new int[1] { 4 });
            column_dict.Add("Object handle", new int[1] { 5 });
            column_dict.Add("Resource amount", new int[1] { 6 });
            column_dict.Add("Width", new int[1] { 7 });
            column_dict.Add("Height", new int[1] { 8 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, Utility.TryParseUInt16(textBox3.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, Utility.TryParseUInt8(textBox6.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, Utility.TryParseUInt8(textBox5.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, SFString.FromString(textBox7.Text, 0, 40));// Utility.FixedLengthString(textBox7.Text, 40));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 6, Utility.TryParseUInt16(textBox2.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 7, Utility.TryParseUInt16(textBox9.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 8, Utility.TryParseUInt16(textBox10.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox3.Text = variant_repr(1);
            textBox6.Text = variant_repr(2);
            textBox4.Text = variant_repr(3);
            textBox5.Text = variant_repr(4);
            textBox7.Text = string_repr(5);
            textBox2.Text = variant_repr(6);
            textBox9.Text = variant_repr(7);
            textBox10.Text = variant_repr(8);

            button_repr(ButtonGoto35, 2057, "Collision data", "Object");
            button_repr(ButtonGoto36, 2065, "Loot", "Object");
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox3, 2016);
        }

        private void ButtonGoto35_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto35, 2057);
            button_repr(ButtonGoto35, 2057, "Collision data", "Object");
        }

        private void ButtonGoto36_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto36, 2065);
            button_repr(ButtonGoto36, 2065, "Loot", "Object");
        }


        public override string get_element_string(int index)
        {
            UInt16 object_id = (UInt16)category[index][0];

            string txt = SFCategoryManager.GetTextFromElement(category[index], 1);
            string object_handle = category[index][5].ToString();
            return object_id.ToString() + " " + object_handle + "/" + txt;
        }

        public override string get_description_string(int elem_key)
        {
            Byte flags = (Byte)category[elem_key][2];
            string txt = "";

            if ((flags & 0x1) == 0x1)
                txt += "Blocks terrain at its position\r\n";
            if ((flags & 0x2) == 0x2)
                txt += "Adjusts height\r\n";
            if ((flags & 0x4) == 0x4)
                txt += "Can place in original editor\r\n";
            if ((flags & 0x80) == 0x80)
                txt += "Contains loot\r\n";
            if ((flags & (0xFF-0x87)) != 0)
                txt += "Unknown flags\r\n";

            return txt;
        }
    }
}
