using System;
using System.Collections.Generic;
using System.IO;

namespace SFEngine.SFLua.lua_sql
{
    public class SFLuaSQLItemData : ILuaParsable
    {
        public string MeshMaleCold = "<undefined>";
        public string MeshFemaleCold = "<undefined>";
        public string MeshMaleWarm = "<undefined>";
        public string MeshFemaleWarm = "<undefined>";
        public double ShadowRNG = 0;
        public double SelectionSize = 0;
        public string AnimSet = "";
        public int Race = 0;
        public int Category = 0;
        public int SubCategory = 0;

        public void ParseLoad(LuaParser.LuaTable table)
        {
            if (table.entries.ContainsKey("meshmalecold"))
            {
                MeshMaleCold = (string)table["meshmalecold"];
            }

            if (table.entries.ContainsKey("meshfemalecold"))
            {
                MeshFemaleCold = (string)table["meshfemalecold"];
            }

            if (table.entries.ContainsKey("meshmalewarm"))
            {
                MeshMaleWarm = (string)table["meshmalewarm"];
            }

            if (table.entries.ContainsKey("meshfemalewarm"))
            {
                MeshFemaleWarm = (string)table["meshfemalewarm"];
            }

            if (table.entries.ContainsKey("shadowrng"))
            {
                ShadowRNG = (double)table["shadowrng"];
            }

            if (table.entries.ContainsKey("selectionsize"))
            {
                SelectionSize = (double)table["selectionsize"];
            }

            if (table.entries.ContainsKey("animset"))
            {
                AnimSet = (string)table["animset"];
            }

            if (table.entries.ContainsKey("race"))
            {
                Race = (int)(double)table["race"];
            }

            if (table.entries.ContainsKey("cat"))
            {
                Category = (int)(double)table["cat"];
            }

            if (table.entries.ContainsKey("subcat"))
            {
                SubCategory = (int)(double)table["subcat"];
            }
        }

        public string ParseToString()
        {
            string ret = "";
            ret += "meshmalecold = \"" + MeshMaleCold.ToString() + "\",";
            ret += "\r\nmeshfemalecold = \"" + MeshFemaleCold.ToString() + "\",";
            ret += "\r\nmeshmalewarm = \"" + MeshMaleWarm.ToString() + "\",";
            ret += "\r\nmeshfemalewarm = \"" + MeshFemaleWarm.ToString() + "\",";
            ret += "\r\nshadowrng = " + ShadowRNG.ToString(Utility.ci) + ",";
            ret += "\r\nselectionsize = " + SelectionSize.ToString(Utility.ci) + ",";
            ret += "\r\nanimset = \"" + AnimSet.ToString() + "\",";
            ret += "\r\nrace = " + Race.ToString() + ",";
            ret += "\r\ncat = " + Category.ToString() + ",";
            ret += "\r\nsubcat = " + SubCategory.ToString() + ",";
            return ret;
        }
    }
}
