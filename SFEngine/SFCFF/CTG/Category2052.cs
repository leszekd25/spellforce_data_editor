using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2052Item : ICategoryItem
    {
        public uint MapID;
        public byte IsPersistent;
        public fixed byte Handle[64];
        public ushort NameID;

        public int GetID() => (int)MapID;
        public void SetID(int id) => MapID = (uint)id;

        public string GetHandleString()
        {
            Encoding encoding = Encoding.GetEncoding(1252);

            fixed (byte* s = Handle)
            {
                return (encoding.GetString(s, 64));
            }
        }
    }

    public class Category2052 : CategoryBaseSingle<Category2052Item>
    {
        public override string GetName()
        {
            return "Map data";
        }

        public override short GetCategoryID()
        {
            return 2052;
        }

        public override short GetCategoryType()
        {
            return 2;
        }
    }
}
