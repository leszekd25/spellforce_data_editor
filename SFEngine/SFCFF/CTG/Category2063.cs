using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2063Item : ICategoryItem
    {
        public ushort WeaponTypeID;
        public ushort NameID;
        public byte Sharpness;

        public int GetID() => WeaponTypeID;
        public void SetID(int id) => WeaponTypeID = (ushort)id;
    }

    public class Category2063 : CategoryBaseSingle<Category2063Item>
    {
        public override string GetName()
        {
            return "Weapon types";
        }

        public override short GetCategoryID()
        {
            return 2063;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
