/*
 * Description of general type conditions
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpellforceDataEditor.SFLua.lua_controls;


namespace SpellforceDataEditor.SFLua
{
    static public class LuaCondition
    {
        static LuaParseFlag LuaParseFlagsConditionParam = LuaParseFlag.SeparatingCommas;

        static public LuaValueComplexControl PlayerUnitExists()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "PlayerUnitExists";

            l.AddLuaControl(new LuaValueGDControl(17, 0), "UnitId");
            l.AddLuaControl(new LuaValueIntControl(1), "Amount");
            l.AddLuaControl(new LuaValueBoolControl(true), "NoWorkers");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();
            return l;
        }

        static public LuaValueComplexControl AvatarSkill()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "AvatarSkill";

            l.AddLuaControl(new LuaValueEnumControl(LuaEnumSkillLeichteKriegskunst.Stichwaffen), "Skill").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Level").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl AvatarStat()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "AvatarStat";

            l.AddLuaControl(new LuaValueEnumControl(LuaEnumAttributeType.Agi), "Stat").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Value").Important = true;
            l.AddLuaControl(new LuaValueBoolControl(false), "BaseStatOnly");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl AvatarLevel()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "AvatarLevel";

            l.AddLuaControl(new LuaValueIntControl(1), "Player");
            l.AddLuaControl(new LuaValueIntControl(-1), "Level").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl UND()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "()");
            l.Value.Name = "UND";

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl NICHT_UND()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "()");
            l.Value.Name = "NICHT_UND";

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl ODER()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "()");
            l.Value.Name = "ODER";

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl NICHT_ODER()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "()");
            l.Value.Name = "NICHT_ODER";

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl FigureHasHealth()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "FigureHasHealth";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueIntControl(100), "Percent");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");


            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl FigureHasAggro()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "FigureHasAggro";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl FigureInRangeNpc()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "FigureInRangeNpc";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueGDControl(36, 0), "TargetNpcId").Important = true;
            l.AddLuaControl(new LuaValueIntControl(5), "Range");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsMonumentInUse()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "IsMonumentInUse";

            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;
            l.AddLuaControl(new LuaValueIntControl(5), "Range");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsClanSize()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "IsClanSize";

            l.AddLuaControl(new LuaValueGDControl(16, 0), "Clan").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Size").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl IsClanLevel()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "IsClanLevel";

            l.AddLuaControl(new LuaValueGDControl(16, 0), "Clan").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Level").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl PlayerHasMoney()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "PlayerHasMoney";

            l.AddLuaControl(new LuaValueIntControl(0), "Copper");
            l.AddLuaControl(new LuaValueIntControl(0), "Silver");
            l.AddLuaControl(new LuaValueIntControl(0), "Gold");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl PlayerHasGood()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "PlayerHasGood";

            l.AddLuaControl(new LuaValueEnumControl(LuaEnumGoodType.GoodFood), "Good").Important = true;
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumSideType.SideAll), "Side");
            l.AddLuaControl(new LuaValueIntControl(1), "Amount");
            l.AddLuaControl(new LuaValueIntControl(1), "Player");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl PlayerBuildingExists()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "PlayerBuildingExists";

            l.AddLuaControl(new LuaValueGDControl(23, 0), "BuildingId").Important = true;
            l.AddLuaControl(new LuaValueIntControl(1), "Player");
            l.AddLuaControl(new LuaValueIntControl(1), "Amount");
            l.AddLuaControl(new LuaValueBoolControl(true), "OnlyWhenBuiltUp");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl PlayerUnitInRange()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "PlayerUnitInRange";

            l.AddLuaControl(new LuaValueIntControl(0), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Y").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Range");
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumFigureType.FigureAll), "FigureType");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl EnumyUnitInRange()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "EnemyUnitInRange";

            l.AddLuaControl(new LuaValueIntControl(0), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Y").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Range");
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueGDControl(17, 0), "UnitId");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl BuildingInRange()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "BuildingInRange";

            l.AddLuaControl(new LuaValueIntControl(0), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Y").Important = true;
            l.AddLuaControl(new LuaValueIntControl(0), "Range");
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumOwnerType.OwnerAll), "Owner");
            l.AddLuaControl(new LuaValueGDControl(23, 0), "BuildingId");
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl FigureJob()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "FigureJob";

            l.AddLuaControl(new LuaValueStringControl("JobIdle"), "Job");
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl FigureInRange()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "FigureInRange";

            l.AddLuaControl(new LuaValueIntControl(-1), "X").Important = true;
            l.AddLuaControl(new LuaValueIntControl(-1), "Y").Important = true;
            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");
            l.AddLuaControl(new LuaValueIntControl(2), "Range");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        //might be bugged
        static public LuaValueComplexControl Negated()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "()");
            l.Value.Name = "Negated";

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl PlayerHasItemNotEquipped()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "PlayerHasItemNotEquipped";
            
            l.AddLuaControl(new LuaValueGDControl(6, 0), "ItemId").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");
            l.AddLuaControl(new LuaValueIntControl(1), "Amount");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl PlayerHasNotItem()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "PlayerHasNotItem";

            l.AddLuaControl(new LuaValueGDControl(6, 0), "ItemId").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");
            l.AddLuaControl(new LuaValueIntControl(1), "Amount");
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumEquipmentMode.EquipmentAll), "Equipment");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl PlayerHasItemEquipped()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "PlayerHasItemEquipped";

            l.AddLuaControl(new LuaValueGDControl(6, 0), "ItemId").Important = true;   //pending change
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");
            l.AddLuaControl(new LuaValueIntControl(1), "Amount");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl PlayerHasItem()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "PlayerHasItem";

            l.AddLuaControl(new LuaValueGDControl(6, 0), "ItemId").Important = true;
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");
            l.AddLuaControl(new LuaValueIntControl(1), "Amount");
            l.AddLuaControl(new LuaValueEnumControl(LuaEnumEquipmentMode.EquipmentAll), "Equipment");

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl FigureAlive()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "FigureAlive";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId").Important = true;   //pending change

            l.RefreshName();
            l.ResetSize();

            return l;
        }


        static public LuaValueComplexControl FigureDead()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "FigureDead";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId").Important = true;   //pending change

            l.RefreshName();
            l.ResetSize();

            return l;
        }


        static public LuaValueComplexControl AvatarLocalTeleport()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "AvatarLocalTeleport";
            

            l.RefreshName();
            l.ResetSize();

            return l;
        }


        static public LuaValueComplexControl IsCreoActive()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "IsCreoActive";

            l.AddLuaControl(new LuaValueGDControl(4, 0), "CreoId").Important = true;   //pending change

            l.RefreshName();
            l.ResetSize();

            return l;
        }


        static public LuaValueComplexControl FigureDying()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "FigureDying";

            l.AddLuaControl(new LuaValueGDControl(36, 0), "NpcId");   //pending change

            l.RefreshName();
            l.ResetSize();

            return l;
        }


        static public LuaValueComplexControl TimeBetween()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "TimeBetween";

            l.AddLuaControl(new LuaValueIntControl(0), "Hour").Important = true;   //pending change
            l.AddLuaControl(new LuaValueIntControl(0), "Minute");   //pending change
            l.AddLuaControl(new LuaValueIntControl(0), "ToHour").Important = true;   //pending change
            l.AddLuaControl(new LuaValueIntControl(0), "ToMinute");   //pending change
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");   //pending change

            l.RefreshName();
            l.ResetSize();

            return l;
        }

        static public LuaValueComplexControl TimeOfDay()
        {
            LuaValueComplexControl l = new LuaValueComplexControl(LuaParseFlagsConditionParam, "{}");
            l.Value.Name = "TimeOfDay";

            l.AddLuaControl(new LuaValueIntControl(0), "Hour").Important = true;   //pending change
            l.AddLuaControl(new LuaValueIntControl(0), "Minute");   //pending change
            l.AddLuaControl(new LuaValueIntControl(15), "TimeFrame");   //pending change
            l.AddLuaControl(new LuaValueIntControl(60), "UpdateInterval");   //pending change

            l.RefreshName();
            l.ResetSize();

            return l;
        }
    }
}
