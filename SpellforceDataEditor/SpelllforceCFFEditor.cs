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
            SearchColumnID.Items.Clear();
            Dictionary<string, int[]>.KeyCollection keys = ElementDisplay.get_column_descriptions();
            foreach (string s in keys)
                SearchColumnID.Items.Add(s);
            ElementDisplay.Visible = false;
            panelSearch.Visible = true;
            panelElemManipulate.Visible = true;
        }

        private void ElementSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ElementSelect.SelectedIndex == -1)
            {
                ElementDisplay.Visible = false;
                return;
            }
            ElementDisplay.Visible = true;
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
            panelElemManipulate.Visible = false;
            ElementSelect.Items.Clear();
            current_indices.Clear();
            int elem_found = 0;
            SFCategory ctg = manager.get_category(CategorySelect.SelectedIndex);
            int columns = ctg.get_element_format().Length;

            //determine columns to search
            List<int> columns_searched = new List<int>();
            if (checkSearchByColumn.Checked)
            {
                int[] query_columns = ElementDisplay.get_column_index(SearchColumnID.Text);
                foreach(int i in query_columns)
                    columns_searched.Add(i);
            }
            else
            {
                for (int i = 0; i < columns; i++)
                    columns_searched.Add(i);
            }

            //search columns for value and append results to the list of results
            if (radioSearchNumeric.Checked)
            {
                int val = Utility.TryParseInt32(SearchQuery.Text);
                foreach(int col in columns_searched)
                {
                    List<int> query_result = manager.query_by_column_numeric(CategorySelect.SelectedIndex, col, val);
                    elem_found += query_result.Count;
                    ElementSelect_add_elements(ctg, query_result);
                }
            }
            else
            {
                string val = SearchQuery.Text;
                foreach (int col in columns_searched)
                {
                    if (ctg.get_element_format()[col] != 's')
                        continue;
                    List<int> query_result = manager.query_by_column_text(CategorySelect.SelectedIndex, col, val);
                    elem_found += query_result.Count;
                    ElementSelect_add_elements(ctg, query_result);
                }
            }
            ElementDisplay.Visible = false;
        }

        private void ButtonElemInsert_Click(object sender, EventArgs e)
        {
            int current_elem = current_indices[ElementSelect.SelectedIndex];
            SFCategory ctg = manager.get_category(CategorySelect.SelectedIndex);
            SFCategoryElement elem = new SFCategoryElement();
            string format_elem = ctg.get_element_format();
            foreach(char c in format_elem)
            {
                elem.add_single_variant(ctg.empty_variant(c));
            }
            List<SFCategoryElement> elems = ctg.get_elements();
            elems.Insert(current_elem+1, elem);
            ElementSelect.Items.Insert(current_elem+1, ctg.get_element_string(manager, current_elem+1));
            current_indices.Insert(current_elem+1, current_elem+1);
        }

        private void ButtonElemRemove_Click(object sender, EventArgs e)
        {
            int current_elem = current_indices[ElementSelect.SelectedIndex];
            SFCategory ctg = manager.get_category(CategorySelect.SelectedIndex);
            List<SFCategoryElement> elems = ctg.get_elements();
            current_indices.RemoveAt(current_elem);
            ElementSelect.Items.RemoveAt(ElementSelect.SelectedIndex);
            elems.RemoveAt(current_elem);
        }

        private void checkSearchByColumn_CheckedChanged(object sender, EventArgs e)
        {
            SearchColumnID.Enabled = checkSearchByColumn.Checked;
        }
    }
}
