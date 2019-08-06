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

        public string GetLocal(int i)
        {
            if (i < NumParams)
                return "__arg" + i.ToString();
            else
                return "__local" + (i - NumParams).ToString();
        }

        public string GetUpValue(int i)    // those are readonly, so its gucci
        {
            return "__upvalue" + i.ToString();
        }

        public DecompileNode GetLocalValue(LuaStack stack, int b, int i)
        {
            return (DecompileNode)stack.Get(stack.Pos - b - i);
        }

        private string VisualiseDecompileStack(LuaStack stack)
        {
            string ret = "";
            for(int i=0;i<=stack.Pos; i++)
            {
                DecompileNode n = (DecompileNode)stack.Get(i);
                char c = ' ';
                switch(n.nodetype)
                {
                    case DecompileNodeType.CHUNK:
                        c = '#';
                        break;
                    case DecompileNodeType.COND:
                        c = 'C';
                        break;
                    case DecompileNodeType.FOREACHLOOP:
                        c = 'L';
                        break;
                    case DecompileNodeType.FORLOOP:
                        c = 'l';
                        break;
                    case DecompileNodeType.FUNCDEF:
                        c = 'F';
                        break;
                    case DecompileNodeType.FUNCTION:
                        c = 'f';
                        break;
                    case DecompileNodeType.IDENTIFIER:
                        c = 'I';
                        break;
                    case DecompileNodeType.INDEXED_VALUE:
                        c = 'X';
                        break;
                    case DecompileNodeType.MULTOPERATOR:
                        c = 'O';
                        break;
                    case DecompileNodeType.RETURN:
                        c = 'R';
                        break;
                    case DecompileNodeType.TABLE:
                        c = 'T';
                        break;
                    case DecompileNodeType.UNOPERATOR:
                        c = 'o';
                        break;
                    case DecompileNodeType.VALUE:
                        c = 'v';
                        break;
                    case DecompileNodeType.VARIABLE:
                        c = 'V';
                        break;
                    case DecompileNodeType.WHILE:
                        c = 'W';
                        break;
                    default:
                        break;
                }
                ret = c + ret;
            }
            return ret;
        }

        public string VisualiseNode(DecompileNode n)
        {
            string ret = "";
            for (int i = 0; i < n.Children.Count; i++)
            {
                DecompileNode n2 = n.Children[i];
                char c = ' ';
                switch (n2.nodetype)
                {
                    case DecompileNodeType.CHUNK:
                        c = '#';
                        break;
                    case DecompileNodeType.COND:
                        c = 'C';
                        break;
                    case DecompileNodeType.FOREACHLOOP:
                        c = 'L';
                        break;
                    case DecompileNodeType.FORLOOP:
                        c = 'l';
                        break;
                    case DecompileNodeType.FUNCDEF:
                        c = 'F';
                        break;
                    case DecompileNodeType.FUNCTION:
                        c = 'f';
                        break;
                    case DecompileNodeType.IDENTIFIER:
                        c = 'I';
                        break;
                    case DecompileNodeType.INDEXED_VALUE:
                        c = 'X';
                        break;
                    case DecompileNodeType.MULTOPERATOR:
                        c = 'O';
                        break;
                    case DecompileNodeType.RETURN:
                        c = 'R';
                        break;
                    case DecompileNodeType.TABLE:
                        c = 'T';
                        break;
                    case DecompileNodeType.UNOPERATOR:
                        c = 'o';
                        break;
                    case DecompileNodeType.VALUE:
                        c = 'v';
                        break;
                    case DecompileNodeType.VARIABLE:
                        c = 'V';
                        break;
                    case DecompileNodeType.WHILE:
                        c = 'W';
                        break;
                    default:
                        break;
                }
                ret = c + ret;
            }
            return ret;
        }

        enum LabelType { IFEND, ELSE, FOR, FOREACH, ANDOR }

        struct LabelInfo
        {
            public int instr_id;
            public LabelType type;

            public LabelInfo(int i, LabelType t)
            {
                instr_id = i;
                type = t;
            }
        }

        public DecompileNode Decompile(LuaStack stack)
        {
            int start_pos = stack.Pos;
            for (int i = 0; i < NumParams; i++)
                stack.Push(new DecompileNode(-1, DecompileNodeType.IDENTIFIER, DecompileDataType.STRING, GetLocal(i)));
            if (IsVarArg)
                stack.Push(new DecompileNode(-1, DecompileNodeType.IDENTIFIER, DecompileDataType.STRING, "..."));

            bool done = false;
            int local_count = NumParams;

            DecompileNode root = new DecompileNode(-1, DecompileNodeType.CHUNK, DecompileDataType.NIL, null);
            List<LabelInfo> label_stack = new List<LabelInfo>();
            // workaround for X or Y value issue
            List<DecompileNode> andor_operator_helper = new List<DecompileNode>();

            for(int i=0;i<Instructions.Count;i++)
            {
                LuaInstruction instr = Instructions[i];
                if (label_stack.Count != 0)
                {
                    while (i == label_stack[label_stack.Count - 1].instr_id)
                    {
                        if (label_stack[label_stack.Count - 1].type == LabelType.ANDOR)
                        {
                            andor_operator_helper[andor_operator_helper.Count-1].AddNode((DecompileNode)stack.Pop());
                            stack.Push(andor_operator_helper[andor_operator_helper.Count-1]);
                            andor_operator_helper.RemoveAt(andor_operator_helper.Count-1);
                        }
                        else if (label_stack[label_stack.Count - 1].type == LabelType.IFEND)
                        {
                            root = root.Parent.Parent;
                        }
                        else if (label_stack[label_stack.Count - 1].type == LabelType.FOR)
                        {
                            DecompileNode forloop_for = root.Parent;
                            root = forloop_for.Parent;
                            local_count -= 3;
                            DecompileNode forloop_step = (DecompileNode)stack.Pop();
                            DecompileNode forloop_to = (DecompileNode)stack.Pop();
                            DecompileNode forloop_from = (DecompileNode)stack.Pop();
                            DecompileNode forloop_localnum = new DecompileNode(i, DecompileNodeType.VALUE, DecompileDataType.STRING,
                                GetLocal(local_count));
                            DecompileNode forloop_table = new DecompileNode(i, DecompileNodeType.TABLE, DecompileDataType.LIST, null);
                            forloop_table.AddNode(forloop_from);
                            forloop_table.AddNode(forloop_to);
                            forloop_table.AddNode(forloop_step);
                            forloop_table.AddNode(forloop_localnum);
                            forloop_for.data = forloop_table;
                        }
                        else if (label_stack[label_stack.Count - 1].type == LabelType.FOREACH)
                        {
                            DecompileNode lforloop_for = root.Parent;
                            root = lforloop_for.Parent;
                            local_count -= 3;
                            DecompileNode lforloop_val = (DecompileNode)stack.Pop();
                            DecompileNode lforloop_key = (DecompileNode)stack.Pop();
                            DecompileNode lforloop_table = (DecompileNode)stack.Pop();
                            lforloop_table.AddNode(lforloop_key);
                            lforloop_table.AddNode(lforloop_val);
                            lforloop_for.data = lforloop_table;
                        }
                        else 
                        {
                            LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaBinaryScript.Decompile(): Unknown error (wrong label type)!");
                        }

                        label_stack.RemoveAt(label_stack.Count - 1);
                        if (label_stack.Count == 0)
                            break;
                    }
                }

                //System.Diagnostics.Debug.WriteLine(" | STACK: " + VisualiseDecompileStack(stack).PadRight(40)+" | ROOT: "+VisualiseNode(root));
                //System.Diagnostics.Debug.WriteLine(i.ToString().PadRight(4)+" | "+instr.ToString().PadRight(20));

                switch (instr.OpCode)
                {
                    case LuaOpCode.OP_END:
                        done = true;
                        break;
                    case LuaOpCode.OP_RETURN:
                        int ret_base = instr.ArgU;
                        DecompileNode ret_n = new DecompileNode(i, DecompileNodeType.RETURN);
                        while (stack.Pos > start_pos + ret_base)
                            ret_n.AddNode((DecompileNode)stack.Pop());
                        root.AddNode(ret_n);
                        break;
                    case LuaOpCode.OP_CALL:
                        int stack_base = instr.ArgA;
                        DecompileNode call_n = new DecompileNode(i, DecompileNodeType.FUNCTION);

                        while(stack.Pos > stack_base+start_pos+1)
                            call_n.AddNode((DecompileNode)stack.Pop(), 0);

                        call_n.data = stack.Pop();

                        if (instr.ArgB == 0)
                            root.AddNode(call_n);
                        else if((instr.ArgB == 1)||(instr.ArgB == 255))
                            stack.Push(call_n);
                        else     // temporary workaround for multireturn values
                        {
                            for(int j=0;j<instr.ArgB; j++)
                            {
                                DecompileNode call_multiret = new DecompileNode(i, DecompileNodeType.INDEXED_VALUE, DecompileDataType.STRING,
                                    call_n);
                                call_multiret.AddNode(new DecompileNode(i, DecompileNodeType.VALUE, DecompileDataType.NUMBER, j));
                                stack.Push(call_multiret);
                            }
                        }

                        break;
                    case LuaOpCode.OP_TAILCALL:
                        int tailcall_ret_base = instr.ArgB;
                        int tailcall_call_base = instr.ArgA;
                        DecompileNode tailcall_n = new DecompileNode(i, DecompileNodeType.RETURN);
                        DecompileNode tailcall_call = new DecompileNode(i, DecompileNodeType.FUNCTION);
                        while (stack.Pos > tailcall_call_base + start_pos + 1)
                            tailcall_call.AddNode((DecompileNode)stack.Pop(), 0);
                        tailcall_call.data = stack.Pop();
                        stack.Push(tailcall_call);
                        while (stack.Pos > start_pos + tailcall_ret_base)
                            tailcall_n.AddNode((DecompileNode)stack.Pop());
                        root.AddNode(tailcall_n);
                        break;
                    case LuaOpCode.OP_PUSHNIL:
                        for(int k=0;k<instr.ArgU; k++)
                            stack.Push(new DecompileNode(i, DecompileNodeType.VALUE, DecompileDataType.NIL, null));
                        break;
                    case LuaOpCode.OP_PUSHINT:
                        stack.Push(new DecompileNode(i, DecompileNodeType.VALUE, DecompileDataType.NUMBER, instr.ArgS));
                        break;
                    case LuaOpCode.OP_PUSHSTRING:
                        stack.Push(new DecompileNode(i, DecompileNodeType.VALUE, DecompileDataType.STRING, Strings[instr.ArgU]));
                        break;
                    case LuaOpCode.OP_PUSHNUM:
                        stack.Push(new DecompileNode(i, DecompileNodeType.VALUE, DecompileDataType.NUMBER, Numbers[instr.ArgU]));
                        break;
                    case LuaOpCode.OP_PUSHNEGNUM:
                        stack.Push(new DecompileNode(i, DecompileNodeType.VALUE, DecompileDataType.NUMBER, -Numbers[instr.ArgU]));
                        break;
                    case LuaOpCode.OP_PUSHSELF:
                        DecompileNode pushself_n = (DecompileNode)stack.Pop();
                        DecompileNode pushself_n2 = new DecompileNode(i, DecompileNodeType.IDENTIFIER, DecompileDataType.STRING,
                            pushself_n.data.ToString() + ":" + Strings[instr.ArgU]);
                        stack.Push(pushself_n2);
                        break;
                    case LuaOpCode.OP_PUSHUPVALUE:
                        stack.Push(new DecompileNode(i, DecompileNodeType.UPIDENTIFIER, DecompileDataType.STRING, GetUpValue(instr.ArgU)));
                        break;
                    case LuaOpCode.OP_GETGLOBAL:                   // todo: rework
                        stack.Push(new DecompileNode(i, DecompileNodeType.IDENTIFIER, DecompileDataType.STRING, Strings[instr.ArgU]));
                        break;
                    case LuaOpCode.OP_GETLOCAL:
                        stack.Push(new DecompileNode(i, DecompileNodeType.IDENTIFIER, DecompileDataType.STRING, GetLocal(instr.ArgU)));
                        break;
                    case LuaOpCode.OP_GETTABLE:
                        DecompileNode gettable_key = (DecompileNode)stack.Pop();
                        DecompileNode gettable_table = (DecompileNode)stack.Pop();
                        DecompileNode gettable_var = new DecompileNode(i, DecompileNodeType.INDEXED_VALUE, DecompileDataType.STRING,
                            gettable_table.data);
                        gettable_var.AddNode(gettable_key);
                        stack.Push(gettable_var);
                        break;
                    case LuaOpCode.OP_GETINDEXED:
                        DecompileNode getindexed_key = new DecompileNode(i, DecompileNodeType.IDENTIFIER, DecompileDataType.STRING,
                            GetLocal(instr.ArgU));
                        DecompileNode getindexed_table = (DecompileNode)stack.Pop();
                        DecompileNode getindexed_var = new DecompileNode(i, DecompileNodeType.INDEXED_VALUE, DecompileDataType.STRING,
                            getindexed_table);
                        getindexed_var.AddNode(getindexed_key);
                        stack.Push(getindexed_var);
                        break;
                    case LuaOpCode.OP_GETDOTTED:
                        DecompileNode getdotted_n = (DecompileNode)stack.Pop();
                        DecompileNode getdotted_n2 = new DecompileNode(i, DecompileNodeType.IDENTIFIER, DecompileDataType.STRING,
                            getdotted_n.data.ToString() + "." + Strings[instr.ArgU]);
                        stack.Push(getdotted_n2);
                        break;
                    case LuaOpCode.OP_CREATETABLE:
                        stack.Push(new DecompileNode(i, DecompileNodeType.TABLE, DecompileDataType.LIST, instr.ArgU));
                        break;
                    case LuaOpCode.OP_SETGLOBAL:                   // set global value to last thing on stack
                        DecompileNode setglobal_name = new DecompileNode(i, DecompileNodeType.IDENTIFIER, DecompileDataType.STRING,
                            Strings[instr.ArgU]);
                        DecompileNode setglobal_n = new DecompileNode(i, DecompileNodeType.VARIABLE, DecompileDataType.USERDATA,
                            setglobal_name);
                        DecompileNode setglobal_n2 = (DecompileNode)stack.Pop();
                        if (setglobal_n2.nodetype == DecompileNodeType.FUNCDEF)
                        {
                            setglobal_n2.data = setglobal_n.data;
                            setglobal_n = setglobal_n2;
                        }
                        else
                            setglobal_n.AddNode(setglobal_n2);
                        root.AddNode(setglobal_n);                 // misses many valid use cases...
                        break;
                    case LuaOpCode.OP_SETLOCAL:
                        DecompileNode setlocal_name = new DecompileNode(i, DecompileNodeType.IDENTIFIER, DecompileDataType.STRING,
                            GetLocal(instr.ArgU));
                        DecompileNode setlocal_n = new DecompileNode(i, DecompileNodeType.VARIABLE, DecompileDataType.USERDATA,
                            setlocal_name);
                        DecompileNode setlocal_n2 = (DecompileNode)stack.Pop();
                        if (setlocal_n2.nodetype == DecompileNodeType.FUNCDEF)
                        {
                            setlocal_n2.data = setlocal_n.data;
                            setlocal_n = setlocal_n2;
                        }
                        else
                            setlocal_n.AddNode(setlocal_n2);
                        root.AddNode(setlocal_n);                 // misses many valid use cases...
                        break;
                    case LuaOpCode.OP_SETTABLE:
                        DecompileNode settable_val = (DecompileNode)stack.Pop();
                        DecompileNode settable_key = (DecompileNode)stack.Pop();
                        DecompileNode settable_table = (DecompileNode)stack.Pop();

                        DecompileNode settable_ind = new DecompileNode(i, DecompileNodeType.INDEXED_VALUE, DecompileDataType.USERDATA,
                            settable_table.data);
                        settable_ind.AddNode(settable_key);

                        DecompileNode settable_var = new DecompileNode(i, DecompileNodeType.VARIABLE, DecompileDataType.USERDATA, settable_ind);
                        settable_var.AddNode(settable_val);

                        root.AddNode(settable_var);
                        break;
                    case LuaOpCode.OP_SETMAP:
                        int elems_d_count = instr.ArgU*2;
                        DecompileNode setmap_n = (DecompileNode)stack.Get(elems_d_count);
                        int setmap_cur_elems = setmap_n.Children.Count;
                        setmap_n.datatype = DecompileDataType.DICT;
                        while(elems_d_count != 0)
                        {
                            DecompileNode val = (DecompileNode)stack.Pop();
                            DecompileNode key = (DecompileNode)stack.Pop();
                            DecompileNode var = new DecompileNode(i, DecompileNodeType.VARIABLE, DecompileDataType.USERDATA, key);
                            var.AddNode(val);
                            setmap_n.AddNode(var, setmap_cur_elems);
                            elems_d_count -= 2;
                        }
                        break;
                    case LuaOpCode.OP_SETLIST:
                        int elems_l_count = instr.ArgB;
                        DecompileNode setlist_n = (DecompileNode)stack.Get(elems_l_count);
                        int setlist_cur_elems = setlist_n.Children.Count;
                        while (elems_l_count != 0)
                        {
                            DecompileNode val = (DecompileNode)stack.Pop();
                            DecompileNode var = new DecompileNode(i, DecompileNodeType.VARIABLE, DecompileDataType.NIL, null);
                            var.AddNode(val);
                            setlist_n.AddNode(var, setlist_cur_elems);
                            elems_l_count -= 1;
                        }
                        break;
                    case LuaOpCode.OP_ADD:
                        DecompileNode add_n = (DecompileNode)stack.Pop();
                        DecompileNode add_n2 = (DecompileNode)stack.Pop();
                        DecompileNode add_n3 = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, "+");
                        add_n3.AddNode(add_n2);
                        add_n3.AddNode(add_n);
                        stack.Push(add_n3);
                        break;
                    case LuaOpCode.OP_ADDI:
                        DecompileNode addi_n = (DecompileNode)stack.Pop();
                        int addi_val = instr.ArgS;
                        bool addi_positive = addi_val >= 0;
                        if (!addi_positive)
                            addi_val = -addi_val;
                        DecompileNode addi_n2 = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING,
                            addi_positive ? "+" : "-");
                        addi_n2.AddNode(addi_n);
                        addi_n2.AddNode(new DecompileNode(i, DecompileNodeType.VALUE, DecompileDataType.NUMBER, addi_val));
                        stack.Push(addi_n2);
                        break;
                    case LuaOpCode.OP_SUB:
                        DecompileNode sub_n = (DecompileNode)stack.Pop();
                        DecompileNode sub_n2 = (DecompileNode)stack.Pop();
                        DecompileNode sub_n3 = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, "-");
                        sub_n3.AddNode(sub_n2);
                        sub_n3.AddNode(sub_n);
                        stack.Push(sub_n3);
                        break;
                    case LuaOpCode.OP_MULT:
                        DecompileNode mult_n = (DecompileNode)stack.Pop();
                        DecompileNode mult_n2 = (DecompileNode)stack.Pop();
                        DecompileNode mult_n3 = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, "*");
                        mult_n3.AddNode(mult_n2);
                        mult_n3.AddNode(mult_n);
                        stack.Push(mult_n3);
                        break;
                    case LuaOpCode.OP_DIV:
                        DecompileNode div_n = (DecompileNode)stack.Pop();
                        DecompileNode div_n2 = (DecompileNode)stack.Pop();
                        DecompileNode div_n3 = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, "/");
                        div_n3.AddNode(div_n2);
                        div_n3.AddNode(div_n);
                        stack.Push(div_n3);
                        break;
                    case LuaOpCode.OP_POW:
                        DecompileNode pow_n = (DecompileNode)stack.Pop();
                        DecompileNode pow_n2 = (DecompileNode)stack.Pop();
                        DecompileNode pow_n3 = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, "^");
                        pow_n3.AddNode(pow_n2);
                        pow_n3.AddNode(pow_n);
                        stack.Push(pow_n3);
                        break;
                    case LuaOpCode.OP_POP:
                        for(int k=0;k<instr.ArgU;k++)
                            stack.Pop();
                        break;
                    case LuaOpCode.OP_CONCAT:
                        List<DecompileNode> concat_nodes = new List<DecompileNode>();
                        int nodes_to_pop = instr.ArgU;
                        for (int k = 0; k < nodes_to_pop; k++)
                            concat_nodes.Add((DecompileNode)stack.Pop());
                        DecompileNode concat_n = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, "..");
                        while (concat_nodes.Count != 0)
                        {
                            concat_n.AddNode(concat_nodes[concat_nodes.Count-1]);
                            concat_nodes.RemoveAt(concat_nodes.Count-1);
                        }
                        stack.Push(concat_n);
                        break;
                    case LuaOpCode.OP_MINUS:
                        DecompileNode minus_n = (DecompileNode)stack.Pop();
                        DecompileNode minus_n2 = new DecompileNode(i, DecompileNodeType.UNOPERATOR, DecompileDataType.STRING, "-");
                        minus_n2.AddNode(minus_n);
                        stack.Push(minus_n2);
                        break;
                    case LuaOpCode.OP_NOT:
                        DecompileNode not_n = (DecompileNode)stack.Pop();
                        DecompileNode not_n2 = new DecompileNode(i, DecompileNodeType.UNOPERATOR, DecompileDataType.STRING, "not ");
                        not_n2.AddNode(not_n);
                        stack.Push(not_n2);
                        break;
                    case LuaOpCode.OP_PUSHNILJMP:     // is it good?
                        DecompileNode pushniljump_control = root.Parent;
                        root = pushniljump_control.Parent;
                        root.RemoveNode(pushniljump_control);
                        DecompileNode pushniljump_op = (DecompileNode)pushniljump_control.data;
                        pushniljump_op.ReverseLogicalOperator();
                        stack.Push(pushniljump_op);
                        i++;
                        break;
                    case LuaOpCode.OP_JMP:        // taken care of on start of loop : )
                        if (instr.ArgS < 0)   // while/continue
                        {
                            root.AddNode(new DecompileNode(i, DecompileNodeType.CONTINUELOOP));
                            DecompileNode control_n = root.Parent;
                            if (control_n.nodetype == DecompileNodeType.COND)
                            {
                                control_n.nodetype = DecompileNodeType.WHILE;
                            }
                        }
                        else // else/break
                        {
                            DecompileNode control_n = root.Parent;
                            if(control_n == null)   // if nil then
                            {
                                DecompileNode jmp_n = new DecompileNode(i, DecompileNodeType.VALUE, DecompileDataType.NIL, null);
                                DecompileNode jmp_op = new DecompileNode(i, DecompileNodeType.UNOPERATOR, DecompileDataType.STRING, "");
                                jmp_op.AddNode(jmp_n);
                                DecompileNode jmp_cond = new DecompileNode(i, DecompileNodeType.COND, DecompileDataType.USERDATA, jmp_op);// skip block if false
                                DecompileNode jmp_nojump = new DecompileNode(i, DecompileNodeType.CHUNK);
                                DecompileNode jmp_jump = new DecompileNode(i, DecompileNodeType.CHUNK);
                                root.AddNode(jmp_cond);
                                jmp_cond.AddNode(jmp_nojump);
                                jmp_cond.AddNode(jmp_jump);
                                root = jmp_nojump;
                                if (instr.ArgS < 0)
                                {
                                    LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaFunction.Decompile(): Unsupported jump index "
                                        + instr.ArgS.ToString());
                                    return null;
                                }
                                label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.IFEND));
                                break;
                            }
                            else if (control_n.nodetype == DecompileNodeType.COND)  // else in if-else block
                            {
                                if (root.Children.Count == 0)   // empty if - delete if block
                                {
                                    root = control_n.Parent;
                                    root.RemoveNode(control_n);
                                    label_stack.RemoveAt(label_stack.Count - 1);
                                    break;
                                }
                                else if (instr.ArgS == 0)  // empty else - skip
                                    break;
                                label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.IFEND));
                                root = control_n.Children[1];
                            }
                            else   // while/for/foreach loop
                            {
                                root.AddNode(new DecompileNode(i, DecompileNodeType.BREAKLOOP));
                            }
                        }
                        break;
                    case LuaOpCode.OP_JMPF:
                        DecompileNode jmpf_n = (DecompileNode)stack.Pop();
                        DecompileNode jmpf_op = new DecompileNode(i, DecompileNodeType.UNOPERATOR, DecompileDataType.STRING, "");
                        jmpf_op.AddNode(jmpf_n);
                        if (instr.ArgS < 0)
                        {
                            DecompileNode jmpf_repeat = new DecompileNode(i, DecompileNodeType.REPEATLOOP, DecompileDataType.USERDATA, jmpf_op);
                            DecompileNode jmpf_repeat_chunk = new DecompileNode(i, DecompileNodeType.CHUNK);
                            jmpf_repeat.AddNode(jmpf_repeat_chunk);
                            for (int k = 0; k < root.Children.Count; k++)
                            {
                                if (root.Children[k].instruction_id >= i + instr.ArgS + 1)
                                {
                                    jmpf_repeat_chunk.AddNode(root.Children[k]);
                                    root.RemoveNode(root.Children[k]);
                                }
                            }
                            root.AddNode(jmpf_repeat);
                            break;
                        }
                        DecompileNode jmpf_cond = new DecompileNode(i, DecompileNodeType.COND, DecompileDataType.USERDATA, jmpf_op);// skip block if false
                        DecompileNode jmpf_nojump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        DecompileNode jmpf_jump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        root.AddNode(jmpf_cond);
                        jmpf_cond.AddNode(jmpf_nojump);
                        jmpf_cond.AddNode(jmpf_jump);
                        root = jmpf_nojump;
                        label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.IFEND));
                        break;
                    case LuaOpCode.OP_JMPT:
                        DecompileNode jmpt_n = (DecompileNode)stack.Pop();
                        DecompileNode jmpt_op = new DecompileNode(i, DecompileNodeType.UNOPERATOR, DecompileDataType.STRING, "not ");
                        jmpt_op.AddNode(jmpt_n);
                        if (instr.ArgS < 0)
                        {
                            DecompileNode jmpt_repeat = new DecompileNode(i, DecompileNodeType.REPEATLOOP, DecompileDataType.USERDATA, jmpt_op);
                            DecompileNode jmpt_repeat_chunk = new DecompileNode(i, DecompileNodeType.CHUNK);
                            jmpt_repeat.AddNode(jmpt_repeat_chunk);
                            for (int k = 0; k < root.Children.Count; k++)
                            {
                                if (root.Children[k].instruction_id >= i + instr.ArgS + 1)
                                {
                                    jmpt_repeat_chunk.AddNode(root.Children[k]);
                                    root.RemoveNode(root.Children[k]);
                                }
                            }
                            root.AddNode(jmpt_repeat);
                            break;
                        }
                        DecompileNode jmpt_cond = new DecompileNode(i, DecompileNodeType.COND, DecompileDataType.USERDATA, jmpt_op);// skip block if false
                        DecompileNode jmpt_nojump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        DecompileNode jmpt_jump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        root.AddNode(jmpt_cond);
                        jmpt_cond.AddNode(jmpt_nojump);
                        jmpt_cond.AddNode(jmpt_jump);
                        root = jmpt_nojump;
                        label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.IFEND));
                        break;
                    case LuaOpCode.OP_JMPNE:
                        DecompileNode jmpne_n2 = (DecompileNode)stack.Pop();
                        DecompileNode jmpne_n1 = (DecompileNode)stack.Pop();
                        DecompileNode jmpne_op = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, "==");
                        jmpne_op.AddNode(jmpne_n1);
                        jmpne_op.AddNode(jmpne_n2);
                        if (instr.ArgS < 0)
                        {
                            DecompileNode jmpne_repeat = new DecompileNode(i, DecompileNodeType.REPEATLOOP, DecompileDataType.USERDATA, jmpne_op);
                            DecompileNode jmpne_repeat_chunk = new DecompileNode(i, DecompileNodeType.CHUNK);
                            jmpne_repeat.AddNode(jmpne_repeat_chunk);
                            for (int k = 0; k < root.Children.Count; k++)
                            {
                                if (root.Children[k].instruction_id >= i + instr.ArgS + 1)
                                {
                                    jmpne_repeat_chunk.AddNode(root.Children[k]);
                                    root.RemoveNode(root.Children[k]);
                                }
                            }
                            root.AddNode(jmpne_repeat);
                            break;
                        }
                        DecompileNode jmpne_cond = new DecompileNode(i, DecompileNodeType.COND, DecompileDataType.USERDATA, jmpne_op);// skip block if not equal
                        DecompileNode jmpne_nojump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        DecompileNode jmpne_jump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        root.AddNode(jmpne_cond);
                        jmpne_cond.AddNode(jmpne_nojump);
                        jmpne_cond.AddNode(jmpne_jump);
                        root = jmpne_nojump;
                        label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.IFEND));
                        break;
                    case LuaOpCode.OP_JMPEQ:
                        DecompileNode jmpeq_n2 = (DecompileNode)stack.Pop();
                        DecompileNode jmpeq_n1 = (DecompileNode)stack.Pop();
                        DecompileNode jmpeq_op = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, "~=");
                        jmpeq_op.AddNode(jmpeq_n1);
                        jmpeq_op.AddNode(jmpeq_n2);
                        if (instr.ArgS < 0)
                        {
                            DecompileNode jmpeq_repeat = new DecompileNode(i, DecompileNodeType.REPEATLOOP, DecompileDataType.USERDATA, jmpeq_op);
                            DecompileNode jmpeq_repeat_chunk = new DecompileNode(i, DecompileNodeType.CHUNK);
                            jmpeq_repeat.AddNode(jmpeq_repeat_chunk);
                            for (int k = 0; k < root.Children.Count; k++)
                            {
                                if (root.Children[k].instruction_id >= i + instr.ArgS + 1)
                                {
                                    jmpeq_repeat_chunk.AddNode(root.Children[k]);
                                    root.RemoveNode(root.Children[k]);
                                }
                            }
                            root.AddNode(jmpeq_repeat);
                            break;
                        }
                        DecompileNode jmpeq_cond = new DecompileNode(i, DecompileNodeType.COND, DecompileDataType.USERDATA, jmpeq_op);// skip block if not equal
                        DecompileNode jmpeq_nojump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        DecompileNode jmpeq_jump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        root.AddNode(jmpeq_cond);
                        jmpeq_cond.AddNode(jmpeq_nojump);
                        jmpeq_cond.AddNode(jmpeq_jump);
                        root = jmpeq_nojump;
                        label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.IFEND));
                        break;
                    case LuaOpCode.OP_JMPGE:
                        DecompileNode jmpge_n2 = (DecompileNode)stack.Pop();
                        DecompileNode jmpge_n1 = (DecompileNode)stack.Pop();
                        DecompileNode jmpge_op = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, "<");
                        jmpge_op.AddNode(jmpge_n1);
                        jmpge_op.AddNode(jmpge_n2);
                        if (instr.ArgS < 0)
                        {
                            DecompileNode jmpge_repeat = new DecompileNode(i, DecompileNodeType.REPEATLOOP, DecompileDataType.USERDATA, jmpge_op);
                            DecompileNode jmpge_repeat_chunk = new DecompileNode(i, DecompileNodeType.CHUNK);
                            jmpge_repeat.AddNode(jmpge_repeat_chunk);
                            for (int k = 0; k < root.Children.Count; k++)
                            {
                                if (root.Children[k].instruction_id >= i + instr.ArgS + 1)
                                {
                                    jmpge_repeat_chunk.AddNode(root.Children[k]);
                                    root.RemoveNode(root.Children[k]);
                                }
                            }
                            root.AddNode(jmpge_repeat);
                            break;
                        }
                        DecompileNode jmpge_cond = new DecompileNode(i, DecompileNodeType.COND, DecompileDataType.USERDATA, jmpge_op);// skip block if not equal
                        DecompileNode jmpge_nojump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        DecompileNode jmpge_jump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        root.AddNode(jmpge_cond);
                        jmpge_cond.AddNode(jmpge_nojump);
                        jmpge_cond.AddNode(jmpge_jump);
                        root = jmpge_nojump;
                        label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.IFEND));
                        break;
                    case LuaOpCode.OP_JMPGT:
                        DecompileNode jmpgt_n2 = (DecompileNode)stack.Pop();
                        DecompileNode jmpgt_n1 = (DecompileNode)stack.Pop();
                        DecompileNode jmpgt_op = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, "<=");
                        jmpgt_op.AddNode(jmpgt_n1);
                        jmpgt_op.AddNode(jmpgt_n2);
                        if (instr.ArgS < 0)
                        {
                            DecompileNode jmpgt_repeat = new DecompileNode(i, DecompileNodeType.REPEATLOOP, DecompileDataType.USERDATA, jmpgt_op);
                            DecompileNode jmpgt_repeat_chunk = new DecompileNode(i, DecompileNodeType.CHUNK);
                            jmpgt_repeat.AddNode(jmpgt_repeat_chunk);
                            for (int k = 0; k < root.Children.Count; k++)
                            {
                                if (root.Children[k].instruction_id >= i + instr.ArgS + 1)
                                {
                                    jmpgt_repeat_chunk.AddNode(root.Children[k]);
                                    root.RemoveNode(root.Children[k]);
                                }
                            }
                            root.AddNode(jmpgt_repeat);
                            break;
                        }
                        DecompileNode jmpgt_cond = new DecompileNode(i, DecompileNodeType.COND, DecompileDataType.USERDATA, jmpgt_op);// skip block if not equal
                        DecompileNode jmpgt_nojump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        DecompileNode jmpgt_jump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        root.AddNode(jmpgt_cond);
                        jmpgt_cond.AddNode(jmpgt_nojump);
                        jmpgt_cond.AddNode(jmpgt_jump);
                        root = jmpgt_nojump;
                        label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.IFEND));
                        break;
                    case LuaOpCode.OP_JMPLE:
                        DecompileNode jmple_n2 = (DecompileNode)stack.Pop();
                        DecompileNode jmple_n1 = (DecompileNode)stack.Pop();
                        DecompileNode jmple_op = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, ">");
                        if (instr.ArgS < 0)
                        {
                            DecompileNode jmple_repeat = new DecompileNode(i, DecompileNodeType.REPEATLOOP, DecompileDataType.USERDATA, jmple_op);
                            DecompileNode jmple_repeat_chunk = new DecompileNode(i, DecompileNodeType.CHUNK);
                            jmple_repeat.AddNode(jmple_repeat_chunk);
                            for (int k = 0; k < root.Children.Count; k++)
                            {
                                if (root.Children[k].instruction_id >= i + instr.ArgS + 1)
                                {
                                    jmple_repeat_chunk.AddNode(root.Children[k]);
                                    root.RemoveNode(root.Children[k]);
                                }
                            }
                            root.AddNode(jmple_repeat);
                            break;
                        }
                        jmple_op.AddNode(jmple_n1);
                        jmple_op.AddNode(jmple_n2);
                        DecompileNode jmple_cond = new DecompileNode(i, DecompileNodeType.COND, DecompileDataType.USERDATA, jmple_op);// skip block if not equal
                        DecompileNode jmple_nojump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        DecompileNode jmple_jump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        root.AddNode(jmple_cond);
                        jmple_cond.AddNode(jmple_nojump);
                        jmple_cond.AddNode(jmple_jump);
                        root = jmple_nojump;
                        label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.IFEND));
                        break;
                    case LuaOpCode.OP_JMPLT:
                        DecompileNode jmplt_n2 = (DecompileNode)stack.Pop();
                        DecompileNode jmplt_n1 = (DecompileNode)stack.Pop();
                        DecompileNode jmplt_op = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, ">=");
                        jmplt_op.AddNode(jmplt_n1);
                        jmplt_op.AddNode(jmplt_n2);
                        if (instr.ArgS < 0)
                        {
                            DecompileNode jmplt_repeat = new DecompileNode(i, DecompileNodeType.REPEATLOOP, DecompileDataType.USERDATA, jmplt_op);
                            DecompileNode jmplt_repeat_chunk = new DecompileNode(i, DecompileNodeType.CHUNK);
                            jmplt_repeat.AddNode(jmplt_repeat_chunk);
                            for (int k = 0; k < root.Children.Count; k++)
                            {
                                if (root.Children[k].instruction_id >= i + instr.ArgS + 1)
                                {
                                    jmplt_repeat_chunk.AddNode(root.Children[k]);
                                    root.RemoveNode(root.Children[k]);
                                }
                            }
                            root.AddNode(jmplt_repeat);
                            break;
                        }
                        DecompileNode jmplt_cond = new DecompileNode(i, DecompileNodeType.COND, DecompileDataType.USERDATA, jmplt_op);// skip block if not equal
                        DecompileNode jmplt_nojump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        DecompileNode jmplt_jump = new DecompileNode(i, DecompileNodeType.CHUNK);
                        root.AddNode(jmplt_cond);
                        jmplt_cond.AddNode(jmplt_nojump);
                        jmplt_cond.AddNode(jmplt_jump);
                        root = jmplt_nojump;
                        label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.IFEND));
                        break;
                    case LuaOpCode.OP_JMPONT:
                        DecompileNode jmpont_n = (DecompileNode)stack.Pop();
                        DecompileNode jmpont_op = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, "or");
                        andor_operator_helper.Add(jmpont_op);
                        jmpont_op.AddNode(jmpont_n);
                        if (instr.ArgS < 0)
                        {
                            LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaFunction.Decompile(): Unsupported jump index "
                                + instr.ArgS.ToString());
                            return null;
                        }
                        label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.ANDOR));
                        break;
                    case LuaOpCode.OP_JMPONF:
                        DecompileNode jmponf_n = (DecompileNode)stack.Pop();
                        DecompileNode jmponf_op = new DecompileNode(i, DecompileNodeType.MULTOPERATOR, DecompileDataType.STRING, "and");
                        andor_operator_helper.Add(jmponf_op);
                        jmponf_op.AddNode(jmponf_n);
                        if (instr.ArgS < 0)
                        {
                            LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaFunction.Decompile(): Unsupported jump index "
                                + instr.ArgS.ToString());
                            return null;
                        }
                        label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.ANDOR));
                        break;
                    case LuaOpCode.OP_FORPREP:
                        int forprep_local = stack.Pos - start_pos;
                        DecompileNode forprep_for = new DecompileNode(i, DecompileNodeType.FORLOOP, DecompileDataType.NUMBER, null);
                        root.AddNode(forprep_for);
                        forprep_for.AddNode(new DecompileNode(i, DecompileNodeType.CHUNK));
                        root = forprep_for.Children[0];
                        local_count += 3;
                        label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.FOR));
                        break;
                    case LuaOpCode.OP_FORLOOP:   // handled separately
                        break;
                    case LuaOpCode.OP_LFORPREP:
                        DecompileNode lforprep_for = new DecompileNode(i, DecompileNodeType.FOREACHLOOP);
                        lforprep_for.AddNode(new DecompileNode(i, DecompileNodeType.CHUNK));
                        root.AddNode(lforprep_for);
                        root = lforprep_for.Children[0];
                        DecompileNode lforprep_key = new DecompileNode(i, DecompileNodeType.IDENTIFIER, DecompileDataType.STRING, GetLocal(stack.Pos));
                        DecompileNode lforprep_val = new DecompileNode(i, DecompileNodeType.IDENTIFIER, DecompileDataType.STRING, GetLocal(stack.Pos + 1));
                        stack.Push(lforprep_key);
                        stack.Push(lforprep_val);
                        local_count += 3;
                        label_stack.Add(new LabelInfo(i + instr.ArgS + 1, LabelType.FOREACH));
                        break;
                    case LuaOpCode.OP_LFORLOOP:  // handled separately
                        break;
                    case LuaOpCode.OP_CLOSURE:                     // function definition
                        DecompileNode n = new DecompileNode(i, DecompileNodeType.FUNCDEF, DecompileDataType.STRING, null);
                        int cur_pos = stack.Pos+1;
                        stack.Push(n);
                        n.AddNode(Functions[instr.ArgA].Decompile(stack));
                        while (stack.Pos != cur_pos)
                            stack.Pop();
                        break;
                    default:
                        LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaFunction.Decompile(): Unsupported opcode " 
                            + instr.OpCode.ToString());
                        return null;
                }
                if (done)
                    break;
            }
            DecompileNode root_args = new DecompileNode(Int32.MaxValue, DecompileNodeType.TABLE, DecompileDataType.LIST, null);
            if(IsVarArg)
                root_args.AddNode((DecompileNode)stack.PopAt(stack.Pos - start_pos - 1));
            for (int i = 0; i < NumParams; i++)
                root_args.AddNode((DecompileNode)stack.PopAt(stack.Pos - start_pos - 1));
            root.data = root_args;
            return root;
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
            catch(Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaBinaryScript(): Error reading script from memory");
                func = null;
            }
        }
    }
}
