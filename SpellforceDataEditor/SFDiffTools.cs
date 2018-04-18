﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    public struct SFDiffElement
    {
        public enum DIFF_TYPE { UNKNOWN = 0, REPLACE, INSERT, REMOVE, CATEGORY, EOF, MD5 };

        public DIFF_TYPE difference_type;
        public int difference_index;
        public SFCategoryElement elem;
        public SFDiffElement(DIFF_TYPE d, int i = -1, SFCategoryElement e = null)
        {
            difference_type = d;
            difference_index = i;
            elem = e;
        }
    }

    public class SFDiffTools
    {
        protected SFCategoryManager manager;
        protected List<SFDiffElement>[] diff_data;
        string presumed_md5 = "";


        public void connect_to(SFCategoryManager man)
        {
            manager = man;
            diff_data = new List<SFDiffElement>[man.get_category_number()];
            for (int i = 0; i < man.get_category_number(); i++)
                diff_data[i] = new List<SFDiffElement>();
        }

        public void clear_data()
        {
            for (int i = 0; i < manager.get_category_number(); i++)
                diff_data[i].Clear();
        }

        public void write_diff_element(BinaryWriter bw, SFCategory cat, SFDiffElement elem)
        {
            bw.Write((Byte)elem.difference_type);
            switch (elem.difference_type)
            {
                case SFDiffElement.DIFF_TYPE.MD5:
                    bw.Write(manager.get_data_md5().ToCharArray());
                    break;
                case SFDiffElement.DIFF_TYPE.REPLACE:
                case SFDiffElement.DIFF_TYPE.INSERT:
                    bw.Write(elem.difference_index);
                    cat.put_element(bw, elem.elem.get());
                    break;
                case SFDiffElement.DIFF_TYPE.REMOVE:
                case SFDiffElement.DIFF_TYPE.CATEGORY:
                    bw.Write(elem.difference_index);
                    break;
            }
            return;
        }

        public SFDiffElement read_diff_element(BinaryReader br, SFCategory cat)
        {
            SFDiffElement elem = new SFDiffElement(SFDiffElement.DIFF_TYPE.EOF);
            elem.difference_type = (SFDiffElement.DIFF_TYPE)br.ReadByte();
            switch(elem.difference_type)
            {
                case SFDiffElement.DIFF_TYPE.MD5:
                    presumed_md5 = Utility.CleanString(br.ReadChars(32));
                    break;
                case SFDiffElement.DIFF_TYPE.REPLACE:
                case SFDiffElement.DIFF_TYPE.INSERT:
                    elem.difference_index = br.ReadInt32();
                    elem.elem = new SFCategoryElement();
                    elem.elem.set(cat.get_element(br));
                    break;
                case SFDiffElement.DIFF_TYPE.REMOVE:
                case SFDiffElement.DIFF_TYPE.CATEGORY:
                    elem.difference_index = br.ReadInt32();
                    break;
            }
            return elem;
        }

        public void save_diff_data(string fname)
        {
            FileStream fs = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Write);
            fs.SetLength(0);
            BinaryWriter bw = new BinaryWriter(fs, Encoding.Default);

            bw.Write((Byte)SFDiffElement.DIFF_TYPE.MD5);
            bw.Write(manager.get_data_md5().ToCharArray());

            for (int i = 0; i < manager.get_category_number(); i++)
            {
                SFCategory cat = manager.get_category(i);
                write_diff_element(bw, null, new SFDiffElement(SFDiffElement.DIFF_TYPE.CATEGORY, i));
                for (int j = 0; j < diff_data[i].Count; j++)
                {
                    write_diff_element(bw, cat, diff_data[i][j]);
                }
            }

            write_diff_element(bw, null, new SFDiffElement(SFDiffElement.DIFF_TYPE.EOF));

            bw.Close();
            fs.Close();
        }

        public bool load_diff_data(string fname)
        {
            FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs, Encoding.Default);

            int current_category = -1;
            SFDiffElement elem;
            bool success = true;

            while (br.PeekChar() != -1)
            {
                elem = read_diff_element(br, manager.get_category(current_category));
                if (elem.difference_type == SFDiffElement.DIFF_TYPE.CATEGORY)
                    current_category = elem.difference_index;
                else if (elem.difference_type == SFDiffElement.DIFF_TYPE.EOF)
                    break;
                else if (elem.difference_type == SFDiffElement.DIFF_TYPE.MD5)   //should be right at the beginning of the file
                {
                    if(presumed_md5 != manager.get_data_md5())
                    {
                        clear_data();
                        success = false;
                        break;
                    }
                }
                else
                    push_change(current_category, elem);
            }

            br.Close();
            fs.Close();
            return success;
        }

        public void commit_change(SFCategory cat, SFDiffElement elem)
        {
            List<SFCategoryElement> elem_list = cat.get_elements();
            switch (elem.difference_type)
            {
                case SFDiffElement.DIFF_TYPE.REPLACE:
                    elem_list[elem.difference_index] = elem.elem;
                    break;
                case SFDiffElement.DIFF_TYPE.INSERT:
                    elem_list.Insert(elem.difference_index+1, elem.elem);
                    break;
                case SFDiffElement.DIFF_TYPE.REMOVE:
                    elem_list.RemoveAt(elem.difference_index);
                    break;
            }
        }

    public void merge_changes()
        {
            for(int i = 0; i < manager.get_category_number(); i++)
            {
                SFCategory cat = manager.get_category(i);
                List<SFCategoryElement> elem_list = cat.get_elements();
                for(int j = 0; j < diff_data[i].Count; j++)
                {
                    SFDiffElement elem = diff_data[i][j];
                    commit_change(cat, elem);
                }
            }
        }

        public void push_change(int cat_index, SFDiffElement elem)
        {
            diff_data[cat_index].Add(elem);
            Console.WriteLine("NEW CHANGE: " + elem.difference_type.ToString());
        }
    }
}
