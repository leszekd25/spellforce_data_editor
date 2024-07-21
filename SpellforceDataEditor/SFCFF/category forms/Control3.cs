using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control3 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2056 c2056;

        public Control3()
        {
            InitializeComponent();

            c2056 = SFCategoryManager.gamedata.c2056;
            category = c2056;

            column_dict.Add("Unknown", new int[6] { 0, 1, 2, 3, 4, 5 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // dont actually do anything here
        }

        public override void show_element()
        {
            Category2056Item item = c2056[current_element];
            textBox1.Text = $"{item.GetData(0).ToString("X")} {item.GetData(1).ToString("X")} {item.GetData(2).ToString("X")} {item.GetData(3).ToString("X")} {item.GetData(4).ToString("X")} {item.GetData(5).ToString("X")}";
        }
    }
}
