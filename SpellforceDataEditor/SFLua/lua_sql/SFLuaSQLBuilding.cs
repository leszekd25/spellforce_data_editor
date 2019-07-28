using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SpellforceDataEditor.SFLua.lua_sql
{
    public class SFLuaSQLBuildingData: ILuaParsable
    {
        public List<string> Mesh;
        public double SelectionScaling;

        public void ParseLoad(LuaParser.LuaTable table)
        {
            Mesh = new List<string>();
            LuaParser.LuaTable mesh_table = (LuaParser.LuaTable)table["mesh"];
            for (int k = 1; k <= mesh_table.entries.Count; k++)
                Mesh.Add((string)mesh_table[(double)k]);

            SelectionScaling = (double)table["selectionscaling"];
        }

        public string ParseToString()
        {
            string ret = "";
            ret += "mesh = \r\n{";
            foreach (string s in Mesh)
                ret += "\r\n\t\"" + s + "\",";
            ret += "\r\n},";
            ret += "\r\nselectionscaling = " + SelectionScaling.ToString(Utility.ci) + ",";
            return ret;
        }
    }

    public class SFLuaSQLBuilding: ILuaSQL
    {
        public Dictionary<int, SFLuaSQLBuildingData> buildings { get; private set; } = null;

        public SFLuaSQLBuildingData this[int index]
        {
            get
            {
                if (buildings.ContainsKey(index))
                    return buildings[index];
                return null;
            }
            set
            {
                if (buildings.ContainsKey(index))
                    buildings[index] = value;
                else
                    buildings.Add(index, value);
            }
        }

        public int Load()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLBuilding.Load() called");

            // check if file exists
            string filename = SFUnPak.SFUnPak.game_directory_name;
            if (filename == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLBuilding.Load(): Game directory not found!");
                return -1;
            }

            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLBuilding.Load(): Executing script script\\sql_building.lua");

            object[] ret = SFLuaEnvironment.ExecuteGameScript("script\\sql_building.lua");
            if (ret == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLBuilding.Load(): Could not execute building script!");
                return -6;
            }

            if (buildings == null)
                buildings = new Dictionary<int, SFLuaSQLBuildingData>();

            int log_current_item = 0;
            try
            {
                buildings.Clear();

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
                    SFLuaSQLBuildingData data = new SFLuaSQLBuildingData();
                    data.ParseLoad(i_table);
                    buildings.Add(_i, data);
                }
            }
            catch (Exception)
            {
                Unload();
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLBuilding.Load(): Error reading object file! Item ID = " + log_current_item.ToString());
                return -3;
            }

            return 0;
        }

        public int Save()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLBuilding.Save(): called");

            if (buildings == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLBuilding.Save(): Buildings are not loaded in!");
                return -4;
            }

            // check if file exists
            string filename = SFUnPak.SFUnPak.game_directory_name;
            if (filename == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLBuilding.Save(): Game directory not found!");
                return -1;
            }
            filename += "\\script\\sql_building.lua";

            try
            {
                File.WriteAllText(filename, "return\r\n" + SFLuaEnvironment.ParseDictToString(buildings));
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLBuilding.Save(): Error writing  building data to file (filename = " + filename + ")");
                return -3;
            }

            return 0;
        }

        public void Unload()
        {
            if (buildings != null)
            {
                buildings.Clear();
                buildings = null;
            }
        }
    }
}
