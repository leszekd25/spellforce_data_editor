using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2036Item : ICategoryItem
    {
        public ushort ButtonID;
        public ushort BuildingID;
        public ushort ButtonNameID;
        public ushort ButtonDescriptionID;
        public ushort Wood;
        public ushort Stone;
        public ushort Iron;
        public ushort Lenya;
        public ushort Aria;
        public ushort Moonsilver;
        public ushort Food;
        public fixed byte Handle[64];
        public uint ResearchTime;

        public int GetID() => ButtonID;
        public void SetID(int id) => ButtonID = (ushort)id;

        public string GetHandleString()
        {
            Encoding encoding = Encoding.GetEncoding(1252);

            fixed (byte* s = Handle)
            {
                return (encoding.GetString(s, 64));
            }
        }
    }

    public class Category2036 : CategoryBaseSingle<Category2036Item>
    {
        public override string GetName()
        {
            return "Unit upgrade data";
        }

        public override short GetCategoryID()
        {
            return 2036;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
