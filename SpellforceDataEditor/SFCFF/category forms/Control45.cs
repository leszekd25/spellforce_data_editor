using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control45 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2064 c2064;

        public Control45()
        {
            InitializeComponent();

            c2064 = SFCategoryManager.gamedata.c2064;
            category = c2064;

            column_dict.Add("Weapon material", "WeaponMaterialID");
            column_dict.Add("Text ID", "NameID");
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            c2064.SetID(current_element, SFEngine.Utility.TryParseUInt16(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2064.SetField(current_element, "NameID", SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = c2064[current_element].WeaponMaterialID.ToString();
            textBox1.Text = c2064[current_element].NameID.ToString();
        }


        public override string get_element_string(int index)
        {
            return $"{c2064[index].WeaponMaterialID} {SFCategoryManager.GetTextByLanguage(c2064[index].NameID, SFEngine.Settings.LanguageID)}";
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox1.Text);
        }
    }
}
