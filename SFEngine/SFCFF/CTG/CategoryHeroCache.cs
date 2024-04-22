using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CategoryHeroCacheItem : ICategoryItem
    {
        public ushort UnitStatsID;
        public ushort RuneItemID;

        public int GetID() => UnitStatsID;
        public void SetID(int id) => UnitStatsID = (ushort)id;
    }

    public class CategoryHeroCache : CategoryBaseSingle<CategoryHeroCacheItem>
    {
        public override string GetName()
        {
            return "Hero cache (private)";
        }

        public override short GetCategoryID()
        {
            return 0;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
