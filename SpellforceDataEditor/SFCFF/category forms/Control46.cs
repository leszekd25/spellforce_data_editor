using System;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control46 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control46()
        {
            InitializeComponent();
            column_dict.Add("Terrain ID", new int[1] { 0 });
            column_dict.Add("Block value", new int[1] { 1 });
            column_dict.Add("Cultivation flags", new int[1] { 2 });
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt16(tb_effID.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseUInt8(textBox1.Text));
        }

        private void textBox2_Validated(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, SFEngine.Utility.TryParseUInt8(textBox2.Text));
        }

        public override void show_element()
        {
            tb_effID.Text = variant_repr(0);
            textBox1.Text = variant_repr(1);
            textBox2.Text = variant_repr(2);
        }

        public override string get_description_string(int elem_key)
        {
            Byte flags = (Byte)category[elem_key][2];
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
