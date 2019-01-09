using System;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using SpellforceDataEditor.SFCFF;


namespace SpellforceDataEditor.special_forms
{
    public partial class SpelllforceCFFEditor : Form
    {
        private bool data_loaded = false;

        private int selected_category_index = -1;
        private int real_category_index = -1;                   //tracer helper
        private int selected_element_index = -1;

        private string version = "2018.12.28.1_3D";

        private SFCFF.category_forms.SFControl ElementDisplay;        //a control which displays all element parameters

        //these parameters control item loading behavior
        private int elementselect_refresh_size = 500;
        private int elementselect_refresh_rate = 50;
        private int loaded_count = 0;

        protected List<int> current_indices = new List<int>();  //list of indices corrsponding to all displayed elements

        protected SFDiffTools diff = new SFDiffTools();
        protected SFCategoryElement diff_current_element = null;//for checking if an element was modified before switching

        protected SFCategoryElement insert_copy_element = null; //if there was an element copied, it's stored here

        private SFDataTracer tracer = new SFDataTracer();

        private special_forms.ReferencesForm refs = null;
        private special_forms.ChangeDataLangForm change_lang = null;
        private special_forms.CalculatorsForm calc = null;

        //constructor
        public SpelllforceCFFEditor()
        {
            InitializeComponent();
            diff.init();

            versionToolStripMenuItem.Text = "Version " + version;
        }

