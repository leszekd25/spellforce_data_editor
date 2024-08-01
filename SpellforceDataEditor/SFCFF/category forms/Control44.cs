using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;


namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control44 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2063 c2063;

        public Control44()
        {
            InitializeComponent();

            c2063 = SFCategoryManager.gamedata.c2063;
            category = c2063;

            column_dict.Add("Weapon type ID", "WeaponTypeID");
            column_dict.Add("Text ID", "NameID");
            column_dict.Add("Sharpness", "Sharpness");
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            c2063.SetID(current_element, SFEngine.Utility.TryParseUInt16(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2063.SetField(current_element, "NameID", SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2063.SetField(current_element, "Sharpness", SFEngine.Utility.TryParseUInt8(textBox2.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = c2063[current_element].WeaponTypeID.ToString();
            textBox1.Text = c2063[current_element].NameID.ToString();
            textBox2.Text = c2063[current_element].Sharpness.ToString();
        }


        public override string get_element_string(int index)
        {
            return $"{c2063[index].WeaponTypeID} {SFCategoryManager.GetTextByLanguage(c2063[index].NameID, SFEngine.Settings.LanguageID)}";
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox1.Text);
        }
    }
}
