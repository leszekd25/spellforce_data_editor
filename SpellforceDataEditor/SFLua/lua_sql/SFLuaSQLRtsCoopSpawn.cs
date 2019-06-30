using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SpellforceDataEditor.SFLua.lua_sql
{
    public class SFMapCoopSpawnTypeDataInfo : ILuaParsable
    {
        public int seconds_per_tick;     // convert from double
        public List<int> units;

        public void ParseLoad(NLua.LuaTable table)
        {
            seconds_per_tick = 0;
            if (table["Seconds"] != null)
                seconds_per_tick += (int)Utility.CastDouble(table["Seconds"]);
            if (table["Minutes"] != null)
                seconds_per_tick += (int)(Utility.CastDouble(table["Minutes"]) * 60);
            if (table["Hours"] != null)
                seconds_per_tick += (int)(Utility.CastDouble(table["Hours"]) * 3600);
            if (seconds_per_tick == 0)
                seconds_per_tick = 60;

            if (table["Units"] != null)
            {
                units = new List<int>();
                NLua.LuaTable i_spawn_data_units_table = (NLua.LuaTable)table["Units"];

                for (long k = 1; k <= i_spawn_data_units_table.Values.Count; k++)
                    units.Add((int)(long)i_spawn_data_units_table[k]);
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
        public SFLua.LuaEnumAiGoal goal;
        public int max_units;
        public List<int> start_units;
        public Dictionary<int, SFMapCoopSpawnTypeDataInfo> data;

        public void ParseLoad(NLua.LuaTable table)
        {
            name = "";
            if (table["Name"] != null)
                name = (string)table["Name"];

            level_range = "";
            if (table["LevelRange"] != null)
                level_range = (string)table["LevelRange"];

            goal = SFLua.LuaEnumAiGoal.GoalDefault;
            if (table["Goal"] != null)
                goal = (SFLua.LuaEnumAiGoal)(long)table["Goal"];

            max_units = 0;
            if (table["MaxClanSize"] != null)
                max_units = (int)(long)table["MaxClanSize"];

            if (table["Init"] != null)
            {
                start_units = new List<int>();
                NLua.LuaTable i_init_table = (NLua.LuaTable)table["Init"];
                for (long j = 1; j <= i_init_table.Values.Count; j++)
                    start_units.Add((int)(long)i_init_table[j]);
            }

            if (table["SpawnData"] != null)
            {
                data = new Dictionary<int, SFMapCoopSpawnTypeDataInfo>();
                NLua.LuaTable i_spawn_table = (NLua.LuaTable)table["SpawnData"];
                List<long> i_spawn_indices = new List<long>();
                foreach (var key in i_spawn_table.Keys)
                    i_spawn_indices.Add((long)key);

                i_spawn_indices.Sort();
                foreach (long j in i_spawn_indices)
                {
                    NLua.LuaTable i_spawn_data_table = (NLua.LuaTable)i_spawn_table[j];
                    SFMapCoopSpawnTypeDataInfo cstdi = new SFMapCoopSpawnTypeDataInfo();
                    cstdi.ParseLoad(i_spawn_data_table);
                    data.Add((int)j, cstdi);
                }
            }
        }

        public string ParseToString()
        {
            string ret = "";
            ret += "Name = \"" + name.ToString() + "\",";
            ret += "\r\nLevelRange = \"" + level_range.ToString() + "\",";
            if (goal != SFLua.LuaEnumAiGoal.GoalDefault)
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
                ret += SFLua.SFLuaEnvironment.ParseDictToString(data);
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
            if (SFLua.SFLuaEnvironment.state == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Load(): Lua is not initialized!");
                return -100;
            }
            if(!Settings.AllowLua)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Load(): Lua is disabled. Check config.txt for AllowLua option");
                return -4;
            }

            // check if file exists
            string filename = SFUnPak.SFUnPak.game_directory_name;
            if (filename == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Load(): Game directory not found!");
                return -1;
            }
            filename += "\\script\\gdsrtscoopspawngroups.lua";
            if (!File.Exists(filename))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Load(): Spawn group file does not exist!");
                return -2;
            }
            
            if (!Settings.ConfirmRunLua(filename))
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Load(): Script execution aborted by user.");
                return -5;
            }

            string script_string = "import = function () end\r\n" + File.ReadAllText(filename);
            object[] ret = SFLua.SFLuaEnvironment.state.DoString(script_string);

            if (coop_spawn_types == null)
                coop_spawn_types = new Dictionary<int, SFMapCoopSpawnTypeInfo>();

            int log_current_spawn = 0;
            try
            {
                coop_spawn_types.Clear();

                NLua.LuaTable table = (NLua.LuaTable)ret[0];
                List<long> indices = new List<long>();
                foreach (var key in table.Keys)
                    indices.Add((long)key);

                indices.Sort();
                // iterate over the rts coop spawn table
                foreach (long i in indices)
                {
                    log_current_spawn = (int)i;
                    NLua.LuaTable i_table = (NLua.LuaTable)table[i];
                    SFMapCoopSpawnTypeInfo csti = new SFMapCoopSpawnTypeInfo();
                    csti.ParseLoad(i_table);
                    coop_spawn_types.Add((int)i, csti);
                }
            }
            catch (Exception)
            {
                coop_spawn_types.Clear();
                coop_spawn_types = null;
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Load(): Error reading spawn file! Spawn ID = "+log_current_spawn.ToString());
                return -3;
            }

            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Load(): Spawn file read successfully, found spawns: "+coop_spawn_types.Count.ToString());

            return 0;
        }

        public int Save()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Save(): called");
            if (SFLua.SFLuaEnvironment.state == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Save(): Lua is not initialized!");
                return -100;
            }

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
            filename += "\\script\\gdsrtscoopspawngroups_new.lua";

            try
            {
                File.WriteAllText(filename, "return\r\n" + SFLua.SFLuaEnvironment.ParseDictToString(coop_spawn_types));
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLRtsCoopSpawn.Save(): Error writing spawn data to file (filename = "+filename+")");
                return -3;
            }

            return 0;
        }
    }
}
