/*
 * LuaValue is the smallest piece of data used in Lua code generation
 * It consists of an object indicating its value, and a name, and it can generate code from itself
 * Additionally, special structs are provided, which serve as types for lua values
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFLua
{
    //for lack of better place, parsing flags are here
    //they're used in LuaValueComplexControl.GenerateCode()
    [Flags]
    public enum LuaParseFlag
    {
        BlockNewLines = 0x01, ParamNewLines = 0x02, Indents = 0x04, LastComma = 0x08,
        IsParameter = 0x10, IgnoreParamName = 0x20, SeparatingCommas = 0x40
    }

    public class LuaValue
    {
        public object Value { get; set; } = null;
        public string Name { get; set; } = "";
        public LuaValue(object v)
        {
            Value = v;
        }
        public string ToCodeString(char ps)
        {
            if (ps == '\0')
                return Value.ToString();
            return ps+Value.ToString()+ps;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    //additional structs
    [Serializable]
    public struct LuaValue_GameDataStruct
    {
        public byte category;
        public ushort id;

        public static bool operator ==(LuaValue_GameDataStruct l1, LuaValue_GameDataStruct l2)
        {
            return (l1.category == l2.category) && (l1.id == l2.id);
        }

        public static bool operator !=(LuaValue_GameDataStruct l1, LuaValue_GameDataStruct l2)
        {
            return !(l1 == l2);
        }

        public override string ToString()
        {
            return id.ToString();
        }
    }
}
