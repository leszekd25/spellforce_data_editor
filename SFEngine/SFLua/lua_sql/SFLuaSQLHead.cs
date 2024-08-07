using System;
using System.Collections.Generic;
using System.IO;

namespace SFEngine.SFLua.lua_sql
{
    public class SFLuaSQLHeadData : ILuaParsable
    {
        public string MeshMale = "<undefined>";
        public string MeshFemale = "<undefined>";

        public void ParseLoad(LuaTable table)
        {
            if (table.TryGet("meshmale", out object o))
            {
                MeshMale = (string)o;
            }

            if (table.TryGet("meshfemale", out object o2))
            {
                MeshFemale = (string)o2;
            }
        }

        public string ParseToString()
        {
            string ret = "";
            ret += "meshmale = \"" + MeshMale.ToString() + "\",";
            ret += "\r\nmeshfemale = \"" + MeshFemale.ToString() + "\",";
            return ret;
        }
    }
}