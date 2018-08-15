using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    //enmu describing type of data a variant possesses
    public enum TYPE { UByte, Byte, UShort, Short, UInt, Int, String, Unknown };

    //this class can hold any type you can read from gamedata.cff file
    public class SFVariant
    {
        public static int[] TYPE_SIZE = { 1, 1, 2, 2, 4, 4, 0, -1 };
        static Type[] types = { typeof(Byte), typeof(SByte), typeof(UInt16), typeof(Int16), typeof(UInt32), typeof(Int32), typeof(char[]) };
        public Object value;
        public TYPE vtype;

        //variant constructor
        public SFVariant()
        {
            vtype = TYPE.Unknown;
            value = null;
        }

        //sets variant to a given object and handles its type
        //couldn't find a better way to do this : /
        public void set(Object obj)
        {
            value = obj;
            Type t = obj.GetType();
            for (int i = 0; i < types.Length; i++)
            {
                if (t.Equals(types[i]))
                {
                    vtype = (TYPE)i;
                    return;
                }
            }
            vtype = TYPE.Unknown;
        }

        //returns int value of a variant (or 0 if can't represent it as an integer)
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

        //returns whether two variants are identical
        public bool same_as(SFVariant v)
        {
            if (vtype != v.vtype)
                return false;
            switch (vtype)
            {
                case TYPE.Byte:
                    return (SByte)value == (SByte)v.value;
                case TYPE.UByte:
                    return (Byte)value == (Byte)v.value;
                case TYPE.Short:
                    return (Int16)value == (Int16)v.value;
                case TYPE.UShort:
                    return (UInt16)value == (UInt16)v.value;
                case TYPE.Int:
                    return (Int32)value == (Int32)v.value;
                case TYPE.UInt:
                    return (UInt32)value == (UInt32)v.value;
                case TYPE.String:
                    return ((char[])value).SequenceEqual((char[])v.value);
                default:
                    return value.Equals(v.value);
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
            foreach (Object obj in objs)
                add_single_variant(obj);
        }

        //sets variant specified by an index
        public void set_single_variant(int index, Object obj)
        {
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

        //creates a new element with identical contents as the original
        public SFCategoryElement get_copy()
        {
            SFCategoryElement elem = new SFCategoryElement();
            foreach (SFVariant v in properties)
                elem.add_single_variant(v.value);

            return elem;
        }

        //returns whether the contents in compared elements are identical
        public bool same_as(SFCategoryElement elem)
        {
            if (properties.Count != elem.get().Count)
                return false;

            for(int i = 0; i < properties.Count; i++)
            {
                if (!properties[i].same_as(elem.get_single_variant(i)))
                    return false;
            }

            return true;
        }
    }
}
