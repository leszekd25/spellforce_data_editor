using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    //struct saves currently viewed element
    public struct SFDataTraceElement
    {
        public int category_index;
        public int category_element;

        public SFDataTraceElement(int cat_i, int cat_e)
        {
            category_index = cat_i;
            category_element = cat_e;
        }
    }

    //list of previously viewed elements
    //user can go back to a previous element whenever he wants
    public class SFDataTracer
    {
        private Stack<SFDataTraceElement> stack;

        public SFDataTracer()
        {
            stack = new Stack<SFDataTraceElement>();
        }

        public void AddTrace(int cat_i, int cat_e)
        {
            stack.Push(new SFDataTraceElement(cat_i, cat_e));
        }

        public bool CanGoBack()
        {
            return (stack.Count != 0);
        }

        public SFDataTraceElement GoBack()
        {
            return stack.Pop();
        }

        public void Clear()
        {
            stack.Clear();
        }
    }
}
