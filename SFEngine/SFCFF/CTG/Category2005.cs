using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2005Item : ICategoryItem
    {
        public ushort StatsID;
        public ushort UnitLevel;
        public byte UnitRace;
        public ushort Agility;
        public ushort Dexterity;
        public ushort Charisma;
        public ushort Intelligence;
        public ushort Stamina;
        public ushort Strength;
        public ushort Wisdom;
        public ushort RandomInit;
        public ushort ResistanceFire;
        public ushort ResistanceIce;
        public ushort ResistanceBlack;
        public ushort ResistanceMind;
        public ushort SpeedWalk;
        public ushort SpeedFight;
        public ushort SpeedCast;
        public ushort UnitSize;
        public ushort ManaUsage;
        public uint SpawnBaseTime;
        public byte UnitFlags;
        public ushort HeadID;
        public byte EquipmentMode;

        public int GetID() => StatsID;
        public void SetID(int id) => StatsID = (ushort)id;
    }

    public class Category2005 : CategoryBaseSingle<Category2005Item>
    {
        public override string GetName()
        {
            return "Unit/hero stats";
        }

        public override short GetCategoryID()
        {
            return 2005;
        }

        public override short GetCategoryType()
        {
            return 9;
        }
    }
}
