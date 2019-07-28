using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFLua.LuaDecompiler
{
    public class LuaState
    {
        public List<object> stack = new List<object>();
        public int stack_position = -1;

        Dictionary<object, object> globals = new Dictionary<object, object>();

        public object this[object  key]
        {
            get
            {
                if (!globals.ContainsKey(key))
                    return null;
                return globals[key];
            }
            set
            {
                if (!globals.ContainsKey(key))
                    globals.Add(key, value);
                else
                    globals[key] = value;
            }
        }

        public object[] Run(LuaBinaryScript scr)
        {
            return scr.func.Execute(this);
        }
    }
}
