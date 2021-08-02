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

using SpellforceDataEditor.SFCFF;

namespace SpellforceDataEditor.special_forms
{
    public partial class ChangeDataLangForm : Form
    {
        public ChangeDataLangForm()
        {
            InitializeComponent();
            LabelDescription.Text = "Choose a line on the right which is\r\nin language you want in Spellforce\r\nPress OK to change data language\r\n(DEPRECATED! USE LanguageID\r\nSETTING IN config.txt!)";

            SFCategoryElement text7055 = SFCategoryManager.gamedata[2016].FindElementBinary<UInt16>(0, 7055).GetCopy();
            int elem_num = text7055.GetSize() / 566;    //566 = sub-element's size
            for (int i = 0; i < elem_num; i++)
            {
                int lang_id = (Byte)(text7055[i * 5 + 1]);
                string single_text = text7055[i * 5 + 4].ToString();
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
                SFCategoryElement text7055 = SFCategoryManager.gamedata[2016].FindElementBinary<UInt16>(0, 7055).GetCopy();
                change_to = (Byte)(text7055[ListSample.SelectedIndex * 5 + 1]);
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

            int elem_count = SFCategoryManager.gamedata[2016].GetElementCount();
            for(int i = 0; i < elem_count; i++)
            {
                bool fromExists = false;
                bool toExists = false;

                SFCategoryElement elem = SFCategoryManager.gamedata[2016][i];
                int text_count = elem.GetSize() / 566;

                for (int j = 0; j < text_count; j++)
                {
                    if ((Byte)elem[j * 5 + 1] == Bto)
                        toExists = true;
                    else if ((Byte)elem[j * 5 + 1] == Bfrom)
                        fromExists = true;
                }

                if (!(fromExists && toExists))
                    continue;

                for (int j = 0; j < text_count; j++)
                {
                    if ((Byte)elem[j * 5 + 1] == Bto)
                        elem[j * 5 + 1] = Bfrom;
                    else if ((Byte)elem[j * 5 + 1] == Bfrom)
                        elem[j * 5 + 1] = Bto;
                }
            }
            MainForm.data.poke_data();    // so the editor knows data has changed
        }
    }
}
