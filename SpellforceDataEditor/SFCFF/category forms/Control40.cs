using System;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control40 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control40()
        {
            InitializeComponent();
            column_dict.Add("Unknown", new int[3] { 0, 1, 2 });
        }

        private void tb_req4_1_TextChanged(object sender, EventArgs e)
        {
            Byte[] data_array = SFEngine.Utility.TryParseByteArray(tb_req4_1.Text, 3);
            set_element_variant(current_element, 0, data_array[0]);
            set_element_variant(current_element, 1, data_array[1]);
            set_element_variant(current_element, 2, data_array[2]);
        }

        public override void show_element()
        {
            tb_req4_1.Text = bytearray_repr(0, 3);
        }
    }
}
