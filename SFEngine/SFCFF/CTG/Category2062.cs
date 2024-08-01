using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2062Item : ICategorySubItem
    {
        public byte SkillMajorID;
        public byte SkillLevel;
        public byte Strength;
        public byte Stamina;
        public byte Agility;
        public byte Dexterity;
        public byte Charisma;
        public byte Intelligence;
        public byte Wisdom;

        public int GetID() => SkillMajorID;
        public void SetID(int id) => SkillMajorID = (byte)id;
        public int GetSubID() => SkillLevel;
        public void SetSubID(int subid) => SkillLevel = (byte)subid;

    }

    public class Category2062 : CategoryBaseMultiple<Category2062Item>
    {
        public override string GetName()
        {
            return "Skill point requirements";
        }

        public override short GetCategoryID()
        {
            return 2062;
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
