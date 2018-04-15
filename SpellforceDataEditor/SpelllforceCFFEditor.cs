using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor
{
    public partial class SpelllforceCFFEditor : Form
    {
        private SFCategoryManager manager;
        private category_forms.SFControl ElementDisplay;
        public SpelllforceCFFEditor()
        {
            InitializeComponent();
            manager = new SFCategoryManager();
        }

        private void loadGameDatacffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(OpenGameData.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                manager.load_cff(OpenGameData.FileName);
                CategorySelect.Enabled = true;
                for (int i = 0; i < manager.get_category_number(); i++)
                    CategorySelect.Items.Add(manager.get_category(i).get_name());
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CategorySelect.Enabled == false)
                return;
            if (SaveGameData.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                manager.save_cff(SaveGameData.FileName);
            }
        }

        private void CategorySelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ElementSelect.Enabled = true;
            SFCategory ctg = manager.get_category(CategorySelect.SelectedIndex);

            ElementSelect.Items.Clear();
            for(int i = 0; i < ctg.get_element_count(); i++)
            {
                ElementSelect.Items.Add(ctg.get_element_string(manager, i));
            }

            ElementDisplay = Assembly.GetExecutingAssembly().CreateInstance(
                "SpellforceDataEditor.category_forms.Control" + (CategorySelect.SelectedIndex+1).ToString())
                as category_forms.SFControl;
            ElementDisplay.set_category(ctg);
            SFControlPanel.Controls.Clear();
            SFControlPanel.Controls.Add(ElementDisplay);
        }

        private void ElementSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ElementDisplay.set_element(ElementSelect.SelectedIndex);
            ElementDisplay.show_element();
        }

    }
}
