using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2015Item : ICategoryItem
    {
        public ushort ItemID;
        public ushort MinDamage;
        public ushort MaxDamage;
        public ushort MinRange;
        public ushort MaxRange;
        public ushort WeaponSpeed;
        public ushort WeaponType;
        public ushort WeaponMaterial;

        public int GetID() => ItemID;
        public void SetID(int id) => ItemID = (ushort)id;
    }

    public class Category2015 : CategoryBaseSingle<Category2015Item>
    {
        public override string GetName()
        {
            return "Item weapon data";
        }

        public override short GetCategoryID()
        {
            return 2015;
        }

        public override short GetCategoryType()
        {
            return 2;
        }
    }
}
