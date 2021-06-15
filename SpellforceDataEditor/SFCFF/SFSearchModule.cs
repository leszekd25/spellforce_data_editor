using System;
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
        private static SearchType _type;
        private static string _format;
        private static int _column_index;
        private static string _query;
        private static int _query_val;

        private static bool ElementMatch(SFCategoryElement elem)
        {
            bool success = false;

            if (_type == SearchType.TYPE_NUMBER)
            {
                for (int k = 0; k < _format.Length; k++)
                {
                    if (_format[k] == 's')
                        continue;
                    if ((_column_index != Utility.NO_INDEX) && (_column_index != k))
                        continue;
                    int val = elem.ToInt(k);
                    if (val == _query_val)
                    {
                        success = true;
                        break;
                    }
                }
            }

            else if (_type == SearchType.TYPE_STRING)
            {
                for (int k = 0; k < _format.Length; k++)
                {
                    if (_format[k] == 's')
                    {
                        if ((_column_index != Utility.NO_INDEX) && (_column_index != k))
                            continue;
                        string val = Utility.CleanString(elem[k]);
                        if (val.ToLower().Contains(_query))
                        {
                            success = true;
                            break;
                        }
                    }
                }
            }

            else if (_type == SearchType.TYPE_BITFIELD)
            {
                for (int k = 0; k < _format.Length; k++)
                {
                    if (_format[k] == 's')
                        continue;
                    if ((_column_index != Utility.NO_INDEX) && (_column_index != k))
                        continue;
                    UInt32 val = (UInt32)elem.ToInt(k);
                    if ((val & _query_val) == _query_val)
                    {
                        success = true;
                        break;
                    }
                }
            }

            return success;
        }


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
            int query_val = Utility.TryParseInt32(query);
            List<int> target = new List<int>();

            int counter = 0;

            if(source == null)
            {
                source = new List<int>();
                for (int i = 0; i < category.GetElementCount(); i++)
                    source.Add(i);
            }

            _query = query;
            _format = category.elem_format;
            _type = type;
            _column_index = column_i;
            _query_val = query_val;

            foreach (int i in source)
            {
                counter++;
                if ((counter % 500 == 0)&&(pbar != null))
                {
                    pbar.Value = ((pbar.Maximum * counter) / source.Count);
                    pbar.GetCurrentParent().Refresh();
                }

                if ((i < 0) || (i >= category.GetElementCount()))
                    continue;

                bool success = false;

                // first look through names in the list
                if(type == SearchType.TYPE_STRING)
                {
                    if (MainForm.data.CachedElementDisplays[category.category_id].get_element_string(i).ToLower().Contains(query))
                    {
                        target.Add(i);
                        continue;
                    }
                }
                // if not found, look through elements
                if(category.category_allow_multiple)
                {
                    for(int j = 0; j < category.element_lists[i].Elements.Count; j++)
                    {
                        success = ElementMatch(category[i, j]);
                        if (success)
                            break;
                    }
                }
                else
                {
                    success = ElementMatch(category[i]);
                }

                if (success)
                    target.Add(i);
            }

            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFSearchModule.Search() concluded, found elements: " + target.Count.ToString());
            return target;
        }
    }
}
