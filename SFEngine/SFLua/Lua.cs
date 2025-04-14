/*
 * Lua module provides an interface to communicate with Lua virtual machine
 * */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.IO;

namespace SFEngine.SFLua
{
    public enum LuaType
    { 
        TNONE = -1,
        USERDATA = 0,
        NIL = 1,
        NUMBER = 2,
        STRING = 3,
        TABLE = 4,
        FUNCTION = 5
    }

    public enum LuaError
    {
        OK = 0,
        RUN = 1,
        FILE = 2,
        SYNTAX = 3,
        MEM = 4,
        ERR = 5
    }

    public enum LuaTagMethod
    {
        TM_NONE = -1,
        TM_GETTABLE = 0,
        TM_SETTABLE,
        TM_INDEX,
        TM_GETGLOBAL,
        TM_SETGLOBAL,
        TM_ADD,
        TM_SUB,
        TM_MUL,
        TM_DIV,
        TM_POW,
        TM_UNM,
        TM_LT,
        TM_CONCAT,
        TM_GC,
        TM_FUNCTION,
        TM_N		/* number of elements in the enum */
    }

    // methods marked as such will be registered to Lua upon RegisterType
    [AttributeUsage(AttributeTargets.Method)]
    public class LuaMethodAttribute: Attribute
    {
        // allowed tags: ADD, SUB, MUL, DIV, POW, UNM, LT, CONCAT
        public LuaTagMethod Tag = LuaTagMethod.TM_NONE;
    }

    // members marked as such will be registered to Lua upon RegisterType
    [AttributeUsage(AttributeTargets.Field)]
    public class LuaMemberAttribute: Attribute
    {
        public bool Get = true;
        public bool Set = true;
    }

    // 
    public class CLuaRefCounter
    {
        public Type t;
        public GCHandle obj;
        public int idx;
    }

    // each registered usertype contains getters, setters and functions that can be called from Lua VM
    public class CLuaUserdata
    {
        public int tag;
        public LuaTable registered_getters;
        public LuaTable registered_setters;
        public LuaTable registered_functions;
    }

    public class Lua : IDisposable
    {
        /*
         * functions this needs to perform:
         * - execute lua script
         * - get globals
         * - set globals
         * - register user classes?
         * */
        public IntPtr L;

        // registering userdata
        Dictionary<Type, CLuaUserdata> udtype_to_ud = new();     // finds tag associated with registered type
        Dictionary<int, Type> udtag_to_udtype = new();     // finds registered type associated with tag
        Dictionary<object, CLuaRefCounter> obj_to_ref = new();   // links C# object with its unmanaged handle and refcount in Lua

        // registry of all registered objects
        int cur_reg_idx = 0;
        Dictionary<GCHandle, int> regobj_to_objidx = new();
        Dictionary<int, GCHandle> objidx_to_regobj = new();

        // registered functions
        HashSet<LuaFunction> registered_functions = new();

        // generating code
        // each Lua object creates its own assembly used for generating special code

        // globals table
        public readonly LuaTable globals;


        // tag methods
        string[] tms = [
            "gettable",
            "settable",
            "index",
            "getglobal",
            "setglobal",
            "add",
            "sub",
            "mul",
            "div",
            "pow",
            "unm",
            "lt",
            "concat",
            "gc",
            "function"
            ];

        // all Lua objects are tracked in this dictionary
        static Dictionary<nint, Lua> luastate_to_lua = new();

        public Lua(int stacksize = 0)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "Initializing Lua 4.01");

            L = LuaNative.lua_open(stacksize);
            LuaNative.lua_baselibopen(L);

            int old_top = LuaNative.lua_gettop(L);
            // retrieve globals table for use from C# level
            LuaNative.lua_getglobals(L);
            globals = (LuaTable)PopObject();

