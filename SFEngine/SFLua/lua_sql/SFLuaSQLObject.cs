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

        public void ParseLoad(LuaTable table)
        {
            if (table.TryGet("name", out object o))
            {
                Name = (string)table["name"];
            }

            Mesh = new List<string>();
            if (table.TryGet("mesh", out o))
            {
                LuaTable mesh_table = (LuaTable)o;
                foreach(KeyValuePair<object, object> kv in mesh_table)
                {
                    Mesh.Add((string)kv.Value);
                }
            }

            if (table.TryGet("shadow", out o))
            {
                Shadow = ((double)table["shadow"] != 0);
            }

            if (table.TryGet("billboarded", out o))
            {
                Billboarded = ((double)table["billboarded"] != 0);
            }

            if (table.TryGet("scale", out o))
            {
                Scale = (double)table["scale"];
            }

            if (table.TryGet("selectionscaling", out o))
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