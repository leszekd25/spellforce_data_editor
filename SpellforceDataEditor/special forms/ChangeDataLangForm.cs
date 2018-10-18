/*
 * This form allows user to force language change of gamedata
 * It works as follows: Form takes a text entry and displays all language variants
 * When user selects language he wants, form changes language id of ALL text entries
 * All entries which had lang ID 1, change it to lang ID of selected language, and vice versa
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.special_forms
{
    public partial class ChangeDataLangForm : Form
    {
        SFCategoryManager manager = null;

        public ChangeDataLangForm()
        {
            InitializeComponent();
            LabelDescription.Text = "Choose a line on the right which is\r\nin language you want in Spellforce\r\nPress OK to change data language";

        }

        //lists languages to allow the user to choose language
        public void connect_to_data(SFCategoryManager man)
        {
            manager = man;
            SFCategoryElement text7055 = manager.get_category(14).find_binary_element<UInt16>(0, 7055).get_copy();
            int elem_num = text7055.get_size() / 566;    //566 = sub-element's size
            for (int i = 0; i < elem_num; i++)
            {
                string single_text = Utility.CleanString(text7055.get_single_variant(i * 5 + 4));
                int lang_id = (Byte)(text7055.get_single_variant(i * 5 + 1).value);
                ListSample.Items.Add(lang_id.ToString() + " " + single_text);
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            LabelDescription.Text = "Changing data...";
            Enabled = false;
            Refresh();
            int change_to = -1;
            if (ListSample.SelectedIndex != -1)
            {
                SFCategoryElement text7055 = manager.get_category(14).find_binary_element<UInt16>(0, 7055).get_copy();
                change_to = (Byte)(text7055.get_single_variant(ListSample.SelectedIndex * 5 + 1).value);
            }
            change_data(1,change_to);
            Close();
        }

        private void change_data(int from, int to)
        {
            if ((to == -1) || (from == -1))
                return;
            if (to == from)
                return;

            Byte Bfrom = (Byte)from;
            Byte Bto = (Byte)to;

            int elem_count = manager.get_category(14).get_element_count();
            for(int i = 0; i < elem_count; i++)
            {
                bool fromExists = false;
                bool toExists = false;

                SFCategoryElement elem = manager.get_category(14).get_element(i);
                int text_count = elem.get_size() / 566;

                for (int j = 0; j < text_count; j++)
                {
                    if ((Byte)elem.get_single_variant(j * 5 + 1).value == Bto)
                        toExists = true;
                    else if ((Byte)elem.get_single_variant(j * 5 + 1).value == Bfrom)
                        fromExists = true;
                }

                if (!(fromExists && toExists))
                    continue;

                for (int j = 0; j < text_count; j++)
                {
                    if ((Byte)elem.get_single_variant(j * 5 + 1).value == Bto)
                        elem.set_single_variant(j * 5 + 1, Bfrom);
                    else if ((Byte)elem.get_single_variant(j * 5 + 1).value == Bfrom)
                        elem.set_single_variant(j * 5 + 1, Bto);
                }
            }
        }
    }
}