            luastate_to_lua[L] = this;
            // register certain types for use from Lua VM level
            RegisterType<GCHandle>();     // actually necessary
            RegisterType<Type>();         // very helpful
            RegisterType<FieldInfo>();    // necessary to be able to put FieldInfo into the table
            RegisterType<Lua>();          // very helpful 
            RegisterType<MethodInfo>();   // necessary
            RegisterTagMethod<MethodInfo>(LuaTagMethod.TM_FUNCTION, LUAOVERRIDE_TM_function_methodwrapper);
            RegisterGlobalFunction("dofile", LUAOVERRIDE_dofile);

            LuaNative.lua_settop(L, old_top);
        }

        ~Lua()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            Close();
        }

        public void Close()
        {
            if (L == IntPtr.Zero)
                return;

            LuaNative.lua_close(L);
            foreach(var kv in obj_to_ref)
            {
                kv.Value.obj.Free();
            }
            foreach(var r in regobj_to_objidx.Keys)
            {
                r.Free();
            }
            luastate_to_lua.Remove(L);
            L = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        // execute Lua code
        public void ExecuteScript(string scr)
        {
            LuaError err = (LuaError)LuaNative.lua_dostring(L, scr);
            if(err != LuaError.OK)
            {
                throw new Exception();
            }
        }

        // get global values
        public object GetGlobal(string key)
        {
            LuaNative.lua_getglobal(L, key);
            return PopObject();
        }

        // remove global from Lua VM
        public void ClearGlobal(string k)
        {
            LuaNative.lua_pushnil(L);
            LuaNative.lua_setglobal(L, k);
        }

        // set global in Lua VM (double)
        public void SetGlobal(string k, double num)
        {
            LuaNative.lua_pushnumber(L, num);
            LuaNative.lua_setglobal(L, k);
        }

        // set global in Lua VM (string)
        public void SetGlobal(string k, string str)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(str);
            LuaNative.lua_pushlstring(L, buffer, (UIntPtr)buffer.Length);
            LuaNative.lua_setglobal(L, k);
        }

        // set global in Lua VM (Lua reference (table, userdata))
        public void SetGlobal(string k, LuaRef r)
        {
            LuaNative.lua_getref(L, r.reference);
            LuaNative.lua_setglobal(L, k);
        }

        // set global in Lua VM (arbitrary object)
        public void SetGlobal(string k, object o)
        {
            // registered userdata
            Type t = o.GetType();
            CLuaUserdata ud;
            while (!udtype_to_ud.TryGetValue(t, out ud))
            {
                if (t.BaseType == null)
                {
                    throw new Exception();
                }
                t = t.BaseType;
            }
            CLuaRefCounter cref;
            if (!obj_to_ref.TryGetValue(o, out cref))
            { 
                cref = new();
                cref.obj = GCHandle.Alloc(o, GCHandleType.Normal);
                cref.idx = cur_reg_idx;
                cref.t = t;
                obj_to_ref.Add(o, cref);

                regobj_to_objidx.Add(cref.obj, cur_reg_idx);
                objidx_to_regobj.Add(cur_reg_idx, cref.obj);
                cur_reg_idx++;
            }

            LuaNative.lua_pushusertag(L, cref.idx, ud.tag);
            LuaNative.lua_setglobal(L, k);
        }

        // push an arbitrary object onto stack
        public void PushObject(object o)
        {
            if (o == null)
            {
                LuaNative.lua_pushnil(L);
                return;
            }
            if (o is double)
            {
                LuaNative.lua_pushnumber(L,(double)o);
                return;
            }
            if (o is float)
            {
                LuaNative.lua_pushnumber(L, (float)o);
                return;
            }
            if (o is ulong)
            {
                LuaNative.lua_pushnumber(L, (ulong)o);
                return;
            }
            if (o is long)
            {
                LuaNative.lua_pushnumber(L, (long)o);
                return;
            }
            if (o is uint)
            {
                LuaNative.lua_pushnumber(L, (uint)o);
                return;
            }
            if (o is int)
            {
                LuaNative.lua_pushnumber(L, (int)o);
                return;
            }
            if (o is ushort)
            {
                LuaNative.lua_pushnumber(L, (ushort)o);
                return;
            }
            if (o is short)
            {
                LuaNative.lua_pushnumber(L, (short)o);
                return;
            }
            if (o is byte)
            {
                LuaNative.lua_pushnumber(L, (byte)o);
                return;
            }
            if (o is sbyte)
            {
                LuaNative.lua_pushnumber(L, (sbyte)o);
                return;
            }
            if(o is char)
            {
                LuaNative.lua_pushnumber(L, (char)o);
                return;
            }
            if(o is bool)
            {
                if((bool)o)
                {
                    LuaNative.lua_pushnumber(L, 1);
                }
                else
                {
                    LuaNative.lua_pushnil(L);
                }
                return;
            }
            if (o is string)
            {
                byte[] buffer = Encoding.ASCII.GetBytes((string)o);
                LuaNative.lua_pushlstring(L, buffer, (UIntPtr)buffer.Length);
                return;
            }
            if (o is LuaRef)
            {
                LuaNative.lua_getref(L, ((LuaRef)o).reference);
                return;
            }

            // registered userdata
            Type t = o.GetType();
            CLuaUserdata ud;
            while(!udtype_to_ud.TryGetValue(t, out ud))
            {
                if(t.BaseType == null)
                {
                    throw new Exception();
                }
                t = t.BaseType;
            }
            CLuaRefCounter cref;
            if (!obj_to_ref.TryGetValue(o, out cref))
            {
                cref = new();
                cref.obj = GCHandle.Alloc(o, GCHandleType.Normal);
                cref.idx = cur_reg_idx;
                cref.t = t;
                obj_to_ref.Add(o, cref);

                regobj_to_objidx.Add(cref.obj, cur_reg_idx);
                objidx_to_regobj.Add(cur_reg_idx, cref.obj);
                cur_reg_idx++;
            }
            LuaNative.lua_pushusertag(L, cref.idx, ud.tag);
        }

        // pop arbitrary object from stack
        public object PopObject()
        {
            int top = LuaNative.lua_gettop(L);

            LuaType t = (LuaType)LuaNative.lua_type(L, top);
            switch (t)
            {
                case LuaType.NUMBER:
                    {
                        double ret = LuaNative.lua_tonumber(L, top);
                        LuaNative.lua_settop(L, -2);  // pop one value
                        return ret;
                    }
                case LuaType.STRING:
                    {
                        IntPtr ret = LuaNative.lua_tostring(L, top);
                        string ret_s = Marshal.PtrToStringAnsi(ret);
                        LuaNative.lua_settop(L, -2);
                        return ret_s;
                    }
                case LuaType.TABLE:
                    {
                        // create reference
                        int new_ref = LuaNative.lua_ref(L, 1);
                        LuaTable ret = new(new_ref, this);
                        return ret;
                    }
                case LuaType.FUNCTION:
                    {
                        // create reference
                        nint fptr = 0;
                        bool ic = (LuaNative.lua_iscfunction(L, -1) == 1);
                        if(ic)
                        {
                            fptr = LuaNative.lua_tocfunction(L, -1);
                        }

                        int new_ref = LuaNative.lua_ref(L, 1);
                        LuaFunction ret = new(new_ref, this);
                        ret.IsC = ic;
                        if ((ic) && (fptr != 0))
                        {
                            CLuaFunction clf = fptr.ToCLuaFunction();
                            ret.CFunc = clf;
                        }
                        return ret;
                    }
                case LuaType.USERDATA:
                    {
                        int tag = LuaNative.lua_tag(L, -1);
                        if(!udtag_to_udtype.ContainsKey(tag))
                        {
                            throw new Exception("Unknown userdata type");
                        }

                        int idx = (int)LuaNative.lua_touserdata(L, -1);
                        if(!objidx_to_regobj.TryGetValue(idx, out GCHandle h))
                        {
                            throw new Exception("Invalid reference to object");
                        }

                        object o = h.Target;

                        LuaNative.lua_settop(L, -2);
                        return o;
                    }
                case LuaType.NIL:
                    {
                        LuaNative.lua_settop(L, -2);
                        return null;
                    }
                default:
                    {
                        // this should never happen
                        throw new Exception();
                    }
            }
        }

        // lua table stuff
        // get table value under field X (think table["X"])
        public object GetTableField(LuaTable t, string field)
        {
            int old_top = LuaNative.lua_gettop(L);
            LuaNative.lua_getref(L, t.reference);
            byte[] buffer = Encoding.ASCII.GetBytes(field);
            LuaNative.lua_pushlstring(L, buffer, (UIntPtr)buffer.Length);
            LuaNative.lua_rawget(L, -2);
            object ret = PopObject();
            LuaNative.lua_settop(L, old_top);
            return ret;
        }
        // get table value at index X (think table[X])
        public object GetTableField(LuaTable t, double field)
        {
            int old_top = LuaNative.lua_gettop(L);
            LuaNative.lua_getref(L, t.reference);
            LuaNative.lua_pushnumber(L, field);
            LuaNative.lua_rawget(L, -2);
            object ret = PopObject();
            LuaNative.lua_settop(L, old_top);
            return ret;
        }
        // get table value under specified object (think table[obj])
        public object GetTableField(LuaTable t, object field)
        {
            int old_top = LuaNative.lua_gettop(L);
            LuaNative.lua_getref(L, t.reference);
            PushObject(field);
            LuaNative.lua_rawget(L, -2);
            object ret = PopObject();
            LuaNative.lua_settop(L, old_top);
            return ret;
        }

        // table["X"] = o
        public void SetTableField(LuaTable t, string field, object o)
        {
            int old_top = LuaNative.lua_gettop(L);
            LuaNative.lua_getref(L, t.reference);
            byte[] buffer = Encoding.ASCII.GetBytes(field);
            LuaNative.lua_pushlstring(L, buffer, (UIntPtr)buffer.Length);
            PushObject(o);
            LuaNative.lua_settable(L, -3);
            LuaNative.lua_settop(L, old_top);
        }
        // table[X} = o
        public void SetTableField(LuaTable t, double field, object o)
        {
            int old_top = LuaNative.lua_gettop(L);
            LuaNative.lua_getref(L, t.reference);
            LuaNative.lua_pushnumber(L, field);
            PushObject(o);
            LuaNative.lua_settable(L, -3);
            LuaNative.lua_settop(L, old_top);
        }
        // table[obj] = o
        public void SetTableField(LuaTable t, object field, object o)
        {
            int old_top = LuaNative.lua_gettop(L);
            LuaNative.lua_getref(L, t.reference);
            PushObject(field);
            PushObject(o);
            LuaNative.lua_settable(L, -3);
            LuaNative.lua_settop(L, old_top);
        }

        // call specified lua function with given parameters
        // returns a table of objects or null if there are no returned values
        public object[] CallFunction(LuaFunction f, params object[] args)
        {
            int old_top = LuaNative.lua_gettop(L);

            LuaNative.lua_getref(L, f.reference);
            for(int i = 0; i < args.Length; i++)
            {
                PushObject(args[i]);
            }

            LuaNative.lua_call(L, args.Length, -1);
            int newtop = LuaNative.lua_gettop(L);
            int nresults = newtop - old_top;

            if(nresults == 0)
            {
                return null;
            }

            object[] ret = new object[nresults];
            for(int i = 0; i < nresults; i++)
            {
                ret[i] = PopObject();
            }

            return ret;
        }

        // register type for interop
        public void RegisterType<T>()
        {
            Type t = typeof(T);
            if(udtype_to_ud.ContainsKey(t))
            {
                return;
            }

            // create userdata for given type
            int newtag = LuaNative.lua_newtag(L);
            udtag_to_udtype[newtag] = t;
            CLuaUserdata ud = new() { tag = newtag };
            udtype_to_ud[t] = ud;


            // gather all Lua fields for the type
            FieldInfo[] fields = t.GetFields();
            foreach (var fi in fields)
            {
                LuaMemberAttribute lma = fi.GetCustomAttribute<LuaMemberAttribute>();
                if (lma != null)
                {
                    // register setter
                    if (lma.Set)
                    {
                        if (ud.registered_setters == null)
                        {
                            ud.registered_setters = new LuaTable(this);
                        }
                        ud.registered_setters[fi.Name] = fi;
                    }
                    // register getter
                    if (lma.Get)
                    {
                        if (ud.registered_getters == null)
                        {
                            ud.registered_getters = new LuaTable(this);
                        }
                        ud.registered_getters[fi.Name] = fi;
                    }
                }
            }

            // gather all Lua methods for the type
            MethodInfo[] methods =  t.GetMethods();
            foreach(var mi in methods)
            {
                LuaMethodAttribute lma = mi.GetCustomAttribute<LuaMethodAttribute>();
                if(lma != null)
                {
                    // check if its a constructor
                    if(mi.IsConstructor)
                    {
                        RegisterTypeConstructor<T>(mi);
                    }
                    // register regular methods
                    if(lma.Tag == LuaTagMethod.TM_NONE)
                    {
                        RegisterMethod<T>(mi.Name, mi);
                    }
                    // register events
                    else
                    {
                        switch(lma.Tag)
                        {
                            case LuaTagMethod.TM_ADD:
                            case LuaTagMethod.TM_SUB:
                            case LuaTagMethod.TM_MUL:
                            case LuaTagMethod.TM_DIV:
                            case LuaTagMethod.TM_POW:
                            case LuaTagMethod.TM_UNM:
                            case LuaTagMethod.TM_LT:
                            case LuaTagMethod.TM_CONCAT:
                                {
                                    throw new NotImplementedException("");
                                }
                            default:
                                {
                                    throw new Exception("Invalid tag method");
                                }
                        }
                    }
                }
            }

            // additional tag methods: GETTABLE, SETTABLE, GC
            RegisterTagMethod<T>(LuaTagMethod.TM_GETTABLE, LUAOVERRIDE_TM_gettable);
            RegisterTagMethod<T>(LuaTagMethod.TM_SETTABLE, LUAOVERRIDE_TM_settable);
            RegisterTagMethod<T>(LuaTagMethod.TM_GC, LUAOVERRIDE_TM_gc);
        }

        // register C# function for interop
        // the function must be static
        public void RegisterGlobalFunction(string name, MethodInfo func)
        {
            if (!func.IsStatic)
            {
                throw new Exception("Cant register non-static global functions (for now...)");
            }

            SetGlobal(name, func);
        }

        // register C Lua function for interop
        // C Lua function is a delegate that takes a lua_State pointer and returns an integer (relative stack position) that indicates how many values were returned
        // see Lua 4.01 documentation
        public LuaFunction RegisterGlobalFunction(string name, CLuaFunction func)
        {
            nint cfunc_ptr = Marshal.GetFunctionPointerForDelegate(func);
            LuaNative.lua_pushcclosure(L, cfunc_ptr, 0);
            LuaNative.lua_setglobal(L, name);
            LuaFunction luafunc = GetGlobal(name) as LuaFunction;
            registered_functions.Add(luafunc);
            return luafunc;
        }

        // register C Lua function as tag method for type T for interop
        public LuaFunction RegisterTagMethod<T>(LuaTagMethod tm, CLuaFunction func)
        {
            Type t = typeof(T);
            if (!udtype_to_ud.TryGetValue(t, out CLuaUserdata ud))
            {
                throw new Exception("Unknown userdata type");
            }

            nint cfunc_ptr = Marshal.GetFunctionPointerForDelegate(func);
            LuaNative.lua_pushcclosure(L, cfunc_ptr, 0);
            LuaFunction luafunc = PopObject() as LuaFunction;
            registered_functions.Add(luafunc);

            PushObject(luafunc);
            LuaNative.lua_settagmethod(L, ud.tag, tms[(int)tm]);

            return luafunc;
        }

        // register C# method of type T for interop
        void RegisterMethod<T>(string name, MethodInfo func)
        {
            if (func.IsStatic)
            {
                throw new Exception("Cant register static methods (for now...)");
            }
            Type t = typeof(T);
            if (!udtype_to_ud.TryGetValue(t, out CLuaUserdata ud))
            {
                throw new Exception("Unknown userdata type");
            }

            // register method
            if (ud.registered_functions == null)
            {
                ud.registered_functions = new LuaTable(this);
            }
            ud.registered_functions[name] = func;
        }

        // register C# constructor for type T for interop
        void RegisterTypeConstructor<T>(MethodInfo func)
        {
            if (!func.IsConstructor)
            {
                throw new Exception("Method is not a constructor");
            }
            if (func.IsStatic)
            {
                throw new Exception("Cant register static constructors");
            }
            Type t = typeof(T);
            if (!udtype_to_ud.TryGetValue(t, out CLuaUserdata ud))
            {
                throw new Exception("Unknown userdata type");
            }

            // register method
            if (ud.registered_functions == null)
            {
                ud.registered_functions = new LuaTable(this);
            }
            ud.registered_functions["new"] = func;
        }


        public bool CompareRef(int ref1, int ref2)
        {
            int top = LuaNative.lua_gettop(L);
            LuaNative.lua_getref(L, ref1);
            LuaNative.lua_getref(L, ref2);
            bool is_equal = LuaNative.lua_equal(L, -1, -2) != 0;
            LuaNative.lua_settop(L, top);
            return is_equal;
        }

        public void DisposeRef(int r, bool finalized)
        {
            if (!finalized)
            {
                LuaNative.lua_unref(L, r);
            }
        }

        public LuaError DoString(string s)
        {
            return (LuaError)LuaNative.lua_dostring(L, s);
        }

        public LuaError DoBuffer(byte[] b)
        {
            return (LuaError)LuaNative.lua_dobuffer(L, b, (nuint)b.Length, "buffer");
        }

        // regular dofile can only execute files from filesystem
        // this override also allows executing files from PAK archives
        // only files from game directory (and subdirectories) can be executed this way
        public LuaError DoFile(string fpath)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFLua, $"Lua.DoFile(\"{fpath}\") called");

            // check if file exists
            string filename = SFUnPak.SFUnPak.game_directory_name;
            if (filename == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "Lua.DoFile(): Game directory not found!");
                return LuaError.FILE;
            }

            string filepath = $"{filename}\\{fpath}";
            if (File.Exists(filepath))
            {
                return (LuaError)LuaNative.lua_dofile(L, filepath);
            }

            if(!SFUnPak.SFUnPak.pak_map.filename_to_pak.TryGetValue("sf34.pak", out int pfi))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "Lua.DoFile(): Could not find the file");
                return LuaError.FILE;
            }

            byte[] buffer = SFUnPak.SFUnPak.LoadFileFrom(pfi, fpath);
            return DoBuffer(buffer);
        }

        public static Lua GetEnv(IntPtr _L)
        {
            return luastate_to_lua[_L];
        }

        // dofile override that allows executing files from PAK archives
        public static int LUAOVERRIDE_dofile(IntPtr _L)
        {
            Lua env = GetEnv(_L);

            string fpath = (string)env.PopObject();
            int old_top = LuaNative.lua_gettop(_L);
            LuaError err = env.DoFile(fpath);
            if(err != LuaError.OK)
            {
                LuaNative.lua_error(_L, $"Error executing dofile (error code: {err})");
                LuaNative.lua_settop(_L, old_top);
                return 0;
            }
            int diff = LuaNative.lua_gettop(_L) - old_top;
            return diff;
        }

        public static int LUAOVERRIDE_TM_gettable(IntPtr _L)
        {
            Lua env = GetEnv(_L);

            object index = env.PopObject();
            object table = env.PopObject();
            string key = index as string;
            if(key == null)
            {
                LuaNative.lua_error(_L, $"trying to index a field with value that is not a string");
                env.PushObject(null);
                return 1;
            }
            if(!env.obj_to_ref.TryGetValue(table, out CLuaRefCounter cref))
            {
                LuaNative.lua_error(_L, $"trying to index an unknown usertype");
                env.PushObject(null);
                return 1;
            }
            Type t = cref.t;
            CLuaUserdata ud = env.udtype_to_ud[t];

            if(ud.registered_getters.TryGet(key, out object o))
            {
                FieldInfo fi = o as FieldInfo;
                object ret = fi.GetValue(table);
                env.PushObject(ret);
                return 1;
            }
            if(ud.registered_functions.TryGet(key, out object f))
            {
                env.PushObject(f);
                return 1;
            }
            LuaNative.lua_error(_L, $"unknown field {key}");
            env.PushObject(null);
            return 1;
        }

        public static int LUAOVERRIDE_TM_settable(IntPtr _L)
        {
            Lua env = GetEnv(_L);

            object v = env.PopObject();
            object index = env.PopObject();
            object table = env.PopObject();
            string key = index as string;
            if (key == null)
            {
                LuaNative.lua_error(_L, $"trying to index a field with value that is not a string");
                env.PushObject(null);
                return 1;
            }
            if (!env.obj_to_ref.TryGetValue(table, out CLuaRefCounter cref))
            {
                LuaNative.lua_error(_L, $"trying to index an unknown usertype");
                env.PushObject(null);
                return 1;
            }
            Type t = cref.t;
            CLuaUserdata ud = env.udtype_to_ud[t];
            if(ud.registered_setters.TryGet(key, out object o))
            {
                FieldInfo fi = o as FieldInfo;
                object conv = Convert.ChangeType(v, fi.FieldType);
                fi.SetValue(table, conv);
                return 0;
            }
            LuaNative.lua_error(_L, $"unknown field {key}");
            return 0;
        }

        // override for Lua garbage collector that also takes care of unregistering these objects from C# level
        public static int LUAOVERRIDE_TM_gc(IntPtr _L)
        {
            Lua env = GetEnv(_L);

            object table = env.PopObject();
            if (!env.obj_to_ref.TryGetValue(table, out CLuaRefCounter cref))
            {
                LuaNative.lua_error(_L, $"trying to garbage collect an unknown usertype");
                env.PushObject(null);
                return 1;
            }
            env.obj_to_ref.Remove(table);
            env.regobj_to_objidx.Remove(cref.obj);
            env.objidx_to_regobj.Remove(cref.idx);
            cref.obj.Free();

            return 0;
        }

        // this allows calling C# functions from Lua using MethodInfo, as long as none of the arguments are MethodInfo themselves
        public static int LUAOVERRIDE_TM_function_methodwrapper(IntPtr _L)
        {
            Lua env = GetEnv(_L);

            List<object> args = new();
            while(true)
            {
                object o = env.PopObject();
                if(o is MethodInfo)
                {
                    object caller = null;
                    MethodInfo wrapper = o as MethodInfo;
                    ParameterInfo[] pi = wrapper.GetParameters();
                    if(!wrapper.IsStatic)
                    {
                        caller = args[args.Count - 1];
                        args.RemoveAt(args.Count - 1);
                    }
                    args.Reverse();
                    for(int i = 0; i < args.Count; i++)
                    {
                        args[i] = Convert.ChangeType(args[i], pi[i].ParameterType);
                    }
                    object ret = wrapper.Invoke(caller, args.ToArray());
                    if(wrapper.ReturnType != typeof(void))
                    {
                        env.PushObject(ret);
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                args.Add(o);
            }
        }
    }
}
