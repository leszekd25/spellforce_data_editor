using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua.lua_controls
{
    public partial class LuaValueControl : UserControl
    {
        static public int WidthBase = 224;
        static public int HeightBase = 24;

        static protected ContextMenuStrip menu = null;
        private bool important = false;
        public int ID { get; set; } = -1;
        public LuaValue Value { get; set; } = null;
        public LuaValue Default { get; set; } = null;
        public bool Important
        {
            get { return important; }
            set
            {
                important = value;
                LabelName.Font = new Font(LabelName.Font, (important ? FontStyle.Bold : FontStyle.Regular));
                LabelName.Refresh();
            }
        }
        public LuaValueControl ParentControl { get; set; } = null;
        public bool AllowRemove { get; set; } = false;
        public string PSChar { get; set; } = "";

        public LuaValueControl()
        {
            InitializeComponent();
        }

        public LuaValueControl(LuaValue v)
        {
            if (v is null)
                throw new ArgumentNullException("ERROR IN LuaValueControl: Value reference is NULL!");
            Value = v;
            InitializeComponent();

            RefreshName();
        }

        public void RefreshName()
        {
            LabelName.Text = Value.Name;
        }

        public virtual void SetControlValue(object val)
        {

        }

        public virtual bool IsDefault()
        {
            return (!Important)&&(Value.Value == null);
        }

        public virtual void ResetSize()
        {
            if (ParentControl != null)
                ParentControl.ResetSize();
            Invalidate();
        }

        public virtual string ToCode(bool ignore_name)
        {
            if (IsDefault())
                return "";

            char cc;
            if (PSChar == "")
                cc = '\0';
            else
                cc = PSChar[0];
            if (ignore_name)
                return Value.ToCodeString(cc);
            return Value.Name + " = " + Value.ToCodeString(cc);
        }

        protected virtual string[] GetMenuItems()
        {
            return new string[] { "Rename", "Move up", "Move down", "Remove" };
        }

        protected virtual void CustomMenuItemCall(string selected)
        {
            return;
        }

        private void OnMenuItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string selected = e.ClickedItem.Text;

            if (selected == "Remove")
            {
                if (AllowRemove)
                {
                    ((special_forms.ScriptBuilderForm)FindForm()).SelectLuaControl(ParentControl);
                    LuaValueComplexControl par = ((LuaValueComplexControl)ParentControl);
                    menu.Dispose();
                    menu = null;
                    par.RemoveLuaControl(this);
                    return;
                }
            }
            else if (selected == "Rename")
            {
                string name = Utility.GetString("Choose parameter name", "Enter parameter name below");
                if (name != null)
                {
                    Value.Name = name;
                    RefreshName();
                }
            }
            //space for move up and move down operations
            else
                CustomMenuItemCall(selected);
            
            menu.Dispose();
            menu = null;
        }


        private void ButtonMenu_Click(object sender, EventArgs e)
        {
            menu = new ContextMenuStrip();
            string[] items = GetMenuItems();
            foreach (string s in items)
                menu.Items.Add(s);
            menu.ItemClicked += new ToolStripItemClickedEventHandler(OnMenuItemClicked);
            System.Diagnostics.Debug.WriteLine("PRESSED");
            menu.Show(this, new Point(0, 0));
        }
    }
}
