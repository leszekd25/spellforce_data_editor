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
    public partial class Control41 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control41()
        {
            InitializeComponent();
            column_dict.Add("Description ID", new int[1] { 0 });
            column_dict.Add("Text ID", new int[1] { 1 });
        }

        private void tb_sd1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, Utility.TryParseUInt16(tb_sd1.Text));
        }

        private void tb_sd2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, Utility.TryParseUInt16(tb_sd2.Text));
        }

        public override void show_element()
        {
            tb_sd1.Text = variant_repr(0);
            tb_sd2.Text = variant_repr(1);
        }

        private void tb_sd2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(tb_sd2, 2016);
        }


        public override string get_element_string(int index)
        {
            UInt16 desc_id = (UInt16)category[index][0];
            string txt = SFCategoryManager.GetTextFromElement(category[index], 1);
            return desc_id.ToString() + " " + txt;
        }
    }
}