        //load game data
        private void loadGameDatacffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenGameData.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                load_data();
            }
        }

        private bool load_data()
        {
            if (data_loaded)
                if (close_data() == DialogResult.Cancel)
                    return false;

            labelStatus.Text = "Loading...";
            statusStrip1.Refresh();

            int result = SFCategoryManager.load_cff(OpenGameData.FileName);
            if (result != 0)
            {
                SFCategoryManager.unload_all();
                if (result == -1)
                    labelStatus.Text = "Failed to open file " + OpenGameData.FileName + ": Block size does not match data";
                else if (result == -2)
                    labelStatus.Text = "Failed to open file " + OpenGameData.FileName + ": Invalid data";
                return false;
            }

            //POSTPONED
            //string diff_filename = OpenGameData.FileName.Replace(".cff", ".dff");
            //bool diff_loaded = diff.load_diff_data(diff_filename);

            this.Text = "SpellforceDataEditor - " + OpenGameData.FileName;
            labelStatus.Text = "Ready";
            //if (!diff_loaded)
            //    labelStatus.Text = "Ready (diff file not found)";

            changeDataLanguageToolStripMenuItem.Enabled = true;
            CategorySelect.Enabled = true;
            for (int i = 0; i < SFCategoryManager.get_category_number(); i++)
                CategorySelect.Items.Add(SFCategoryManager.get_category(i).get_name());

            data_loaded = true;

            CategorySelect.SelectedIndex = 0;

            GC.Collect();

            return true;
        }

        //save game data
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save_data();
        }

        private bool save_data()
        {
            if (!data_loaded)
                return false; ;

            CategorySelect.Focus();
            DialogResult result = SaveGameData.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                diff_resolve_current_element();

                labelStatus.Text = "Saving...";

                SFCategoryManager.save_cff(SaveGameData.FileName);

                //POSTPONED
                //string diff_filename = SaveGameData.FileName.Replace(".cff", ".dff");
                //diff.save_diff_data(diff_filename);

                labelStatus.Text = "Saved";

                return true;
            }
            return false;
        }

        //if an element was changed between selecting it and this occuring, notify diff tool that a change occured
        private void diff_resolve_current_element()
        {
            if (diff_current_element == null)
                return;

            SFCategory cat = SFCategoryManager.get_category(real_category_index);
            if (!diff_current_element.same_as(cat.get_element(selected_element_index)))
            {
                diff.push_change(real_category_index, new SFDiffElement(SFDiffElement.DIFF_TYPE.REPLACE, selected_element_index, diff_current_element, cat.get_element(selected_element_index)));
                diff_current_element = cat.get_element(selected_element_index).get_copy();
            }
        }

        //sets a new element for comparison
        private void diff_set_new_element()
        {
            SFCategory cat = SFCategoryManager.get_category(real_category_index);
            diff_current_element = cat.get_element(selected_element_index).get_copy();
        }

        //spawns a new control to display element data
        private void set_element_display(int ind_c)
        {
            if (ElementDisplay != null)
                ElementDisplay.Dispose();

            ElementDisplay = Assembly.GetExecutingAssembly().CreateInstance(
                "SpellforceDataEditor.SFCFF.category_forms.Control" + (ind_c + 1).ToString())
                as SFCFF.category_forms.SFControl;
            ElementDisplay.set_category(SFCategoryManager.get_category(ind_c));
            ElementDisplay.BringToFront();

            labelDescription.SendToBack();

            SearchPanel.Controls.Clear();
            SearchPanel.Controls.Add(ElementDisplay);
        }

        //restores element display to show elements from currently selected category
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
            Focus();

            diff_resolve_current_element();
            diff_current_element = null;

            panelElemManipulate.Visible = false;
            panelElemCopy.Visible = false;
            ElementSelect.Enabled = true;

            SFCategory ctg = SFCategoryManager.get_category(CategorySelect.SelectedIndex);
            if (selected_category_index != CategorySelect.SelectedIndex)
            {
                ButtonElemInsert.BackColor = SystemColors.Control;
                insert_copy_element = null;
            }
            selected_category_index = CategorySelect.SelectedIndex;
            real_category_index = selected_category_index;

            ElementSelect_refresh(ctg);       //clear all elements and start loading new elements

            set_element_display(real_category_index);
            ElementDisplay.Visible = false;

            SearchPanel.Controls.Clear();
            SearchPanel.Controls.Add(ElementDisplay);
            SearchColumnID.Items.Clear();
            SearchColumnID.SelectedIndex = -1;
            SearchColumnID.Text = "";
            Dictionary<string, int[]>.KeyCollection keys = ElementDisplay.get_column_descriptions();
            foreach (string s in keys)
                SearchColumnID.Items.Add(s);
            panelSearch.Visible = true;
            ClearSearchButton.Enabled = false;
            ContinueSearchButton.Enabled = false;

            ButtonElemInsert.BackColor = SystemColors.Control;

            undoCtrlZToolStripMenuItem.Enabled = diff.can_undo_changes(selected_category_index);
            redoCtrlYToolStripMenuItem.Enabled = diff.can_redo_changes(selected_category_index);

            Tracer_Clear();
        }

        //what happens when you choose element from a list
        private void ElementSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            diff_resolve_current_element();

            SFCategory ctg = SFCategoryManager.get_category(selected_category_index);

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

            if (MainForm.viewer != null)
                MainForm.viewer.GenerateScene(real_category_index, selected_element_index);
        }

        //start loading all elements from a category
        public void ElementSelect_refresh(SFCategory ctg)
        {
            ElementSelect.Items.Clear();
            current_indices.Clear();

            for (int i = 0; i < ctg.get_element_count(); i++)
                current_indices.Add(i);

            labelDescription.Text = "";
            labelStatus.Text = "Loading...";

            loaded_count = 0;
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
        public void Tracer_StepForward(int cat_i, int cat_e, bool log_trace = true)
        {
            //check if element exists
            if (SFCategoryManager.get_category(cat_i) == null)
                return;
            if ((cat_e >= SFCategoryManager.get_category(cat_i).get_element_count())||(cat_e < 0))
                return;

            CategorySelect.Focus();

            diff_resolve_current_element();

            //for now it's like this
            //todo: allow tracing no matter the state
            if(log_trace)
                tracer.AddTrace(real_category_index, selected_element_index);

            real_category_index = cat_i;
            selected_element_index = cat_e;
            diff_set_new_element();

            set_element_display(cat_i);
            if (SFCategoryManager.get_category(cat_i).get_element(cat_e) != null)
            {
                ElementDisplay.Visible = true;
                ElementDisplay.set_element(cat_e);
                ElementDisplay.show_element();
                if (MainForm.viewer != null)
                    MainForm.viewer.GenerateScene(cat_i, cat_e);
            }

            labelDescription.Text = SFCategoryManager.get_category(cat_i).get_element_description(cat_e);

            label_tracedesc.Text = "Category " + (cat_i + 1).ToString() + " | " + SFCategoryManager.get_category(cat_i).get_element_string(cat_e);
            buttonTracerBack.Visible = true;
        }

        //when you press Back, you step out and return to previously viewed element
        public void Tracer_StepBack()
        {
            CategorySelect.Focus();

            diff_resolve_current_element();

            buttonTracerBack.Visible = false;

            if (!tracer.CanGoBack())
                return;

            SFDataTraceElement trace = tracer.GoBack();
            int cat_i = trace.category_index;
            int cat_e = trace.category_element;

            real_category_index = cat_i;
            selected_element_index = cat_e;
            if (MainForm.viewer != null)
                MainForm.viewer.GenerateScene(cat_i, cat_e);

            diff_set_new_element();

            set_element_display(cat_i);
            ElementDisplay.Visible = true;
            ElementDisplay.set_element(cat_e);
            ElementDisplay.show_element();

            labelDescription.Text = SFCategoryManager.get_category(cat_i).get_element_description(cat_e);

            label_tracedesc.Text = "Category " + (cat_i + 1).ToString() + " | " + SFCategoryManager.get_category(cat_i).get_element_string(cat_e);
            if (tracer.CanGoBack())
                buttonTracerBack.Visible = true;
            else
                label_tracedesc.Text = "";
        }

        //searches elements within entire category
        private void SearchButton_Click(object sender, EventArgs e)
        {
            SFCategory cat = SFCategoryManager.get_category(selected_category_index);
            if (cat == null)
                return;

            ProgressBar_Main.Visible = true;

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

            statusStrip1.Refresh();

            current_indices = SFSearchModule.Search(cat, current_indices, query, stype, col, ProgressBar_Main);

            ElementSelect.Items.Clear();

            ContinueSearchButton.Enabled = true;
            ClearSearchButton.Enabled = true;

            loaded_count = 0;
            RestartTimer();

            Tracer_Clear();
        }

        //searches elements within elements on the list
        private void ContinueSearchButton_Click(object sender, EventArgs e)
        {
            SFCategory cat = SFCategoryManager.get_category(selected_category_index);
            if (cat == null)
                return;

            ProgressBar_Main.Visible = true;

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

            current_indices = SFSearchModule.Search(cat, current_indices, query, stype, col, ProgressBar_Main);

            ElementSelect.Items.Clear();

            ContinueSearchButton.Enabled = true;

            loaded_count = 0;
            RestartTimer();

            Tracer_Clear();
        }

        //what happens when you add an element to a category
        //can copy stored elements
        private void ButtonElemInsert_Click(object sender, EventArgs e)
        {
            if (ElementSelect.SelectedIndex == -1)
                return;

            resolve_category_index();

            int current_elem = current_indices[ElementSelect.SelectedIndex];

            SFCategory ctg = SFCategoryManager.get_category(real_category_index);
            SFCategoryElement elem;
            if (insert_copy_element == null)
                elem = ctg.generate_empty_element();
            else
                elem = insert_copy_element.get_copy();

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

            SFCategory ctg = SFCategoryManager.get_category(real_category_index);
            SFCategoryElement elem = ctg.get_element(current_elem).get_copy();

            List<SFCategoryElement> elems = ctg.get_elements();
            for (int i = current_elem; i < current_indices.Count; i++)
                current_indices[i] = current_indices[i] - 1;
            current_indices.RemoveAt(current_elem);
            ElementSelect.Items.RemoveAt(ElementSelect.SelectedIndex);
            elems.RemoveAt(current_elem);

            ElementDisplay.Visible = false;

            diff_current_element = null;
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
            SFCategory ctg = SFCategoryManager.get_category(selected_category_index);

            int max_items = current_indices.Count;
            int last = Math.Min(max_items, loaded_count+elementselect_refresh_size);

            for (;loaded_count <last;loaded_count++)
                ElementSelect.Items.Add(ctg.get_element_string(current_indices[loaded_count]));

            if (max_items == 0)
                ProgressBar_Main.Value = 0;
            else
            {
                ProgressBar_Main.Value = (int)(((Single)last / (Single)max_items) * ProgressBar_Main.Maximum);
            }

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
                
                if (max_items == ctg.get_element_count())
                {
                    panelElemManipulate.Visible = true;
                }
                panelElemCopy.Visible = true;

                changeDataLanguageToolStripMenuItem.Enabled = true;
            }
        }

        //timer can be restarted if elements are to be gradually filled into the list again
        private void RestartTimer()
        {
            ElementSelect_RefreshTimer.Enabled = true;
            ElementSelect_RefreshTimer.Interval = elementselect_refresh_rate;
            ElementSelect_RefreshTimer.Start();

            panelElemManipulate.Visible = false;
            panelElemCopy.Visible = false;

            ProgressBar_Main.Visible = true;
            ProgressBar_Main.Value = 0;

            changeDataLanguageToolStripMenuItem.Enabled = false;
        }

        //close gamedata.cff
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (data_loaded)
                close_data();
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
            ElementSelect.Items.Clear();
            ElementSelect.Enabled = false;
            current_indices.Clear();
            loaded_count = 0;

            panelElemManipulate.Visible = false;
            panelElemCopy.Visible = false;
            ButtonElemInsert.BackColor = SystemColors.Control;
            insert_copy_element = null;

            if (ElementDisplay != null)
                ElementDisplay.Visible = false;
            labelDescription.Text = "";

            CategorySelect.Items.Clear();
            CategorySelect.Enabled = false;

            panelSearch.Visible = false;
            ContinueSearchButton.Enabled = false;

            selected_category_index = -1;
            real_category_index = -1;
            selected_element_index = -1;

            labelStatus.Text = "";
            ProgressBar_Main.Visible = false;
            ProgressBar_Main.Value = 0;
            statusStrip1.Refresh();

            undoCtrlZToolStripMenuItem.Enabled = false;
            redoCtrlYToolStripMenuItem.Enabled = false;
            changeDataLanguageToolStripMenuItem.Enabled = false;

            diff_current_element = null;

            SFCategoryManager.unload_all();
            diff.clear_data();
            Tracer_Clear();

            data_loaded = false;

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
            if (data_loaded)
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

        //undoes the last change within category
        private void undo_change()
        {
            if (!diff.can_undo_changes(selected_category_index))
                return;

            //if element is being edited and belongs to the category, make sure to push the change before doing anything else
            if (panelElemManipulate.Visible)
                diff_resolve_current_element();

            validate_focused_control();

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

                    ElementSelect.Items.Insert(elem_change.difference_index, SFCategoryManager.get_category(selected_category_index).get_element_string(elem_change.difference_index));
                    for (int i = elem_change.difference_index; i < current_indices.Count; i++)
                        current_indices[i] = current_indices[i] + 1;
                    current_indices.Insert(elem_change.difference_index, elem_change.difference_index);

                    if (selected_category_index == real_category_index)
                    {
                        diff_current_element = SFCategoryManager.get_category(selected_category_index).get_element(selected_element_index).get_copy();
                    }

                }

                if(elem_change.difference_type == SFDiffElement.DIFF_TYPE.REPLACE)
                {
                    if ((selected_element_index == elem_change.difference_index) && (selected_category_index == real_category_index))
                    {
                        diff_current_element = null;
                    }

                    ElementSelect.Items[elem_change.difference_index] = SFCategoryManager.get_category(selected_category_index).get_element_string(elem_change.difference_index);

                    if ((selected_category_index == real_category_index)&&(selected_element_index != -1))
                    {
                        diff_current_element = SFCategoryManager.get_category(selected_category_index).get_element(selected_element_index).get_copy();
                        ElementDisplay.set_element(selected_element_index);
                        ElementDisplay.show_element();
                    }
                }
            }

            undoCtrlZToolStripMenuItem.Enabled = diff.can_undo_changes(selected_category_index);
            redoCtrlYToolStripMenuItem.Enabled = diff.can_redo_changes(selected_category_index);
        }

        //redoes previously undone change within category
        private void redo_change()
        {
            if (!diff.can_redo_changes(selected_category_index))
                return;

            validate_focused_control();

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

                    for (int i = elem_change.difference_index + 1; i < current_indices.Count; i++)
                        current_indices[i] = current_indices[i] + 1;
                    current_indices.Insert(elem_change.difference_index + 1, elem_change.difference_index + 1);
                    ElementSelect.Items.Insert(elem_change.difference_index + 1, SFCategoryManager.get_category(selected_category_index).get_element_string(elem_change.difference_index+1));

                    if (selected_category_index == real_category_index)
                    {
                        diff_current_element = SFCategoryManager.get_category(selected_category_index).get_element(selected_element_index).get_copy();
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

                    ElementSelect.Items[elem_change.difference_index] = SFCategoryManager.get_category(selected_category_index).get_element_string(elem_change.difference_index);

                    if (selected_category_index == real_category_index)
                    {
                        diff_current_element = SFCategoryManager.get_category(selected_category_index).get_element(selected_element_index).get_copy();
                        ElementDisplay.set_element(selected_element_index);
                        ElementDisplay.show_element();
                    }
                }
            }

            undoCtrlZToolStripMenuItem.Enabled = diff.can_undo_changes(selected_category_index);
            redoCtrlYToolStripMenuItem.Enabled = diff.can_redo_changes(selected_category_index);
        }

        //stores copied element to be pasted elsewhere
        private void ButtonElemCopy_Click(object sender, EventArgs e)
        {
            if (ElementSelect.SelectedIndex == -1)
                return;

            insert_copy_element = SFCategoryManager.get_category(selected_category_index).get_element(selected_element_index).get_copy();
            ButtonElemInsert.BackColor = Color.Yellow;
        }

        //clears stored copy
        private void ButtonElemClear_Click(object sender, EventArgs e)
        {
            if (insert_copy_element == null)
                return;

            insert_copy_element = null;
            ButtonElemInsert.BackColor = SystemColors.Control;
        }

        //if there were elements searched, restores list to display whole category data
        private void ClearSearchButton_Click(object sender, EventArgs e)
        {
            ContinueSearchButton.Enabled = false;
            ClearSearchButton.Enabled = false;
            ElementSelect_refresh(SFCategoryManager.get_category(selected_category_index));
        }

        //special option to change game language
        private void changeDataLanguageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            change_lang = new special_forms.ChangeDataLangForm();

            change_lang.ShowDialog();

            change_lang = null;

            labelStatus.Text = "Done";
        }

        //external
        public int get_selected_category_index()
        {
            return selected_category_index;
        }

        //called from the outside, updates element name on the list
        //very ugly, due to lack of knowledge of windows' event handling...
        public void external_set_element_select_string(SFCategory ctg, int elem_index)
        {
            if (selected_category_index != real_category_index)
                return;
            if (SFCategoryManager.get_category(selected_category_index) != ctg)
                return;
            int index = Utility.find_binary_index(current_indices, elem_index);
            if (index != -1)
            {
                ElementSelect.SelectedIndexChanged -= new System.EventHandler(this.ElementSelect_SelectedIndexChanged);
                ElementSelect.Items[index] = ctg.get_element_string(elem_index);
                ElementSelect.SelectedIndexChanged += new System.EventHandler(this.ElementSelect_SelectedIndexChanged);
            }
        }

        //used if you want to keep editing one element
        private void validate_focused_control()
        {
            if (ElementDisplay == null)
                return;

            foreach(Control c in ElementDisplay.Controls)
            {
                if (c.Focused)
                {
                    ElementDisplay.Focus();
                    c.Focus();
                }
            }
        }

        private void findAllReferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if((real_category_index == -1)||(selected_element_index == -1))
                return;

            if (refs == null)
            {
                refs = new special_forms.ReferencesForm();
                refs.FormClosed += new FormClosedEventHandler(this.refs_FormClosed);
                refs.Show();
            }
            else
                refs.BringToFront();
            refs.set_referenced_element(this, real_category_index, selected_element_index);
        }

        private void refs_FormClosed(object sender, FormClosedEventArgs e)
        {
            refs = null;
        }

        private void calculatorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (calc == null)
            {
                calc = new special_forms.CalculatorsForm();
                calc.FormClosed += new FormClosedEventHandler(this.calc_FormClosed);
                calc.Show();
            }
            else
                calc.BringToFront();
        }

        private void calc_FormClosed(object sender, FormClosedEventArgs e)
        {
            calc = null;
        }
    }
}
