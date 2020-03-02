using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFLua.LuaDecompiler
{
    public class LuaStack
    {
        public List<object> stack = new List<object>();
        public int Pos { get { return stack.Count - 1; } }

        public void Push(object o)
        {
            stack.Add(o);
        }

        public object Pop()
        {
            if (Pos == Utility.NO_INDEX)
                throw new AccessViolationException("LuaStack.Pop(): Stack is empty!");
            object o = stack[Pos];
            stack.RemoveAt(Pos);
            return o;
        }

        public object Get(int reference_stack_pos)
        {
            if (reference_stack_pos >= stack.Count)
                return null;
            return stack[Pos - reference_stack_pos];
        }

        public void Set(int reference_stack_pos, object o)
        {
            stack[Pos - reference_stack_pos] = o;
        }

        public object PopAt(int reference_stack_pos)
        {
            object o = stack[Pos-reference_stack_pos];
            stack.RemoveAt(Pos-reference_stack_pos);
            return o;
        }
    }
}
