using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2050Item : ICategoryItem
    {
        public ushort ObjectID;
        public ushort NameID;
        public byte Flags;
        public byte FlattenMode;
        public byte PolygonNum;
        public fixed byte Handle[41];
        public ushort ResourceAmount;
        public ushort Width;
        public ushort Height;

        public int GetID() => ObjectID;
        public void SetID(int id) => ObjectID = (ushort)id;
        
        public string GetHandleString()
        {
            Encoding encoding = Encoding.GetEncoding(1252);

            fixed (byte* s = Handle)
            {
                return (encoding.GetString(s, 41));
            }
        }
    }

    public class Category2050 : CategoryBaseSingle<Category2050Item>
    {
        public override string GetName()
        {
            return "Environment objects data";
        }

        public override short GetCategoryID()
        {
            return 2050;
        }

        public override short GetCategoryType()
        {
            return 6;
        }
    }
}
