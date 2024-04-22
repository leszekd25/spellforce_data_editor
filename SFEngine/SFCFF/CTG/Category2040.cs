using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2040Item : ICategorySubItem
    {
        public ushort UnitID;
        public byte LootIndex;
        public ushort ItemID1;
        public byte ItemChance1;
        public ushort ItemID2;
        public byte ItemChance2;
        public ushort ItemID3;

        public int GetID() => UnitID;
        public void SetID(int id) => UnitID = (ushort)id;
        public int GetSubID() => LootIndex;
        public void SetSubID(int subid) => LootIndex = (byte)subid;

    }

    public class Category2040 : CategoryBaseMultiple<Category2040Item>
    {
        public override string GetName()
        {
            return "Corpse loot";
        }

        public override short GetCategoryID()
        {
            return 2040;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
