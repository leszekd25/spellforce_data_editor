using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2064Item : ICategoryItem
    {
        public ushort WeaponMaterialID;
        public ushort NameID;

        public int GetID() => WeaponMaterialID;
        public void SetID(int id) => WeaponMaterialID = (ushort)id;
    }

    public class Category2064 : CategoryBaseSingle<Category2064Item>
    {
        public override string GetName()
        {
            return "Weapon materials";
        }

        public override short GetCategoryID()
        {
            return 2064;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
