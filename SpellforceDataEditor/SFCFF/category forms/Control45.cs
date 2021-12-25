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
    public partial class Control45 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control45()
        {
            InitializeComponent();
            column_dict.Add("Weapon material", new int[1] { 0 });
            column_dict.Add("Text ID", new int[1] { 1 });
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt16(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = variant_repr(0);
            textBox1.Text = variant_repr(1);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 2016);
        }



        public override string get_element_string(int index)
        {
            UInt16 elem_id = (UInt16)category[index][0];
            string txt = SFCategoryManager.GetTextFromElement(category[index], 1);
            return elem_id.ToString() + " " + txt;
        }
    }
}
