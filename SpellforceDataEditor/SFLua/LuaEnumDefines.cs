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
    public enum LuaEnumQuestState { StateUnknown = 0, StateUnsolvable = 1, StateKnown = 2, StateActive = 3, StateSolved = 4 }

    public enum LuaEnumVariable { GlobalVariable = 0, NpcVariable = 0, PlayerVariable = 1 }

    public enum LuaEnumOperator { Add = 0 }

    public enum LuaEnumVariableCompare { IsEqual = 0, IsGreater = 3, IsGreaterOrEqual = 4, IsLess = 1, IsLessOrEqual = 2, IsNotEqual = 5}

    public enum LuaEnumWalkMode { Walk = 0, Run = 1 }

    public enum LuaEnumRTSSpawn { None = 0, Once = -1 }

    public enum LuaEnumSpawnTime { AnimalSpawnTime = 300 }

    public enum LuaEnumOther { WaitForEndOfSpeech = 1 }

    public enum LuaEnumTargetType { Figure = 1, Building = 2, Object = 3, World = 4, Area = 5 }

    public enum LuaEnumTransferType { None = 0, Take = 1, Give = 2, Exchange = 3 }

    public enum LuaEnumSlotType { SlotHead = 0, SlotRightHand = 1, SlotChest = 2, SotLeftHand = 3, SlotRightRing = 4, SlotLegs = 5, SlotLeftRing = 6 }

    public enum LuaEnumDirection { East = 0, SouthEast = 1, South = 2, SouthWest = 3, West = 4, NorthWest = 5, North = 6, NorthEast = 7 }

    public enum LuaEnumNumberConstants { NumDirection = 8 }

    public enum LuaEnumFaceMode { FaceDirection = 0, FaceNpc = 1 }

    public enum LuaEnumSkillMode { SkillEasy = 0, SkillMedium = 1, SkillHard = 2 }

    public enum LuaEnumAiGoal { GoalDefault = 0, GoalIdle = 1, GoalNomadic = 3, GoalAggressive = 4, GoalDefensive = 5, GoalScript = 6, GoalCoopAggressive = 7, GoalCoopDefensive = 8, GoalNone = 9 }

    public enum LuaEnumGoodType { GoodBoard = 1, GoodStone = 2, GoodMithril = 4, GoodFood = 5, GoodIron = 7, GoodManaElixir = 18, GoodManaHerb = 19 }

    public enum LuaEnumSideType { SideLight = 0, SideDark = 1, SideAll = 0 }

    public enum LuaEnumGotoMode { GotoNormal = 0, GotoForced = 1, GotoContinuous = 2 }

    public enum LuaEnumQuestType { MainQuest = 1, SideQuest = 0 }

    public enum LuaEnumOwnerType { OwnerNpc = 0, OwnerPlayer = 1, OwnerAll = 2 }

    public enum LuaEnumFigureType { FigureAvatar = 0, FigureHero = 1, FigureRest = 2, FigureAll = 3 }

    public enum LuaEnumJobType { JobIdle }

    public enum LuaEnumAttributeType { Agi = 1, Cha = 2, Dex = 3, Int = 4, Sta = 5, Str = 6, Wis = 7 }

    public enum LuaEnumInventoryMode { InvEquipment = 0, InvSpellBook = 1, InvRuneBoard = 2, InvPlans = 3, InvQuestBook = 4, InvMerchant = 5, InvCharacter = 6 }

    public enum LuaEnumPointsType { PointsStats = 0, PointsSkills = 1, PointsBoth = 2}

    public enum LuaEnumAlertType { AlertAttack = 0, AlertScout = 1, AlertNotify = 2, AlertDragica = 3 }

    public enum LuaEnumEquipmentMode { EquipmentAvatar = 0, EquipmentHero = 1, EquipmentAll = 2 }

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
        static public List<Type> lua_enums { get; private set; } = new List<Type>();

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
