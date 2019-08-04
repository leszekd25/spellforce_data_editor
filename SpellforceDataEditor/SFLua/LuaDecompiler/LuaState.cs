using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFLua.LuaDecompiler
{
    public class LuaState
    {
        LuaStack stack = new LuaStack();

        public object[] Run(LuaBinaryScript scr)
        {
            return scr.func.Execute(stack);
        }
    }
}
