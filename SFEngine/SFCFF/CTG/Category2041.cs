using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2041Item : ICategoryItem
    {
        public ushort MerchantID;
        public ushort UnitID;

        public int GetID() => MerchantID;
        public void SetID(int id) => MerchantID = (ushort)id;
    }

    public class Category2041 : CategoryBaseSingle<Category2041Item>
    {
        public override string GetName()
        {
            return "Merchants link with unit general data";
        }

        public override short GetCategoryID()
        {
            return 2041;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
