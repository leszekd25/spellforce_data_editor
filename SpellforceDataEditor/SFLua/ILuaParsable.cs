using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NLua;

namespace SpellforceDataEditor.SFLua
{
    public interface ILuaParsable
    {
        void ParseLoad(LuaTable table);
        string ParseToString();
    }
}
