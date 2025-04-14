/*
 * LuaNative module provides wrappers for Lua 4.0.1 functions
 * See Lua 4.0.1 documentation for description of these functions
 * */

using System.Runtime.InteropServices;

using lua_State = System.IntPtr;
using charptr_t = System.IntPtr;
using lua_Number = System.Double;
using size_t = System.UIntPtr;
using lua_CFunction = System.IntPtr;
using voidptr_t = System.IntPtr;

namespace SFEngine.SFLua
{
    internal static class LuaNative
    {
        private const string LibName = "lua4.dll";

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern lua_State lua_open(int stacksize);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_close(lua_State L);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_gettop(lua_State L);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_settop(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushvalue(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_remove(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_insert(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_stackspace(lua_State L);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_type(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern charptr_t lua_typename(lua_State L, int t);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_isnumber(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_isstring(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_iscfunction(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_tag(lua_State L, int index);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_equal(lua_State L, int index1, int index2);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_lessthan(lua_State L, int index1, int index2);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern lua_Number lua_tonumber(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern charptr_t lua_tostring(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t lua_strlen(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern lua_CFunction lua_tocfunction(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern voidptr_t lua_touserdata(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern voidptr_t lua_topointer(lua_State L, int index);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushnil(lua_State L);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushnumber(lua_State L, lua_Number n);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void lua_pushlstring(lua_State Ll, byte[] s, size_t len);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void lua_pushstring(lua_State L, charptr_t s);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushcclosure(lua_State L, lua_CFunction fn, int n);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushusertag(lua_State L, voidptr_t u, int tag);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void lua_getglobal(lua_State L, [MarshalAs(UnmanagedType.LPStr)] string name);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_gettable(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawget(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawgeti(lua_State L, int index, int n);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_getglobals(lua_State L);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void lua_gettagmethod(lua_State L, int tag, [MarshalAs(UnmanagedType.LPStr)] string ev);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getref(lua_State L, int r);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_newtable(lua_State L);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void lua_setglobal(lua_State L, [MarshalAs(UnmanagedType.LPStr)] string name);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_settable(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawset(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawseti(lua_State L, int index, int n);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_setglobals(lua_State L);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void lua_settagmethod(lua_State L, int tag, [MarshalAs(UnmanagedType.LPStr)] string ev);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_ref(lua_State L, int l);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_call(lua_State L, int nargs, int nresults);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawcall(lua_State L, int nargs, int nresults);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int lua_dofile(lua_State L, [MarshalAs(UnmanagedType.LPStr)] string filename);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int lua_dostring(lua_State L, [MarshalAs(UnmanagedType.LPStr)] string str);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int lua_dobuffer(lua_State L, [MarshalAs(UnmanagedType.LPArray)] byte[] buff, size_t size, [MarshalAs(UnmanagedType.LPStr)] string name);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getgcthreshold(lua_State L);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getgccount(lua_State L);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_setgcthreshold(lua_State L, int newthreshold);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_newtag(lua_State L);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_copytagmethods(lua_State L, int tagto, int tagfrom);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_settag(lua_State L, int tag);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void lua_error(lua_State L, [MarshalAs(UnmanagedType.LPStr)] string s);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_unref(lua_State L, int r);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_next(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getn(lua_State L, int index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_concat(lua_State L, int n);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern voidptr_t lua_newuserdata(lua_State L, int size);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_baselibopen(lua_State L);
    }
}
