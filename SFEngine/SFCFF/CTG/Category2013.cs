using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2013Item : ICategoryItem
    {
        public ushort ItemID;
        public ushort InstalledScrollItemID;

        public int GetID() => ItemID;
        public void SetID(int id) => ItemID = (ushort)id;
    }

    public class Category2013 : CategoryBaseSingle<Category2013Item>
    {
        public override string GetName()
        {
            return "Inventory spell scroll link with installed spell scroll";
        }

        public override short GetCategoryID()
        {
            return 2013;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
