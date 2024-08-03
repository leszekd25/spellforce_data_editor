using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.PointOfService;
using Windows.Data.Text;
using System.Configuration;
using System.Windows.Forms;
using Windows.Media.Playback;
using System.Linq.Expressions;

namespace SFEngine.SFLua.LuaTokenizer
{
    public enum TokenType
    {
        /* terminal symbols denoted by reserved words */
        TK_AND, TK_BREAK,
        TK_DO, TK_ELSE, TK_ELSEIF, TK_END, TK_FOR, TK_FUNCTION, TK_IF, TK_IN, TK_LOCAL,
        TK_NIL, TK_NOT, TK_OR, TK_REPEAT, TK_RETURN, TK_THEN, TK_UNTIL, TK_WHILE,
        /* other terminal symbols */
        TK_NAME, TK_CONCAT, TK_DOTS, TK_EQ, TK_GE, TK_LE, TK_NE, TK_NUMBER,
        TK_STRING, TK_EOS,
        /* these are made up by me :) */
        TK_WHITESPACE, TK_NEWLINE, TK_COMMA, TK_SQBRACKET_OPEN, TK_SQBRACKET_CLOSE,
        TK_DOT_SINGLE, TK_SEMICOLON, TK_ASSIGN, TK_GT, TK_LT, TK_COMMENT,
        TK_ADD, TK_SUB, TK_MUL, TK_DIV, TK_EXP,
        TK_RNDBRACKET_OPEN, TK_RNDBRACKET_CLOSE,
        TK_CRLBRACKET_OPEN, TK_CRLBRACKET_CLOSE,
        TK_UPVALUE, TK_COLON,
    }


    public class Token
    {
        public TokenType type;
        public object value;

        public override string ToString()
        {
            return $"{type} {(value == null? "": value.ToString())}";
        }

        public T Get<T>()
        {
            return (T)value;
        }
    }

    public class Block
    {
        public List<Statement> statements = new();

        public static bool TryGet(List<Token> t, int s, out int e, out Block n)
        {
            n = new();
            while(true)
            {
                if(Statement.TryGet(t, s, out int ex, out Statement os))
                {
                    n.statements.Add(os);
                    s = ex;
                    if (t[s].type == TokenType.TK_SEMICOLON)
                    {
                        s++;
                    }
                }
                else
                {
                    break;
                }
                if((s <= t.Count)&& (t[s].type == TokenType.TK_EOS))
                {
                    s++;
                    break;
                }
            }
            e = s;
            return true;
        }
    }

    public abstract class Statement
    {
        public static bool TryGet(List<Token> t, int s, out int e, out Statement n)
        {
            return TryGetDoBlockEnd(t, s, out e, out n)
                || TryGetLocal(t, s, out e, out n)
                || TryGetWhile(t, s, out e, out n)
                || TryGetRepeat(t, s, out e, out n)
                || TryGetIf(t, s, out e, out n)
                || TryGetReturn(t, s, out e, out n)
                || TryGetBreak(t, s, out e, out n)
                || TryGetForNumber(t, s, out e, out n)
                || TryGetForTable(t, s, out e, out n)
                || TryGetFunction(t, s, out e, out n)
                || TryGetFunctionCall(t, s, out e, out n)
                || TryGetAssignment(t, s, out e, out n);
        }


        public static bool TryGetDoBlockEnd(List<Token> t, int s, out int e, out Statement n)
        {
            StatementDoBlockEnd m;

            e = s;
            if (t[s].type == TokenType.TK_DO)
            {
                s++;
                m = new();
                if (Block.TryGet(t, s, out s, out m.block))
                {
                    if (t[s].type == TokenType.TK_END)
                    {
                        s++;
                        n = m;
                        e = s;
                        return true;
                    }
                }
            }
            n = null;
            return false;
        }

