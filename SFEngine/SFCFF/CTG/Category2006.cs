using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2006Item : ICategorySubItem
    {
        public ushort UnitStatsID;
        public byte SkillMajorID;
        public byte SkillMinorID;
        public byte SkillLevel;

        public int GetID() => UnitStatsID;
        public void SetID(int id) => UnitStatsID = (ushort)id;
        public int GetSubID() => SkillMajorID;
        public void SetSubID(int subid) => SkillMajorID = (byte)subid;

    }

    public class Category2006: CategoryBaseMultiple<Category2006Item>
    {
        public override string GetName()
        {
            return "Hero/worker skills";
        }

        public override short GetCategoryID()
        {
            return 2006;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
