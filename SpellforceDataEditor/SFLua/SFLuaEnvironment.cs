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
                sw.Write(Utility.TabulateString(dict[key].ParseToString(), 2));
                sw.Write("\r\n\t},");
            }
            sw.Write("\r\n}");
            sw.Close();
            return sw.ToString();
        }

        public static object[] ExecuteGameScript(string fname)
        {
            if (!SFUnPak.SFUnPak.game_directory_specified)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaEnvironment.ExecuteGameScript(): Game directory is not specified!");
                return null;
            }

            if(File.Exists(SFUnPak.SFUnPak.game_directory_name+"\\"+fname))
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
                return "";
            }
            else
            {
                if (item_data.MeshMaleCold != "<undefined>")
                    return item_data.MeshMaleCold;
                else if (item_data.MeshMaleWarm != "<undefined>")
                    return item_data.MeshMaleWarm;
                return "";
            }
        }
    }
}
