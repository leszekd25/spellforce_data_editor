using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SpellforceDataEditor.SFLua.lua_sql
{
    public class SFLuaSQLObjectData: ILuaParsable
    {
        public string Name = "";
        public List<string> Mesh;
        public bool Shadow = false;
        public bool Billboarded = false;
        public double Scale = 1;
        public double SelectionScaling = 0;

        public void ParseLoad(LuaParser.LuaTable table)
        {
            if(table.entries.ContainsKey("name"))
                Name = (string)table["name"];

            Mesh = new List<string>();
            if (table.entries.ContainsKey("mesh"))
            {
                LuaParser.LuaTable mesh_table = (LuaParser.LuaTable)table["mesh"];
                for (int k = 1; k <= mesh_table.entries.Count; k++)
                    Mesh.Add((string)mesh_table[(double)k]);
            }

            if (table.entries.ContainsKey("shadow"))
                Shadow = ((double)table["shadow"] != 0);
            if (table.entries.ContainsKey("billboarded"))
                Billboarded = ((double)table["billboarded"] != 0);
            if (table.entries.ContainsKey("scale"))
                Scale = (double)table["scale"];
            if (table.entries.ContainsKey("selectionscaling"))
                SelectionScaling = (double)table["selectionscaling"];
        }

        public string ParseToString()
        {
            string ret = "";
            ret += "name = \"" + Name + "\",";
            ret += "\r\nmesh = \r\n{";
            foreach (string s in Mesh)
                ret += "\r\n\t\"" + s + "\",";
            ret += "\r\n},";
            ret += "\r\nshadow = " + (Shadow ? 1 : 0).ToString() + ",";
            ret += "\r\nbillboarded = " + (Billboarded ? 1 : 0).ToString() + ",";
            ret += "\r\nscale = " + Scale.ToString() + ",";
            ret += "\r\nselectionscaling = " + SelectionScaling.ToString(Utility.ci) + ",";
            return ret;
        }
    }

    public class SFLuaSQLObject: ILuaSQL
    {
        public Dictionary<int, SFLuaSQLObjectData> objects { get; private set; } = null;
        
        public SFLuaSQLObjectData this[int index]
        {
            get
            {
                if (objects.ContainsKey(index))
                    return objects[index];
                return null;
            }
            set
            {
                if (objects.ContainsKey(index))
                    objects[index] = value;
                else
                    objects.Add(index, value);
            }
        }

        public int Load()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLObject.Load() called");

            // check if file exists
            string filename = SFUnPak.SFUnPak.game_directory_name;
            if (filename == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLObject.Load(): Game directory not found!");
                return -1;
            }

            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLObject.Load(): Executing script script\\sql_object.lua");

            object[] ret = SFLuaEnvironment.ExecuteGameScript("script\\sql_object.lua");
            if (ret == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLObject.Load(): Could not execute object script!");
                return -6;
            }

            if (objects == null)
                objects = new Dictionary<int, SFLuaSQLObjectData>();

            int log_current_item = 0;
            try
            {
                objects.Clear();

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
                    SFLuaSQLObjectData data = new SFLuaSQLObjectData();
                    data.ParseLoad(i_table);
                    objects.Add(_i, data);
                }
            }
            catch (Exception)
            {
                Unload();
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLObject.Load(): Error reading object file! Item ID = " + log_current_item.ToString());
                return -3;
            }

            return 0;
        }

        public int Save()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLObject.Save(): called");

            if (objects == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLObject.Save(): Objects are not loaded in!");
                return -4;
            }

            // check if file exists
            string filename = SFUnPak.SFUnPak.game_directory_name;
            if (filename == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLObject.Save(): Game directory not found!");
                return -1;
            }
            filename += "\\script\\sql_object.lua";

            try
            {
                File.WriteAllText(filename, "return\r\n" + SFLuaEnvironment.ParseDictToString(objects));
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLObject.Save(): Error writing object data to file (filename = " + filename + ")");
                return -3;
            }

            return 0;
        }

        public void Unload()
        {
            if (objects != null)
            {
                objects.Clear();
                objects = null;
            }
        }
    }
}
