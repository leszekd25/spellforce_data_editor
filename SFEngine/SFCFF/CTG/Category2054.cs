using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2054Item: ICategoryItem
    {
        public ushort SpellLineID;
        public ushort TextID;
        public byte Flags;
        public byte MagicType;
        public byte MinLevel;
        public byte MaxLevel;
        public byte Availability;
        public fixed byte UIHandle[64];
        public ushort DescriptionID;

        public int GetID() => SpellLineID;
        public void SetID(int id) => SpellLineID = (ushort)id;
    }

    public class Category2054: CategoryBaseSingle<Category2054Item>
    {
        public override string GetName()
        {
            return "Spell type data";
        }

        public override short GetCategoryID()
        {
            return 2054;
        }

        public override short GetCategoryType()
        {
            return 5;
        }
    }
}
