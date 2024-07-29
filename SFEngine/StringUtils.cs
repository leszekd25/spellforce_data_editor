using SFEngine.SFCFF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine
{
    // https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm
    // a helper to read lines without allocating strings
    public static class StringUtils
    {
        // Must be a ref struct as it contains a ReadOnlySpan<char>
        public ref struct LineSplitEnumerator
        {
            private ReadOnlySpan<char> _str;

            public LineSplitEnumerator(string str)
            {
                _str = str.AsSpan();
            }

            public bool NextLine(ref ReadOnlySpan<char> line)
            {
                var span = _str;
                if (span.Length == 0) // Reach the end of the string
                    return false;

                var index = span.IndexOfAny('\r', '\n');
                if (index == -1) // The string is composed of only one line
                {
                    _str = ReadOnlySpan<char>.Empty; // The remaining string is an empty string
                    line = span;
                    return true;
                }

                if (index < span.Length - 1 && span[index] == '\r')
                {
                    // Try to consume the '\n' associated to the '\r'
                    var next = span[index + 1];
                    if (next == '\n')
                    {
                        line = span.Slice(0, index);
                        _str = span.Slice(index + 2);
                        return true;
                    }
                }

                line = span.Slice(0, index);
                _str = span.Slice(index + 1);
                return true;
            }
        }

        public static int ASCIIToLower(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] >= 0x41 && data[i] < 0x5A)
                {
                    data[i] += 0x20;
                }
                if (data[i] == 0)
                {
                    return i;
                }
            }
            return Utility.NO_INDEX;
        }

        static public byte[] FromString(string s, byte lang_id, int char_count)
        {
            Encoding encoding;
            switch (lang_id)
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

            byte[] bytes = new byte[char_count];
            unsafe
            {
                fixed (char* ptr = s.AsSpan())
                {
                    fixed (byte* ptr2 = &bytes[0])
                    {
                        int enc_result = encoding.GetBytes(ptr, s.Length, ptr2, char_count);
                    }
                }
            }
            return bytes;
        }
    }
}
