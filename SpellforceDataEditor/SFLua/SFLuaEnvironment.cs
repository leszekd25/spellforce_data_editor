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
        public static lua_sql.SFLuaSQLRtsCoopSpawn coop_spawns { get; private set; } = new lua_sql.SFLuaSQLRtsCoopSpawn();
        public static lua_sql_forms.SFLuaSQLRtsCoopSpawnForm coop_spawns_form { get; private set; } = null;

        public static string ParseDictToString<T>(Dictionary<int, T> dict) where T: ILuaParsable
        {
            string ret = "";
            ret += "{";

            List<int> keys = new List<int>(dict.Keys);
            keys.Sort();
            foreach (int key in keys)
            {
                ret += "\r\n\t[" + key.ToString() + "] = \r\n\t{";
                ret += Utility.TabulateString(dict[key].ParseToString(), 2);
                ret += "\r\n\t},";
            }

            ret += "\r\n}";
            return ret;
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
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "SFLuaEnvironment.ExecuteGameScript(): File does not exist! File name: "+fname);
                return null;
            }
        }

        public static void ShowRtsCoopSpawnGroupsForm()
        {
            if (coop_spawns_form != null)
                return;

            coop_spawns_form = new lua_sql_forms.SFLuaSQLRtsCoopSpawnForm();
            coop_spawns_form.ShowDialog();
            coop_spawns_form = null;
        }
    }
}
