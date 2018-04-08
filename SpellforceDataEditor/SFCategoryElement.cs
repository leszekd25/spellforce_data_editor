using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    public enum TYPE { UByte, Byte, UShort, Short, UInt, Int, Float, String, Unknown };

    public class SFVariant
    {
        public Object value;
        public TYPE vtype;
        public SFVariant()
        {
            vtype = TYPE.Unknown;
            return;
        }
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
            if (t.Equals(typeof(string)))
            {
                vtype = TYPE.String;
                return;
            }
            vtype = TYPE.Unknown;
        }
    }

    public abstract class SFCategoryElement
    {
        protected SFVariant[] properties;
        public SFCategoryElement()
        {
            properties = null;
        }
        public void set(Object[] objs)
        {
            properties = new SFVariant[objs.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                properties[i] = new SFVariant();
                properties[i].set(objs[i]);
            }
            return;
        }
        public SFVariant[] get()
        {
            return properties;
        }
    }
}
