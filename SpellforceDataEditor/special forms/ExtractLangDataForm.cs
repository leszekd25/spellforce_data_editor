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
    public partial class ExtractLangDataForm : Form
    {
        public ExtractLangDataForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // determine if text category exists in current gamedata
            SFCFF.SFCategory text_cat = SFCFF.SFCategoryManager.gamedata[2016];
            if(text_cat == null)
            {
                MessageBox.Show("Current gamedata does not contain text category! Aborting...");
                this.Close();
                return;
            }

            // determine if provided lang ID is valid
            byte lang_id = Utility.TryParseUInt8(textBox1.Text, 255);
            if (lang_id == 255)
            {
                MessageBox.Show("Incorrect language ID provided! Aborting...");
                this.Close();
                return;
            }

            // determine if provided new lang ID is valid
            byte new_lang_id = Utility.TryParseUInt8(textBox2.Text, 255);
            if (new_lang_id == 255)
            {
                MessageBox.Show("Incorrect language ID provided! Aborting...");
                this.Close();
                return;
            }

            // create new gamedata
            SFCFF.SFGameData new_gd = new SFCFF.SFGameData();

            // create new category
            SFCFF.SFCategory new_cat = SFCFF.SFCategory.Create(2016, 3);

            // move text entries with given ID from main gd to new gd
            for(int i = 0; i < text_cat.GetElementCount(); i++)
            {
                // find text entry with given lang ID
                for(int j = 0; j < text_cat.element_lists[i].Elements.Count(); j++)
                {
                    byte tmp_lang_id = (byte)text_cat[i, j][1];
                    if(tmp_lang_id == lang_id)
                    {
                        // add the entry to new gamedata, with new lang ID
                        SFCFF.SFCategoryElementList new_list = new SFCFF.SFCategoryElementList();
                        new_list.Elements.Add(text_cat[i, j].GetCopy());
                        new_list.Elements[new_list.Elements.Count - 1][1] = new_lang_id;
                        new_list.ElementStatus.Add(SFCFF.SFCategoryElementStatus.ADDED);          // maybe will be needed in the future
                        new_cat.element_lists.Add(new_list);
                        break;
                    }
                }
            }
            new_gd.categories.Add(2016, new_cat);

            // save gamedata
            int result = new_gd.Save("_OUTPUT.cff");
            if(result != 0)
            {
                MessageBox.Show("Failed to export text data!");
            }
            else
            {
                MessageBox.Show("Successfully exported text data.");
            }
            this.Close();
        }
    }
}
