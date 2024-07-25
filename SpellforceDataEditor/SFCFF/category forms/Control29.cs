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

            column_dict.Add("Merchant ID", new int[1] { 0 });
            column_dict.Add("Unit ID", new int[1] { 1 });
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

            button_repr(ButtonGoto30, SFCategoryManager.gamedata.c2042, "Inventory", "Merchant");
            button_repr(ButtonGoto31, SFCategoryManager.gamedata.c2047, "Sell/Buy rate", "Merchant");
        }


        public override string get_element_string(int index)
        {
            return $"{c2041[current_element].MerchantID} {SFCategoryManager.GetUnitName(c2041[current_element].UnitID)}";
        }
    }
}
