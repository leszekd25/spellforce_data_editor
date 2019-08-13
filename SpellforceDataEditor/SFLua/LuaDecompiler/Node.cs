using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SpellforceDataEditor.SFLua.LuaDecompiler
{
    public interface INode
    {
        void WriteLuaString(StringWriter sw);
    }

    public interface IRValue: INode
    {

    }

    public interface IStatement: INode
    {
        int InstructionID { get; set; }
    }

    public class Node: INode
    {
        public static int Depth = 0;

        public Node parent;

        public virtual void WriteLuaString(StringWriter sw)
        {

        }
    }
}
