/*
 * Definition of events describing AI behavior
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpellforceDataEditor.SFLua.lua_controls;

namespace SpellforceDataEditor.SFLua
{
    static public class LuaRtsSpawnSystem
    {
        static LuaParseFlag LuaParseFlagsSpawn = LuaParseFlag.BlockNewLines | LuaParseFlag.Indents | LuaParseFlag.ParamNewLines | LuaParseFlag.SeparatingCommas | LuaParseFlag.LastComma;
        static LuaParseFlag LuaParseFlagsList = LuaParseFlag.BlockNewLines | LuaParseFlag.Indents | LuaParseFlag.IsParameter | LuaParseFlag.LastComma | LuaParseFlag.ParamNewLines | LuaParseFlag.SeparatingCommas;

        static LuaParseFlag LuaParseFlagsTable = LuaParseFlag.IgnoreParamName | LuaParseFlag.IsParameter | LuaParseFlag.SeparatingCommas;
        static LuaParseFlag LuaParseFlagsNamedTable = LuaParseFlag.IsParameter | LuaParseFlag.SeparatingCommas;

        static public LuaValueComplexControl RtsSpawnNT()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsSpawn, "{}");
            l.Value.Name = "RtsSpawnNT";

            l.AddLuaControl(new LuaValueGDControl(16, 0), "Clan").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "MaxClanSize").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Y").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Range");
            l.AddLuaControl(new LuaValueGDControl(17, 0), "Chief");
            l.AddLuaControl(new LuaValueIntControl(0), "AvatarMinLevel");
            l.AddLuaControl(new LuaValueIntControl(0), "AvatarMaxLevel");
            l.AddLuaControl(new LuaValueStringControl(""), "Timer").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Init").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "SpawnData").Important = true;
            //todo: fix this one
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "NpcBuildingExists");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsList, "{}"), "CampDestroyedActions");
            l.AddLuaControl(new LuaValueStringControl("Materialize"), "Effect");
            l.AddLuaControl(new LuaValueIntControl(2000), "Length");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl InitSpawn()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsSpawn, "{}");
            l.Value.Name = "InitSpawn";

            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Groups").Important = true;
            l.AddLuaControl(new LuaValueGDControl(16, 0), "Clan").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsSpawn, "{}"), "Conditions");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl RtsSpawn()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsSpawn, "{}");
            l.Value.Name = "RtsSpawn";

            l.AddLuaControl(new LuaValueGDControl(16, 0), "Clan").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "MaxClanSize").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "MaxClanLevel");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Groups").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsSpawn, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueStringControl(""), "Effect");
            l.AddLuaControl(new LuaValueIntControl(2000), "Length");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl RtsGroup()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsSpawn, "{}");
            l.Value.Name = "[group]";
            
            l.AddLuaControl(new LuaValueIntControl(0), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Y").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Range");
            l.AddLuaControl(new LuaValueGDControl(17, 0), "Chief");
            l.AddLuaControl(new LuaValueIntControl(0), "AvatarMinLevel");
            l.AddLuaControl(new LuaValueIntControl(0), "AvatarMaxLevel");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsSpawn, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsSpawn, "{}"), "BeginConditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Units").Important = true;
            l.AddLuaControl(new LuaValueBoolControl(false), "SpawnEffect");
            l.AddLuaControl(new LuaValueIntControl(0), "SpawnLimit");
            l.AddLuaControl(new LuaValueBoolControl(false), "ShuffleUnits");
            l.AddLuaControl(new LuaValueDoubleControl(0), "WaitTime").Important = true;
            
            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl AiBehavior()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsSpawn, "{}");
            l.Value.Name = "Clan_#";

            l.AddLuaControl(new LuaValueEnumControl(LuaEnumAiGoal.GoalIdle), "Goal").Important = true;
            //nomadic
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsNamedTable, "{}"), "NomadicRange");
            //aggressive
            l.AddLuaControl(new LuaValueIntControl(0), "MinimalHomePointCrew");
            l.AddLuaControl(new LuaValueIntControl(1), "MaximalHomePointCrew");
            l.AddLuaControl(new LuaValueIntControl(0), "StandByCrew");
            l.AddLuaControl(new LuaValueIntControl(0), "ScoutGroupSize");
            l.AddLuaControl(new LuaValueIntControl(0), "MaximalNumberScoutGroups");
            l.AddLuaControl(new LuaValueIntControl(5), "AttackGroupSize");
            l.AddLuaControl(new LuaValueParameterControl("Player"), "Enemy");
            //todo
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsNamedTable, "{}"), "HomePoint");
            l.AddLuaControl(new LuaValueIntControl(64), "Range");
            l.AddLuaControl(new LuaValueIntControl(60), "AlarmedWaitTime");
            l.AddLuaControl(new LuaValueIntControl(600), "AttackWaitTime");
            l.AddLuaControl(new LuaValueIntControl(300), "AttackFrequency");
            //defensive
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateWaitTime");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsNamedTable, "{}"), "Lookout");
            l.AddLuaControl(new LuaValueIntControl(1800), "FirstAttackWaitTime");


            l.RefreshName();
            l.ResetSize();

            return l;
        }
    }
}
