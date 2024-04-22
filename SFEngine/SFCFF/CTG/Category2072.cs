using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2072Item : ICategoryItem
    {
        public byte ItemSetID;
        public ushort DescriptionID;
        public byte ItemSetType;

        public int GetID() => ItemSetID;
        public void SetID(int id) => ItemSetID = (byte)id;
    }

    public class Category2072 : CategoryBaseSingle<Category2072Item>
    {
        public override string GetName()
        {
            return "Item sets";
        }

        public override short GetCategoryID()
        {
            return 2072;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
