using System;

namespace SpellforceDataEditor.SFLua
{
    class SFLuaWinformManager
    {
        public static lua_sql_forms.SFLuaSQLRtsCoopSpawnForm coop_spawns_form { get; private set; } = null;
        public static lua_sql_forms.SFLuaSQLItemForm items_form { get; private set; } = null;
        public static lua_sql_forms.SFLuaSQLObjectForm objects_form { get; private set; } = null;
        public static lua_sql_forms.SFLuaSQLBuildingForm buildings_form { get; private set; } = null;
        public static lua_sql_forms.SFLuaSQLHeadForm heads_form = null;


        public static void ShowRtsCoopSpawnGroupsForm()
        {
            if (coop_spawns_form != null)
            {
                return;
            }

            coop_spawns_form = new lua_sql_forms.SFLuaSQLRtsCoopSpawnForm();
            coop_spawns_form.ShowDialog();
            coop_spawns_form = null;
            GC.Collect();
        }

        public static void ShowSQLItemForm()
        {
            if (items_form != null)
            {
                return;
            }

            items_form = new lua_sql_forms.SFLuaSQLItemForm();
            items_form.ShowDialog();
            items_form = null;
            GC.Collect();
        }

        public static void ShowSQLObjectForm()
        {
            if (objects_form != null)
            {
                return;
            }

            objects_form = new lua_sql_forms.SFLuaSQLObjectForm();
            objects_form.ShowDialog();
            objects_form = null;
            GC.Collect();
        }

        public static void ShowSQLBuildingForm()
        {
            if (buildings_form != null)
            {
                return;
            }

            buildings_form = new lua_sql_forms.SFLuaSQLBuildingForm();
            buildings_form.ShowDialog();
            buildings_form = null;
            GC.Collect();
        }

        public static void ShowSQLHeadForm()
        {
            if (heads_form != null)
            {
                return;
            }

            heads_form = new lua_sql_forms.SFLuaSQLHeadForm();
            heads_form.ShowDialog();
            heads_form = null;
            GC.Collect();
        }

    }
}
