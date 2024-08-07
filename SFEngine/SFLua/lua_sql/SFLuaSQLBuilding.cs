using System;
using System.Collections.Generic;
using System.IO;

namespace SFEngine.SFLua.lua_sql
{
    public class SFLuaSQLBuildingData : ILuaParsable
    {
        public List<string> Mesh;
        public double SelectionScaling;

        public void ParseLoad(LuaTable table)
        {
            Mesh = new List<string>();
            LuaTable lt = table["mesh"] as LuaTable;
            if (lt != null)
            {
                foreach(KeyValuePair<object, object> kv in lt)
                {
                    Mesh.Add((string)kv.Value);
                }
            }

            if (lt.TryGet("selectionscaling", out object o))
            {
                SelectionScaling = (double)o;
            }
            else
            {
                SelectionScaling = 0;
            }
        }

        public string ParseToString()
        {
            string ret = "";
            ret += "mesh = \r\n{";
            foreach (string s in Mesh)
            {
                ret += "\r\n\t\"" + s + "\",";
            }

            ret += "\r\n},";
            ret += "\r\nselectionscaling = " + SelectionScaling.ToString(Utility.ci) + ",";
            return ret;
        }
    }
}