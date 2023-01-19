using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFEngine.SFCFF
{
    public struct SFOutlineData
    {
        public List<short> Data;

        public override bool Equals(object obj)
        {
            if (!(obj is SFOutlineData))
            {
                return false;
            }

            if (((SFOutlineData)obj).Data.Count != Data.Count)
            {
                return false;
            }

            for (int i = 0; i < Data.Count; i++)
            {
                if (((SFOutlineData)obj).Data[i] != Data[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int h = 1300813;
            for (int i = 0; i < Data.Count; i += 2)
            {
                h = h * 1300367 + ((Data[i] << 16) * (Data[i + 1])).GetHashCode();
            }

            return h;
        }

        public SFOutlineData GetCopy()
        {
            SFOutlineData ret = new SFOutlineData() { Data = new List<short>() };
            ret.Data.AddRange(Data);

            return ret;
        }
    }

    public struct SFString
    {
        public byte LanguageID;
        public byte[] RawData;

        public override bool Equals(object obj)
        {
            if (!(obj is SFString))
            {
                return false;
            }

            if (((SFString)obj).LanguageID != LanguageID)
            {
                return false;
            }

            if (!RawData.SequenceEqual(((SFString)obj).RawData))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int h = 1300813;
            h = h * 1300367 + LanguageID.GetHashCode();
            for (int i = 0; i < RawData.Length; i++)
            {
                h = (h * 31) ^ RawData[i];
            }

            return h;
        }

        public SFString GetCopy()
        {
            SFString str = new SFString();
            str.LanguageID = LanguageID;
            str.RawData = new byte[RawData.Length];
            Array.Copy(RawData, str.RawData, RawData.Length);
            return str;
        }

        public override string ToString()
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
                    encoding = Encoding.Default;
                    break;
            }

            return (encoding.GetString(RawData)).Replace("\0", string.Empty);
        }

        static public SFString FromString(string s, byte lang_id, int char_count = 0)
        {
            SFString str = new SFString() { LanguageID = lang_id };
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
                    encoding = Encoding.Default;
                    break;
            }

            byte[] data = encoding.GetBytes(s);
            if (char_count == 0)
            {
                str.RawData = data;
            }
            else
            {
                str.RawData = new byte[char_count];
                str.RawData.Initialize();
                Array.Copy(data, str.RawData, Math.Min(char_count, data.Length));
            }
            return str;
        }
    }

    public enum SFCategoryElementStatus { UNCHANGED, MODIFIED, ADDED, REMOVED }

    //category element is a single entry from a category
    //this entry can hold different types of data depending on which category it belongs to
    //supported  types:  byte, sbyte,  ushort, short,  uint,  int, string(byte[]), SFOutlineData
    public class SFCategoryElement
    {
        public List<object> variants = new List<object>();

        public object this[int index]
        {
            get
            {
                return variants[index];
            }
            set
            {
                variants[index] = value;
            }
        }

        //adds a single variant (specified by an object) to the property list
        public void AddVariant(Object obj)
        {
            variants.Add(obj);
        }

        //adds a single variant (specified by an object) to the property list
        public void InsertVariant(Object obj, int pos)
        {
            variants.Insert(pos, obj);
        }

        //adds variants from a list of objects
        public void AddVariants(Object[] objs)
        {
            foreach (Object obj in objs)
            {
                variants.Add(obj);
            }
        }

        //returns size (in bytes) of an element, depending of variant types
        public int GetSize()
        {
            int s = 0;
            foreach (object v in variants)
            {
                Type t = v.GetType();
                if ((t == typeof(SByte)) || (t == typeof(Byte)))
                {
                    s += 1;
                }
                else if ((t == typeof(Int16)) || (t == typeof(UInt16)))
                {
                    s += 2;
                }
                else if ((t == typeof(Int32)) || (t == typeof(UInt32)))
                {
                    s += 4;
                }
                else if (t == typeof(SFString))
                {
                    s += ((SFString)v).RawData.Length;
                }
                else if (t == typeof(SFOutlineData))
                {
                    s += 1 + ((SFOutlineData)v).Data.Count * 2;
                }
            }
            return s;
        }

        //returns whether the contents in compared elements are identical
        public bool SameAs(SFCategoryElement elem)
        {
            if (variants.Count != elem.variants.Count)
            {
                return false;
            }

            for (int i = 0; i < variants.Count; i++)
            {
                if (variants[i].GetType() != elem.variants[i].GetType())
                {
                    return false;
                }
                else if (!variants[i].Equals(elem.variants[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // returns whether a given sequence of elements between two elements is identical
        public static bool Compare(SFCategoryElement e1, SFCategoryElement e2, int start1, int start2, int len)
        {
            if ((e1.variants.Count < start1) || (e1.variants.Count < start1 + len - 1) || (e2.variants.Count < start2) || (e2.variants.Count < start2 + len - 1))
            {
                return false;
            }

            for (int i = 0; i < len; i++)
            {
                if (e1.variants[i + start1].GetType() != e2.variants[i + start2].GetType())
                {
                    return false;
                }
                else if (!e1.variants[i + start1].Equals(e2.variants[i + start2]))
                {
                    return false;
                }
            }

            return true;
        }

        //returns int value of a variant (or 0 if can't represent it as an integer)
        // todo: why is this  here? move somewhere else, more fitting
        public int ToInt(int index)
        {
            Type t = variants[index].GetType();
            if (t == typeof(SByte))
            {
                return (int)(SByte)variants[index];
            }

            if (t == typeof(Byte))
            {
                return (int)(Byte)variants[index];
            }

            if (t == typeof(Int16))
            {
                return (int)(Int16)variants[index];
            }

            if (t == typeof(UInt16))
            {
                return (int)(UInt16)variants[index];
            }

            if (t == typeof(int))
            {
                return (int)variants[index];
            }

            if (t == typeof(UInt32))
            {
                return (int)(UInt32)variants[index];
            }

            LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, "SFVariant.to_int(): Type is not a number");
            return 0;
        }

        // returns a specified range of objects variants represent
        public object[] CopyRaw(int index_start, int count)
        {
            object[] res = new object[count];
            for (int i = 0; i < count; i++)
            {
                object v = variants[i + index_start];
                Type t = v.GetType();
                if (t == typeof(SFString))
                {
                    res[i] = ((SFString)v).GetCopy();
                }
                else if (t == typeof(SByte))
                {
                    res[i] = (SByte)v;
                }
                else if (t == typeof(Byte))
                {
                    res[i] = (Byte)v;
                }
                else if (t == typeof(Int16))
                {
                    res[i] = (Int16)v;
                }
                else if (t == typeof(UInt16))
                {
                    res[i] = (UInt16)v;
                }
                else if (t == typeof(Int32))
                {
                    res[i] = (Int32)v;
                }
                else if (t == typeof(UInt32))
                {
                    res[i] = (UInt32)v;
                }
                else if (t == typeof(SFOutlineData))
                {
                    res[i] = ((SFOutlineData)v).GetCopy();
                }
            }
            return res;
        }

        // removes a range of variants from this element
        public void RemoveRaw(int index_start, int count)
        {
            for (int i = 0; i < count; i++)
            {
                variants.RemoveAt(index_start);
            }
        }

        // inserts a range of objects as variants at specified position
        public void PasteRaw(object[] data, int index_start)
        {
            for (int i = 0; i < data.Length; i++)
            {
                variants.Insert(index_start + i, data[i]);
            }
        }

        // creates a new element with identical contents as the original
        public SFCategoryElement GetCopy()
        {
            SFCategoryElement elem = new SFCategoryElement();
            elem.PasteRaw(CopyRaw(0, variants.Count), 0);

            return elem;
        }

        public override int GetHashCode()
        {
            int h = 1300813;
            foreach (var e in variants)
            {
                h = h * 1300367 + e.GetHashCode();
            }

            return h;
        }
    }

    // this is used for categories of which elements are made from multiple subelements
    public class SFCategoryElementList
    {
        public List<SFCategoryElement> Elements = new List<SFCategoryElement>();
        public List<SFCategoryElementStatus> ElementStatus = new List<SFCategoryElementStatus>();

        public SFCategoryElement this[int i] { get { return Elements[i]; } set { Elements[i] = value; } }

        public SFCategoryElement GetElementByID(int id)
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i].ToInt(1) == id)
                {
                    return Elements[i];
                }
            }

            return null;
        }

        public int GetIndexByID(int id)
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i].ToInt(1) == id)
                {
                    return i;
                }
            }

            return Utility.NO_INDEX;
        }

        public SFCategoryElementList GetCopy()
        {
            SFCategoryElementList e = new SFCategoryElementList();
            foreach (var elem in Elements)
            {
                e.Elements.Add(elem.GetCopy());
            }

            return e;
        }

        public bool SameAs(SFCategoryElementList e)
        {
            if (e == null)
            {
                return false;
            }

            if (e.Elements.Count != Elements.Count)
            {
                return false;
            }

            for (int i = 0; i < Elements.Count; i++)
            {
                if (!Elements[i].SameAs(e.Elements[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public void SetStatusAll(SFCategoryElementStatus status)
        {
            ElementStatus.Clear();
            for (int i = 0; i < Elements.Count; i++)
            {
                ElementStatus.Add(status);
            }
        }

        public int GetID()
        {
            return Elements[0].ToInt(0);
        }

        public override int GetHashCode()
        {
            int h = 1300813;
            foreach (var e in Elements)
            {
                h = h * 1300367 + e.GetHashCode();
            }

            return h;
        }
    }
}
