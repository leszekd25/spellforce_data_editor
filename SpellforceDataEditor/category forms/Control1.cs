using System;
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
            Byte[] data_array = Utility.TryParseByteArray(tb_sd1.Text, 4);
            category.set_element_variant(current_element, 21, data_array[0]);
            category.set_element_variant(current_element, 22, data_array[1]);
            category.set_element_variant(current_element, 23, data_array[2]);
            category.set_element_variant(current_element, 24, data_array[3]);
        }

        private void tb_sd2_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(tb_sd2.Text, 4);
            category.set_element_variant(current_element, 25, data_array[0]);
            category.set_element_variant(current_element, 26, data_array[1]);
            category.set_element_variant(current_element, 27, data_array[2]);
            category.set_element_variant(current_element, 28, data_array[3]);
        }

        private void tb_sd3_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(tb_sd3.Text, 4);
            category.set_element_variant(current_element, 29, data_array[0]);
            category.set_element_variant(current_element, 30, data_array[1]);
            category.set_element_variant(current_element, 31, data_array[2]);
            category.set_element_variant(current_element, 32, data_array[3]);
        }

        private void tb_sd4_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(tb_sd4.Text, 4);
            category.set_element_variant(current_element, 33, data_array[0]);
            category.set_element_variant(current_element, 34, data_array[1]);
            category.set_element_variant(current_element, 35, data_array[2]);
            category.set_element_variant(current_element, 36, data_array[3]);
        }

        private void tb_sd5_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(tb_sd5.Text, 4);
            category.set_element_variant(current_element, 37, data_array[0]);
            category.set_element_variant(current_element, 38, data_array[1]);
            category.set_element_variant(current_element, 39, data_array[2]);
            category.set_element_variant(current_element, 40, data_array[3]);
        }

        private void tb_sd6_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(tb_sd6.Text, 4);
            category.set_element_variant(current_element, 41, data_array[0]);
            category.set_element_variant(current_element, 42, data_array[1]);
            category.set_element_variant(current_element, 43, data_array[2]);
            category.set_element_variant(current_element, 44, data_array[3]);
        }

        private void tb_sd7_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(tb_sd7.Text, 4);
            category.set_element_variant(current_element, 45, data_array[0]);
            category.set_element_variant(current_element, 46, data_array[1]);
            category.set_element_variant(current_element, 47, data_array[2]);
            category.set_element_variant(current_element, 48, data_array[3]);
        }

        private void tb_sd8_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(tb_sd8.Text, 4);
            category.set_element_variant(current_element, 49, data_array[0]);
            category.set_element_variant(current_element, 50, data_array[1]);
            category.set_element_variant(current_element, 51, data_array[2]);
            category.set_element_variant(current_element, 52, data_array[3]);
        }

        private void tb_sd9_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(tb_sd9.Text, 4);
            category.set_element_variant(current_element, 53, data_array[0]);
            category.set_element_variant(current_element, 54, data_array[1]);
            category.set_element_variant(current_element, 55, data_array[2]);
            category.set_element_variant(current_element, 56, data_array[3]);
        }

        private void tb_sd10_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(tb_sd10.Text, 4);
            category.set_element_variant(current_element, 57, data_array[0]);
            category.set_element_variant(current_element, 58, data_array[1]);
            category.set_element_variant(current_element, 59, data_array[2]);
            category.set_element_variant(current_element, 60, data_array[3]);
        }

        private void tb_unk_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = Utility.TryParseByteArray(tb_unk.Text, 4);
            category.set_element_variant(current_element, 61, data_array[0]);
            category.set_element_variant(current_element, 62, data_array[1]);
            category.set_element_variant(current_element, 63, data_array[2]);
            category.set_element_variant(current_element, 64, data_array[3]);
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
            tb_sd1.Text = bytearray_repr(21, 4);
            tb_sd2.Text = bytearray_repr(25, 4);
            tb_sd3.Text = bytearray_repr(29, 4);
            tb_sd4.Text = bytearray_repr(33, 4);
            tb_sd5.Text = bytearray_repr(37, 4);
            tb_sd6.Text = bytearray_repr(41, 4);
            tb_sd7.Text = bytearray_repr(45, 4);
            tb_sd8.Text = bytearray_repr(49, 4);
            tb_sd9.Text = bytearray_repr(53, 4);
            tb_sd10.Text = bytearray_repr(57, 4);
            tb_unk.Text = bytearray_repr(61, 4);
        }
    }
}
