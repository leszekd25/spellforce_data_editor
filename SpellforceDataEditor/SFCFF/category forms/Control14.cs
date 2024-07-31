using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control14 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2018 c2018;

        public Control14()
        {
            InitializeComponent();

            c2018 = SFCategoryManager.gamedata.c2018;
            category = c2018;

            column_dict.Add("Spell item ID", "SpellItemID");
            column_dict.Add("Effect ID", "EffectID");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2018.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2018.SetField(current_element, "EffectID", SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        public override void show_element()
        {
            textBox1.Text = c2018[current_element].SpellItemID.ToString();
            textBox2.Text = c2018[current_element].EffectID.ToString();
        }


        public override string get_element_string(int index)
        {
            UInt16 item_id = c2018[index].SpellItemID;
            UInt16 effect_id = c2018[index].EffectID;
            return $"{item_id} {SFCategoryManager.GetItemName(item_id)} | {SFCategoryManager.GetEffectName(effect_id, true)}";
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2003, textBox1.Text);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2002, textBox2.Text);
        }
    }
}
