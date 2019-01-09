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
    public partial class Control46 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control46()
        {
            InitializeComponent();
            column_dict.Add("Terrain ID", new int[1] { 0 });
            column_dict.Add("Unknown", new int[1] { 1 });
            column_dict.Add("Rendering flags", new int[1] { 2 });
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, Utility.TryParseUInt16(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, Utility.TryParseUInt8(textBox1.Text));
        }

        private void SetRenderFlag(int f, bool s)
        {
            int flags = (Byte)category.get_element_variant(current_element, 2).value;
            if (s)
                flags |= (1 << f);
            else
            {
                if ((flags & (1 << f)) != 0)
                    flags -= (1 << f);
            }
            set_element_variant(current_element, 2, (Byte)flags);
        }

        private void flagDepthWrite_CheckedChanged(object sender, EventArgs e)
        {
            SetRenderFlag(0, flagDepthWrite.Checked);
        }

        private void flagDepthReadOn_CheckedChanged(object sender, EventArgs e)
        {
            SetRenderFlag(1, flagDepthReadOn.Checked);
        }

        private void flagCulling_CheckedChanged(object sender, EventArgs e)
        {
            SetRenderFlag(2, flagCulling.Checked);
        }

        public override void show_element()
        {
            tb_effID.Text = variant_repr(0);
            textBox1.Text = variant_repr(1);
            int flags = (Byte)category.get_element_variant(current_element, 2).value;
            flagDepthWrite.Checked = ((flags & 1) != 0);
            flagDepthReadOn.Checked = ((flags & 2) != 0);
            flagCulling.Checked = ((flags & 4) != 0);
        }

    }
}
