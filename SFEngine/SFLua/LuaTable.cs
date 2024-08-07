using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFLua
{
    public class LuaTable: LuaRef
    {
        public LuaTable(int r, Lua lua): base(r, lua)
        {

        }

        public LuaTable(Lua lua): base(0, lua)
        {
            LuaNative.lua_newtable(lua.L);
            reference = LuaNative.lua_ref(lua.L, 1);
        }

        public object this[double f]
        {
            get
            {
                return _lua.GetTableField(this, f);
            }
            set
            {
                _lua.SetTableField(this, f, value);
            }
        }
        public object this[string f]
        {
            get
            {
                return _lua.GetTableField(this, f);
            }
            set
            {
                _lua.SetTableField(this, f, value);
            }
        }
        public object this[object f]
        {
            get
            {
                return _lua.GetTableField(this, f);
            }
            set
            {
                _lua.SetTableField(this, f, value);
            }
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return GetDict().GetEnumerator();
        }

        // slow; use only when absolutely required
        public Dictionary<object, object> GetDict()
        {
            Dictionary<object, object> t = new();

            int old_top = LuaNative.lua_gettop(_lua.L);
            _lua.PushObject(this);
            LuaNative.lua_pushnil(_lua.L);
            while(LuaNative.lua_next(_lua.L, -2) != 0)
            {
                object value = _lua.PopObject();
                object key = _lua.PopObject();
                t[key] = value;
                _lua.PushObject(key);
            }
            LuaNative.lua_settop(_lua.L, old_top);

            return t;
        }

        public bool TryGet(double key, out object o)
        {
            o = _lua.GetTableField(this, key);
            return o != null;
        }
        public bool TryGet(string key, out object o)
        {
            o = _lua.GetTableField(this, key);
            return o != null;
        }
        public bool TryGet(object key, out object o)
        {
            o = _lua.GetTableField(this, key);
            return o != null;
        }
    }
}
