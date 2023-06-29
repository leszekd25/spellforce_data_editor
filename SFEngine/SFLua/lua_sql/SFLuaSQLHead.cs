using System;
using System.Collections.Generic;
using System.IO;

namespace SFEngine.SFLua.lua_sql
{
    public class SFLuaSQLHeadData : ILuaParsable
    {
        public string MeshMale = "<undefined>";
        public string MeshFemale = "<undefined>";

        public void ParseLoad(LuaParser.LuaTable table)
        {

            if (table.entries.ContainsKey("meshmale"))
            {
                MeshMale = (string)table["meshmale"];
            }

            if (table.entries.ContainsKey("meshfemale"))
            {
                MeshFemale = (string)table["meshfemale"];
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
