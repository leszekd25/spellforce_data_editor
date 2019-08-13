using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFLua.LuaDecompiler
{
    public class Loop : Node, IStatement
    {
        public int InstructionID { get; set; }

        public Chunk LoopChunk = new Chunk();

        public Loop()
        {
            LoopChunk.parent = this;
        }
    }

    public class For: Loop
    {
        public Identifier name;
        public IRValue from;
        public IRValue to;
        public IRValue step;

        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write("for ", Depth);
            name.WriteLuaString(sw);
            sw.Write(" = ");
            from.WriteLuaString(sw);
            sw.Write(", ");
            to.WriteLuaString(sw);
            if((!(step is Num))||((Num)step).value != 1)
            {
                sw.Write(", ");
                step.WriteLuaString(sw);
            }
            sw.WriteLine(" do");
            Depth += 1;
            LoopChunk.WriteLuaString(sw);
            Depth -= 1;
            sw.WriteLine(Utility.TabulateString("end", Depth));
        }

        public override string ToString()
        {
            return "FOR";
        }
    }

    public class Foreach: Loop
    {
        public Identifier index;
        public Identifier value;
        public IRValue table;

        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write("for ");
            index.WriteLuaString(sw);
            sw.Write(", ");
            value.WriteLuaString(sw);
            sw.Write(" in ");
            table.WriteLuaString(sw);
            sw.WriteLine(" do");
            Depth += 1;
            LoopChunk.WriteLuaString(sw);
            Depth -= 1;
            sw.Write(Utility.TabulateString("end", Depth));
        }

        public override string ToString()
        {
            return "FOREACH";
        }
    }

    public class While: Loop
    {
        public IOperatorLogic Condition;

        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write("while ");
            Condition.WriteLuaString(sw);
            sw.WriteLine(" do");
            Depth += 1;
            LoopChunk.WriteLuaString(sw);
            Depth -= 1;
            sw.Write(Utility.TabulateString("end", Depth));
        }

        public override string ToString()
        {
            return "WHILE";
        }
    }

    public class Repeat: Loop
    {
        public IOperatorLogic Condition;

        public override void WriteLuaString(StringWriter sw)
        {
            sw.WriteLine("repeat");
            Depth += 1;
            LoopChunk.WriteLuaString(sw);
            Depth -= 1;
            sw.Write(Utility.TabulateString("until ", Depth));
            Condition.WriteLuaString(sw);
            sw.WriteLine("");
        }

        public override string ToString()
        {
            return "REPEAT";
        }
    }
    
    public class Continue : Node, IStatement
    {
        public int InstructionID { get; set; }

        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write("continue");
        }

        public override string ToString()
        {
            return "CONTINUE";
        }
    }

    public class Break : Node, IStatement
    {
        public int InstructionID { get; set; }

        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write("break");
        }

        public override string ToString()
        {
            return "BREAK";
        }
    }
}
