﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.category_forms
{
    public partial class Control1 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control1()
        {
            InitializeComponent();
            column_dict.Add("Spell effect ID", new int[1] { 0 });
            column_dict.Add("Spell type ID", new int[1] { 1 });
            column_dict.Add("Requirement 1 1", new int[1] { 2 }); column_dict.Add("Requirement 1 2", new int[1] { 3 }); column_dict.Add("Requirement 1 3", new int[1] { 4 });
            column_dict.Add("Requirement 2 1", new int[1] { 5 }); column_dict.Add("Requirement 2 2", new int[1] { 6 }); column_dict.Add("Requirement 2 3", new int[1] { 7 });
            column_dict.Add("Requirement 3 1", new int[1] { 8 }); column_dict.Add("Requirement 3 2", new int[1] { 9 }); column_dict.Add("Requirement 3 3", new int[1] { 10 });
            column_dict.Add("Requirement 4 1", new int[1] { 11 }); column_dict.Add("Requirement 4 2", new int[1] { 12 }); column_dict.Add("Requirement 4 3", new int[1] { 13 });
            column_dict.Add("Mana cost", new int[1] { 14 });
            column_dict.Add("Cast time", new int[1] { 15 });
            column_dict.Add("Recast time", new int[1] { 16 });
            column_dict.Add("Minimum range", new int[1] { 17 });
            column_dict.Add("Maximum range", new int[1] { 18 });
            column_dict.Add("Casting type 1", new int[1] { 19 });
            column_dict.Add("Casting type 2", new int[1] { 20 });
            column_dict.Add("Spell data 1", new int[1] { 21 });
            column_dict.Add("Spell data 2", new int[1] { 22 });
            column_dict.Add("Spell data 3", new int[1] { 23 });
            column_dict.Add("Spell data 4", new int[1] { 24 });
            column_dict.Add("Spell data 5", new int[1] { 25 });
            column_dict.Add("Spell data 6", new int[1] { 26 });
            column_dict.Add("Spell data 7", new int[1] { 27 });
            column_dict.Add("Spell data 8", new int[1] { 28 });
            column_dict.Add("Spell data 9", new int[1] { 29 });
            column_dict.Add("Spell data 10", new int[1] { 30 });
            column_dict.Add("Unknown", new int[4] { 31, 32, 33, 34 });
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(tb_effID.Text));
        }

        private void tb_typeID_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt16(tb_typeID.Text));
        }

        private void tb_req1_1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.TryParseUInt8(tb_req1_1.Text));
        }

        private void tb_req1_2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 3, Utility.TryParseUInt8(tb_req1_2.Text));
        }

        private void tb_req1_3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 4, Utility.TryParseUInt8(tb_req1_3.Text));
        }

        private void tb_req2_1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 5, Utility.TryParseUInt8(tb_req2_1.Text));
        }

        private void tb_req2_2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 6, Utility.TryParseUInt8(tb_req2_2.Text));
        }

        private void tb_req2_3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 7, Utility.TryParseUInt8(tb_req2_3.Text));
        }

        private void tb_req3_1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 8, Utility.TryParseUInt8(tb_req3_1.Text));
        }

        private void tb_req3_2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 9, Utility.TryParseUInt8(tb_req3_2.Text));
        }

        private void tb_req3_3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 10, Utility.TryParseUInt8(tb_req3_3.Text));
        }

        private void tb_req4_1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 11, Utility.TryParseUInt8(tb_req4_1.Text));
        }

        private void tb_req4_2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 12, Utility.TryParseUInt8(tb_req4_2.Text));
        }

        private void tb_req4_3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 13, Utility.TryParseUInt8(tb_req4_3.Text));
        }

        private void tb_mnc_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 14, Utility.TryParseUInt16(tb_mnc.Text));
        }

        private void tb_ctm_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 15, Utility.TryParseUInt32(tb_ctm.Text));
        }

        private void tb_rtm_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 16, Utility.TryParseUInt32(tb_rtm.Text));
        }

        private void tb_rng_min_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 17, Utility.TryParseUInt16(tb_rng_min.Text));
        }

        private void tb_rng_max_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 18, Utility.TryParseUInt16(tb_rng_max.Text));
        }

        private void tb_ct1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 19, Utility.TryParseUInt8(tb_ct1.Text));
        }

        private void tb_ct2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 20, Utility.TryParseUInt8(tb_ct2.Text));
        }

        private void tb_sd1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 21, Utility.TryParseUInt32(tb_sd1.Text));
        }

        private void tb_sd2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 22, Utility.TryParseUInt32(tb_sd2.Text));
        }

        private void tb_sd3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 23, Utility.TryParseUInt32(tb_sd3.Text));
        }

        private void tb_sd4_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 24, Utility.TryParseUInt32(tb_sd4.Text));
        }

        private void tb_sd5_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 25, Utility.TryParseUInt32(tb_sd5.Text));
        }

        private void tb_sd6_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 26, Utility.TryParseUInt32(tb_sd6.Text));
        }

        private void tb_sd7_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 27, Utility.TryParseUInt32(tb_sd7.Text));
        }

        private void tb_sd8_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 28, Utility.TryParseUInt32(tb_sd8.Text));
        }

        private void tb_sd9_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 29, Utility.TryParseUInt32(tb_sd9.Text));
        }

        private void tb_sd10_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 30, Utility.TryParseUInt32(tb_sd10.Text));
        }

        private void tb_unk_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(tb_unk.Text, 4);
            category.set_element_variant(current_element, 31, data_array[0]);
            category.set_element_variant(current_element, 32, data_array[1]);
            category.set_element_variant(current_element, 33, data_array[2]);
            category.set_element_variant(current_element, 34, data_array[3]);
        }

        private void set_data_labels(string[] p)
        {
            lb_sd1.Text = p[0];
            lb_sd2.Text = p[1];
            lb_sd3.Text = p[2];
            lb_sd4.Text = p[3];
            lb_sd5.Text = p[4];
            lb_sd6.Text = p[5];
            lb_sd7.Text = p[6];
            lb_sd8.Text = p[7];
            lb_sd9.Text = p[8];
            lb_sd10.Text = p[9];
        }

        public override void show_element()
        {
            tb_effID.Text = variant_repr(0);
            tb_typeID.Text = variant_repr(1);
            tb_req1_1.Text = variant_repr(2);
            tb_req1_2.Text = variant_repr(3);
            tb_req1_3.Text = variant_repr(4);
            tb_req2_1.Text = variant_repr(5);
            tb_req2_2.Text = variant_repr(6);
            tb_req2_3.Text = variant_repr(7);
            tb_req3_1.Text = variant_repr(8);
            tb_req3_2.Text = variant_repr(9);
            tb_req3_3.Text = variant_repr(10);
            tb_req4_1.Text = variant_repr(11);
            tb_req4_2.Text = variant_repr(12);
            tb_req4_3.Text = variant_repr(13);
            tb_mnc.Text = variant_repr(14);
            tb_ctm.Text = variant_repr(15);
            tb_rtm.Text = variant_repr(16);
            tb_rng_min.Text = variant_repr(17);
            tb_rng_max.Text = variant_repr(18);
            tb_ct1.Text = variant_repr(19);
            tb_ct2.Text = variant_repr(20);
            tb_sd1.Text = variant_repr(21);
            tb_sd2.Text = variant_repr(22);
            tb_sd3.Text = variant_repr(23);
            tb_sd4.Text = variant_repr(24);
            tb_sd5.Text = variant_repr(25);
            tb_sd6.Text = variant_repr(26);
            tb_sd7.Text = variant_repr(27);
            tb_sd8.Text = variant_repr(28);
            tb_sd9.Text = variant_repr(29);
            tb_sd10.Text = variant_repr(30);
            tb_unk.Text = bytearray_repr(31, 4);
            set_data_labels(SFSpellDescriptor.get(Utility.TryParseUInt16(tb_typeID.Text)));
        }
    }
}
