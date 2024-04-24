using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2024Item : ICategoryItem
    {
        public ushort UnitID;
        public ushort NameID;
        public ushort StatsID;
        public uint ExperienceGain;
        public ushort ExperienceFalloff;
        public uint CopperLoot;
        public ushort CopperVariance;
        public byte Rangedness;
        public ushort MeatValue;
        public ushort Armor;
        public fixed byte Handle[40];
        public byte CanBePlaced;

        public int GetID() => UnitID;
        public void SetID(int id) => UnitID = (ushort)id;

        public string GetHandleString()
        {
            Encoding encoding = Encoding.GetEncoding(1252);

            fixed (byte* s = Handle)
            {
                return (encoding.GetString(s, 40));
            }
        }
    }

    public class Category2024 : CategoryBaseSingle<Category2024Item>
    {
        public override string GetName()
        {
            return "Unit general data/link with unit stats";
        }

        public override short GetCategoryID()
        {
            return 2024;
        }

        public override short GetCategoryType()
        {
            return 8;
        }
    }
}
