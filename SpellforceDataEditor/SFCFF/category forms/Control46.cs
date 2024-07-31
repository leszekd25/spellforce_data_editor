using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control46 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2032 c2032;

        public Control46()
        {
            InitializeComponent();

            c2032 = SFCategoryManager.gamedata.c2032;
            category = c2032;

            column_dict.Add("Terrain ID", "TerrainID");
            column_dict.Add("Block value", "BlockValue");
            column_dict.Add("Cultivation flags", "CultivationFlags");
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            c2032.SetID(current_element, SFEngine.Utility.TryParseUInt16(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2032.SetField(current_element, "BlockValue", SFEngine.Utility.TryParseUInt8(textBox1.Text));
        }

        private void textBox2_Validated(object sender, EventArgs e)
        {
            c2032.SetField(current_element, "CultivationFlags", SFEngine.Utility.TryParseUInt8(textBox2.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = c2032[current_element].TerrainID.ToString();
            textBox1.Text = c2032[current_element].BlockValue.ToString();
            textBox2.Text = c2032[current_element].CultivationFlags.ToString();
        }

        public override string get_description_string(int elem_key)
        {
            Byte flags = c2032[elem_key].CultivationFlags;
            string txt = "";

            if ((flags & 0x1) == 0x1)
            {
                txt += "Allows cultivation of grain\r\n";
            }

            if ((flags & 0x2) == 0x2)
            {
                txt += "Allows cultivation of mushroom\r\n";
            }

            if ((flags & 0x4) == 0x4)
            {
                txt += "Allows cultivation of trees\r\n";
            }

            return txt;
        }
    }
}
