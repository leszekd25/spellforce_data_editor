/*
 * If desired, user can add their own parameters wherever they want
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpellforceDataEditor.SFLua.lua_controls;

namespace SpellforceDataEditor.SFLua
{
    static public class LuaParameter
    {
        static public LuaValueBoolControl BoolParameter()
        {
            LuaValueBoolControl l = new LuaValueBoolControl(false);
            l.Value.Name = "bool";

            l.RefreshName();

            return l;
        }

        static public LuaValueColorControl ColorParameter()
        {
            LuaValueColorControl l = new LuaValueColorControl(0xFFFFFFFF);
            l.Value.Name = "color";

            l.RefreshName();

            return l;
        }

        static public LuaValueComplexControl ComplexParameter()
        {
            LuaValueComplexControl l = new LuaValueComplexControl();
            l.Value.Name = "complex";

            l.RefreshName();

            return l;
        }

        static public LuaValueCustomControl CustomParameter()
        {
            LuaValueCustomControl l = new LuaValueCustomControl("");
            l.Value.Name = "custom";

            l.RefreshName();

            return l;
        }

        static public LuaValueDoubleControl DoubleParameter()
        {
            LuaValueDoubleControl l = new LuaValueDoubleControl(0);
            l.Value.Name = "double";

            l.RefreshName();

            return l;
        }

        static public LuaValueEnumControl EnumParameter()
        {
            LuaValueEnumControl l = new LuaValueEnumControl(LuaEnumOther.WaitForEndOfSpeech);
            l.Value.Name = "enum";

            l.RefreshName();

            return l;
        }

        static public LuaValueIntControl IntParameter()
        {
            LuaValueIntControl l = new LuaValueIntControl(0);
            l.Value.Name = "int";

            l.RefreshName();

            return l;
        }

        static public LuaValueParameterControl ParameterParameter()
        {
            LuaValueParameterControl l = new LuaValueParameterControl("");
            l.Value.Name = "parameter";

            l.RefreshName();

            return l;
        }

        static public LuaValueStringControl StringParameter()
        {
            LuaValueStringControl l = new LuaValueStringControl("");
            l.Value.Name = "string";

            l.RefreshName();

            return l;
        }
    }
}
