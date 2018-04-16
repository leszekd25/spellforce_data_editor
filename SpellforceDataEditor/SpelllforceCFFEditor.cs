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
        private SFCategoryManager manager;                      //category manager to control all data
        private int selected_category_index;
        private category_forms.SFControl ElementDisplay;        //a control which displays all element parameters
        //these parameters control item loading behavior
        private int elementselect_next_index = 0;
        private int elementselect_last_index = 0;
        private int elementselect_refresh_size = 100;
        private int elementselect_refresh_rate = 50;
        protected List<int> current_indices;                    //list of indices corrsponding to all displayed elements

        //constructor
        public SpelllforceCFFEditor()
        {
            InitializeComponent();
            manager = new SFCategoryManager();
            current_indices = new List<int>();
        }

        //load game data
        private void loadGameDatacffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(OpenGameData.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CategorySelect.Enabled)
                    close_data();
                manager.load_cff(OpenGameData.FileName);
                CategorySelect.Enabled = true;
                for (int i = 0; i < manager.get_category_number(); i++)
                    CategorySelect.Items.Add(manager.get_category(i).get_name());
                GC.Collect();
            }
        }

        //save game data
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CategorySelect.Enabled)
                return;
            if (SaveGameData.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                manager.save_cff(SaveGameData.FileName);
            }
        }

        //what happens when you choose category from a list
        private void CategorySelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelElemManipulate.Visible = false;
            ElementSelect.Enabled = true;
            SFCategory ctg = manager.get_category(CategorySelect.SelectedIndex);
            selected_category_index = CategorySelect.SelectedIndex;

            ElementSelect_refresh(ctg);

            ElementDisplay = Assembly.GetExecutingAssembly().CreateInstance(
                "SpellforceDataEditor.category_forms.Control" + (selected_category_index + 1).ToString())
                as category_forms.SFControl;
            ElementDisplay.set_category(ctg);
            SearchPanel.Controls.Clear();
            SearchPanel.Controls.Add(ElementDisplay);
            SearchColumnID.Items.Clear();
            SearchColumnID.SelectedIndex = -1;
            SearchColumnID.Text = "";
            Dictionary<string, int[]>.KeyCollection keys = ElementDisplay.get_column_descriptions();
            foreach (string s in keys)
                SearchColumnID.Items.Add(s);
            ElementDisplay.Visible = false;
            panelSearch.Visible = true;
        }

        //what happens when you choose element from a list
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

        //start loading all elements from a category
        public void ElementSelect_refresh(SFCategory ctg)
        {
            if (ElementSelect_RefreshTimer.Enabled)
            {
                ElementSelect_RefreshTimer.Stop();
                ElementSelect_RefreshTimer.Enabled = false;
            }
            ElementSelect.Items.Clear();
            current_indices.Clear();
            ElementSelect_RefreshTimer.Enabled = true;
            elementselect_next_index = 0;
            elementselect_last_index = ctg.get_element_count();
            ElementSelect_RefreshTimer.Interval = 10;
            ElementSelect_RefreshTimer.Start();
        }

        //add elements to element selector
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

        //what happens when you click search button
        private void SearchButton_Click(object sender, EventArgs e)
        {
            //determine if you can search for an item
            if (!ElementSelect.Enabled)
                return;
            if (SearchQuery.Text == "")
                return;

            //prepare form for a search
            bool timer_was_enabled = ElementSelect_RefreshTimer.Enabled;
            if (ElementSelect_RefreshTimer.Enabled)
            {
                ElementSelect_RefreshTimer.Stop();
                ElementSelect_RefreshTimer.Enabled = false;
            }
            int elem_found = 0;
            SFCategory ctg = manager.get_category(selected_category_index);
            int columns = ctg.get_element_format().Length;

            //determine columns to search
            List<int> columns_searched = new List<int>();

            if ((checkSearchByColumn.Checked) && (SearchColumnID.Text != ""))
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

            //if searched, looks for string in element description
            List<int> query_result = new List<int>();
            if (checkSearchDescription.Checked)
            {
                string val = SearchQuery.Text;
                //all elements already displayed
                for(int i = 0; i < ElementSelect.Items.Count; i++)
                {
                    string elem_str = (string)ElementSelect.Items[i];
                    if (elem_str.Contains(val))
                    {
                        elem_found += 1;
                        query_result.Add(current_indices[i]);
                    }
                }
                //elements yet to be displayed
                if (timer_was_enabled)
                {
                    for (int i = ElementSelect.Items.Count; i < ctg.get_element_count(); i++)
                    {
                        string elem_str = ctg.get_element_string(manager, i);
                        if (elem_str.Contains(val))
                        {
                            elem_found += 1;
                            query_result.Add(i);
                        }
                    }
                }
            }

            //now that descriptions have been looked at, remove all elements from element selector
            ElementSelect.Items.Clear();
            current_indices.Clear();

            //if any elements were found in previous phase, add them immediately
            ElementSelect_add_elements(ctg, query_result);

            //search columns for value and append results to element selector
            if (radioSearchNumeric.Checked)
            {
                int val = Utility.TryParseInt32(SearchQuery.Text);
                foreach(int col in columns_searched)
                {
                    query_result = manager.query_by_column_numeric(CategorySelect.SelectedIndex, col, val);
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
                    query_result = manager.query_by_column_text(CategorySelect.SelectedIndex, col, val);
                    elem_found += query_result.Count;
                    ElementSelect_add_elements(ctg, query_result);
                }
            }

            //finishing touch
            ElementDisplay.Visible = false;
            panelElemManipulate.Visible = false;
        }

        //what happens when you add an element to a category
        private void ButtonElemInsert_Click(object sender, EventArgs e)
        {
            int current_elem = current_indices[ElementSelect.SelectedIndex];
            SFCategory ctg = manager.get_category(selected_category_index);
            SFCategoryElement elem = ctg.generate_empty_element();
            List<SFCategoryElement> elems = ctg.get_elements();
            elems.Insert(current_elem+1, elem);
            ElementSelect.Items.Insert(current_elem+1, ctg.get_element_string(manager, current_elem+1));
            for (int i = current_elem+1; i < current_indices.Count; i++)
                current_indices[i] = current_indices[i] + 1;
            current_indices.Insert(current_elem+1, current_elem+1);
        }

        //what happens when you remove element from category
        private void ButtonElemRemove_Click(object sender, EventArgs e)
        {
            int current_elem = current_indices[ElementSelect.SelectedIndex];
            SFCategory ctg = manager.get_category(selected_category_index);
            List<SFCategoryElement> elems = ctg.get_elements();
            for (int i = current_elem; i < current_indices.Count; i++)
                current_indices[i] = current_indices[i] - 1;
            current_indices.RemoveAt(current_elem);
            ElementSelect.Items.RemoveAt(ElementSelect.SelectedIndex);
            elems.RemoveAt(current_elem);
        }

        //switch column search on and off
        private void checkSearchByColumn_CheckedChanged(object sender, EventArgs e)
        {
            SearchColumnID.Enabled = checkSearchByColumn.Checked;
        }

        //this is where elements are added if category is being refreshed
        private void ElementSelect_RefreshTimer_Tick(object sender, EventArgs e)
        {
            SFCategory ctg = manager.get_category(selected_category_index);

            int last = Math.Min(elementselect_next_index + elementselect_refresh_size, elementselect_last_index);
            for (int i = elementselect_next_index; i < last; i++)
            {
                ElementSelect.Items.Add(ctg.get_element_string(manager, i));
                current_indices.Add(i);
            }

            elementselect_next_index += elementselect_refresh_size;
            if(last != elementselect_last_index)
            {
                ElementSelect_RefreshTimer.Interval = elementselect_refresh_rate;
                ElementSelect_RefreshTimer.Start();
            }
            else
            {
                ElementSelect_RefreshTimer.Enabled = false;
                panelElemManipulate.Visible = true;
            }
        }

        //close gamedata.cff
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CategorySelect.Enabled)
            {
                close_data();
            }
        }

        //actually clear all data and close gamedata.cff
        public void close_data()
        {
            if (ElementSelect_RefreshTimer.Enabled)
            {
                ElementSelect_RefreshTimer.Stop();
                ElementSelect_RefreshTimer.Enabled = false;
            }
            if(ElementDisplay != null)
                ElementDisplay.Visible = false;
            ElementSelect.Items.Clear();
            ElementSelect.Enabled = false;
            CategorySelect.Items.Clear();
            CategorySelect.Enabled = false;
            panelElemManipulate.Visible = false;
            panelSearch.Visible = false;
            selected_category_index = -1;
            manager.unload_all();
            GC.Collect();
        }

        //exit application
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CategorySelect.Enabled)
            {
                close_data();
            }
            Application.Exit();
        }
    }
}
