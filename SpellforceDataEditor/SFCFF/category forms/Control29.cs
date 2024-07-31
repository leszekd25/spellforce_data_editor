using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control29 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2041 c2041;

        public Control29()
        {
            InitializeComponent();

            c2041 = SFCategoryManager.gamedata.c2041;
            category = c2041;

            column_dict.Add("Merchant ID", "MerchantID");
            column_dict.Add("Unit ID", "UnitID");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2041.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2041.SetField(current_element, "UnitID", SFEngine.Utility.TryParseUInt16(textBox3.Text));
        }

        public override void show_element()
        {
            textBox1.Text = c2041[current_element].MerchantID.ToString();
            textBox3.Text = c2041[current_element].UnitID.ToString();

            button_repr(ButtonGoto30, 2042);
            button_repr(ButtonGoto31, 2047);
        }


        public override string get_element_string(int index)
        {
            return $"{c2041[index].MerchantID} {SFCategoryManager.GetUnitName(c2041[index].UnitID)}";
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2024, textBox3.Text);
        }

        private void ButtonGoto30_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto30, 2042);
        }

        private void ButtonGoto31_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto31, 2047);
        }
    }
}
