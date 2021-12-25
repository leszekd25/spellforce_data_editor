using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFEngine.SFCFF;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control9 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control9()
        {
            InitializeComponent();
            column_dict.Add("Inventory item ID", new int[1] { 0 });
            column_dict.Add("Installed item ID", new int[1] { 1 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, SFEngine.Utility.TryParseUInt16(textBox2.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox2.Text = variant_repr(1);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 2003);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox2, 2003);
        }



        public override string get_element_string(int index)
        {
            UInt16 item_id1 = (UInt16)category[index][0];
            string txt1 = SFCategoryManager.GetItemName(item_id1);

            UInt16 item_id2 = (UInt16)category[index][1];
            string txt2 = SFCategoryManager.GetItemName(item_id2);

            return txt1 + " | " + txt2;
        }
    }
}
