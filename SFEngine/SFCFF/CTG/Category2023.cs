using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2023Item : ICategorySubItem
    {
        public byte ClanID;
        public byte ClanID2;
        public byte Relation;

        public int GetID() => ClanID;
        public void SetID(int id) => ClanID = (byte)id;
        public int GetSubID() => ClanID2;
        public void SetSubID(int subid) => ClanID2 = (byte)subid;

    }

    public class Category2023 : CategoryBaseMultiple<Category2023Item>
    {
        public override string GetName()
        {
            return "Faction relations";
        }

        public override short GetCategoryID()
        {
            return 2023;
        }

        public override short GetCategoryType()
        {
            return 2;
        }

        public override bool GetSubitemDiffBehavior()
        {
            return true;
        }
    }
}
