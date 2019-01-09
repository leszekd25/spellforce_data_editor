using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SpellforceDataEditor
{
    //helper class providing with useful functions
    public class Utility
    {
        public const string S_NONAME = "<no name>";
        public const string S_MISSING = "<missing>";
        public const string S_UNKNOWN = "<unknown>";
        public const string S_NONE = "<none>";

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

        static public Double TryParseDouble(string s, Double def = 0)
        {
            Double val = 0;
            if (Double.TryParse(s, out val))
                return val;
            else
                return def;
        }

        //for when you don't know what data actually does
        //turns a string of hexadecimal values (divided by ' ') into an array of bytes
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
                bytearray[i] = Convert.ToByte(hex, 16);
                i++;
            }

            return bytearray;
        }

        //turns string into a char array of a given length
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

        //turns string variant into actual string (all zeros are truncated)
        static public string CleanString(SFCFF.SFVariant ch)
        {
            if (ch.vtype != SFCFF.SFVARIANT_TYPE.String)
                return "";

            return (new string((char[])ch.value)).Replace("\0", string.Empty);
        }

        //turns char array into actual string (all zeros are truncated)
        static public string CleanString(char[] ch)
        {
            return (new string(ch).Replace("\0", string.Empty));
        }

        //used for header manipulation, inserts unsigned int into a given array at a given index
        //prone to endianess errors...
        static public void CopyUInt32ToByteArray(UInt32 elem, ref Byte[] bytearray, int index)
        {
            bytearray[index] = (byte)(elem % 256);
            bytearray[index+1] = (byte)(elem >> 8);
            bytearray[index+2] = (byte)(elem >> 16);
            bytearray[index+3] = (byte)(elem >> 24);
        }

        //general purpose binary serach
        static public int find_binary_index<T>(IList<T> list, T value) where T: IComparable
        {
            int current_start = 0;
            int current_end = list.Count - 1;
            int current_center;
            T val;
            while (current_start <= current_end)
            {
                current_center = (current_start + current_end) / 2;    //care about overflow
                val = list[current_center];
                if (val.CompareTo(value) == 0)
                    return current_center;
                if (val.CompareTo(value) < 0)
                    current_start = current_center + 1;
                else
                    current_end = current_center - 1;
            }
            return -1;
        }

        //returns whether an object derives from given class
        static public bool DerivedFrom<T>(T obj, Type type)
        {
            Type derived = typeof(T);
            while(derived != typeof(object))
            {
                if (derived == type)
                    return true;
                derived = derived.BaseType;
            }
            return false;
        }

        static public string GetString(string  caption, string label)
        {
            special_forms.utility_forms.GetStringForm form = new special_forms.utility_forms.GetStringForm();
            form.SetDescription(caption, label);
            form.ShowDialog();
            if (form.Result == DialogResult.Cancel)
                return null;
            return form.ResultString;
        }
    }
}
