/*
 * Description of general type events
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpellforceDataEditor.SFLua.lua_controls;

namespace SpellforceDataEditor.SFLua
{
    static public class LuaEvent
    {
        static LuaParseFlag LuaParseFlagsEvent = LuaParseFlag.BlockNewLines | LuaParseFlag.Indents | LuaParseFlag.ParamNewLines | LuaParseFlag.SeparatingCommas;
        static LuaParseFlag LuaParseFlagsTable = LuaParseFlag.BlockNewLines | LuaParseFlag.Indents | LuaParseFlag.IsParameter | LuaParseFlag.LastComma | LuaParseFlag.ParamNewLines | LuaParseFlag.SeparatingCommas;
        static LuaParseFlag LuaParseFlagsParamList = LuaParseFlag.IsParameter | LuaParseFlag.SeparatingCommas;
        static LuaParseFlag LuaParseFlagsList = LuaParseFlag.SeparatingCommas | LuaParseFlag.IgnoreParamName;

        static public LuaValueComplexControl OnEvent()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnEvent";

            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions").Important = true;
            l.AddLuaControl(new LuaValueBoolControl(false), "NotInDialog");
            l.AddLuaControl(new LuaValueBoolControl(false), "RemoveTransition");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnIdleGoHome()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnIdleGoHome";

            l.AddLuaControl(new LuaValueIntControl(0), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Y").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumDirection.East), "Direction");
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumWalkMode.Walk), "WalkMode");
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumGotoMode.GotoNormal), "GotoMode");
            l.AddLuaControl(new LuaValueIntControl(0), "Range");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "AbortConditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "AbortActions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "HomeActions");
            l.AddLuaControl(new LuaValueBoolControl(false), "NotInDialog");
            l.AddLuaControl(new LuaValueIntControl(0), "UpdateInterval");
            l.AddLuaControl(new LuaValueBoolControl(false), "NoIdle");
            l.AddLuaControl(new LuaValueBoolControl(false), "CheckOnlyAggro");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnDeadPlayerGoHome()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnDeadPlayerGoHome";

            l.AddLuaControl(new LuaValueIntControl(0), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Y").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumDirection.East), "Direction").Important = true;
            l.AddLuaControl(new LuaValueBoolControl(false), "KeepFollowing");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "HomeActions");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnIdleEvent()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnIdleEvent";

            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions").Important = true;
            l.AddLuaControl(new LuaValueBoolControl(false), "NotInDialog");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnPlatformOneTimeEvent()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnPlatformOneTimeEvent";

            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions").Important = true;
            l.AddLuaControl(new LuaValueIntControl(10), "UpdateInterval");
            l.AddLuaControl(new LuaValueBoolControl(false), "NotInDialog");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnOneTimeEvent()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnOneTimeEvent";

            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");
            l.AddLuaControl(new LuaValueBoolControl(false), "NotInDialog");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnToggleEvent()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnToggleEvent";

            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "OnConditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "OnActions").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "OffConditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "OffActions").Important = true;
            l.AddLuaControl(new LuaValueIntControl(10), "UpdateInterval");
            l.AddLuaControl(new LuaValueBoolControl(false), "NotInDialog");
            l.AddLuaControl(new LuaValueBoolControl(false), "ResetOnPlatformLoad");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnDeath()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnDeath";

            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnDelayCommand()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnDelayCommand";

            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumVariable.NpcVariable), "Type");
            l.AddLuaControl(new LuaValueIntControl(1), "ReRuns");
            l.AddLuaControl(new LuaValueIntControl(1), "Seconds");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");
            l.AddLuaControl(new LuaValueBoolControl(false), "NotInDialog");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnIdleDoTorchJob()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnIdleDoTorchJob";

            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumDirection.East), "Direction").Important = true;
            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueBoolControl(false), "TorchOff");
            l.AddLuaControl(new LuaValueStringControl(""), "Effect");
            l.AddLuaControl(new LuaValueIntControl(-1), "Length");
            l.AddLuaControl(new LuaValueBoolControl(true), "WaitForIdle");
            l.AddLuaControl(new LuaValueIntControl(30), "UpdateInterval");
            l.AddLuaControl(new LuaValueBoolControl(false), "NotInDialog");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnIdleWalkPath()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnIdleWalkPath";

            l.AddLuaControl(new LuaValueIntControl(0), "Loop");
            l.AddLuaControl(new LuaValueBoolControl(false), "WaitForIdle");
            l.AddLuaControl(new LuaValueIntControl(-1), "Length");
            l.AddLuaControl(new LuaValueStringControl(""), "SetEffect");
            l.AddLuaControl(new LuaValueStringControl(""), "StopEffect");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "EndConditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "EndActions");
            l.AddLuaControl(new LuaValueBoolControl(false), "NotInDialog");
            l.AddLuaControl(new LuaValueIntControl(0), "WaitForPlayerInRange");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl AddGotoLocation()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "AddGotoLocation";

            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueIntControl(1), "Range");
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumWalkMode.Walk), "WalkMode");
            l.AddLuaControl(new LuaValueIntControl(0), "XRand");
            l.AddLuaControl(new LuaValueIntControl(0), "YRand");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ClearGotoLocations()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "ClearGotoLocations";

            l.RefreshName();
            l.Collapse();

            return l;
        }

        static public LuaValueComplexControl ResetWalkPath()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "ResetWalkPath";

            l.AddLuaControl(new LuaValueIntControl(1), "Num");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnPortalEvent()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnPortalEvent";

            l.AddLuaControl(new LuaValueEnumControl(LuaEnumPortalType.StadtTor), "Type").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "OpenConditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "OpenActions");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnPlayerReviveOrPlatformEnter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnPlayerReviveOrPlatformEnter";

            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnPlayerDeathOrPlatformChange()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnPlayerDeathOrPlatformChange";

            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnQuestItemLost()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnQuestItemLost";

            l.AddLuaControl(new LuaValueGDControl(6, 0), "ItemID").Important = true; //pending change
            l.AddLuaControl(new LuaValueStringControl(""), "ItemFlagName").Important = true;
            l.AddLuaControl(new LuaValueIntControl(1), "Amount");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnQuestItemRemove()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnQuestItemRemove";

            l.AddLuaControl(new LuaValueGDControl(6, 0), "ItemID").Important = true; //pending change
            l.AddLuaControl(new LuaValueStringControl(""), "ItemFlagName").Important = true;
            l.AddLuaControl(new LuaValueIntControl(1), "Amount");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnQuestItemFound()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnQuestItemFound";

            l.AddLuaControl(new LuaValueGDControl(6, 0), "ItemID").Important = true; //pending change
            l.AddLuaControl(new LuaValueStringControl(""), "ItemFlagName").Important = true;
            l.AddLuaControl(new LuaValueIntControl(1), "Amount");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnAttackPattern()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnAttackPattern";

            LuaValueComplexControl alpha = new LuaValueComplexControl(LuaParseFlagsTable, "{}");
            alpha.AddLuaControl(new LuaValueIntControl(1), "Retries");
            alpha.AddLuaControl(new LuaValueDoubleControl(1), "GuardTime");

            LuaValueComplexControl group1 = new LuaValueComplexControl(LuaParseFlagsParamList, "{}");
            group1.AddLuaControl(new LuaValueIntControl(-1), "X");
            group1.AddLuaControl(new LuaValueIntControl(-1), "Y");
            group1.AddLuaControl(new LuaValueEnumControl(LuaEnumDirection.East), "Direction");
            group1.AddLuaControl(new LuaValueEnumControl(LuaEnumWalkMode.Walk), "WalkMode");
            group1.ResetSize();

            alpha.AddLuaControl(group1, "[1]");
            alpha.ResetSize();

            l.AddLuaControl(alpha, "PatternAlpha").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "PatternDelta");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "PatternOmega");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "CommonGoal");
            l.AddLuaControl(new LuaValueStringControl(""), "Name").Important = true;
            l.AddLuaControl(new LuaValueBoolControl(false), "RestartAfterCommonGoal");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnFollowMe()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnFollowMe";

            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumDirection.East), "Direction");
            l.AddLuaControl(new LuaValueIntControl(20), "LeadRange");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "HomeActions");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnFollowToggle()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnFollowToggle";

            l.AddLuaControl(new LuaValueBoolControl(false), "FollowOnlyOnce");
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueIntControl(-1), "Target").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "FollowConditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "FollowActions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "StopFollowConditions").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "StopFollowActions");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl OnFollowForever()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "OnFollowForever";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueGDControl(36, 0), "Target").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SpawnOnlyWhen()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "SpawnOnlyWhen";

            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");
            l.AddLuaControl(new LuaValueGDControl(36, 65535), "Target");
            l.AddLuaControl(new LuaValueIntControl(0), "X");
            l.AddLuaControl(new LuaValueIntControl(0), "Y");
            l.AddLuaControl(new LuaValueGDControl(16, 0), "Clan");
            l.AddLuaControl(new LuaValueGDControl(17, 0), "UnitId");
            l.AddLuaControl(new LuaValueBoolControl(false), "NoSpawnEffect");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl Respawn()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "Respawn";

            l.AddLuaControl(new LuaValueIntControl(0), "WaitTime").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "DeathActions");
            l.AddLuaControl(new LuaValueGDControl(36, 65535), "Target");
            l.AddLuaControl(new LuaValueIntControl(0), "X");
            l.AddLuaControl(new LuaValueIntControl(0), "Y");
            l.AddLuaControl(new LuaValueGDControl(16, 0), "Clan");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsList, "{}"), "Chief");
            l.AddLuaControl(new LuaValueGDControl(17, 0), "UnitId");
            l.AddLuaControl(new LuaValueBoolControl(false), "NoSpawnEffect");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl Despawn()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "Despawn";

            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions").Important = true;
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");
            l.AddLuaControl(new LuaValueBoolControl(false), "PlayDeathAnim");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl Umspawn()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsEvent, "{}");
            l.Value.Name = "Umspawn";
            
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Conditions");
            l.AddLuaControl(new LuaValueComplexControl(LuaParseFlagsTable, "{}"), "Actions");
            l.AddLuaControl(new LuaValueGDControl(36, 65535), "Target");
            l.AddLuaControl(new LuaValueIntControl(0), "X");
            l.AddLuaControl(new LuaValueIntControl(0), "Y");
            l.AddLuaControl(new LuaValueGDControl(16, 0), "Clan");
            l.AddLuaControl(new LuaValueGDControl(17, 0), "UnitId");
            l.AddLuaControl(new LuaValueBoolControl(false), "PlayDeathAnim");
            l.AddLuaControl(new LuaValueBoolControl(false), "NoSpawnEffect");
            l.AddLuaControl(new LuaValueBoolControl(true), "OnlyOnce");

            l.RefreshName();
            l.ResetSize();

            return l;
        }
    }
}
