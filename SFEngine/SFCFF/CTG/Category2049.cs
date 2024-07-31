using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2049Item : ICategoryItem
    {
        public ushort HeadID;

        public int GetID() => HeadID;
        public void SetID(int id) => HeadID = (ushort)id;
    }

    public class Category2049 : CategoryBaseSingle<Category2049Item>
    {
        public override string GetName()
        {
            return "Heads";
        }

        public override short GetCategoryID()
        {
            return 2049;
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
