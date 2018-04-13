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
    public partial class Control47 : SpellforceDataEditor.category_forms.SFControl
    {
        public Control47()
        {
            InitializeComponent();
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(tb_effID.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = variant_repr(0);
        }
    }
}
