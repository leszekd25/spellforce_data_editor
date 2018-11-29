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
    public partial class LuaDialogSayControl : SpellforceDataEditor.SFLua.lua_dialog.LuaDialogControl
    {
        public LuaDialogSayControl()
        {
            InitializeComponent();
            StringText.SetControlValue(""); StringText.Default = new LuaValue(""); StringText.PSChar = "\"\"";
            StringText.Value.Name = "String"; StringText.RefreshName();
            StringTag.SetControlValue(""); StringTag.Default = new LuaValue(""); StringTag.PSChar = "\"\"";
            StringTag.Value.Name = "Tag"; StringTag.RefreshName();
            ColorColor.SetControlValue((uint)0xFFFFFFFF); ColorColor.Default = new LuaValue((uint)0xFFFFFFFF); ColorColor.PSChar = "";
            ColorColor.Value.Name = "Color"; ColorColor.RefreshName();
            Conditions.Value = new LuaValue(null); Conditions.Default = new LuaValue(null); Conditions.PSChar = "{}";
            Conditions.Value.Name = "Conditions"; Conditions.RefreshName();
            Actions.Value = new LuaValue(null); Actions.Default = new LuaValue(null); Actions.PSChar = "{}";
            Actions.Value.Name = "Actions"; Actions.RefreshName();
            Rename("Say");
        }

        public Point GetNextAnswerPosition()
        {
            Point p = new Point(10, 10);
            if (PanelControls.Controls.Count != 0)
                p = PanelControls.Controls[0].Location;
            foreach (Control c in PanelControls.Controls)
                p.X += c.Width + 10;
            return p;
        }

        //generate code
        public void AddAnswer()
        {
            LuaDialogAnswerControl say = new LuaDialogAnswerControl();
            Point n_pos = GetNextAnswerPosition();
            PanelControls.Controls.Add(say);
            say.Location = new Point(n_pos.X + PanelControls.AutoScrollPosition.X, n_pos.Y + PanelControls.AutoScrollPosition.Y);
            ResetSize();
        }

        public void FixControlPositions()
        {
            if (PanelControls.Controls.Count != 0)
            {
                Control first_c = PanelControls.Controls[0];
                first_c.Location = new Point(10 + PanelControls.AutoScrollPosition.X, 10 + PanelControls.AutoScrollPosition.Y);
            }
            for (int i = 1; i < PanelControls.Controls.Count; i++)
            {
                Control c1 = PanelControls.Controls[i - 1];
                Control c2 = PanelControls.Controls[i];
                c2.Location = new Point(c1.Location.X + c1.Width + 10, c1.Location.Y);
            }
        }

        public override void ResetSize()
        {
            if (Collapsed)
            {
                Height = 116;
                Width = 200;
                PanelControls.Hide();
            }
            else
            {
                PanelControls.Show();
                Size new_panel_size = new Size(236, 166);

                FixControlPositions();
                foreach (Control say in PanelControls.Controls)
                {
                    new_panel_size.Width = Math.Max(new_panel_size.Width, say.Location.X + say.Width + 10);
                    new_panel_size.Height = Math.Max(new_panel_size.Height, say.Height);
                }
                new_panel_size.Width = Math.Min(new_panel_size.Width, 346);
                new_panel_size.Height = Math.Min(new_panel_size.Height, 346);

                PanelControls.Size = new_panel_size;
                Actions.Location = new Point(Conditions.Location.X, Conditions.Location.Y + Conditions.Height);
                PanelControls.Location = new Point(PanelControls.Location.X, Actions.Location.Y + Actions.Height);
                this.Size = new Size(PanelControls.Location.X + PanelControls.Width + 4, PanelControls.Location.Y + PanelControls.Height + 4);
                this.Width = PanelControls.Location.X + Math.Max(236, Math.Max(Conditions.Width, Math.Max(PanelControls.Width, Actions.Width))) + 4;
            }

            PropagateSize();
        }

        public LuaDialogAnswerControl GetAnswer(int i)
        {
            return ((LuaDialogAnswerControl)PanelControls.Controls[i]);
        }

        public int GetAnswerCount()
        {
            return PanelControls.Controls.Count;
        }

        public LuaValueComplexControl GetConditions()
        {
            return Conditions;
        }

        public LuaValueComplexControl GetActions()
        {
            return Actions;
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

        public CheckBox GetAllowChooseAnswer()
        {
            return CheckBoxChooseAnswer;
        }

        private void ButtonAnswer_Click(object sender, EventArgs e)
        {
            AddAnswer();
        }
    }
}
