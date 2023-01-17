﻿/*
 * SFDataTraceElement is a basic block of a tracing stack
 * SFDataTracer uses these blocks to communicate with the SpelllforceCFFEditor, allowing back and forth gamedata element change
 */

using System.Collections.Generic;

namespace SpellforceDataEditor.SFCFF
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
