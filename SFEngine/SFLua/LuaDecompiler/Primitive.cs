using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFLua.LuaDecompiler
{
    public class Primitive : Node, IRValue
    {

    }

    public class Num: Primitive
    {
        public double value;

        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write(value.ToString(Utility.ci));
        }

        public override string ToString()
        {
            return "NUM | "+value.ToString();
        }
    }

    public class Str: Primitive
    {
        public string value;

        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write('"'+value.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\"", "\\\"")+'"');
        }

        public override string ToString()
        {
            return "STR | " + value;
        }
    }

    public class Nil: Primitive
    {
        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write("nil");
        }

        public override string ToString()
        {
            return "NIL";
        }
    }
}
