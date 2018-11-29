/*
 * Description of general type actions
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpellforceDataEditor.SFLua.lua_controls;


namespace SpellforceDataEditor.SFLua
{
    static public class LuaAction
    {
        static LuaParseFlag LuaParseFlagsActionParam = LuaParseFlag.SeparatingCommas;


        static public LuaValueComplexControl SetHealth()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "SetHealth";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueIntControl(0), "HitPoints").Important = true;
            l.AddLuaControl(new LuaValueBoolControl(false), "Decrease");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl Kill()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "Kill";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl HoldPosition()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "HoldPosition";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetNoFightFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "SetNoFightFlagFalse";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetNoFightFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "SetNoFightFlagTrue";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetFreezeFlagFalse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "SetFreezeFlagFalse";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetFreezeFlagTrue()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "SetFreezeFlagTrue";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetMinimapAlert()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "SetMinimapAlert";

            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumAlertType.AlertNotify), "Type");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ShowDebugText()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "ShowDebugText";

            l.AddLuaControl(new LuaValueStringControl(""), "String").Important = true;
            l.AddLuaControl(new LuaValueStringControl(""), "Tag");
            l.AddLuaControl(new LuaValueBoolControl(true), "ClearAfterSpeech");
            l.AddLuaControl(new LuaValueColorControl(0xFFFF3300), "Color");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetInfoText()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "SetInfoText";

            l.AddLuaControl(new LuaValueStringControl(""), "String").Important = true;
            l.AddLuaControl(new LuaValueStringControl(""), "Tag");
            l.AddLuaControl(new LuaValueBoolControl(true), "ClearAfterSpeech");
            l.AddLuaControl(new LuaValueColorControl(0xFFCBCBCB), "Color");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl Outcry()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "Outcry";

            l.AddLuaControl(new LuaValueStringControl(""), "String").Important = true;
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueStringControl(""), "Tag");
            l.AddLuaControl(new LuaValueColorControl(0xFFCBCBCB), "Color");
            l.AddLuaControl(new LuaValueBoolControl(true), "Delay");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl TalkAnimStop()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "TalkAnimStop";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl TalkAnimPlay()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "TalkAnimPlay";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl RemoveObject()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "RemoveObject";

            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;
            l.AddLuaControl(new LuaValueGDControl(33, 0), "Object").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SpecialPlaceObjects()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam | LuaParseFlag.IgnoreParamName, "{}");
            l.Value.Name = "SpecialPlaceObjects";

            LuaValueComplexControl ot = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            ot.Value.Name = "";

            ot.AddLuaControl(new LuaValueIntControl(-1), "X");
            ot.AddLuaControl(new LuaValueIntControl(-1), "Y");
            ot.AddLuaControl(new LuaValueGDControl(33, 0), "Object");

            ot.RefreshName();
            ot.ResetSize();

            l.AddLuaControl(ot).Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl PlaceObject()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "PlaceObject";

            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;
            l.AddLuaControl(new LuaValueGDControl(33, 0), "Object").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Angle");
            l.AddLuaControl(new LuaValueBoolControl(false), "DestroyNpc");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl RevealUnExplored()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "RevealUnExplored";

            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;
            l.AddLuaControl(new LuaValueIntControl(10), "Range");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl EndDialog()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "EndDialog";

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetDialogType()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "SetDialogType";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumQuestType.MainQuest), "Type");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl EnableDialog()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "EnableDialog";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl RemoveDialog()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "RemoveDialog";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl DebugLog()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "DebugLog";

            l.AddLuaControl(new LuaValueStringControl(""), "String").Important = true;
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ClanAttack()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "ClanAttack";

            l.AddLuaControl(new LuaValueIntControl(0), "Number").Important = true;
            l.AddLuaControl(new LuaValueGDControl(16, 0), "SourceClan").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "TargetX").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "TargetY").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ChangeGoal()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "ChangeGoal";

            l.AddLuaControl(new LuaValueGDControl(16, 0), "Clan").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumAiGoal.GoalNone), "Goal");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl AttackTarget()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "AttackTarget";

            l.AddLuaControl(new LuaValueIntControl(-1), "Target").Important = true;
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueBoolControl(false), "FriendlyFire");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl AttackWave()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "AttackWave";

            l.AddLuaControl(new LuaValueGDControl(15, 0), "SourceRace").Important = true;
            l.AddLuaControl(new LuaValueGDControl(15, 0), "TargetRace").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Percent").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl TransferXP()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "TransferXP";

            l.AddLuaControl(new LuaValueIntControl(0), "XP").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumTransferType.Give), "Flag");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl TransferMoney()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "TransferMoney";

            l.AddLuaControl(new LuaValueIntControl(0), "Copper");
            l.AddLuaControl(new LuaValueIntControl(0), "Silver");
            l.AddLuaControl(new LuaValueIntControl(0), "Gold");
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumTransferType.Give), "Flag");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl TransferItem()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "TransferItem";

            l.AddLuaControl(new LuaValueGDControl(6, 0), "TakeItem");
            l.AddLuaControl(new LuaValueGDControl(6, 0), "GiveItem");
            l.AddLuaControl(new LuaValueIntControl(1), "Amount");
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumTransferType.Give), "Flag");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl TransferResource()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "TransferResource";

            l.AddLuaControl(new LuaValueEnumControl(LuaEnumGoodType.GoodFood), "Resource").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumSideType.SideAll), "Side").Important = true;
            l.AddLuaControl(new LuaValueIntControl(10), "Amount");
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumTransferType.Give), "Flag");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl StopEffect()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "StopEffect";

            l.AddLuaControl(new LuaValueEnumControl(LuaEnumTargetType.Figure), "TargetType");
            l.AddLuaControl(new LuaValueIntControl(0), "X");
            l.AddLuaControl(new LuaValueIntControl(0), "Y");
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl SetEffect()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "SetEffect";

            l.AddLuaControl(new LuaValueEnumControl(LuaEnumTargetType.Figure), "TargetType");
            l.AddLuaControl(new LuaValueIntControl(0), "X");
            l.AddLuaControl(new LuaValueIntControl(0), "Y");
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueStringControl("NoEffect"), "Effect").Important = true;
            l.AddLuaControl(new LuaValueDoubleControl(0), "Length");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl CastSpell()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "CastSpell";

            l.AddLuaControl(new LuaValueIntControl(-1), "Target").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumTargetType.Figure), "TargetType").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueGDControl(0, 0), "Spell").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl LookAtFigure()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "LookAtFigure";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "Target").Important = true;
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl LookAtDirection()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "LookAtDirection";

            l.AddLuaControl(new LuaValueEnumControl(LuaEnumDirection.East), "Direction").Important = true;
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ChangeRace()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "ChangeRace";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueGDControl(15, 0), "RaceId").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ChangeUnit()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "ChangeUnit";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueGDControl(17, 0), "Unit").Important = true;
            l.AddLuaControl(new LuaValueBoolControl(false), "ChangeLevel");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ChangeEquipment()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "ChangeRace";

            l.AddLuaControl(new LuaValueEnumControl(LuaEnumSlotType.SlotHead), "Slot").Important = true;
            l.AddLuaControl(new LuaValueGDControl(6, 0), "Item").Important = true;
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ChangeBuildingOwwner()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "ChangeBuildingOwner";

            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ChangeFigureOwner()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "ChangeFigureOwner";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ChangeObject()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "ChangeRace";

            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;
            l.AddLuaControl(new LuaValueGDControl(33, 0), "Object").Important = true;

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl Vanish()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "Vanish";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl Stop()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "Stop";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl StopFollow()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "StopFollow";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "Target").Important = true;
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl Follow()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "Follow";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "Target").Important = true;
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl Spawn()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "Spawn";

            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueGDControl(36, 0), "Target");

            LuaValueComplexControl ul = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            ul.Value.Name = "UnitId";

            ul.AddLuaControl(new LuaValueIntControl(0), "");

            ul.RefreshName();
            ul.ResetSize();

            l.AddLuaControl(ul);
            l.AddLuaControl(new LuaValueIntControl(0), "XRand");
            l.AddLuaControl(new LuaValueIntControl(0), "YRand");
            l.AddLuaControl(new LuaValueGDControl(16, 0), "Clan");
            l.AddLuaControl(new LuaValueIntControl(0), "Range");
            l.AddLuaControl(new LuaValueBoolControl(false), "NotPersistant");
            l.AddLuaControl(new LuaValueStringControl(""), "Effect");
            l.AddLuaControl(new LuaValueIntControl(0), "Length");
            l.AddLuaControl(new LuaValueBoolControl(false), "HardMode");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl Goto()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsActionParam, "{}");
            l.Value.Name = "Goto";

            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;
            l.AddLuaControl(new LuaValueIntControl(5), "Range");
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId"); //pending change
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumWalkMode.Walk), "WalkMode");
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumGotoMode.GotoNormal), "GotoMode");
            l.AddLuaControl(new LuaValueIntControl(0), "XRand");
            l.AddLuaControl(new LuaValueIntControl(0), "YRand");

            l.RefreshName();
            l.ResetSize();

            return l;
        }
    }
}
