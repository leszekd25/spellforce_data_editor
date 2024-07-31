using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control47 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2049 c2049;

        public Control47()
        {
            InitializeComponent();

            c2049 = SFCategoryManager.gamedata.c2049;
            category = c2049;
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            c2049.SetID(current_element, SFEngine.Utility.TryParseUInt16(tb_effID.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = c2049[current_element].HeadID.ToString();
        }
    }
}
