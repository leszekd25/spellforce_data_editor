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
        private int elementselect_refresh_size = 100;
        private int elementselect_refresh_rate = 50;
        private int loaded_count = 0;
        protected List<int> current_indices;                    //list of indices corrsponding to all displayed elements
        protected SFDiffTools diff;
        protected SFCategoryElement diff_current_element = null;//for checking if an element was modified before switching
        //element can be switched by:
        //-selecting another element from the list
        //-resetting category
        //-using tracer features to change element
        private SFDataTracer tracer;

        //constructor
        public SpelllforceCFFEditor()
        {
            InitializeComponent();
            manager = new SFCategoryManager();
            manager.set_application_form(this);
            diff = new SFDiffTools();
            diff.connect_to(manager);
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
                string diff_filename = OpenGameData.FileName.Replace(".cff", ".dff");
                bool diff_loaded = diff.load_diff_data(diff_filename);
                this.Text = "SpellforceDataEditor - "+OpenGameData.FileName;
                labelStatus.Text = "Ready";
                if (!diff_loaded)
                    labelStatus.Text = "Ready (diff file not found)";
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
                string diff_filename = SaveGameData.FileName.Replace(".cff", ".dff");
                diff.save_diff_data(diff_filename);
                labelStatus.Text = "Saved";
                return true;
            }
            return false;
        }

        private void diff_resolve_current_element()
        {
            if (diff_current_element == null)
                return;
            SFCategory cat = manager.get_category(real_category_index);
            if (!diff_current_element.same_as(cat.get_element(selected_element_index)))
            {
                diff.push_change(real_category_index, new SFDiffElement(SFDiffElement.DIFF_TYPE.REPLACE, selected_element_index, diff_current_element, cat.get_element(selected_element_index)));
                ElementSelect.Items[selected_element_index] = manager.get_category(selected_category_index).get_element_string(current_indices[selected_element_index]);
            }
        }

        private void diff_set_new_element()
        {
            SFCategory cat = manager.get_category(real_category_index);
            diff_current_element = cat.get_element(selected_element_index).get_copy();
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
            diff_resolve_current_element();
            diff_current_element = null;

            panelElemManipulate.Visible = false;
            ElementSelect.Enabled = true;

            SFCategory ctg = manager.get_category(CategorySelect.SelectedIndex);
            selected_category_index = CategorySelect.SelectedIndex;
            real_category_index = selected_category_index;

            ElementSelect_refresh(ctg);       //clear all elements and start loading new elements

            set_element_display(real_category_index);
            SearchPanel.Controls.Clear();
            SearchPanel.Controls.Add(ElementDisplay);

            SearchColumnID.Items.Clear();
            SearchColumnID.SelectedIndex = -1;
            SearchColumnID.Text = "";
            Dictionary<string, int[]>.KeyCollection keys = ElementDisplay.get_column_descriptions();
            foreach (string s in keys)
                SearchColumnID.Items.Add(s);

            undoCtrlZToolStripMenuItem.Enabled = diff.can_undo_changes(selected_category_index);
            redoCtrlYToolStripMenuItem.Enabled = diff.can_redo_changes(selected_category_index);

            ElementDisplay.Visible = false;
            panelSearch.Visible = true;
            ContinueSearchButton.Enabled = false;
            Tracer_Clear();
        }

        //what happens when you choose element from a list
        private void ElementSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            diff_resolve_current_element();
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
            diff_set_new_element();

            undoCtrlZToolStripMenuItem.Enabled = diff.can_undo_changes(selected_category_index);
            redoCtrlYToolStripMenuItem.Enabled = diff.can_redo_changes(selected_category_index);

            labelDescription.Text = ctg.get_element_description(current_indices[ElementSelect.SelectedIndex]);

            Tracer_Clear();
        }

        //start loading all elements from a category
        public void ElementSelect_refresh(SFCategory ctg)
        {
            ElementSelect.Items.Clear();
            current_indices.Clear();
            for (int i = 0; i < ctg.get_element_count(); i++)
                current_indices.Add(i);
            loaded_count = 0;
            labelDescription.Text = "";
            labelStatus.Text = "Loading...";
            RestartTimer();
        }

        //clears all tracer data
        public void Tracer_Clear()
        {
            tracer.Clear();
            buttonTracerBack.Visible = false;
            label_tracedesc.Text = "";
        }

        //when you right-click orange field, you step into the linked element edit mode
        public void Tracer_StepForward(int cat_i, int cat_e)
        {
            diff_resolve_current_element();

            Console.WriteLine("step into category " + (cat_i + 1).ToString() + " elem " + cat_e.ToString());
            Console.WriteLine("from category " + (real_category_index + 1).ToString() + " elem " + selected_element_index.ToString());
            tracer.AddTrace(real_category_index, selected_element_index);
            real_category_index = cat_i;
            selected_element_index = cat_e;
            diff_set_new_element();

            set_element_display(cat_i);
            ElementDisplay.Visible = true;
            ElementDisplay.set_element(cat_e);
            ElementDisplay.show_element();

            labelDescription.Text = manager.get_category(cat_i).get_element_description(cat_e);
            label_tracedesc.Text = "Category " + (cat_i + 1).ToString() + " | " + manager.get_category(cat_i).get_element_string(cat_e);
            buttonTracerBack.Visible = true;
        }

        public void Tracer_StepBack()
        {
            diff_resolve_current_element();

            buttonTracerBack.Visible = false;
            if (!tracer.CanGoBack())
                return;
            SFDataTraceElement trace = tracer.GoBack();

            int cat_i = trace.category_index;
            int cat_e = trace.category_element;

            //not working, fix!

            real_category_index = cat_i;
            selected_element_index = cat_e;
            diff_set_new_element();

            set_element_display(cat_i);
            ElementDisplay.Visible = true;
            ElementDisplay.set_element(cat_e);
            ElementDisplay.show_element();

            labelDescription.Text = manager.get_category(cat_i).get_element_description(cat_e);
            label_tracedesc.Text = "Category " + (cat_i + 1).ToString() + " | " + manager.get_category(cat_i).get_element_string(cat_e);
            if (tracer.CanGoBack())
                buttonTracerBack.Visible = true;
            else
                label_tracedesc.Text = "";
        }

        //what happens when you click search button
        private void SearchButton_Click(object sender, EventArgs e)
        {
            SFCategory cat = manager.get_category(selected_category_index);
            if (cat == null)
                return;
            string query = SearchQuery.Text;
            SearchType stype;
            if (radioSearchNumeric.Checked)
                stype = SearchType.TYPE_NUMBER;
            else if (radioSearchText.Checked)
                stype = SearchType.TYPE_STRING;
            else
                stype = SearchType.TYPE_BITFIELD;
            int col = SearchColumnID.SelectedIndex;
            if (!checkSearchByColumn.Checked)
                col = -1;
            current_indices.Clear();
            for (int i = 0; i < cat.get_element_count(); i++)
                current_indices.Add(i);
            labelStatus.Text = "Searching...";
            current_indices = SFSearchModule.Search(cat, current_indices, query, stype, col);
            //update the selection box
            ElementSelect.Items.Clear();
            loaded_count = 0;
            ContinueSearchButton.Enabled = true;
            RestartTimer();
            Tracer_Clear();
        }

        private void ContinueSearchButton_Click(object sender, EventArgs e)
        {
            SFCategory cat = manager.get_category(selected_category_index);
            if (cat == null)
                return;
            string query = SearchQuery.Text;
            SearchType stype;
            if (radioSearchNumeric.Checked)
                stype = SearchType.TYPE_NUMBER;
            else if (radioSearchText.Checked)
                stype = SearchType.TYPE_STRING;
            else
                stype = SearchType.TYPE_BITFIELD;
            int col = SearchColumnID.SelectedIndex;
            if (!checkSearchByColumn.Checked)
                col = -1;
            labelStatus.Text = "Searching...";
            current_indices = SFSearchModule.Search(cat, current_indices, query, stype, col);
            //update the selection box
            ElementSelect.Items.Clear();
            loaded_count = 0;
            ContinueSearchButton.Enabled = true;
            RestartTimer();
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
            ElementSelect.Items.Insert(current_elem+1, ctg.get_element_string(current_elem+1));
            for (int i = current_elem+1; i < current_indices.Count; i++)
                current_indices[i] = current_indices[i] + 1;
            current_indices.Insert(current_elem+1, current_elem+1);
            diff.push_change(real_category_index, new SFDiffElement(SFDiffElement.DIFF_TYPE.INSERT, current_elem, null, elem));
            undoCtrlZToolStripMenuItem.Enabled = diff.can_undo_changes(selected_category_index);
            redoCtrlYToolStripMenuItem.Enabled = diff.can_redo_changes(selected_category_index);

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
            SFCategoryElement elem = ctg.get_element(current_elem).get_copy();
            List<SFCategoryElement> elems = ctg.get_elements();
            for (int i = current_elem; i < current_indices.Count; i++)
                current_indices[i] = current_indices[i] - 1;
            current_indices.RemoveAt(current_elem);
            ElementSelect.Items.RemoveAt(ElementSelect.SelectedIndex);
            elems.RemoveAt(current_elem);
            diff_current_element = null;
            ElementDisplay.Visible = false;
            diff.push_change(real_category_index, new SFDiffElement(SFDiffElement.DIFF_TYPE.REMOVE, current_elem, elem, null));
            undoCtrlZToolStripMenuItem.Enabled = diff.can_undo_changes(selected_category_index);
            redoCtrlYToolStripMenuItem.Enabled = diff.can_redo_changes(selected_category_index);

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

            int max_items = current_indices.Count;

            int last = Math.Min(max_items, loaded_count+elementselect_refresh_size);

            for (;loaded_count <last;loaded_count++)
                ElementSelect.Items.Add(ctg.get_element_string(current_indices[loaded_count]));
            if (max_items == 0)
                ProgressBar_Main.Value = 0;
            else
                ProgressBar_Main.Value = (int)(((Single)last/(Single)max_items)*ProgressBar_Main.Maximum);

            if(last != max_items)
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

        private void RestartTimer()
        {
            ElementSelect_RefreshTimer.Enabled = true;
            ElementSelect_RefreshTimer.Interval = elementselect_refresh_rate;
            ElementSelect_RefreshTimer.Start();
            panelElemManipulate.Visible = false;
            ProgressBar_Main.Visible = true;
            ProgressBar_Main.Value = 0;
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
            ContinueSearchButton.Enabled = false;
            selected_category_index = -1;
            real_category_index = -1;
            manager.unload_all();
            labelStatus.Text = "";
            ProgressBar_Main.Visible = false;
            ProgressBar_Main.Value = 0;
            loaded_count = 0;
            current_indices.Clear();
            Tracer_Clear();
            diff_current_element = null;
            diff.clear_data();
            labelDescription.Text = "";
            undoCtrlZToolStripMenuItem.Enabled = false;
            redoCtrlYToolStripMenuItem.Enabled = false;
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

        private void undoCtrlZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undo_change();
        }

        private void redoCtrlYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            redo_change();
        }

        private void undo_change()
        {
            if (!diff.can_undo_changes(selected_category_index))
                return;

            //if element is being edited and belongs to the category, make sure to push the change before doing anything else
            if (panelElemManipulate.Visible)
                diff_resolve_current_element();

            SFDiffElement elem_change = diff.get_next_undo_change(selected_category_index);
            diff.undo_change(selected_category_index);

            if (panelElemManipulate.Visible)
            {
                if (elem_change.difference_type == SFDiffElement.DIFF_TYPE.INSERT)
                {
                    if ((selected_element_index == elem_change.difference_index+1) && (selected_category_index == real_category_index))
                    {
                        diff_current_element = null;
                        set_element_display(selected_category_index);
                    }

                    for (int i = elem_change.difference_index+1; i < current_indices.Count; i++)
                        current_indices[i] = current_indices[i] - 1;
                    current_indices.RemoveAt(elem_change.difference_index + 1);
                    ElementSelect.Items.RemoveAt(elem_change.difference_index + 1);
                }
                if (elem_change.difference_type == SFDiffElement.DIFF_TYPE.REMOVE)
                {
                    if (selected_category_index == real_category_index)
                    {
                        diff_current_element = null;
                    }

                    ElementSelect.Items.Insert(elem_change.difference_index, manager.get_category(selected_category_index).get_element_string(elem_change.difference_index));
                    for (int i = elem_change.difference_index; i < current_indices.Count; i++)
                        current_indices[i] = current_indices[i] + 1;
                    current_indices.Insert(elem_change.difference_index, elem_change.difference_index);

                    if (selected_category_index == real_category_index)
                    {
                        diff_current_element = manager.get_category(selected_category_index).get_element(current_indices[selected_element_index]).get_copy();
                    }

                }
                if(elem_change.difference_type == SFDiffElement.DIFF_TYPE.REPLACE)
                {
                    if ((selected_element_index == elem_change.difference_index) && (selected_category_index == real_category_index))
                    {
                        diff_current_element = null;
                    }

                    ElementSelect.Items[elem_change.difference_index] = manager.get_category(selected_category_index).get_element_string(current_indices[elem_change.difference_index]);

                    if (selected_category_index == real_category_index)
                    {
                        diff_current_element = manager.get_category(selected_category_index).get_element(current_indices[selected_element_index]).get_copy();
                        ElementDisplay.set_element(current_indices[selected_element_index]);
                        ElementDisplay.show_element();
                    }
                }
            }

            undoCtrlZToolStripMenuItem.Enabled = diff.can_undo_changes(selected_category_index);
            redoCtrlYToolStripMenuItem.Enabled = diff.can_redo_changes(selected_category_index);
        }

        private void redo_change()
        {
            if (!diff.can_redo_changes(selected_category_index))
                return;

            SFDiffElement elem_change = diff.get_next_redo_change(selected_category_index);
            diff.redo_change(selected_category_index);

            if (panelElemManipulate.Visible)
            {
                if (elem_change.difference_type == SFDiffElement.DIFF_TYPE.INSERT)
                {
                    if ((selected_element_index == elem_change.difference_index + 1) && (selected_category_index == real_category_index))
                    {
                        diff_current_element = null;
                    }

                    ElementSelect.Items.Insert(elem_change.difference_index + 1, manager.get_category(selected_category_index).get_element_string(current_indices[elem_change.difference_index+1]));
                    for (int i = elem_change.difference_index + 1; i < current_indices.Count; i++)
                        current_indices[i] = current_indices[i] + 1;
                    current_indices.Insert(elem_change.difference_index + 1, elem_change.difference_index + 1);

                    if (selected_category_index == real_category_index)
                    {
                        diff_current_element = manager.get_category(selected_category_index).get_element(current_indices[selected_element_index]).get_copy();
                    }
                }
                if (elem_change.difference_type == SFDiffElement.DIFF_TYPE.REMOVE)
                {
                    if ((selected_element_index == elem_change.difference_index) && (selected_category_index == real_category_index))
                    {
                        diff_current_element = null;
                        set_element_display(selected_category_index);
                    }

                    for (int i = elem_change.difference_index; i < current_indices.Count; i++)
                        current_indices[i] = current_indices[i] - 1;
                    current_indices.RemoveAt(elem_change.difference_index);
                    ElementSelect.Items.RemoveAt(elem_change.difference_index);
                }
                if (elem_change.difference_type == SFDiffElement.DIFF_TYPE.REPLACE)
                {
                    if ((selected_element_index == elem_change.difference_index) && (selected_category_index == real_category_index))
                    {
                        diff_current_element = null;
                    }

                    ElementSelect.Items[elem_change.difference_index] = manager.get_category(selected_category_index).get_element_string(current_indices[elem_change.difference_index]);

                    if (selected_category_index == real_category_index)
                    {
                        diff_current_element = manager.get_category(selected_category_index).get_element(current_indices[selected_element_index]).get_copy();
                        ElementDisplay.set_element(current_indices[selected_element_index]);
                        ElementDisplay.show_element();
                    }
                }
            }

            undoCtrlZToolStripMenuItem.Enabled = diff.can_undo_changes(selected_category_index);
            redoCtrlYToolStripMenuItem.Enabled = diff.can_redo_changes(selected_category_index);
        }

        private void SpelllforceCFFEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (e.KeyCode == Keys.Z)
                    undo_change();
                else if (e.KeyCode == Keys.Y)
                    redo_change();
            }
        }
    }
}