        public static bool TryGetAssignment(List<Token> t, int s, out int e, out Statement n)
        {
            StatementAssignment m;

            if (Varlist.TryGet(t, s, out int ex, out Varlist v))
            {
                s = ex;
                if ((s + 1 < t.Count) && (t[s].type == TokenType.TK_ASSIGN))
                {
                    if (ExpressionList.TryGet(t, s + 1, out ex, out ExpressionList expl))
                    {
                        m = new();
                        m.vl = v;
                        m.el = expl;
                        s = ex;
                        n = m;
                        e = s;
                        return true;
                    }
                }
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetWhile(List<Token> t, int s, out int e, out Statement n)
        {
            StatementWhile m;

            if (t[s].type == TokenType.TK_WHILE)
            {
                if ((s + 1 < t.Count) && (Expression.TryGet(t, s + 1, out int ex, out Expression n2)))
                {
                    s = ex;
                    if ((s + 1 < t.Count) && (t[s].type == TokenType.TK_DO) && (Block.TryGet(t, s + 1, out ex, out Block b)))
                    {
                        s = ex;
                        if ((s < t.Count) && (t[s].type == TokenType.TK_END))
                        {
                            m = new();
                            m.exp = n2;
                            m.block = b;
                            s++;
                            n = m;
                            e = s;
                            return true;
                        }
                    }
                }
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }



        public static bool TryGetRepeat(List<Token> t, int s, out int e, out Statement n)
        {
            StatementRepeat m;

            if (t[s].type == TokenType.TK_REPEAT)
            {
                if ((s + 1 < t.Count) && (Block.TryGet(t, s + 1, out int ex, out Block b)))
                {
                    s = ex;
                    if ((s + 1 < t.Count) && (t[s].type == TokenType.TK_UNTIL) && (Expression.TryGet(t, s + 1, out ex, out Expression n2)))
                    {
                        m = new();
                        m.exp = n2;
                        m.block = b;
                        s = ex;
                        n = m;
                        e = s;
                        return true;
                    }
                }
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetIf(List<Token> t, int s, out int e, out Statement n)
        {
            StatementIf m;

            if (t[s].type == TokenType.TK_IF)
            {
                if ((s + 1 < t.Count) && (Expression.TryGet(t, s + 1, out int ex, out Expression n2)))
                {
                    s = ex;
                    if ((s + 1 < t.Count) && (t[s].type == TokenType.TK_THEN) && (Block.TryGet(t, s + 1, out ex, out Block b)))
                    {
                        m = new();
                        m.if_exp = n2;
                        m.if_block = b;
                        s = ex;
                        // elseif
                        while ((s < t.Count) && (t[s].type == TokenType.TK_ELSEIF))
                        {
                            if (s + 1 < t.Count)
                            {
                                if (Expression.TryGet(t, s + 1, out ex, out n2))
                                {
                                    s = ex;
                                    if (s + 1 < t.Count)
                                    {
                                        if ((t[s].type == TokenType.TK_THEN) && (Block.TryGet(t, s + 1, out ex, out b)))
                                        {
                                            m.optional_elseif_exp.Add(n2);
                                            m.optional_elseif_block.Add(b);
                                            s = ex;
                                            continue;
                                        }
                                    }
                                }
                            }
                            goto fail;
                        }
                        // else
                        if ((s < t.Count) && (t[s].type == TokenType.TK_ELSE))
                        {
                            if (s + 1 < t.Count)
                            {
                                if (Block.TryGet(t, s + 1, out ex, out b))
                                {
                                    m.optional_else_block = b;
                                    s = ex;
                                }
                                else
                                {
                                    goto fail;
                                }
                            }
                            else
                            {
                                goto fail;
                            }
                        }
                        n = m;
                        e = s;
                        return true;
                    }
                }
            }
        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetReturn(List<Token> t, int s, out int e, out Statement n)
        {
            StatementReturn m;

            if (t[s].type == TokenType.TK_RETURN)
            {
                m = new();
                if ((s + 1 < t.Count) && (ExpressionList.TryGet(t, s + 1, out int ex, out ExpressionList expl)))
                {
                    m.optional_explist = expl;
                    s = ex;
                    n = m;
                    e = s;
                    return true;
                }
                s++;
                n = m;
                e = s;
                return true;
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetBreak(List<Token> t, int s, out int e, out Statement n)
        {
            StatementBreak m;

            if (t[s].type == TokenType.TK_BREAK)
            {
                m = new();
                s++;
                n = m;
                e = s;
                return true;
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetForNumber(List<Token> t, int s, out int e, out Statement n)
        {
            StatementForNumber m;

            if (t[s].type == TokenType.TK_FOR)
            {
                if ((s + 3 < t.Count) && (t[s + 1].type == TokenType.TK_NAME) && (t[s + 2].type == TokenType.TK_ASSIGN)
                    && (Expression.TryGet(t, s + 3, out int ex, out Expression n2)))
                {
                    string vn = t[s + 1].Get<string>();
                    s = ex;
                    if ((s + 1 < t.Count) && (t[s].type == TokenType.TK_COMMA) && (Expression.TryGet(t, s + 1, out ex, out Expression n3)))
                    {
                        m = new();
                        m.varname = vn;
                        m.for_start = n2;
                        m.for_end = n3;
                        s = ex;
                        if ((s < t.Count) && (t[s].type == TokenType.TK_COMMA))
                        {
                            if ((s + 1 < t.Count) && (Expression.TryGet(t, s + 1, out ex, out Expression n4)))
                            {
                                m.optional_for_step = n4;
                                s = ex;
                            }
                            else
                            {
                                goto fail;
                            }
                        }

                        if ((s + 1 < t.Count) && (t[s].type == TokenType.TK_DO) && (Block.TryGet(t, s + 1, out ex, out Block b)))
                        {
                            m.block = b;
                            s = ex;
                            if ((s < t.Count) && (t[s].type == TokenType.TK_END))
                            {
                                s++;
                                n = m;
                                e = s;
                                return true;
                            }
                        }
                    }
                }
            }
        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetForTable(List<Token> t, int s, out int e, out Statement n)
        {
            StatementForTable m;

            if (t[s].type == TokenType.TK_FOR)
            {
                if ((s + 5 < t.Count) && (t[s + 1].type == TokenType.TK_NAME) && (t[s + 2].type == TokenType.TK_COMMA) && (t[s + 3].type == TokenType.TK_NAME)
                    && (t[s + 4].type == TokenType.TK_IN) && (Expression.TryGet(t, s + 5, out int ex, out Expression n2)))
                {
                    m = new();
                    m.name_index = t[s + 1].Get<string>();
                    m.name_value = t[s + 3].Get<string>();
                    m.expression = n2;
                    s = ex;
                    if ((s + 1 < t.Count) && (t[s].type == TokenType.TK_DO) && (Block.TryGet(t, s + 1, out ex, out Block b)))
                    {
                        m.block = b;
                        s = ex;
                        if ((s < t.Count) && (t[s].type == TokenType.TK_END))
                        {
                            s++;
                            n = m;
                            e = s;
                            return true;
                        }
                    }
                }
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetFunctionCall(List<Token> t, int s, out int e, out Statement n)
        {
            StatementFunctionCall m;

            if (FunctionCall.TryGet(t, s, out int ex, out FunctionCall f))
            {
                m = new();
                m.fc = f;
                s = ex;
                n = m;
                e = s;
                return true;
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetLocal(List<Token> t, int s, out int e, out Statement n)
        {
            StatementLocal m;

            if (t[s].type == TokenType.TK_LOCAL)
            {
                if ((s + 1 < t.Count) && (Declist.TryGet(t, s, out int ex, out Declist d)))
                {
                    m = new();
                    m.dl = d;
                    s = ex;
                    if ((s < t.Count) && (t[s].type == TokenType.TK_ASSIGN))
                    {
                        if ((s + 1 < t.Count) && (ExpressionList.TryGet(t, s + 1, out ex, out ExpressionList expl)))
                        {
                            m.optional_el = expl;
                            s = ex;
                        }
                        else
                        {
                            goto fail;
                        }
                    }
                    n = m;
                    e = s;
                    return true;
                }
            }
        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetFunction(List<Token> t, int s, out int e, out Statement n)
        {
            StatementFunction m;

            if (t[s].type == TokenType.TK_FUNCTION)
            {
                if ((s + 1 >= t.Count) || (!FuncName.TryGet(t, s + 1, out int ex, out FuncName fn)))
                {
                    goto fail;
                }

                s = ex;
                if ((s + 1 >= t.Count) || (t[s].type != TokenType.TK_RNDBRACKET_OPEN))
                {
                    goto fail;
                }

                m = new();
                m.funcname = fn;
                if (!ParameterList.TryGet(t, s + 1, out ex, out m.parameters))
                {
                    goto fail;
                }

                s = ex;
                if ((s + 1 >= t.Count) || (t[s].type != TokenType.TK_RNDBRACKET_CLOSE))
                {
                    goto fail;
                }
                if (!Block.TryGet(t, s + 1, out ex, out m.block))
                {
                    goto fail;
                }
                s = ex;

                if ((s >= t.Count) || (t[s].type != TokenType.TK_END))
                {
                    goto fail;
                }
                s += 1;
                n = m;
                e = s;
                return true;
            }
        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class StatementDoBlockEnd: Statement
    {
        public Block block;
    }

    public class StatementAssignment: Statement
    {
        public Varlist vl;
        public ExpressionList el;
    }

    public class StatementWhile : Statement
    {
        public Expression exp;
        public Block block;
    }

    public class StatementRepeat : Statement
    {
        public Expression exp;
        public Block block;
    }

    public class StatementIf: Statement
    {
        public Expression if_exp;
        public Block if_block;
        public List<Expression> optional_elseif_exp = new();
        public List<Block> optional_elseif_block = new();
        public Block optional_else_block;
    }

    public class StatementReturn: Statement
    {
        public ExpressionList optional_explist;
    }

    public class StatementBreak: Statement
    {
        
    }

    public class StatementForNumber: Statement
    {
        public string varname;
        public Expression for_start;
        public Expression for_end;
        public Expression optional_for_step;
        public Block block;
    }

    public class StatementForTable : Statement
    {
        public string name_index;
        public string name_value;
        public Expression expression;
        public Block block;
    }

    public class StatementFunctionCall: Statement
    {
        public FunctionCall fc;
    }

    public class StatementLocal: Statement
    {
        public Declist dl;
        public ExpressionList optional_el;
    }

    public class StatementFunction: Statement
    {
        public FuncName funcname;
        public ParameterList parameters;
        public Block block;
    }

    public class Declist
    {
        public List<string> names = new();
        
        public static bool TryGet(List<Token> t, int s, out int e, out Declist n)
        {
            if (t[s].type == TokenType.TK_STRING)
            {
                n = new();
                n.names.Add(t[s].Get<string>());
                s++;
                while ((s < t.Count) && (t[s].type == TokenType.TK_COMMA))
                {
                    if ((s + 1 < t.Count) && (t[s + 1].type == TokenType.TK_STRING))
                    {
                        n.names.Add(t[s + 1].Get<string>());
                        s += 2;
                        continue;
                    }
                    goto fail;
                }
                e = s;
                return true;
            }
        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }


    public class Varlist
    {
        public List<Var> vars = new();

        public static bool TryGet(List<Token> t, int s, out int e, out Varlist n)
        {
            if(Var.TryGet(t, s, out int ex, out Var v))
            {
                n = new();
                n.vars.Add(v);
                s = ex;
                while ((s < t.Count) && (t[s].type == TokenType.TK_COMMA))
                {
                    if ((s + 1 < t.Count) && (Var.TryGet(t, s + 1, out ex, out v)))
                    {
                        n.vars.Add(v);
                        s = ex;
                        continue;
                    }
                    goto fail;
                }
                e = s;
                return true;
            }
        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class Var
    {
        public string name;
        public List<VarOrFuncTail> optional_vof_tail;
        public VarTail optional_tail;

        public static bool TryGet(List<Token> t, int s, out int e, out Var n)
        {
            if (t[s].type == TokenType.TK_NAME)
            {
                n = new();
                n.name = t[s].Get<string>();
                s++;
                int last_tail_index = SFEngine.Utility.NO_INDEX;
                while (s < t.Count)
                {
                    if (VarOrFuncTail.TryGet(t, s, out int ex, out VarOrFuncTail voft))
                    {
                        last_tail_index = s;
                        s = ex;
                        n.optional_vof_tail.Add(voft);
                    }
                    else
                    {
                        break;
                    }
                }
                if(last_tail_index != SFEngine.Utility.NO_INDEX)
                {
                    // convert last tail to optional if possible
                    if(VarTail.TryGet(t, last_tail_index, out int ex2, out VarTail vt))
                    {
                        n.optional_tail = vt;
                        n.optional_vof_tail.RemoveAt(n.optional_vof_tail.Count - 1);
                    }
                    else
                    {
                        goto fail;
                    }
                }
                e = s;
                return true;
            }
        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class VarTail
    {
        public static bool TryGet(List<Token> t, int s, out int e, out VarTail n)
        {
            return TryGetIndexed(t, s, out e, out n)
                || TryGetDotted(t, s, out e, out n);
        }

        public static bool TryGetIndexed(List<Token> t, int s, out int e, out VarTail n)
        {
            VarTailIndexed m;

            if (t[s].type == TokenType.TK_SQBRACKET_OPEN)
            {
                if ((s + 1 < t.Count) && (Expression.TryGet(t, s + 1, out int ex, out Expression n2)))
                {
                    s = ex;
                    if ((s < t.Count) && (t[s].type == TokenType.TK_SQBRACKET_CLOSE))
                    {
                        m = new();
                        m.expression = n2;
                        s++;
                        n = m;
                        e = s;
                        return true;
                    }
                }
            }

            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetDotted(List<Token> t, int s, out int e, out VarTail n)
        {
            VarTailDotted m;

            if (t[s].type == TokenType.TK_DOT_SINGLE)
            {
                if ((s + 1 < t.Count) && (t[s + 1].type == TokenType.TK_NAME))
                {
                    m = new();
                    m.name = t[s + 1].Get<string>();
                    s += 2;
                    n = m;
                    e = s;
                    return true;
                }
            }

            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class VarTailIndexed: VarTail
    {
        public Expression expression;
    }

    public class VarTailDotted : VarTail
    {
        public string name;
    }

    public class VarOrFunc
    {
        public string name;
        public List<VarOrFuncTail> optional_tail = new();

        public static bool TryGet(List<Token> t, int s, out int e, out VarOrFunc n)
        {
            if (t[s].type == TokenType.TK_NAME)
            {
                n = new();
                n.name = t[s].Get<string>();
                s++;
                while(s < t.Count)
                {
                    if(VarOrFuncTail.TryGet(t, s, out int ex, out VarOrFuncTail voft))
                    {
                        s = ex;
                        n.optional_tail.Add(voft);
                    }
                    else
                    {
                        break;
                    }
                }
                e = s;
                return true;
            }

            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class VarOrFuncTail
    {
        public static bool TryGet(List<Token> t, int s, out int e, out VarOrFuncTail n)
        {
            return TryGetVar(t, s, out e, out n)
                || TryGetFunc(t, s, out e, out n);
        }

        public static bool TryGetVar(List<Token> t, int s, out int e, out VarOrFuncTail n)
        {
            VarOrFuncTailVar m;

            if (VarTail.TryGet(t, s, out int ex, out VarTail v))
            {
                m = new();
                m.vt = v;
                s = ex;
                n = m;
                e = s;
                return true;
            }

            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetFunc(List<Token> t, int s, out int e, out VarOrFuncTail n)
        {
            VarOrFuncTailFunc m;

            if (FunctionCallTail.TryGet(t, s, out int ex, out FunctionCallTail fc))
            {
                m = new();
                m.fct = fc;
                s = ex;
                n = m;
                e = s;
                return true;
            }

            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class VarOrFuncTailVar : VarOrFuncTail
    {
        public VarTail vt;
    }

    public class VarOrFuncTailFunc : VarOrFuncTail
    {
        public FunctionCallTail fct;
    }

    public class FunctionCall
    {
        public string name;
        public List<VarOrFuncTail> optional_vof_tail = new();
        public FunctionCallTail tail;

        public static bool TryGet(List<Token> t, int s, out int e, out FunctionCall n)
        {
            if (t[s].type == TokenType.TK_NAME)
            {
                n = new();
                n.name = t[s].Get<string>();
                s++;
                int last_tail_index = SFEngine.Utility.NO_INDEX;
                while (s < t.Count)
                {
                    if (VarOrFuncTail.TryGet(t, s, out int ex, out VarOrFuncTail voft))
                    {
                        last_tail_index = s;
                        s = ex;
                        n.optional_vof_tail.Add(voft);
                    }
                    else
                    {
                        break;
                    }
                }
                if (last_tail_index != SFEngine.Utility.NO_INDEX)
                {
                    // convert last tail to optional if possible
                    if (FunctionCallTail.TryGet(t, last_tail_index, out int ex2, out FunctionCallTail fct))
                    {
                        n.tail = fct;
                        n.optional_vof_tail.RemoveAt(n.optional_vof_tail.Count - 1);
                        s = ex2;
                        e = s;
                        return true;
                    }
                }
            }
        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class FunctionCallTail
    {
        public static bool TryGet(List<Token> t, int s, out int e, out FunctionCallTail n)
        {
            return TryGetArgs(t, s, out e, out n)
                || TryGetMethod(t, s, out e, out n);
        }

        public static bool TryGetArgs(List<Token> t, int s, out int e, out FunctionCallTail n)
        {
            FunctionCallTailArgs m;

            if (ArgumentList.TryGet(t, s, out int ex, out ArgumentList args))
            {
                m = new();
                m.args = args;
                s = ex;
                n = m;
                e = s;
                return true;
            }

            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetMethod(List<Token> t, int s, out int e, out FunctionCallTail n)
        {
            FunctionCallTailMethod m;

            if (t[s].type == TokenType.TK_COLON)
            {
                if ((s + 2 < t.Count) && (t[s + 1].type == TokenType.TK_NAME) && (ArgumentList.TryGet(t, s + 2, out int ex, out ArgumentList args)))
                {
                    m = new();
                    m.name = t[s + 1].Get<string>();
                    m.args = args;
                    s = ex;
                    n = m;
                    e = s;
                    return true;
                }
            }

            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class FunctionCallTailArgs : FunctionCallTail
    {
        public ArgumentList args;
    }

    public class FunctionCallTailMethod : FunctionCallTail
    {
        public string name;
        public ArgumentList args;
    }


    public class Expression
    {
        public static bool TryGet(List<Token> t, int s, out int e, out Expression n)
        {
            // binary operators have ability to explode number of nodes, so gotta handle that separately...
            Expression e1;
            int ex = SFEngine.Utility.NO_INDEX;
            bool result = TryGetBrackets(t, s, out ex, out e1)
                || TryGetNil(t, s, out ex, out e1)
                || TryGetNumber(t, s, out ex, out e1)
                || TryGetLiteral(t, s, out ex, out e1)
                || TryGetUpValue(t, s, out ex, out e1)
                || TryGetUnaryOp(t, s, out ex, out e1)
                || TryGetFunction(t, s, out ex, out e1)
                || TryGetTable(t, s, out ex, out e1)
                || TryGetVarOrFunc(t, s, out ex, out e1);
            if(result)
            {
                s = ex;
                if (s < t.Count)
                {
                    switch (t[s].type)
                    {
                        case TokenType.TK_AND:
                        case TokenType.TK_OR:
                        case TokenType.TK_CONCAT:
                        case TokenType.TK_EQ:
                        case TokenType.TK_LT:
                        case TokenType.TK_LE:
                        case TokenType.TK_NE:
                        case TokenType.TK_GE:
                        case TokenType.TK_GT:
                        case TokenType.TK_ADD:
                        case TokenType.TK_SUB:
                        case TokenType.TK_MUL:
                        case TokenType.TK_DIV:
                        case TokenType.TK_EXP:
                            TokenType binop = t[s].type;
                            if (s + 1 < t.Count)
                            {
                                s += 1;

                                Expression e2;
                                result = TryGetBrackets(t, s, out ex, out e2)
                                    || TryGetNil(t, s, out ex, out e2)
                                    || TryGetNumber(t, s, out ex, out e2)
                                    || TryGetLiteral(t, s, out ex, out e2)
                                    || TryGetUpValue(t, s, out ex, out e2)
                                    || TryGetUnaryOp(t, s, out ex, out e2)
                                    || TryGetFunction(t, s, out ex, out e2)
                                    || TryGetTable(t, s, out ex, out e2)
                                    || TryGetVarOrFunc(t, s, out ex, out e2);
                                if (result)
                                {
                                    ExpressionBinaryOperator ebo = new();
                                    ebo.operator_type = binop;
                                    ebo.left = e1;
                                    ebo.right = e2;
                                    n = ebo;
                                    s = ex;
                                    e = s;
                                    return true;
                                }
                            }
                            goto fail;
                        default:
                            goto success;
                    }
                }
                else
                {
                    goto success;
                }
            }
            else
            {
                goto fail;
            }

        success:
            e = s;
            n = e1;
            return true;
        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }


        public static bool TryGetBrackets(List<Token> t, int s, out int e, out Expression n)
        {
            ExpressionBrackets m;

            if ((t[s].type == TokenType.TK_RNDBRACKET_OPEN) && (s + 1 < t.Count))
            {
                if (Expression.TryGet(t, s + 1, out int ex, out Expression n2))
                {
                    s = ex;
                    if ((s < t.Count) && (t[s].type == TokenType.TK_RNDBRACKET_CLOSE))
                    {
                        s++;
                        m = new();
                        m.inner = n2;
                        n = m;
                        e = s;
                        return true;
                    }
                }
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetNil(List<Token> t, int s, out int e, out Expression n)
        {
            ExpressionNil m;

            if (t[s].type == TokenType.TK_NIL)
            {
                m = new();
                s++;
                n = m;
                e = s;
                return true;
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }


        public static bool TryGetNumber(List<Token> t, int s, out int e, out Expression n)
        {
            ExpressionNumber m;

            if (t[s].type == TokenType.TK_NUMBER)
            {
                m = new();
                m.num = t[s].Get<double>();
                s++;
                n = m;
                e = s;
                return true;
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetLiteral(List<Token> t, int s, out int e, out Expression n)
        {
            ExpressionLiteral m;

            if (t[s].type == TokenType.TK_STRING)
            {
                m = new();
                m.str = t[s].Get<string>();
                s++;
                n = m;
                e = s;
                return true;
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }


        public static bool TryGetVarOrFunc(List<Token> t, int s, out int e, out Expression n)
        {
            ExpressionVarOrFunc m;

            if (VarOrFunc.TryGet(t, s, out int ex, out VarOrFunc v))
            {
                m = new();
                m.var = v;
                s = ex;
                n = m;
                e = s;
                return true;
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }


        public static bool TryGetUpValue(List<Token> t, int s, out int e, out Expression n)
        {
            ExpressionUpValue m;

            if (UpValue.TryGet(t, s, out int ex, out UpValue uv))
            {
                m = new();
                m.upval = uv;
                s = ex;
                n = m;
                e = s;
                return true;
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetFunction(List<Token> t, int s, out int e, out Expression n)
        {
            ExpressionFunction m;
            if (Function.TryGet(t, s, out int ex, out Function f))
            {
                m = new();
                m.func = f;
                s = ex;
                n = m;
                e = s;
                return true;
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetTable(List<Token> t, int s, out int e, out Expression n)
        {
            ExpressionTableConstructor m;

            if (TableConstructor.TryGet(t, s, out int ex, out TableConstructor tb))
            {
                m = new();
                m.tab = tb;
                s = ex;
                n = m;
                e = s;
                return true;
            }

            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetUnaryOp(List<Token> t, int s, out int e, out Expression n)
        {
            ExpressionUnaryOperator m;

            switch (t[s].type)
            {
                case TokenType.TK_NOT:
                case TokenType.TK_SUB:
                    if ((s + 1 < t.Count) && (Expression.TryGet(t, s + 1, out int ex, out Expression n2)))
                    {
                        m = new();
                        m.operator_type = t[s].type;
                        m.expression = n2;
                        s = ex;
                        n = m;
                        e = s;
                        return true;
                    }
                    goto fail;
                default:
                    goto fail;
            }
        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class ExpressionBrackets : Expression
    {
        public Expression inner;
    }

    public class ExpressionNil: Expression
    {

    }

    public class ExpressionNumber: Expression
    {
        public double num;
    }

    public class ExpressionLiteral: Expression
    {
        public string str;
    }

    public class ExpressionVarOrFunc : Expression
    {
        public VarOrFunc var;
    }

    public class ExpressionUpValue: Expression
    {
        public UpValue upval;
    }

    public class ExpressionFunction : Expression
    {
        public Function func;
    }

    public class ExpressionTableConstructor: Expression
    {
        public TableConstructor tab;
    }

    public class ExpressionBinaryOperator: Expression
    {
        public TokenType operator_type;
        public Expression left;
        public Expression right;
    }

    public class ExpressionUnaryOperator: Expression
    {
        public TokenType operator_type;
        public Expression expression;
    }

    public class ExpressionList
    {
        public List<Expression> expressions = new();

        public static bool TryGet(List<Token> t, int s, out int e, out ExpressionList n)
        {
            if(Expression.TryGet(t, s, out int ex, out Expression n2))
            {
                n = new();
                n.expressions.Add(n2);
                s = ex;
                while (true)
                {
                    if ((s + 1 < t.Count) && (t[s].type == TokenType.TK_COMMA))
                    {
                        if (!Expression.TryGet(t, s + 1, out ex, out n2))
                        {
                            goto fail;
                        }
                        n.expressions.Add(n2);
                        s = ex;
                    }
                    else
                    {
                        break;
                    }
                }
                e = s;
                return true;
            }

        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class TableConstructor
    {
        public FieldList fields;
        public static bool TryGet(List<Token> t, int s, out int e, out TableConstructor n)
        {
            if (t[s].type == TokenType.TK_CRLBRACKET_OPEN)
            {
                if ((s + 1 < t.Count) && (FieldList.TryGet(t, s + 1, out int ex, out FieldList fl)))
                {
                    s = ex;
                    if ((s < t.Count) && (t[s].type == TokenType.TK_CRLBRACKET_CLOSE))
                    {
                        n = new();
                        n.fields = fl;
                        s += 1;
                        e = s;
                        return true;
                    }
                }
            }

            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class FieldList
    {
        public FieldListValues values = new();
        public FieldListFields fields = new();

        public static bool TryGet(List<Token> t, int s, out int e, out FieldList n)
        {
            // determine which goes first: values or fields
            int start = s;
            FieldListValues flv = null;
            FieldListFields flf = null;
            if(FieldListFields.TryGet(t, start, out int ex1, out flf))
            {
                if(flf.fields.Count != 0)
                {
                    s = ex1;
                    goto read_flv2;
                }
                else if(FieldListValues.TryGet(t, start, out int ex2, out flv))
                {
                    if (flv.values.Count != 0)
                    {
                        s = ex2;
                        goto read_flf2;
                    }
                    else  // could not determine, first side must be empty
                    {
                        goto read_either2;
                    }
                }
                else  // could read fields but not values? fail
                {
                    goto fail;
                }
            }
            else if (FieldListValues.TryGet(t, start, out int ex2, out flv))
            {
                s = ex2;
                goto read_flf2;
            }
            else
            {
                goto fail;
            }

        read_flv2:
            if ((s < t.Count) && (t[s].type == TokenType.TK_SEMICOLON))
            {
                if ((s + 1 < t.Count) && (FieldListValues.TryGet(t, s + 1, out int ex3, out flv)))
                {
                    s = ex3;
                }
                else
                {
                    goto fail;
                }
            }
            goto success;

        read_flf2:
            if ((s < t.Count) && (t[s].type == TokenType.TK_SEMICOLON))
            {
                if ((s + 1 < t.Count) && (FieldListFields.TryGet(t, s + 1, out int ex3, out flf)))
                {
                    s = ex3;
                }
                else
                {
                    goto fail;
                }
            }
            goto success;
        read_either2:
            if ((s < t.Count) && (t[s].type == TokenType.TK_SEMICOLON))
            {
                if (s + 1 < t.Count)
                {
                    int start2 = s + 1; 
                    if (FieldListFields.TryGet(t, start2, out int ex4, out flf))
                    {
                        if(flf.fields.Count != 0)
                        {
                            s = ex4;
                            goto success;
                        }
                        else if (FieldListValues.TryGet(t, start2, out ex4, out flv))
                        {
                            if(flv.values.Count != 0)
                            {
                                s = ex4;
                                goto success;
                            }
                            else  // second side is empty; semicolon was last token
                            {
                                s = start2;
                                goto success;
                            }
                        }
                        else
                        {
                            goto fail;
                        }
                    }
                    else if (FieldListValues.TryGet(t, start2, out ex4, out flv))
                    {
                        s = ex4;
                        goto success;
                    }
                    else
                    {
                        goto fail;
                    }
                }
                else
                {
                    goto fail;
                }
            }
            goto success;

        success:
            n = new();
            n.values = flv;
            n.fields = flf;
            e = s;
            return true;


        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class FieldListValues
    {
        public List<Expression> values = new();

        public static bool TryGet(List<Token> t, int s, out int e, out FieldListValues n)
        {
            n = new();
            if (Expression.TryGet(t, s, out int ex, out Expression n2))
            {
                n.values.Add(n2);
                s = ex;
            }
            else
            {
                e = s;
                return true;
            }

            while ((s < t.Count) && (t[s].type == TokenType.TK_COMMA))
            {
                if ((s + 1 < t.Count) && (Expression.TryGet(t, s + 1, out ex, out n2)))
                {
                    n.values.Add(n2);
                    s = ex;
                }
                else
                {
                    break;
                }
            }
            if ((s < t.Count) && (t[s].type == TokenType.TK_COMMA))
            {
                s++;
            }
            e = s;
            return true;

        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class FieldListFields
    {
        public List<Field> fields = new();

        public static bool TryGet(List<Token> t, int s, out int e, out FieldListFields n)
        {
            n = new();
            if(Field.TryGet(t, s, out int ex, out Field f))
            {
                n.fields.Add(f);
                s = ex;
            }
            else
            {
                e = s;
                return true;
            }

            while ((s < t.Count) && (t[s].type == TokenType.TK_COMMA))
            {
                if ((s + 1 < t.Count) && (Field.TryGet(t, s + 1, out ex, out f)))
                {
                    n.fields.Add(f);
                    s = ex;
                }
                else
                {
                    break;
                }
            }
            if ((s < t.Count) && (t[s].type == TokenType.TK_COMMA))
            {
                s++;
            }
            e = s;
            return true;

        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class Field
    {
        public static bool TryGet(List<Token> t, int s, out int e, out Field n)
        {
            return TryGetExpression(t, s, out e, out n)
                || TryGetName(t, s, out e, out n);
        }

        public static bool TryGetName(List<Token> t, int s, out int e, out Field n)
        {
            FieldName m;

            if (t[s].type == TokenType.TK_NAME)
            {
                if ((s + 2 < t.Count) && (t[s + 1].type == TokenType.TK_ASSIGN) && (Expression.TryGet(t, s + 2, out int ex, out Expression n2)))
                {
                    m = new();
                    m.name = t[s].Get<string>();
                    m.expression = n2;
                    s = ex;
                    n = m;
                    e = s;
                    return true;
                }
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetExpression(List<Token> t, int s, out int e, out Field n)
        {
            FieldExpression m;

            if (t[s].type == TokenType.TK_SQBRACKET_OPEN)
            {
                if ((s + 1 < t.Count) && (Expression.TryGet(t, s + 1, out int ex, out Expression n2)))
                {
                    s = ex;
                    if ((s + 2 < t.Count) && (t[s].type == TokenType.TK_SQBRACKET_CLOSE) && (t[s + 1].type == TokenType.TK_ASSIGN)
                            && (Expression.TryGet(t, s + 2, out ex, out Expression n3)))
                    {
                        m = new();
                        m.name = n2;
                        m.expression = n3;
                        s = ex;
                        n = m;
                        e = s;
                        return true;
                    }
                }
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class FieldName: Field
    {
        public string name;
        public Expression expression;
    }

    public class FieldExpression: Field
    {
        public Expression name;
        public Expression expression;
    }

    public class ArgumentList
    {
        public static bool TryGet(List<Token> t, int s, out int e, out ArgumentList n)
        {
            return TryGetExpressionList(t, s, out e, out n)
                || TryGetTable(t, s, out e, out n)
                || TryGetLiteral(t, s, out e, out n);
        }

        public static bool TryGetExpressionList(List<Token> t, int s, out int e, out ArgumentList n)
        {
            ArgumentListExpressionList m;

            if ((t[s].type == TokenType.TK_RNDBRACKET_OPEN) && (s + 1 < t.Count))
            {
                if (ExpressionList.TryGet(t, s + 1, out int ex, out ExpressionList n2))
                {
                    s = ex;
                    if ((s < t.Count) && (t[s].type == TokenType.TK_RNDBRACKET_CLOSE))
                    {
                        m = new();
                        m.explist = n2;
                        s++;
                        n = m;
                        e = s;
                        return true;
                    }
                }
                else if (t[s + 1].type == TokenType.TK_RNDBRACKET_CLOSE)
                {
                    m = new();
                    m.explist = new();
                    s += 2;
                    n = m;
                    e = s;
                    return true;
                }
            }

            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetTable(List<Token> t, int s, out int e, out ArgumentList n)
        {
            ArgumentListTable m;

            if (TableConstructor.TryGet(t, s, out int ex, out TableConstructor tc))
            {
                m = new();
                m.tc = tc;
                s = ex;
                n = m;
                e = s;
                return true;
            }

            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetLiteral(List<Token> t, int s, out int e, out ArgumentList n)
        {
            ArgumentListLiteral m;

            if (t[s].type == TokenType.TK_STRING)
            {
                m = new();
                m.str = t[s].Get<string>();
                s++;
                n = m;
                e = s;
                return true;
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class ArgumentListExpressionList: ArgumentList
    {
        public ExpressionList explist = new();
    }

    public class ArgumentListTable: ArgumentList
    {
        public TableConstructor tc;
    }

    public class ArgumentListLiteral : ArgumentList
    {
        public string str;
    }

    public class Function
    {
        public ParameterList parameters;
        public Block block;

        public static bool TryGet(List<Token> t, int s, out int e, out Function n)
        {
            if (t[s].type == TokenType.TK_FUNCTION)
            {
                if ((s + 2 >= t.Count) || (t[s + 1].type != TokenType.TK_RNDBRACKET_OPEN))
                {
                    goto fail;
                }

                n = new();
                if(!ParameterList.TryGet(t, s+2, out int ex, out n.parameters))
                {
                    goto fail;
                }
                s = ex + 1;

                if ((s + 2 >= t.Count) || (t[s + 1].type != TokenType.TK_RNDBRACKET_CLOSE))
                {
                    goto fail;
                }
                if(!Block.TryGet(t, s+2, out ex, out n.block))
                {
                    goto fail;
                }
                s = ex + 1;

                if((s >= t.Count) || (t[s].type != TokenType.TK_END))
                {
                    goto fail;
                }
                s += 1;
                e = s;
                return true;
            }
        fail:
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class FuncName
    {
        public static bool TryGet(List<Token> t, int s, out int e, out FuncName n)
        {
            return TryGetName(t, s, out e, out n)
                || TryGetDotted(t, s, out e, out n)
                || TryGetField(t, s, out e, out n);
        }

        public static bool TryGetName(List<Token> t, int s, out int e, out FuncName n)
        {
            FuncNameName m;

            if ((t[s].type == TokenType.TK_NAME))
            {
                m = new();
                m.name = t[s].Get<string>();
                s += 1;
                n = m;
                e = s;
                return true;
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetDotted(List<Token> t, int s, out int e, out FuncName n)
        {
            FuncNameDotted m;

            if ((t[s].type == TokenType.TK_NAME) && (s + 2 < t.Count) && (t[s + 1].type == TokenType.TK_DOT_SINGLE) && (t[s + 2].type == TokenType.TK_NAME))
            {
                m = new();
                m.main = t[s].Get<string>();
                m.field = t[s + 2].Get<string>();
                s += 3;
                n = m;
                e = s;
                return true;
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }

        public static bool TryGetField(List<Token> t, int s, out int e, out FuncName n)
        {
            FuncNameField m;

            if ((t[s].type == TokenType.TK_NAME) && (s + 2 < t.Count) && (t[s + 1].type == TokenType.TK_COLON) && (t[s + 2].type == TokenType.TK_NAME))
            {
                m = new();
                m.main = t[s].Get<string>();
                m.field = t[s + 2].Get<string>();
                s += 3;
                n = m;
                e = s;
                return true;
            }
            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class FuncNameName: FuncName
    {
        public string name;
    }

    public class FuncNameDotted: FuncName
    {
        public string main;
        public string field;
    }

    public class FuncNameField: FuncName
    {
        public string main;
        public string field;
    }

    public class ParameterList
    {
        public List<string> names = new();
        public bool has_varargs;

        public static bool TryGet(List<Token> t, int s, out int e, out ParameterList n)
        {
            if (t[s].type == TokenType.TK_NAME)
            {
                n = new();
                n.names.Add(t[s].Get<string>());
                s++;
                while (true)
                {
                    if (s + 1 < t.Count)
                    {
                        if ((t[s].type == TokenType.TK_COMMA) && (t[s + 1].type == TokenType.TK_NAME))
                        {
                            n.names.Add(t[s + 1].Get<string>());
                            s += 2;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (s + 1 < t.Count)
                {
                    if ((t[s].type == TokenType.TK_COMMA) && (t[s + 1].type == TokenType.TK_DOTS))
                    {
                        n.has_varargs = true;
                        s += 2;
                    }
                }
                e = s;
                return true;
            }
            else if (t[s].type == TokenType.TK_DOTS)
            {
                n = new();
                n.has_varargs = true;
                s++;
                e = s;
                return true;
            }

            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }

    public class UpValue
    {
        public string name;

        public static bool TryGet(List<Token> t, int s, out int e, out UpValue n)
        {
            if (t[s].type == TokenType.TK_UPVALUE)
            {
                if(s+1 < t.Count)
                {
                    if (t[s+1].type == TokenType.TK_NAME)
                    {
                        n = new();
                        n.name = t[s + 1].Get<string>();
                        s += 2;
                        e = s;
                        return true;
                    }
                }
            }

            e = SFEngine.Utility.NO_INDEX;
            n = null;
            return false;
        }
    }





    public class Parser
    {
        public Block parsed_script;

        public Parser(string c)
        {
            // step 1 of parsing scripts: turn sequence of characters into sequence of tokens
            List<Token> tokens = new();
            string result_s;
            double result_d;

            int s = 0;
            while(true)
            {
                if(s == c.Length)
                {
                    tokens.Add(new() { type = TokenType.TK_EOS });
                    break;
                }

                switch(c[s])
                {
                    case ' ':
                    case '\t':
                    case '\r':
                        break;
                    case '\n':
                        break;
                    case '$':
                    case '!':
                    case '&':
                    case '#':
                    case '`':
                    case '@':
                    case '?':
                    case '\\':
                        break;
                    case '+':
                        tokens.Add(new() { type = TokenType.TK_ADD });
                        break;
                    case '-':
                        if ((s + 1 < c.Length) && (c[s + 1] == '-'))
                        {
                            s = ReadCommentString(ref c, s, out result_s);
                            // comments are excluded from tokenlist parsed by the parser
                            // tokens.Add(new() { type = TokenType.TK_COMMENT, value = new string(result_s) });
                        }
                        else
                        {
                            tokens.Add(new() { type = TokenType.TK_SUB });
                        }
                        break;
                    case '*':
                        tokens.Add(new() { type = TokenType.TK_MUL });
                        break;
                    case '/':
                        tokens.Add(new() { type = TokenType.TK_DIV });
                        break;
                    case '^':
                        tokens.Add(new() { type = TokenType.TK_EXP });
                        break;
                    case '(':
                        tokens.Add(new() { type = TokenType.TK_RNDBRACKET_OPEN });
                        break;
                    case ')':
                        tokens.Add(new() { type = TokenType.TK_RNDBRACKET_CLOSE });
                        break;
                    case '{':
                        tokens.Add(new() { type = TokenType.TK_CRLBRACKET_OPEN });
                        break;
                    case '}':
                        tokens.Add(new() { type = TokenType.TK_CRLBRACKET_CLOSE });
                        break;
                    case '[':
                        if ((s + 1 < c.Length) && (c[s + 1] == '['))
                        {
                            s = ReadLongString(ref c, s, out result_s);
                            tokens.Add(new() { type = TokenType.TK_STRING, value = new string(result_s) });
                        }
                        else
                        {
                            tokens.Add(new() { type = TokenType.TK_SQBRACKET_OPEN });
                        }
                        break;
                    case ']':
                        tokens.Add(new() { type = TokenType.TK_SQBRACKET_CLOSE });
                        break;
                    case '%':
                        tokens.Add(new() { type = TokenType.TK_UPVALUE });
                        break;
                    case '=':
                        if ((s + 1 < c.Length) && (c[s + 1] == '='))
                        {
                            tokens.Add(new() { type = TokenType.TK_EQ });
                            s++;
                        }
                        else
                        {
                            tokens.Add(new() { type = TokenType.TK_ASSIGN });
                        }
                        break;
                    case '<':
                        if ((s + 1 < c.Length) && (c[s + 1] == '='))
                        {
                            tokens.Add(new() { type = TokenType.TK_LE });
                            s++;
                        }
                        else
                        {
                            tokens.Add(new() { type = TokenType.TK_LT });
                        }
                        break;
                    case '>':
                        if ((s + 1 < c.Length) && (c[s + 1] == '='))
                        {
                            tokens.Add(new() { type = TokenType.TK_GE });
                            s++;
                        }
                        else
                        {
                            tokens.Add(new() { type = TokenType.TK_GT });
                        }
                        break;
                    case '~':
                        if ((s + 1 < c.Length) && (c[s + 1] == '='))
                        {
                            tokens.Add(new() { type = TokenType.TK_NE });
                            s++;
                        }
                        break;
                    case '"':
                    case '\'':
                        s = ReadString(ref c, s, out result_s);
                        tokens.Add(new() { type = TokenType.TK_STRING, value = new string(result_s) });
                        break;
                    case '.':
                        if (s + 1 < c.Length)
                        {
                            if (c[s + 1] == '.')
                            {
                                if (s + 2 < c.Length)
                                {
                                    if (c[s + 2] == '.')
                                    {
                                        tokens.Add(new() { type = TokenType.TK_DOTS });
                                        s += 2;
                                    }
                                    else
                                    {
                                        tokens.Add(new() { type = TokenType.TK_CONCAT });
                                        s += 1;
                                    }
                                }
                                else
                                {
                                    tokens.Add(new() { type = TokenType.TK_CONCAT });
                                    s += 1;
                                }
                            }
                            else
                            {
                                if ((c[s + 1] >= '0') && (c[s + 1] <= '9'))
                                {
                                    s = ReadNumber(ref c, s, out result_d);
                                    tokens.Add(new() { type = TokenType.TK_NUMBER, value = result_d });
                                }
                            }
                        }
                        else
                        {
                            tokens.Add(new() { type = TokenType.TK_DOT_SINGLE });
                        }
                        break;
                    case ':':
                        tokens.Add(new() { type = TokenType.TK_COLON });
                        break;
                    case ';':
                        tokens.Add(new() { type = TokenType.TK_SEMICOLON });
                        break;
                    case ',':
                        tokens.Add(new() { type = TokenType.TK_COMMA });
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        s = ReadNumber(ref c, s, out result_d);
                        tokens.Add(new() { type = TokenType.TK_NUMBER, value = result_d });
                        break;
                    case '_':
                        goto tname;
                    default:
                        if (!char.IsAsciiLetter(c[s]))
                        {
                            if(char.IsControl(c[s]))
                            {
                                throw new Exception("Tokenizer(): invalid character");
                            }
                            break;
                        }
                    tname:
                        s = ReadNameString(ref c, s, out result_s);
                        // check reserved words
                        switch (result_s)
                        {
                            case "and":
                                tokens.Add(new() { type = TokenType.TK_AND });
                                break;
                            case "break":
                                tokens.Add(new() { type = TokenType.TK_BREAK });
                                break;
                            case "do":
                                tokens.Add(new() { type = TokenType.TK_DO });
                                break;
                            case "else":
                                tokens.Add(new() { type = TokenType.TK_ELSE });
                                break;
                            case "elseif":
                                tokens.Add(new() { type = TokenType.TK_ELSEIF });
                                break;
                            case "end":
                                tokens.Add(new() { type = TokenType.TK_END });
                                break;
                            case "for":
                                tokens.Add(new() { type = TokenType.TK_FOR });
                                break;
                            case "function":
                                tokens.Add(new() { type = TokenType.TK_FUNCTION });
                                break;
                            case "if":
                                tokens.Add(new() { type = TokenType.TK_IF });
                                break;
                            case "in":
                                tokens.Add(new() { type = TokenType.TK_IN });
                                break;
                            case "local":
                                tokens.Add(new() { type = TokenType.TK_LOCAL });
                                break;
                            case "nil":
                                tokens.Add(new() { type = TokenType.TK_NIL });
                                break;
                            case "not":
                                tokens.Add(new() { type = TokenType.TK_NOT });
                                break;
                            case "or":
                                tokens.Add(new() { type = TokenType.TK_OR });
                                break;
                            case "repeat":
                                tokens.Add(new() { type = TokenType.TK_REPEAT });
                                break;
                            case "return":
                                tokens.Add(new() { type = TokenType.TK_RETURN });
                                break;
                            case "then":
                                tokens.Add(new() { type = TokenType.TK_THEN });
                                break;
                            case "until":
                                tokens.Add(new() { type = TokenType.TK_UNTIL });
                                break;
                            case "while":
                                tokens.Add(new() { type = TokenType.TK_WHILE });
                                break;
                            default:
                                tokens.Add(new() { type = TokenType.TK_NAME, value = new string(result_s) });
                                break;
                        }

                        break;
                }

                s++;
            }

            // step 2: turn sequence of tokens into a tree that satisfies the description below
            /*
             x* chunk := {<stat> [;]}
             x* block := <chunk>
             x* stat := do <block> end
             x* stat := <varlist> = <explist>
             x* stat := while <exp> do <block> end
             x* stat := repeat <block> until <exp>
             x* stat := if <exp> then <block> {elseif <exp> then <block>} [else <block>] end
             x* stat := return [<explist>]
             x* stat := break
             x* stat := for <name> = <exp> , <exp> [, <exp>] do <block> end
             x* stat := for <name> , <name> in <exp> do <block> end
             x* stat := functioncall
             x* stat := local <declist> [= <explist>]
             x* stat := function <funcname> ( <parlist> ) <block> end
             x* varlist := <var> {, <var>}
             x* declist := <name> {, <name>}
             x* varorfunc = <name> { <vftail> }
             x* functioncall := <name> { <vftail> } <fctail>
             x* var := <name> [{ <vftail> } <vtail> ]
             x* vftail := <vtail>
             x* vftail := <fctail>
             x* vtail := \[ <exp> \]
             x* vtail := . <name>
             x* fctail := <args>
             x* fctail := : <name> <args>
             x* exp := ( <exp> )
             x* exp := <nil>
             x* exp := <number>
             x* exp := <literal>
             x* exp := <varorfunc>
             x* exp := <upvalue>
             x* exp := <function>
             x* exp := <tableconstructor>
             x* exp := <exp> <binop> <exp>
             x* exp := <unop> <exp>
             x* explist := <exp> {, <exp>}
             x* tableconstructor := { <fieldlist> }
             x* fieldlist := <lfieldlist> [; <ffieldlist>]
             x* fieldlist := <ffieldlist> [; <lfieldlist>]
             x* lfieldlist := [<exp> {, <exp>} [,]]
             x* ffieldlist := [<ffield> {, <ffield>} [,]]
             x* ffield := \[ <exp> \] = <exp>
             x* ffield := <name> = <exp>
             x* args := ( [<explist>] )
             x* args := <tableconstructor>
             x* args := <literal>
             x* function := function ( <parlist> ) <block> end
             x* funcname := <name>
             x* funcname := <name> . <name>
             x* funcname := <name> : <name>
             x* parlist := ...
             x* parlist := <name> {, <name>} [, ...]
             x* upvalue := % <name>
             * */
            if(!Block.TryGet(tokens, 0, out int parsed_num, out parsed_script))
            {
                throw new Exception("Parser(): Failed to parse token list");
            }
        }

        private int ReadNameString(ref string c, int s, out string result)
        {
            int start = s;
            while (true)
            {
                if (s == c.Length)
                {
                    break;
                }

                if ((char.IsAsciiLetterOrDigit(c[s])) || (c[s] == '_'))
                {
                    s++;
                    continue;
                }
                break;
            }
            result = c.Substring(start, s - start);
            s -= 1;
            return s;
        }

        private int ReadNumber(ref string c, int s, out double result)
        {
            int start = s;
            int dotpos = -1;
            int exppos = -1;
            bool stop = false;
            while(s < c.Length)
            {
                switch(c[s])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        s++;
                        break;
                    case '.':
                        if(dotpos == s-1)
                        {
                            throw new Exception("Tokenizer.ReadNumber(): invalid number");
                        }
                        else if(dotpos == -1)
                        {
                            if(exppos != -1)
                            {
                                stop = true;
                                break;
                            }
                            dotpos = s;
                            s++;
                        }
                        else
                        {
                            stop = true;
                            break;
                        }
                        break;
                    case 'e':
                    case 'E':
                        if(exppos != -1)
                        {
                            stop = true;
                            break;
                        }
                        exppos = s;
                        s++;
                        break;
                    case '+':
                    case '-':
                        if(exppos != s-1)
                        {
                            stop = true;
                            break;
                        }
                        s++;
                        break;
                    default:
                        stop = true;
                        break;
                }
                if(stop)
                {
                    break;
                }
            }
            double.TryParse(c.Substring(start, s - start), out result);
            return s - 1;
        }

        private int ReadString(ref string c, int s, out string result)
        {
            char lim = c[s];
            s += 1;
            StringWriter sw = new();
            while (true)
            {
                if (s == c.Length)
                {
                    throw new Exception("Tokenizer.ReadString(): Unfinished string");
                }
                if (c[s] == lim)
                {
                    break;
                }

                switch (c[s])
                {
                    case '\\':
                        if (s + 1 < c.Length)
                        {
                            switch (c[s + 1])
                            {
                                case 'a':
                                    sw.Write('\a');
                                    break;
                                case 'b':
                                    sw.Write('\b');
                                    break;
                                case 'f':
                                    sw.Write('\f');
                                    break;
                                case 'n':
                                    sw.Write('\n');
                                    break;
                                case 'r':
                                    sw.Write('\r');
                                    break;
                                case 't':
                                    sw.Write('\t');
                                    break;
                                case 'v':
                                    sw.Write('\v');
                                    break;
                                case '\n':
                                    sw.Write('\n');
                                    break;
                                case '0':
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                case '6':
                                case '7':
                                case '8':
                                case '9':
                                    int cc = 0;
                                    int ii = 0;
                                    do
                                    {
                                        cc = 10 * cc + (c[s + 1] - '0');
                                        s++;
                                        ii++;
                                        if(ii >= 3)
                                        {
                                            break;
                                        }
                                        if(s + 1 >= c.Length)
                                        {
                                            break;
                                        }
                                        if ((c[s + 1] < '0') || (c[s + 1] > '9'))
                                        {
                                            break;
                                        }
                                    }
                                    while (true);
                                    if(cc >= 256)
                                    {
                                        throw new Exception("Tokenizer.ReadString(): invalid character escape sequence");
                                    }
                                    sw.Write((char)cc);
                                    s++;
                                    break;
                                default:
                                    sw.Write(c[s]);
                                    s++;
                                    sw.Write(c[s]);
                                    break;
                            }
                        }
                        break;
                    default:
                        sw.Write(c[s]);
                        break;
                }
                s++;
            }
            result = sw.ToString();
            return s;
        }

        private int ReadLongString(ref string c, int s, out string result)
        {
            int cont = 0;
            s += 2;
            cont += 1;
            int start = s;
            while (true)
            {
                if (s == c.Length)
                {
                    throw new Exception("Tokenizer.ReadLongString(): Unfinished string");
                }

                switch (c[s])
                {
                    case '[':
                        if ((s + 1 < c.Length) && (c[s + 1] == '['))
                        {
                            cont += 1;
                        }
                        break;
                    case ']':
                        if ((s + 1 < c.Length) && (c[s + 1] == ']'))
                        {
                            cont -= 1;
                            if (cont == 0)
                            {
                                goto endloop;
                            }
                        }
                        break;
                    default:
                        break;
                }

                s++;
            }
        endloop:
            result = c.Substring(start, s - start);
            s += 1;
            return s;
        }

        private int ReadCommentString(ref string c, int s, out string result)
        {
            s += 2;
            int start = s;
            while (true)
            {
                if (s == c.Length)
                {
                    goto endloop;
                }

                switch (c[s])
                {
                    case '\n':
                        goto endloop;
                    default:
                        break;
                }

                s++;
            }
        endloop:
            result = c.Substring(start, s - start);
            s -= 1;
            return s;
        }
    }
}
