using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2056Item : ICategoryItem
    {
        public fixed byte Data[6];

        public int GetID() => Data[0];
        public void SetID(int id) => Data[0] = (byte)id;
        public byte GetData(int index)
        {
            if((index < 0)||(index >= 6))
            {
                throw new Exception();
            }
            return Data[index];
        }
    }

    public class Category2056: CategoryBaseSingle<Category2056Item>
    {
        public override string GetName()
        {
            return "Unused";
        }

        public override short GetCategoryID()
        {
            return 2056;
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
