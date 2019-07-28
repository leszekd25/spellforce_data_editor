using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SpellforceDataEditor.SFLua.lua_sql
{
    public class SFLuaSQLHeadData: ILuaParsable
    {
        public string MeshMale;
        public string MeshFemale;

        public void ParseLoad(LuaParser.LuaTable table)
        {
            MeshMale = (string)table["meshmale"];
            MeshFemale = (string)table["meshfemale"];
        }

        public string ParseToString()
        {
            string ret = "";
            ret += "meshmale = \"" + MeshMale.ToString() + "\",";
            ret += "\r\nmeshfemale = \"" + MeshFemale.ToString() + "\",";
            return ret;
        }
    }

    public class SFLuaSQLHead: ILuaSQL
    {
        public Dictionary<int, SFLuaSQLHeadData> heads { get; private set; } = null;

        public SFLuaSQLHeadData this[int index]
        {
            get
            {
                if (heads.ContainsKey(index))
                    return heads[index];
                return null;
            }
            set
            {
                if (heads.ContainsKey(index))
                    heads[index] = value;
                else
                    heads.Add(index, value);
            }
        }

        public int Load()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLHead.Load() called");

            // check if file exists
            string filename = SFUnPak.SFUnPak.game_directory_name;
            if (filename == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLHead.Load(): Game directory not found!");
                return -1;
            }

            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLHead.Load(): Executing script script\\sql_head.lua");

            object[] ret = SFLuaEnvironment.ExecuteGameScript("script\\sql_head.lua");
            if (ret == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLHead.Load(): Could not execute head script!");
                return -6;
            }

            if (heads == null)
                heads = new Dictionary<int, SFLuaSQLHeadData>();

            int log_current_item = 0;
            try
            {
                heads.Clear();

                LuaParser.LuaTable table = (LuaParser.LuaTable)ret[0];
                List<double> indices = new List<double>();
                foreach (var key in table.entries.Keys)
                    indices.Add((double)key);

                indices.Sort();
                // iterate over the rts coop spawn table
                foreach (double i in indices)
                {
                    int _i = (int)i;
                    log_current_item = _i;
                    LuaParser.LuaTable i_table = (LuaParser.LuaTable)table[i];
                    SFLuaSQLHeadData data = new SFLuaSQLHeadData();
                    data.ParseLoad(i_table);
                    heads.Add(_i, data);
                }
            }
            catch (Exception e)
            {
                Unload();
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLHead.Load(): Error reading item file! Item ID = " + log_current_item.ToString());
                return -3;
            }

            return 0;
        }

        public int Save()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLHead.Save(): called");

            if (heads == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLHead.Save(): Items are not loaded in!");
                return -4;
            }

            // check if file exists
            string filename = SFUnPak.SFUnPak.game_directory_name;
            if (filename == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLHead.Save(): Game directory not found!");
                return -1;
            }
            filename += "\\script\\sql_head.lua";

            try
            {
                File.WriteAllText(filename, "return\r\n" + SFLuaEnvironment.ParseDictToString(heads));
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLHead.Save(): Error writing head data to file (filename = " + filename + ")");
                return -3;
            }

            return 0;
        }

        public void Unload()
        {
            if (heads != null)
            {
                heads.Clear();
                heads = null;
            }
        }
    }
}
