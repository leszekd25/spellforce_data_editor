using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua.lua_controls
{
    public partial class LuaValueParameterControl : SpellforceDataEditor.SFLua.lua_controls.LuaValueControl
    {
        public LuaValueParameterControl()
        {
            InitializeComponent();
        }

        public LuaValueParameterControl(string v)
        {
            Value = new LuaValue(v);
            Default = new LuaValue(v);
            InitializeComponent();
            PSChar = "";

            RefreshName();

            TextBoxValue.Text = Value.ToString();
        }


        public override bool IsDefault()
        {
            return (!Important) && ((string)Value.Value) == ((string)Default.Value);
        }

        public override void SetControlValue(object val)
        {
            if (Value == null)
                Value = new LuaValue(val);
            TextBoxValue.Text = (string)val;
        }

        private void TextBoxValue_TextChanged(object sender, EventArgs e)
        {
            Value.Value = TextBoxValue.Text;
        }
    }
}
