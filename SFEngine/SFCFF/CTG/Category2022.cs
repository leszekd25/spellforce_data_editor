using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2022Item : ICategoryItem
    {
        public byte RaceID;
        public byte VisRangeDay;
        public byte VisRangeNight;
        public byte HearRange;
        public byte AggroRangeFactor;
        public byte Moral;
        public byte Aggresiveness;
        public ushort TextID;
        public byte Flags;
        public ushort FactionID;
        public byte DmgTakenBlunt;
        public byte DmgTakenSlash;
        public ushort AIFlags;
        public byte GroupSizeMin;
        public byte GroupSizeMax;
        public byte GroupChance;
        public byte GroupFormation;
        public ushort Flee;
        public ushort RetreatOnDmg;
        public ushort RetreatFollow;
        public byte AttackSpeedFactor;

        public int GetID() => RaceID;
        public void SetID(int id) => RaceID = (byte)id;
    }

    public class Category2022 : CategoryBaseSingle<Category2022Item>
    {
        public override string GetName()
        {
            return "Race stats";
        }

        public override short GetCategoryID()
        {
            return 2022;
        }

        public override short GetCategoryType()
        {
            return 7;
        }
    }
}
