using System.Collections.Generic;
using System.IO;

namespace SFEngine.SFLua.LuaDecompiler
{
    public class Chunk : Node
    {
        public List<IStatement> Items = new List<IStatement>();

        public override void WriteLuaString(StringWriter sw)
        {
            foreach (IStatement i in Items)
            {
                sw.Write(Utility.TabulateString("", Depth));
                i.WriteLuaString(sw);
                sw.WriteLine();
            }
        }

        public override string ToString()
        {
            return "CHUNK (" + Items.Count.ToString() + ")";
        }
    }
}
