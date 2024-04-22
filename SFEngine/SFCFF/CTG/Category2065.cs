using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2065Item : ICategorySubItem
    {
        public ushort ObjectID;
        public byte LootIndex;
        public ushort ItemID1;
        public byte ItemChance1;
        public ushort ItemID2;
        public byte ItemChance2;
        public ushort ItemID3;

        public int GetID() => ObjectID;
        public void SetID(int id) => ObjectID = (ushort)id;
        public int GetSubID() => LootIndex;
        public void SetSubID(int subid) => LootIndex = (byte)subid;

    }

    public class Category2065 : CategoryBaseMultiple<Category2065Item>
    {
        public override string GetName()
        {
            return "Environment object loot";
        }

        public override short GetCategoryID()
        {
            return 2065;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
