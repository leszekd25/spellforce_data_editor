using SFEngine.SFLua.LuaDecompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using lua_State = System.IntPtr;

namespace SFEngine.SFLua
{
    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int CLuaFunction(lua_State luaState);

    public static class CLuaFunctionExt
    {
        public static CLuaFunction ToCLuaFunction(this IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;

            return Marshal.GetDelegateForFunctionPointer<CLuaFunction>(ptr);
        }
    }

    public class LuaFunction: LuaRef
    {
        public bool IsC;
        public CLuaFunction CFunc;

        internal LuaFunction(int r, Lua lua) : base(r, lua)
        {

        }
    }
}
