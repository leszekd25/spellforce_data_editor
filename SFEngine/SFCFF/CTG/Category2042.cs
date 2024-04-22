using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2042Item : ICategorySubItem
    {
        public ushort MerchantID;
        public ushort ItemID;
        public ushort Stock;

        public int GetID() => MerchantID;
        public void SetID(int id) => MerchantID = (ushort)id;
        public int GetSubID() => ItemID;
        public void SetSubID(int subid) => ItemID = (ushort)subid;

    }

    public class Category2042 : CategoryBaseMultiple<Category2042Item>
    {
        public override string GetName()
        {
            return "Merchant inventory";
        }

        public override short GetCategoryID()
        {
            return 2042;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
