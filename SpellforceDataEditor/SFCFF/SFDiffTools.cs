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
        //replace: element was changed
        //insert: a new element was added
        //remove: an element was removed
        //category: denotes start of category changes
        //eof: denotes end of file
        //md5: denotes cff file nd5 hash (needed for file comparison)
        //lastindex: denotes the last change index that is in effect in data
        public enum DIFF_TYPE { UNKNOWN = 0, REPLACE, INSERT, REMOVE, CATEGORY, EOF, MD5, LASTINDEX };

        public DIFF_TYPE difference_type;         //action that is taken
        public int difference_index;              //index in a category that the action refers to (or other data when applicable)
        public SFCategoryElement elem_old;        //element before change
        public SFCategoryElement elem_new;        //element after change
        public SFDiffElement(DIFF_TYPE d, int i = -1, SFCategoryElement e1 = null, SFCategoryElement e2 = null)
        {
            difference_type = d;
            difference_index = i;
            elem_old = e1;
            elem_new = e2;
        }
    }

    //this class is the backbone of editor's diff tool (and undo/redo?)
    //it contains functions to deal with saving data manipulation order: replacement, insertion and removal of elements
    public class SFDiffTools
    {
        protected List<SFDiffElement>[] diff_data;    //list of differences between original and modified in each category
        protected int[] diff_current_index;           //list of the highest index in diff list that is in effect in data; -1 is minimum value, diff_data[].Count-1 is max value
        string presumed_md5 = "";

        //connects diff tool to a SFCategoryManager it operates on
        public void init()
        {
            diff_data = new List<SFDiffElement>[SFGameData.categoryNumber];
            diff_current_index = new int[SFGameData.categoryNumber];
            for (int i = 0; i < SFGameData.categoryNumber; i++)
            {
                diff_current_index[i] = -1;
                diff_data[i] = new List<SFDiffElement>();
            }
        }

        //clear all diff datal usually called in tandem with SFCategoryManager.unload_all()
        public void clear_data()
        {
            for (int i = 0; i < SFGameData.categoryNumber; i++)
            {
                diff_data[i].Clear();
                diff_current_index[i] = -1;
            }
        }

        //puts a single diff piece into the buffer
        public void write_diff_element(BinaryWriter bw, SFCategory cat, SFDiffElement elem)
        {
            bw.Write((Byte)elem.difference_type);
            switch (elem.difference_type)
            {
                case SFDiffElement.DIFF_TYPE.MD5:
                    bw.Write(SFCategoryManager.gamedata.gamedata_md5.ToCharArray());
                    break;
                case SFDiffElement.DIFF_TYPE.REPLACE:
                    bw.Write(elem.difference_index);
                    cat.WriteElementToBuffer(bw, elem.elem_old.variants);
                    cat.WriteElementToBuffer(bw, elem.elem_new.variants);
                    break;
                case SFDiffElement.DIFF_TYPE.INSERT:
                    bw.Write(elem.difference_index);
                    cat.WriteElementToBuffer(bw, elem.elem_new.variants);
                    break;
                case SFDiffElement.DIFF_TYPE.REMOVE:
                    bw.Write(elem.difference_index);
                    cat.WriteElementToBuffer(bw, elem.elem_old.variants);
                    break;
                case SFDiffElement.DIFF_TYPE.CATEGORY:
                case SFDiffElement.DIFF_TYPE.LASTINDEX:
                    bw.Write(elem.difference_index);
                    break;
                default:
                    LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, "SFDiffTols.write_diff_element(): Unknown diff type (type: "+elem.difference_index.ToString()+")");
                    break;
            }
            return;
        }

        //reads a single diff piece from the buffer
        public SFDiffElement read_diff_element(BinaryReader br, SFCategory cat)
        {
            SFDiffElement elem = new SFDiffElement(SFDiffElement.DIFF_TYPE.EOF);
            elem.difference_type = (SFDiffElement.DIFF_TYPE)br.ReadByte();
            switch(elem.difference_type)
            {
                case SFDiffElement.DIFF_TYPE.MD5:
                    presumed_md5 = new string(br.ReadChars(32));
                    break;
                case SFDiffElement.DIFF_TYPE.REPLACE:
                    elem.difference_index = br.ReadInt32();
                    elem.elem_old = new SFCategoryElement();
                    elem.elem_new = new SFCategoryElement();
                    elem.elem_old.AddVariants(cat.GetElementFromBuffer(br));
                    elem.elem_new.AddVariants(cat.GetElementFromBuffer(br));
                    break;
                case SFDiffElement.DIFF_TYPE.INSERT:
                    elem.difference_index = br.ReadInt32();
                    elem.elem_new = new SFCategoryElement();
                    elem.elem_new.AddVariants(cat.GetElementFromBuffer(br));
                    break;
                case SFDiffElement.DIFF_TYPE.REMOVE:
                    elem.difference_index = br.ReadInt32();
                    elem.elem_old = new SFCategoryElement();
                    elem.elem_old.AddVariants(cat.GetElementFromBuffer(br));
                    break;
                case SFDiffElement.DIFF_TYPE.CATEGORY:
                case SFDiffElement.DIFF_TYPE.LASTINDEX:
                    elem.difference_index = br.ReadInt32();
                    break;
                default:
                    LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, "SFDiffTols.read_diff_element(): Unknown diff type (type: " + elem.difference_index.ToString() + ")");
                    break;
            }
            return elem;
        }

        //saves diff data in chronological order, and category order
        public void save_diff_data(string fname)
        {
            FileStream fs = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Write);
            fs.SetLength(0);
            BinaryWriter bw = new BinaryWriter(fs, Encoding.Default);

            bw.Write((Byte)SFDiffElement.DIFF_TYPE.MD5);
            bw.Write(SFCategoryManager.gamedata.gamedata_md5.ToCharArray());

            for (int i = 0; i < SFGameData.categoryNumber; i++)
            {
                SFCategory cat = SFCategoryManager.gamedata[i];
                write_diff_element(bw, null, new SFDiffElement(SFDiffElement.DIFF_TYPE.CATEGORY, i));
                for (int j = 0; j < diff_data[i].Count; j++)
                {
                    write_diff_element(bw, cat, diff_data[i][j]);
                }
                write_diff_element(bw, null, new SFDiffElement(SFDiffElement.DIFF_TYPE.LASTINDEX, diff_current_index[i]));
            }

            write_diff_element(bw, null, new SFDiffElement(SFDiffElement.DIFF_TYPE.EOF));

            //bw.Close();
            fs.Close();
        }

        //loads data from a diff file
        //returns false if MD5 hashes of originally edited CFF and currently loaded CFF mismatch
        public bool load_diff_data(string fname)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(fname, FileMode.Open, FileAccess.Read);
            }
            catch(FileNotFoundException)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFDiffTols.load_diff_data(): Can't open file "+fname);
                return false;
            }
            catch(Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFDiffTols.load_diff_data(): Unknown error");
                return false;
            }

            BinaryReader br = new BinaryReader(fs, Encoding.Default);

            int current_category = -1;
            SFDiffElement elem;
            bool success = true;

            while (br.PeekChar() != -1)
            {
                elem = read_diff_element(br, SFCategoryManager.gamedata[current_category]);
                if (elem.difference_type == SFDiffElement.DIFF_TYPE.CATEGORY)
                    current_category = elem.difference_index;
                else if (elem.difference_type == SFDiffElement.DIFF_TYPE.LASTINDEX)
                    diff_current_index[current_category] = elem.difference_index;
                else if (elem.difference_type == SFDiffElement.DIFF_TYPE.EOF)
                    break;
                else if (elem.difference_type == SFDiffElement.DIFF_TYPE.MD5)   //should be right at the beginning of the file
                {
                    if (presumed_md5 != SFCategoryManager.gamedata.gamedata_md5)
                    {
                        clear_data();
                        LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFDiffTols.load_diff_data(): MD5 hashes don't match!");
                        success = false;
                        break;
                    }
                }
                else
                    push_change(current_category, elem);
            }

            //br.Close();
            fs.Close();
            return success;
        }

        //this function modifies the category data depending on what needs to be done
        public void commit_change(SFCategory cat, SFDiffElement elem)
        {
            List<SFCategoryElement> elem_list = cat.elements;
            switch (elem.difference_type)
            {
                case SFDiffElement.DIFF_TYPE.REPLACE:
                    elem_list[elem.difference_index] = elem.elem_new;
                    break;
                case SFDiffElement.DIFF_TYPE.INSERT:
                    elem_list.Insert(elem.difference_index+1, elem.elem_new);
                    break;
                case SFDiffElement.DIFF_TYPE.REMOVE:
                    elem_list.RemoveAt(elem.difference_index);
                    break;
            }
        }

        //reverted commit_change
        public void revert_change(SFCategory cat, SFDiffElement elem)
        {
            List<SFCategoryElement> elem_list = cat.elements;
            switch (elem.difference_type)
            {
                case SFDiffElement.DIFF_TYPE.REPLACE:
                    elem_list[elem.difference_index] = elem.elem_old;
                    break;
                case SFDiffElement.DIFF_TYPE.REMOVE:
                    elem_list.Insert(elem.difference_index, elem.elem_old);
                    break;
                case SFDiffElement.DIFF_TYPE.INSERT:
                    elem_list.RemoveAt(elem.difference_index+1);
                    break;
            }
        }

        //modifies data from all categories at once according to diff data
        //BROKEN! do not use for the time being
        public void merge_changes()
        {
            for(int i = 0; i < SFGameData.categoryNumber; i++)
            {
                SFCategory cat = SFCategoryManager.gamedata[i];
                for(int j = 0; j <= diff_current_index[i]; j++)
                {
                    SFDiffElement elem = diff_data[i][j];
                    commit_change(cat, elem);
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
            if (cat_index == -1)
                return false;
            return (diff_current_index[cat_index] >= 0);
        }

        //returns whether there are changes that can be redone
        public bool can_redo_changes(int cat_index)
        {
            if (cat_index == -1)
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
