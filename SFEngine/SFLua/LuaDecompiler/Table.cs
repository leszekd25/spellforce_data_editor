using System.Collections.Generic;
using System.IO;

namespace SFEngine.SFLua.LuaDecompiler
{
    public class TableAssignment : Node
    {
        public Primitive index;
        public IRValue value;

        public override void WriteLuaString(StringWriter sw)
        {
            if (index is Num)
            {
                sw.Write("[");
                index.WriteLuaString(sw);
                sw.Write("] = ");
            }
            else if (index is Str)
            {
                sw.Write(((Str)index).value.ToString() + " = ");
            }
            // nil type skipped
            value.WriteLuaString(sw);
        }

        public override string ToString()
        {
            return "TABLE_ITEM";
        }
    }

    public class Table : Node, IRValue
    {
        public List<TableAssignment> Items = new List<TableAssignment>();
        public bool IsList = true;     // used in executable stuff

        public override void WriteLuaString(StringWriter sw)
        {
            sw.WriteLine();
            sw.WriteLine(Utility.TabulateString("{", Depth));


            Depth += 1;
            for (int i = 0; i < Items.Count; i++)
            {
                sw.Write(Utility.TabulateString("", Depth));
                Items[i].WriteLuaString(sw);
                if (i != Items.Count - 1)
                {
                    if ((Items[i].index is Nil) ^ (Items[i + 1].index is Nil))
                    {
                        sw.WriteLine(";");
                    }
                    else
                    {
                        sw.WriteLine(",");
                    }
                }
            }
            Depth -= 1;

            sw.WriteLine("");
            sw.Write(Utility.TabulateString("}", Depth));
        }

        public override string ToString()
        {
            return "TABLE (" + Items.Count.ToString() + ")";
        }
    }
}
