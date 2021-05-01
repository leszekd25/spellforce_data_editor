/*
 * SFModCFFChangeElement is a struct representing a single deviation between two gamedata blocks
 * SFModCGGChanges is the container which holds these changes, and allows for generation of the changes bny specifying the two gamedata blocks
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using SpellforceDataEditor.SFCFF;


namespace SpellforceDataEditor.SFMod
{
    public enum SFModCFFChangeType
    {
        REMOVE = 0, INSERT, REPLACE, NEW_CATEGORY, NEWER_CATEGORY, MISSING_CATEGORY, OLDER_CATEGORY
    }

    public struct SFModCFFChangeElement
    {
        public SFModCFFChangeType type;
        public ushort category_index;
        public ushort element_index;
        public SFCategoryElement element;

        // loads a single change from a stream
        static public SFModCFFChangeElement Load(BinaryReader br)
        {
            SFModCFFChangeElement celem = new SFModCFFChangeElement();
            if (br.ReadUInt32() != 0)
                throw new Exception("SFModCFFChanges.Load(): Invalid data in stream!");
            celem.type = (SFModCFFChangeType)br.ReadInt16();
            celem.category_index = br.ReadUInt16();
            celem.element_index = br.ReadUInt16();
            celem.element = null;
            if (celem.type == SFModCFFChangeType.REMOVE)
                return celem;
            celem.element = new SFCategoryElement();
            try
            {
                celem.element.AddVariants(SFCategoryManager.gamedata[celem.category_index].GetElementFromBuffer(br));
            }
            catch(Exception)
            {
                throw new InvalidDataException("SFModCFFChangeElement.Load: Can't load element!");
            }
            return celem;
        }

        // saves a change to a stream
        public int Save(BinaryWriter bw)
        {
            bw.Write((UInt32)0);
            bw.Write((Int16)type);
            bw.Write(category_index);
            bw.Write(element_index);
            if (type == SFModCFFChangeType.REMOVE)
                return 0;
            if (element == null)
                return -1;
            SFCategoryManager.gamedata[category_index].WriteElementToBuffer(bw, element.variants);
            return 0;
        }

        public override string ToString()
        {
            return type.ToString() + " | " + category_index.ToString() + ", " + element_index.ToString();
        }
    }

    public class SFModCFFChanges
    {
        public List<SFModCFFChangeElement> changes { get; private set; } = new List<SFModCFFChangeElement>();

        // loads list of changes from a stream
        public void Load(BinaryReader br)
        {
            changes.Clear();
            br.ReadInt64();
            int change_count = br.ReadInt32();
            for(int i = 0; i < change_count; i++)
            {
                changes.Add(SFModCFFChangeElement.Load(br));
            }
            return;
        }

        // frees memory*
        public void Unload()
        {
            changes.Clear();
        }

        // saves the changes to a stream
        public int Save(BinaryWriter bw)
        {
            long init_pos = bw.BaseStream.Position;
            bw.Write((long)0);

            bw.Write(changes.Count);
            for(int i = 0; i < changes.Count; i++)
            {
                if (changes[i].Save(bw) != 0)
                    return -1;
            }

            long new_pos = bw.BaseStream.Position;
            bw.BaseStream.Position = init_pos;
            bw.Write(new_pos - init_pos);
            bw.BaseStream.Position = new_pos;
            return 0;
        }

        // called from ModCreator upon selecting a file
        // generates a list of changes given two data files
        // returns whether succeeded
        public int GenerateDiff(string orig_fname, string new_fname)
        {
            changes.Clear();

            SFChunk.SFChunkFile sfcf_orig = new SFChunk.SFChunkFile();
            SFChunk.SFChunkFile sfcf_new = new SFChunk.SFChunkFile();
            sfcf_orig.OpenFile(orig_fname);
            sfcf_new.OpenFile(new_fname);

            SFCategory orig_data = null;
            SFCategory new_data = null;

            SFGameData gd_orig = new SFGameData();
            SFGameData gd_new = new SFGameData();
            gd_orig.Load(orig_fname);
            gd_new.Load(new_fname);

            foreach(var cat in gd_orig.categories)
            {
                orig_data = cat.Value;
                new_data = gd_new[cat.Key];
                // if original gamedata contains category that the new gamedata does not contain, add MISSING change
                if(new_data == null)
                {
                    changes.Add(new SFModCFFChangeElement() { type = SFModCFFChangeType.MISSING_CATEGORY, category_index = (ushort)cat.Key });
                    continue;
                }

                // if original category is newer than new category, add OLDER change
                if(new_data.category_type < orig_data.category_type)
                {
                    changes.Add(new SFModCFFChangeElement() { type = SFModCFFChangeType.OLDER_CATEGORY, category_index = (ushort)cat.Key });
                    continue;
                }
                // if original category is older than new category, add NEWER change
                if(new_data.category_type > orig_data.category_type)
                {
                    changes.Add(new SFModCFFChangeElement() { type = SFModCFFChangeType.NEWER_CATEGORY, category_index = (ushort)cat.Key });
                    continue;
                }

                // now finally find changed elements

                int orig_i = 0;
                int new_i = 0;
                int orig_id, new_id;
                bool orig_end = false;
                bool new_end = false;

                while (true)
                {
                    bool is_change = false;
                    SFModCFFChangeType change_type = SFModCFFChangeType.REPLACE;

                    if (new_i == new_data.GetElementCount())
                        new_end = true;
                    if (orig_i == orig_data.GetElementCount())
                        orig_end = true;

                    if (orig_end && new_end)
                        break;

                    orig_id = orig_data.GetElementID(orig_i);
                    new_id = new_data.GetElementID(new_i);

                    if (orig_end)
                    {
                        is_change = true;
                        change_type = SFModCFFChangeType.INSERT;
                    }
                    else if (new_end)
                    {
                        is_change = true;
                        change_type = SFModCFFChangeType.REMOVE;
                    }
                    else
                    {
                        if (orig_id == new_id)
                        {
                            if (orig_data[orig_i].SameAs(new_data[new_i]))
                            {
                                // no change, addition, or deletion
                            }
                            else
                            {
                                is_change = true;
                                change_type = SFModCFFChangeType.REPLACE;
                                // change!
                            }
                        }
                        else if (orig_id > new_id)
                        {
                            is_change = true;
                            change_type = SFModCFFChangeType.INSERT;
                            // addition!

                            orig_i -= 1;
                        }
                        else if (orig_id < new_id)
                        {
                            is_change = true;
                            change_type = SFModCFFChangeType.REMOVE;
                            // deletion!

                            new_i -= 1;
                        }
                    }

                    if (is_change)
                    {
                        SFModCFFChangeElement change_elem = new SFModCFFChangeElement();
                        change_elem.type = change_type;
                        change_elem.category_index = (ushort)cat.Key;
                        if (change_type != SFModCFFChangeType.REMOVE)
                        {
                            change_elem.element = new_data[new_i].GetCopy();
                            change_elem.element_index = (UInt16)(new_id);
                        }
                        else
                            change_elem.element_index = (UInt16)(orig_id);
                        changes.Add(change_elem);
                    }

                    if (orig_i < orig_data.GetElementCount())
                        orig_i += 1;
                    if (new_i < new_data.GetElementCount())
                        new_i += 1;
                }
            }

            // have to find categories that do not exist in old gamedata now
            foreach (var cat in gd_new.categories)
            {
                new_data = cat.Value;
                orig_data = gd_orig[cat.Key];
                // if new gamedata contains category that the old gamedata does not contain, add NEW change
                if (orig_data == null)
                {
                    changes.Add(new SFModCFFChangeElement() { type = SFModCFFChangeType.NEW_CATEGORY, category_index = (ushort)cat.Key });
                    continue;
                }
            }

            gd_new.Unload();
            gd_orig.Unload();
            return 0;
        }

        // applies the change to the gamedata stored in the memory
        // this stored gamedata will ultimately replace the one in spellforce directory
        public int Apply(SFGameData gd)
        {
            for(int i = 0; i < changes.Count; i++)
            {
                SFModCFFChangeElement change = changes[i];
                SFCategory cat = gd[change.category_index];
                List<SFCategoryElement> elems = cat.elements;
                int index;
                switch (change.type)
                {
                    case SFModCFFChangeType.REPLACE:
                        index = cat.GetElementIndex(change.element_index);
                        if (index == -1)
                            elems.Insert(index, change.element);
                        else
                            elems[index] = change.element;
                        break;
                    case SFModCFFChangeType.REMOVE:
                        index = cat.GetElementIndex(change.element_index);
                        if (index == -1)
                            break;
                        //elems.RemoveAt(index);                  // NOTE: mods can not remove elements from gamedata, as it would effectively destroy transitive properties of mods (order does not matter)
                        break;
                    case SFModCFFChangeType.INSERT:
                        // find suitable position for an element
                        index = cat.GetNewElementIndex(change.element_index);
                        if (index == -1)
                        {
                            index = cat.GetElementIndex(change.element_index);
                            elems[index] = change.element;
                        }
                        else
                            elems.Insert(index, change.element);
                        break;
                    default:
                        continue;
                }
            }
            return 0;
        }

        // deletions temporarily pulled out
        public override string ToString()
        {
            string ret = "";
            int[] stat = new int[7];
            foreach(SFModCFFChangeElement e in changes)
                stat[(int)e.type]++;
            ret += "Deletions: " + stat[0].ToString() + ", additions: " + stat[1].ToString() + ", modifications: " + stat[2].ToString() 
                + ", new categories: " + stat[3].ToString() + ", newer categories: " + stat[4].ToString()
                + ", missing categories: " + stat[5].ToString() + ", older categories: " + stat[6].ToString();
            return ret;
        }
    }
}
