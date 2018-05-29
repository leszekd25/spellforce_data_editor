using System;
using System.IO;
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
        enum MODE { DEFAULT = 0, FOLLOW = 1 }

        private SFCategoryManager manager;                      //category manager to control all data
        private int selected_category_index = -1;
        private int real_category_index = -1;                   //tracer helper
        private int selected_element_index = -1;
        private category_forms.SFControl ElementDisplay;        //a control which displays all element parameters
        //these parameters control item loading behavior
        private int elementselect_next_index = 0;
        private int elementselect_last_index = 0;
        private int elementselect_refresh_size = 100;
        private int elementselect_refresh_rate = 50;
        protected List<int> current_indices;                    //list of indices corrsponding to all displayed elements
        //diff tools postponed temporarily
        private SFDataTracer tracer;

        //constructor
        public SpelllforceCFFEditor()
        {
            InitializeComponent();
            manager = new SFCategoryManager();
            manager.set_application_form(this);
            tracer = new SFDataTracer();
            current_indices = new List<int>();
        }

        //load game data
        private void loadGameDatacffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(OpenGameData.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CategorySelect.Enabled)
                    close_data();
                labelStatus.Text = "Loading...";
                ProgressBar_Main.Visible = true;
                manager.load_cff(OpenGameData.FileName, ProgressBar_Main);
                this.Text = "SpellforceDataEditor - "+OpenGameData.FileName;
                labelStatus.Text = "Ready";
                ProgressBar_Main.Visible = false; ;
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
            save_data();
        }

        private bool save_data()
        {
            DialogResult result = SaveGameData.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                labelStatus.Text = "Saving...";
                manager.save_cff(SaveGameData.FileName);
                labelStatus.Text = "Saved";
                return true;
            }
            return false;
        }

        private void set_element_display(int ind_c)
        {
            if (ElementDisplay != null)
                ElementDisplay.Dispose();
            ElementDisplay = Assembly.GetExecutingAssembly().CreateInstance(
                "SpellforceDataEditor.category_forms.Control" + (ind_c + 1).ToString())
                as category_forms.SFControl;
            ElementDisplay.set_category(manager.get_category(ind_c));
            ElementDisplay.BringToFront();
            labelDescription.SendToBack();
            SearchPanel.Controls.Clear();
            SearchPanel.Controls.Add(ElementDisplay);
        }

        private void resolve_category_index()
        {
            if (real_category_index != selected_category_index)
            {
                real_category_index = selected_category_index;
                set_element_display(real_category_index);
            }
        }

        //what happens when you choose category from a list
        private void CategorySelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelElemManipulate.Visible = false;
            ElementSelect.Enabled = true;

            SFCategory ctg = manager.get_category(CategorySelect.SelectedIndex);
            selected_category_index = CategorySelect.SelectedIndex;
            real_category_index = selected_category_index;

            ElementSelect_refresh(ctg);

            set_element_display(real_category_index);
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
            Tracer_Clear();
        }

        //what happens when you choose element from a list
        private void ElementSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            SFCategory ctg = manager.get_category(selected_category_index);

            if (ElementSelect.SelectedIndex == -1)
            {
                ElementDisplay.Visible = false;
                labelDescription.Text = "";
                return;
            }

            resolve_category_index();

            ElementDisplay.Visible = true;
            ElementDisplay.set_element(current_indices[ElementSelect.SelectedIndex]);
            ElementDisplay.show_element();

            selected_element_index = current_indices[ElementSelect.SelectedIndex];
            labelDescription.Text = ctg.get_element_description(manager, current_indices[ElementSelect.SelectedIndex]);

            Tracer_Clear();
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
            labelDescription.Text = "";
            labelStatus.Text = "Loading...";
            ProgressBar_Main.Visible = true;
            ProgressBar_Main.Value = 0;
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

        public void Tracer_Clear()
        {
            tracer.Clear();
            buttonTracerBack.Visible = false;
        }

        //when you right-click orange field, you step into the linked element edit mode
        public void Tracer_StepForward(int cat_i, int cat_e)
        {
            Console.WriteLine("step into category " + (cat_i + 1).ToString() + " elem " + cat_e.ToString());
            Console.WriteLine("from category " + (real_category_index + 1).ToString() + " elem " + selected_element_index.ToString());
            tracer.AddTrace(real_category_index, selected_element_index);
            real_category_index = cat_i;
            selected_element_index = cat_e;

            set_element_display(cat_i);
            ElementDisplay.Visible = true;
            ElementDisplay.set_element(cat_e);
            ElementDisplay.show_element();
            labelDescription.Text = manager.get_category(cat_i).get_element_description(manager, cat_e);

            buttonTracerBack.Visible = true;
        }

        public void Tracer_StepBack()
        {
            buttonTracerBack.Visible = false;
            if (!tracer.CanGoBack())
                return;
            SFDataTraceElement trace = tracer.GoBack();

            int cat_i = trace.category_index;
            int cat_e = trace.category_element;

            //not working, fix!

            real_category_index = cat_i;
            selected_element_index = cat_e;

            set_element_display(cat_i);
            ElementDisplay.Visible = true;
            ElementDisplay.set_element(cat_e);
            ElementDisplay.show_element();
            labelDescription.Text = manager.get_category(cat_i).get_element_description(manager, cat_e);

            if (tracer.CanGoBack())
                buttonTracerBack.Visible = true;
        }

        //what happens when you click search button
        private void SearchButton_Click(object sender, EventArgs e)
        {
            //determine if you can search for an item
            if (!ElementSelect.Enabled)
                return;
            if (SearchQuery.Text == "")
                return;

            resolve_category_index();

            //prepare form for a search
            bool timer_was_enabled = ElementSelect_RefreshTimer.Enabled;
            if (ElementSelect_RefreshTimer.Enabled)
            {
                ElementSelect_RefreshTimer.Stop();
                ElementSelect_RefreshTimer.Enabled = false;
            }
            int elem_found = 0;
            SFCategory ctg = manager.get_category(real_category_index);
            int columns = ctg.get_element_format().Length;
            List<int> query_result = new List<int>();

            //for progress bar
            int data_searched = 0;
            int currently_searched = 0;
            if (checkSearchDescription.Checked)
                data_searched++;

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
            data_searched += columns_searched.Count;

            //set up progress bar
            labelStatus.Text = "Searching...";
            ProgressBar_Main.Visible = true;
            ProgressBar_Main.Value = 0;

            //if searched, looks for string in element description
            
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
                currently_searched++;
            }

            //now that descriptions have been looked at, remove all elements from element selector
            ElementSelect.Items.Clear();
            current_indices.Clear();
            labelDescription.Text = "";

            //update progress bar
            ProgressBar_Main.Value = (currently_searched * ProgressBar_Main.Maximum) / data_searched;

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
                    currently_searched++;
                    ProgressBar_Main.Value = (currently_searched * ProgressBar_Main.Maximum) / data_searched;
                }
            }
            else if (radioSearchText.Checked)
            {
                string val = SearchQuery.Text;
                foreach (int col in columns_searched)
                {
                    if (ctg.get_element_format()[col] != 's')
                        continue;
                    query_result = manager.query_by_column_text(CategorySelect.SelectedIndex, col, val);
                    elem_found += query_result.Count;
                    ElementSelect_add_elements(ctg, query_result);
                    currently_searched++;
                    ProgressBar_Main.Value = (currently_searched * ProgressBar_Main.Maximum) / data_searched;
                }
            }
            else if (radioSearchFlag.Checked)
            {
                UInt32 val = Utility.TryParseUInt32(SearchQuery.Text);
                foreach (int col in columns_searched)
                {
                    query_result = manager.query_by_column_flag(CategorySelect.SelectedIndex, col, (int)val);
                    elem_found += query_result.Count;
                    ElementSelect_add_elements(ctg, query_result);
                    currently_searched++;
                    ProgressBar_Main.Value = (currently_searched * ProgressBar_Main.Maximum) / data_searched;
                }
            }

            //finishing touch
            ElementDisplay.Visible = false;
            panelElemManipulate.Visible = false;
            labelStatus.Text = "Ready";
            ProgressBar_Main.Visible = false;
            Tracer_Clear();

        }

        //what happens when you add an element to a category
        private void ButtonElemInsert_Click(object sender, EventArgs e)
        {
            if (ElementSelect.SelectedIndex == -1)
                return;

            resolve_category_index();

            int current_elem = current_indices[ElementSelect.SelectedIndex];
            SFCategory ctg = manager.get_category(real_category_index);
            SFCategoryElement elem = ctg.generate_empty_element();
            List<SFCategoryElement> elems = ctg.get_elements();
            elems.Insert(current_elem+1, elem);
            ElementSelect.Items.Insert(current_elem+1, ctg.get_element_string(manager, current_elem+1));
            for (int i = current_elem+1; i < current_indices.Count; i++)
                current_indices[i] = current_indices[i] + 1;
            current_indices.Insert(current_elem+1, current_elem+1);

            Tracer_Clear();
        }

        //what happens when you remove element from category
        private void ButtonElemRemove_Click(object sender, EventArgs e)
        {
            if (ElementSelect.SelectedIndex == -1)
                return;

            resolve_category_index();

            int current_elem = current_indices[ElementSelect.SelectedIndex];
            SFCategory ctg = manager.get_category(real_category_index);
            List<SFCategoryElement> elems = ctg.get_elements();
            for (int i = current_elem; i < current_indices.Count; i++)
                current_indices[i] = current_indices[i] - 1;
            current_indices.RemoveAt(current_elem);
            ElementSelect.Items.RemoveAt(ElementSelect.SelectedIndex);
            elems.RemoveAt(current_elem);
            ElementDisplay.Visible = false;

            Tracer_Clear();
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
            ProgressBar_Main.Value = (int)(((Single)last/(Single)elementselect_last_index)*ProgressBar_Main.Maximum);

            elementselect_next_index += elementselect_refresh_size;
            if(last != elementselect_last_index)
            {
                ElementSelect_RefreshTimer.Interval = elementselect_refresh_rate;
                ElementSelect_RefreshTimer.Start();
            }
            else
            {
                ProgressBar_Main.Visible = false;
                labelStatus.Text = "Ready";
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
        public DialogResult close_data()
        {
            //ask first to close currend gamedata.cff, if user clicks Cancel, function return immediately
            DialogResult result;
            if (!CategorySelect.Enabled)
               return DialogResult.No;
            else
                result = MessageBox.Show("Do you want to save before quitting?", "Save before quit?", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                if (!save_data())
                    return DialogResult.Cancel;
            }
            else if (result == DialogResult.Cancel)
                return result;

            //close everything
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
            real_category_index = -1;
            manager.unload_all();
            labelStatus.Text = "";
            ProgressBar_Main.Visible = false;
            ProgressBar_Main.Value = 0;
            Tracer_Clear();
            labelDescription.Text = "";
            this.Text = "SpellforceDataEditor";
            GC.Collect();

            return result;
        }

        //exit application
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AskBeforeExit(object sender, FormClosingEventArgs e)
        {
            if (CategorySelect.Enabled)
            {
                DialogResult result = close_data();
                if (result == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        private void buttonTracerBack_Click(object sender, EventArgs e)
        {
            Tracer_StepBack();
        }
    }
}
