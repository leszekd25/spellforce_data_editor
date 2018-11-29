using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua.lua_controls
{
    public partial class LuaValueColorControl : SpellforceDataEditor.SFLua.lua_controls.LuaValueControl
    {
        public LuaValueColorControl()
        {
            Value = new LuaValue(0);
            Default = new LuaValue(0);
            InitializeComponent();

            RefreshName();

            ButtonColorPick.BackColor = Color.FromArgb(0);
        }

        public LuaValueColorControl(uint v)
        {
            Value = new LuaValue(v);
            Default = new LuaValue(v);
            InitializeComponent();

            RefreshName();

            ButtonColorPick.BackColor = Color.FromArgb((int)(v));
        }

        public override bool IsDefault()
        {
            return (!Important) && ((uint)Value.Value) == ((uint)Default.Value);
        }

        public override void SetControlValue(object val)
        {
            if (Value == null)
                Value = new LuaValue(val);
            ButtonColorPick.BackColor = Color.FromArgb((int)((uint)val >> 24), (int)((uint)val >> 16) & 0xFF, (int)((uint)val >> 8) & 0xFF, (int)((uint)val) & 0xFF);
            Value.Value = (uint)val;
        }

        private void ButtonColorPick_Click(object sender, EventArgs e)
        {
            if(ColorDialog.ShowDialog() == DialogResult.OK)
            {
                ButtonColorPick.BackColor = ColorDialog.Color;
                Value.Value = (uint)(ColorDialog.Color.ToArgb());
            }
        }
    }
}
