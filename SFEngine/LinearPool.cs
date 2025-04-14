using System.Collections.Generic;

namespace SFEngine
{
    // a more lenient variant of ArrayPool
    public class LinearPool<T>
    {
        public int first_unused = 0;
        public int last_used = Utility.NO_INDEX;
        public int used_count = 0;
        public List<T> elements { get; private set; } = new List<T>();
        public List<bool> elem_active { get; private set; } = new List<bool>();

        public int Add(T elem)
        {
            int elem_index;
            if (first_unused == used_count)
            {
                if(first_unused == elements.Count)
                {
                    elements.Add(elem);
                    elem_active.Add(true);
                }
                else
                {
                    elements[first_unused] = elem;
                    elem_active[first_unused] = true;
                }
                elem_index = first_unused;
                last_used = first_unused;
                first_unused += 1;
            }
            else
            {
                elements[first_unused] = elem;
                elem_active[first_unused] = true;
                elem_index = first_unused;

                bool found = false;
                for (int i = first_unused + 1; i <= last_used; i++)
                {
                    if (!elem_active[i])
                    {
                        first_unused = i;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    first_unused = used_count + 1;
                }
            }
            used_count += 1;
            return elem_index;
        }

        public void RemoveAt(int index)
        {
            // only remove if index is used
            if (elem_active[index])
            {
                elem_active[index] = false;
                used_count -= 1;

                // first unused is this index, if this index is smaller than any other unused index
                if (index < first_unused)
                {
                    first_unused = index;
                }

                // if this index is last used, find the largest used index smaller than this
                if (index == last_used)
                {
                    for (int i = last_used - 1; i >= 0; i--)
                    {
                        if (elem_active[i])
                        {
                            last_used = i;
                            break;
                        }
                    }
                }

                // if there are no used indices, set last used to -1
                if (used_count == 0)
                {
                    last_used = -1;
                }
            }
        }

        public IEnumerable<T> GetItems()
        {
            for(int i = 0; i <= last_used; i++)
            {
                if(elem_active[i])
                {
                    yield return elements[i];
                }
            }
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
