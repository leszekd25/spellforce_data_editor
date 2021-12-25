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
    public partial class Control42 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control42()
        {
            InitializeComponent();
            column_dict.Add("Description ID", new int[1] { 0 });
            column_dict.Add("Text ID", new int[1] { 1 });
            column_dict.Add("Advanced text ID", new int[1] { 2 });
        }

        private void tb_sd3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt16(tb_sd3.Text));
        }

        private void tb_sd4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseUInt16(tb_sd4.Text));
        }

        private void sb_sd5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, SFEngine.Utility.TryParseUInt16(sb_sd5.Text));
        }

        public override void show_element()
        {
            tb_sd3.Text = variant_repr(0);
            tb_sd4.Text = variant_repr(1);
            sb_sd5.Text = variant_repr(2);
        }

        private void tb_sd4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(tb_sd4, 2016);
        }

        private void sb_sd5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(sb_sd5, 2016);
        }


        public override string get_element_string(int index)
        {
            UInt16 elem_id = (UInt16)category[index][0];
            string txt = SFCategoryManager.GetTextFromElement(category[index], 2);
            return elem_id.ToString() + " " + txt;
        }

        public override string get_description_string(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(category[index], 1);
            return "Text ID: " + txt;
        }
    }
}
