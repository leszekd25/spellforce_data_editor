using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua.lua_controls
{
    public partial class LuaValueGDControl : SpellforceDataEditor.SFLua.lua_controls.LuaValueControl
    {
        public LuaValueGDControl()
        {
            InitializeComponent();
        }

        public LuaValueGDControl(LuaValue_GameDataStruct v)
        {
            Value = new LuaValue(v);
            Default = new LuaValue(v);
            InitializeComponent();

            RefreshName();

            TextBoxValue.Text = v.id.ToString();
        }

        public LuaValueGDControl(byte cat, ushort el)
        {
            LuaValue_GameDataStruct l;
            l.category = cat;
            l.id = el;
            Value = new LuaValue(l);
            Default = new LuaValue(l);
            InitializeComponent();

            RefreshName();

            TextBoxValue.Text = l.id.ToString();
        }

        public override bool IsDefault()
        {
            return (!Important) && ((LuaValue_GameDataStruct)Value.Value) == ((LuaValue_GameDataStruct)Default.Value);
        }

        public override void SetControlValue(object val)
        {
            if (Value == null)
                Value = new LuaValue(val);
            TextBoxValue.Text = ((ushort)val).ToString();
        }

        private void TextBoxValue_TextChanged(object sender, EventArgs e)
        {
            LuaValue_GameDataStruct l;
            l.category = ((LuaValue_GameDataStruct)Value.Value).category;
            l.id = Utility.TryParseUInt16(TextBoxValue.Text);
            Value.Value = l;
        }

        private void TextBoxValue_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (!SFCategoryManager.ready)
                    return;
                LuaValue_GameDataStruct l = ((LuaValue_GameDataStruct)Value.Value);
                int elem_key = (int)l.id;
                int real_elem_id;
                SFCategory cat = SFCategoryManager.get_category(l.category);
                char format = cat.get_element_format()[0];
                if (format == 'B')
                    real_elem_id = cat.find_element_index<Byte>(0, (Byte)elem_key);
                else if (format == 'H')
                    real_elem_id = cat.find_element_index<UInt16>(0, (UInt16)elem_key);
                else if (format == 'I')
                    real_elem_id = cat.find_element_index<UInt32>(0, (UInt32)elem_key);
                else
                    return;
                if (real_elem_id == -1)
                    return;
                ((special_forms.ScriptBuilderForm)ParentForm).Editor.Tracer_StepForward(l.category, real_elem_id, false);
            }
        }
    }
}
