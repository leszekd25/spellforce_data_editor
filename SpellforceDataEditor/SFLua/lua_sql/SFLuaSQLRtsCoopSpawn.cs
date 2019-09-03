using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SpellforceDataEditor.SFLua.lua_sql
{
    public enum LuaEnumAiGoal
    {
        GoalDefault = 0,
        GoalIdle = 1,
        GoalNomadic = 3,
        GoalAggressive = 4,
        GoalDefensive = 5,
        GoalScript = 6,
        GoalCoopAggressive = 7,
        GoalCoopDefensive = 8,
        GoalNone = 9
    }

    public class SFMapCoopSpawnTypeDataInfo : ILuaParsable
    {
        public int seconds_per_tick;     // convert from double
        public List<int> units;

        public void ParseLoad(LuaParser.LuaTable table)
        {
            seconds_per_tick = 0;
            if (table["Seconds"] != null)
                seconds_per_tick += (int)(double)table["Seconds"];
            if (table["Minutes"] != null)
                seconds_per_tick += (int)((double)table["Minutes"] * 60);
            if (table["Hours"] != null)
                seconds_per_tick += (int)((double)table["Hours"] * 3600);
            if (seconds_per_tick == 0)
                seconds_per_tick = 60;

            if (table["Units"] != null)
            {
                units = new List<int>();
                LuaParser.LuaTable i_spawn_data_units_table = (LuaParser.LuaTable)table["Units"];

                for (int k = 1; k <= i_spawn_data_units_table.entries.Count; k++)
                    units.Add((int)(double)i_spawn_data_units_table[(double)k]);
            }
        }

        public string ParseToString()
        {
            string ret = "";
            ret += "Seconds = " + seconds_per_tick.ToString() + ",";
            if (units != null)
            {
                ret += "\r\nUnits = \r\n{\r\n\t";
                foreach (int i in units)
                    ret += i.ToString() + ", ";
                ret += "\r\n},";
            }
            return ret;
        }
    }

    public class SFMapCoopSpawnTypeInfo : ILuaParsable
    {
        public string name;
        public string level_range;
        public LuaEnumAiGoal goal;
        public int max_units;
        public List<int> start_units;
        public Dictionary<int, SFMapCoopSpawnTypeDataInfo> data;

        public void ParseLoad(LuaParser.LuaTable table)
        {
            name = "";
            if (table["Name"] != null)
                name = (string)table["Name"];

            level_range = "";
            if (table["LevelRange"] != null)
                level_range = (string)table["LevelRange"];

            goal = LuaEnumAiGoal.GoalDefault;
            if (table["Goal"] != null)
            {
                string s = (string)table["Goal"];
                bool success = Enum.TryParse(s, out goal);
                if (!success)
                    goal = LuaEnumAiGoal.GoalDefault;
            }

            max_units = 0;
            if (table["MaxClanSize"] != null)
                max_units = (int)(double)table["MaxClanSize"];

            if (table["Init"] != null)
            {
                start_units = new List<int>();
                LuaParser.LuaTable i_init_table = (LuaParser.LuaTable)table["Init"];
                for (int j = 1; j <= i_init_table.entries.Count; j++)
                    start_units.Add((int)(double)i_init_table[(double)j]);
            }

            if (table["SpawnData"] != null)
            {
                data = new Dictionary<int, SFMapCoopSpawnTypeDataInfo>();
                LuaParser.LuaTable i_spawn_table = (LuaParser.LuaTable)table["SpawnData"];
                List<double> i_spawn_indices = new List<double>();
                foreach (var key in i_spawn_table.entries.Keys)
                    i_spawn_indices.Add((double)key);

                i_spawn_indices.Sort();
                foreach (double j in i_spawn_indices)
                {
                    int _j = (int)j;
                    LuaParser.LuaTable i_spawn_data_table = (LuaParser.LuaTable)i_spawn_table[j];
                    SFMapCoopSpawnTypeDataInfo cstdi = new SFMapCoopSpawnTypeDataInfo();
                    cstdi.ParseLoad(i_spawn_data_table);
                    data.Add(_j, cstdi);
                }
            }
        }

        public string ParseToString()
        {
            string ret = "";
            ret += "Name = \"" + name.ToString() + "\",";
            ret += "\r\nLevelRange = \"" + level_range.ToString() + "\",";
            if (goal != LuaEnumAiGoal.GoalDefault)
                ret += "\r\nGoal = " + goal.ToString() + ",";
            if (max_units != 0)
                ret += "\r\nMaxClanSize = " + max_units.ToString() + ",";
            if (start_units != null)
            {
                ret += "\r\nInit = \r\n{\r\n\t";
                foreach (int i in start_units)
                    ret += i.ToString() + ", ";
                ret += "\r\n},";
            }
            if (data != null)
            {
                ret += "\r\nSpawnData = \r\n";
                ret += SFLuaEnvironment.ParseDictToString(data);
                ret += ",";
            }
            return ret;
        }
    }
    
    public class SFLuaSQLRtsCoopSpawn: ILuaSQL
    {
        public Dictionary<int, SFMapCoopSpawnTypeInfo> coop_spawn_types { get; private set; } = null;

        public int Load()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Load() called");

            // check if file exists
            string filename = SFUnPak.SFUnPak.game_directory_name;
            if (filename == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Load(): Game directory not found!");
                return -1;
            }

            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Load(): Executing script script\\gdsrtscoopspawngroups.lua");

            object[] ret = SFLuaEnvironment.ExecuteGameScript("script\\gdsrtscoopspawngroups.lua");
            if(ret==null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Load(): Could not execute spawn script!");
                return -6;
            }

            if (coop_spawn_types == null)
                coop_spawn_types = new Dictionary<int, SFMapCoopSpawnTypeInfo>();

            int log_current_spawn = 0;
            try
            {
                coop_spawn_types.Clear();

                LuaParser.LuaTable table = (LuaParser.LuaTable)ret[0];
                List<double> indices = new List<double>();
                foreach (var key in table.entries.Keys)
                    indices.Add((double)key);

                indices.Sort();
                // iterate over the rts coop spawn table
                foreach (double i in indices)
                {
                    int _i = (int)i;
                    log_current_spawn = _i;
                    LuaParser.LuaTable i_table = (LuaParser.LuaTable)table[i];
                    SFMapCoopSpawnTypeInfo csti = new SFMapCoopSpawnTypeInfo();
                    csti.ParseLoad(i_table);
                    coop_spawn_types.Add(_i, csti);
                }
            }
            catch (Exception)
            {
                Unload();
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Load(): Error reading spawn file! Spawn ID = " + log_current_spawn.ToString());
                return -3;
            }
            

            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Load(): Spawn file read successfully, found spawns: "+coop_spawn_types.Count.ToString());

            return 0;
        }

        public int Save()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Save(): called");

            if (coop_spawn_types == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Save(): Spawns are not loaded in!");
                return -4;
            }

            // check if file exists
            string filename = SFUnPak.SFUnPak.game_directory_name;
            if (filename == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Save(): Game directory not found!");
                return -1;
            }
            filename += "\\script\\gdsrtscoopspawngroups.lua";

            try
            {
                File.WriteAllText(filename, "return\r\n" + SFLuaEnvironment.ParseDictToString(coop_spawn_types));
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Save(): Error writing spawn data to file (filename = "+filename+")");
                return -3;
            }

            return 0;
        }

        public void Unload()
        {
            if (coop_spawn_types != null)
            {
                coop_spawn_types.Clear();
                coop_spawn_types = null;
            }
        }
    }
}
