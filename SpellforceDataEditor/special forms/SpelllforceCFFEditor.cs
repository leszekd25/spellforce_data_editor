﻿using System;
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

        private int selected_category_id = -1;
        private int real_category_id = -1;                   //tracer helper
        private int selected_element_index = -1;

        private SFCFF.category_forms.SFControl ElementDisplay;        //a control which displays all element parameters
        public Dictionary<int, SFCFF.category_forms.SFControl> CachedElementDisplays = new Dictionary<int, SFCFF.category_forms.SFControl>();   // element names and descriptions are read from here

        //these parameters control item loading behavior
        private int elementselect_refresh_size = 1000;
        private int elementselect_refresh_rate = 50;
        private int loaded_count = 0;

        protected List<int> current_indices = new List<int>();  //list of indices corrsponding to all displayed elements

        protected SFCategoryElement insert_copy_element = null; //if there was an element copied, it's stored here
        protected SFCategoryElementList insert_copy_element_list = null; //if there was an element copied, it's stored here - multiple allowed only

        private SFDataTracer tracer = new SFDataTracer();

        private special_forms.ReferencesForm refs = null;
        private special_forms.ChangeDataLangForm change_lang = null;
        private special_forms.CalculatorsForm calc = null;

        // element selection coloring stuff
        private SolidBrush bckg_unchanged = new SolidBrush(Color.White);
        private SolidBrush bckg_modified = new SolidBrush(Color.FromArgb(200, 200, 100));
        private SolidBrush bckg_added = new SolidBrush(Color.FromArgb(100, 200, 100));
        private SolidBrush bckg_removed = new SolidBrush(Color.FromArgb(200, 100, 100));
        private SolidBrush bckg_selected = new SolidBrush(Color.FromArgb(40, 40, 200));
        private SolidBrush text_default = new SolidBrush(Color.Black);
        private SolidBrush text_selected = new SolidBrush(Color.White);

        //constructor
        public SpelllforceCFFEditor()
        {
            InitializeComponent();

            if ((MainForm.mapedittool != null) && (MainForm.mapedittool.ready))    // gamedata is already loaded by this point
            {
                mapeditor_set_gamedata();
                MessageBox.Show("Gamedata editor is now synchronized with map editor! Any changes saved will permanently alter gamedata in your Spellforce directory.");
            }
        }

        //load game data
        private void loadGameDatacffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((MainForm.mapedittool != null)&&(MainForm.mapedittool.ready))
            {
                MessageBox.Show("Can not open gamedata while Map Editor is open.");
                return;
            }

            SFCFF.helper_forms.LoadGamedataForm LoadGD = new SFCFF.helper_forms.LoadGamedataForm();
            if (LoadGD.ShowDialog() != DialogResult.OK)
                return;

            switch(LoadGD.Mode)
            {
                case SFCFF.helper_forms.LoadGamedataForm.GDMode.FULL:
                    load_data(LoadGD.MainGDFileName);
                    break;
                case SFCFF.helper_forms.LoadGamedataForm.GDMode.DEPENDENCY:
                    load_data_dependency(LoadGD.MainGDFileName, LoadGD.DependencyGDFileNames);
                    break;
                case SFCFF.helper_forms.LoadGamedataForm.GDMode.DIFF:
                    load_data_diff(LoadGD.MainGDFileName, LoadGD.DiffGDFileName, LoadGD.DependencyGDFileNames);
                    break;
                case SFCFF.helper_forms.LoadGamedataForm.GDMode.MERGE:
                    load_data_merge(LoadGD.MergeGDFileNames);
                    break;
                default:
                    break;
            }
        }

        public bool load_data(string fname)
        {
            if (data_loaded)
                if (close_data() == DialogResult.Cancel)
                    return false;

            labelStatus.Text = "Loading...";
            statusStrip1.Refresh();

            SFCFF.SFGameData gamedata = new SFGameData();

            if (gamedata.Load(fname) < 0)
            {
                labelStatus.Text = "Failed to open file " + fname;
                return false;
            }

            SFCFF.SFCategoryManager.Set(gamedata);
            SFCFF.SFGameData.CalculateStatus(null, null, ref gamedata);

            this.Text = "GameData Editor - " + fname;
            labelStatus.Text = "Ready";

            CategorySelect.Enabled = true;
            foreach(var cat in SFCategoryManager.gamedata.categories)
            {
                CategorySelect.Items.Add(Tuple.Create(cat.Key, cat.Value.category_name));
                CachedElementDisplays.Add(cat.Key, get_element_display_from_category(cat.Key));
                CachedElementDisplays[cat.Key].set_category(SFCategoryManager.gamedata[cat.Key]);
            }

            data_loaded = true;
            data_changed = false;

            if (SFCategoryManager.gamedata.categories.Count > 0)
                CategorySelect.SelectedIndex = 0;
            else
                CategorySelect.SelectedIndex = -1;

            GC.Collect();

            return true;
        }

        public bool load_data_dependency(string main_fname, List<string> dependency)
        {
            if ((dependency == null)||(dependency.Count == 0))
                return load_data(main_fname);

            if (data_loaded)
                if (close_data() == DialogResult.Cancel)
                    return false;

            labelStatus.Text = string.Format("Loading dependencies (0/{0})...", dependency.Count);
            statusStrip1.Refresh();

            // merge dependencies
            SFCFF.SFGameData dep_gamedata = new SFGameData();
            if (dep_gamedata.Load(dependency[0]) < 0)
            {
                labelStatus.Text = "Failed to open file " + dependency[0];
                return false;
            }
            for (int i = 1; i < dependency.Count; i++)
            {
                labelStatus.Text = string.Format("Loading dependencies ({0}/{1})...", i, dependency.Count);
                statusStrip1.Refresh();
                SFCFF.SFGameData dep2_gamedata = new SFGameData();
                SFCFF.SFGameData dep_result_gamedata;
                if (dep2_gamedata.Load(dependency[i]) < 0)
                {
                    labelStatus.Text = "Failed to open file " + dependency[i];
                    return false;
                }
                SFGameData.Merge(dep_gamedata, dep2_gamedata, out dep_result_gamedata);
                dep_gamedata = dep_result_gamedata;
            }

            // load main gd
            labelStatus.Text = "Loading main gamedata...";
            statusStrip1.Refresh();
            SFCFF.SFGameData main_gamedata = new SFGameData();
            if (main_gamedata.Load(main_fname) < 0)
            {
                labelStatus.Text = "Failed to open file " + main_fname;
                return false;
            }

            // calculate status
            labelStatus.Text = "Calculating changes...";
            statusStrip1.Refresh();
            SFCFF.SFGameData result_gamedata = new SFGameData();
            SFCFF.SFGameData.Merge(dep_gamedata, main_gamedata, out result_gamedata);
            SFCFF.SFGameData.CalculateStatus(dep_gamedata, main_gamedata, ref result_gamedata);

            SFCFF.SFCategoryManager.Set(result_gamedata);
            SFCFF.SFCategoryManager.gd_dependencies = dependency;

            this.Text = "GameData Editor - " + main_fname;
            labelStatus.Text = "Ready";

            CategorySelect.Enabled = true;
            foreach (var cat in SFCategoryManager.gamedata.categories)
            {
                CategorySelect.Items.Add(Tuple.Create(cat.Key, cat.Value.category_name));
                CachedElementDisplays.Add(cat.Key, get_element_display_from_category(cat.Key));
                CachedElementDisplays[cat.Key].set_category(SFCategoryManager.gamedata[cat.Key]);
            }

            data_loaded = true;
            data_changed = false;

            if (SFCategoryManager.gamedata.categories.Count > 0)
                CategorySelect.SelectedIndex = 0;
            else
                CategorySelect.SelectedIndex = -1;

            GC.Collect();

            return true;
        }


        public bool load_data_diff(string main_fname, string diff_fname, List<string> dependency)
        {
            if (diff_fname == "")
                return load_data_dependency(main_fname, dependency);

            if (data_loaded)
                if (close_data() == DialogResult.Cancel)
                    return false;

            SFCFF.SFGameData dep_gamedata = null;

            // load and merge dependencies
            if ((dependency != null)&&(dependency.Count > 0))
            {
                labelStatus.Text = string.Format("Loading dependencies (0/{0})...", dependency.Count);
                statusStrip1.Refresh();

                dep_gamedata = new SFGameData();
                if (dep_gamedata.Load(dependency[0]) < 0)
                {
                    labelStatus.Text = "Failed to open file " + dependency[0];
                    return false;
                }
                for (int i = 1; i < dependency.Count; i++)
                {
                    labelStatus.Text = string.Format("Loading dependencies ({0}/{1})...", i, dependency.Count);
                    statusStrip1.Refresh();
                    SFCFF.SFGameData dep2_gamedata = new SFGameData();
                    SFCFF.SFGameData dep_result_gamedata;
                    if (dep2_gamedata.Load(dependency[i]) < 0)
                    {
                        labelStatus.Text = "Failed to open file " + dependency[i];
                        return false;
                    }
                    SFGameData.Merge(dep_gamedata, dep2_gamedata, out dep_result_gamedata);
                    dep_gamedata = dep_result_gamedata;
                }
            }

            // load main gd
            labelStatus.Text = "Loading main gamedata...";
            statusStrip1.Refresh();
            SFCFF.SFGameData main_gamedata = new SFGameData();
            if (main_gamedata.Load(main_fname) < 0)
            {
                labelStatus.Text = "Failed to open file " + main_fname;
                return false;
            }

            // merge main with dependencies
            if(dep_gamedata != null)
            {
                SFCFF.SFGameData tmp_gamedata = new SFGameData();
                SFCFF.SFGameData.Merge(dep_gamedata, main_gamedata, out tmp_gamedata);
                main_gamedata = tmp_gamedata;
            }

            // load diff gd
            labelStatus.Text = "Loading diff gamedata...";
            statusStrip1.Refresh();
            SFCFF.SFGameData diff_gamedata = new SFGameData();
            if (diff_gamedata.Load(diff_fname) < 0)
            {
                labelStatus.Text = "Failed to open file " + diff_fname;
                return false;
            }

            // calculate status
            labelStatus.Text = "Calculating changes...";
            statusStrip1.Refresh();
            SFCFF.SFGameData result_gamedata = new SFGameData();
            SFCFF.SFGameData.Merge(main_gamedata, diff_gamedata, out result_gamedata);
            SFCFF.SFGameData.CalculateStatus(main_gamedata, diff_gamedata, ref result_gamedata);

            SFCFF.SFCategoryManager.Set(result_gamedata);
            SFCFF.SFCategoryManager.gd_dependencies = dependency;

            this.Text = "GameData Editor - " + main_fname;
            labelStatus.Text = "Ready";

            CategorySelect.Enabled = true;
            foreach (var cat in SFCategoryManager.gamedata.categories)
            {
                CategorySelect.Items.Add(Tuple.Create(cat.Key, cat.Value.category_name));
                CachedElementDisplays.Add(cat.Key, get_element_display_from_category(cat.Key));
                CachedElementDisplays[cat.Key].set_category(SFCategoryManager.gamedata[cat.Key]);
            }

            data_loaded = true;
            data_changed = false;

            if (SFCategoryManager.gamedata.categories.Count > 0)
                CategorySelect.SelectedIndex = 0;
            else
                CategorySelect.SelectedIndex = -1;

            GC.Collect();

            return true;
        }

        public bool load_data_merge(List<string> merge_list)
        {
            if (merge_list.Count == 0)
                return false;

            if (data_loaded)
                if (close_data() == DialogResult.Cancel)
                    return false;

            labelStatus.Text = string.Format("Loading (0/{0})...", merge_list.Count);
            statusStrip1.Refresh();

            // merge dependencies
            SFCFF.SFGameData merge_gamedata = new SFGameData();
            if (merge_gamedata.Load(merge_list[0]) < 0)
            {
                labelStatus.Text = "Failed to open file " + merge_list[0];
                return false;
            }
            for (int i = 1; i < merge_list.Count; i++)
            {
                labelStatus.Text = string.Format("Loading ({0}/{1})...", i, merge_list.Count);
                statusStrip1.Refresh();
                SFCFF.SFGameData merge2_gamedata = new SFGameData();
                SFCFF.SFGameData merge_result_gamedata;
                if (merge2_gamedata.Load(merge_list[i]) < 0)
                {
                    labelStatus.Text = "Failed to open file " + merge_list[i];
                    return false;
                }
                SFGameData.Merge(merge_gamedata, merge2_gamedata, out merge_result_gamedata);
                merge_gamedata = merge_result_gamedata;
            }

            SFCFF.SFCategoryManager.Set(merge_gamedata);
            SFCFF.SFGameData.CalculateStatus(null, null, ref merge_gamedata);

            this.Text = "GameData Editor - multiple files";
            labelStatus.Text = "Ready";

            CategorySelect.Enabled = true;
            foreach (var cat in SFCategoryManager.gamedata.categories)
            {
                CategorySelect.Items.Add(Tuple.Create(cat.Key, cat.Value.category_name));
                CachedElementDisplays.Add(cat.Key, get_element_display_from_category(cat.Key));
                CachedElementDisplays[cat.Key].set_category(SFCategoryManager.gamedata[cat.Key]);
            }

            data_loaded = true;
            data_changed = false;

            if (SFCategoryManager.gamedata.categories.Count > 0)
                CategorySelect.SelectedIndex = 0;
            else
                CategorySelect.SelectedIndex = -1;

            GC.Collect();

            return true;
        }

        // gamedata is already loaded, just connect with the gamedata editor
        public void mapeditor_set_gamedata()
        {
            SFCategoryManager.manual_SetGamedata();

            this.Text = "GameData Editor - synchronized with MapEditor";
            labelStatus.Text = "Ready";
            
            CategorySelect.Enabled = true;
            foreach (var cat in SFCategoryManager.gamedata.categories)
            {
                CategorySelect.Items.Add(Tuple.Create(cat.Key, cat.Value.category_name));
                CachedElementDisplays.Add(cat.Key, get_element_display_from_category(cat.Key));
                CachedElementDisplays[cat.Key].set_category(SFCategoryManager.gamedata[cat.Key]);
            }

            data_loaded = true;
            data_changed = false;

            if (SFCategoryManager.gamedata.categories.Count > 0)
                CategorySelect.SelectedIndex = 0;
            else
                CategorySelect.SelectedIndex = -1;
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

            if ((MainForm.mapedittool != null) && (MainForm.mapedittool.ready))    // dont ask when synchronized
            {
                return save_data_full(SFUnPak.SFUnPak.game_directory_name + "\\data\\GameData.cff");
            }
            else
            {
                SFCFF.helper_forms.SaveGamedataForm sgd = new SFCFF.helper_forms.SaveGamedataForm();
                if (sgd.ShowDialog() != DialogResult.OK)
                    return false;

                switch (sgd.Mode)
                {
                    case SFCFF.helper_forms.SaveGamedataForm.GDMode.FULL:
                        return save_data_full(sgd.MainGDFileName);
                    case SFCFF.helper_forms.SaveGamedataForm.GDMode.DEPENDENCY:
                        return save_data_dependency(sgd.MainGDFileName);
                    default:
                        break;
                }
            }

            return false;
        }

        public bool save_data_full(string fname)
        {
            labelStatus.Text = "Saving...";

            SFCategoryManager.Save(fname);

            labelStatus.Text = "Saved";

            data_changed = false;

            return true;
        }

        public bool save_data_dependency(string fname)
        {
            labelStatus.Text = "Saving...";

            SFCategoryManager.SaveDiff(fname);

            labelStatus.Text = "Saved";

            data_changed = false;

            return true;
        }

        private SFCFF.category_forms.SFControl get_element_display_from_category(int cat)
        {
            switch(cat)
            {
                case 2002:
                    return new SFCFF.category_forms.Control1();
                case 2054:
                    return new SFCFF.category_forms.Control2();
                case 2056:
                    return new SFCFF.category_forms.Control3();
                case 2005:
                    return new SFCFF.category_forms.Control4();
                case 2006:
                    return new SFCFF.category_forms.Control5();
                case 2067:
                    return new SFCFF.category_forms.Control6();
                case 2003:
                    return new SFCFF.category_forms.Control7();
                case 2004:
                    return new SFCFF.category_forms.Control8();
                case 2013:
                    return new SFCFF.category_forms.Control9();
                case 2015:
                    return new SFCFF.category_forms.Control10();
                case 2017:
                    return new SFCFF.category_forms.Control11();
                case 2014:
                    return new SFCFF.category_forms.Control12();
                case 2012:
                    return new SFCFF.category_forms.Control13();
                case 2018:
                    return new SFCFF.category_forms.Control14();
                case 2016:
                    return new SFCFF.category_forms.Control15();
                case 2022:
                    return new SFCFF.category_forms.Control16();
                case 2023:
                    return new SFCFF.category_forms.Control17();
                case 2024:
                    return new SFCFF.category_forms.Control18();
                case 2025:
                    return new SFCFF.category_forms.Control19();
                case 2026:
                    return new SFCFF.category_forms.Control20();
                case 2028:
                    return new SFCFF.category_forms.Control21();
                case 2040:
                    return new SFCFF.category_forms.Control22();
                case 2001:
                    return new SFCFF.category_forms.Control23();
                case 2029:
                    return new SFCFF.category_forms.Control24();
                case 2030:
                    return new SFCFF.category_forms.Control25();
                case 2031:
                    return new SFCFF.category_forms.Control26();
                case 2039:
                    return new SFCFF.category_forms.Control27();
                case 2062:
                    return new SFCFF.category_forms.Control28();
                case 2041:
                    return new SFCFF.category_forms.Control29();
                case 2042:
                    return new SFCFF.category_forms.Control30();
                case 2047:
                    return new SFCFF.category_forms.Control31();
                case 2044:
                    return new SFCFF.category_forms.Control32();
                case 2048:
                    return new SFCFF.category_forms.Control33();
                case 2050:
                    return new SFCFF.category_forms.Control34();
                case 2057:
                    return new SFCFF.category_forms.Control35();
                case 2065:
                    return new SFCFF.category_forms.Control36();
                case 2051:
                    return new SFCFF.category_forms.Control37();
                case 2052:
                    return new SFCFF.category_forms.Control38();
                case 2053:
                    return new SFCFF.category_forms.Control39();
                case 2055:
                    return new SFCFF.category_forms.Control40();
                case 2058:
                    return new SFCFF.category_forms.Control41();
                case 2059:
                    return new SFCFF.category_forms.Control42();
                case 2061:
                    return new SFCFF.category_forms.Control43();
                case 2063:
                    return new SFCFF.category_forms.Control44();
                case 2064:
                    return new SFCFF.category_forms.Control45();
                case 2032:
                    return new SFCFF.category_forms.Control46();
                case 2049:
                    return new SFCFF.category_forms.Control47();
                case 2036:
                    return new SFCFF.category_forms.Control48();
                case 2072:
                    return new SFCFF.category_forms.Control49();
                default:
                    return null;
            }
        }

        //spawns a new control to display element data
        private void set_element_display(int cat)
        {
            if ((ElementDisplay != null)&&(SearchPanel.Controls.Contains(ElementDisplay)))
                SearchPanel.Controls.Remove(ElementDisplay);

            ElementDisplay = CachedElementDisplays[cat];
            ElementDisplay.BringToFront();

            labelDescription.SendToBack();

            SearchPanel.Controls.Add(ElementDisplay);
        }

        //restores element display to show elements from currently selected category
        private void resolve_category_index()
        {
            if (real_category_id != selected_category_id)
            {
                real_category_id = selected_category_id;
                set_element_display(real_category_id);
            }
        }

        //what happens when you choose category from a list
        private void CategorySelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CategorySelect.SelectedIndex == -1)
                return;

            // force textboxes to validate, submitting data
            Focus();

            // set visibility
            panelElemManipulate.Visible = false;
            panelElemCopy.Visible = false;
            ElementSelect.Enabled = true;

            // set current category
            SFCategory ctg = SFCategoryManager.gamedata[((Tuple<int, string>)CategorySelect.SelectedItem).Item1];
            if (selected_category_id != ((Tuple<int, string>)CategorySelect.SelectedItem).Item1)
            {
                ButtonElemAdd.BackColor = SystemColors.Control;
                ButtonElemInsert.BackColor = SystemColors.Control;
                insert_copy_element = null;
            }
            selected_category_id = ((Tuple<int, string>)CategorySelect.SelectedItem).Item1;
            real_category_id = selected_category_id;

            // set display form for elements of this category
            set_element_display(real_category_id);
            ElementDisplay.Visible = false;

            // clear all elements and start loading new elements
            ElementSelect_refresh(ctg);

            // search panel setup
            SearchColumnID.Items.Clear();
            SearchColumnID.SelectedIndex = -1;
            SearchColumnID.Text = "";
            Dictionary<string, int[]>.KeyCollection keys = ElementDisplay.get_column_descriptions();
            foreach (string s in keys)
                SearchColumnID.Items.Add(s);
            panelSearch.Visible = true;
            ClearSearchButton.Enabled = false;
            ContinueSearchButton.Enabled = false;

            ButtonElemAdd.BackColor = SystemColors.Control;
            ButtonElemInsert.BackColor = SystemColors.Control;

            Tracer_Clear();
        }

        //what happens when you choose element from a list
        private void ElementSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            SFCategory ctg = SFCategoryManager.gamedata[selected_category_id];

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

            labelDescription.Text = ElementDisplay.get_description_string(current_indices[ElementSelect.SelectedIndex]);

            Tracer_Clear();

            if (MainForm.viewer != null)
                MainForm.viewer.GenerateScene(real_category_id, selected_element_index);

            if(refs!=null)
                refs.set_referenced_element(real_category_id, selected_element_index);
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

        private void ElementSelect_DrawItem(object sender, DrawItemEventArgs e)
        {
            bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);

            int index = e.Index;
            if (index >= 0 && index < ElementSelect.Items.Count)
            {
                SFCategory ctg = SFCategoryManager.gamedata[selected_category_id];
                string text = ElementSelect.Items[index].ToString();
                Graphics g = e.Graphics;

                //background:
                SolidBrush backgroundBrush;
                if (selected)
                    backgroundBrush = bckg_selected;
                else
                {
                    switch (ctg.element_status[index])
                    {
                        case SFCategoryElementStatus.ADDED:
                            backgroundBrush = bckg_added;
                            break;
                        case SFCategoryElementStatus.MODIFIED:
                            backgroundBrush = bckg_modified;
                            break;
                        case SFCategoryElementStatus.REMOVED:
                            backgroundBrush = bckg_removed;
                            break;
                        case SFCategoryElementStatus.UNCHANGED:
                        default:
                            backgroundBrush = bckg_unchanged;
                            break;
                    }
                }

                g.FillRectangle(backgroundBrush, e.Bounds);

                //text:
                SolidBrush foregroundBrush = (selected) ? text_selected : text_default;
                g.DrawString(text, e.Font, foregroundBrush, ElementSelect.GetItemRectangle(index).Location);
            }
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

            BringToFront();
            CategorySelect.Focus();

            //for now it's like this
            //todo: allow tracing no matter the state
            if(log_trace)
                tracer.AddTrace(real_category_id, selected_element_index);

            real_category_id = cat_i;
            selected_element_index = cat_e;

            set_element_display(cat_i);
            if (SFCategoryManager.gamedata[cat_i][cat_e] != null)
            {
                ElementDisplay.Visible = true;
                ElementDisplay.set_element(cat_e);
                ElementDisplay.show_element();
                if (MainForm.viewer != null)
                    MainForm.viewer.GenerateScene(cat_i, cat_e);
            }

            labelDescription.Text = CachedElementDisplays[cat_i].get_description_string(cat_e);

            label_tracedesc.Text = "Category " + cat_i.ToString() + " | " + CachedElementDisplays[cat_i].get_element_string(cat_e);
            buttonTracerBack.Visible = true;
        }

        //when you press Back, you step out and return to previously viewed element
        public void Tracer_StepBack()
        {
            CategorySelect.Focus();

            buttonTracerBack.Visible = false;

            if (!tracer.CanGoBack())
                return;

            SFDataTraceElement trace = tracer.GoBack();
            int cat_i = trace.category_index;
            int cat_e = trace.category_element;

            real_category_id = cat_i;
            selected_element_index = cat_e;
            if (MainForm.viewer != null)
                MainForm.viewer.GenerateScene(cat_i, cat_e);

            set_element_display(cat_i);
            ElementDisplay.Visible = true;
            ElementDisplay.set_element(cat_e);
            ElementDisplay.show_element();

            labelDescription.Text = CachedElementDisplays[cat_i].get_description_string(cat_e);

            label_tracedesc.Text = "Category " + cat_i.ToString() + " | " + CachedElementDisplays[cat_i].get_element_string(cat_e);
            if (tracer.CanGoBack())
                buttonTracerBack.Visible = true;
            else
                label_tracedesc.Text = "";
        }

        //searches elements within entire category
        private void SearchButton_Click(object sender, EventArgs e)
        {
            SFCategory cat = SFCategoryManager.gamedata[selected_category_id];
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
            SFCategory cat = SFCategoryManager.gamedata[selected_category_id];
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


        //what happens when you insert an element to a category
        //can copy stored elements
        private void ButtonElemAdd_Click(object sender, EventArgs e)
        {
            resolve_category_index();

            SFCategory ctg = SFCategoryManager.gamedata[real_category_id];
            List<SFCategoryElement> elems = ctg.elements;

            int current_elem = elems.Count-1;
            int last_id = ctg.GetElementID(current_elem);

            SFCategoryElement elem;
            if (insert_copy_element == null)
                elem = ctg.GetEmptyElement();
            else
                elem = insert_copy_element.GetCopy();

            if (ctg.GetElementFormat()[0] == 'B')
                elem[0] = (byte)(last_id + 1);
            else if (ctg.GetElementFormat()[0] == 'H')
                elem[0] = (ushort)(last_id + 1);
            else if (ctg.GetElementFormat()[0] == 'I')
                elem[0] = (uint)(last_id + 1);
            else
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "ButtonElemAdd_Click(): Unknown category format");
                throw new Exception("Unknown category format");
            }


            elems.Insert(current_elem + 1, elem);
            ElementSelect.Items.Insert(current_elem + 1, CachedElementDisplays[real_category_id].get_element_string(current_elem + 1));
            for (int i = current_elem + 1; i < current_indices.Count; i++)
                current_indices[i] = current_indices[i] + 1;
            current_indices.Insert(current_elem + 1, current_elem + 1);

            Tracer_Clear();

            ElementSelect.SelectedIndex = current_elem + 1;
        }

        //what happens when you insert an element to a category
        //can copy stored elements
        private void ButtonElemInsert_Click(object sender, EventArgs e)
        {
            if (ElementSelect.SelectedIndex == -1)
                return;

            resolve_category_index();

            int current_elem = current_indices[ElementSelect.SelectedIndex];

            SFCategory ctg = SFCategoryManager.gamedata[real_category_id];
            SFCategoryElement elem;
            if (insert_copy_element == null)
                elem = ctg.GetEmptyElement();
            else
                elem = insert_copy_element.GetCopy();

            List<SFCategoryElement> elems = ctg.elements;
            elems.Insert(current_elem+1, elem);
            ElementSelect.Items.Insert(current_elem+1, CachedElementDisplays[real_category_id].get_element_string(current_elem + 1));
            for (int i = current_elem+1; i < current_indices.Count; i++)
                current_indices[i] = current_indices[i] + 1;
            current_indices.Insert(current_elem+1, current_elem+1);

            Tracer_Clear();

            ElementSelect.SelectedIndex = current_elem + 1;
        }

        //what happens when you remove element from category
        private void ButtonElemRemove_Click(object sender, EventArgs e)
        {
            if (ElementSelect.SelectedIndex == -1)
                return;

            resolve_category_index();

            int current_elem = current_indices[ElementSelect.SelectedIndex];

            SFCategory ctg = SFCategoryManager.gamedata[real_category_id];
            SFCategoryElement elem = ctg[current_elem].GetCopy();

            List<SFCategoryElement> elems = ctg.elements;
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
            ElementSelect.BeginUpdate();

            SFCategory ctg = SFCategoryManager.gamedata[selected_category_id];

            int max_items = current_indices.Count;
            int last = Math.Min(max_items, loaded_count+elementselect_refresh_size);

            for (; loaded_count < last; loaded_count++)
                ElementSelect.Items.Add(ElementDisplay.get_element_string(current_indices[loaded_count]));//(ctg.GetElementString(current_indices[loaded_count]));

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
            
        }

        //close gamedata.cff
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((MainForm.mapedittool != null) && (MainForm.mapedittool.ready))
            {
                MessageBox.Show("Can not close gamedata while Map Editor is open.");
                return;
            }
            if (data_loaded)
                close_data();
        }

        //actually clear all data and close gamedata.cff
        public DialogResult close_data()
        {
            //ask first to close currend gamedata.cff, if user clicks Cancel, function return immediately
            DialogResult result;
            if (!data_loaded)
                return DialogResult.No;
            else if (!data_changed)
                result = DialogResult.No;
            else if ((MainForm.mapedittool != null) && (MainForm.mapedittool.ready))
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

            foreach(var elemd in CachedElementDisplays)
            {
                elemd.Value.set_category(null);
                elemd.Value.Dispose();
            }
            CachedElementDisplays.Clear();

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
            ButtonElemAdd.BackColor = SystemColors.Control;
            ButtonElemInsert.BackColor = SystemColors.Control;
            insert_copy_element = null;

            if (ElementDisplay != null)
                ElementDisplay.Visible = false;
            labelDescription.Text = "";

            CategorySelect.Items.Clear();
            CategorySelect.Enabled = false;

            panelSearch.Visible = false;
            ContinueSearchButton.Enabled = false;

            selected_category_id = -1;
            real_category_id = -1;
            selected_element_index = -1;

            labelStatus.Text = "";
            ProgressBar_Main.Visible = false;
            ProgressBar_Main.Value = 0;
            statusStrip1.Refresh();

            undoCtrlZToolStripMenuItem.Enabled = false;
            redoCtrlYToolStripMenuItem.Enabled = false;

            SFCategoryManager.UnloadAll();
            Tracer_Clear();

            data_loaded = false;
            data_changed = false;

            this.Text = "GameData Editor";

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
                if ((MainForm.mapedittool != null) && (MainForm.mapedittool.ready))
                    return;

                DialogResult result = close_data();
                if (result == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        private void buttonTracerBack_Click(object sender, EventArgs e)
        {
            Tracer_StepBack();
        }

        //stores copied element to be pasted elsewhere
        private void ButtonElemCopy_Click(object sender, EventArgs e)
        {
            if (ElementSelect.SelectedIndex == -1)
                return;

            insert_copy_element = SFCategoryManager.gamedata[selected_category_id][selected_element_index].GetCopy();
            ButtonElemAdd.BackColor = Color.Yellow;
            ButtonElemInsert.BackColor = Color.Yellow;
        }

        //clears stored copy
        private void ButtonElemClear_Click(object sender, EventArgs e)
        {
            if (insert_copy_element == null)
                return;

            insert_copy_element = null;
            ButtonElemAdd.BackColor = SystemColors.Control;
            ButtonElemInsert.BackColor = SystemColors.Control;
        }

        //if there were elements searched, restores list to display whole category data
        private void ClearSearchButton_Click(object sender, EventArgs e)
        {
            ContinueSearchButton.Enabled = false;
            ClearSearchButton.Enabled = false;
            ElementSelect_refresh(SFCategoryManager.gamedata[selected_category_id]);
        }

        //special option to change game language
        private void changeDataLanguageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            change_lang = new special_forms.ChangeDataLangForm();

            change_lang.ShowDialog();

            change_lang = null;

            labelStatus.Text = "Done";
        }

        //called from the outside, updates element name on the list
        //very ugly, due to lack of knowledge of windows' event handling...
        public void external_set_element_select_string(SFCategory ctg, int elem_index)
        {
            if (selected_category_id != real_category_id)
                return;
            if (SFCategoryManager.gamedata[selected_category_id] != ctg)
                return;
            int index = Utility.find_binary_index(current_indices, elem_index);
            if (index != -1)
            {
                ElementSelect.SelectedIndexChanged -= new System.EventHandler(this.ElementSelect_SelectedIndexChanged);
                ElementSelect.Items[index] = CachedElementDisplays[ctg.category_id].get_element_string(elem_index);
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
            if((real_category_id == -1)||(selected_element_index == -1))
                return;

            if (refs == null)
            {
                refs = new special_forms.ReferencesForm();
                refs.FormClosed += new FormClosedEventHandler(this.refs_FormClosed);
                refs.Show();
            }
            else
                refs.BringToFront();

            refs.set_referenced_element(real_category_id, selected_element_index);
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
    }
}
