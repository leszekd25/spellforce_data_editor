using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFLua.LuaDecompiler
{
    public enum ChunkType { IF, ELSEIF, ELSE, WHILE, FOR, FOREACH, REPEAT, AND, OR, BREAK, CONTINUE}

    public class ChunkInterval
    {
        public int start;
        public int end;
        public ChunkType ctype;

        public ChunkInterval(int s, int e, ChunkType t)
        {
            if(s<e)
            {
                start = s;
                end = e;
            }
            else
            {
                start = e;
                end = s;
            }
            ctype = t;
        }
    }

    public class Decompiler
    {
        List<IRValue> stack = new List<IRValue>();
        LuaLocalVariableRegistry locals = new LuaLocalVariableRegistry();
        int start_pos = -1;
        List<int> instr_ids = new List<int>();

        public void Push(IRValue val)
        {
            stack.Add(val);
            locals.UpdateLocal(stack.Count - 1, instr_ids[instr_ids.Count-1]);
        }

        public IRValue Pop()
        {
            locals.UnregisterLocal(stack.Count - 1);
            IRValue val = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            return val;
        }

        // three passes: find repeat loops, find for/foreach loops and ifs, find while loops
        public List<ChunkInterval> PreloadChunks(LuaBinaryFunction fnc)
        {
            List<ChunkInterval> chunks = new List<ChunkInterval>();
            List<ChunkInterval> chunk_stack = new List<ChunkInterval>();

            // first read backwards to find repeat, for, foreach loops

            for (int i = fnc.Instructions.Count - 1; i >= 0; i--)
            {
                LuaInstruction instr = fnc.Instructions[i];

                ChunkInterval interval = null;
                switch (instr.OpCode)
                {
                    case LuaOpCode.OP_JMPEQ:
                    case LuaOpCode.OP_JMPGE:
                    case LuaOpCode.OP_JMPGT:
                    case LuaOpCode.OP_JMPLE:
                    case LuaOpCode.OP_JMPLT:
                    case LuaOpCode.OP_JMPNE:
                    case LuaOpCode.OP_JMPT:
                    case LuaOpCode.OP_JMPF:
                        if (instr.ArgS < 0)
                            interval = new ChunkInterval(i, i + instr.ArgS, ChunkType.REPEAT);
                        break;
                    case LuaOpCode.OP_FORLOOP:
                        interval = new ChunkInterval(i, i + instr.ArgS, ChunkType.FOR);
                        break;
                    case LuaOpCode.OP_LFORLOOP:
                        interval = new ChunkInterval(i, i + instr.ArgS, ChunkType.FOREACH);
                        break;
                }
                if (interval != null)
                {
                    int pos = 0;
                    for (int j = 0; j < chunks.Count; j++)
                    {
                        pos = j;
                        ChunkInterval ci = chunks[j];
                        if (ci.start > interval.start)
                            break;
                        pos = chunks.Count;
                    }
                    chunks.Insert(pos, interval);
                }
            }

            // read forward to find other chunks

            for (int i = 0; i < fnc.Instructions.Count; i++)
            {
                LuaInstruction instr = fnc.Instructions[i];

                ChunkInterval interval = null;
                switch (instr.OpCode)
                {
                    case LuaOpCode.OP_JMP:   // fix
                        if (instr.ArgS >= 0)
                        {
                            // break instruction
                            if ((fnc.Instructions[i + instr.ArgS].OpCode == LuaOpCode.OP_JMP)
                                && (fnc.Instructions[i + instr.ArgS].ArgS < 0))
                                interval = new ChunkInterval(i, i, ChunkType.BREAK);
                            else if ((fnc.Instructions[i + instr.ArgS].OpCode == LuaOpCode.OP_LFORLOOP)
                                || (fnc.Instructions[i + instr.ArgS].OpCode == LuaOpCode.OP_FORLOOP))
                            {
                                bool is_valid = false;
                                // look for IF that ends at ELSE start (which right now is i)
                                for (int k = 0; k < chunks.Count; k++)
                                    if ((chunks[k].end == i) && (chunks[k].ctype == ChunkType.IF))
                                    {
                                        is_valid = true;
                                        break;
                                    }

                                if (!is_valid)
                                    interval = new ChunkInterval(i, i, ChunkType.BREAK);
                                else
                                    interval = new ChunkInterval(i, i + instr.ArgS, ChunkType.ELSE);
                            }
                            else
                                interval = new ChunkInterval(i, i + instr.ArgS, ChunkType.ELSE);
                        }
                        break;
                    case LuaOpCode.OP_JMPEQ:
                    case LuaOpCode.OP_JMPGE:
                    case LuaOpCode.OP_JMPGT:
                    case LuaOpCode.OP_JMPLE:
                    case LuaOpCode.OP_JMPLT:
                    case LuaOpCode.OP_JMPNE:
                    case LuaOpCode.OP_JMPT:
                    case LuaOpCode.OP_JMPF:
                        if (instr.ArgS >= 0)
                        {
                            // fit AND here
                            interval = new ChunkInterval(i, i + instr.ArgS, ChunkType.IF);
                        }
                        break;
                }
                if (interval != null)
                {
                    int pos = 0;
                    for (int j = 0; j < chunks.Count; j++)
                    {
                        pos = j;
                        ChunkInterval ci = chunks[j];
                        if (ci.start > interval.start)
                            break;
                        pos = chunks.Count;
                    }
                    chunks.Insert(pos, interval);
                }
            }

            // follow chunks to fix remaining errors, find while loops
            int current_chunk_index = 0;

            for (int i = 0; i < fnc.Instructions.Count; i++)
            {
                LuaInstruction instr = fnc.Instructions[i];

                ChunkInterval interval = null;

                if ((current_chunk_index < chunks.Count) && (chunks[current_chunk_index].start == i))
                {
                    chunk_stack.Add(chunks[current_chunk_index]);
                    current_chunk_index += 1;
                }

                if (chunk_stack.Count != 0)
                {
                    switch (instr.OpCode)
                    {
                        case LuaOpCode.OP_JMP:
                            // change if to while, if jump opcode is found, else create while loop
                            if (instr.ArgS < 0)
                            {
                                if (chunk_stack[chunk_stack.Count - 1].ctype == ChunkType.IF)
                                    chunk_stack[chunk_stack.Count - 1].ctype = ChunkType.WHILE;
                                else
                                    interval = new ChunkInterval(i, i + instr.ArgS + 1, ChunkType.WHILE);
                            }

                            // change else to if, in case if is not found
                            if((instr.ArgS >= 0)&&(chunk_stack[chunk_stack.Count-1].ctype == ChunkType.ELSE))
                            {
                                bool is_valid = false;
                                // look for IF that ends at ELSE start (which right now is i)
                                for(int k = chunks.IndexOf(chunk_stack[chunk_stack.Count-1])-1; k>=0; k--)
                                    if((chunks[k].end == i)&&(chunks[k].ctype == ChunkType.IF))
                                    {
                                        is_valid = true;
                                        break;
                                    }

                                if(!is_valid)
                                    chunk_stack[chunk_stack.Count - 1].ctype = ChunkType.IF;
                            }
                            break;
                    }

                    for(int k = 0; k < chunk_stack.Count; k++)
                        if (chunk_stack[k].end == i)
                            chunk_stack.RemoveAt(k);
                }
                else
                {
                    switch(instr.OpCode)
                    {
                        case LuaOpCode.OP_JMP:
                            if(instr.ArgS < 0)
                                interval = new ChunkInterval(i, i + instr.ArgS + 1, ChunkType.WHILE);
                            break;
                    }
                }

                if (interval != null)
                {
                    int pos = 0;
                    for (int j = 0; j < chunks.Count; j++)
                    {
                        pos = j;
                        ChunkInterval ci = chunks[j];
                        if (ci.start > interval.start)
                            break;
                        pos = chunks.Count;
                    }
                    chunks.Insert(pos, interval);
                }
            }

            return chunks;
        }

        public Chunk Decompile(LuaBinaryFunction fnc, Closure cl = null, List<string> upvalues = null)
        {
            if (fnc.Instructions.Count == 0)
                throw new Exception("tmp exception");
            //return null;
            if (fnc.Instructions[fnc.Instructions.Count - 1].OpCode != LuaOpCode.OP_END)
                throw new Exception("tmp exception");
            //return null;

            // set up function arguments if any
            for(int i=0;i!=fnc.NumParams;i++)
            {
                Push(new Str() { value = "_arg" + i.ToString() });
                locals.RegisterLocal(stack.Count - 1, "_arg" + i.ToString());
            }
            if(fnc.IsVarArg)
            {
                Push(new Str() { value = "..." });
                locals.RegisterLocal(stack.Count - 1, "arg");
            }

            Chunk root = new Chunk();
            List<ChunkInterval> chunks = PreloadChunks(fnc);
            int next_chunk = 0;
            List<ChunkInterval> chunk_stack = new List<ChunkInterval>();
            instr_ids.Add(0);
            List<OperatorBinaryLogic> andor_stack = new List<OperatorBinaryLogic>();
            List<int> andor_pos_stack = new List<int>();

            for(instr_ids[instr_ids.Count-1] = 0; instr_ids[instr_ids.Count - 1] < fnc.Instructions.Count; instr_ids[instr_ids.Count - 1]++)
            {
                LuaInstruction instr = fnc.Instructions[instr_ids[instr_ids.Count - 1]];
                //System.Diagnostics.Debug.WriteLine("STACK COUNT: "+stack.Count
                //    +" | INSTR #"+instr_ids[instr_ids.Count-1].ToString()
                //    +" | "+instr.ToString());

                // check instruction type for action
                switch(instr.OpCode)
                {
                    case LuaOpCode.OP_END:   
                        break;

                    case LuaOpCode.OP_RETURN:
                        Return return_ret = new Return() { InstructionID = instr_ids[instr_ids.Count - 1], parent = root };
                        for (int k = 0; stack.Count > start_pos + 1 + instr.ArgU; k++)
                            return_ret.Items.Insert(0, Pop());
                        root.Items.Add(return_ret);
                        break;

                    case LuaOpCode.OP_CALL:
                        if(instr.ArgB==0)
                        {
                            Table call_proc_table = new Table();
                            Procedure call_proc = new Procedure()
                            {
                                InstructionID = instr_ids[instr_ids.Count - 1],
                                Arguments = call_proc_table,
                                parent = root
                            };
                            for (int k = 0; stack.Count > start_pos + instr.ArgA + 1 + 1; k++)
                                call_proc_table.Items.Insert(0, new TableAssignment() { index = new Nil(), value = Pop() });
                            if ((call_proc_table.Items.Count == 1) && (call_proc_table.Items[0].value is Table))
                                call_proc.Arguments = (Table)call_proc_table.Items[0].value;
                            call_proc.Name = (ILValue)Pop();
                            if (call_proc.Name is SelfIdentifier)        // special case...
                                call_proc.Arguments.Items.RemoveAt(0);   // this will be nil

                            root.Items.Add(call_proc);
                        }
                        else
                        {
                            Table call_func_table = new Table();
                            Function call_func = new Function() { Arguments = call_func_table };
                            for (int k = 0; stack.Count > start_pos + instr.ArgA + 1 + 1; k++)
                                call_func_table.Items.Insert(0, new TableAssignment() { index = new Nil(), value = Pop() });
                            if ((call_func_table.Items.Count == 1) && (call_func_table.Items[0].value is Table))
                                call_func.Arguments = (Table)call_func_table.Items[0].value;
                            call_func.Name = (ILValue)Pop();
                            if (call_func.Name is SelfIdentifier)
                                call_func.Arguments.Items.RemoveAt(0);   // this will be nil


                            if (instr.ArgB == 255)
                                Push(call_func);
                            else
                                for (int k = 0; k < instr.ArgB; k++)
                                    Push(call_func);    // will get cleaned up in later phases
                        }
                        break;

                    case LuaOpCode.OP_TAILCALL:
                        Table tailcall_func_table = new Table();
                        Function tailcall_func = new Function() { Arguments = tailcall_func_table };
                        for (int k = 0; stack.Count > start_pos + instr.ArgA + 1 + 1; k++)
                            tailcall_func_table.Items.Insert(0, new TableAssignment() { index = new Nil(), value = Pop() });
                        if ((tailcall_func_table.Items.Count == 1) && (tailcall_func_table.Items[0].value is Table))
                            tailcall_func.Arguments = (Table)tailcall_func_table.Items[0].value;
                        tailcall_func.Name = (ILValue)Pop();
                        if (tailcall_func.Name is SelfIdentifier)
                            tailcall_func.Arguments.Items.RemoveAt(0);   // this will be nil
                        Push(tailcall_func);

                        Return tailcall_ret = new Return() { InstructionID = instr_ids[instr_ids.Count - 1], parent = root };
                        for (int k = 0; stack.Count > start_pos + 1 + instr.ArgB; k++)
                            tailcall_ret.Items.Insert(0, Pop());
                        root.Items.Add(tailcall_ret);
                        break;

                    case LuaOpCode.OP_PUSHNIL:
                        // special case for loop break inside some specific variants
                        if(instr_ids[instr_ids.Count-1]!=0)
                            if (fnc.Instructions[instr_ids[instr_ids.Count - 1] - 1].OpCode == LuaOpCode.OP_JMP)
                            {
                                if ((fnc.Instructions[instr_ids[instr_ids.Count - 1] - 2].OpCode == LuaOpCode.OP_POP)
                                && (fnc.Instructions[instr_ids[instr_ids.Count - 1] - 2].ArgU == instr.ArgU))
                                    break;
                            }
                        for (int k = 0; k < instr.ArgU; k++)
                            Push(new Nil());
                        break;

                    case LuaOpCode.OP_PUSHINT:
                        Push(new Num() { value = instr.ArgS });
                        break;

                    case LuaOpCode.OP_PUSHSTRING:
                        Push(new Str() { value = fnc.Strings[instr.ArgU] });
                        break;

                    case LuaOpCode.OP_PUSHNUM:
                        Push(new Num() { value = fnc.Numbers[instr.ArgU] });
                        break;

                    case LuaOpCode.OP_PUSHNEGNUM:
                        Push(new Num() { value = -fnc.Numbers[instr.ArgU] });
                        break;

                    case LuaOpCode.OP_PUSHSELF:
                        Push(new SelfIdentifier()
                        {
                            instance = (ILValue)Pop(),    // this must be ILValue, or error is thrown
                            method = new Identifier() { name = fnc.Strings[instr.ArgU] }
                        });
                        Push(new Nil());
                        break;

                    case LuaOpCode.OP_PUSHUPVALUE:
                        Push(new UpIdentifier() { name = upvalues[instr.ArgU] });
                        break;

                    case LuaOpCode.OP_GETGLOBAL:
                        Push(new Identifier() { name = fnc.Strings[instr.ArgU] });
                        break;

                    case LuaOpCode.OP_GETLOCAL:
                        // where can this be moved instead...
                        if(!locals.IsLocalRegistered(instr.ArgU+start_pos+1))
                        {
                            int last_updated = locals.GetLastUpdated(instr.ArgU + start_pos + 1);
                            locals.RegisterLocal(instr.ArgU + start_pos + 1, "_loc" + (instr.ArgU + start_pos + 1).ToString());
                            Assignment loc_a = new Assignment()
                            {
                                Left = new LocalIdentifier() { name = locals.registry[instr.ArgU + start_pos + 1] },
                                Right = stack[instr.ArgU + start_pos + 1]
                            };

                            Chunk parent_chunk = root;
                            int loc_pos = root.Items.Count - 1;
                            bool found = false;
                            while(true)
                            {
                                if ((parent_chunk.Items.Count != 0)
                                    && (parent_chunk.Items[loc_pos].InstructionID <= last_updated))
                                {
                                    found = true;
                                    break;
                                }

                                loc_pos -= 1;
                                if (loc_pos >= 0)
                                    continue;

                                loc_pos = -1;

                                //if loc_pos = -1, search for next parent chunk 
                                if (parent_chunk.parent == null)
                                {
                                    found = true;
                                    break;
                                }

                                if (((IStatement)parent_chunk.parent).InstructionID <= last_updated)
                                {
                                    found = true;
                                    break;
                                }

                                parent_chunk = (Chunk)parent_chunk.parent.parent;
                                loc_pos = parent_chunk.Items.Count - 1;
                            }

                            if (found)
                            {
                                loc_a.InstructionID = last_updated;
                                loc_a.parent = parent_chunk;
                                parent_chunk.Items.Insert(loc_pos+1, loc_a);
                            }
                            else
                                throw new Exception("tmp exception");
                            //return null;
                        }

                        Push(new Identifier() { name = locals.GetLocal(instr.ArgU + start_pos + 1) });
                        break;

                    case LuaOpCode.OP_GETTABLE:    // might be popped in wrong order...
                        Push(new IndexedIdentifier()
                        {
                            index = Pop(),
                            name = (ILValue)Pop()
                        });
                        break;

                    case LuaOpCode.OP_GETINDEXED:
                        if (!locals.IsLocalRegistered(instr.ArgU + start_pos + 1))
                        {
                            int last_updated = locals.GetLastUpdated(instr.ArgU + start_pos + 1);
                            locals.RegisterLocal(instr.ArgU + start_pos + 1, "_loc" + (instr.ArgU + start_pos + 1).ToString());
                            Assignment loc_a = new Assignment()
                            {
                                Left = new LocalIdentifier() { name = locals.registry[instr.ArgU + start_pos + 1] },
                                Right = stack[instr.ArgU + start_pos + 1]
                            };

                            Chunk parent_chunk = root;
                            int loc_pos = root.Items.Count - 1;
                            bool found = false;
                            while (true)
                            {
                                if ((parent_chunk.Items.Count != 0)
                                    && (parent_chunk.Items[loc_pos].InstructionID <= last_updated))
                                {
                                    found = true;
                                    break;
                                }

                                loc_pos -= 1;
                                if (loc_pos >= 0)
                                    continue;

                                loc_pos = -1;

                                //if loc_pos = -1, search for next parent chunk 
                                if (parent_chunk.parent == null)
                                {
                                    found = true;
                                    break;
                                }

                                if (((IStatement)parent_chunk.parent).InstructionID <= last_updated)
                                {
                                    found = true;
                                    break;
                                }

                                parent_chunk = (Chunk)parent_chunk.parent.parent;
                                loc_pos = parent_chunk.Items.Count - 1;
                            }

                            if (found)
                            {
                                loc_a.InstructionID = last_updated;
                                loc_a.parent = parent_chunk;
                                parent_chunk.Items.Insert(loc_pos + 1, loc_a);
                            }
                            else
                                throw new Exception("tmp exception");
                            //return null;
                        }

                        Push(new IndexedIdentifier()
                        {
                            index = new Identifier() { name = locals.GetLocal(instr.ArgU + start_pos + 1) },
                            name = (ILValue)Pop()
                        });
                        break;

                    case LuaOpCode.OP_GETDOTTED:
                        Push(new DottedIdentifier
                        {
                            instance = (ILValue)Pop(),
                            member = new Identifier() { name = fnc.Strings[instr.ArgU] }
                        });
                        break;

                    case LuaOpCode.OP_CREATETABLE:
                        Push(new Table());
                        break;

                    case LuaOpCode.OP_SETGLOBAL:
                        root.Items.Add(new Assignment()
                        {
                            Left = new Identifier() { name = fnc.Strings[instr.ArgU] },
                            Right = Pop(),
                            InstructionID = instr_ids[instr_ids.Count - 1],
                            parent = root
                        });
                        break;

                    case LuaOpCode.OP_SETLOCAL:
                        // where can this be moved instead...
                        if (!locals.IsLocalRegistered(instr.ArgU + start_pos + 1))
                        {
                            int last_updated = locals.GetLastUpdated(instr.ArgU + start_pos + 1);
                            locals.RegisterLocal(instr.ArgU + start_pos + 1, "_loc" + (instr.ArgU + start_pos + 1).ToString());
                            Assignment loc_a = new Assignment()
                            {
                                Left = new LocalIdentifier() { name = locals.registry[instr.ArgU + start_pos + 1] },
                                Right = stack[instr.ArgU + start_pos + 1]
                            };

                            Chunk parent_chunk = root;
                            int loc_pos = root.Items.Count - 1;
                            bool found = false;
                            while (true)
                            {
                                if ((parent_chunk.Items.Count != 0)
                                    && (parent_chunk.Items[loc_pos].InstructionID <= last_updated))
                                {
                                    found = true;
                                    break;
                                }

                                loc_pos -= 1;
                                if (loc_pos >= 0)
                                    continue;

                                loc_pos = -1;

                                //if loc_pos = -1, search for next parent chunk 
                                if (parent_chunk.parent == null)
                                {
                                    found = true;
                                    break;
                                }

                                if (((IStatement)parent_chunk.parent).InstructionID <= last_updated)
                                {
                                    found = true;
                                    break;
                                }

                                parent_chunk = (Chunk)parent_chunk.parent.parent;
                                loc_pos = parent_chunk.Items.Count - 1;
                            }

                            if (found)
                            {
                                loc_a.InstructionID = last_updated;
                                loc_a.parent = parent_chunk;
                                parent_chunk.Items.Insert(loc_pos + 1, loc_a);
                            }
                            else
                                throw new Exception("tmp exception");
                            //return null;
                        }
                        
                        root.Items.Add(new Assignment()
                        {
                            Left = new Identifier() { name = locals.GetLocal(instr.ArgU + start_pos + 1) },
                            Right = Pop(),
                            InstructionID = instr_ids[instr_ids.Count - 1],
                            parent = root
                        });
                        break;

                    case LuaOpCode.OP_SETTABLE:
                        root.Items.Add(new Assignment()
                        {
                            Right = Pop(),
                            Left = new IndexedIdentifier()
                            {
                                index = Pop(),
                                name = (ILValue)Pop(),
                            },
                            InstructionID = instr_ids[instr_ids.Count - 1],
                            parent = root
                        });
                        for (int k = 3; k < instr.ArgB; k++)
                            Pop();
                        break;

                    case LuaOpCode.OP_SETMAP:
                        Table setmap_table = (Table)stack[stack.Count - 2 * instr.ArgU - 1];
                        int setmap_table_c = setmap_table.Items.Count;
                        setmap_table.IsList = false;
                        for(int setmap_elems = instr.ArgU; setmap_elems > 0; setmap_elems--)
                            setmap_table.Items.Insert(setmap_table_c, new TableAssignment()
                            {
                                value = Pop(),
                                index = (Primitive)Pop()
                            });
                        break;

                    case LuaOpCode.OP_SETLIST:
                        Table setlist_table = (Table)stack[stack.Count - instr.ArgU - 1];
                        int setlist_table_c = setlist_table.Items.Count;
                        for (int setlist_elems = instr.ArgU; setlist_elems > 0; setlist_elems--)
                            setlist_table.Items.Insert(setlist_table_c, new TableAssignment()
                            {
                                value = Pop(),
                                index = new Nil()
                            });
                        break;

                    case LuaOpCode.OP_ADD:
                        OperatorBinaryArithmetic add_op = new OperatorBinaryArithmetic() { op_type = OperatorType.ADD };
                        add_op.Values.Insert(0, Pop());
                        add_op.Values.Insert(0, Pop());
                        Push(add_op);
                        break;

                    case LuaOpCode.OP_ADDI:
                        bool is_positive = instr.ArgS >= 0;
                        OperatorBinaryArithmetic addi_op = new OperatorBinaryArithmetic()
                            { op_type = (is_positive? OperatorType.ADD: OperatorType.SUB) };
                        addi_op.Values.Insert(0, new Num() { value = (is_positive ? instr.ArgS : -instr.ArgS) });
                        addi_op.Values.Insert(0, Pop());
                        Push(addi_op);
                        break;

                    case LuaOpCode.OP_SUB:
                        OperatorBinaryArithmetic sub_op = new OperatorBinaryArithmetic() { op_type = OperatorType.SUB };
                        sub_op.Values.Insert(0, Pop());
                        sub_op.Values.Insert(0, Pop());
                        Push(sub_op);
                        break;

                    case LuaOpCode.OP_MULT:
                        OperatorBinaryArithmetic mul_op = new OperatorBinaryArithmetic() { op_type = OperatorType.MUL };
                        mul_op.Values.Insert(0, Pop());
                        mul_op.Values.Insert(0, Pop());
                        Push(mul_op);
                        break;

                    case LuaOpCode.OP_DIV:
                        OperatorBinaryArithmetic div_op = new OperatorBinaryArithmetic() { op_type = OperatorType.DIV };
                        div_op.Values.Insert(0, Pop());
                        div_op.Values.Insert(0, Pop());
                        Push(div_op);
                        break;

                    case LuaOpCode.OP_POW:
                        OperatorBinaryArithmetic pow_op = new OperatorBinaryArithmetic() { op_type = OperatorType.POW };
                        pow_op.Values.Insert(0, Pop());
                        pow_op.Values.Insert(0, Pop());
                        Push(pow_op);
                        break;

                    case LuaOpCode.OP_CONCAT:
                        OperatorBinaryArithmetic concat_op = new OperatorBinaryArithmetic() { op_type = OperatorType.CONCAT };
                        for (int k = 0; k < instr.ArgU; k++)
                            concat_op.Values.Insert(0, Pop());
                        Push(concat_op);
                        break;

                    case LuaOpCode.OP_POP:
                        // special case for loop break inside some specific variants
                        if(fnc.Instructions[instr_ids[instr_ids.Count-1]+1].OpCode == LuaOpCode.OP_JMP)
                        {
                            if ((fnc.Instructions[instr_ids[instr_ids.Count - 1] + 2].OpCode == LuaOpCode.OP_PUSHNIL)
                            && (fnc.Instructions[instr_ids[instr_ids.Count - 1] + 2].ArgU == instr.ArgU))
                                break;
                        }
                        for(int k=0;k<instr.ArgU;k++)
                            Pop();
                        break;

                    case LuaOpCode.OP_MINUS:
                        Push(new OperatorUnaryArithmetic() { op_type = OperatorType.MINUS, Value = Pop() });
                        break;

                    case LuaOpCode.OP_NOT:
                        Push(new OperatorUnaryLogic() { op_type = OperatorType.FALSE, Value = Pop() });
                        break;

                    case LuaOpCode.OP_PUSHNILJMP:
                        Fork pushniljmp_fork = (Fork)root.parent;
                        root = (Chunk)pushniljmp_fork.parent;
                        root.Items.Remove(pushniljmp_fork);
                        Push((OperatorBinaryLogic)pushniljmp_fork.IfCondition);

                        chunk_stack.RemoveAt(chunk_stack.Count - 1);
                        chunks.RemoveAt(next_chunk - 1);
                        next_chunk -= 1;

                        instr_ids[instr_ids.Count - 1] += 1;
                        break;

                    case LuaOpCode.OP_JMP:
                        break;

                    case LuaOpCode.OP_JMPF:
                        OperatorBinaryLogic jmpf_op = new OperatorBinaryLogic() { op_type = OperatorType.NE };
                        jmpf_op.Values.Add(Pop());
                        jmpf_op.Values.Add(new Nil());
                        Push(jmpf_op);
                        break;

                    case LuaOpCode.OP_JMPT:
                        OperatorBinaryLogic jmpt_op = new OperatorBinaryLogic() { op_type = OperatorType.EQ };
                        jmpt_op.Values.Add(Pop());
                        jmpt_op.Values.Add(new Nil());
                        Push(jmpt_op);
                        break;

                    case LuaOpCode.OP_JMPNE:
                        OperatorBinaryLogic jmpne_op = new OperatorBinaryLogic() { op_type = OperatorType.EQ };
                        jmpne_op.Values.Insert(0, Pop());
                        jmpne_op.Values.Insert(0, Pop());
                        Push(jmpne_op);
                        break;

                    case LuaOpCode.OP_JMPEQ:
                        OperatorBinaryLogic jmpeq_op = new OperatorBinaryLogic() { op_type = OperatorType.NE };
                        jmpeq_op.Values.Insert(0, Pop());
                        jmpeq_op.Values.Insert(0, Pop());
                        Push(jmpeq_op);
                        break;

                    case LuaOpCode.OP_JMPGE:
                        OperatorBinaryLogic jmpge_op = new OperatorBinaryLogic() { op_type = OperatorType.LT };
                        jmpge_op.Values.Insert(0, Pop());
                        jmpge_op.Values.Insert(0, Pop());
                        Push(jmpge_op);
                        break;

                    case LuaOpCode.OP_JMPGT:
                        OperatorBinaryLogic jmpgt_op = new OperatorBinaryLogic() { op_type = OperatorType.LE };
                        jmpgt_op.Values.Insert(0, Pop());
                        jmpgt_op.Values.Insert(0, Pop());
                        Push(jmpgt_op);
                        break;

                    case LuaOpCode.OP_JMPLE:
                        OperatorBinaryLogic jmple_op = new OperatorBinaryLogic() { op_type = OperatorType.GT };
                        jmple_op.Values.Insert(0, Pop());
                        jmple_op.Values.Insert(0, Pop());
                        Push(jmple_op);
                        break;

                    case LuaOpCode.OP_JMPLT:
                        OperatorBinaryLogic jmplt_op = new OperatorBinaryLogic() { op_type = OperatorType.GE };
                        jmplt_op.Values.Insert(0, Pop());
                        jmplt_op.Values.Insert(0, Pop());
                        Push(jmplt_op);
                        break;

                    case LuaOpCode.OP_JMPONT:
                        OperatorBinaryLogic jmpont_op = new OperatorBinaryLogic() { op_type = OperatorType.OR };
                        jmpont_op.Values.Add(Pop());
                        andor_stack.Add(jmpont_op);
                        andor_pos_stack.Add(instr_ids[instr_ids.Count - 1] + instr.ArgS);
                        break;

                    case LuaOpCode.OP_JMPONF:
                        OperatorBinaryLogic jmponf_op = new OperatorBinaryLogic() { op_type = OperatorType.AND };
                        jmponf_op.Values.Add(Pop());
                        andor_stack.Add(jmponf_op);
                        andor_pos_stack.Add(instr_ids[instr_ids.Count - 1] + instr.ArgS);
                        break;

                    // these 4 are handled separately
                    case LuaOpCode.OP_FORPREP:
                        break;

                    case LuaOpCode.OP_FORLOOP:
                        break;

                    case LuaOpCode.OP_LFORPREP:
                        break;

                    case LuaOpCode.OP_LFORLOOP:
                        break;

                    case LuaOpCode.OP_CLOSURE:
                        List<string> _upvalues = null;
                        if (instr.ArgB != 0)
                        {
                            _upvalues = new List<string>();
                            for (int k = 0; k < instr.ArgB; k++)
                                _upvalues.Add(((Identifier)Pop()).name);
                        }

                        Closure closure_cl = new Closure();
                        Push(closure_cl);
                        int current_start_pos = start_pos;
                        start_pos = stack.Count - 1;

                        Decompile(fnc.Functions[instr.ArgA], closure_cl, _upvalues);

                        start_pos = current_start_pos;
                        //while (stack.Count > start_pos + 1 + 1)  // room for closure
                        //    Pop();
                        break;

                    default:
                        throw new Exception("Invalid opcode!");
                        return null;
                }

                // check andor stack form and/or operator to push into main stacl
                while(andor_stack.Count > 0)
                {
                    if (andor_pos_stack[andor_pos_stack.Count - 1] == instr_ids[instr_ids.Count - 1])
                    {
                        andor_stack[andor_stack.Count - 1].Values.Add(Pop());
                        Push(andor_stack[andor_stack.Count - 1]);
                        andor_stack.RemoveAt(andor_stack.Count - 1);
                        andor_pos_stack.RemoveAt(andor_pos_stack.Count - 1);
                    }
                    else
                        break;
                }

                // check chunk list for possible new statement
                if(chunks.Count > 0)
                {
                    if((next_chunk < chunks.Count)&&(chunks[next_chunk].start == instr_ids[instr_ids.Count - 1]))
                    {
                        chunk_stack.Add(chunks[next_chunk]);

                        switch(chunks[next_chunk].ctype)
                        {
                            case ChunkType.BREAK:
                                root.Items.Add(new Break());
                                break;
                            case ChunkType.FOR:
                                locals.RegisterLocal(stack.Count - 3, "i_" + (stack.Count - 3).ToString());
                                For forloop = new For()
                                {
                                    InstructionID = instr_ids[instr_ids.Count - 1],
                                    step = stack[stack.Count - 1],
                                    to = stack[stack.Count - 2],
                                    from = stack[stack.Count - 3],
                                    parent = root,
                                    name = new Identifier() { name = locals.GetLocal(stack.Count - 3) }
                                };
                                root.Items.Add(forloop);
                                root = forloop.LoopChunk;
                                break;

                            case ChunkType.FOREACH:
                                locals.RegisterLocal(stack.Count, "i_" + (stack.Count).ToString());
                                locals.RegisterLocal(stack.Count + 1, "v_" + (stack.Count + 1).ToString());
                                Push(new Identifier() { name = locals.GetLocal(stack.Count) });
                                Push(new Identifier() { name = locals.GetLocal(stack.Count) });
                                Foreach foreachloop = new Foreach()
                                {
                                    InstructionID = instr_ids[instr_ids.Count - 1],
                                    value = (Identifier)stack[stack.Count - 1],
                                    index = (Identifier)stack[stack.Count - 2],
                                    table = stack[stack.Count - 3],
                                    parent = root
                                };
                                root.Items.Add(foreachloop);
                                root = foreachloop.LoopChunk;
                                break;

                            case ChunkType.IF:    // get condition
                                Fork if_fork = new Fork()
                                {
                                    InstructionID = instr_ids[instr_ids.Count - 1],
                                    parent = root
                                };
                                if (stack[stack.Count-1] is IOperatorLogic)
                                    if_fork.IfCondition = (IOperatorLogic)Pop();
                                else
                                    if_fork.IfCondition = new OperatorUnaryLogic()
                                    {
                                        Value = new Nil(),
                                        op_type = OperatorType.TRUE
                                    };
                                root.Items.Add(if_fork);
                                root = if_fork.IfChunk;
                                break;

                            case ChunkType.ELSE:
                                for(int j = 0; j < chunk_stack.Count-1; j++)
                                    if((chunk_stack[j].end == instr_ids[instr_ids.Count-1])
                                        &&(chunk_stack[j].ctype == ChunkType.IF))
                                    {
                                        chunk_stack.RemoveAt(j);
                                        break;
                                    }
                                Fork else_fork = (Fork)root.parent;
                                root = else_fork.ElseChunk;
                                break;

                            case ChunkType.WHILE:
                                While whileloop = new While()
                                {
                                    InstructionID = instr_ids[instr_ids.Count - 1],
                                    parent = root
                                };
                                if (stack[stack.Count - 1] is IOperatorLogic)
                                    whileloop.Condition = (IOperatorLogic)Pop();
                                else
                                    whileloop.Condition = new OperatorUnaryLogic()
                                    {
                                        Value = new Num() { value = 1 },
                                        op_type = OperatorType.TRUE
                                    };
                                root.Items.Add(whileloop);
                                root = whileloop.LoopChunk;
                                break;

                            case ChunkType.REPEAT:
                                Repeat repeatloop = new Repeat()
                                {
                                    InstructionID = instr_ids[instr_ids.Count - 1],
                                    parent = root
                                };    // condition later
                                root.Items.Add(repeatloop);
                                root = repeatloop.LoopChunk;
                                break;
                            
                            default:
                                throw new Exception("tmp exception");
                                //return null;
                        }

                        next_chunk += 1;
                    }

                    if(chunk_stack.Count != 0)
                    {
                        while(chunk_stack[chunk_stack.Count-1].end == instr_ids[instr_ids.Count - 1])
                        {
                            bool jump_parent = true;
                            switch(chunk_stack[chunk_stack.Count-1].ctype)
                            {
                                case ChunkType.REPEAT:
                                    ((Repeat)root.parent).Condition = (IOperatorLogic)Pop();
                                    break;
                                case ChunkType.FOR:
                                case ChunkType.FOREACH:
                                    Pop(); Pop(); Pop();
                                    break;
                                case ChunkType.BREAK:
                                    jump_parent = false;
                                    break;
                                default:
                                    break;
                            }

                            if(jump_parent)
                                root = (Chunk)root.parent.parent;     // it's certain that such parent exists

                            chunk_stack.RemoveAt(chunk_stack.Count - 1);
                            if (chunk_stack.Count == 0)
                                break;
                        }
                    }
                }
            }
            instr_ids.RemoveAt(instr_ids.Count - 1);

            Simplify(root);
            // here is the simplification and fixing (multireturn functions pass)
            // todo...

            if(cl != null)
            {
                cl.ClosureChunk = root;
                while (stack.Count > start_pos + 1 + fnc.NumParams + (fnc.IsVarArg ? 1 : 0))
                    Pop();
                if (fnc.IsVarArg)
                    cl.Arguments.Add(new Identifier() { name = ((Str)Pop()).value });
                for (int i = 0; i != fnc.NumParams; i++)
                    cl.Arguments.Add(new Identifier() { name = ((Str)Pop()).value });
                cl.Arguments.Reverse();   // account for insertion order
                return null;         // chunk taken over by closure
            }

            return root;
        }


        public void Simplify(Chunk node)
        {
            int npos = 0;
            // assignment pass
            while(true)
            {
                if(node.Items.Count == npos)
                    break;

                IStatement current_statement = node.Items[npos];

                // roll two assignments into one - multireturn function
                if (current_statement is Assignment)
                {
                    Assignment n1 = (Assignment)current_statement;

                    if (n1.Right is Closure)
                        Simplify(((Closure)n1.Right).ClosureChunk);

                    if (npos + 1 < node.Items.Count)
                    {
                        if (node.Items[npos + 1] is Assignment)
                        {
                            Assignment n2 = (Assignment)node.Items[npos + 1];
                            if ((n1.Right is Function) && (n1.Right == n2.Right))
                            {
                                MultiAssignment mas = new MultiAssignment() { InstructionID = n1.InstructionID };
                                mas.Left.Insert(0, n1.Left);
                                mas.Left.Insert(0, n2.Left);
                                mas.Right.Insert(0, n1.Right);

                                node.Items.Remove(n1);
                                node.Items.Remove(n2);
                                node.Items.Insert(npos, mas);
                                npos -= 1;
                            }
                        }
                    }
                }
                else if (current_statement is MultiAssignment)
                {
                    if (npos + 1 < node.Items.Count)
                    {
                        if (node.Items[npos + 1] is Assignment)
                        {
                            MultiAssignment n1 = (MultiAssignment)current_statement;
                            Assignment n2 = (Assignment)node.Items[npos + 1];
                            if ((n1.Right[0] is Function) && (n1.Right[0] == n2.Right))
                            {
                                n1.Left.Insert(0, n2.Left);

                                node.Items.Remove(n2);
                                npos -= 1;
                            }
                        }
                    }
                }
                else if (current_statement is Fork)
                {
                    Fork cf = (Fork)current_statement;
                    Simplify(cf.IfChunk);
                    for (int i = 0; i < cf.ElseifChunks.Count; i++)
                        Simplify(cf.ElseifChunks[i]);
                    Simplify(cf.ElseChunk);
                }
                else if (current_statement is Loop)
                    Simplify(((Loop)current_statement).LoopChunk);
                npos += 1;
            }
        }
    }
}
