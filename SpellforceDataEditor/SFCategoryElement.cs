﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    //enmu describing type of data a variant possesses
    public enum TYPE { UByte, Byte, UShort, Short, UInt, Int, Float, String, Unknown };

    //this class can hold any type you can read from gamedata.cff file
    public class SFVariant
    {
        public Object value;
        public TYPE vtype;
        public static int[] TYPE_SIZE = { 1, 1, 2, 2, 4, 4, 4, 0, -1 };

        //variant constructor
        public SFVariant()
        {
            vtype = TYPE.Unknown;
            return;
        }

        //sets variant to a given object and handles its type
        public void set(Object obj)
        {
            value = obj;
            Type t = obj.GetType();
            if (t.Equals(typeof(Byte)))
            {
                vtype = TYPE.UByte;
                return;
            }
            if (t.Equals(typeof(SByte)))
            {
                vtype = TYPE.Byte;
                return;
            }
            if (t.Equals(typeof(Int16)))
            {
                vtype = TYPE.Short;
                return;
            }
            if (t.Equals(typeof(UInt16)))
            {
                vtype = TYPE.UShort;
                return;
            }
            if (t.Equals(typeof(Int32)))
            {
                vtype = TYPE.Int;
                return;
            }
            if (t.Equals(typeof(UInt32)))
            {
                vtype = TYPE.UInt;
                return;
            }
            if (t.Equals(typeof(float)))
            {
                vtype = TYPE.Float;
                return;
            }
            if (t.Equals(typeof(char[])))
            {
                vtype = TYPE.String;
                return;
            }
            vtype = TYPE.Unknown;
        }

        //turns variant into an int (if possible)
        public int to_int()
        {
            switch(vtype)
            {
                case TYPE.Byte:
                    return (int)(SByte)value;
                case TYPE.UByte:
                    return (int)(Byte)value;
                case TYPE.Short:
                    return (int)(Int16)value;
                case TYPE.UShort:
                    return (int)(UInt16)value;
                case TYPE.Int:
                    return (int)(Int32)value;
                case TYPE.UInt:
                    return (int)(UInt32)value;
                default:
                    return 0;
            }
        }
    }

    //category element is a single entry from a category
    //this entry can hold different types of data depending on which category it belongs to
    public class SFCategoryElement
    {
        protected List<SFVariant> properties;

        //element constructor
        public SFCategoryElement()
        {
            properties = new List<SFVariant>();
        }

        //adds a single variant (specified by an object) to the property list
        public void add_single_variant(Object obj)
        {
            SFVariant v = new SFVariant();
            v.set(obj);
            properties.Add(v);
        }

        //adds variants from a list of objects
        public void set(Object[] objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                SFVariant variant = new SFVariant();
                variant.set(objs[i]);
                properties.Add(variant);
            }
            return;
        }

        //sets variant specified by an index
        public void set_single_variant(int index, Object obj)
        {
            properties[index] = new SFVariant();
            properties[index].set(obj);
        }

        //returns property list
        public List<SFVariant> get()
        {
            return properties;
        }

        //returns a single variant specified by an index
        public SFVariant get_single_variant(int index)
        {
            if (index >= properties.Count)
                return null;
            return properties[index];
        }

        //returns size (in bytes) of an element, depending of variant types
        //unspecified behavior for TYPE.Unknown variant
        public int get_size()
        {
            int s = 0;
            foreach(SFVariant v in properties)
            {
                if (v.vtype == TYPE.String)
                {
                    s += ((char[])v.value).Length;
                }
                else
                    s += SFVariant.TYPE_SIZE[(int)v.vtype];
            }
            return s;
        }
    }
}
