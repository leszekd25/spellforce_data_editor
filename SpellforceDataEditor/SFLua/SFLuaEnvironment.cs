using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NLua;

namespace SpellforceDataEditor.SFLua
{
    public static class SFLuaEnvironment
    {
        public static Lua state { get; private set; } = null;
        public static lua_sql.SFLuaSQLRtsCoopSpawn coop_spawns { get; private set; } = new lua_sql.SFLuaSQLRtsCoopSpawn();
        
        public static void Init()
        {
            if (state != null)
                return;
            state = new Lua();

            InitializeConstants();
        }

        private static void InitializeConstants()
        {
            if (LuaEnumUtility.lua_enums.Count == 0)
                LuaEnumUtility.LoadEnums();

            string script = "";

            foreach (Type t in LuaEnumUtility.lua_enums)
            {
                string[] enum_names = t.GetEnumNames();
                Array enum_values = t.GetEnumValues();
                for (int i = 0; i < enum_names.Length; i++)
                    script += enum_names[i] + " = " + ((int)enum_values.GetValue(i)).ToString() + "\r\n";
            }

            state.DoString(script);
        }

        public static string ParseDictToString<T>(Dictionary<int, T> dict) where T: ILuaParsable
        {
            string ret = "";
            ret += "{";

            List<int> keys = new List<int>(dict.Keys);
            keys.Sort();
            foreach (int key in keys)
            {
                ret += "\r\n\t[" + key.ToString() + "] = \r\n\t{";
                ret += Utility.TabulateString(dict[key].ParseToString(), 2);
                ret += "\r\n\t},";
            }

            ret += "\r\n}";
            return ret;
        }
    }
}
