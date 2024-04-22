using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2059Item : ICategoryItem
    {
        public ushort ExtDescriptionID;
        public ushort TextID;
        public ushort ExtTextID;

        public int GetID() => ExtDescriptionID;
        public void SetID(int id) => ExtDescriptionID = (ushort)id;
    }

    public class Category2059 : CategoryBaseSingle<Category2059Item>
    {
        public override string GetName()
        {
            return "Extended description data";
        }

        public override short GetCategoryID()
        {
            return 2059;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
