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

    public class LuaBinaryHeader
    {
        static byte ID_Chunk = 27;
        static string Signature = "Lua";
        static byte LuaVersion = 0x40;
        static long LuaNumberFormat = (long)(Math.PI * 1E8);
        public LuaBinaryHeader(BinaryReader br)
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

        public bool LoadIDChunk(BinaryReader br)
        {
            return (br.ReadByte() == ID_Chunk);
        }

        public bool LoadSignature(BinaryReader br)
        {
            for (int i = 0; i < 3; i++)
                if (br.ReadChar() != Signature[i])
                    return false;
            return true;
        }

        public bool LoadVersion(BinaryReader br)
        {
            return (br.ReadByte() == LuaVersion);
        }

        // dummy function
        public bool LoadSwap(BinaryReader br)
        {
            br.ReadByte();
            return false;
        }

        // dummy function
        public void LoadDataSizes(BinaryReader br)
        {
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
        }

        public bool LoadNumberFormat(BinaryReader br)
        {
            return (long)br.ReadDouble() == LuaNumberFormat;
        }
    }

    public class LuaBinaryFunction
    {
        string Source = "";
        int LineDefined;
        int NumParams;
        bool IsVarArg;
        int MaxStackSize;

        List<LuaLocVar> LocVars;
        List<int> LinesInfo;
        List<double> Numbers;
        List<string> Strings;
        List<LuaBinaryFunction> Functions;
        List<LuaInstruction> Instructions;

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
            string s = "";
            for (int i = 0; i < size - 1; i++)
                s += (char)br.ReadByte();
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

            string func_dump = "";
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

            sw.Close();
        }

        public object[] Execute(LuaState lua)
        {
            int start_pos = lua.stack_position;
            Dictionary<object, object> locals = new Dictionary<object, object>();
            List<object> ret = new List<object>();
            bool done = false;

            int list_elems;
            LuaParser.LuaTable table;
            int table_stack_pos;

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
                        {
                            ret.Add(lua.stack[lua.stack_position]);
                            lua.stack_position -= 1;
                        }
                        break;
                    case LuaOpCode.OP_PUSHNIL:
                        lua.stack_position += 1;
                        lua.stack.Add(null);
                        break;
                    case LuaOpCode.OP_PUSHINT:
                        lua.stack_position += 1;
                        lua.stack.Add((double)i.ArgS);
                        break;
                    case LuaOpCode.OP_PUSHSTRING:
                        lua.stack_position += 1;
                        lua.stack.Add(Strings[i.ArgU]);
                        break;
                    case LuaOpCode.OP_PUSHNUM:
                        lua.stack_position += 1;
                        lua.stack.Add(Numbers[i.ArgU]);
                        break;
                    case LuaOpCode.OP_GETGLOBAL:                   // todo: rework
                        lua.stack_position += 1;
                        lua.stack.Add(Strings[i.ArgU]);
                        break;
                    case LuaOpCode.OP_CREATETABLE:
                        lua.stack_position += 1;
                        lua.stack.Add(new LuaParser.LuaTable());
                        break;
                    case LuaOpCode.OP_SETLIST:
                        list_elems = i.ArgB;
                        table_stack_pos = lua.stack_position - list_elems;
                        table = (LuaParser.LuaTable)(lua.stack[table_stack_pos]);
                        while (list_elems != 0)
                        {
                            table[(double)list_elems] = lua.stack[table_stack_pos + list_elems];
                            lua.stack.RemoveAt(table_stack_pos + list_elems);
                            lua.stack_position -= 1;
                            list_elems -= 1;
                        }
                        break;
                    case LuaOpCode.OP_SETMAP:
                        list_elems = i.ArgU;
                        table_stack_pos = lua.stack_position - 2 * list_elems;
                        table = (LuaParser.LuaTable)(lua.stack[table_stack_pos]);
                        while(list_elems != 0)
                        {
                            table[lua.stack[table_stack_pos + list_elems * 2 - 1]] = lua.stack[table_stack_pos + list_elems * 2];
                            lua.stack.RemoveAt(table_stack_pos + list_elems * 2);
                            lua.stack.RemoveAt(table_stack_pos + list_elems * 2 - 1);
                            lua.stack_position -= 2;
                            list_elems -= 1;
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
        public LuaBinaryHeader header = null;
        public LuaBinaryFunction func = null;

        public LuaBinaryScript(BinaryReader br)
        {
            try
            {
                header = new LuaBinaryHeader(br);
                func = new LuaBinaryFunction(br);
            }
            catch(Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaBinaryScript(): Error reading script from memory");
                header = null;
                func = null;
            }
            func.DumpAll();
        }
    }
}
