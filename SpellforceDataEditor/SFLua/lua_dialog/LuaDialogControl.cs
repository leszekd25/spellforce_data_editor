/*
 * LuaDialogControl is a base control for all dialog controls
 * These are used to visualise a dialog structure used by NPC scripts
 * */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua.lua_dialog
{
    public partial class LuaDialogControl : UserControl
    {
        public bool Collapsed { get; set; } = false;

        public LuaDialogControl()
        {
            InitializeComponent();
        }

        //renames the control
        public void Rename(string name)
        {
            LabelName.Text = name;
        }

        //must be called at the end of ResetSize()
        //allows dialog control to recursively resize to fit its content in a parent control
        public void PropagateSize()
        {
            if ((Parent != null) && (Parent.Parent != null))
            {
                if (Parent.Parent is LuaDialogControl)
                    ((LuaDialogControl)Parent.Parent).ResetSize();
            }
        }

        //sets size based on content and whether or not the control is collapsed
        public virtual void ResetSize()
        {
            this.Size = new Size(100, 24);
            PropagateSize();
        }

        //collapses the control
        public void Collapse()
        {
            if (Collapsed)
                return;
            Collapsed = true;
            ResetSize();
        }

        //expands the control
        public void Expand()
        {
            if (!Collapsed)
                return;
            Collapsed = false;
            ResetSize();
        }

        public void ToggleCollapsed()
        {
            if (Collapsed)
                Expand();
            else
                Collapse();
        }

        //if the control contains a panel with its children, returns the panel
        public virtual Panel GetControlPanel()
        {
            return null;
        }

        //removes this control, along with all its children, from the dialog structure
        public void Delete()
        {
            Control par = Parent;
            Parent.Controls.Remove(this);
            if ((par != null)&&(par.Parent != null))
            {
                if (par.Parent is LuaDialogControl)
                    ((LuaDialogControl)par.Parent).ResetSize();
                else if (par.Parent is TabPage)
                    ((special_forms.ScriptBuilderForm)par.FindForm()).FixPhasePositions((Panel)par);
            }

        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void collapseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Collapse();
        }

        private void expandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Expand();
        }

        //copy from (?)
        //drag and drop (?)
    }
}
