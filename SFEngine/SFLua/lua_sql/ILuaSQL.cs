using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SFEngine.SFLua.lua_sql
{
    // returns error code
    public interface ILuaSQL
    {
        int Load(Lua l);
        int Save();
        void Unload();
    }

    public interface ILuaParsable
    {
        void ParseLoad(LuaTable table);
        string ParseToString();
    }

    public class SFLuaSQL<T>: ILuaSQL where T: class, ILuaParsable, new()
    {
        public bool loaded = false;
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

        public int Load(Lua lua)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQL.Load(): called");
            if (loaded)
            {
                return 0;
            }
            if(lua.DoFile(script_name) != LuaError.OK)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQL.Load(): Could not execute script!");
                return -6;
            }
            LuaTable table = lua.PopObject() as LuaTable;
            if(table == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaSQL.Load(): Invalid return from script");
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

                Dictionary<object, object> table_dict = table.GetDict();

                List<double> indices = new List<double>();
                foreach (var key in table_dict.Keys)
                {
                    indices.Add((double)key);
                }

                indices.Sort();
                // iterate over the rts coop spawn table
                foreach (double i in indices)
                {
                    int _i = (int)i;
                    log_current_item = _i;
                    LuaTable i_table = (LuaTable)table_dict[i];
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

            loaded = true;
            return 0;
        }

        public int Save()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaSQL.Save(): called");

            if (!loaded)
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
                StringBuilder sb = new();
                sb.Append($"return\r\n{{\r\n");
                List<int> keys = items.Keys.ToList();
                keys.Sort();
                foreach (var i in keys)
                {
                    sb.Append($"[{i}] = \r\n\t{{");
                    sb.Append(items[i].ParseToString());
                    sb.Append($"\r\n\t}},");
                }
                sb.Append($"}}\r\n");
                File.WriteAllText(filename, sb.ToString());
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
            if (loaded)
            {
                items.Clear();
                items = null;
            }
            loaded = false;
        }
    }
}
