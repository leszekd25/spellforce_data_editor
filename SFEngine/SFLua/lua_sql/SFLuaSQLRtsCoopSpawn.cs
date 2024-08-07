using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SFEngine.SFLua.lua_sql
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

        public void ParseLoad(LuaTable table)
        {
            seconds_per_tick = 0;
            if (table.TryGet("Seconds", out object o))
            {
                seconds_per_tick += (int)(double)o;
            }

            if (table.TryGet("Minutes", out o))
            {
                seconds_per_tick += (int)((double)o * 60);
            }

            if (table.TryGet("Hours", out o))
            {
                seconds_per_tick += (int)((double)o * 3600);
            }

            if (seconds_per_tick == 0)
            {
                seconds_per_tick = 60;
            }

            if (table.TryGet("Units", out o))
            {
                units = new List<int>();
                LuaTable i_spawn_data_units_table = (LuaTable)o;
                foreach(KeyValuePair<object, object> kv in i_spawn_data_units_table)
                {
                    units.Add((int)(double)kv.Value);
                }
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
                {
                    ret += i.ToString() + ", ";
                }

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

        public void ParseLoad(LuaTable table)
        {
            name = "";
            if (table.TryGet("Name", out object o))
            {
                name = (string)o;
            }

            level_range = "";
            if (table.TryGet("LevelRange", out o))
            {
                level_range = (string)o;
            }

            goal = LuaEnumAiGoal.GoalDefault;
            if (table.TryGet("Goal", out o))
            {
                string s = (string)o;
                bool success = Enum.TryParse(s, out goal);
                if (!success)
                {
                    goal = LuaEnumAiGoal.GoalDefault;
                }
            }

            max_units = 0;
            if (table.TryGet("MaxClanSize", out o))
            {
                max_units = (int)(double)o;
            }

            if (table.TryGet("Init", out o))
            {
                start_units = new List<int>();
                LuaTable i_init_table = (LuaTable)o;
                foreach (KeyValuePair<object, object> kv in i_init_table)
                {
                    start_units.Add((int)(double)kv.Value);
                }
            }

            if (table.TryGet("SpawnData", out o))
            {
                data = new Dictionary<int, SFMapCoopSpawnTypeDataInfo>();
                LuaTable i_spawn_table = (LuaTable)o;

                var table_dict = i_spawn_table.GetDict();

                List<double> i_spawn_indices = new List<double>();
                foreach (var key in table_dict.Keys)
                {
                    i_spawn_indices.Add((double)key);
                }

                i_spawn_indices.Sort();
                foreach (double j in i_spawn_indices)
                {
                    int _j = (int)j;
                    LuaTable i_spawn_data_table = (LuaTable)table_dict[j];
                    SFMapCoopSpawnTypeDataInfo cstdi = new SFMapCoopSpawnTypeDataInfo();
                    cstdi.ParseLoad(i_spawn_data_table);
                    data.Add(_j, cstdi);
                }
            }
        }

        public string ParseToString()
        {
            StringBuilder ret = new();
            ret.Append($"Name = \"{name}\",");
            ret.Append($"\r\nLevelRange = \"{level_range}\",");
            if (goal != LuaEnumAiGoal.GoalDefault)
            {
                ret.Append($"\r\nGoal = {goal},");
            }

            if (max_units != 0)
            {
                ret.Append($"\r\nMaxClanSize = {max_units},");
            }

            if (start_units != null)
            {
                ret.Append($"\r\nInit = \r\n{{\r\n\t");
                foreach (int i in start_units)
                {
                    ret.Append($"{i}, ");
                }

                ret.Append($"\r\n}},");
            }
            if (data != null)
            {
                ret.Append($"\r\nSpawndata = \r\n{{\r\n\t");

                List<int> indices = data.Keys.ToList();
                indices.Sort();
                foreach(var i in indices)
                {
                    ret.Append($"[{i}] = \r\n\t{{");
                    ret.Append(data[i].ParseToString());
                    ret.Append($"\r\n\t}},");
                }

                ret.Append($"\r\n}},");
            }
            return ret.ToString();
        }
    }
}
