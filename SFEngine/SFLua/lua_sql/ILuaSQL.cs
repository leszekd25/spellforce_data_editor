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
