using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFLua.LuaDecompiler
{
    public class Return : Node, IStatement
    {
        public int InstructionID { get; set; }

        public List<IRValue> Items = new List<IRValue>();

        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write("return ");
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].WriteLuaString(sw);
                if (i != Items.Count - 1)
                    sw.Write(", ");
            }
        }

        public override string ToString()
        {
            return "RETURN (" + Items.Count.ToString() + ")";
        }
    }
}
