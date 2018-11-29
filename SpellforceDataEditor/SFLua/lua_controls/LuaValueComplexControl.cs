using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SpellforceDataEditor.SFLua.lua_controls;

namespace SpellforceDataEditor.SFLua.lua_controls
{
    public partial class LuaValueComplexControl : SpellforceDataEditor.SFLua.lua_controls.LuaValueControl
    {
        static public int WidthComplexBase = 280;
        static public int HeightComplexBase = 60;
        static public int WidthDiff = WidthComplexBase - WidthBase;
        static public int HeightDiff = HeightComplexBase - HeightBase;
        public int YOffset = 0;

        public bool Collapsed { get; protected set; } = false;
        public LuaParseFlag ParseFlags { get; set; } = 0;

        // controls managed by ValuesPanel
        // actual values are in parameters

        public LuaValueComplexControl()
        {
            ParseFlags = 0;
            PSChar = "";
            Value = new LuaValue(null);
            Value.Name = "";
            Default = new LuaValue(null);

            InitializeComponent();
            RefreshName();
        }

        public LuaValueComplexControl(LuaParseFlag parse = 0, string ps = "")
        {
            ParseFlags = parse;
            PSChar = ps;
            Value = new LuaValue(null);
            Value.Name = "ew";
            Default = new LuaValue(null);

            InitializeComponent();
            RefreshName();
        }

        public LuaValueControl AddLuaControl(LuaValueControl c, string n = null)
        {
            if (n != null)
            {
                c.Value.Name = n;
                c.RefreshName();
            }
            c.ParentControl = this;

            ValuesPanel.Controls.Add(c);

            return c;
        }

        public void RemoveLuaControl(int index)
        {
            Control c = ValuesPanel.Controls[index];
            ValuesPanel.Controls.RemoveAt(index);
            c.Dispose();

            ResetSize();
        }

        public void RemoveLuaControl(LuaValueControl c)
        {
            for (int i = 0; i < ValuesPanel.Controls.Count; i++)
                if (ValuesPanel.Controls[i] == c)
                {
                    RemoveLuaControl(i);
                    return;
                }
        }

        public void RemoveSelf()
        {
            if (ParentControl is LuaValueComplexControl)
                ((LuaValueComplexControl)ParentControl).RemoveLuaControl(this);
        }

        public int GetTotalControlY()
        {
            int result = HeightComplexBase;
            if (Collapsed)
                return result;
            foreach (Control l in ValuesPanel.Controls)
            {
                result += l.Height;
            }
            return result;
        }

        public int GetTotalControlX()
        {
            int result = WidthComplexBase;
            if (Collapsed)
                return result;
            foreach (Control l in ValuesPanel.Controls)
            {
                result = Math.Max(result, l.Width + WidthDiff);
            }
            return result;
        }

        public override void ResetSize()
        {
            if (Collapsed)
            {
                Height = HeightBase;
                Width = WidthBase;
            }
            else
            {
                Height = GetTotalControlY();
                Width = GetTotalControlX();
                ValuesPanel.Width = Width - WidthDiff;
                ValuesPanel.Height = Height - HeightDiff;
            }
            if (ParentControl != null)
                ParentControl.ResetSize();
            else if (Parent is lua_dialog.LuaDialogControl)
                ((lua_dialog.LuaDialogControl)Parent).ResetSize();
            Invalidate();
        }

        public void SwitchParameterOrder(int param_1, int param_2)
        {
            //get both LuaValueControls
            LuaValueControl c1 = (LuaValueControl)ValuesPanel.Controls[param_1];
            LuaValueControl c2 = (LuaValueControl)ValuesPanel.Controls[param_2];
            int pi1 = ValuesPanel.Controls.GetChildIndex(c1);
            int pi2 = ValuesPanel.Controls.GetChildIndex(c2);

            //switch controls
            ValuesPanel.Controls.SetChildIndex(c1, pi2);
            ValuesPanel.Controls.SetChildIndex(c2, pi1);
        }

        public void Collapse()
        {
            if (Collapsed)
                return;
            Collapsed = true;
            ValuesPanel.Hide();
            ResetSize();
        }

        public void Expand()
        {
            if (!Collapsed)
                return;
            Collapsed = false;
            ValuesPanel.Show();
            ResetSize();
        }

        public void ToggleCollapsed()
        {
            if (Collapsed)
                Expand();
            else
                Collapse();
        }

        public LuaValue this[int index]
        {
            get
            {
                return ((LuaValueControl)ValuesPanel.Controls[index]).Value;
            }
        }

        public FlowLayoutPanel GetValuesPanel()
        {
            return ValuesPanel;
        }

        private void LuaValueComplexControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                ToggleCollapsed();
        }

        public override bool IsDefault()
        {
            return (!Important) && (GetValuesPanel().Controls.Count == 0);
        }

        public override string ToCode(bool ignore_name)
        {
            string code = "";
            if(!ignore_name)
            {
                code += Value.Name;
                if ((ParseFlags & LuaParseFlag.IsParameter) != 0)
                    code += " = ";
            }
            if (PSChar != "")
                code += PSChar[0];
            if ((ParseFlags & LuaParseFlag.BlockNewLines) != 0)
                code += "\r\n";

            int max_params = ValuesPanel.Controls.Count;
            for(int i = 0; i < max_params;)
            {
                LuaValueControl block = (LuaValueControl)ValuesPanel.Controls[i];
                bool is_default = block.IsDefault();

                //System.Diagnostics.Debug.WriteLine(block.Value.Name+" "+block.IsDefault().ToString());
                if (!is_default)
                {
                    System.Diagnostics.Debug.WriteLine(block.Value.Name);
                    string block_code = block.ToCode((ParseFlags & LuaParseFlag.IgnoreParamName) != 0);
                    if ((ParseFlags & LuaParseFlag.Indents) != 0)
                        block_code = block_code.Replace("\n", "\n\t");
                    code += block_code;
                }
                i += 1;
                //determine whether to put comma

                if(((ParseFlags & LuaParseFlag.SeparatingCommas)!= 0)&&(!is_default))
                    code += ",";
                if ((i == max_params) && ((ParseFlags & LuaParseFlag.LastComma) == 0))
                    code = code.Remove(code.Length - 1, 1);
                if (!is_default)
                {
                    if (i != max_params)
                    {
                        if ((ParseFlags & LuaParseFlag.ParamNewLines) != 0)
                            code += "\r\n";
                        else
                            code += " ";
                    }
                }
            }

            if ((ParseFlags & LuaParseFlag.BlockNewLines) != 0)
                code += "\r\n";
            if (PSChar != "")
                code += PSChar[1];

            //System.Diagnostics.Debug.WriteLine(Value.Name);
            //System.Diagnostics.Debug.WriteLine(code);

            //System.Diagnostics.Debug.WriteLine(Value.Name + " " + IsDefault().ToString() + "; code -> " + code);

            return code;
        }
    }
}
