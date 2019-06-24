using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using SpellforceDataEditor.SFLua;
using SpellforceDataEditor.SFLua.lua_controls;
using SpellforceDataEditor.SFLua.lua_filesystem;
using SpellforceDataEditor.SFLua.lua_dialog;
using SpellforceDataEditor.special_forms.utility_forms;
using Gma.System.MouseKeyHook;

namespace SpellforceDataEditor.special_forms
{
    public partial class ScriptBuilderForm : Form
    {
        IMouseEvents mouse_hook;
        
        LuaValueControl selected_lua_control = null;
        ScriptProject project = null;
        public special_forms.SpelllforceCFFEditor Editor { get; private set; } = null;

        public ScriptBuilderForm()
        {
            InitializeComponent();
            mouse_hook = Hook.GlobalEvents();
            mouse_hook.MouseClick += OnGlobalMouseClick;
            FillListBoxWithMethods(typeof(LuaEvent));
            FillScriptTypeMenu();
            RefreshScriptInfo();


            if (LuaEnumUtility.lua_enums.Count == 0)
                LuaEnumUtility.LoadEnums();

            SetStatus("Ready");
        }

        public void Link(special_forms.SpelllforceCFFEditor spf)
        {
            Editor = spf;
        }

        public void SetStatus(string text)
        {
            StatusText.Text = text;
        }

        private void FillListBoxWithMethods(Type func)
        {
            ListCodeEntries.Items.Clear();

            if (func == null)
                return;

            List<string> list_items = new List<string>();
            foreach (MethodInfo s in func.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
                list_items.Add(s.Name);
            list_items.Sort();

            foreach (string s in list_items)
                ListCodeEntries.Items.Add(s);
        }

        private void FillScriptTypeMenu()
        {
            changeActiveScriptTypeToolStripMenuItem.DropDownItems.Clear();
            foreach (string s in typeof(ScriptType).GetEnumNames())
                changeActiveScriptTypeToolStripMenuItem.DropDownItems.Add(s);
        }

        private void TabDragCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TabDragCode.SelectedIndex == -1)
                return;

            string name = TabDragCode.SelectedTab.Name;
            name = "SpellforceDataEditor.SFLua.Lua" + name.Substring(3, name.Length - 4);
            //System.Diagnostics.Debug.WriteLine(name);
            FillListBoxWithMethods(Type.GetType(name));
        }

        ///https://stackoverflow.com/questions/2411062/how-to-get-control-under-mouse-cursor
        public static Control FindControlAtPoint(Control container, Point pos)
        {
            Control child;
            foreach (Control c in container.Controls)
            {
                if (c.Visible && c.Bounds.Contains(pos))
                {
                    child = FindControlAtPoint(c, new Point(pos.X - c.Left, pos.Y - c.Top));
                    if (child == null) return c;
                    else return child;
                }
            }
            return null;
        }

        public static Control FindControlAtCursor(Form form)
        {
            Point pos = Cursor.Position;
            if (form.Bounds.Contains(pos))
                return FindControlAtPoint(form, form.PointToClient(pos));
            return null;
        }
        ///

        public void SelectLuaControl(LuaValueControl c)
        {
            if (selected_lua_control != null)
            {
                selected_lua_control.BackColor = SystemColors.InactiveCaption;
                if (selected_lua_control.Parent is LuaDialogControl)
                    selected_lua_control.BackColor = selected_lua_control.Parent.BackColor;
            }
            selected_lua_control = c;
            if(c != null)
                c.BackColor = SystemColors.ActiveCaption;
        }
        
        private void OnGlobalMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            Control c = FindControlAtCursor(this);
            bool IsLuaValueControl = false;

            if (c != null)
            {
                if (c is LuaValueControl)
                    IsLuaValueControl = true;
                else if ((c.Parent != null) && (c.Parent is LuaValueControl))
                {
                    c = c.Parent;
                    IsLuaValueControl = true;
                }
            }

