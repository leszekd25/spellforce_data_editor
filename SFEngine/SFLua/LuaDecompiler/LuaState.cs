namespace SFEngine.SFLua.LuaDecompiler
{
    public class LuaState
    {
        LuaStack stack = new LuaStack();

        public object[] Run(LuaBinaryScript scr)
        {
            return scr.func.Execute(stack);
        }
    }
}
