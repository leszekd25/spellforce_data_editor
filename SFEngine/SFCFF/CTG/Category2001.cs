using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2001Item : ICategorySubItem
    {
        public ushort ArmyUnitID;
        public byte BuildingIndex;
        public ushort BuildingID;

        public int GetID() => ArmyUnitID;
        public void SetID(int id) => ArmyUnitID = (ushort)id;
        public int GetSubID() => BuildingIndex;
        public void SetSubID(int subid) => BuildingIndex = (byte)subid;

    }

    public class Category2001 : CategoryBaseMultiple<Category2001Item>
    {
        public override string GetName()
        {
            return "Army unit building requirements";
        }

        public override short GetCategoryID()
        {
            return 2001;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
