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
    public unsafe struct Category2012Item : ICategorySubItem
    {
        public ushort ItemID;
        public byte UIIndex;
        public fixed byte UIHandle[64];
        public ushort IsScaledDown;

        public int GetID() => ItemID;
        public void SetID(int id) => ItemID = (ushort)id;
        public int GetSubID() => UIIndex;
        public void SetSubID(int subid) => UIIndex = (byte)subid;

        public string GetHandleString()
        {
            Encoding encoding = Encoding.GetEncoding(1252);

            fixed (byte* s = UIHandle)
            {
                return (encoding.GetString(s, 64));
            }
        }
    }

    public class Category2012 : CategoryBaseMultiple<Category2012Item>
    {
        public override string GetName()
        {
            return "Item UI data";
        }

        public override short GetCategoryID()
        {
            return 2012;
        }

        public override short GetCategoryType()
        {
            return 1;
        }

        public override bool GetSubitemDiffBehavior()
        {
            return true;
        }
    }
}
