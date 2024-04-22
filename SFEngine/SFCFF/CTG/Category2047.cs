using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2047Item : ICategorySubItem
    {
        public ushort MerchantID;
        public byte ItemType;
        public ushort PriceMultiplier;

        public int GetID() => MerchantID;
        public void SetID(int id) => MerchantID = (ushort)id;
        public int GetSubID() => ItemType;
        public void SetSubID(int subid) => ItemType = (byte)subid;

    }

    public class Category2047 : CategoryBaseMultiple<Category2047Item>
    {
        public override string GetName()
        {
            return "Merchant sell/buy rate";
        }

        public override short GetCategoryID()
        {
            return 2047;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
