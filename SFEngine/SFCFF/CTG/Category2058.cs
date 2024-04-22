using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2058Item : ICategoryItem
    {
        public ushort DescriptionID;
        public ushort TextID;

        public int GetID() => DescriptionID;
        public void SetID(int id) => DescriptionID = (ushort)id;
    }

    public class Category2058 : CategoryBaseSingle<Category2058Item>
    {
        public override string GetName()
        {
            return "Description link with txt data";
        }

        public override short GetCategoryID()
        {
            return 2058;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
