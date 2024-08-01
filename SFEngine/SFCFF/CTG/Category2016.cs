using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2016Item : ICategorySubItem
    {
        public ushort TextID;
        public byte LanguageID;
        public byte Mode;
        public fixed byte Handle[50];
        public fixed byte Content[512];

        public int GetID() => TextID;
        public void SetID(int id) => TextID = (ushort)id;
        public int GetSubID() => LanguageID;
        public void SetSubID(int subid) => LanguageID = (byte)subid;

        public string GetContentString()
        {
            Encoding encoding;
            switch (LanguageID)
            {
                case 5:
                    encoding = Encoding.GetEncoding(1251);
                    break;
                case 6:
                    encoding = Encoding.GetEncoding(1250);
                    break;
                default:
                    encoding = Encoding.GetEncoding(1252);
                    break;
            }

            fixed (byte* s = Content)
            {
                return (encoding.GetString(s, 512));
            }
        }

        public string GetHandleString()
        {
            Encoding encoding = Encoding.GetEncoding(1252);

            fixed (byte* s = Handle)
            {
                return (encoding.GetString(s, 50));
            }
        }
    }

    public class Category2016 : CategoryBaseMultiple<Category2016Item>
    {
        public override string GetName()
        {
            return "Text data";
        }

        public override short GetCategoryID()
        {
            return 2016;
        }

        public override short GetCategoryType()
        {
            return 3;
        }

        public override bool GetSubitemDiffBehavior()
        {
            return true;
        }
    }
}
