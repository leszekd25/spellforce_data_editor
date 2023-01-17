using System;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control47 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control47()
        {
            InitializeComponent();
            column_dict.Add("Unknown ID", new int[1] { 0 });
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt8(tb_effID.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = variant_repr(0);
        }
    }
}
