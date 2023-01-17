using System;
using System.Collections.Generic;
using System.IO;

namespace SFEngine.SFLua.LuaDecompiler
{
    public enum OperatorType
    {
        ADD, SUB, MUL, DIV, POW, CONCAT, MINUS,
        OR, AND, TRUE, FALSE,
        EQ, LE, LT, NE, GT, GE
    }

    public interface IOperatorUnary : INode
    {

    }

    public interface IOperatorBinary : INode
    {

    }

    public interface IOperatorArithmetic : INode
    {

    }

    public interface IOperatorLogic : INode
    {
        void ReverseOperator();
    }

    public class Operator : Node, IRValue
    {
        public OperatorType op_type;
        public bool HasBraces = true;

        public override string ToString()
        {
            return "OPERATOR " + op_type.ToString();
        }
    }

    public class OperatorBinaryArithmetic : Operator, IOperatorBinary, IOperatorArithmetic
    {
        public List<IRValue> Values = new List<IRValue>();

        public override void WriteLuaString(StringWriter sw)
        {
            if (HasBraces)
            {
                sw.Write("(");
            }

            for (int i = 0; i < Values.Count; i++)
            {
                Values[i].WriteLuaString(sw);
                if (i != Values.Count - 1)
                {
                    sw.Write(" ");
                    switch (op_type)
                    {
                        case OperatorType.ADD:
                            sw.Write("+");
                            break;
                        case OperatorType.SUB:
                            sw.Write("-");
                            break;
                        case OperatorType.MUL:
                            sw.Write("*");
                            break;
                        case OperatorType.DIV:
                            sw.Write("/");
                            break;
                        case OperatorType.POW:
                            sw.Write("^");
                            break;
                        case OperatorType.CONCAT:
                            sw.Write("..");
                            break;
                        default:
                            throw new Exception("OperatorBinaryArithmetic.WriteLuaString(): invalid operator type");
                    }
                    sw.Write(" ");
                }
            }

            if (HasBraces)
            {
                sw.Write(")");
            }
        }
    }

    public class OperatorUnaryArithmetic : Operator, IOperatorUnary, IOperatorArithmetic
    {
        public IRValue Value;

        public override void WriteLuaString(StringWriter sw)
        {
            if (HasBraces)
            {
                sw.Write("(");
            }

            switch (op_type)
            {
                case OperatorType.MINUS:
                    sw.Write("-");
                    break;
                default:
                    throw new Exception("OperatorUnaryArithmetic.WriteLuaString(): invalid operator type");
            }
            Value.WriteLuaString(sw);

            if (HasBraces)
            {
                sw.Write(")");
            }
        }
    }

    public class OperatorBinaryLogic : Operator, IOperatorBinary, IOperatorLogic
    {
        public List<IRValue> Values = new List<IRValue>();

        public override void WriteLuaString(StringWriter sw)
        {
            if (HasBraces)
            {
                sw.Write("(");
            }

            for (int i = 0; i < Values.Count; i++)
            {
                Values[i].WriteLuaString(sw);
                if (i != Values.Count - 1)
                {
                    sw.Write(" ");
                    switch (op_type)
                    {
                        case OperatorType.OR:
                            sw.Write("or");
                            break;
                        case OperatorType.AND:
                            sw.Write("and");
                            break;
                        case OperatorType.EQ:
                            sw.Write("==");
                            break;
                        case OperatorType.LE:
                            sw.Write("<=");
                            break;
                        case OperatorType.LT:
                            sw.Write("<");
                            break;
                        case OperatorType.NE:
                            sw.Write("~=");
                            break;
                        case OperatorType.GT:
                            sw.Write(">");
                            break;
                        case OperatorType.GE:
                            sw.Write(">=");
                            break;
                        default:
                            throw new Exception("OperatorBinaryLogic.WriteLuaString(): invalid operator type");
                    }
                    sw.Write(" ");
                }
            }

            if (HasBraces)
            {
                sw.Write(")");
            }
        }

        public void ReverseOperator()
        {
            switch (op_type)
            {
                case OperatorType.OR:
                    op_type = OperatorType.AND;
                    break;
                case OperatorType.AND:
                    op_type = OperatorType.OR;
                    break;
                case OperatorType.EQ:
                    op_type = OperatorType.NE;
                    break;
                case OperatorType.LE:
                    op_type = OperatorType.GT;
                    break;
                case OperatorType.LT:
                    op_type = OperatorType.GE;
                    break;
                case OperatorType.NE:
                    op_type = OperatorType.EQ;
                    break;
                case OperatorType.GE:
                    op_type = OperatorType.LT;
                    break;
                case OperatorType.GT:
                    op_type = OperatorType.LE;
                    break;
                default:
                    throw new Exception("OperatorBinaryLogic.ReverseOperator(): Invalid operator type!");
            }
            foreach (IRValue v in Values)
            {
                if (v is IOperatorLogic)
                {
                    ((IOperatorLogic)v).ReverseOperator();
                }
            }
        }
    }

    public class OperatorUnaryLogic : Operator, IOperatorUnary, IOperatorLogic
    {
        public IRValue Value;

        public override void WriteLuaString(StringWriter sw)
        {
            if (HasBraces)
            {
                sw.Write("(");
            }

            switch (op_type)
            {
                case OperatorType.TRUE:
                    sw.Write("");
                    break;
                case OperatorType.FALSE:
                    sw.Write("not ");
                    break;
                default:
                    throw new Exception("OperatorUnaryLogic.WriteLuaString(): invalid operator type");
            }
            Value.WriteLuaString(sw);

            if (HasBraces)
            {
                sw.Write(")");
            }
        }

        public void ReverseOperator()
        {
            switch (op_type)
            {
                case OperatorType.TRUE:
                    op_type = OperatorType.FALSE;
                    break;
                case OperatorType.FALSE:
                    op_type = OperatorType.TRUE;
                    break;
                default:
                    throw new Exception("OperatorUnaryLogic.ReverseOperator(): Invalid operator type!");
            }

            if (Value is IOperatorLogic)
            {
                ((IOperatorLogic)Value).ReverseOperator();
            }
        }
    }
}
