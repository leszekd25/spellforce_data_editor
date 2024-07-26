using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2028Item : ICategorySubItem
    {
        public ushort ArmyUnitID;
        public byte ResourceType;
        public byte ResourceValue;

        public int GetID() => ArmyUnitID;
        public void SetID(int id) => ArmyUnitID = (ushort)id;
        public int GetSubID() => ResourceType;
        public void SetSubID(int subid) => ResourceType = (byte)subid;

    }

    public class Category2028 : CategoryBaseMultiple<Category2028Item>
    {
        public override string GetName()
        {
            return "Army unit resource requirements";
        }

        public override short GetCategoryID()
        {
            return 2028;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
