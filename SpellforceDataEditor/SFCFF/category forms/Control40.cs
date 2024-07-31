using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control40 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2055 c2055;

        public Control40()
        {
            InitializeComponent();

            c2055 = SFCategoryManager.gamedata.c2055;
            category = c2055;
        }

        private void tb_req4_1_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = SFEngine.Utility.TryParseByteArray(tb_req4_1.Text, 3);

            c2055.SetField(current_element, "B1", data_array[0]);
            c2055.SetField(current_element, "B2", data_array[1]);
            c2055.SetField(current_element, "B3", data_array[2]);
        }

        public override void show_element()
        {
            tb_req4_1.Text = $"{c2055[current_element].B1:X2} {c2055[current_element].B2:X2} {c2055[current_element].B3:X2}";
        }
    }
}
