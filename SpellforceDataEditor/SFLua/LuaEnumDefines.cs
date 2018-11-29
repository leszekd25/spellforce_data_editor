/*
 * Description of available enums
 * LuaEnumUtility helps transform enums into lua code
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SpellforceDataEditor.SFLua
{
    public enum LuaEnumQuestState { StateUnknown, StateUnsolvable, StateKnown, StateActive, StateSolved }

    public enum LuaEnumVariable { GlobalVariable = 0, NpcVariable = 0, PlayerVariable = 1 }

    public enum LuaEnumOperator { Add }

    public enum LuaEnumVariableCompare { IsEqual, IsGreater, IsGreaterOrEqual, IsLess, IsLessOrEqual, IsNotEqual }

    public enum LuaEnumWalkMode { Walk = 0, Run = 1 }

    public enum LuaEnumRTSSpawn { None = 0, Once = -1 }

    public enum LuaEnumSpawnTime { AnimalSpawnTime = 300 }

    public enum LuaEnumOther { WaitForEndOfSpeech = 1 }

    public enum LuaEnumTargetType { Figure = 1, Building = 2, Object = 3, World = 4, Area = 5 }

    public enum LuaEnumTransferType { Take, Give, Exchange }

    public enum LuaEnumSlotType { SlotHead = 0, SlotRightHand = 1, SlotChest = 2, SotLeftHand = 3, SlotRightRing = 4, SlotLegs = 5, SlotLeftRing = 6 }

    public enum LuaEnumDirection { East, SouthEast, South, SouthWest, West, NorthWest, North, NorthEast }

    public enum LuaEnumNumberConstants { NumDirection = 8 }

    public enum LuaEnumFaceMode { FaceDirection, FaceNpc }

    public enum LuaEnumSkillMode { SkillEasy = 0, SkillMedium = 1, SkillHard = 2 }

    public enum LuaEnumAiGoal { GoalDefault = 0, GoalIdle = 1, GoalNomadic = 3, GoalAggressive = 4, GoalDefensive = 5, GoalScript = 6, GoalCoopAggressive = 7, GoalCoopDefensive = 8, GoalNone = 9 }

    public enum LuaEnumGoodType { GoodBoard, GoodStone, GoodMithril, GoodFood, GoodIron, GoodManaElixir, GoodManaHerb }

    public enum LuaEnumSideType { SideLight, SideDark, SideAll}

    public enum LuaEnumGotoMode { GotoNormal, GotoForced, GotoContinuous }

    public enum LuaEnumQuestType { MainQuest, SideQuest }

    public enum LuaEnumOwnerType { OwnerNpc, OwnerPlayer, OwnerAll }

    public enum LuaEnumFigureType { FigureAvatar, FigureHero, FigureRest, FigureAll }

    public enum LuaEnumJobType { JobIdle }

    public enum LuaEnumAttributeType { Agi, Cha, Dex, Int, Sta, Str, Wis }

    public enum LuaEnumInventoryMode { InvEquipment, InvSpellBook, InvRuneBoard, InvPlans, InvQuestBook, InvMerchant, InvCharacter }

    public enum LuaEnumPointsType { PointsStats, PointsSkills, PointsBoth }

    public enum LuaEnumAlertType { AlertAttack, AlertScout, AlertNotify, AlertDtagica }

    public enum LuaEnumEquipmentMode { EquipmentAvatar, EquipmentHero, EquipmentAll }

    public enum LuaEnumColors { ColorWhite, ColorPureWhite, ColorPureBlack, ColorPureRed, ColorPureGreen, ColorPureBlue }

    public enum LuaEnumPortalType { StadtTor, PortalKeep, SteinTorGross, SteinTorKlein, ElfenTor, DunkelelfenTor, PalisadenTor, FeuerelfenTor }

    public enum LuaEnumSkillMajor
    {
        LeichteKriegskunst,
        SchwereKriegskunst,
        Fernkampf,
        WeisseMagie,
        ElementarMagie,
        MentalMagie,
        SchwarzeMagie
    }

    public enum LuaEnumSkillLeichteKriegskunst
    {
        Stichwaffen,
        KleineSchwerter,
        KleineSchlagwaffen,
        LeichteRuestungen
    }

    public enum LuaEnumSkillSchwereKriegskunst
    {
        GrosseSchwerter,
        GrosseSchlagwaffen,
        SchwereRuestungen,
        Schilde
    }

    public enum LuaEnumSkillFernkampf
    {
        Bogen,
        Armbrust
    }

    public enum LuaEnumSkillWeisseMagie
    {
        Leben,
        Natur,
        Segnung
    }

    public enum LuaEnumSkillElementarMagie
    {
        Feuer,
        Eis,
        Erde
    }

    public enum LuaEnumSkillMentalMagie
    {
        Verzauberung,
        Offensiv,
        Defensiv
    }

    public enum LuaEnumSkillSchwarzeMagie
    {
        Tod,
        Nekromantie,
        Fluch
    }

    static public class LuaEnumUtility
    {
        static public List<Type> lua_enums = new List<Type>();

        //only called once - load enums and store them in a list
        static public void LoadEnums()
        {
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsEnum && t.Name.StartsWith("LuaEnum")
                    select t;
            lua_enums = q.ToList();
        }

        //returns an enum, given its name
        /*static public Enum GetEnumFromString(string s)
        {
            for(int i = 0; i < lua_enums.Count; i++)
            {
                Type e = lua_enums[i];
                string[] ss = e.GetEnumNames();
                for(int j = 0; j < ss.Length; j++)
                    if(ss[j] == s)
                    {
                        Enum q = (Enum)e.GetEnumValues().GetValue(j);
                        return q;
                    }
            }
            return null;
        }*/
    }
}
