using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2067Item : ICategorySubItem
    {
        public ushort UnitStatsID;
        public byte SpellIndex;
        public ushort SpellID;

        public int GetID() => UnitStatsID;
        public void SetID(int id) => UnitStatsID = (ushort)id;
        public int GetSubID() => SpellIndex;
        public void SetSubID(int subid) => SpellIndex = (byte)subid;

    }

    public class Category2067 : CategoryBaseMultiple<Category2067Item>
    {
        public override string GetName()
        {
            return "Hero spells";
        }

        public override short GetCategoryID()
        {
            return 2067;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