            if (IsLuaValueControl)
                SelectLuaControl((LuaValueControl)c);
        }

        //https://stackoverflow.com/questions/3183352/close-button-in-tabcontrol/36070187
        private void TabScripts_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= TabScripts.TabCount)
                return;
            Rectangle r = TabScripts.GetTabRect(e.Index);
            e.Graphics.DrawRectangle(new Pen(Brushes.IndianRed, 3), r.Right - 16, r.Bottom - 14, 12, 12);
            e.Graphics.DrawString(this.TabScripts.TabPages[e.Index].Text, e.Font, Brushes.Black, r.Left + 12, r.Top + 4);
            e.DrawFocusRectangle();
        }

        private void TabScripts_MouseDown(object sender, MouseEventArgs e)
        {
            //Looping through the controls.
            for (int i = 0; i < this.TabScripts.TabPages.Count; i++)
            {
                Rectangle r = TabScripts.GetTabRect(i);
                //Getting the position of the "x" mark.
                Rectangle closeButton = new Rectangle(r.Right - 15, r.Top + 4, 9, 7);
                if (closeButton.Contains(e.Location))
                {
                    if (MessageBox.Show("Would you like to Close this Tab?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (TabScripts.TabPages[i].Tag is ScriptGenerator)
                            ((ScriptGenerator)TabScripts.TabPages[i].Tag).BlocksToNodes();
                        else if (TabScripts.TabPages[i].Tag is LuaDialogManager)
                        {
                            ((LuaDialogManager)TabScripts.TabPages[i].Tag).BlocksToNodes();
                            ((LuaDialogManager)TabScripts.TabPages[i].Tag).PhaseControls = null;
                        }
                        SetStatus("Closed tab " + TabScripts.TabPages[i].Text);
                        TabScripts.TabPages.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        //

        public Point GetPanelItemsSize(Panel p)
        {
            Point result = new Point(0, 0);
            foreach (Control c in p.Controls)
                result = new Point(Math.Max(result.X, c.Width), result.Y + 10 + c.Height);
            return result;
        }

        public Point GetNextPhasePosition(Panel panel)
        {
            Point p = new Point(10, 10);
            if (panel.Controls.Count != 0)
                p = panel.Controls[0].Location;
            foreach (Control c in panel.Controls)
                p.Y += c.Height + 10;
            return p;
        }

        public void FixPhasePositions(Panel panel)
        {
            Point p = new Point(10, 10);
            for(int i = 0; i < panel.Controls.Count; i++)
            {
                LuaDialogPhaseControl phase = (LuaDialogPhaseControl)panel.Controls[i];
                phase.Location = new Point(p.X + panel.AutoScrollPosition.X, p.Y + panel.AutoScrollPosition.Y);
                p = new Point(p.X, p.Y + phase.Height + 10);
            }
        }

        //todo: split this function
        private void ListCodeEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListCodeEntries.SelectedIndex == -1)
                return;

            string name = TabDragCode.SelectedTab.Name;
            if (name == "TabDialogs")
            {
                if (TabScripts.TabPages.Count == 0)
                    return;
                if (!(TabScripts.SelectedTab.Tag is LuaDialogManager))
                    return;

                string option_name = ListCodeEntries.SelectedItem.ToString();

                Panel panel = (Panel)TabScripts.SelectedTab.Controls[0];
                LuaDialogPhaseControl phase = new LuaDialogPhaseControl();
                Point n_pos = GetNextPhasePosition(panel);
                panel.Controls.Add(phase);
                phase.Location = n_pos;
                phase.GetPhaseTextBox().Text = (0).ToString();
                phase.Rename(option_name);
                phase.GetPhaseTextBox().Enabled = (option_name == "OnAnswer");
            }
            else
            {
                if (selected_lua_control == null)
                    return;

                if (selected_lua_control is LuaValueComplexControl)
                {

                    name = "SpellforceDataEditor.SFLua.Lua" + name.Substring(3, name.Length - 4);

                    Type generator_type = Type.GetType(name);
                    MethodInfo generator = generator_type.GetMethod(ListCodeEntries.SelectedItem.ToString());
                    LuaValueControl new_control = (LuaValueControl)generator.Invoke(null, null);

                    new_control.AllowRemove = true;
                    ((LuaValueComplexControl)selected_lua_control).AddLuaControl(new_control);
                    ((LuaValueComplexControl)selected_lua_control).ResetSize();
                }
            }
        }

        private void ScriptBuilderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = ProjectClose();
            if (result == DialogResult.Cancel)
                e.Cancel = true;
            else
                mouse_hook.MouseClick -= OnGlobalMouseClick;
        }

        //closes current project and creates a new one
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //prompt to close current project
            if (ProjectClose() == DialogResult.Cancel)
                return;

            //get name for new project
            string proj_name = Utility.GetString("Choose project name", "Enter project name below");
            if (proj_name == null)
                return;

            //create a new project
            project = new ScriptProject();
            project.Name = proj_name;
            LabelProject.Text = "Project: " + proj_name;

            SetStatus("Created project " + proj_name);
        }

        private void addPlatformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (project == null)
            {
                SetStatus("Can't create platform without a project");
                return;
            }

            string plat_name = Utility.GetString("Choose platform name", "Enter platform name below");
            if (plat_name == null)
                return;

            project.PlatformCreate(plat_name);
            RefreshPlatformListBox();

            SetStatus("Created platform " + plat_name);
        }

        private void RefreshPlatformListBox()
        {
            string cur_plat = null;
            if (ListPlatforms.SelectedIndex != -1)
                cur_plat = ListPlatforms.SelectedItem.ToString();

            ListPlatforms.Items.Clear();
            if (project == null)
                return;

            List<string> platforms = project.PlatformGetNames();
            platforms.Sort();
            for (int i = 0; i < platforms.Count; i++)
                ListPlatforms.Items.Add(platforms[i]);

            if (cur_plat != null)
                ListPlatforms.SelectedIndex = ListPlatforms.Items.IndexOf(cur_plat);
        }

        private void addScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (project == null)
            {
                SetStatus("Can't create a script without a project");
                return;
            }

            if (ListPlatforms.SelectedIndex == -1)
            {
                SetStatus("Select a platform first");
                return;
            }
            ScriptPlatform cur_platform = project.PlatformGet(ListPlatforms.SelectedItem.ToString());

            string scr_name = Utility.GetString("Choose script name", "Enter script name below");
            if (scr_name == null)
                return;

            cur_platform.ScriptCreate(scr_name);
            RefreshScriptListBox();
        }

        private void RefreshScriptListBox()
        {
            ListScripts.Items.Clear();
            if (project == null)
                return;

            if (ListPlatforms.SelectedIndex == -1)
                return;
            ScriptPlatform cur_platform = project.PlatformGet(ListPlatforms.SelectedItem.ToString());

            List<string> scripts = cur_platform.ScriptGetNames();
            scripts.Sort();
            for (int i = 0; i < scripts.Count; i++)
                ListScripts.Items.Add(scripts[i]);
        }

        private void ListPlatforms_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshScriptListBox();
        }

        private void OpenScript(string name)
        {
            if (project == null)
            {
                SetStatus("Can't open a script without a project");
                return;
            }

            if (ListPlatforms.SelectedIndex == -1)
            {
                SetStatus("Can't open a script without a platform");
                return;
            }
            ScriptPlatform cur_platform = project.PlatformGet(ListPlatforms.SelectedItem.ToString());

            ScriptGenerator script = cur_platform.ScriptGet(name);
            if (script == null)
            {
                SetStatus("Can't find script " + name + " on a selected platform");
                return;
            }

            //create new tab layout
            Panel pan = new Panel();
            pan.SuspendLayout();
            pan.Size = new Size(497, 370);
            pan.BackColor = SystemColors.Window;
            pan.AutoScroll = true;

            LuaValueComplexControl root = new LuaValueComplexControl(LuaParseFlag.BlockNewLines | LuaParseFlag.ParamNewLines | LuaParseFlag.SeparatingCommas);

            pan.Controls.Add(root);

            TabScripts.TabPages.Insert(0, script.Name);
            TabScripts.TabPages[0].Name = name;
            TabScripts.TabPages[0].Controls.Add(pan);
            TabScripts.TabPages[0].Tag = script;

            TabScripts.SelectedIndex = 0;
            RefreshScriptInfo();
            script.NodesToBlocks(root);
            SelectLuaControl(root);
            pan.ResumeLayout();

            SetStatus("Opened script " + name);
        }

        private void ListScripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListScripts.SelectedIndex == -1)
                return;
            string s_name = ListScripts.SelectedItem.ToString();

            //look for open scripts
            for(int i = 0; i < TabScripts.TabPages.Count; i++)
                if(TabScripts.TabPages[i].Name == s_name)
                {
                    TabScripts.SelectedIndex = i;
                    RefreshScriptInfo();
                    return;
                }

            //open script
            OpenScript(s_name);
        }

        private DialogResult ProjectSave()
        {
            if (project == null)
                return DialogResult.No;

            DialogResult result = SaveProjectDialog.ShowDialog();
            if (result != DialogResult.OK)
                return result;

            //get all open scripts to update node structure
            foreach (TabPage t in TabScripts.TabPages)
            {
                if (t.Tag is ScriptGenerator)
                    ((ScriptGenerator)t.Tag).BlocksToNodes();
                else if (t.Tag is LuaDialogManager)
                    ((LuaDialogManager)t.Tag).BlocksToNodes();
            }
            //save project
            project.Save(SaveProjectDialog.FileName);

            SetStatus("Project saved");
            return DialogResult.OK;
        }

        private DialogResult ProjectClose()
        {
            if (project == null)
                return DialogResult.OK;

            DialogResult res = MessageBox.Show("Do you want to save before quitting?", "Save before quit?", MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Cancel)
                return res;
            if (res == DialogResult.OK)
            {
                res = ProjectSave();
                if (res == DialogResult.Cancel)
                    return res;
            }

            TabScripts.TabPages.Clear();
            ListScripts.Items.Clear();
            ListPlatforms.Items.Clear();
            LabelProject.Text = "Project:";

            project = null;
            SetStatus("Project closed");
            return res;
        }

        private DialogResult ProjectLoad()
        {
            DialogResult res = DialogResult.OK;

            if (project != null)
                res = ProjectClose();
            if (res == DialogResult.Cancel)
                return res;

            res = OpenProjectDialog.ShowDialog();
            if (res != DialogResult.OK)
                return res;

            project = new ScriptProject();
            project.Load(OpenProjectDialog.FileName);

            LabelProject.Text = "Project: " + project.Name;
            RefreshPlatformListBox();

            SetStatus("Project loaded");
            return res;
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectSave();
        }

        private void loadProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectLoad();
        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectClose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void previewActiveScriptCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //get open script
            if ((TabScripts.TabCount == 0) || (TabScripts.SelectedTab == null))
                return;

            ScriptGenerator script = null;

            if (TabScripts.SelectedTab.Tag is ScriptGenerator)
                script = (ScriptGenerator)TabScripts.SelectedTab.Tag;
            else if (TabScripts.SelectedTab.Tag is LuaDialogManager)
            {
                //find an open tab with fitting name, return if no tab found, get script from that tab
                string s_name = TabScripts.SelectedTab.Name.Substring(0, TabScripts.SelectedTab.Name.Length - 9); // " (dialog)"
                TabPage tp = null;
                foreach (TabPage t in TabScripts.TabPages)
                    if (t.Name == s_name)
                    {
                        tp = t;
                        break;
                    }
                if (tp == null)
                    return;
                script = (ScriptGenerator)tp.Tag;
            }
            else
                return;

            ShowCodeForm sf = new ShowCodeForm();
            sf.SetText(script.BuildCode());
            sf.ShowDialog();
        }

        private void renameProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (project == null)
            {
                SetStatus("Open or create a project first");
                return;
            }

            string n_name = Utility.GetString("Choose project name", "Enter project name below");
            if (n_name != null)
            {
                project.Name = n_name;
                LabelProject.Text = "Project: " + project.Name;
            }
        }

        private void renamePlatformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (project == null)
            {
                SetStatus("Open or create a project first");
                return;
            }

            if (ListPlatforms.SelectedIndex == -1)
            {
                SetStatus("Select a platform first");
                return;
            }
            ScriptPlatform cur_platform = project.PlatformGet(ListPlatforms.SelectedItem.ToString());
            if (cur_platform == null)
                return;

            int p_index = ListPlatforms.SelectedIndex;
            string n_name = Utility.GetString("Choose platform name", "Enter platform name below");
            if (n_name != null)
            {
                project.PlatformRename(cur_platform.Name, n_name);
                ListPlatforms.Items[p_index] = n_name;
            }
        }

        private void renameScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((TabScripts.TabCount == 0) || (TabScripts.SelectedTab == null))
            {
                SetStatus("No script tab selected");
                return;
            }

            if (!(TabScripts.SelectedTab.Tag is ScriptGenerator))
            {
                SetStatus("Select a script tab first (dialog tab doesn't count)");
                return;
            }

            string n_name = Utility.GetString("Choose script name", "Enter script name below");
            if (n_name != null)
            {
                string old_name = ((ScriptGenerator)(TabScripts.SelectedTab.Tag)).Name;
                TabScripts.SelectedTab.Name = n_name;
                TabScripts.SelectedTab.Text = n_name;
                ((ScriptGenerator)(TabScripts.SelectedTab.Tag)).Platform.ScriptRename(old_name, n_name);
                //also change name of an open dialog tab
                foreach (TabPage t in TabScripts.TabPages)
                    if((t.Name == old_name+" (dialog)")||(t.Text == old_name+" (dialog)"))
                    {
                        t.Name = n_name + " (dialog)";
                        t.Text = n_name + " (dialog)";
                        break;
                    }
                RefreshScriptListBox();
                RefreshScriptInfo();
                SetStatus("Renamed script " + old_name + " to " + n_name);
            }
        }

        private void changeActiveScriptTypeToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if ((TabScripts.TabCount == 0) || (TabScripts.SelectedTab == null))
            {
                SetStatus("No script tab selected");
                return;
            }

            if (!(TabScripts.SelectedTab.Tag is ScriptGenerator))
            {
                SetStatus("Select a script tab first (dialog tab doesn't count)");
                return;
            }

            ((ScriptGenerator)(TabScripts.SelectedTab.Tag)).SType = (ScriptType)Enum.Parse(typeof(ScriptType), e.ClickedItem.Text);
            RefreshScriptInfo();
        }

        private void RefreshScriptInfo()
        {
            PanelScriptInfo.Visible = true;
            if ((TabScripts.TabCount == 0) || (TabScripts.SelectedTab == null) || (!(TabScripts.SelectedTab.Tag is ScriptGenerator)))
            {
                LabelScriptName.Text = "";
                LabelScriptType.Text = "";
                ButtonDialogEdit.Visible = false;
                PanelScriptInfo.Visible = false;
                PanelScriptInfo.Tag = null;
                return;
            }
            LabelScriptName.Text = ((ScriptGenerator)TabScripts.SelectedTab.Tag).Name;
            LabelScriptType.Text = ((ScriptGenerator)TabScripts.SelectedTab.Tag).SType.ToString("F");
            ButtonDialogEdit.Visible = ((ScriptGenerator)TabScripts.SelectedTab.Tag).SType == ScriptType.SCRIPT_NPC;
            PanelScriptInfo.Tag = TabScripts.SelectedTab.Tag;
        }

        private void TabScripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshScriptInfo();
            SelectLuaControl(null);
        }

        private void OpenDialog(string name)
        {
            if (project == null)
                return;

            ScriptGenerator script = (ScriptGenerator)PanelScriptInfo.Tag;
            if (script == null)
            {
                SetStatus("Error: no script found!");
                return;
            }

            if (script.dialog == null)
                script.dialog = new SFLua.lua_dialog.LuaDialogManager();

            //create new tab layout
            Panel pan = new Panel();
            pan.SuspendLayout();
            pan.Size = new Size(497, 370);
            pan.BackColor = SystemColors.Window;
            pan.AutoScroll = true;

            TabScripts.TabPages.Insert(0, script.Name+" (dialog)");
            TabScripts.TabPages[0].Name = name+" (dialog)";
            TabScripts.TabPages[0].Controls.Add(pan);
            TabScripts.TabPages[0].Tag = script.dialog;
            script.dialog.PhaseControls = pan;

            TabScripts.SelectedIndex = 0;
            RefreshScriptInfo();
            script.dialog.NodesToBlocks(pan);
            pan.ResumeLayout();

            SetStatus("Opened dialog for a script " + script.Name);
        }

        private void ButtonDialogEdit_Click(object sender, EventArgs e)
        {
            string s_name = ((ScriptGenerator)PanelScriptInfo.Tag).Name;

            //look for open scripts
            for (int i = 0; i < TabScripts.TabPages.Count; i++)
                if (TabScripts.TabPages[i].Name == s_name+" (dialog)")
                {
                    TabScripts.SelectedIndex = i;
                    return;
                }

            //open script
            OpenDialog(s_name);
        }

        public void MoveDialogTabViewToPhase(int phase_id)
        {
            if (TabScripts.TabPages.Count == 0)
                return;
            if (!(TabScripts.SelectedTab.Tag is LuaDialogManager))
                return;

            Panel panel = (Panel)TabScripts.SelectedTab.Controls[0];
            for(int i = 0; i < panel.Controls.Count; i++)
            {
                LuaDialogPhaseControl phase = (LuaDialogPhaseControl)panel.Controls[i];
                if(Utility.TryParseInt32(phase.GetPhaseTextBox().Text) == phase_id)
                {
                    panel.AutoScrollPosition = new Point(0, panel.AutoScrollPosition.Y + phase.Location.Y);
                    break;
                }
            }
        }
    }
}
