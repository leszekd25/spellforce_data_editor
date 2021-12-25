using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFLua.LuaDecompiler
{
    public class Fork : Node, IStatement
    {
        public int InstructionID { get; set; }

        public IOperatorLogic IfCondition;
        public Chunk IfChunk = new Chunk();
        public List<Chunk> ElseifChunks = new List<Chunk>();
        public List<IOperatorLogic> ElseifConditions = new List<IOperatorLogic>();
        public Chunk ElseChunk = new Chunk();

        public Fork()
        {
            IfChunk.parent = this;
            ElseChunk.parent = this;
        }

        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write("if ", Depth);
            IfCondition.WriteLuaString(sw);
            sw.WriteLine(" then");
            Depth += 1;
            IfChunk.WriteLuaString(sw);
            Depth -= 1;
            if(ElseifChunks.Count != 0)
            {
                for(int i=0; i < ElseifChunks.Count; i++)
                {
                    sw.Write(Utility.TabulateString("elseif ", Depth));
                    ElseifConditions[i].WriteLuaString(sw);
                    sw.WriteLine(" then");
                    Depth += 1;
                    ElseifChunks[i].WriteLuaString(sw);
                    Depth -= 1;
                }
            }
            if(ElseChunk.Items.Count != 0)
            {
                sw.WriteLine(Utility.TabulateString("else", Depth));
                Depth += 1;
                ElseChunk.WriteLuaString(sw);
                Depth -= 1;
            }
            sw.Write(Utility.TabulateString("end", Depth));
        }

        public override string ToString()
        {
            return "IF (" + (2 + (ElseifChunks != null ? ElseifChunks.Count : 0)).ToString() + ")";
        }
    }
}
