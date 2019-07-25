using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SpellforceDataEditor.SFCFF
{
    //category element is a single entry from a category
    //this entry can hold different types of data depending on which category it belongs to
    // supported  types:  byte, sbyte,  ushort, short,  uint,  int, string(byte[])
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
                variants.Add(obj);
        }

        //returns size (in bytes) of an element, depending of variant types
        public int GetSize()
        {
            int s = 0;
            foreach(object v in variants)
            {
                Type t = v.GetType();
                if ((t == typeof(SByte)) || (t == typeof(Byte)))
                    s += 1;
                else if ((t == typeof(Int16)) || (t == typeof(UInt16)))
                    s += 2;
                else if ((t == typeof(Int32)) || (t == typeof(UInt32)))
                    s += 4;
                else if (t == typeof(byte[]))
                    s += ((byte[])v).Length;
            }
            return s;
        }

        //creates a new element with identical contents as the original
        public SFCategoryElement GetCopy()
        {
            SFCategoryElement elem = new SFCategoryElement();
            foreach (object v in variants)
            {
                Type t = v.GetType();
                if (t == typeof(byte[]))
                {
                    byte[] barr = new byte[((byte[])v).Length];
                    Array.Copy(((byte[])v), barr, barr.Length);
                    elem.AddVariant(barr);
                }
                else if (t == typeof(SByte))
                    elem.AddVariant((SByte)v);
                else if (t == typeof(Byte))
                    elem.AddVariant((Byte)v);
                else if (t == typeof(Int16))
                    elem.AddVariant((Int16)v);
                else if (t == typeof(UInt16))
                    elem.AddVariant((UInt16)v);
                else if (t == typeof(Int32))
                    elem.AddVariant((Int32)v);
                else if (t == typeof(UInt32))
                    elem.AddVariant((UInt32)v);
            }

            return elem;
        }

        //returns whether the contents in compared elements are identical
        public bool SameAs(SFCategoryElement elem)
        {
            if (variants.Count != elem.variants.Count)
                return false;

            for(int i = 0; i < variants.Count; i++)
            {
                if (variants[i].GetType() != elem.variants[i].GetType())
                    return false;
                if(variants[i].GetType()==typeof(byte[]))
                {
                    if (!((byte[])variants[i]).SequenceEqual((byte[])elem.variants[i]))
                        return false;
                }
                else if (!variants[i].Equals(elem.variants[i]))
                    return false;
            }

            return true;
        }

        //returns int value of a variant (or 0 if can't represent it as an integer)
        // todo: why is this  here? move somewhere else, more fitting
        public int ToInt(int index)
        {
            Type t = variants[index].GetType();
            if (t == typeof(SByte))
                return (int)(SByte)variants[index];
            if (t == typeof(Byte))
                return (int)(Byte)variants[index];
            if (t == typeof(Int16))
                return (int)(Int16)variants[index];
            if (t == typeof(UInt16))
                return (int)(UInt16)variants[index];
            if (t == typeof(int))
                return (int)variants[index];
            if (t == typeof(UInt32))
                return (int)(UInt32)variants[index];
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
                if (t == typeof(byte[]))
                {
                    byte[] barr = new byte[((byte[])v).Length];
                    Array.Copy(((byte[])v), barr, barr.Length);
                    res[i] = barr;
                }
                else if (t == typeof(SByte))
                    res[i] = (SByte)v;
                else if (t == typeof(Byte))
                    res[i] = (Byte)v;
                else if (t == typeof(Int16))
                    res[i] = (Int16)v;
                else if (t == typeof(UInt16))
                    res[i] = (UInt16)v;
                else if (t == typeof(Int32))
                    res[i] = (Int32)v;
                else if (t == typeof(UInt32))
                    res[i] = (UInt32)v;
            }
            return res;
        }

        // removes a range of variants from this element
        public void RemoveRaw(int index_start, int count)
        {
            for (int i = 0; i < count; i++)
                variants.RemoveAt(index_start);
        }

        // inserts a range of objects as variants at specified position
        public void PasteRaw(object[] data, int index_start)
        {
            for (int i = 0; i < data.Length; i++)
                variants.Insert(index_start + i, data[i]);
        }
    }
}
