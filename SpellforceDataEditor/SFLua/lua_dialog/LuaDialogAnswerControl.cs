using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SpellforceDataEditor.SFLua.lua_controls;

namespace SpellforceDataEditor.SFLua.lua_dialog
{
    public partial class LuaDialogAnswerControl : SpellforceDataEditor.SFLua.lua_dialog.LuaDialogControl
    {
        public LuaDialogAnswerControl()
        {
            InitializeComponent();
            StringText.SetControlValue(""); StringText.Default = new LuaValue(""); StringText.PSChar = "\"\"";
            StringText.Value.Name = "String"; StringText.RefreshName();
            StringTag.SetControlValue(""); StringTag.Default = new LuaValue(""); StringTag.PSChar = "\"\"";
            StringTag.Value.Name = "Tag"; StringTag.RefreshName();
            ColorColor.SetControlValue((uint)0xFFFFFFFF); ColorColor.Default = new LuaValue((uint)0xFFFFFFFF); ColorColor.PSChar = "";
            ColorColor.Value.Name = "Color"; ColorColor.RefreshName();
            IntGotoPhase.SetControlValue((int)0); IntGotoPhase.Default = new LuaValue((int)0); IntGotoPhase.PSChar = "";
            IntGotoPhase.Value.Name = "AnswerId"; IntGotoPhase.RefreshName();
            Conditions.Value = new LuaValue(null); Conditions.Default = new LuaValue(null); Conditions.PSChar = "{}";
            Conditions.Value.Name = "Conditions"; Conditions.RefreshName();
            Rename("Say");
        }

        public override void ResetSize()
        {
            if (Collapsed)
            {
                this.Conditions.Hide();
                Height = 119;
                Width = 200;
            }
            else
            {
                Conditions.Show();
                this.Size = new Size(Conditions.Location.X + Conditions.Width + 4, Conditions.Location.Y + Conditions.Height + 4);
                this.Width = Math.Max(239, Conditions.Width);
            }

            PropagateSize();
        }

        public LuaValueComplexControl GetConditions()
        {
            return Conditions;
        }

        public LuaValueStringControl GetTag()
        {
            return StringTag;
        }

        public LuaValueStringControl GetText()
        {
            return StringText;
        }

        public LuaValueColorControl GetColor()
        {
            return ColorColor;
        }

        public LuaValueIntControl GetPhase()
        {
            return IntGotoPhase;
        }

        private void IntGotoPhase_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                special_forms.ScriptBuilderForm form = (special_forms.ScriptBuilderForm)FindForm();
                form.MoveDialogTabViewToPhase((int)IntGotoPhase.Value.Value);
            }
        }
    }
}
