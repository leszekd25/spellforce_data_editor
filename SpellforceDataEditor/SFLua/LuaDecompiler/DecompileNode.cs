using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SpellforceDataEditor.SFLua.LuaDecompiler
{
    public enum DecompileNodeType
    {
        CHUNK, COND, IDENTIFIER, FUNCDEF, FOREACHLOOP, UPIDENTIFIER, REPEATLOOP,
        UNOPERATOR, MULTOPERATOR, VALUE, INDEXED_VALUE, FUNCTION, SELFIDENTIFIER,
        TABLE, VARIABLE, FORLOOP, RETURN, WHILE, CONTINUELOOP, BREAKLOOP
    }
    public enum DecompileDataType { NIL, NUMBER, STRING, LIST, DICT, FUNCTION, USERDATA }
    public class DecompileNode
    {
        public DecompileNodeType nodetype;
        public DecompileDataType datatype;
        public object data;
        public DecompileNode Parent = null;
        public List<DecompileNode> Children = new List<DecompileNode>();
        public int instruction_id = -1;

        public DecompileNode(int id, DecompileNodeType nt, DecompileDataType dt = DecompileDataType.NIL, object d = null)
        {
            instruction_id = id;
            nodetype = nt;
            datatype = dt;
            data = d;
        }

        public void AddNode(DecompileNode n, int pos = -1)
        {
            if (!Children.Contains(n))
            {
                if (pos == -1)
                    Children.Add(n);
                else
                    Children.Insert(pos, n);
            }
            n.Parent = this;
        }

        public void RemoveNode(DecompileNode n)
        {
            n.Parent = null;
            if(Children.Contains(n))
                Children.Remove(n);
        }

        public void SetParent(DecompileNode n)
        {
            if (Parent != null)
                Parent.RemoveNode(this);
            if (n != null)
                n.AddNode(this);
        }

        public string ToScript()
        {
            StringWriter sw = new StringWriter();
            WriteLuaString(sw);
            return sw.ToString();
        }

        public void ReverseLogicalOperator()
        {
            if(nodetype == DecompileNodeType.MULTOPERATOR)
            {
                string s = data.ToString();
                switch (s)
                {
                    case "==":
                        data = "~=";
                        break;
                    case "~=":
                        data = "==";
                        break;
                    case ">=":
                        data = "<";
                        break;
                    case "<=":
                        data = ">";
                        break;
                    case ">":
                        data = "<=";
                        break;
                    case "<":
                        data = ">=";
                        break;
                }
            }
            else if(nodetype == DecompileNodeType.UNOPERATOR)
            {
                string s = data.ToString();
                switch(s)
                {
                    case "":
                        data = "not ";
                        break;
                    case "not ":
                        data = "";
                        break;
                }
            }
        }

        public void WriteLuaString(StringWriter sw, int tab_count = 0)
        {
            System.Diagnostics.Debug.WriteLine(ToString()+" TABC "+tab_count.ToString());
            switch(nodetype)
            {
                case DecompileNodeType.CHUNK:
                    foreach (DecompileNode n in Children)
                    {
                        n.WriteLuaString(sw, tab_count);
                        sw.WriteLine();
                    }
                    break;
                case DecompileNodeType.RETURN:
                    sw.Write(Utility.TabulateString("return ", tab_count));
                    for(int i=0;i<Children.Count; i++)
                    {
                        DecompileNode n = Children[i];
                        n.WriteLuaString(sw, tab_count);
                        if(i!=Children.Count-1)
                            sw.Write(", ");
                    }
                    break;
                case DecompileNodeType.FUNCDEF:
                    sw.Write(Utility.TabulateString("function ", tab_count));
                    ((DecompileNode)data).WriteLuaString(sw, tab_count);
                    sw.Write("(", tab_count);
                    DecompileNode funcdef_args = (DecompileNode)Children[0].data;
                    for(int i=0;i<funcdef_args.Children.Count; i++)
                    {
                        sw.Write(funcdef_args.Children[i].data.ToString());
                        if (i != funcdef_args.Children.Count - 1)
                            sw.Write(", ");
                    }
                    sw.WriteLine(")");
                    Children[0].WriteLuaString(sw, tab_count + 1);
                    sw.Write(Utility.TabulateString("end", tab_count));
                    break;
                case DecompileNodeType.FUNCTION:
                    if (Parent != null)  // workaround...
                    {
                        if ((Parent.nodetype == DecompileNodeType.CHUNK) || (Parent.nodetype == DecompileNodeType.TABLE))
                            for (int i = 0; i < tab_count; i++)
                                sw.Write("\t");
                    }

                    ((DecompileNode)data).WriteLuaString(sw, tab_count);
                    if (Children.Count == 0)
                    {
                        sw.Write("()");
                        break;
                    }
                    else if (Children.Count == 1)
                    {
                        if (Children[0].nodetype == DecompileNodeType.TABLE)
                        {

                            if (Children[0].datatype == DecompileDataType.LIST)
                            {
                                sw.Write("(");
                                foreach (DecompileNode n in Children[0].Children)
                                {
                                    n.WriteLuaString(sw, tab_count);
                                    sw.Write(", ");
                                }
                                sw.Write(")");
                            }
                            else
                            {
                                Children[0].WriteLuaString(sw, tab_count);
                            }
                        }
                        else
                        {
                            sw.Write("(");
                            Children[0].WriteLuaString(sw, tab_count);
                            sw.Write(")");
                        }
                    }
                    else
                    {
                        sw.Write("(");
                        for (int i = 0; i < Children.Count; i++)
                        {
                            DecompileNode n = Children[i];
                            n.WriteLuaString(sw, tab_count);
                            if (i != Children.Count - 1)
                                sw.Write(", ");
                        }
                        sw.Write(")");
                    }
                    break;
                case DecompileNodeType.TABLE:
                    sw.WriteLine(Utility.TabulateString("", tab_count));
                    sw.WriteLine(Utility.TabulateString("{", tab_count));
                    if (datatype == DecompileDataType.DICT)   // mixed values and variables
                    {
                        for(int i=0;i<Children.Count;i++)
                        {
                            DecompileNode n = Children[i];
                            n.WriteLuaString(sw, tab_count + 1);
                            if(i!=Children.Count-1)
                            {
                                if ((n.data == null) ^ (Children[i + 1].data == null))     // if variable data is null, its a plain value
                                    sw.WriteLine(';');
                                else
                                    sw.WriteLine(',');
                            }
                        }
                        sw.WriteLine("");
                    }
                    else   // only values
                    {
                        foreach (DecompileNode n in Children)
                        {
                            n.WriteLuaString(sw, tab_count + 1);
                            sw.WriteLine(", ");
                        }
                    }
                    sw.Write(Utility.TabulateString("}", tab_count));
                    break;
                case DecompileNodeType.VALUE:
                    string val_s = "";
                    if (data == null)
                        val_s = "nil";
                    else if (data.GetType() == typeof(string))
                    {
                        string s = data.ToString();
                        if (s.Contains('\n'))
                            val_s = "[[" + s + "]]";
                        else
                            val_s = '"' + s + '"';
                    }
                    else
                        val_s = data.ToString();
                    sw.Write(val_s);
                    break;
                case DecompileNodeType.INDEXED_VALUE:
                    ((DecompileNode)data).WriteLuaString(sw, tab_count);
                    sw.Write("[");
                    Children[0].WriteLuaString(sw, tab_count);
                    sw.Write("]");
                    break;
                case DecompileNodeType.IDENTIFIER:
                    sw.Write(data.ToString());
                    break;
                case DecompileNodeType.UPIDENTIFIER:
                    sw.Write("%" + data.ToString());
                    break;
                case DecompileNodeType.SELFIDENTIFIER:
                    Children[0].WriteLuaString(sw, tab_count);
                    sw.Write(":");
                    Children[1].WriteLuaString(sw, tab_count);
                    break;
                case DecompileNodeType.VARIABLE:
                    for (int i = 0; i < tab_count; i++)
                        sw.Write("\t");
                    if (data != null)
                    {
                        DecompileNode key = (DecompileNode)data;
                        if (key.nodetype == DecompileNodeType.VALUE)
                        {
                            if (key.datatype == DecompileDataType.STRING)
                                sw.Write(key.data.ToString());
                            else if (key.datatype == DecompileDataType.NUMBER)
                                sw.Write("[" + key.data.ToString() + "]");
                            else
                                key.WriteLuaString(sw, tab_count);
                        }
                        else
                            key.WriteLuaString(sw, tab_count);
                        sw.Write(" = ");
                    }
                    Children[0].WriteLuaString(sw, tab_count);
                    break;
                case DecompileNodeType.FORLOOP:
                    DecompileNode forloop_table = (DecompileNode)data;
                    sw.Write(Utility.TabulateString("for ", tab_count));
                    forloop_table.Children[3].WriteLuaString(sw, tab_count);
                    sw.Write(" = ");
                    forloop_table.Children[0].WriteLuaString(sw, tab_count);
                    sw.Write(", ");
                    forloop_table.Children[1].WriteLuaString(sw, tab_count);
                    sw.Write(", ");
                    forloop_table.Children[2].WriteLuaString(sw, tab_count);
                    sw.WriteLine(" do");
                    Children[0].WriteLuaString(sw, tab_count + 1);
                    sw.WriteLine(Utility.TabulateString("end", tab_count));
                    break;
                case DecompileNodeType.REPEATLOOP:
                    DecompileNode repeat_cond = (DecompileNode)data;
                    sw.WriteLine(Utility.TabulateString("repeat", tab_count));
                    Children[0].WriteLuaString(sw, tab_count + 1);
                    sw.Write("until ");
                    repeat_cond.WriteLuaString(sw, tab_count);
                    sw.WriteLine("");
                    break;
                case DecompileNodeType.FOREACHLOOP:
                    DecompileNode foreachloop_table = (DecompileNode)data;
                    sw.Write(Utility.TabulateString("for ", tab_count));
                    foreachloop_table.Children[1].WriteLuaString(sw, 0);
                    sw.Write(", ");
                    foreachloop_table.Children[2].WriteLuaString(sw, 0);
                    sw.Write(" in ");
                    foreachloop_table.Children[0].WriteLuaString(sw, 0);
                    sw.WriteLine(" do");
                    Children[0].WriteLuaString(sw, tab_count + 1);
                    sw.WriteLine(Utility.TabulateString("end", tab_count));
                    break;
                case DecompileNodeType.MULTOPERATOR:
                    sw.Write("(");
                    for(int i=0;i<Children.Count; i++)
                    {
                        Children[i].WriteLuaString(sw, tab_count);
                        if (i != Children.Count - 1)
                            sw.Write(" " + data.ToString() + " ");
                    }
                    sw.Write(")");
                    break;
                case DecompileNodeType.UNOPERATOR:
                    sw.Write("("+data.ToString());
                    Children[0].WriteLuaString(sw, tab_count);
                    sw.Write(")");
                    break;
                case DecompileNodeType.COND:
                    sw.Write(Utility.TabulateString("if ", tab_count));
                    ((DecompileNode)data).WriteLuaString(sw, tab_count);
                    sw.WriteLine(" then");
                    Children[0].WriteLuaString(sw, tab_count + 1);
                    if (Children[1].Children.Count != 0)
                    {
                        sw.WriteLine(Utility.TabulateString("else", tab_count));
                        Children[1].WriteLuaString(sw, tab_count + 1);
                    }
                    sw.Write(Utility.TabulateString("end", tab_count));
                    break;
                case DecompileNodeType.WHILE:
                    sw.Write(Utility.TabulateString("while ", tab_count));
                    ((DecompileNode)data).WriteLuaString(sw, tab_count);
                    sw.WriteLine(" do");
                    Children[0].WriteLuaString(sw, tab_count + 1);
                    sw.Write(Utility.TabulateString("end", tab_count));
                    break;
                case DecompileNodeType.CONTINUELOOP:
                    sw.Write(Utility.TabulateString("continue", tab_count));
                    break;
                case DecompileNodeType.BREAKLOOP:
                    sw.Write(Utility.TabulateString("break", tab_count));
                    break;
                default:
                    break;
            }
        }

        public override string ToString()
        {
            return nodetype.ToString() + " | " + datatype.ToString() + " | " + Children.Count.ToString() + " children";
        }
    }
}
