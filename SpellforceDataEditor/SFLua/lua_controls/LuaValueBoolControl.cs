using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua.lua_controls
{
    public partial class LuaValueBoolControl : SpellforceDataEditor.SFLua.lua_controls.LuaValueControl
    {
        public LuaValueBoolControl()
        {
            InitializeComponent();
        }

        public LuaValueBoolControl(bool v)
        {
            Value = new LuaValue(v);
            Default = new LuaValue(v);
            InitializeComponent();

            RefreshName();

            ComboValue.SelectedIndex = (bool)(Value.Value)?1:0;
        }
        
        public override bool IsDefault()
        {
            return (!Important) && ((bool)Value.Value) == ((bool)Default.Value);
        }

        public override string ToCode(bool ignore_name)
        {
            if (IsDefault())
                return "";

            if (ignore_name)
                return ((bool)Value.Value) ? "TRUE" : "FALSE";// Value.ToCodeString(cc);
            return Value.Name + " = " + (((bool)Value.Value) ? "TRUE" : "FALSE");//Value.ToCodeString(cc);
        }

        public override void SetControlValue(object val)
        {
            if (Value == null)
                Value = new LuaValue(val);
            ComboValue.SelectedIndex = ((bool)val) ? 1 : 0;
        }

        private void ComboValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            Value.Value = ComboValue.SelectedIndex == 1;
        }
    }
}
