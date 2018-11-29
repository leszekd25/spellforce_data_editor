/*
 * LuaDialogPhaseControl is the main dialog block
 * It's responsible for generating code based on its children - LuaDialogSayControl and LuaDialogAnswerControl
 * One phase can contain multiple says, and each of says -> multiple answers
 * */

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
    public partial class LuaDialogPhaseControl : SpellforceDataEditor.SFLua.lua_dialog.LuaDialogControl
    {
        public LuaDialogPhaseControl()
        {
            InitializeComponent();
            Rename("Phase");
            //resize (350, 300)
        }

        public LuaDialogPhaseControl(int phase_id)
        {
            InitializeComponent();
            Rename("Phase");
            TextBoxPhase.Text = phase_id.ToString();
            //resize (350, 300)
        }

        //takes a lua complex control, builds a list of codes generated from control's children, and returns the list
        public List<string> GetTableCode(LuaValueComplexControl table)
        {
            List<string> ret = new List<string>();
            for (int i = 0; i < table.GetValuesPanel().Controls.Count; i++)
                ret.Add(((LuaValueComplexControl)table.GetValuesPanel().Controls[i]).ToCode(false));
            return ret;
        }

        //helper function which generates a negated code from supplied code
        public string Negated(string s)
        {
            if (s.StartsWith("Negated("))
                return s.Substring(0, s.Length - 1).Substring(8);
            return "Negated(" + s + ")";
        }

        //generates a one-line condition code from all supplied condition codes
        public string UnifyConditions(List<string> lines)
        {
            string ret = "";
            for(int i = lines.Count-1; i >= 0; i--)
            {
                if (i != lines.Count - 1)
                    ret = "UND(" + lines[i] + " , " + ret + ")";
                else
                    ret = lines[i];
            }
            return ret;
        }

        //generates code from the structure
        public string GenerateCode()
        {
            //step 1. gather all says and answers
            int SayCount = GetSayCount();
            List<int> AnswerCount = new List<int>();
            for (int i = 0; i < SayCount; i++)
                AnswerCount.Add(GetSay(i).GetAnswerCount());
            //step 2. build code snippets for say condition, say action and each answer condition
            List<int> SayConditionCount = new List<int>();
            for (int i = 0; i < SayCount; i++)
                SayConditionCount.Add(GetSay(i).GetConditions().GetValuesPanel().Controls.Count);
            List<int> SayActionCount = new List<int>();
            for (int i = 0; i < SayCount; i++)
                SayActionCount.Add(GetSay(i).GetActions().GetValuesPanel().Controls.Count);

            List<List<string>> SayConditionCode = new List<List<string>>();
            for (int i = 0; i < SayCount; i++)
            {
                SayConditionCode.Add(new List<string>());
                for (int j = 0; j < SayConditionCount[i]; j++)
                    SayConditionCode[i].Add(((LuaValueComplexControl)GetSay(i).GetConditions().GetValuesPanel().Controls[j]).ToCode(false));
            }
            List<List<string>> SayActionCode = new List<List<string>>();
            for (int i = 0; i < SayCount; i++)
            {
                SayActionCode.Add(new List<string>());
                for (int j = 0; j < SayActionCount[i]; j++)
                    SayActionCode[i].Add(((LuaValueComplexControl)GetSay(i).GetActions().GetValuesPanel().Controls[j]).ToCode(false));
            }
            List<List<string>> AnswerConditionCode = new List<List<string>>();
            for(int i = 0; i < SayCount; i++)
            {
                AnswerConditionCode.Add(new List<string>());
                for(int j = 0; j < AnswerCount[i]; j++)
                {
                    AnswerConditionCode[i].Add(UnifyConditions(GetTableCode(GetSay(i).GetAnswer(j).GetConditions())));
                }
            }

            //step 3. iterate through every possibility and build code
            string code_name = LabelName.Text;
            bool add_answer_id = (code_name == "OnAnswer");
            string code = "";
            for(int i = 0; i < SayCount; i++)
            {
                string ans_str = ((AnswerCount[i] > 1)||(GetSay(i).GetAllowChooseAnswer().Checked)) ? "OfferAnswer" : "Answer";
                int max_combinations = 1 << AnswerCount[i];
                //determine which answers show up unconditionally, so the options aren't shown up twice
                int flag_unconditions = 0;
                for(int j = 0; j < AnswerCount[i]; j++)
                {
                    if (AnswerConditionCode[i][j] == "")
                        flag_unconditions += (1 << j);
                }
                for(int j = 0; j < max_combinations; j++)
                {
                    //if any uncondition is to show up again, ignore this combination
                    if ((j & flag_unconditions) != 0)
                        continue;
                    //header
                    code += code_name + "{" + (add_answer_id ? TextBoxPhase.Text + ";" : "") + "\r\n";
                    code += "\tConditions = {\r\n";
                    //conditions for the say
                    for (int k = 0; k < SayConditionCount[i]; k++)
                        code += "\t\t" + SayConditionCode[i][k] + ",\r\n";
                    //conditions for the actions
                    int flag_test = 0x1;
                    for(int k = 0; k < AnswerCount[i]; k++)
                    {
                        //if it's an uncondition, don't add any condition, otherwise check which circumstance this is
                        if(AnswerConditionCode[i][k]!="")
                            code += "\t\t" + (((j & flag_test) == 0) ? Negated(AnswerConditionCode[i][k]) : AnswerConditionCode[i][k])+",\r\n";
                        flag_test <<= 1;
                    }
                    code += "\t},\r\n";
                    code += "\tActions = {\r\n";
                    //actions for the say
                    for (int k = 0; k < SayActionCount[i]; k++)
                        code += "\t\t" + SayActionCode[i][k] + ",\r\n";
                    //say code
                    code += "\t\tSay{Tag = " + (GetSay(i).GetTag().IsDefault() ? "\"\"" : GetSay(i).GetTag().ToCode(true))
                        + " , String = " + (GetSay(i).GetText().IsDefault() ? "\"\"" : GetSay(i).GetText().ToCode(true))
                        + ", Color = " + (GetSay(i).GetColor().IsDefault() ? "ColorWhite" : GetSay(i).GetColor().ToCode(true))
                        + "},\r\n";
                    //answer code
                    flag_test = 0x1;
                    for(int k = 0; k < AnswerCount[i]; k++)
                    {
                        //only show answer if circumstance allows for it, or if it's an uncondition
                        if(((j & flag_test) != 0)||(AnswerConditionCode[i][k] == ""))
                        {
                            code += "\t\t" + ans_str + "{Tag = " + (GetSay(i).GetAnswer(k).GetTag().IsDefault() ? "\"\"" : GetSay(i).GetAnswer(k).GetTag().ToCode(true))
                                + " , String = " + (GetSay(i).GetAnswer(k).GetText().IsDefault() ? "\"\"" : GetSay(i).GetAnswer(k).GetText().ToCode(true))
                                + " , Color = " + (GetSay(i).GetAnswer(k).GetColor().IsDefault() ? "ColorWhite" : GetSay(i).GetAnswer(k).GetColor().ToCode(true))
                                + (GetSay(i).GetAnswer(k).GetPhase().IsDefault() ? "" : " , AnswerId = " + GetSay(i).GetAnswer(k).GetPhase().ToCode(true))
                                + "},\r\n";
                        }
                        flag_test <<= 1;
                    }
                    //footer
                    code += "\t}}\r\n\r\n";
                }
            }

            return code;
        }

        //adds say block
        public void AddSay()
        {
            LuaDialogSayControl say = new LuaDialogSayControl();
            Point n_pos = GetNextSayPosition();
            PanelControls.Controls.Add(say);
            say.Location = new Point(n_pos.X + PanelControls.AutoScrollPosition.X, n_pos.Y + PanelControls.AutoScrollPosition.Y);
            ResetSize();
        }

        //returns next empty position for a say
        public Point GetNextSayPosition()
        {
            Point p = new Point(10, 10);
            if (PanelControls.Controls.Count != 0)
                p = PanelControls.Controls[0].Location;
            foreach (Control c in PanelControls.Controls)
                p.X += c.Width + 10;
            return p;
        }

        //returns a say block given its index
        public LuaDialogSayControl GetSay(int i)
        {
            return ((LuaDialogSayControl)PanelControls.Controls[i]);
        }

        //returns how many says there are in a phase
        public int GetSayCount()
        {
            return PanelControls.Controls.Count;
        }

        private void ButtonAddSay_Click(object sender, EventArgs e)
        {
            AddSay();
        }

        //returns phase id textbox
        public TextBox GetPhaseTextBox()
        {
            return TextBoxPhase;
        }

        //resets all say positions
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
                Height = 53; //todo: remove arbitrary values
                Width = 350;
                PanelControls.Hide();
            }
            else
            {
                PanelControls.Show();
                Size new_panel_size = new Size(343, 273);

                FixControlPositions();
                foreach (Control say in PanelControls.Controls)
                {
                    new_panel_size.Width = Math.Max(new_panel_size.Width, say.Location.X + say.Width + 10);
                    new_panel_size.Height = Math.Max(new_panel_size.Height, say.Height);
                }
                new_panel_size.Width = Math.Min(new_panel_size.Width, 470);
                new_panel_size.Height = Math.Min(new_panel_size.Height, 273);

                PanelControls.Size = new_panel_size;
                this.Size = new Size(PanelControls.Location.X + PanelControls.Width + 4, PanelControls.Location.Y + PanelControls.Height + 4);
            }

            special_forms.ScriptBuilderForm form = (special_forms.ScriptBuilderForm)FindForm();
            form.FixPhasePositions((Panel)this.Parent);

            PropagateSize();
        }
    }
}
