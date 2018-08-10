using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    public enum SearchType { TYPE_NUMBER = 0, TYPE_STRING = 1, TYPE_BITFIELD = 2};

    public static class SFSearchModule
    {
        public static List<int> Search(SFCategory category, List<int> source, string query, SearchType type, int column_i)
        {
            string format = category.get_element_format();
            int elem_length = format.Length;
            List<int> target = new List<int>();

            foreach (int i in source)
            {
                SFCategoryElement elem = category.get_element(i);
                if (elem == null)
                    continue;

                int elem_count = elem.get().Count / elem_length;

                if (type == SearchType.TYPE_NUMBER)
                {
                    int query_val = Utility.TryParseInt32(query, int.MinValue);
                    bool success = false;
                    for (int j = 0; j < elem_count; j++)
                    {
                        for (int k = 0; k < elem_length; k++)
                        {
                            if (format[k] == 's')
                                continue;
                            if ((column_i != -1) && (column_i != k))
                                continue;
                            int val = elem.get_single_variant(j * elem_length + k).to_int();
                            if (val == query_val)
                            {
                                success = true;
                                break;
                            }
                        }
                        if (success)
                        {
                            target.Add(i);
                            //System.Diagnostics.Debug.WriteLine(i);
                            break;
                        }
                    }
                }

                else if (type == SearchType.TYPE_STRING)
                {
                    if(category.get_element_string(i).Contains(query))
                    {
                        target.Add(i);
                        continue;
                    }
                    bool success = false;
                    for (int j = 0; j < elem_count; j++)
                    {
                        for (int k = 0; k < elem_length; k++)
                        {
                            if (format[k] == 's')
                            {
                                if ((column_i != -1) && (column_i != k))
                                    continue;
                                string val = Utility.CleanString(elem.get_single_variant(j * elem_length + k));//(int)elem.get_single_variant(j * elem_length + k).value;
                                if (val == query)
                                {
                                    success = true;
                                    break;
                                }
                            }
                        }
                        if (success)
                        {
                            target.Add(i);
                            break;
                        }
                    }
                }

                else if(type == SearchType.TYPE_BITFIELD)
                {
                    UInt32 query_val = Utility.TryParseUInt32(query, UInt32.MaxValue);
                    bool success = false;
                    for (int j = 0; j < elem_count; j++)
                    {
                        for (int k = 0; k < elem_length; k++)
                        {
                            if (format[k] == 's')
                                continue;
                            if ((column_i != -1) && (column_i != k))
                                continue;
                            UInt32 val = (UInt32)elem.get_single_variant(j * elem_length + k).to_int();
                            if ((val & query_val) == query_val)
                            {
                                success = true;
                                break;
                            }
                        }
                        if (success)
                        {
                            target.Add(i);
                            break;
                        }
                    }
                }
            }
            return target;
        }
    }
}
