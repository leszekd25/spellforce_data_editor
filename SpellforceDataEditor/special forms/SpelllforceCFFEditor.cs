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
        public bool data_loaded { get; private set; } = false;
        public bool data_changed { get; private set; } = false;
        public bool synchronized_with_mapeditor { get; private set; } = false;   // this blocks undo/redo if true

        private int selected_category_index = -1;
        private int real_category_index = -1;                   //tracer helper
        private int selected_element_index = -1;

        private SFCFF.category_forms.SFControl ElementDisplay;        //a control which displays all element parameters

        //these parameters control item loading behavior
        private int elementselect_refresh_size = 1000;
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

            if(MainForm.mapedittool != null)
            {
                mapeditor_set_gamedata(SFCFF.SFCategoryManager.gamedata);
                MessageBox.Show("Gamedata editor is now synchronized with map editor! Any changes saved will permanently alter gamedata in your Spellforce directory.");
            }
        }

        //load game data
        private void loadGameDatacffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenGameData.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                load_data();
            }
        }

        public bool load_data()
        {
            if (data_loaded)
                if (close_data() == DialogResult.Cancel)
                    return false;

            labelStatus.Text = "Loading...";
            statusStrip1.Refresh();

            int result = SFCategoryManager.Load(OpenGameData.FileName);
            if (result != 0)
            {
                SFCategoryManager.UnloadAll();
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
            for (int i = 0; i < SFGameData.categoryNumber; i++)
                CategorySelect.Items.Add(SFCategoryManager.gamedata[i].GetName());

            data_loaded = true;
            data_changed = false;
            synchronized_with_mapeditor = false;
            SetUndoRedoEnabled(true);

            CategorySelect.SelectedIndex = 0;

            GC.Collect();

            return true;
        }

        public void mapeditor_set_gamedata(SFGameData gd)
        {
            SFCategoryManager.manual_SetGamedata(gd);

            this.Text = "SpellforceDataEditor - synchronized with MapEditor";
            labelStatus.Text = "Ready";
            //if (!diff_loaded)
            //    labelStatus.Text = "Ready (diff file not found)";

            changeDataLanguageToolStripMenuItem.Enabled = true;
            CategorySelect.Enabled = true;
            for (int i = 0; i < SFGameData.categoryNumber; i++)
                CategorySelect.Items.Add(SFCategoryManager.gamedata[i].GetName());

            data_loaded = true;
            data_changed = false;
            synchronized_with_mapeditor = true;
            SetUndoRedoEnabled(false);

            CategorySelect.SelectedIndex = 0;
        }

        //save game data
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save_data();
        }

        public bool save_data()
        {
            if (!data_loaded)
                return false;

            CategorySelect.Focus();
            if (synchronized_with_mapeditor)    // dont ask when synchronized
            {
                diff_resolve_current_element();

                labelStatus.Text = "Saving...";

                SFCategoryManager.Save(SFUnPak.SFUnPak.game_directory_name + "\\data\\GameData.cff");

                labelStatus.Text = "Saved";
                
                data_changed = false;

                return true;
            }
            else
            {
                DialogResult result = SaveGameData.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    diff_resolve_current_element();

                    labelStatus.Text = "Saving...";

                    SFCategoryManager.Save(SaveGameData.FileName);

                    //POSTPONED
                    //string diff_filename = SaveGameData.FileName.Replace(".cff", ".dff");
                    //diff.save_diff_data(diff_filename);

                    labelStatus.Text = "Saved";

                    data_changed = false;

                    return true;
                }
            }
            return false;
        }

        //if an element was changed between selecting it and this occuring, notify diff tool that a change occured
        private void diff_resolve_current_element()
        {
            if (diff_current_element == null)
                return;

            SFCategory cat = SFCategoryManager.gamedata[real_category_index];
            if (!diff_current_element.SameAs(cat[selected_element_index]))
            {
                data_changed = true;
                if(!synchronized_with_mapeditor)
                    diff.push_change(real_category_index, new SFDiffElement(SFDiffElement.DIFF_TYPE.REPLACE, selected_element_index, diff_current_element, cat[selected_element_index]));
                diff_current_element = cat[selected_element_index].GetCopy();
            }
        }

        //sets a new element for comparison
        private void diff_set_new_element()
        {
            SFCategory cat = SFCategoryManager.gamedata[real_category_index];
            diff_current_element = cat[selected_element_index].GetCopy();
        }

        //spawns a new control to display element data
        private void set_element_display(int ind_c)
        {
            if (ElementDisplay != null)
                ElementDisplay.Dispose();

            ElementDisplay = Assembly.GetExecutingAssembly().CreateInstance(
                "SpellforceDataEditor.SFCFF.category_forms.Control" + (ind_c + 1).ToString())
                as SFCFF.category_forms.SFControl;
            ElementDisplay.set_category(SFCategoryManager.gamedata[ind_c]);
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
            if (CategorySelect.SelectedIndex == -1)
                return;

            Focus();

            diff_resolve_current_element();
            diff_current_element = null;

            panelElemManipulate.Visible = false;
            panelElemCopy.Visible = false;
            ElementSelect.Enabled = true;

            SFCategory ctg = SFCategoryManager.gamedata[CategorySelect.SelectedIndex];
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

            SFCategory ctg = SFCategoryManager.gamedata[selected_category_index];

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

            labelDescription.Text = ctg.GetElementDescription(current_indices[ElementSelect.SelectedIndex]);

            Tracer_Clear();

            if (MainForm.viewer != null)
                MainForm.viewer.GenerateScene(real_category_index, selected_element_index);

            if(refs!=null)
                refs.set_referenced_element(real_category_index, selected_element_index);
        }

        //start loading all elements from a category
        public void ElementSelect_refresh(SFCategory ctg)
        {
            ElementSelect.Items.Clear();
            current_indices.Clear();

            for (int i = 0; i < ctg.GetElementCount(); i++)
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
            if (SFCategoryManager.gamedata[cat_i] == null)
                return;
            if ((cat_e >= SFCategoryManager.gamedata[cat_i].GetElementCount())||(cat_e < 0))
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
            if (SFCategoryManager.gamedata[cat_i][cat_e] != null)
            {
                ElementDisplay.Visible = true;
                ElementDisplay.set_element(cat_e);
                ElementDisplay.show_element();
                if (MainForm.viewer != null)
                    MainForm.viewer.GenerateScene(cat_i, cat_e);
            }

            labelDescription.Text = SFCategoryManager.gamedata[cat_i].GetElementDescription(cat_e);

            label_tracedesc.Text = "Category " + (cat_i + 1).ToString() + " | " + SFCategoryManager.gamedata[cat_i].GetElementString(cat_e);
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

            labelDescription.Text = SFCategoryManager.gamedata[cat_i].GetElementDescription(cat_e);

            label_tracedesc.Text = "Category " + (cat_i + 1).ToString() + " | " + SFCategoryManager.gamedata[cat_i].GetElementString(cat_e);
            if (tracer.CanGoBack())
                buttonTracerBack.Visible = true;
            else
                label_tracedesc.Text = "";
        }

        //searches elements within entire category
        private void SearchButton_Click(object sender, EventArgs e)
        {
            SFCategory cat = SFCategoryManager.gamedata[selected_category_index];
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
            for (int i = 0; i < cat.GetElementCount(); i++)
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
            SFCategory cat = SFCategoryManager.gamedata[selected_category_index];
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

            SFCategory ctg = SFCategoryManager.gamedata[real_category_index];
            SFCategoryElement elem;
            if (insert_copy_element == null)
                elem = ctg.GetEmptyElement();
            else
                elem = insert_copy_element.GetCopy();

            List<SFCategoryElement> elems = ctg.elements;
            elems.Insert(current_elem+1, elem);
            ElementSelect.Items.Insert(current_elem+1, ctg.GetElementString(current_elem+1));
            for (int i = current_elem+1; i < current_indices.Count; i++)
                current_indices[i] = current_indices[i] + 1;
            current_indices.Insert(current_elem+1, current_elem+1);

            if (!synchronized_with_mapeditor)
            {
                diff.push_change(real_category_index, new SFDiffElement(SFDiffElement.DIFF_TYPE.INSERT, current_elem, null, elem));

                undoCtrlZToolStripMenuItem.Enabled = diff.can_undo_changes(selected_category_index);
                redoCtrlYToolStripMenuItem.Enabled = diff.can_redo_changes(selected_category_index);
            }

            Tracer_Clear();
        }

        //what happens when you remove element from category
        private void ButtonElemRemove_Click(object sender, EventArgs e)
        {
            if (ElementSelect.SelectedIndex == -1)
                return;

            resolve_category_index();

            int current_elem = current_indices[ElementSelect.SelectedIndex];

            SFCategory ctg = SFCategoryManager.gamedata[real_category_index];
            SFCategoryElement elem = ctg[current_elem].GetCopy();

            List<SFCategoryElement> elems = ctg.elements;
            for (int i = current_elem; i < current_indices.Count; i++)
                current_indices[i] = current_indices[i] - 1;
            current_indices.RemoveAt(current_elem);
            ElementSelect.Items.RemoveAt(ElementSelect.SelectedIndex);
            elems.RemoveAt(current_elem);

            ElementDisplay.Visible = false;

            diff_current_element = null;
            if (!synchronized_with_mapeditor)
            {
                diff.push_change(real_category_index, new SFDiffElement(SFDiffElement.DIFF_TYPE.REMOVE, current_elem, elem, null));

                undoCtrlZToolStripMenuItem.Enabled = diff.can_undo_changes(selected_category_index);
                redoCtrlYToolStripMenuItem.Enabled = diff.can_redo_changes(selected_category_index);
            }

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
            ElementSelect.BeginUpdate();
            SFCategory ctg = SFCategoryManager.gamedata[selected_category_index];

            int max_items = current_indices.Count;
            int last = Math.Min(max_items, loaded_count+elementselect_refresh_size);

            for (;loaded_count <last;loaded_count++)
                ElementSelect.Items.Add(ctg.GetElementString(current_indices[loaded_count]));

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
                
                if (max_items == ctg.GetElementCount())
                {
                    panelElemManipulate.Visible = true;
                }
                panelElemCopy.Visible = true;

                changeDataLanguageToolStripMenuItem.Enabled = true;
                System.Diagnostics.Debug.WriteLine("Elements: " + ElementSelect.Items.Count.ToString());
            }
            ElementSelect.EndUpdate();
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
            else if (!data_changed)
                result = DialogResult.No;
            else if (synchronized_with_mapeditor)
                result = DialogResult.Yes;
            else
                result = MessageBox.Show("Do you want to save gamedata before quitting? (Recommended when synchronized with Map Editor)", "Save before quit?", MessageBoxButtons.YesNoCancel);

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

            SFCategoryManager.UnloadAll();
            diff.clear_data();
            Tracer_Clear();

            data_loaded = false;
            data_changed = false;
            synchronized_with_mapeditor = false;

            this.Text = "SpellforceDataEditor";

            GC.Collect();

            return result;
        }

        //exit application
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        // called before closing the form
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
            if (synchronized_with_mapeditor)
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

                    ElementSelect.Items.Insert(elem_change.difference_index, SFCategoryManager.gamedata[selected_category_index].GetElementString(elem_change.difference_index));
                    for (int i = elem_change.difference_index; i < current_indices.Count; i++)
                        current_indices[i] = current_indices[i] + 1;
                    current_indices.Insert(elem_change.difference_index, elem_change.difference_index);

                    if (selected_category_index == real_category_index)
                    {
                        diff_current_element = SFCategoryManager.gamedata[selected_category_index][selected_element_index].GetCopy();
                    }

                }

                if(elem_change.difference_type == SFDiffElement.DIFF_TYPE.REPLACE)
                {
                    if ((selected_element_index == elem_change.difference_index) && (selected_category_index == real_category_index))
                    {
                        diff_current_element = null;
                    }

                    ElementSelect.Items[elem_change.difference_index] = SFCategoryManager.gamedata[selected_category_index].GetElementString(elem_change.difference_index);

                    if ((selected_category_index == real_category_index)&&(selected_element_index != -1))
                    {
                        diff_current_element = SFCategoryManager.gamedata[selected_category_index][selected_element_index].GetCopy();
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
            if (synchronized_with_mapeditor)
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
                    ElementSelect.Items.Insert(elem_change.difference_index + 1, SFCategoryManager.gamedata[selected_category_index].GetElementString(elem_change.difference_index+1));

                    if (selected_category_index == real_category_index)
                    {
                        diff_current_element = SFCategoryManager.gamedata[selected_category_index][selected_element_index].GetCopy();
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

                    ElementSelect.Items[elem_change.difference_index] = SFCategoryManager.gamedata[selected_category_index].GetElementString(elem_change.difference_index);

                    if (selected_category_index == real_category_index)
                    {
                        diff_current_element = SFCategoryManager.gamedata[selected_category_index][selected_element_index].GetCopy();
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

            insert_copy_element = SFCategoryManager.gamedata[selected_category_index][selected_element_index].GetCopy();
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
            ElementSelect_refresh(SFCategoryManager.gamedata[selected_category_index]);
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
            if (SFCategoryManager.gamedata[selected_category_index] != ctg)
                return;
            int index = Utility.find_binary_index(current_indices, elem_index);
            if (index != -1)
            {
                ElementSelect.SelectedIndexChanged -= new System.EventHandler(this.ElementSelect_SelectedIndexChanged);
                ElementSelect.Items[index] = ctg.GetElementString(elem_index);
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

            refs.set_referenced_element(real_category_index, selected_element_index);
        }

        private void refs_FormClosed(object sender, FormClosedEventArgs e)
        {
            refs.FormClosed -= new FormClosedEventHandler(this.refs_FormClosed);
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
            calc.FormClosed -= new FormClosedEventHandler(this.calc_FormClosed);
            calc = null;
        }

        public void poke_data()
        {
            data_changed = true;
        }

        public void mapeditor_desynchronize()
        {
            synchronized_with_mapeditor = false;
            SetUndoRedoEnabled(true);
        }

        private void SetUndoRedoEnabled(bool enabled)
        {
            undoCtrlZToolStripMenuItem.Enabled = enabled;
            redoCtrlYToolStripMenuItem.Enabled = enabled;
            if(enabled)
            {
                undoCtrlZToolStripMenuItem.Enabled = diff.can_undo_changes(selected_category_index);
                redoCtrlYToolStripMenuItem.Enabled = diff.can_redo_changes(selected_category_index);
            }
        }

        // assumes arguments are valid... is valid
        public void mapeditor_insert_npc(int new_npc_index, SFCategoryElement new_npc_data)
        {
            if (selected_category_index != 36)
            {
                SFCategoryManager.gamedata[36].elements.Insert(new_npc_index, new_npc_data);
                return;
            }

            resolve_category_index();

            SFCategory ctg = SFCategoryManager.gamedata[real_category_index];

            List<SFCategoryElement> elems = ctg.elements;
            elems.Insert(new_npc_index, new_npc_data);
            ElementSelect.Items.Insert(new_npc_index, ctg.GetElementString(new_npc_index));
            for (int i = new_npc_index; i < current_indices.Count; i++)
                current_indices[i] = current_indices[i] + 1;
            current_indices.Insert(new_npc_index, new_npc_index);

            // NO DIFF CHANGE HERE, TODO!
            Tracer_Clear();

            // select item
            ElementSelect.SelectedIndex = new_npc_index;
        }

        public void mapeditor_remove_npc(int npc_index)
        {
            if (selected_category_index != 36)
            {
                SFCategoryManager.gamedata[36].elements.RemoveAt(npc_index);
                return;
            }

            resolve_category_index();

            SFCategory ctg = SFCategoryManager.gamedata[real_category_index];

            List<SFCategoryElement> elems = ctg.elements;
            for (int i = npc_index; i < current_indices.Count; i++)
                current_indices[i] = current_indices[i] - 1;
            current_indices.RemoveAt(npc_index);
            ElementSelect.Items.RemoveAt(npc_index);
            elems.RemoveAt(npc_index);

            ElementDisplay.Visible = false;

            Tracer_Clear();
        }
    }
}
