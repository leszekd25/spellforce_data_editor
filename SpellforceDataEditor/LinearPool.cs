using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    // a more lenient variant of ArrayPool
    public class LinearPool<T>
    {
        public int first_unused = 0;
        public int last_used = Utility.NO_INDEX;
        public int used_count = 0;
        public List<T> elements { get; private set; } = new List<T>();
        public List<bool> elem_active { get; set; } = new List<bool>();

        public int Add(T elem)
        {
            int elem_index = Utility.NO_INDEX;
            if (first_unused == elements.Count)
            {
                elements.Add(elem);
                elem_active.Add(true);
                first_unused += 1;
                last_used = elements.Count - 1;
                elem_index = last_used;
            }
            else
            {
                elements[first_unused] = elem;
                elem_active[first_unused] = true;
                elem_index = first_unused;

                bool found = false;
                for (int i = first_unused + 1; i < elements.Count; i++)
                    if (!elem_active[i])
                    {
                        first_unused = i;
                        found = true;
                        break;
                    }
                if (!found)
                    first_unused = elements.Count;
            }
            used_count += 1;
            return elem_index;
        }

        public void RemoveAt(int index)
        {
            if (elem_active[index])
            {
                elem_active[index] = false;
                used_count -= 1;
                if (index < first_unused)
                    first_unused = index;
                if (index == last_used)
                    for (int i = last_used - 1; i >= 0; i--)
                        if (elem_active[index])
                        {
                            last_used = i;
                            break;
                        }
                if (used_count == 0)
                    last_used = -1;
            }
        }

        public void Remove(T elem)
        {
            int index = elements.IndexOf(elem);
            if (index != Utility.NO_INDEX)
                RemoveAt(index);
        }

        public void Clear()
        {
            elements.Clear();
            elem_active.Clear();
            last_used = -1;
            first_unused = 0;
            used_count = 0;
        }
    }
}
