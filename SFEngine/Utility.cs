using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace SFEngine
{
    //helper class providing with useful functions
    public static class Utility
    {
        public const string S_NONAME = "<no name>";
        public const string S_MISSING = "<missing>";
        public const string S_UNKNOWN = "<unknown>";
        public const string S_NONE = "<none>";
        public const int NO_INDEX = -1;
        public static CultureInfo ci { get; } = CultureInfo.CreateSpecificCulture("en-GB");

        //functions which try to convert a string to the respective type
        static public SByte TryParseInt8(string s, SByte def = 0)
        {
            if (SByte.TryParse(s, out sbyte val))
            {
                return val;
            }
            else
            {
                return def;
            }
        }

        static public Int16 TryParseInt16(string s, Int16 def = 0)
        {
            if (Int16.TryParse(s, out short val))
            {
                return val;
            }
            else
            {
                return def;
            }
        }

        static public Int32 TryParseInt32(string s, Int32 def = 0)
        {
            if (Int32.TryParse(s, out int val))
            {
                return val;
            }
            else
            {
                return def;
            }
        }

        static public Byte TryParseUInt8(string s, Byte def = 0)
        {
            if (Byte.TryParse(s, out byte val))
            {
                return val;
            }
            else
            {
                return def;
            }
        }

        static public UInt16 TryParseUInt16(string s, UInt16 def = 0)
        {
            if (UInt16.TryParse(s, out ushort val))
            {
                return val;
            }
            else
            {
                return def;
            }
        }

        static public UInt32 TryParseUInt32(string s, UInt32 def = 0)
        {
            if (UInt32.TryParse(s, out uint val))
            {
                return val;
            }
            else
            {
                return def;
            }
        }

        static public Single TryParseFloat(string s, Single def = 0)
        {
            if (Single.TryParse(s, out float val))
            {
                return val;
            }
            else
            {
                return def;
            }
        }

        static public Double TryParseDouble(string s, Double def = 0)
        {
            if (Double.TryParse(s, out double val))
            {
                return val;
            }
            else
            {
                return def;
            }
        }

        //for when you don't know what data actually does
        //turns a string of hexadecimal values (divided by ' ') into an array of bytes
        static public Byte[] TryParseByteArray(string s, int def_length = 1)
        {
            string[] array = s.Split(' ');
            if (array.Length != def_length)
            {
                Byte[] errarray = new Byte[def_length];
                for (int j = 0; j < def_length; j++)
                {
                    errarray[j] = 0;
                }

                return errarray;
            }

            Byte[] bytearray = new Byte[def_length];

            int i = 0;
            foreach (string hex in array)
            {
                bytearray[i] = Convert.ToByte(hex, 16);
                i++;
            }

            return bytearray;
        }

        //used for header manipulation, inserts unsigned int into a given array at a given index
        //prone to endianess errors...
        static public void CopyUInt32ToByteArray(UInt32 elem, ref Byte[] bytearray, int index)
        {
            bytearray[index] = (byte)(elem % 256);
            bytearray[index + 1] = (byte)(elem >> 8);
            bytearray[index + 2] = (byte)(elem >> 16);
            bytearray[index + 3] = (byte)(elem >> 24);
        }

        //general purpose binary serach
        static public int find_binary_index<T>(IList<T> list, T value) where T : IComparable
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
                {
                    return current_center;
                }

                if (val.CompareTo(value) < 0)
                {
                    current_start = current_center + 1;
                }
                else
                {
                    current_end = current_center - 1;
                }
            }
            return NO_INDEX;
        }

        //returns whether an object derives from given class
        static public bool DerivedFrom<T>(T obj, Type type)
        {
            Type derived = typeof(T);
            while (derived != typeof(object))
            {
                if (derived == type)
                {
                    return true;
                }

                derived = derived.BaseType;
            }
            return false;
        }

        static public string ReadSFString(BinaryReader br)
        {
            string s = "";
            int size = br.ReadInt32();
            for (int i = 0; i < size; i++)
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
            for (int i = 0; i < data.Length; i++)
            {
                a = (a + data[i]) % 65521;
                b = (b + a) % 65521;
            }
            return (b << 16) | a;
        }

        static public string TabulateString(string s, int tabs)
        {
            string replacement = "";
            for (int i = 0; i < tabs; i++)
            {
                replacement += "\t";
            }

            return replacement + s;
        }

        static public string TabulateStringNewline(string s, int tabs)
        {
            string replacement = "\r\n";
            for (int i = 0; i < tabs; i++)
            {
                replacement += "\t";
            }

            return replacement + s.Replace("\r\n", replacement);
        }

        static public float BilinearInterpolation(float tl, float tr, float bl, float br, float t1, float t2)
        {
            return (tl * (1 - t1) * (1 - t2))
                 + (tr * t1 * (1 - t2))
                 + (bl * (1 - t1) * t2)
                 + (br * t1 * t2);
        }

        static public byte[] ToByteArray<T>(T[] arr) where T : struct        // T: struct means you can copy the contents
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
                {
                    handle.Free();                                          // must free the allocated unmanaged memory asap
                }
            }
        }

        static public T[] FromByteArray<T>(byte[] arr) where T : struct
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
                {
                    handle.Free();
                }
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
                {
                    return Utility.NO_INDEX;
                }

                if (val.CompareTo(id) < 0)
                {
                    current_start = current_center + 1;
                }
                else
                {
                    current_end = current_center - 1;
                }
            }
            return current_start;
        }

        // copies source file to destination, creates directories when needed
        static public int CopyFile(string src_file, string dst_file)
        {
            if (!File.Exists(src_file))
            {
                LogUtils.Log.Error(LogUtils.LogSource.Main, "Utility.CopyFile(): " + src_file + " doesn't exist!");
                return -1;
            }

            try
            {
                FileInfo fo = new FileInfo(dst_file);
                if (!fo.Directory.Exists)
                {
                    fo.Directory.Create();
                }

                if (File.Exists(dst_file))
                {
                    LogUtils.Log.Info(LogUtils.LogSource.Main, "Utility.CopyFile(): Overwriting file " + dst_file + " with " + src_file);
                }
                else
                {
                    LogUtils.Log.Info(LogUtils.LogSource.Main, "Utility.CopyFile(): Copying file " + src_file + " to " + dst_file);
                }

                File.Copy(src_file, dst_file, true);
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.Main, "Utility.CopyFile(): Can't copy file " + src_file + " to " + dst_file);

                return -2;
            }

            return 0;
        }

        // GetRelativePath("C:\\dir1", "C:\\dir1\\dir4\\file.txt") -> "\\dir4\\file.txt")
        static public bool GetRelativePath(string src_dir, string f, out string result)
        {
            result = "";

            int occ = f.IndexOf(src_dir);
            if (occ >= 0)
            {
                result = f.Substring(occ + src_dir.Length, f.Length - occ - src_dir.Length);
                return true;
            }

            return false;
        }

        static public int CopyDirectory(string src_dir, string dst_dir)
        {
            if (!Directory.Exists(src_dir))
            {
                LogUtils.Log.Error(LogUtils.LogSource.Main, "Utility.CopyDirectory: " + src_dir + " doesn't exist!");
                return -1;
            }
            LogUtils.Log.Info(LogUtils.LogSource.Main, "Utility.CopyDirectory: Copying directory " + src_dir + " to " + dst_dir);

            try
            {
                Directory.CreateDirectory(dst_dir);
                string[] files = Directory.GetFiles(src_dir);
                string f_rel;
                foreach (string f in files)
                {
                    GetRelativePath(src_dir, f, out f_rel);
                    CopyFile(f, dst_dir + f_rel);
                }

                string[] directories = Directory.GetDirectories(src_dir);
                string d_rel;
                foreach (string d in directories)
                {
                    GetRelativePath(src_dir, d, out d_rel);
                    CopyDirectory(d, dst_dir + d_rel);
                }
            }
            catch (Exception)
            {
                LogUtils.Log.Info(LogUtils.LogSource.Main, "Utility.CopyDirectory: Can't copy " + src_dir + " to " + dst_dir);
                return -2;
            }

            return 0;
        }
    }
}
