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
        private Stack<SFDataTraceElement> stack;              //stack which holds which elements from which categories were accessed (in chronological order)

        public SFDataTracer()
        {
            stack = new Stack<SFDataTraceElement>();
        }

        //pushes new value on the stack
        public void AddTrace(int cat_i, int cat_e)
        {
            stack.Push(new SFDataTraceElement(cat_i, cat_e));
        }

        //returns whether there are any elements preceding currently viewed one which can be returned to
        public bool CanGoBack()
        {
            return (stack.Count != 0);
        }

        //returns to the previously viewed element
        public SFDataTraceElement GoBack()
        {
            return stack.Pop();
        }

        //clears whole stack, effectively removing viewing history
        public void Clear()
        {
            stack.Clear();
        }
    }
}
