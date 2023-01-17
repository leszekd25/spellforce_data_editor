namespace SFEngine.SFLua
{
    public interface ILuaParsable
    {
        void ParseLoad(LuaParser.LuaTable table);
        string ParseToString();
    }
}
