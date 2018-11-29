/*
 * Description of events and structures related to cutscenes
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpellforceDataEditor.SFLua.lua_controls;

namespace SpellforceDataEditor.SFLua
{
    static public class LuaCinematic
    {
        static LuaParseFlag LuaParseFlagsEvent = LuaParseFlag.BlockNewLines | LuaParseFlag.Indents | LuaParseFlag.ParamNewLines | LuaParseFlag.SeparatingCommas;
        static LuaParseFlag LuaParseFlagsTable = LuaParseFlag.BlockNewLines | LuaParseFlag.Indents | LuaParseFlag.IsParameter | LuaParseFlag.LastComma | LuaParseFlag.ParamNewLines | LuaParseFlag.SeparatingCommas;
        static LuaParseFlag LuaParseFlagsAction = LuaParseFlag.SeparatingCommas;

        static public LuaValueCustomControl CameraScript()
        {
            LuaValueCustomControl l = new LuaValueCustomControl("");
            l.Value.Name = "CameraScript";

            l.Value.Value = "$(cameraname) = {\r\n[[\r\n\tCamera:ScriptReset()\r\n\tCamera:ScriptAddSpline(0,$(duration),1,\"script\\$(scriptpath).lua\")\r\n\tCamera:ScriptStart()\r\n]]";
            l.GetParamsControl().AddLuaControl(new LuaValueParameterControl("camera#"), "cameraname").Important = true;
            l.GetParamsControl().AddLuaControl(new LuaValueDoubleControl(10), "duration").Important = true;
            l.GetParamsControl().AddLuaControl(new LuaValueParameterControl(""), "scriptpath").Important = true;
            l.GetParamsControl().Important = true;
            l.GetParamsControl().ResetSize();

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl CreateCutScene()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "CreateCutScene";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "BeginConditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "BeginActions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "TimedActions").Important = true;
            l.AddLuaControl(new LuaValueBoolControl(true), "PlayOnlyOnce");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ExecuteCameraScript()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsAction, "{}");
            l.Value.Name = "ExecuteCameraScript";

            l.AddLuaControl(new LuaValueParameterControl(""), "Script").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public _LuaValueHiddenParameterControl WaitForEndOfSpeech()
        {
            _LuaValueHiddenParameterControl l = new _LuaValueHiddenParameterControl("WaitForEndOfSpeech");
            l.Value.Name = "WaitForEndOfSpeech";

            l.RefreshName();
            l.ResetSize();

            return l;
        }


        static public LuaValueComplexControl CutSceneSay()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsAction, "{}");
            l.Value.Name = "CutSceneSay";

            l.AddLuaControl(new LuaValueStringControl(""), "String").Important = true;
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId").Important = true;
            l.AddLuaControl(new LuaValueStringControl(""), "Tag");
            l.AddLuaControl(new LuaValueColorControl(0xFFFF3300), "Color");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl CutSceneEnd()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsAction, "{}");
            l.Value.Name = "CutSceneEnd";

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl CutSceneBegin()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsAction, "{}");
            l.Value.Name = "CutSceneBegin";

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl TimedAction()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsTable, "{}");
            l.Value.Name = "[#]";

            l.RefreshName();
            l.ResetSize();

            return l;
        }
    }
}
