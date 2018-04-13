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
    public partial class Control42 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control42()
        {
            InitializeComponent();
        }

        private void tb_sd3_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(tb_sd3.Text));
        }

        private void tb_sd4_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt16(tb_sd4.Text));
        }

        private void sb_sd5_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.TryParseUInt16(sb_sd5.Text));
        }

        public override void show_element()
        {
            tb_sd3.Text = variant_repr(0);
            tb_sd4.Text = variant_repr(1);
            sb_sd5.Text = variant_repr(2);
        }
    }
}
