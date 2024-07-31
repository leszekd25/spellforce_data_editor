using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2055Item : ICategoryItem
    {
        public byte B1;
        public byte B2;
        public byte B3;

        public int GetID() => B1;
        public void SetID(int id) => B1 = (byte)id;
    }

    public class Category2055 : CategoryBaseSingle<Category2055Item>
    {
        public override string GetName()
        {
            return "Unknown";
        }

        public override short GetCategoryID()
        {
            return 2055;
        }

        public override short GetCategoryType()
        {
            return 1;
        }

        public override List<string> GetSearchableFields()
        {
            return new() { };
        }
    }
}
