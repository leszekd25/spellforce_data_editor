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

        public void ParseLoad(LuaTable table)
        {
            if (table.TryGet("meshmalecold", out object o))
            {
                MeshMaleCold = (string)o;
            }

            if (table.TryGet("meshfemalecold", out o))
            {
                MeshFemaleCold = (string)o;
            }

            if (table.TryGet("meshmalewarm", out o))
            {
                MeshMaleWarm = (string)o;
            }

            if (table.TryGet("meshfemalewarm", out o))
            {
                MeshFemaleWarm = (string)o;
            }

            if (table.TryGet("shadowrng", out o))
            {
                ShadowRNG = (double)o;
            }

            if (table.TryGet("selectionsize", out o))
            {
                SelectionSize = (double)o;
            }

            if (table.TryGet("animset", out o))
            {
                AnimSet = (string)o;
            }

            if (table.TryGet("race", out o))
            {
                Race = (int)(double)o;
            }

            if (table.TryGet("cat", out o))
            {
                Category = (int)(double)o;
            }

            if (table.TryGet("subcat", out o))
            {
                SubCategory = (int)(double)o;
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