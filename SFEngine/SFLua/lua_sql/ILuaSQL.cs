using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFLua.lua_sql
{
    // returns error code
    public interface ILuaSQL
    {
        int Load();
        int Save();
        void Unload();
    }
}
