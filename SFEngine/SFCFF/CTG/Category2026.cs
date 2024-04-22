using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2026Item : ICategorySubItem
    {
        public ushort UnitID;
        public byte SpellIndex;
        public ushort SpellID;

        public int GetID() => UnitID;
        public void SetID(int id) => UnitID = (ushort)id;
        public int GetSubID() => SpellIndex;
        public void SetSubID(int subid) => SpellIndex = (byte)subid;

    }

    public class Category2026 : CategoryBaseMultiple<Category2026Item>
    {
        public override string GetName()
        {
            return "Unit spells";
        }

        public override short GetCategoryID()
        {
            return 2026;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
