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
    public partial class Control29 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control29()
        {
            InitializeComponent();
            column_dict.Add("Merchant ID", new int[1] { 0 });
            column_dict.Add("Unit ID", new int[1] { 1 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, Utility.TryParseUInt16(textBox3.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox3.Text = variant_repr(1);

            button_repr(ButtonGoto30, 2042, "Inventory", "Merchant");
            button_repr(ButtonGoto31, 2047, "Sell/Buy rate", "Merchant");
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox3, 2024);
        }

        private void ButtonGoto30_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto30, 2042);
            button_repr(ButtonGoto30, 2042, "Inventory", "Merchant");
        }

        private void ButtonGoto31_Click(object sender, EventArgs e)
        {
            button_step_into(ButtonGoto31, 2047);
            button_repr(ButtonGoto31, 2047, "Sell/Buy rate", "Merchant");
        }


        public override string get_element_string(int index)
        {
            UInt16 merchant_id = (UInt16)category[index][0];
            UInt16 unit_id = (UInt16)category[index][1];
            string txt_unit = SFCategoryManager.GetUnitName(unit_id);
            return merchant_id.ToString() + " " + txt_unit;
        }
    }
}
