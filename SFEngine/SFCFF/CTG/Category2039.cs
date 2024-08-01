using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2039Item : ICategorySubItem
    {
        public byte SkillMajorID;
        public byte SkillMinorID;
        public ushort TextID;

        public int GetID() => SkillMajorID;
        public void SetID(int id) => SkillMajorID = (byte)id;
        public int GetSubID() => SkillMinorID;
        public void SetSubID(int subid) => SkillMinorID = (byte)subid;

    }

    public class Category2039 : CategoryBaseMultiple<Category2039Item>
    {
        public override string GetName()
        {
            return "Skills link with text data";
        }

        public override short GetCategoryID()
        {
            return 2039;
        }

        public override short GetCategoryType()
        {
            return 1;
        }

        public override bool GetSubitemDiffBehavior()
        {
            return true;
        }
    }
}
