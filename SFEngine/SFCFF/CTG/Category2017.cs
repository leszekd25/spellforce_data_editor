using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2017Item : ICategorySubItem
    {
        public ushort ItemID;
        public byte ReqIndex;
        public byte SkillMajorID;
        public byte SkillMinorID;
        public byte SkillLevel;

        public int GetID() => ItemID;
        public void SetID(int id) => ItemID = (ushort)id;
        public int GetSubID() => ReqIndex;
        public void SetSubID(int subid) => ReqIndex = (byte)subid;

    }

    public class Category2017 : CategoryBaseMultiple<Category2017Item>
    {
        public override string GetName()
        {
            return "Item skill requirements";
        }

        public override short GetCategoryID()
        {
            return 2017;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
