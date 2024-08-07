using SFEngine.SFLua.lua_sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SFEngine.SFLua
{
    public static class LuaSQLDatabase
    {
        public static bool data_loaded { get; private set; } = false;
        public static lua_sql.SFLuaSQL<lua_sql.SFMapCoopSpawnTypeInfo> coop_spawns { get; private set; } = new lua_sql.SFLuaSQL<lua_sql.SFMapCoopSpawnTypeInfo>() { script_name = "script\\gdsrtscoopspawngroups.lua" };
        public static lua_sql.SFLuaSQL<lua_sql.SFLuaSQLItemData> items { get; private set; } = new lua_sql.SFLuaSQL<lua_sql.SFLuaSQLItemData>() { script_name = "script\\sql_item.lua" };
        public static lua_sql.SFLuaSQL<lua_sql.SFLuaSQLObjectData> objects { get; private set; } = new lua_sql.SFLuaSQL<lua_sql.SFLuaSQLObjectData>() { script_name = "script\\sql_object.lua" };
        public static lua_sql.SFLuaSQL<lua_sql.SFLuaSQLBuildingData> buildings { get; private set; } = new lua_sql.SFLuaSQL<lua_sql.SFLuaSQLBuildingData>() { script_name = "script\\sql_building.lua" };
        public static lua_sql.SFLuaSQL<lua_sql.SFLuaSQLHeadData> heads { get; private set; } = new lua_sql.SFLuaSQL<lua_sql.SFLuaSQLHeadData>() { script_name = "script\\sql_head.lua" };

        static public int GetDecompiledString(string fname, ref string result)
        {
            if (!SFUnPak.SFUnPak.game_directory_specified)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaEnvironment.GetDecompiledString(): Game directory is not specified!");
                return -1;
            }

            byte[] data = SFUnPak.SFUnPak.LoadFileFrom("sf34.pak", fname);
            if (data == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaEnvironment.GetDecompiledString(): Could not find file " + fname + " in game paks!");
                return -2;
            }
            else
            {
                MemoryStream ms = new MemoryStream(data);
                BinaryReader br = new BinaryReader(ms);

                LuaDecompiler.LuaBinaryScript scr = new LuaDecompiler.LuaBinaryScript(br);
                if (scr.func == null)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaEnvironment.GetDecompiledString(): Could not load binary script from file " + fname);
                    return -3;
                }

                br.Close();

                LuaDecompiler.Decompiler dec = new LuaDecompiler.Decompiler();
                LuaDecompiler.Node res;

                try
                {
                    res = dec.Decompile(scr.func);
                }
                catch (Exception)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaEnvironment.GetDecompiledString(): Could not decompile binary script " + fname);
                    return -4;
                }

                string ret = "";
                try
                {
                    StringWriter sw = new StringWriter();
                    res.WriteLuaString(sw);
                    ret = sw.ToString();
                }
                catch (Exception)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaEnvironment.GetDecompiledString(): Could not generate result from decompiled script " + fname);
                    return -5;
                }

                result = ret;
            }

            return 0;
        }

        public static void LoadSQL(Lua lua, bool force = true)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaEnvironment.LoadSQL() called");

            if ((!force) && (data_loaded))
            {
                return;
            }

            data_loaded = true;
            int result = coop_spawns.Load(lua);
            if (result != 0)
            {
                UnloadSQL();
                return;
            }
            result = items.Load(lua);
            if (result != 0)
            {
                UnloadSQL();
                return;
            }
            result = objects.Load(lua);
            if (result != 0)
            {
                UnloadSQL();
                return;
            }
            result = buildings.Load(lua);
            if (result != 0)
            {
                UnloadSQL();
                return;
            }
            result = heads.Load(lua);
            if (result != 0)
            {
                UnloadSQL();
                return;
            }

            GC.Collect();
        }

        public static void UnloadSQL()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "SFLuaEnvironment.UnloadSQL() called");

            if (!data_loaded)
            {
                return;
            }

            coop_spawns.Unload();
            items.Unload();
            objects.Unload();
            buildings.Unload();
            heads.Unload();
            data_loaded = false;
        }

        public static string GetItemMesh(int item_id, bool is_female)
        {
            lua_sql.SFLuaSQLItemData item_data = items[item_id];
            if (item_data == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaEnvironment.GetItemMesh(): Item does not exist (item id " + item_id.ToString() + ")");
                return "";
            }

            if (is_female)
            {
                if (item_data.MeshFemaleCold != "<undefined>")
                {
                    return item_data.MeshFemaleCold;
                }
                else if (item_data.MeshFemaleWarm != "<undefined>")
                {
                    return item_data.MeshFemaleWarm;
                }
                else if (item_data.MeshMaleCold != "<undefined>")
                {
                    return item_data.MeshMaleCold;
                }
                else if (item_data.MeshMaleWarm != "<undefined>")
                {
                    return item_data.MeshMaleWarm;
                }

                return "";
            }
            else
            {
                if (item_data.MeshMaleCold != "<undefined>")
                {
                    return item_data.MeshMaleCold;
                }
                else if (item_data.MeshMaleWarm != "<undefined>")
                {
                    return item_data.MeshMaleWarm;
                }
                else if (item_data.MeshFemaleCold != "<undefined>")
                {
                    return item_data.MeshFemaleCold;
                }
                else if (item_data.MeshFemaleWarm != "<undefined>")
                {
                    return item_data.MeshFemaleWarm;
                }

                return "";
            }
        }
    }
}