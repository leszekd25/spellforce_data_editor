/*
 * there is a nasty bug in here, hard to catch
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFCFF
{
    //struct which holds data needed for diff tool class to operate properly
    public struct SFDiffElement
    {
        //unknown type: nobody knows
        //replace: element was changed, replace_sub: subelement was changed
        //insert: a new element was added, insert_sub: a new subelement was added
        //remove: an element was removed, remove_sub: a subelement was removed
        //lastindex: denotes the last change index that is in effect in data
        public enum DIFF_TYPE { UNKNOWN = 0, REPLACE, REPLACE_SUB, INSERT, INSERT_SUB, REMOVE, REMOVE_SUB };

        public DIFF_TYPE difference_type;         //action that is taken
        public int difference_index;              //index in a category that the action refers to (or other data when applicable)
        public int subdifference_index;           //index in element list, only applicable to _SUB types
        public object elem_old;        //element before change, can be either SFCategoryElement or SFCategoryElementList
        public object elem_new;        //element after change,  can be either SFCategoryElement or SFCategoryElementList

        public SFDiffElement(DIFF_TYPE d, int i = Utility.NO_INDEX, int j = Utility.NO_INDEX, SFCategoryElement e1 = null, SFCategoryElement e2 = null)
        {
            difference_type = d;
            difference_index = i;
            subdifference_index = j;
            elem_old = e1;
            elem_new = e2;
        }

        public SFDiffElement(DIFF_TYPE d, int i = Utility.NO_INDEX, int j = Utility.NO_INDEX, SFCategoryElementList e1 = null, SFCategoryElementList e2 = null)
        {
            difference_type = d;
            difference_index = i;
            subdifference_index = j;
            elem_old = e1;
            elem_new = e2;
        }
    }

    //this class is the backbone of editor's diff tool (and undo/redo?)
    //it contains functions to deal with saving data manipulation order: replacement, insertion and removal of elements
    public class SFDiffTools
    {
        protected Dictionary<int, List<SFDiffElement>> diff_data;   //list of differences between original and modified in each category
        protected Dictionary<int, int> diff_current_index;          //list of the highest index in diff list that is in effect in data; -1 is minimum value, diff_data[].Count-1 is max value 

        //connects diff tool to a SFCategoryManager it operates on
        public void init()
        {
            diff_data = new Dictionary<int, List<SFDiffElement>>();
            diff_current_index = new Dictionary<int, int>();
            foreach(var cat in SFCategoryManager.gamedata.categories)
            {
                diff_data.Add(cat.Key, new List<SFDiffElement>());
                diff_current_index.Add(cat.Key, Utility.NO_INDEX);
            }
        }

        //clear all diff data (usually called in tandem with SFCategoryManager.unload_all()
        public void clear_data()
        {
            diff_data.Clear();
            diff_current_index.Clear();
        }


        //this function modifies the category data depending on what needs to be done
        public void commit_change(SFCategory cat, SFDiffElement elem)
        {
            if (cat.category_allow_multiple)
            {
                switch (elem.difference_type)
                {
                    case SFDiffElement.DIFF_TYPE.REPLACE:
                        cat.element_lists[elem.difference_index] = (SFCategoryElementList)(elem.elem_new);
                        break;
                    case SFDiffElement.DIFF_TYPE.INSERT:
                        cat.element_lists.Insert(elem.difference_index + 1, (SFCategoryElementList)(elem.elem_new));
                        break;
                    case SFDiffElement.DIFF_TYPE.REMOVE:
                        cat.element_lists.RemoveAt(elem.difference_index);
                        break;
                    case SFDiffElement.DIFF_TYPE.REPLACE_SUB:
                        cat[elem.difference_index, elem.subdifference_index] = (SFCategoryElement)(elem.elem_new);
                        break;
                    case SFDiffElement.DIFF_TYPE.INSERT_SUB:
                        cat.element_lists[elem.difference_index].Elements.Insert(elem.subdifference_index + 1, (SFCategoryElement)(elem.elem_new));
                        break;
                    case SFDiffElement.DIFF_TYPE.REMOVE_SUB:
                        cat.element_lists[elem.difference_index].Elements.RemoveAt(elem.subdifference_index);
                        break;
                }
            }
            else
            {
                switch (elem.difference_type)
                {
                    case SFDiffElement.DIFF_TYPE.REPLACE:
                        cat[elem.difference_index] = (SFCategoryElement)(elem.elem_new);
                        break;
                    case SFDiffElement.DIFF_TYPE.INSERT:
                        cat.elements.Insert(elem.difference_index + 1, (SFCategoryElement)(elem.elem_new));
                        break;
                    case SFDiffElement.DIFF_TYPE.REMOVE:
                        cat.elements.RemoveAt(elem.difference_index);
                        break;
                    case SFDiffElement.DIFF_TYPE.UNKNOWN:
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false, "SFDiffTools.commit_change(): Invalid category type!");
                        break;
                }
            }
        }

        //reverted commit_change
        public void revert_change(SFCategory cat, SFDiffElement elem)
        {
            if (cat.category_allow_multiple)
            {
                switch (elem.difference_type)
                {
                    case SFDiffElement.DIFF_TYPE.REPLACE:
                        cat.element_lists[elem.difference_index] = (SFCategoryElementList)(elem.elem_old);
                        break;
                    case SFDiffElement.DIFF_TYPE.REMOVE:
                        cat.element_lists.Insert(elem.difference_index, (SFCategoryElementList)(elem.elem_old));
                        break;
                    case SFDiffElement.DIFF_TYPE.INSERT:
                        cat.element_lists.RemoveAt(elem.difference_index + 1);
                        break;
                    case SFDiffElement.DIFF_TYPE.REPLACE_SUB:
                        cat[elem.difference_index, elem.subdifference_index] = (SFCategoryElement)(elem.elem_old);
                        break;
                    case SFDiffElement.DIFF_TYPE.REMOVE_SUB:
                        cat.element_lists[elem.difference_index].Elements.Insert(elem.subdifference_index, (SFCategoryElement)(elem.elem_old));
                        break;
                    case SFDiffElement.DIFF_TYPE.INSERT_SUB:
                        cat.element_lists[elem.difference_index].Elements.RemoveAt(elem.subdifference_index + 1);
                        break;
                }
            }
            else
            {
                switch (elem.difference_type)
                {
                    case SFDiffElement.DIFF_TYPE.REPLACE:
                        cat[elem.difference_index] = (SFCategoryElement)(elem.elem_old);
                        break;
                    case SFDiffElement.DIFF_TYPE.REMOVE:
                        cat.elements.Insert(elem.difference_index, (SFCategoryElement)(elem.elem_old));
                        break;
                    case SFDiffElement.DIFF_TYPE.INSERT:
                        cat.elements.RemoveAt(elem.difference_index + 1);
                        break;
                    case SFDiffElement.DIFF_TYPE.UNKNOWN:
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false, "SFDiffTools.revert_change(): Invalid category type!");
                        break;

                }
            }
        }

        //adds diff piece to diff data
        //doing this in chronological order guarantees (?) data integrity when modifying data based on diff data
        //if there are changes in category which have index higher than diff_current_index, they're discarded, then a new change is made
        public void push_change(int cat_index, SFDiffElement elem)
        {
            if (elem.difference_type == SFDiffElement.DIFF_TYPE.UNKNOWN)
                return;

            int last_change = diff_data[cat_index].Count-1;
            int change_difference = last_change - diff_current_index[cat_index];
            if(change_difference > 0)
            {
                for (int i = 0; i < change_difference; i++)
                {
                    System.Diagnostics.Debug.WriteLine((last_change - i).ToString() + " CHANGE REMOVED: " + diff_data[cat_index][last_change - i].difference_type.ToString() + " [" + diff_data[cat_index][last_change - i].difference_index.ToString() + "]");
                    diff_data[cat_index].RemoveAt(last_change - i);
                }
            }
            System.Diagnostics.Debug.WriteLine((last_change-change_difference+1).ToString() + " CHANGE ADDED: " + elem.difference_type.ToString() + " [" + elem.difference_index.ToString() + "]");
            diff_data[cat_index].Add(elem);
            diff_current_index[cat_index]++;
        }

        //undos a change previously commited
        //undone changes can be lost when another change is pushed
        public void undo_change(int cat_index)
        {
            int current_change = diff_current_index[cat_index];
            if (current_change < 0)
                return;

            SFCategory cat = SFCategoryManager.gamedata[cat_index];
            revert_change(cat, diff_data[cat_index][current_change]);
            diff_current_index[cat_index]--;
        }

        //redos a change previously undone
        //the only way to restore undone changes
        public void redo_change(int cat_index)
        {
            int current_change = diff_current_index[cat_index];
            if (current_change >= diff_data[cat_index].Count-1)
                return;

            SFCategory cat = SFCategoryManager.gamedata[cat_index];
            commit_change(cat, diff_data[cat_index][current_change+1]);
            diff_current_index[cat_index]++;
        }

        //returns whether there are changes that can be undone
        public bool can_undo_changes(int cat_index)
        {
            if (cat_index == Utility.NO_INDEX)
                return false;
            return (diff_current_index[cat_index] >= 0);
        }

        //returns whether there are changes that can be redone
        public bool can_redo_changes(int cat_index)
        {
            if (cat_index == Utility.NO_INDEX)
                return false;
            return (diff_current_index[cat_index] < diff_data[cat_index].Count-1);
        }

        //returns the action that will be taken (in reverse) when undo action is made
        public SFDiffElement get_next_undo_change(int cat_index)
        {
            return diff_data[cat_index][diff_current_index[cat_index]];
        }

        //returns the action that will be taken when redo action is made
        public SFDiffElement get_next_redo_change(int cat_index)
        {
            return diff_data[cat_index][diff_current_index[cat_index]+1];
        }
    }
}
