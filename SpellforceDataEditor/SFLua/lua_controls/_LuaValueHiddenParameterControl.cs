using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua.lua_controls
{
    public partial class _LuaValueHiddenParameterControl : SpellforceDataEditor.SFLua.lua_controls.LuaValueControl
    {
        public _LuaValueHiddenParameterControl()
        {
            InitializeComponent();
        }

        public _LuaValueHiddenParameterControl(string v)
        {
            Value = new LuaValue(v);
            Default = new LuaValue(v);
            InitializeComponent();
            PSChar = "";

            RefreshName();
        }

        public override bool IsDefault()
        {
            return false;
        }

        public override string ToCode(bool ignore_name)
        {
            return Value.ToCodeString('\0');
        }
    }
}
