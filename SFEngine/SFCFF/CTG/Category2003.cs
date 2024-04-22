using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2003Item : ICategoryItem
    {
        public ushort ItemID;
        public byte ItemType1;
        public byte ItemType2;
        public ushort NameID;
        public ushort UnitStatsID;
        public ushort ArmyUnitID;
        public ushort BuildingID;
        public byte Option;
        public uint SellValue;
        public uint BuyValue;
        public byte ItemSetID;

        public int GetID() => ItemID;
        public void SetID(int id) => ItemID = (ushort)id;
    }

    public class Category2003 : CategoryBaseSingle<Category2003Item>
    {
        public override string GetName()
        {
            return "Item general info";
        }

        public override short GetCategoryID()
        {
            return 2003;
        }

        public override short GetCategoryType()
        {
            return 4;
        }
    }
}
