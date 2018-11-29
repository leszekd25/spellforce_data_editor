using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using SpellforceDataEditor.SFLua.lua_controls;
using SpellforceDataEditor.SFLua.lua_filesystem;

namespace SpellforceDataEditor.SFLua.lua_dialog
{
    public class LuaDialogAnswer
    {
        public string Text { get; set; } = "";
        public string Tag { get; set; } = "";
        public uint Color { get; set; } = 0xFFFFFFFF;
        public int GotoPhase { get; set; } = 0;
        public List<ScriptNode> ConditionNodes { get; private set; } = new List<ScriptNode>();
        public List<LuaValueControl> ConditionControls { get; private set; } = new List<LuaValueControl>();

        public void BlocksToNodes(LuaDialogAnswerControl answer)
        {
            LuaUtility.BlocksToNodes(answer.GetConditions(), ConditionControls, ConditionNodes);
        }

        public void NodesToBlocks(LuaDialogAnswerControl answer)
        {
            LuaUtility.NodesToBlocks(answer.GetConditions(), ConditionControls, ConditionNodes);
        }

        public void Save(BinaryWriter br)
        {
            //write text, tag, color, goto phase
            //write condition nodes count, condition nodes
            br.Write(Text);
            br.Write(Tag);
            br.Write(Color);
            br.Write(GotoPhase);
            System.Diagnostics.Debug.WriteLine("ANSWER SAVE! " + Text + " : " + Tag);
            br.Write(ConditionNodes.Count);
            for (int i = 1; i < ConditionNodes.Count; i++)
                ConditionNodes[i].Save(br);
        }

        public void Load(BinaryReader br)
        {
            Text = br.ReadString();
            Tag = br.ReadString();
            Color = br.ReadUInt32();
            GotoPhase = br.ReadInt32();
            System.Diagnostics.Debug.WriteLine("ANSWER LOAD! " + Text + " : " + Tag);

            int condition_count = br.ReadInt32();
            ConditionNodes.Clear();
            ConditionControls.Clear();

            ScriptNode n = new ScriptNode();
            n.name = "Conditions"; n.id = 0; n.parent_id = -1;
            n.type = ScriptNodeValueType.COMPLEX; n.value = null; n.def = null;
            n.parse_flags = 0; n.pschar = "{}"; n.important = true;
            ConditionNodes.Add(n);

            for (int i = 1; i < condition_count; i++)
            {
                ScriptNode node = ScriptNode.Load(br);
                ConditionNodes.Add(node);
            }
        }
    }

    public class LuaDialogSay
    {
        public string Text { get; set; } = "";
        public string Tag { get; set; } = "";
        public uint Color { get; set; } = 0xFFFFFFFF;
        public bool AllowChooseAnswer { get; set; } = false;
        public List<ScriptNode> ConditionNodes { get; private set; } = new List<ScriptNode>();
        public List<LuaValueControl> ConditionControls { get; private set; } = new List<LuaValueControl>();
        public List<ScriptNode> ActionNodes { get; private set; } = new List<ScriptNode>();
        public List<LuaValueControl> ActionControls { get; private set; } = new List<LuaValueControl>();
        public List<LuaDialogAnswer> Answers { get; private set; } = new List<LuaDialogAnswer>();

        public void BlocksToNodes(LuaDialogSayControl say)
        {
            LuaUtility.BlocksToNodes(say.GetConditions(), ConditionControls, ConditionNodes);
            LuaUtility.BlocksToNodes(say.GetActions(), ActionControls, ActionNodes);
            Answers.Clear();
            for (int i = 0; i < say.GetAnswerCount(); i++)
            {
                Answers.Add(new LuaDialogAnswer());
                LuaDialogAnswerControl answer = say.GetAnswer(i);
                Answers[i].Text = (string)answer.GetText().Value.Value;
                Answers[i].Tag = (string)answer.GetTag().Value.Value;
                Answers[i].Color = (uint)answer.GetColor().Value.Value;
                Answers[i].GotoPhase = (int)answer.GetPhase().Value.Value;
                Answers[i].BlocksToNodes(say.GetAnswer(i));
            }
        }

