using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFLua.LuaDecompiler
{
    public class Assignment: Node, IStatement
    {
        public ILValue Left;
        public IRValue Right;
        public int InstructionID { get; set; }

        public override void WriteLuaString(StringWriter sw)
        {
            Left.WriteLuaString(sw);
            if (Right != null) 
            {
                sw.Write(" = ");
                Right.WriteLuaString(sw);
            }
        }
    }

    public class MultiAssignment: Node, IStatement
    {
        public List<ILValue> Left = new List<ILValue>();
        public List<IRValue> Right = new List<IRValue>();
        public int InstructionID { get; set; }

        public override void WriteLuaString(StringWriter sw)
        {
            for(int i = 0; i < Left.Count; i++)
            {
                Left[i].WriteLuaString(sw);
                if (i != Left.Count - 1)
                    sw.Write(", ");
            }
            sw.Write(" = ");
            for (int i = 0; i < Right.Count; i++)
            {
                Right[i].WriteLuaString(sw);
                if (i != Right.Count - 1)
                    sw.Write(", ");
            }
        }
    }
}
