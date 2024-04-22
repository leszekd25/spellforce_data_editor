using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2044Item : ICategoryItem
    {
        public byte ResourceID;
        public ushort TextID;

        public int GetID() => ResourceID;
        public void SetID(int id) => ResourceID = (byte)id;
    }

    public class Category2044 : CategoryBaseSingle<Category2044Item>
    {
        public override string GetName()
        {
            return "Resources link with data";
        }

        public override short GetCategoryID()
        {
            return 2044;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
