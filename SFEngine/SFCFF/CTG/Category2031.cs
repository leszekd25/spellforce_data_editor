using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2031Item : ICategorySubItem
    {
        public ushort BuildingID;
        public byte ResourceID;
        public ushort ResourceRequirement;

        public int GetID() => BuildingID;
        public void SetID(int id) => BuildingID = (ushort)id;
        public int GetSubID() => ResourceID;
        public void SetSubID(int subid) => ResourceID = (byte)subid;

    }

    public class Category2031 : CategoryBaseMultiple<Category2031Item>
    {
        public override string GetName()
        {
            return "Building resource requirements";
        }

        public override short GetCategoryID()
        {
            return 2031;
        }

        public override short GetCategoryType()
        {
            return 2;
        }
    }
}
