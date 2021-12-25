using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFLua.LuaDecompiler
{
    public class Executable : Node
    {
        public ILValue Name;
        public Table Arguments;

        public override void WriteLuaString(StringWriter sw)
        {
            Name.WriteLuaString(sw);
            if (Arguments.IsList)
            {
                sw.Write("(");
                for(int i = 0; i < Arguments.Items.Count; i++)
                {
                    Arguments.Items[i].WriteLuaString(sw);
                    if (i != Arguments.Items.Count - 1)
                        sw.Write(", ");
                }
                sw.Write(")");
            }
            else
            {
                Arguments.WriteLuaString(sw);
            }
        }
    }

    public class Procedure: Executable, IStatement
    {
        public int InstructionID { get; set; }

        public override string ToString()
        {
            return "PROCEDURE (" + Arguments.Items.Count.ToString() + ")";
        }
    }

    public class Function: Executable, ILValue, IRValue
    {
        public override string ToString()
        {
            return "FUNCTION (" + Arguments.Items.Count.ToString() + ")";
        }
    }

    public class Closure: Node, IRValue
    {
        public List<Identifier> Arguments = new List<Identifier>();
        public Chunk ClosureChunk;

        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write("function(");
            for (int i = 0; i < Arguments.Count; i++)
            {
                Arguments[i].WriteLuaString(sw);
                if (i != Arguments.Count - 1)
                    sw.Write(", ");
            }
            sw.WriteLine(")");
            Depth += 1;
            ClosureChunk.WriteLuaString(sw);
            Depth -= 1;
            sw.WriteLine(Utility.TabulateString("end", Depth));
        }

        public override string ToString()
        {
            return "CLOSURE (" + Arguments.Count.ToString() + ")";
        }
    }
}
