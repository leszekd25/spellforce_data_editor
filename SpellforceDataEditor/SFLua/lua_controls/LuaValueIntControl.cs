using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua.lua_controls
{
    public partial class LuaValueIntControl : SpellforceDataEditor.SFLua.lua_controls.LuaValueControl
    {
        public LuaValueIntControl()
        {
            InitializeComponent();
        }

        public LuaValueIntControl(int v)
        {
            Value = new LuaValue(v);
            Default = new LuaValue(v);
            InitializeComponent();

            RefreshName();

            TextBoxValue.Text = Value.ToString();
        }

        public override bool IsDefault()
        {
            return (!Important) && ((int)Value.Value) == ((int)Default.Value);
        }

        public override void SetControlValue(object val)
        {
            if (Value == null)
                Value = new LuaValue(val);
            TextBoxValue.Text = ((int)val).ToString();
        }

        private void TextBoxValue_TextChanged(object sender, EventArgs e)
        {
            Value.Value = Utility.TryParseInt32(TextBoxValue.Text);
        }
    }
}
