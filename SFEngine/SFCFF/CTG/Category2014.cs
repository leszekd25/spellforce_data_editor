using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2014Item : ICategorySubItem
    {
        public ushort ItemID;
        public byte EffectIndex;
        public ushort EffectID;

        public int GetID() => ItemID;
        public void SetID(int id) => ItemID = (ushort)id;
        public int GetSubID() => EffectIndex;
        public void SetSubID(int subid) => EffectIndex = (byte)subid;

    }

    public class Category2014 : CategoryBaseMultiple<Category2014Item>
    {
        public override string GetName()
        {
            return "Item weapon effects/inventory scroll link with spell";
        }

        public override short GetCategoryID()
        {
            return 2014;
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
