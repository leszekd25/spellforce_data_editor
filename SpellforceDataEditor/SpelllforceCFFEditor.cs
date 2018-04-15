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

        protected List<int> current_indices;

        public SpelllforceCFFEditor()
        {
            InitializeComponent();
            manager = new SFCategoryManager();
            current_indices = new List<int>();
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

            ElementSelect_refresh(ctg);

            ElementDisplay = Assembly.GetExecutingAssembly().CreateInstance(
                "SpellforceDataEditor.category_forms.Control" + (CategorySelect.SelectedIndex+1).ToString())
                as category_forms.SFControl;
            ElementDisplay.set_category(ctg);
            SearchPanel.Controls.Clear();
            SearchPanel.Controls.Add(ElementDisplay);
            panelSearch.Visible = true;
        }

        private void ElementSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ElementDisplay.set_element(current_indices[ElementSelect.SelectedIndex]);
            ElementDisplay.show_element();
        }

        public void ElementSelect_refresh(SFCategory ctg)
        {
            ElementSelect.Items.Clear();
            current_indices.Clear();
            for (int i = 0; i < ctg.get_element_count(); i++)
            {
                ElementSelect.Items.Add(ctg.get_element_string(manager, i));
                current_indices.Add(i);
            }
        }

        public void ElementSelect_add_elements(SFCategory ctg, List<int> indices)
        {
            for (int i = 0; i < indices.Count; i++)
            {
                int index = indices[i];
                if (!current_indices.Contains(index))
                {
                    ElementSelect.Items.Add(ctg.get_element_string(manager, indices[i]));
                    current_indices.Add(indices[i]);
                }
            }
        }

        private void findElementByValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            return;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (!ElementSelect.Enabled)
                return;
            if (SearchQuery.Text == "")
                return;
            ElementSelect.Items.Clear();
            current_indices.Clear();
            int elem_found = 0;
            SFCategory ctg = manager.get_category(CategorySelect.SelectedIndex);
            int columns = ctg.get_element_format().Length;
            if (radioSearchNumeric.Checked)
            {
                int val = Utility.TryParseInt32(SearchQuery.Text);
                for (int i = 0; i < columns; i++)
                {
                    List<int> query_result = manager.query_by_column_numeric(CategorySelect.SelectedIndex, i, val);
                    elem_found += query_result.Count;
                    ElementSelect_add_elements(ctg, query_result);
                }
            }
            else
            {
                string val = SearchQuery.Text;
                for (int i = 0; i < columns; i++)
                {
                    if (ctg.get_element_format()[i] != 's')
                        continue;
                    List<int> query_result = manager.query_by_column_text(CategorySelect.SelectedIndex, i, val);
                    elem_found += query_result.Count;
                    ElementSelect_add_elements(ctg, query_result);
                }
            }
        }
    }
}
