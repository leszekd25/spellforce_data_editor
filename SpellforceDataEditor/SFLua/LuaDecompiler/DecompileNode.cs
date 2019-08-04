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
        CHUNK, COND, IDENTIFIER, FUNCDEF, FOREACHLOOP,
        UNOPERATOR, MULTOPERATOR, VALUE, INDEXED_VALUE, FUNCTION,
        TABLE, VARIABLE, FORLOOP, RETURN
    }
    public enum DecompileDataType { NIL, NUMBER, STRING, LIST, DICT, FUNCTION, USERDATA }
    public class DecompileNode
    {
        public DecompileNodeType nodetype;
        public DecompileDataType datatype;
        public object data;
        public DecompileNode Parent = null;
        public List<DecompileNode> Children = new List<DecompileNode>();

        public DecompileNode(DecompileNodeType nt, DecompileDataType dt = DecompileDataType.NIL, object d = null)
        {
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

        public void WriteLuaString(StringWriter sw, int tab_count = 0)
        {
            //System.Diagnostics.Debug.WriteLine(ToString()+" TABC "+tab_count.ToString());
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
                    sw.Write("return ");
                    for(int i=0;i<Children.Count; i++)
                    {
                        DecompileNode n = Children[i];
                        n.WriteLuaString(sw, tab_count);
                        if(i!=Children.Count-1)
                            sw.Write(", ");
                    }
                    break;
                case DecompileNodeType.FUNCDEF:
                    sw.Write(Utility.TabulateString("function " + data.ToString() + "(",  tab_count));
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
                    if ((Parent.nodetype == DecompileNodeType.CHUNK)||(Parent.nodetype == DecompileNodeType.TABLE))
                        for (int i = 0; i < tab_count; i++)
                            sw.Write("\t");

                    ((DecompileNode)data).WriteLuaString(sw, tab_count);
                    if(Children.Count==0)
                    {
                        sw.Write("()");
                        break;
                    }

                    if(Children[0].nodetype == DecompileNodeType.TABLE)
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
                    if (datatype == DecompileDataType.DICT)
                    {
                        foreach (DecompileNode n in Children)
                        {
                            if (n.datatype == DecompileDataType.NUMBER)
                                sw.Write(Utility.TabulateString("[" + n.data.ToString() + "] = ", tab_count + 1));
                            else
                                sw.Write(Utility.TabulateString(n.data.ToString() + " = ", tab_count + 1));
                            n.Children[0].WriteLuaString(sw, tab_count + 1);
                            sw.WriteLine(", ");
                        }
                    }
                    else
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
                    if (data.GetType() == typeof(string))
                        val_s = '"' + data.ToString() + '"';
                    else
                        val_s = data.ToString();
                    sw.Write(val_s);
                    break;
                case DecompileNodeType.INDEXED_VALUE:
                    sw.Write(data.ToString()+"[");
                    Children[0].WriteLuaString(sw, tab_count);
                    sw.Write("]");
                    break;
                case DecompileNodeType.IDENTIFIER:
                    sw.Write(data.ToString());
                    break;
                case DecompileNodeType.VARIABLE:
                    if (datatype == DecompileDataType.STRING)
                    {
                        sw.Write(Utility.TabulateString(data.ToString() + " = ", tab_count));
                        Children[0].WriteLuaString(sw, tab_count);
                    }
                    else if(datatype == DecompileDataType.DICT)
                    {
                        sw.Write(Utility.TabulateString(data.ToString() + "[", tab_count));
                        Children[0].WriteLuaString(sw, tab_count);
                        sw.Write("] = ");
                        Children[1].WriteLuaString(sw, tab_count);
                    }
                    break;
                case DecompileNodeType.FORLOOP:
                    DecompileNode forloop_table = (DecompileNode)data;
                    sw.Write(Utility.TabulateString("for ", tab_count));
                    sw.Write(forloop_table.Children[3].data.ToString());
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
                case DecompileNodeType.FOREACHLOOP:
                    DecompileNode foreachloop_table = (DecompileNode)data;
                    sw.Write(Utility.TabulateString("for ", tab_count));
                    foreachloop_table.Children[0].WriteLuaString(sw, tab_count);
                    sw.Write(", ");
                    foreachloop_table.Children[1].WriteLuaString(sw, tab_count);
                    sw.WriteLine(" in " + foreachloop_table.data.ToString() + " do");
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
                    if(Children[1].Children.Count!=0)
                    {
                        sw.WriteLine(Utility.TabulateString("else", tab_count));
                        Children[1].WriteLuaString(sw, tab_count + 1);
                    }
                    sw.Write(Utility.TabulateString("end", tab_count));
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
