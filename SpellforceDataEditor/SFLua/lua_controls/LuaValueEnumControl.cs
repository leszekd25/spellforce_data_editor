using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua.lua_controls
{
    public partial class LuaValueEnumControl : SpellforceDataEditor.SFLua.lua_controls.LuaValueControl
    {
        public LuaValueEnumControl()
        {
            InitializeComponent();
        }

        public LuaValueEnumControl(Enum v)
        {
            Value = new LuaValue(v);
            Default = new LuaValue(v);
            InitializeComponent();

            RefreshName();

            SetControlValue(v);
        }

        public override bool IsDefault()
        {
            return (!Important) && ((Enum)Value.Value) == ((Enum)Default.Value);
        }

        public override void SetControlValue(object val)
        {
            System.Diagnostics.Debug.WriteLine(val.ToString());
            if (Value == null)
                Value = new LuaValue(val);

            SetEnumType(val.GetType());

            string cur = ((Enum)val).ToString("F");
            for (int i = 0; i < ComboValue.Items.Count; i++)
            {
                if (cur == ComboValue.Items[i].ToString())
                {
                    ComboValue.SelectedIndex = i;
                    break;
                }
            }
        }

        private void SetEnumType(Type t)
        {
            ComboValue.Items.Clear();

            foreach (string s in t.GetEnumNames())
                ComboValue.Items.Add(s);

            Value.Value = (Enum)t.GetEnumValues().GetValue(0);

            ComboValue.SelectedIndex = 0;
        }

        protected override void CustomMenuItemCall(string selected)
        {
            if(selected == "Change Enum type")
            {
                special_forms.utility_forms.SelectLuaEnumForm select_enum = new special_forms.utility_forms.SelectLuaEnumForm();
                select_enum.ShowDialog();
                if (select_enum.Result == DialogResult.OK)
                    SetEnumType(select_enum.EnumType);
            }
        }

        protected override string[] GetMenuItems()
        {
            return new string[] { "Rename", "Change Enum type", "Move up", "Move down", "Remove" };
        }

        private void ComboValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboValue.SelectedIndex == -1)
                return;
            Value.Value = (Enum)Enum.Parse((Value.Value).GetType(), ComboValue.SelectedItem.ToString());
        }
    }
}
