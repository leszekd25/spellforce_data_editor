using System;
using System.Collections.Generic;
using System.IO;

namespace SFEngine.SFLua.lua_sql
{
    public class SFLuaSQLBuildingData : ILuaParsable
    {
        public List<string> Mesh;
        public double SelectionScaling;

        public void ParseLoad(LuaParser.LuaTable table)
        {
            Mesh = new List<string>();
            if (table.entries.ContainsKey("mesh"))
            {
                LuaParser.LuaTable mesh_table = (LuaParser.LuaTable)table["mesh"];
                for (int k = 1; k <= mesh_table.entries.Count; k++)
                {
                    Mesh.Add((string)mesh_table[(double)k]);
                }
            }

            if (table.entries.ContainsKey("selectionscaling"))
            {
                SelectionScaling = (double)table["selectionscaling"];
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
