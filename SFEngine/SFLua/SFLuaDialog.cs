using System.Collections.Generic;

namespace SFEngine.SFLua
{
    public struct SFLuaSayData
    {
        public string Tag;
        public string Str;
    }

    public struct SFLuaAnswerData
    {
        public string Tag;
        public string Str;
        public int Id;
    }

    public class SFLuaDialog
    {
        public string Conditions = "";
        public string Actions = "";
        public int Id = -1;
        public SFLuaSayData Say;
        public List<SFLuaAnswerData> Answers { get; } = new List<SFLuaAnswerData>();
        public bool EndDialog = false;
    }
}
