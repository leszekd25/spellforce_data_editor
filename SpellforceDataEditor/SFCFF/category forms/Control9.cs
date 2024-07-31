using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control9 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2013 c2013;

        public Control9()
        {
            InitializeComponent();

            c2013 = SFCategoryManager.gamedata.c2013;
            category = c2013;

            column_dict.Add("Inventory item ID", "ItemID");
            column_dict.Add("Installed item ID", "InstalledScrollItemID");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2013.SetField(current_element, "ItemID", SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2013.SetField(current_element, "InstalledScrollItemID", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        public override void show_element()
        {
            Category2013Item item = c2013[current_element];
            textBox1.Text = item.ItemID.ToString();
            textBox2.Text = item.InstalledScrollItemID.ToString();
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2003, textBox1.Text);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2003, textBox2.Text);
        }

        public override string get_element_string(int index)
        {
            return $"{SFCategoryManager.GetItemName(c2013[index].ItemID)} | {SFCategoryManager.GetItemName(c2013[index].InstalledScrollItemID)}";
        }
    }
}
