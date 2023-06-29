using System;
using System.Collections.Generic;
using System.IO;

namespace SFEngine.SFLua.lua_sql
{
    public class SFLuaSQLObjectData : ILuaParsable
    {
        public string Name = "";
        public List<string> Mesh;
        public bool Shadow = false;
        public bool Billboarded = false;
        public double Scale = 1;
        public double SelectionScaling = 0;

        public void ParseLoad(LuaParser.LuaTable table)
        {
            if (table.entries.ContainsKey("name"))
            {
                Name = (string)table["name"];
            }

            Mesh = new List<string>();
            if (table.entries.ContainsKey("mesh"))
            {
                LuaParser.LuaTable mesh_table = (LuaParser.LuaTable)table["mesh"];
                for (int k = 1; k <= mesh_table.entries.Count; k++)
                {
                    Mesh.Add((string)mesh_table[(double)k]);
                }
            }

            if (table.entries.ContainsKey("shadow"))
            {
                Shadow = ((double)table["shadow"] != 0);
            }

            if (table.entries.ContainsKey("billboarded"))
            {
                Billboarded = ((double)table["billboarded"] != 0);
            }

            if (table.entries.ContainsKey("scale"))
            {
                Scale = (double)table["scale"];
            }

            if (table.entries.ContainsKey("selectionscaling"))
            {
                SelectionScaling = (double)table["selectionscaling"];
            }
        }

        public string ParseToString()
        {
            string ret = "";
            ret += "name = \"" + Name + "\",";
            ret += "\r\nmesh = \r\n{";
            foreach (string s in Mesh)
            {
                ret += "\r\n\t\"" + s + "\",";
            }

            ret += "\r\n},";
            ret += "\r\nshadow = " + (Shadow ? 1 : 0).ToString() + ",";
            ret += "\r\nbillboarded = " + (Billboarded ? 1 : 0).ToString() + ",";
            ret += "\r\nscale = " + Scale.ToString() + ",";
            ret += "\r\nselectionscaling = " + SelectionScaling.ToString(Utility.ci) + ",";
            return ret;
        }
    }
}
