using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SpellforceDataEditor.special_forms.utility_forms;

namespace SpellforceDataEditor.SFLua.lua_controls
{
    public partial class LuaValueCustomControl : SpellforceDataEditor.SFLua.lua_controls.LuaValueControl
    {
        public LuaValueCustomControl()
        {
            InitializeComponent();
        }

        public LuaValueCustomControl(string v)
        {
            Value = new LuaValue(v);
            Default = new LuaValue(v);
            InitializeComponent();

            luaParams.ParentControl = this;
            luaParams.Value.Name = "Parameters";
            luaParams.RefreshName();

            RefreshName();
        }

        public override bool IsDefault()
        {
            return (!Important) && ((string)Value.Value) == ((string)Default.Value);
        }

        public override void ResetSize()
        {
            Width = luaParams.Width + 40;
            Height = luaParams.Height + 24;
            if (ParentControl != null)
                ParentControl.ResetSize();
            Invalidate();
        }

        public LuaValueComplexControl GetParamsControl()
        {
            return luaParams;
        }

        private void ButtonCustomCode_Click(object sender, EventArgs e)
        {
            ShowCodeForm code_form = new ShowCodeForm();
            code_form.SetText((string)(Value.Value));

            code_form.ShowDialog();

            Value.Value = code_form.GetText();
            code_form.Dispose();
        }

        public override void SetControlValue(object val)
        {
            if (Value == null)
                Value = new LuaValue(val);
            Value.Value = (string)val;
        }

        public override string ToCode(bool ignore_name)
        {
            //replace all parameter name occurences with code
            string s = Value.ToCodeString('\0');
            //iterate over all param controls
            foreach(Control c in luaParams.GetValuesPanel().Controls)
            {
                LuaValueControl param = (LuaValueControl)c;
                string replace_string = "$(" + param.Value.Name + ")";
                s = s.Replace(replace_string, param.ToCode(true));
            }
            return s;
        }
    }
}
