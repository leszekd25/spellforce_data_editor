using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SpellforceDataEditor
{
    //helper class providing with useful functions
    public static class Utility
    {
        public const string S_NONAME = "<no name>";
        public const string S_MISSING = "<missing>";
        public const string S_UNKNOWN = "<unknown>";
        public const string S_NONE = "<none>";
        public static CultureInfo ci { get; } = CultureInfo.CreateSpecificCulture("en-GB");
        
        //functions which try to convert a string to the respective type
        static public SByte TryParseInt8(string s, SByte def = 0)
        {
            if (SByte.TryParse(s, out sbyte val))
                return val;
            else
                return def;
        }

        static public Int16 TryParseInt16(string s, Int16 def = 0)
        {
            if (Int16.TryParse(s, out short val))
                return val;
            else
                return def;
        }

        static public Int32 TryParseInt32(string s, Int32 def = 0)
        {
            if (Int32.TryParse(s, out int val))
                return val;
            else
                return def;
        }

        static public Byte TryParseUInt8(string s, Byte def = 0)
        {
            if (Byte.TryParse(s, out byte val))
                return val;
            else
                return def;
        }

        static public UInt16 TryParseUInt16(string s, UInt16 def = 0)
        {
            if (UInt16.TryParse(s, out ushort val))
                return val;
            else
                return def;
        }

        static public UInt32 TryParseUInt32(string s, UInt32 def = 0)
        {
            if (UInt32.TryParse(s, out uint val))
                return val;
            else
                return def;
        }

        static public Single TryParseFloat(string s, Single def = 0)
        {
            s = s.Replace('.', ',');
            if (Single.TryParse(s, out float val))
                return val;
            else
                return def;
        }

        static public Double TryParseDouble(string s, Double def = 0)
        {
            s = s.Replace('.', ',');
            if (Double.TryParse(s, out double val))
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
        static public Byte[] FixedLengthString(string s, int length)
        {
            byte[] charray = new byte[length];
            char[] text_array = s.ToCharArray();

            for (int i = 0; i < length; i++)
                if (i < text_array.Length)
                    charray[i] = (byte)text_array[i];
                else
                    charray[i] = 0;

            return charray;
        }

        //turns string variant into actual string (all zeros are truncated)
        static public string CleanString(object ch)
        {
            if (ch.GetType() !=  typeof(byte[]))
                return "";

            byte[] bytearray = (byte[])ch;
            return (Encoding.Default.GetString(bytearray)).Replace("\0", string.Empty);
        }

        //turns char array into actual string (all zeros are truncated)
        static public string CleanString(byte[] ch)
        {
            return (Encoding.Default.GetString(ch)).Replace("\0", string.Empty);
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

        static public string ReadSFString(BinaryReader br)
        {
            string s = "";
            int size = br.ReadInt32();
            for(int i = 0; i < size; i++)
            {
                s += (char)br.ReadByte();
                br.ReadByte();
            }
            return s;
        }

        static public void WriteSFString(BinaryWriter bw, string s)
        {
            bw.Write(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                bw.Write((char)s[i]);
                bw.Write((byte)0);
            }
        }

        static public uint CalculateAdler32Checksum(byte[] data)
        {
            uint a = 1, b = 0;
            for(int i = 0; i < data.Length; i++)
            {
                a = (a + data[i]) % 65521;
                b = (b + a) % 65521;
            }
            return (b << 16) | a;
        }

        static public string TabulateString(string s, int tabs)
        {
            string replacement = "\r\n";
            for (int i = 0; i < tabs; i++)
                replacement += "\t";
            return replacement+s.Replace("\r\n", replacement);
        }

        static public float BilinearInterpolation(float tl, float tr, float bl, float br, float t1, float t2)
        {
            return (tl * (1 - t1) * (1 - t2))
                 + (tr * t1 * (1 - t2))
                 + (bl * (1 - t1) * t2)
                 + (br * t1 * t2);
        }

        static public byte[] ToByteArray<T>(T[] arr) where T: struct        // T: struct means you can copy the  contents
        {
            GCHandle handle = GCHandle.Alloc(arr, GCHandleType.Pinned);     // handle to an unmanaged object;
                                                                            // pinned  means address wont change
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                byte[] dst = new byte[arr.Length * Marshal.SizeOf(typeof(T))];
                Marshal.Copy(ptr, dst, 0, dst.Length);                      // need to  use marshal copy  (unmanaged memory)
                return dst;
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();                                          // must free the allocated unmanaged memory asap
            }
        }

        static public T[] FromByteArray<T>(byte[]  arr) where T:  struct
        {
            T[] destination = new T[arr.Length / Marshal.SizeOf(typeof(T))];
            GCHandle handle = GCHandle.Alloc(destination, GCHandleType.Pinned);
            try
            {
                IntPtr pointer = handle.AddrOfPinnedObject();
                Marshal.Copy(arr, 0, pointer, arr.Length);
                return destination;
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }

        // returns an index of the value if it were to be inserted into the index list such that ascending order is preserved
        // returns -1 if value exists in the list
        // assumes list is sorted in ascending order
        static public int FindNewIndexOf(List<int> list, int id)
        {
            int current_start = 0;
            int current_end = list.Count - 1;
            int current_center;
            int val;
            while (current_start <= current_end)
            {

                current_center = (current_start + current_end) / 2;    //care about overflow (though its not happening in this case)
                val = list[current_center];
                if (val.CompareTo(id) == 0)
                    return -1;
                if (val.CompareTo(id) < 0)
                    current_start = current_center + 1;
                else
                    current_end = current_center - 1;
            }
            return current_start;
        }
    }
}