        public void NodesToBlocks(LuaDialogSayControl say)
        {
            LuaUtility.NodesToBlocks(say.GetConditions(), ConditionControls, ConditionNodes);
            LuaUtility.NodesToBlocks(say.GetActions(), ActionControls, ActionNodes);
            for(int i = 0; i < Answers.Count; i++)
            {
                say.AddAnswer();
                LuaDialogAnswerControl answer = say.GetAnswer(i);
                answer.GetText().SetControlValue(Answers[i].Text);
                answer.GetTag().SetControlValue(Answers[i].Tag);
                answer.GetColor().SetControlValue(Answers[i].Color);
                answer.GetPhase().SetControlValue(Answers[i].GotoPhase);
                Answers[i].NodesToBlocks(answer);
            }
        }

        public void Save(BinaryWriter br)
        {
            //write text, tag, color, allowchooseanswer
            //write condition nodes count, condition nodes
            //write action nodes count, action nodes
            //write answer count
            //write answers
            br.Write(Text);
            br.Write(Tag);
            br.Write(Color);
            br.Write(AllowChooseAnswer);
            System.Diagnostics.Debug.WriteLine("SAY SAVE! " + Text + " : " + Tag);
            br.Write(ConditionNodes.Count);
            for (int i = 1; i < ConditionNodes.Count; i++)
                ConditionNodes[i].Save(br);
            br.Write(ActionNodes.Count);
            for (int i = 1; i < ActionNodes.Count; i++)
                ActionNodes[i].Save(br);

            br.Write(Answers.Count);
            for (int i = 0; i < Answers.Count; i++)
                Answers[i].Save(br);
        }

        public void Load(BinaryReader br)
        {
            Answers.Clear();

            Text = br.ReadString();
            Tag = br.ReadString();
            Color = br.ReadUInt32();
            AllowChooseAnswer = br.ReadBoolean();
            System.Diagnostics.Debug.WriteLine("SAY LOAD! " + Text + " : " + Tag);

            int condition_count = br.ReadInt32();
            ConditionNodes.Clear();
            ConditionControls.Clear();

            ScriptNode n = new ScriptNode();
            n.name = "Conditions"; n.id = 0; n.parent_id = -1;
            n.type = ScriptNodeValueType.COMPLEX; n.value = null; n.def = null;
            n.parse_flags = 0; n.pschar = "{}"; n.important = true;
            ConditionNodes.Add(n);

            for (int i = 1; i < condition_count; i++)
            {
                ScriptNode node = ScriptNode.Load(br);
                ConditionNodes.Add(node);
            }

            int action_count = br.ReadInt32();
            ActionNodes.Clear();
            ActionControls.Clear();

            n = new ScriptNode();
            n.name = "Actions"; n.id = 0; n.parent_id = -1;
            n.type = ScriptNodeValueType.COMPLEX; n.value = null; n.def = null;
            n.parse_flags = 0; n.pschar = "{}"; n.important = true;
            ActionNodes.Add(n);

            for (int i = 1; i < action_count; i++)
            {
                ScriptNode node = ScriptNode.Load(br);
                ActionNodes.Add(node);
            }

            int answer_count = br.ReadInt32();
            for(int i = 0; i < answer_count; i++)
            {
                LuaDialogAnswer answer = new LuaDialogAnswer();
                answer.Load(br);
                Answers.Add(answer);
            }
        }
    }



    public class LuaDialogPhase
    {
        public int ID { get; set; } = -1;
        public List<LuaDialogSay> Says { get; private set; } = new List<LuaDialogSay>();

