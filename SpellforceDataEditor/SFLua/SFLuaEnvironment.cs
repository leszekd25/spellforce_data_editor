using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua
{
    public static class SFLuaEnvironment
    {
        public static bool data_loaded { get; private set; } = false;
        public static lua_sql.SFLuaSQLRtsCoopSpawn coop_spawns { get; private set; } = new lua_sql.SFLuaSQLRtsCoopSpawn();
        public static lua_sql.SFLuaSQLItem items { get; private set; } = new lua_sql.SFLuaSQLItem();
        public static lua_sql.SFLuaSQLObject objects { get; private set; } = new lua_sql.SFLuaSQLObject();
        public static lua_sql.SFLuaSQLBuilding buildings { get; private set; } = new lua_sql.SFLuaSQLBuilding();
        public static lua_sql.SFLuaSQLHead heads { get; private set; } = new lua_sql.SFLuaSQLHead();
        public static lua_sql_forms.SFLuaSQLRtsCoopSpawnForm coop_spawns_form { get; private set; } = null;
        public static lua_sql_forms.SFLuaSQLItemForm items_form { get; private set; } = null;
        public static lua_sql_forms.SFLuaSQLObjectForm objects_form { get; private set; } = null;
        public static lua_sql_forms.SFLuaSQLBuildingForm buildings_form { get; private set; } = null;
        public static lua_sql_forms.SFLuaSQLHeadForm heads_form = null;

        public static string ParseDictToString<T>(Dictionary<int, T> dict) where T: ILuaParsable
        {
            StringWriter sw = new StringWriter();
            sw.Write("{");

            List<int> keys = new List<int>(dict.Keys);
            keys.Sort();
            foreach (int key in keys)
            {
                sw.Write("\r\n\t[" + key.ToString() + "] = \r\n\t{");
                sw.Write(Utility.TabulateStringNewline(dict[key].ParseToString(), 2));
                sw.Write("\r\n\t},");
            }
            sw.Write("\r\n}");
            sw.Close();
            return sw.ToString();
        }

        // only used in SQL stuff
        public static object[] ExecuteGameScript(string fname)
        {
            if (!SFUnPak.SFUnPak.game_directory_specified)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaEnvironment.ExecuteGameScript(): Game directory is not specified!");
                return null;
            }

            if (File.Exists(SFUnPak.SFUnPak.game_directory_name+"\\"+fname))
            {
                LuaParser.LuaTable test = new LuaParser.LuaTable();
                LuaParser.LuaScript scr = new  LuaParser.LuaScript(File.ReadAllText(SFUnPak.SFUnPak.game_directory_name + "\\" + fname));
                scr.position = scr.code.IndexOf('{');       // temporary
                if (!test.Parse(scr))
                    return null;

                return new object[] { test };
            }
            else
            {
                // testing  luadec
                MemoryStream ms = SFUnPak.SFUnPak.LoadFileFrom("sf34.pak", fname);
                BinaryReader br = new BinaryReader(ms);
                LuaDecompiler.LuaBinaryScript scr = new LuaDecompiler.LuaBinaryScript(br);
                LuaDecompiler.LuaState state = new LuaDecompiler.LuaState();
                object[] ret = state.Run(scr);
                br.Close();
                return ret;
            }
        }

        static public int GetDecompiledString(string fname, ref string result)
        {
            if (!SFUnPak.SFUnPak.game_directory_specified)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaEnvironment.GetDecompiledString(): Game directory is not specified!");
                return -1;
            }

            MemoryStream ms = SFUnPak.SFUnPak.LoadFileFrom("sf34.pak", fname);
            if (ms == null)
                return -2;
            else
            {
                BinaryReader br = new BinaryReader(ms);
                LuaDecompiler.LuaBinaryScript scr = null;
                try
                {
                    scr = new LuaDecompiler.LuaBinaryScript(br);
                }
                catch (Exception)
                {
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
                    return -5;
                }

                result = ret;
            }

            return 0;
        }

        // general version, likely still bad...
        static public int OpenScript(string fname)
        {
            if (!SFUnPak.SFUnPak.game_directory_specified)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaEnvironment.OpenScript(): Game directory is not specified!");
                return -1;
            }

            if (!File.Exists(SFUnPak.SFUnPak.game_directory_name + "\\" + fname))
            {
                string ret = "";

                int result = GetDecompiledString(fname, ref ret);
                if (result == -2)
                {
                    if (MessageBox.Show("Script does not exist. Create a new script?", "Script not found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        FileInfo fo = new FileInfo(SFUnPak.SFUnPak.game_directory_name + "\\" + fname);
                        fo.Directory.Create();
                        File.Create("@" + SFUnPak.SFUnPak.game_directory_name + "\\" + fname);
                    }
                    else
                        return -2;
                }
                else if (result == 0)
                {
                    FileInfo fo = new FileInfo(SFUnPak.SFUnPak.game_directory_name + "\\" + fname);
                    fo.Directory.Create();
                    File.WriteAllText(SFUnPak.SFUnPak.game_directory_name + "\\" + fname, ret);
                }
                else
                    return -3;
            }

            System.Diagnostics.Process.Start(SFUnPak.SFUnPak.game_directory_name + "\\" + fname);

            return 0;
        }

        // specialized version; arguments are named
        static public int OpenNPCScript(int platform_id, int npc_id)
        {
            string fname = "script\\p" + platform_id.ToString() + "\\n" + npc_id.ToString() + ".lua";
            if (!SFUnPak.SFUnPak.game_directory_specified)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaEnvironment.OpenNPCScript(): Game directory is not specified!");
                return -1;
            }
            if (!File.Exists(SFUnPak.SFUnPak.game_directory_name + "\\" + fname))
            {
                string ret = "";

                int result = GetDecompiledString(fname, ref ret);
                if (result == -2)
                {
                    if (MessageBox.Show("Script does not exist. Create a new script?", "Script not found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (!Directory.Exists(SFUnPak.SFUnPak.game_directory_name + "\\script"))
                            Directory.CreateDirectory(SFUnPak.SFUnPak.game_directory_name + "\\script");
                        if (!Directory.Exists(SFUnPak.SFUnPak.game_directory_name + "\\script\\p" + platform_id.ToString()))
                            Directory.CreateDirectory(SFUnPak.SFUnPak.game_directory_name + "\\script\\p" + platform_id.ToString());

                        File.Create("@" + SFUnPak.SFUnPak.game_directory_name + "\\" + fname);
                    }
                    else
                        return -2;
                }
                else if (result == 0)
                {
                    if (!Directory.Exists(SFUnPak.SFUnPak.game_directory_name + "\\script"))
                        Directory.CreateDirectory(SFUnPak.SFUnPak.game_directory_name + "\\script");
                    if (!Directory.Exists(SFUnPak.SFUnPak.game_directory_name + "\\script\\p" + platform_id.ToString()))
                        Directory.CreateDirectory(SFUnPak.SFUnPak.game_directory_name + "\\script\\p" + platform_id.ToString());

                    ret = ret.Replace("__arg0", "_Type");
                    ret = ret.Replace("__arg1", "_PlatformId");
                    ret = ret.Replace("__arg2", "_NpcId");
                    ret = ret.Replace("__arg3", "_X");
                    ret = ret.Replace("__arg4", "_Y");

                    File.WriteAllText(SFUnPak.SFUnPak.game_directory_name + "\\" + fname, ret);
                }
                else
                    return -3;
            }

            System.Diagnostics.Process.Start(SFUnPak.SFUnPak.game_directory_name + "\\" + fname);
            
            return 0;
        }

        public static void ShowRtsCoopSpawnGroupsForm()
        {
            if (coop_spawns_form != null)
                return;

            coop_spawns_form = new lua_sql_forms.SFLuaSQLRtsCoopSpawnForm();
            coop_spawns_form.ShowDialog();
            coop_spawns_form = null;
            GC.Collect();
        }

        public static void ShowSQLItemForm()
        {
            if (items_form != null)
                return;

            items_form = new lua_sql_forms.SFLuaSQLItemForm();
            items_form.ShowDialog();
            items_form = null;
            GC.Collect();
        }

        public static void ShowSQLObjectForm()
        {
            if (objects_form != null)
                return;

            objects_form = new lua_sql_forms.SFLuaSQLObjectForm();
            objects_form.ShowDialog();
            objects_form = null;
            GC.Collect();
        }

        public static void ShowSQLBuildingForm()
        {
            if (buildings_form != null)
                return;

            buildings_form = new lua_sql_forms.SFLuaSQLBuildingForm();
            buildings_form.ShowDialog();
            buildings_form = null;
            GC.Collect();
        }

        public static void ShowSQLHeadForm()
        {
            if (heads_form != null)
                return;

            heads_form = new lua_sql_forms.SFLuaSQLHeadForm();
            heads_form.ShowDialog();
            heads_form = null;
            GC.Collect();
        }

        public static void LoadSQL(bool force = true)
        {
            if ((!force) && (data_loaded))
                return;

            data_loaded = true;
            int result = coop_spawns.Load();
            if (result != 0)
            {
                UnloadSQL();
                return;
            }
            result = items.Load();
            if (result != 0)
            {
                UnloadSQL();
                return;
            }
            result = objects.Load();
            if (result != 0)
            {
                UnloadSQL();
                return;
            }
            result = buildings.Load();
            if (result != 0)
            {
                UnloadSQL();
                return;
            }
            result = heads.Load();
            if (result != 0)
            {
                UnloadSQL();
                return;
            }

            GC.Collect();
        }

        public static void UnloadSQL()
        {
            if (!data_loaded)
                return;
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
            if(item_data==null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaEnvironment.GetItemMesh(): Item does not exist (item id " + item_id.ToString() + ")");
                return "";
            }

            if (is_female)
            {
                if (item_data.MeshFemaleCold != "<undefined>")
                    return item_data.MeshFemaleCold;
                else if (item_data.MeshFemaleWarm != "<undefined>")
                    return item_data.MeshFemaleWarm;
                else if (item_data.MeshMaleCold != "<undefined>")
                    return item_data.MeshMaleCold;
                else if (item_data.MeshMaleWarm != "<undefined>")
                    return item_data.MeshMaleWarm;
                return "";
            }
            else
            {
                if (item_data.MeshMaleCold != "<undefined>")
                    return item_data.MeshMaleCold;
                else if (item_data.MeshMaleWarm != "<undefined>")
                    return item_data.MeshMaleWarm;
                else if (item_data.MeshFemaleCold != "<undefined>")
                    return item_data.MeshFemaleCold;
                else if (item_data.MeshFemaleWarm != "<undefined>")
                    return item_data.MeshFemaleWarm;
                return "";
            }
        }
    }
}
