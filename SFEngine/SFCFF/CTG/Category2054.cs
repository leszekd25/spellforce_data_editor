using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2054Item: ICategoryItem
    {
        public ushort SpellLineID;    // 0
        public ushort TextID;         // 2
        public byte Flags;            // 4
        public byte MagicType;        // 5
        public byte MinLevel;         // 6
        public byte MaxLevel;         // 7
        public byte Availability;     // 8
        public fixed byte UIHandle[64];  // 9
        public ushort DescriptionID;  // 73

        public int GetID() => SpellLineID;
        public void SetID(int id) => SpellLineID = (ushort)id;

        public string GetHandleString()
        {
            Encoding encoding = Encoding.GetEncoding(1252);

            fixed (byte* s = UIHandle)
            {
                return (encoding.GetString(s, 64));
            }
        }
    }

    public class Category2054: CategoryBaseSingle<Category2054Item>
    {
        public override string GetName()
        {
            return "Spell type data";
        }

        public override short GetCategoryID()
        {
            return 2054;
        }

        public override short GetCategoryType()
        {
            return 5;
        }
    }
}
