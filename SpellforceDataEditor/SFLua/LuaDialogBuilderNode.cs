using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFLua
{
    public enum LuaDialogBuilderNodeType { SAY = 0, ANSWER = 1, COMMON = 2, END = 3 }
    public class LuaDialogBuilderNode
    {
        public LuaDialogBuilderNodeType type = LuaDialogBuilderNodeType.COMMON;
        public string tag = "";
        public string text = "";
        public int id = -1;
        public string conditions = "";
        public string actions = "";

        public List<LuaDialogBuilderNode> previous { get; } = new List<LuaDialogBuilderNode>();
        public List<LuaDialogBuilderNode> next { get; } = new List<LuaDialogBuilderNode>();
    }
}
