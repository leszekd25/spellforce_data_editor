using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    public class Utility
    {
        //functions which try to convert a string to the respective type
        static public SByte TryParseInt8(string s, SByte def = 0)
        {
            SByte val = 0;
            if (SByte.TryParse(s, out val))
                return val;
            else
                return def;
        }

        static public Int16 TryParseInt16(string s, Int16 def = 0)
        {
            Int16 val = 0;
            if (Int16.TryParse(s, out val))
                return val;
            else
                return def;
        }

        static public Int32 TryParseInt32(string s, Int32 def = 0)
        {
            Int32 val = 0;
            if (Int32.TryParse(s, out val))
                return val;
            else
                return def;
        }

        static public Byte TryParseUInt8(string s, Byte def = 0)
        {
            Byte val = 0;
            if (Byte.TryParse(s, out val))
                return val;
            else
                return def;
        }

        static public UInt16 TryParseUInt16(string s, UInt16 def = 0)
        {
            UInt16 val = 0;
            if (UInt16.TryParse(s, out val))
                return val;
            else
                return def;
        }

        static public UInt32 TryParseUInt32(string s, UInt32 def = 0)
        {
            UInt32 val = 0;
            if (UInt32.TryParse(s, out val))
                return val;
            else
                return def;
        }

        static public Single TryParseFloat(string s, Single def = 0)
        {
            Single val = 0;
            if (Single.TryParse(s, out val))
                return val;
            else
                return def;
        }

        static public Byte[] TryParseByteArray(string s, int def_length = 1)
        {
            string[] array = s.Split(' ');
            if(array.Length != def_length)
            {
                Byte[] errarray = new Byte[def_length];
                for (int j = 0; j < def_length; j++)
                    errarray[j] = 0;
                return errarray;
            }
            Byte[] bytearray = new Byte[def_length];
            int i = 0;
            foreach(string hex in array)
            {
                Console.WriteLine(hex);
                bytearray[i] = Convert.ToByte(hex, 16);
                i++;
            }
            return bytearray;
        }

        static public Char[] FixedLengthString(string s, int length)
        {
            char[] charray = new char[length];
            char[] text_array = s.ToCharArray();
            for (int i = 0; i < length; i++)
                if (i < text_array.Length)
                    charray[i] = text_array[i];
                else
                    charray[i] = '\0';
            return charray;
        }

        static public string CleanString(SFVariant ch)
        {
            if (ch.vtype != TYPE.String)
                return "";
            return (new string((char[])ch.value)).Replace("\0", string.Empty);
        }

        static public void CopyUInt32ToByteArray(UInt32 elem, ref Byte[] bytearray, int index)
        {
            bytearray[index] = (byte)(elem % 256);
            bytearray[index+1] = (byte)(elem >> 8);
            bytearray[index+2] = (byte)(elem >> 16);
            bytearray[index+3] = (byte)(elem >> 24);
        }
    }
}