        public void BlocksToNodes(LuaDialogPhaseControl phase)
        {
            Says.Clear();
            for (int i = 0; i < phase.GetSayCount(); i++)
            {
                Says.Add(new LuaDialogSay());
                LuaDialogSayControl say = phase.GetSay(i);
                Says[i].Text = (string)say.GetText().Value.Value;
                Says[i].Tag = (string)say.GetTag().Value.Value;
                Says[i].Color = (uint)say.GetColor().Value.Value;
                Says[i].AllowChooseAnswer = say.GetAllowChooseAnswer().Checked;
                Says[i].BlocksToNodes(phase.GetSay(i));
            }
        }

        public void NodesToBlocks(LuaDialogPhaseControl phase)
        {
            for(int i = 0; i < Says.Count; i++)
            {
                phase.AddSay();
                LuaDialogSayControl say = phase.GetSay(i);
                say.GetText().SetControlValue(Says[i].Text);
                say.GetTag().SetControlValue(Says[i].Tag);
                say.GetColor().SetControlValue(Says[i].Color);
                say.GetAllowChooseAnswer().Checked = Says[i].AllowChooseAnswer;
                Says[i].NodesToBlocks(say);
            }
        }

        public void Save(BinaryWriter br)
        {
            //write id
            //write say count
            //write says
            br.Write(ID);
            br.Write(Says.Count);
            System.Diagnostics.Debug.WriteLine("PHASE SAVE! " + ID.ToString());
            for (int i = 0; i < Says.Count; i++)
                Says[i].Save(br);
        }

        public void Load(BinaryReader br)
        {
            Says.Clear();
            ID = br.ReadInt32();
            int say_count = br.ReadInt32();
            System.Diagnostics.Debug.WriteLine("PHASE LOAD! " + ID.ToString());
            for (int i = 0; i < say_count; i++)
            {
                LuaDialogSay say = new LuaDialogSay();
                say.Load(br);
                Says.Add(say);
            }
        }
    }

    public class LuaDialogManager
    {
        public Panel PhaseControls { get; set; } = null;
        public List<LuaDialogPhase> Phases { get; private set; } = new List<LuaDialogPhase>();

        public string GenerateCode()
        {
            if (PhaseControls == null)
                return "";

            string code = "";
            for (int i = 0; i < PhaseControls.Controls.Count; i++)
                code += ((LuaDialogPhaseControl)PhaseControls.Controls[i]).GenerateCode();

            return code;
        }

        public void BlocksToNodes()
        {
            Phases.Clear();
            for (int i = 0; i < PhaseControls.Controls.Count; i++)
            {
                Phases.Add(new LuaDialogPhase());
                LuaDialogPhaseControl phase = (LuaDialogPhaseControl)PhaseControls.Controls[i];
                Phases[i].ID = Utility.TryParseInt32(phase.GetPhaseTextBox().Text);
                Phases[i].BlocksToNodes(phase);
            }
        }

        public void NodesToBlocks(Panel panel)
        {
            PhaseControls = panel;
            for(int i = 0; i < Phases.Count; i++)
            {
                LuaDialogPhaseControl phase = new LuaDialogPhaseControl(Phases[i].ID);
                System.Drawing.Point n_pos = ((SpellforceDataEditor.special_forms.ScriptBuilderForm)panel.FindForm()).GetNextPhasePosition(panel);
                panel.Controls.Add(phase);
                phase.Location = n_pos;
                Phases[i].NodesToBlocks(phase);
            }
        }

        public void Save(BinaryWriter br)
        {
            br.Write(Phases.Count);
            System.Diagnostics.Debug.WriteLine("DIALOG SAVE! ");
            for (int i = 0; i < Phases.Count; i++)
            {
                Phases[i].Save(br);
            }
            //write phase count
            //write phases
        }

        public void Load(BinaryReader br)
        {
            PhaseControls = null;
            Phases.Clear();

            System.Diagnostics.Debug.WriteLine("DIALOG LOAD!");
            int phase_count = br.ReadInt32();
            for(int i = 0; i < phase_count; i++)
            {
                LuaDialogPhase phase = new LuaDialogPhase();
                phase.Load(br);
                Phases.Add(phase);
            }
        }
    }
}
