/*
 * Definitions for conditions and actions which deal with saving game progress
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpellforceDataEditor.SFLua.lua_controls;

namespace SpellforceDataEditor.SFLua
{
    static public class LuaVariable
    {
        static LuaParseFlag LuaParseFlagsVarParam = LuaParseFlag.SeparatingCommas;

        //actions

        static public LuaValueComplexControl SetItemFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetItemFlagTrue";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetItemFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetItemFlagFalse";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetRewardFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetRewardFlagTrue";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetRewardFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetRewardFlagFalse";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }
        static public LuaValueComplexControl SetGlobalFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetGlobalFlagTrue";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetGlobalFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetGlobalFlagFalse";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetPlayerFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetPlayerFlagTrue";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetPlayerFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetPlayerFlagFalse";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetNpcFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetNpcFlagTrue";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetNpcFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetNpcFlagFalse";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetPlatformFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetPlatformFlagTrue";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetPlatformFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetPlatformFlagFalse";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ResetGlobalCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "ResetGlobalCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IncreaseGlobalCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IncreaseGlobalCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(1), "Step");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl DecreaseGlobalCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "DecreaseGlobalCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(1), "Step");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ResetPlayerCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "ResetPlayerCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IncreasePlayerCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IncreasePlayerCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(1), "Step");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl DecreasePlayerCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "DecreasePlayerCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(1), "Step");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ResetNpcCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "ResetNpcCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IncreaseNpcCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IncreaseNpcCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(1), "Step");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl DecreaseNpcCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "DecreaseNpcCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(1), "Step");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IncreasePlatformCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IncreasePlatformCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(1), "Step");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetGlobalState()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetGlobalState";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueStringControl(""), "State").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetPlayerState()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetPlayerState";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueStringControl(""), "State").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetNpcState()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetNpcState";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueStringControl(""), "State").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetPlatformState()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetPlatformState";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueStringControl(""), "State").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetGlobalTimeStamp()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetGlobalTimeStamp";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ClearGlobalTimeStamp()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "ClearGlobalTimeStamp";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetPlayerTimeStamp()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetPlayerTimeStamp";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ClearPlayerTimeStamp()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "ClearPlayerTimeStamp";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetNpcTimeStamp()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetNpcTimeStamp";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ClearNpcTimeStamp()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "ClearNpcTimeStamp";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetPlatformTimeStamp()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "SetPlatformTimeStamp";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ClearPlatformTimeStamp()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "ClearPlatformTimeStamp";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl QuestBegin()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "QuestBegin";

            l.AddLuaControl(new LuaValueGDControl(42, 0), "QuestId").Important = true;
            l.AddLuaControl(new LuaValueBoolControl(false), "SubQuestActivate");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl QuestSolve()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "QuestSolve";

            l.AddLuaControl(new LuaValueGDControl(42, 0), "QuestId").Important = true;
            l.AddLuaControl(new LuaValueBoolControl(false), "ActivateNextQuest");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl QuestChangeState()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "QuestChangeState";

            l.AddLuaControl(new LuaValueGDControl(42, 0), "QuestId").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumQuestState.StateActive), "State").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        //conditions

        static public LuaValueComplexControl IsItemFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsItemFlagTrue";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsItemFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsItemFlagFalse";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsRewardFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsRewardFlagTrue";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsRewardFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsRewardFlagFalse";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }
        static public LuaValueComplexControl IsGlobalFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsGlobalFlagTrue";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsGlobalFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsGlobalFlagFalse";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsPlayerFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsPlayerFlagTrue";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsPlayerFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsPlayerFlagFalse";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsNpcFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsNpcFlagTrue";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsNpcFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsNpcFlagFalse";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsPlatformFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsPlatformFlagTrue";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsPlatformFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsPlatformFlagFalse";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsGlobalCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsGlobalCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Value").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumVariableCompare.IsEqual), "Operator");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsPlayerCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsPlayerCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Value").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumVariableCompare.IsEqual), "Operator");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsNpcCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsNpcCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Value").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumVariableCompare.IsEqual), "Operator");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsPlatformCounter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsPlatformCounter";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Value").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumVariableCompare.IsEqual), "Operator");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsGlobalState()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsGlobalState";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueStringControl(""), "State").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumVariableCompare.IsEqual), "Operator");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsPlayerState()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsPlayerState";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueStringControl(""), "State").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumVariableCompare.IsEqual), "Operator");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsNpcState()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsNpcState";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueStringControl(""), "State").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumVariableCompare.IsEqual), "Operator");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsPlatformState()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsPlatformState";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueStringControl(""), "State").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumVariableCompare.IsEqual), "Operator");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsGlobalTimeElapsed()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsGlobalTimeElapsed";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Seconds").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsNpcTimeElapsed()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsNpcTimeElapsed";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Seconds").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsPlayerTimeElapsed()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsPlayerTimeElapsed";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Seconds").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsPlatformTimeElapsed()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "IsPlatformTimeElapsed";

            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Seconds").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl QuestState()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsVarParam, "{}");
            l.Value.Name = "QuestState";

            l.AddLuaControl(new LuaValueGDControl(42, 0), "QuestId").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumQuestState.StateSolved), "State").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }
    }
}
