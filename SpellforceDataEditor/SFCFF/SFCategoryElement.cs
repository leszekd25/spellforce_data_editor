using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFCFF
{
    //enmu describing type of data a variant possesses
    public enum SFVARIANT_TYPE { UByte, Byte, UShort, Short, UInt, Int, String, Unknown };

    //this class can hold any type you can read from gamedata.cff file
    public class SFVariant
    {
        public static int[] TYPE_SIZE = { 1, 1, 2, 2, 4, 4, 0, -1 };
        static Type[] types = { typeof(Byte), typeof(SByte), typeof(UInt16), typeof(Int16), typeof(UInt32), typeof(Int32), typeof(byte[]) };
        public Object value;
        public SFVARIANT_TYPE vtype;

        //variant constructor
        public SFVariant()
        {
            vtype = SFVARIANT_TYPE.Unknown;
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
                    vtype = (SFVARIANT_TYPE)i;
                    return;
                }
            }
            vtype = SFVARIANT_TYPE.Unknown;
        }

        //returns int value of a variant (or 0 if can't represent it as an integer)
        public int to_int()
        {
            switch(vtype)
            {
                case SFVARIANT_TYPE.Byte:
                    return (int)(SByte)value;
                case SFVARIANT_TYPE.UByte:
                    return (int)(Byte)value;
                case SFVARIANT_TYPE.Short:
                    return (int)(Int16)value;
                case SFVARIANT_TYPE.UShort:
                    return (int)(UInt16)value;
                case SFVARIANT_TYPE.Int:
                    return (int)(Int32)value;
                case SFVARIANT_TYPE.UInt:
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
                case SFVARIANT_TYPE.Byte:
                    return (SByte)value == (SByte)v.value;
                case SFVARIANT_TYPE.UByte:
                    return (Byte)value == (Byte)v.value;
                case SFVARIANT_TYPE.Short:
                    return (Int16)value == (Int16)v.value;
                case SFVARIANT_TYPE.UShort:
                    return (UInt16)value == (UInt16)v.value;
                case SFVARIANT_TYPE.Int:
                    return (Int32)value == (Int32)v.value;
                case SFVARIANT_TYPE.UInt:
                    return (UInt32)value == (UInt32)v.value;
                case SFVARIANT_TYPE.String:
                    return ((byte[])value).SequenceEqual((byte[])v.value);
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

        //adds a single variant (specified by an object) to the property list
        public void insert_single_variant(Object obj, int pos)
        {
            SFVariant v = new SFVariant();
            v.set(obj);
            properties.Insert(pos, v);
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
                if (v.vtype == SFVARIANT_TYPE.String)
                {
                    s += ((byte[])v.value).Length;
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

        // returns a specified range of objects variants represent
        public object[] copy_raw(int index_start, int count)
        {
            object[] res = new object[count];
            for (int i = 0; i < count; i++)
                res[i] = properties[i + index_start].value;
            return res;
        }

        // removes a range of variants from this element
        public void remove_raw(int index_start, int count)
        {
            for (int i = 0; i < count; i++)
                properties.RemoveAt(index_start);
        }

        // inserts a range of objects as variants at specified position
        public void paste_raw(object[] data, int index_start)
        {
            for (int i = 0; i < data.Length; i++)
                insert_single_variant(data[i], index_start + i);
        }
    }
}
