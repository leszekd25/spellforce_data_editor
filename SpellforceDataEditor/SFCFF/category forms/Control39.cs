﻿using System;
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
    public partial class Control39 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control39()
        {
            InitializeComponent();
            column_dict.Add("Portal ID", new int[1] { 0 });
            column_dict.Add("Map ID", new int[1] { 1 });
            column_dict.Add("Position X", new int[1] { 2 });
            column_dict.Add("Position Y", new int[1] { 3 });
            column_dict.Add("Unknown", new int[1] { 4 });
            column_dict.Add("Name ID", new int[1] { 5 });
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, Utility.TryParseUInt16(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, Utility.TryParseUInt32(textBox1.Text));
        }

        private void tb_rng_min_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, Utility.TryParseUInt16(tb_rng_min.Text));
        }

        private void tb_rng_max_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 3, Utility.TryParseUInt16(tb_rng_max.Text));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 4, (Byte)(checkBox1.Checked ? 1 : 0));
        }

        private void tb_req4_1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 5, Utility.TryParseUInt16(tb_req4_1.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = variant_repr(0);
            textBox1.Text = variant_repr(1);
            tb_rng_min.Text = variant_repr(2);
            tb_rng_max.Text = variant_repr(3);
            checkBox1.Checked = ((Byte)category[current_element][4] != 0);
            tb_req4_1.Text = variant_repr(5);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 2052);
        }

        private void tb_req4_1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(tb_req4_1, 2016);
        }


        public override string get_element_string(int index)
        {
            UInt16 object_id = (UInt16)category[index][0];
            string txt = SFCategoryManager.GetTextFromElement(category[index], 5);
            return object_id.ToString() + " " + txt;
        }

        public override string get_description_string(int index)
        {
            string map_handle = "";
            UInt32 map_id = (UInt32)category[index][1];
            SFCategoryElement map_elem = SFCategoryManager.gamedata[2052].FindElementBinary<UInt32>(0, map_id);
            if (map_elem == null)
                map_handle = Utility.S_NONAME;
            else
                map_handle = map_elem[2].ToString();
            return "Map handle: " + map_handle;
        }
    }
}
