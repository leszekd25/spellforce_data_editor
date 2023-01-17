using System.IO;

namespace SFEngine.SFLua.LuaDecompiler
{
    public interface ILValue : INode
    {

    }

    public class Identifier : Node, ILValue, IRValue
    {
        public string name;

        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write(name);
        }

        public override string ToString()
        {
            return "ID | " + name;
        }
    }

    public class IndexedIdentifier : Node, ILValue, IRValue
    {
        public ILValue name;
        public IRValue index;

        public override void WriteLuaString(StringWriter sw)
        {
            name.WriteLuaString(sw);
            sw.Write("[");
            index.WriteLuaString(sw);
            sw.Write("]");
        }

        public override string ToString()
        {
            return "ID[x]";
        }
    }

    public class DottedIdentifier : Node, ILValue, IRValue
    {
        public ILValue instance;
        public ILValue member;

        public override void WriteLuaString(StringWriter sw)
        {
            instance.WriteLuaString(sw);
            sw.Write(".");
            member.WriteLuaString(sw);
        }

        public override string ToString()
        {
            return "ID.x";
        }
    }

    public class SelfIdentifier : Node, ILValue, IRValue
    {
        public ILValue instance;
        public ILValue method;

        public override void WriteLuaString(StringWriter sw)
        {
            instance.WriteLuaString(sw);
            sw.Write(":");
            method.WriteLuaString(sw);
        }

        public override string ToString()
        {
            return "ID:x()";
        }
    }

    public class UpIdentifier : Node, ILValue, IRValue
    {
        public string name;

        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write("%" + name);
        }

        public override string ToString()
        {
            return "%ID | " + name;
        }
    }

    public class LocalIdentifier : Node, ILValue, IRValue
    {
        public string name;

        public override void WriteLuaString(StringWriter sw)
        {
            sw.Write("local " + name);
        }

        public override string ToString()
        {
            return "LOCAL ID | " + name;
        }
    }
}
