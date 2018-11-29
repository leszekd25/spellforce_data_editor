using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua.lua_controls
{
    public partial class LuaValueDoubleControl : SpellforceDataEditor.SFLua.lua_controls.LuaValueControl
    {
        public LuaValueDoubleControl()
        {
            InitializeComponent();
        }

        public LuaValueDoubleControl(double v)
        {
            Value = new LuaValue(v);
            Default = new LuaValue(v);
            InitializeComponent();

            RefreshName();

            TextBoxValue.Text = Value.ToString();
        }
        
        public override bool IsDefault()
        {
            return (!Important) && ((double)Value.Value) == ((double)Default.Value);
        }

        public override void SetControlValue(object val)
        {
            if (Value == null)
                Value = new LuaValue(val);
            TextBoxValue.Text = ((double)val).ToString();
        }

        private void TextBoxValue_TextChanged(object sender, EventArgs e)
        {
            Value.Value = Utility.TryParseDouble(TextBoxValue.Text);
        }
    }
}
