using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFLua
{
    public interface ILuaParsable
    {
        void ParseLoad(LuaParser.LuaTable table);
        string ParseToString();
    }
}
