﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace SpellforceDataEditor.SFCFF
{
    //search mode
    public enum SearchType { TYPE_NUMBER = 0, TYPE_STRING = 1, TYPE_BITFIELD = 2};

    //static class with a singular purpose of searching game data for elements matching query
    public static class SFSearchModule
    {
        //searches data for elements matching the query and returns list with indices
        //inputs:
        //category to be searched
        //list of indiced to be searched
        //query to be searched
        //search mode (see above)
        //if a column is provided, only a specific column of data is looked at
        //w reference to tool strip progress bar
        public static List<int> Search(SFCategory category, List<int> source, string query, SearchType type, int column_i, System.Windows.Forms.ToolStripProgressBar pbar)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFSearchModule.Search() called, query: '"+query+"'");
            query = query.Trim();
            if (type == SearchType.TYPE_STRING)
                query = query.ToLower();
            else
                query = Regex.Replace(query, "[^0-9+-]", "");

            string format = category.GetElementFormat();
            int elem_length = format.Length;
            int query_val = Utility.TryParseInt32(query);
            List<int> target = new List<int>();

            int counter = 0;

            if(source == null)
            {
                source = new List<int>();
                for (int i = 0; i < category.GetElementCount(); i++)
                    source.Add(i);
            }

            foreach (int i in source)
            {
                counter++;
                if ((counter % 500 == 0)&&(pbar != null))
                {
                    pbar.Value = ((pbar.Maximum * counter) / source.Count);
                    pbar.GetCurrentParent().Refresh();
                }

                SFCategoryElement elem = category[i];
                if (elem == null)
                    continue;
                int elem_count = elem.variants.Count / elem_length;

                bool success = false;

                if (type == SearchType.TYPE_NUMBER)
                {
                    for (int j = 0; j < elem_count; j++)
                    {
                        for (int k = 0; k < elem_length; k++)
                        {
                            if (format[k] == 's')
                                continue;
                            if ((column_i != Utility.NO_INDEX) && (column_i != k))
                                continue;
                            int val = elem.ToInt(j * elem_length + k);
                            if (val == query_val)
                            {
                                success = true;
                                break;
                            }
                        }
                        if (success)
                            break;
                    }
                }

                else if (type == SearchType.TYPE_STRING)
                {
                    if(MainForm.data.CachedElementDisplays[category.category_id].get_element_string(i).ToLower().Contains(query))
                    {
                        target.Add(i);
                        continue;
                    }
                    for (int j = 0; j < elem_count; j++)
                    {
                        for (int k = 0; k < elem_length; k++)
                        {
                            if (format[k] == 's')
                            {
                                if ((column_i != Utility.NO_INDEX) && (column_i != k))
                                    continue;
                                string val = Utility.CleanString(elem[j * elem_length + k]);
                                if (val.ToLower().Contains(query))
                                {
                                    success = true;
                                    break;
                                }
                            }
                        }
                        if (success)
                            break;
                    }
                }

                else if(type == SearchType.TYPE_BITFIELD)
                {
                    for (int j = 0; j < elem_count; j++)
                    {
                        for (int k = 0; k < elem_length; k++)
                        {
                            if (format[k] == 's')
                                continue;
                            if ((column_i != Utility.NO_INDEX) && (column_i != k))
                                continue;
                            UInt32 val = (UInt32)elem.ToInt(j * elem_length + k);
                            if ((val & query_val) == query_val)
                            {
                                success = true;
                                break;
                            }
                        }
                        if (success)
                            break;
                    }
                }

                if (success)
                    target.Add(i);
            }

            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFSearchModule.Search() concluded, found elements: " + target.Count.ToString());
            return target;
        }
    }
}
