using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SpellforceDataEditor.SFLua.LuaDecompiler
{
    public struct LuaLocVar
    {
        public string name;
        public int start;
        public int end;
    }

    static public class LuaBinaryHeader
    {
        static byte ID_Chunk = 27;
        static string Signature = "Lua";
        static byte LuaVersion = 0x40;
        static long LuaNumberFormat = (long)(Math.PI * 1E8);
        static public void Validate(BinaryReader br)
        {
            if (!LoadIDChunk(br))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaBinaryHeader(): Invalid file header (ID_CHUNK != 27)");
                throw new InvalidDataException();
            }
            if (!LoadSignature(br))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaBinaryHeader(): Invalid file header (sentinel value 'Lua' not found)");
                throw new InvalidDataException();
            }
            if (!LoadVersion(br))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaBinaryHeader(): Invalid file header (version != 0x40)");
                throw new InvalidDataException();
            }
            bool swap = LoadSwap(br);
            LoadDataSizes(br);
            if (!LoadNumberFormat(br))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaBinaryHeader(): Invalid file header (number formats do not match)");
                throw new InvalidDataException();
            }
        }

        static public bool LoadIDChunk(BinaryReader br)
        {
            return (br.ReadByte() == ID_Chunk);
        }

        static public bool LoadSignature(BinaryReader br)
        {
            for (int i = 0; i < 3; i++)
                if (br.ReadChar() != Signature[i])
                    return false;
            return true;
        }

        static public bool LoadVersion(BinaryReader br)
        {
            return (br.ReadByte() == LuaVersion);
        }

        // dummy function
        static public bool LoadSwap(BinaryReader br)
        {
            br.ReadByte();
            return false;
        }

        // dummy function
        static public void LoadDataSizes(BinaryReader br)
        {
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
        }

        static public bool LoadNumberFormat(BinaryReader br)
        {
            return (long)br.ReadDouble() == LuaNumberFormat;
        }
    }

    public class LuaBinaryFunction
    {
        public string Source = "";
        public int LineDefined;
        public int NumParams;
        public bool IsVarArg;
        public int MaxStackSize;

        public List<LuaLocVar> LocVars;
        public List<int> LinesInfo;
        public List<double> Numbers;
        public List<string> Strings;
        public List<LuaBinaryFunction> Functions;
        public List<LuaInstruction> Instructions;

        public LuaBinaryFunction(BinaryReader br)
        {
            Source = LoadString(br);
            LineDefined = br.ReadInt32();
            NumParams = br.ReadInt32();
            IsVarArg = (br.ReadByte() != 0);
            MaxStackSize = br.ReadInt32();

            LoadLocals(br);
            LoadLines(br);
            LoadConstants(br);
            if(!LoadCode(br))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaBinaryFunction(): Invalid code chunk (last instruction != OP_END)");
                throw new InvalidDataException();
            }
        }

        public string LoadString(BinaryReader br)
        {
            int size = br.ReadInt32();
            if (size == 0)
                return "";
            string s = new String(br.ReadChars(size - 1));
            /*for (int i = 0; i < size - 1; i++)
                s += (char)br.ReadByte();*/
            br.ReadByte();
            return s;
        }

        public void LoadLocals(BinaryReader br)
        {
            int n = br.ReadInt32();
            LocVars = new List<LuaLocVar>();
            for(int i=0;i<n;i++)
            {
                LuaLocVar v;
                v.name = LoadString(br);
                v.start = br.ReadInt32();
                v.end = br.ReadInt32();
                LocVars.Add(v);
            }
        }

        public void LoadLines(BinaryReader br)
        {
            int n = br.ReadInt32();
            LinesInfo = new List<int>();
            for(int i=0;i<n;i++)
            {
                LinesInfo.Add(br.ReadInt32());
            }
        }

        public void LoadConstants(BinaryReader br)
        {
            int n = br.ReadInt32();
            Strings = new List<string>();
            for (int i = 0; i < n; i++)
            {
                //System.Diagnostics.Debug.WriteLine(i.ToString());
                Strings.Add(LoadString(br));
            }

            n = br.ReadInt32();
            Numbers = new List<double>();
            for (int i = 0; i < n; i++)
                Numbers.Add(br.ReadDouble());

            n = br.ReadInt32();
            Functions = new List<LuaBinaryFunction>();
            for (int i = 0; i < n; i++)
                Functions.Add(new LuaBinaryFunction(br));
        }

        public bool LoadCode(BinaryReader br)
        {
            int n = br.ReadInt32();
            Instructions = new List<LuaInstruction>();
            for (int i = 0; i < n; i++)
                Instructions.Add(new LuaInstruction(br.ReadUInt32()));
            if (Instructions[Instructions.Count - 1].OpCode != LuaOpCode.OP_END)
                return false;

            return true;
        }

        // doomp eet
        public void DumpAll()
        {
            FileStream fs = new FileStream("func_dump.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            Dump(sw);

            sw.Close();
        }

        public void Dump(StreamWriter sw)
        {
            sw.WriteLine("NUMBERS DUMP");
            for (int i = 0; i < Numbers.Count; i++)
                sw.WriteLine("NUMBER #" + i.ToString() + ": " + Numbers[i].ToString());
            sw.WriteLine("");
            sw.WriteLine("STRINGS DUMP");
            for (int i = 0; i < Strings.Count; i++)
                sw.WriteLine("STRING #" + i.ToString() + ": " + Strings[i].ToString());
            sw.WriteLine("");
            sw.WriteLine("INSTR   DUMP");
            for (int i = 0; i < Instructions.Count; i++)
                sw.WriteLine("INSTR  #" + i.ToString() + ": " + Instructions[i].ToString());
            sw.WriteLine();
            sw.WriteLine();
            foreach (LuaBinaryFunction f in Functions)
                f.Dump(sw);
        }

        // minimum required to run LUA scripts
        public object[] Execute(LuaStack stack)
        {
            int start_pos = stack.Pos;
            Dictionary<object, object> locals = new Dictionary<object, object>();
            List<object> ret = new List<object>();
            bool done = false;

            int list_elems;
            LuaParser.LuaTable table;

            foreach (LuaInstruction i in Instructions)
            {
                switch(i.OpCode)
                {
                    case LuaOpCode.OP_END:
                        done = true;
                        break;
                    case LuaOpCode.OP_RETURN:                   // todo: rework
                        done = true;
                        int stack_return_count = i.ArgU;
                        for(int k=0; k<=stack_return_count; k++)
                            ret.Add(stack.Pop());
                        break;
                    case LuaOpCode.OP_PUSHNIL:
                        stack.Push(null);
                        break;
                    case LuaOpCode.OP_PUSHINT:
                        stack.Push((double)i.ArgS);
                        break;
                    case LuaOpCode.OP_PUSHSTRING:
                        stack.Push(Strings[i.ArgU]);
                        break;
                    case LuaOpCode.OP_PUSHNUM:
                        stack.Push(Numbers[i.ArgU]);
                        break;
                    case LuaOpCode.OP_GETGLOBAL:                   // todo: rework
                        stack.Push(Strings[i.ArgU]);
                        break;
                    case LuaOpCode.OP_CREATETABLE:
                        stack.Push(new LuaParser.LuaTable());
                        break;
                    case LuaOpCode.OP_SETLIST:
                        list_elems = i.ArgB;
                        table = (LuaParser.LuaTable)stack.Get(list_elems);
                        for(;list_elems!=0; list_elems-=1)
                            table[(double)list_elems] = stack.Pop();
                        break;
                    case LuaOpCode.OP_SETMAP:
                        list_elems = i.ArgU;
                        table = (LuaParser.LuaTable)stack.Get(list_elems * 2);
                        for(;list_elems!=0;list_elems-=1)
                        {
                            object val = stack.Pop();
                            object key = stack.Pop();
                            table[key] = val;
                        }
                        break;
                    default:
                        LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaFunction.Execute(): Unsupported opcode " + i.OpCode.ToString());
                        return null;
                }
                if (done)
                    break;
            }
            if (ret.Count == 0)
                return null;
            return ret.ToArray();
        }
    }

    public class LuaBinaryScript
    {
        public LuaBinaryFunction func = null;

        public LuaBinaryScript(BinaryReader br)
        {
            try
            {
                LuaBinaryHeader.Validate(br);
                func = new LuaBinaryFunction(br);
            }
            catch(Exception e)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaBinaryScript(): Error reading script from memory");
                func = null;
            }
        }
    }
}
