using System;
using System.Collections.Generic;
using System.IO;

namespace SFEngine.SFLua.lua_sql
{
    // returns error code
    public interface ILuaSQL
    {
        int Load();
        int Save();
        void Unload();
    }

    public class SFLuaSQL<T>: ILuaSQL where T: class, ILuaParsable, new()
    {
        public Dictionary<int, T> items { get; private set; } = null;
        public string script_name;

        public T this[int index]
        {
            get
            {
                if (items.ContainsKey(index))
                {
                    return items[index];
                }

                return null;
            }
            set
            {
                if (items.ContainsKey(index))
                {
                    items[index] = value;
                }
                else
                {
                    items.Add(index, value);
                }
            }
        }

        public int Load()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, $"SFLuaSQL.Load(\"{script_name}\") called");

            // check if file exists
            string filename = SFUnPak.SFUnPak.game_directory_name;
            if (filename == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQL.Load(): Game directory not found!");
                return -1;
            }

            LogUtils.Log.Info(LogUtils.LogSource.SFLua, $"SFLuaSQL.Load(): Executing script \"{script_name}\"");

            object[] ret = SFLuaEnvironment.ExecuteGameScript(script_name);
            if (ret == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQL.Load(): Could not execute script!");
                return -6;
            }

            if (items == null)
            {
                items = new Dictionary<int, T>();
            }

            int log_current_item = 0;
            try
            {
                items.Clear();

                LuaParser.LuaTable table = (LuaParser.LuaTable)ret[0];
                List<double> indices = new List<double>();
                foreach (var key in table.entries.Keys)
                {
                    indices.Add((double)key);
                }

                indices.Sort();
                // iterate over the rts coop spawn table
                foreach (double i in indices)
                {
                    int _i = (int)i;
                    log_current_item = _i;
                    LuaParser.LuaTable i_table = (LuaParser.LuaTable)table[i];
                    T data = new T();
                    data.ParseLoad(i_table);
                    items.Add(_i, data);
                }
            }
            catch (Exception)
            {
                Unload();
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQL.Load(): Error reading file! Item ID = " + log_current_item.ToString());
                return -3;
            }

            return 0;
        }

        public int Save()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQL.Save(): called");

            if (items == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQLg.Save(): Data not found!");
                return -4;
            }

            // check if file exists
            string filename = SFUnPak.SFUnPak.game_directory_name;
            if (filename == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQL.Save(): Game directory not found!");
                return -1;
            }
            filename += "\\" + script_name;

            try
            {
                File.WriteAllText(filename, "return\r\n" + SFLuaEnvironment.ParseDictToString(items));
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQL.Save(): Error writing data to file (filename = " + filename + ")");
                return -3;
            }

            return 0;
        }
        public void Unload()
        {
            if (items != null)
            {
                items.Clear();
                items = null;
            }
        }
    }
}
