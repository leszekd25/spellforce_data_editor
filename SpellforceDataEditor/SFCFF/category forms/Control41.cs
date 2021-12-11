using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control41 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control41()
        {
            InitializeComponent();
            column_dict.Add("Description ID", new int[1] { 0 });
            column_dict.Add("Text ID", new int[1] { 1 });
        }

        private void tb_sd1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, Utility.TryParseUInt16(tb_sd1.Text));
        }

        private void tb_sd2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, Utility.TryParseUInt16(tb_sd2.Text));
        }

        public override void show_element()
        {
            tb_sd1.Text = variant_repr(0);
            tb_sd2.Text = variant_repr(1);

            textbox_repr(tb_sd2, 2016);
        }

        private void tb_sd2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (SFCategoryManager.gamedata[2016] == null)
                    return;

                int cur_id = Utility.TryParseInt32(tb_sd2.Text);
                int ind = SFCategoryManager.gamedata[2016].GetElementIndex(cur_id);

                if ((ind == Utility.NO_INDEX)||(ind == 0))
                {
                    int new_id;
                    int new_ind;
                    if (cur_id == 0)
                    {
                        new_ind = SFCategoryManager.gamedata[2016].GetElementCount();
                        new_id = SFCategoryManager.gamedata[2016].GetElementID(new_ind - 1) + 1;
                    }
                    else
                    {
                        new_ind = SFCategoryManager.gamedata[2016].GetNextNewElementIndex(cur_id, out new_id);
                    }

                    SFCategoryElementList new_elem_list = SFCategoryManager.gamedata[2016].GetEmptyElementList();
                    new_elem_list[0][0] = (ushort)new_id;
                    SFCategoryManager.gamedata[2016].element_lists.Insert(new_ind, new_elem_list);
                    SFCategoryManager.gamedata[2016].element_status.Insert(new_ind, SFCategoryElementStatus.ADDED);
                    tb_sd2.Text = new_id.ToString();
                    tb_sd2.BackColor = Color.DarkOrange;
                }
                else
                    step_into(tb_sd2, 2016);
            }
        }


        public override string get_element_string(int index)
        {
            UInt16 desc_id = (UInt16)category[index][0];
            string txt = SFCategoryManager.GetTextFromElement(category[index], 1);
            return desc_id.ToString() + " " + txt;
        }
    }
}
