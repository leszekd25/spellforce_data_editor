using SFEngine.SFChunk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    public delegate void dOnVertex2030Added(int elem_index, int subelem_index, int v_index);
    public delegate void dOnVertex2030Modified(int elem_index, int subelem_index, int v_index);
    public delegate void dOnVertex2030Removed(int elem_index, int subelem_index, int v_index);

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2030Item: ICategorySubItem
    {
        public ushort BuildingID;
        public byte PolygonID;
        public byte CastsShadow;
        public List<short> Coords;

        public int GetByteCount() { return 5 + 2 * Coords.Count; }

        public int GetID() => BuildingID;
        public void SetID(int id) => BuildingID = (ushort)id;
        public int GetSubID() => PolygonID;
        public void SetSubID(int subid) => PolygonID = (byte)subid;

        public void CopyTo(ref Category2030Item item)
        {
            item.BuildingID = BuildingID;
            item.PolygonID = PolygonID;
            item.CastsShadow = CastsShadow;
            item.Coords = new List<short>(Coords);
        }

        public bool IsEqualTo(ref Category2030Item item)
        {
            if((BuildingID == item.BuildingID)&&(PolygonID == item.PolygonID)&&(CastsShadow == item.CastsShadow))
            {
                if(Coords.SequenceEqual(item.Coords))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class Category2030 : ICategory
    {
        bool Loaded;
        UndoRedoQueue urq;

        public dOnElementAdded OnElementAdded;
        public dOnElementRemoved OnElementRemoved;
        public dOnSubElementAdded OnSubElementAdded;
        public dOnSubElementModified OnSubElementModified;
        public dOnSubElementRemoved OnSubElementRemoved;
        public dOnVertex2030Added OnVertexAdded;
        public dOnVertex2030Modified OnVertexModified;
        public dOnVertex2030Removed OnVertexRemoved;

        public virtual string GetName()
        {
            return "Building collision data";
        }

        public virtual short GetCategoryID()
        {
            return 2030;
        }

        public virtual short GetCategoryType()
        {
            return 2;
        }

        public int GetNumOfItems()
        {
            return Indices.Count;
        }

        public int GetByteCount()
        {
            int v = 0;
            for(int i = 0; i < Items.Count; i++)
            {
                v += Items[i].GetByteCount();
            }
            return v;
        }

        public List<Category2030Item> Items = new List<Category2030Item>();
        public List<int> Indices = new List<int>();

        public int GetSubItemIndex(int index, int subindex)
        {
            int from_start = Indices[index];
            int from_end;
            if (index == Indices.Count - 1)
            {
                from_end = Items.Count;
            }
            else
            {
                from_end = Indices[index + 1];
            }
            if (from_start + subindex >= from_end)
            {
                throw new Exception();
            }

            return from_start + subindex;
        }

        public Category2030Item this[int index]
        {
            get
            {
                return Items[index];
            }
        }

        public Category2030Item this[int index, int subindex]
        {
            get
            {
                return Items[GetSubItemIndex(index, subindex)];
            }
        }

        void CalculateIndices()
        {
            int cur_item_id = -1;
            for (int i = 0; i < Items.Count; i++)
            {
                int item_id = Items[i].GetID();
                if (item_id != cur_item_id)
                {
                    cur_item_id = item_id;
                    Indices.Add(i);
                }
            }
        }

        public void AdjustIndices(int index_start, int increment)
        {
            for (int i = index_start; i < Indices.Count; i++)
            {
                Indices[i] += increment;
            }
        }

        public bool Load(SFChunkFile file)
        {
            SFChunkFileChunk chunk = file.GetChunkByID(GetCategoryID());
            if (chunk == null)
            {
                return true;
            }
            if (chunk.header.ChunkDataType != GetCategoryType())
            {
                return false;
            }
            BinaryReader br = chunk.Open();

            while(br.BaseStream.Position < br.BaseStream.Length)
            {
                Category2030Item item = new Category2030Item();
                item.BuildingID = br.ReadUInt16();
                item.PolygonID = br.ReadByte();
                item.CastsShadow = br.ReadByte();
                item.Coords = new List<short>();
                ushort coords_num = (ushort)(br.ReadByte());
                for(int i = 0; i < coords_num; i++)
                {
                    short x = br.ReadInt16();
                    short y = br.ReadInt16();
                    item.Coords.Add(x); item.Coords.Add(y);
                }
                Items.Add(item);
            }

            chunk.Close();

            CalculateIndices();

            Loaded = true;
            return true;
        }

        public bool IsLoaded()
        {
            return Loaded;
        }

        public bool Save(SFChunkFile sfcf)
        {
            if (Items.Count == 0)
            {
                return true;
            }

            byte[] data = new byte[GetByteCount()];
            using MemoryStream ms = new MemoryStream(data);
            using BinaryWriter bw = new BinaryWriter(ms);

            for(int i = 0; i < Items.Count; i++)
            {
                Category2030Item item = Items[i];
                bw.Write(item.BuildingID);
                bw.Write(item.PolygonID);
                bw.Write(item.CastsShadow);
                bw.Write((byte)(item.Coords.Count / 2));
                for(int j = 0; j < item.Coords.Count; j++)
                {
                    bw.Write(item.Coords[j]);
                }
            }

            sfcf.AddChunk(GetCategoryID(), 0, false, GetCategoryType(), data);

            return true;
        }

        public bool MergeFrom(ICategory c1, ICategory c2)
        {
            if ((!(c1 is Category2030)) || (!(c2 is Category2030)))
            {
                return false;
            }
            Category2030 cat1 = c1 as Category2030;
            Category2030 cat2 = c2 as Category2030;

            // double list ladder
            int orig_i = 0;
            int new_i = 0;
            int orig_id = 0;
            int new_id = 0;
            bool orig_end = false;
            bool new_end = false;

            while (true)
            {
                new_end = (new_i == cat2.GetNumOfItems());
                orig_end = (orig_i == cat1.GetNumOfItems());

                if (orig_end && new_end)
                {
                    break;
                }

                if (!orig_end)
                {
                    cat1.GetID(orig_i, out orig_id);
                }
                if (!new_end)
                {
                    cat2.GetID(new_i, out new_id);
                }

                if (orig_end)
                {
                    int main_index = cat2.Indices[new_i];
                    for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                    {
                        Category2030Item it = new();
                        cat2[main_index + j].CopyTo(ref it);
                        Items.Add(it);
                    }
                }
                else if (new_end)
                {
                    int main_index = cat1.Indices[orig_i];
                    for (int j = 0; j < cat1.GetItemSubItemNum(orig_i); j++)
                    {
                        Category2030Item it = new();
                        cat1[main_index + j].CopyTo(ref it);
                        Items.Add(it);
                    }
                }
                else
                {
                    if (orig_id == new_id)
                    {
                        int main_index = cat2.Indices[new_i];
                        for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                        {
                            Category2030Item it = new();
                            cat2[main_index + j].CopyTo(ref it);
                            Items.Add(it);
                        }
                    }
                    else if (orig_id > new_id)
                    {
                        int main_index = cat2.Indices[new_i];
                        for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                        {
                            Category2030Item it = new();
                            cat2[main_index + j].CopyTo(ref it);
                            Items.Add(it);
                        }
                        // addition!

                        orig_i -= 1;
                    }
                    else if (orig_id < new_id)
                    {
                        int main_index = cat1.Indices[orig_i];
                        for (int j = 0; j < cat1.GetItemSubItemNum(orig_i); j++)
                        {
                            Category2030Item it = new();
                            cat1[main_index + j].CopyTo(ref it);
                            Items.Add(it);
                        }

                        new_i -= 1;
                    }
                }

                if (!orig_end)
                {
                    orig_i += 1;
                }
                if (!new_end)
                {
                    new_i += 1;
                }
            }

            CalculateIndices();
            Loaded = true;
            return true;
        }

        public bool DiffFrom(ICategory c1, ICategory c2)
        {
            if ((!(c1 is Category2030)) || (!(c2 is Category2030)))
            {
                return false;
            }
            Category2030 cat1 = c1 as Category2030;
            Category2030 cat2 = c2 as Category2030;

            // climb double list ladder
            // elems have same IDs: only add elem2 if the elems themselves arent equal
            // elem1 has lower ID than elem2: dont add any elem
            // elem1 has higher ID than elem2: add elem2

            // double list ladder

            Span<Category2030Item> items1_span = CollectionsMarshal.AsSpan(cat1.Items);
            Span<Category2030Item> items2_span = CollectionsMarshal.AsSpan(cat2.Items);
            int orig_i = 0;
            int new_i = 0;
            int orig_id = 0;
            int new_id = 0;
            bool orig_end = false;
            bool new_end = false;

            while (true)
            {
                new_end = (new_i == cat2.GetNumOfItems());
                orig_end = (orig_i == cat1.GetNumOfItems());

                if (orig_end && new_end)
                {
                    break;
                }

                if (!orig_end)
                {
                    cat1.GetID(orig_i, out orig_id);
                }
                if (!new_end)
                {
                    cat2.GetID(new_i, out new_id);
                }

                if (orig_end)
                {
                    int main_index = cat2.Indices[new_i];
                    for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                    {
                        Category2030Item it = new();
                        cat2[main_index + j].CopyTo(ref it);
                        Items.Add(it);
                    }
                }
                else if (new_end)
                {

                }
                else
                {
                    if (orig_id == new_id)
                    {
                        if (cat1.GetItemSubItemNum(orig_i) != cat2.GetItemSubItemNum(new_i))
                        {
                            int main_index = cat2.Indices[new_i];
                            for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                            {
                                Category2030Item it = new();
                                cat2[main_index + j].CopyTo(ref it);
                                Items.Add(it);
                            }
                        }
                        else
                        {
                            int main_index1 = cat1.Indices[orig_i];
                            int main_index2 = cat2.Indices[new_i];
                            bool equal = true;
                            for(int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                            {
                                if (!cat1[main_index1+j].IsEqualTo(ref items2_span[main_index2+j]))
                                {
                                    equal = false;
                                    break;
                                }
                            }
                            if (!equal)
                            {
                                int main_index = cat2.Indices[new_i];
                                for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                                {
                                    Category2030Item it = new();
                                    cat2[main_index + j].CopyTo(ref it);
                                    Items.Add(it);
                                }
                            }
                        }
                    }
                    else if (orig_id > new_id)
                    {
                        int main_index = cat2.Indices[new_i];
                        for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                        {
                            Category2030Item it = new();
                            cat2[main_index + j].CopyTo(ref it);
                            Items.Add(it);
                        }
                        // addition!

                        orig_i -= 1;
                    }
                    else if (orig_id < new_id)
                    {

                        new_i -= 1;
                    }
                }

                if (!orig_end)
                {
                    orig_i += 1;
                }
                if (!new_end)
                {
                    new_i += 1;
                }
            }

            CalculateIndices();
            Loaded = true;
            return true;
        }

        public bool Clear()
        {
            Items.Clear();
            Indices.Clear();
            urq = null;
            Loaded = false;
            return true;
        }

        public bool Sort()
        {
            Items.Sort();
            Indices.Clear();
            CalculateIndices();
            return true;
        }

        public bool AddEmpty(int new_index)
        {
            IUndoRedo iur = new UndoRedoElementAdd2030()
            {
                cat = this,
                index = new_index,
                item = new() { Coords = new() }
            };
            iur.Commit(urq);

            return true;
        }

        public bool AddID(int new_index, int new_id)
        {
            Category2030Item item = new() { Coords = new() };
            item.SetID(new_id);

            IUndoRedo iur = new UndoRedoElementAdd2030()
            {
                cat = this,
                index = new_index,
                item = item
            };
            iur.Commit(urq);

            return true;
        }

        public bool AddItem(int new_index, Category2030Item item)
        {
            IUndoRedo iur = new UndoRedoElementAdd2030()
            {
                cat = this,
                index = new_index,
                item = item
            };
            iur.Commit(urq);

            return true;
        }

        public bool AddSubItem(int new_index, int new_subindex, Category2030Item item)
        {
            IUndoRedo iur = new UndoRedoSubElementAdd2030()
            {
                cat = this,
                index = new_index,
                subindex = new_subindex,
                item = item
            };
            iur.Commit(urq);

            return true;
        }

        public bool SetSubItem(int index, int subindex, Category2030Item item)
        {
            IUndoRedo iur = new UndoRedoSubElementReplace2030()
            {
                cat = this,
                index = index,
                subindex = subindex,
                new_item = item
            };
            iur.Commit(urq);

            return true;
        }

        public bool AddCoord(int index, int subindex, int new_coord_index, short x, short y)
        {
            IUndoRedo iur = new UndoRedoVertexAdd2030()
            {
                cat = this,
                index = index,
                subindex = subindex,
                v_index = new_coord_index,
                new_x = x,
                new_y = y
            };
            iur.Commit(urq);

            return true;
        }

        public bool SetCoord(int index, int subindex, int coord_index, short x, short y)
        {
            IUndoRedo iur = new UndoRedoVertexModify2030()
            {
                cat = this,
                index = index,
                subindex = subindex,
                v_index = coord_index,
                new_x = x,
                new_y = y
            };
            iur.Commit(urq);

            return true;
        }

        public bool RemoveCoord(int index, int subindex, int coord_index)
        {
            IUndoRedo iur = new UndoRedoVertexRemove2030()
            {
                cat = this,
                index = index,
                subindex = subindex,
                v_index = coord_index,
            };
            iur.Commit(urq);

            return true;
        }

        public bool Copy(int from_index, int new_index)
        {
            int from_start = Indices[from_index];
            int from_end;
            if (from_index == Indices.Count - 1)
            {
                from_end = Items.Count - 1;
            }
            else
            {
                from_end = Indices[from_index + 1] - 1;
            }

            urq?.OpenCluster();
            IUndoRedo iur = new UndoRedoElementAdd2030()
            {
                cat = this,
                index = new_index,
                item = Items[from_start]
            };
            iur.Commit(urq);
            for (int i = 1; i <= (from_end - from_start); i++)
            {
                IUndoRedo iur2 = new UndoRedoSubElementAdd2030()
                {
                    cat = this,
                    index = new_index,
                    subindex = i,
                    item = Items[from_start + i]
                };
                iur2.Commit(urq);
            }
            urq?.CloseCluster();

            return true;
        }

        public bool Remove(int index)
        {
            IUndoRedo iur = new UndoRedoElementRemove2030()
            {
                cat = this,
                index = index,
            };
            iur.Commit(urq);

            return true;
        }

        public bool RemoveSub(int index, int subindex)
        {
            IUndoRedo iur = new UndoRedoSubElementRemove2030()
            {
                cat = this,
                index = index,
                subindex = subindex
            };
            iur.Commit(urq);

            return true;
        }

        public bool GetID(int index, out int id)
        {
            id = Items[Indices[index]].GetID();
            return true;
        }

        public bool SetID(int index, int id)
        {
            IUndoRedo iur = new UndoRedoElementSetID2030()
            {
                cat = this,
                index = index,
                new_id = id
            };
            iur.Commit(urq);

            return true;
        }

        public bool CalculateNewItemIndex(int new_id, out int index)
        {
            int current_start = 0;
            int current_end = Indices.Count - 1;
            int current_center;
            int intermediate;
            int val;
            while (current_start <= current_end)
            {
                current_center = (current_start + current_end) / 2;    //care about overflow (though its not happening in this case)
                intermediate = Indices[current_center];
                val = Items[intermediate].GetID();
                if (val == new_id)
                {
                    index = current_center;
                    return false;
                }

                if (val < new_id)
                {
                    current_start = current_center + 1;
                }
                else
                {
                    current_end = current_center - 1;
                }
            }
            index = current_start;
            return true;
        }

        public bool GetItemString(int index, SFGameDataNew gd, out string str)
        {
            str = Items[Indices[index]].GetID().ToString();
            return true;
        }

        public bool GetItemDescription(int index, SFGameDataNew gd, out string desc)
        {
            desc = $"DESC {Items[Indices[index]].GetID()}";
            return true;
        }

        public bool GetItemIndex(int id, out int index)
        {
            int current_start = 0;
            int current_end = Indices.Count - 1;
            int current_center;
            while (current_start <= current_end)
            {
                current_center = (current_start + current_end) / 2;    //care about overflow
                int intermediate = Indices[current_center];
                int cur_id = Items[intermediate].GetID();
                if (cur_id == id)
                {
                    index = current_center;
                    return true;
                }

                if (cur_id < id)
                {
                    current_start = current_center + 1;
                }
                else
                {
                    current_end = current_center - 1;
                }
            }

            index = Utility.NO_INDEX;
            return false;
        }

        public bool GetItemSubItemIndex(int id, int index, out int subindex)
        {
            bool result = GetItemIndex(id, out subindex);
            if (result)
            {
                subindex += index;
            }
            return result;
        }

        public int GetItemSubItemNum(int index)
        {
            if (index == Indices.Count - 1)
            {
                return Items.Count - Indices[index];
            }
            return Indices[index + 1] - Indices[index];
        }

        public bool GetItemSubIndex(int id, int subid, out int index)
        {
            if (!GetItemIndex(id, out index))
            {
                return false;
            }
            index = Indices[index];

            for (; index < Items.Count; index++)
            {
                if (Items[index].GetSubID() == subid)
                {
                    return true;
                }
                if (Items[index].GetID() != id)
                {
                    return false;
                }
            }
            return false;
        }

        public void SetField<U>(int index, int subindex, string field_name, U value)
        {
            IUndoRedo iur = new UndoRedoSubElementSetField2030<U>()
            {
                cat = this,
                index = index,
                subindex = subindex,
                field_name = field_name,
                value = value
            };
            iur.Commit(urq);
        }

        public bool GetFirstUnusedID(out int id, out int index)
        {
            if (Indices.Count == 0)
            {
                id = 1;
                index = 0;
                return true;
            }
            int cur_id = 1;
            for (int i = 0; i < Indices.Count; i++)
            {
                if (Items[Indices[i]].GetID() == 0)
                {
                    continue;
                }
                if (cur_id != Items[Indices[i]].GetID())
                {
                    id = cur_id;
                    index = i;
                    return true;
                }
                cur_id++;
            }
            id = Items[Indices[^1]].GetID() + 1;
            index = Indices.Count;
            return true;
        }

        public bool GetLastUsedID(out int id, out int index)
        {
            if (Indices.Count == 0)
            {
                id = 0;
                index = 0;
                return true;
            }
            id = Items[Indices[^1]].GetID();
            index = Indices.Count;
            return true;
        }

        public bool EnableUndoRedo(UndoRedoQueue queue)
        {
            urq = queue;
            return true;
        }

        public bool SetOnElementAddedCallback(dOnElementAdded cb)
        {
            OnElementAdded = cb;
            return true;
        }

        public bool SetOnElementModifiedCallback(dOnElementModified cb)
        {
            return false;
        }

        public bool SetOnElementRemovedCallback(dOnElementRemoved cb)
        {
            OnElementRemoved = cb;
            return true;
        }

        public bool SetOnSubElementModifiedCallback(dOnSubElementModified cb)
        {
            OnSubElementModified = cb;
            return true;
        }

        public bool SetOnSubElementAddedCallback(dOnSubElementAdded cb)
        {
            OnSubElementAdded = cb;
            return true;
        }

        public bool SetOnSubElementRemovedCallback(dOnSubElementRemoved cb)
        {
            OnSubElementRemoved = cb;
            return true;
        }

        public bool SetOnVertexAdded(dOnVertex2030Added cb)
        {
            OnVertexAdded = cb;
            return true;
        }

        public bool SetOnVertexModified(dOnVertex2030Modified cb)
        {
            OnVertexModified = cb;
            return true;
        }

        public bool SetOnVertexRemoved(dOnVertex2030Removed cb)
        {
            OnVertexRemoved = cb;
            return true;
        }

        public bool ClearCallbacks()
        {
            OnElementAdded = null;
            OnElementRemoved = null;
            OnSubElementAdded = null;
            OnSubElementModified = null;
            OnSubElementRemoved = null;
            OnVertexAdded = null;
            OnVertexModified = null;
            OnVertexRemoved = null;
            return true;
        }

        public virtual List<string> GetSearchableFields()
        {
            return new(){ "BuildingID", "PolygonID", "CastsShadow" };
        }

        public List<int> QueryItems(object value, string field_name, SearchOption option)
        {
            if (field_name == "")  // search all fields
            {
                HashSet<int> result = new();
                List<int> intermediate = new();
                foreach (var f in GetSearchableFields())
                {
                    intermediate.Clear();
                    QueryItemsField(value, f, option, intermediate);
                    result.UnionWith(intermediate);
                }
                return result.ToList();
            }
            else
            {
                List<int> result = new();
                QueryItemsField(value, field_name, option, result);
                return result;
            }
        }

        void QueryItemsField(object value, string field_name, SearchOption option, List<int> result)
        {
            try
            {
                Utility.GetFieldData<Category2030Item>(field_name, out uint field_offset, out Type ft, out uint field_num);
                if ((option & SearchOption.IS_NUMBER) != SearchOption.NONE)
                {
                    bool as_flag = (option & SearchOption.NUMBER_AS_BITMASK) != SearchOption.NONE;
                    if (field_num != 1)
                    {
                        return;
                    }
                    int num = (int)value;
                    if (ft == typeof(byte))
                    {
                        QueryItemsFieldNumber<byte>(num, field_offset, as_flag, result);
                    }
                    else if (ft == typeof(sbyte))
                    {
                        QueryItemsFieldNumber<sbyte>(num, field_offset, as_flag, result);
                    }
                    else if (ft == typeof(ushort))
                    {
                        QueryItemsFieldNumber<ushort>(num, field_offset, as_flag, result);
                    }
                    else if (ft == typeof(short))
                    {
                        QueryItemsFieldNumber<short>(num, field_offset, as_flag, result);
                    }
                    else if (ft == typeof(uint))
                    {
                        QueryItemsFieldNumber<uint>(num, field_offset, as_flag, result);
                    }
                    else if (ft == typeof(int))
                    {
                        QueryItemsFieldNumber<int>(num, field_offset, as_flag, result);
                    }
                }
                else if ((option & SearchOption.IS_STRING) != SearchOption.NONE)
                {
                    if (field_num == 1)
                    {
                        return;
                    }
                    if (ft != typeof(byte))
                    {
                        return;
                    }

                    string str = (string)value;
                    QueryItemsFieldString(str, field_offset, field_num, (option & SearchOption.NUMBER_AS_BITMASK) != SearchOption.NONE, result);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        void QueryItemsFieldNumber<U>(int num, uint field_offset, bool as_flag, List<int> result)
        {
            Span<Category2030Item> items_span = CollectionsMarshal.AsSpan(Items);
            int cur_value = 0;
            unsafe
            {
                fixed (Category2030Item* ptr = items_span)
                {
                    if (as_flag)
                    {
                        for (int i = 0; i < Indices.Count; i++)
                        {
                            for (int j = 0; j < GetItemSubItemNum(i); j++)
                            {
                                U* ptr2 = (U*)(((byte*)(&ptr[Indices[i] + j])) + field_offset);
                                cur_value = Convert.ToInt32(*ptr2);

                                if (cur_value == num)
                                {
                                    result.Add(i);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Indices.Count; i++)
                        {
                            for (int j = 0; j < GetItemSubItemNum(i); j++)
                            {
                                U* ptr2 = (U*)(((byte*)(&ptr[Indices[i] + j])) + field_offset);
                                cur_value = Convert.ToInt32(*ptr2);

                                if (cur_value == num)
                                {
                                    result.Add(i);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        void QueryItemsFieldString(string str, uint field_offset, uint field_size, bool ignore_case, List<int> result)
        {
            Span<Category2030Item> items_span = CollectionsMarshal.AsSpan(Items);
            Encoding encoding = Encoding.GetEncoding(1252);
            unsafe
            {
                fixed (Category2030Item* ptr = items_span)
                {
                    if (ignore_case)
                    {
                        str = str.ToLowerInvariant();
                        for (int i = 0; i < Indices.Count; i++)
                        {
                            for (int j = 0; j < GetItemSubItemNum(i); j++)
                            {
                                byte* ptr2 = (((byte*)(&ptr[Indices[i] + j])) + field_offset);

                                string str2 = encoding.GetString(ptr2, (int)field_size).ToLowerInvariant();
                                if (str2.Contains(str))
                                {
                                    result.Add(i);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Indices.Count; i++)
                        {
                            for (int j = 0; j < GetItemSubItemNum(i); j++)
                            {
                                byte* ptr2 = (((byte*)(&ptr[Indices[i] + j])) + field_offset);

                                string str2 = encoding.GetString(ptr2, (int)field_size);
                                if (str2.Contains(str))
                                {
                                    result.Add(i);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class UndoRedoElementAdd2030 : IUndoRedo
    {
        public Category2030 cat;
        public int index;
        public Category2030Item item;

        public bool Init()
        {
            Redo();
            return true;
        }

        public void Undo()
        {
            int main_index;
            if (index >= cat.Indices.Count)
            {
                main_index = cat.Items.Count;
            }
            else
            {
                main_index = cat.Indices[index];
            }

            cat.Items.RemoveAt(main_index);
            cat.Indices.RemoveAt(index);
            cat.AdjustIndices(index, -1);
            cat.OnElementRemoved?.Invoke(cat.GetCategoryID(), index);
        }

        public void Redo()
        {
            int main_index;
            if (index >= cat.Indices.Count)
            {
                main_index = cat.Items.Count;
            }
            else
            {
                main_index = cat.Indices[index];
            }
            cat.Items.Insert(main_index, item);
            cat.Indices.Insert(index, main_index);
            cat.AdjustIndices(index + 1, 1);
            cat.OnElementAdded?.Invoke(cat.GetCategoryID(), index);
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Add item at index {index}";
        }
    }

    public class UndoRedoElementSetID2030 : IUndoRedo
    {
        public Category2030 cat;
        public int index;
        public int new_id;
        int previous_id;

        public bool Init()
        {
            if (index >= cat.Indices.Count)
            {
                throw new Exception();
            }

            cat.GetID(index, out previous_id);
            if(previous_id == new_id)
            {
                return false;
            }
            Redo();
            return true;
        }

        public void Undo()
        {
            Span<Category2030Item> span = CollectionsMarshal.AsSpan(cat.Items);
            int from_start = cat.Indices[index];
            int from_end;
            if (index == cat.Indices.Count - 1)
            {
                from_end = cat.Items.Count - 1;
            }
            else
            {
                from_end = cat.Indices[index + 1] - 1;
            }

            for (int i = from_start; i <= from_end; i++)
            {
                span[i].SetID(previous_id);
                cat.OnSubElementModified?.Invoke(cat.GetCategoryID(), index, i - from_start);
            }
        }

        public void Redo()
        {
            Span<Category2030Item> span = CollectionsMarshal.AsSpan(cat.Items);
            int from_start = cat.Indices[index];
            int from_end;
            if (index == cat.Indices.Count - 1)
            {
                from_end = cat.Items.Count - 1;
            }
            else
            {
                from_end = cat.Indices[index + 1] - 1;
            }

            for (int i = from_start; i <= from_end; i++)
            {
                span[i].SetID(new_id);
                cat.OnSubElementModified?.Invoke(cat.GetCategoryID(), index, i - from_start);
            }
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Set ID of item at index {index}";
        }
    }

    public class UndoRedoElementRemove2030 : IUndoRedo
    {
        public Category2030 cat;
        public int index;
        List<Category2030Item> item;

        public bool Init()
        {
            int main_index;
            if (index >= cat.Indices.Count)
            {
                throw new Exception();
            }
            main_index = cat.Indices[index];

            item = new();
            for (int i = 0; i < cat.GetItemSubItemNum(index); i++)
            {
                Category2030Item it = new();
                cat.Items[main_index + i].CopyTo(ref it);
                item.Add(it);
            }

            Redo();
            return true;
        }

        public void Undo()
        {
            int main_index;
            if (index >= cat.Indices.Count)
            {
                main_index = cat.Items.Count;
            }
            else
            {
                main_index = cat.Indices[index];
            }

            for (int i = 0; i < item.Count; i++)
            {
                cat.Items.Insert(main_index + i, item[i]);
            }
            cat.Indices.Insert(index, main_index);
            cat.AdjustIndices(index + 1, item.Count);
            cat.OnElementAdded?.Invoke(cat.GetCategoryID(), index);
        }

        public void Redo()
        {
            int from_start = cat.Indices[index];
            int from_end;
            if (index == cat.Indices.Count - 1)
            {
                from_end = cat.Items.Count - 1;
            }
            else
            {
                from_end = cat.Indices[index + 1] - 1;
            }

            for (int i = 0; i <= (from_end - from_start); i++)
            {
                cat.Items.RemoveAt(from_start);
            }
            cat.Indices.RemoveAt(index);
            cat.AdjustIndices(index, from_start - from_end);
            cat.OnElementRemoved?.Invoke(cat.GetCategoryID(), index);
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Remove item at index {index}";
        }
    }

    public class UndoRedoSubElementAdd2030 : IUndoRedo
    {
        public Category2030 cat;
        public int index;
        public int subindex;
        public Category2030Item item;

        public bool Init()
        {
            if (index >= cat.Indices.Count)
            {
                throw new Exception();
            }
            if (subindex > cat.GetItemSubItemNum(index))
            {
                throw new Exception();
            }

            Redo();
            return true;
        }

        public void Undo()
        {
            int main_index = cat.Indices[index];
            cat.Items.RemoveAt(main_index + subindex);
            cat.AdjustIndices(index + 1, -1);
            cat.OnSubElementRemoved?.Invoke(cat.GetCategoryID(), index, subindex);
        }

        public void Redo()
        {
            int main_index = cat.Indices[index];
            cat.Items.Insert(main_index + subindex, item);
            cat.AdjustIndices(index + 1, 1);
            cat.OnSubElementAdded?.Invoke(cat.GetCategoryID(), index, subindex);
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Add subitem at index {index}, {subindex}";
        }
    }

    public class UndoRedoSubElementReplace2030 : IUndoRedo
    {
        public Category2030 cat;
        public int index;
        public int subindex;
        public Category2030Item new_item;
        Category2030Item previous_item;

        public bool Init()
        {
            if (index >= cat.Indices.Count)
            {
                throw new Exception();
            }
            if (subindex > cat.GetItemSubItemNum(index))
            {
                throw new Exception();
            }

            cat[index, subindex].CopyTo(ref previous_item);
            Redo();
            return true;
        }

        public void Undo()
        {
            int main_index = cat.Indices[index];
            cat.Items[main_index + subindex] = previous_item;
            cat.OnSubElementModified?.Invoke(cat.GetCategoryID(), index, subindex);
        }

        public void Redo()
        {
            int main_index = cat.Indices[index];
            cat.Items[main_index + subindex] = new_item;
            cat.OnSubElementModified?.Invoke(cat.GetCategoryID(), index, subindex);
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Replace subitem at index {index}, {subindex}";
        }
    }

    public class UndoRedoSubElementRemove2030 : IUndoRedo
    {
        public Category2030 cat;
        public int index;
        public int subindex;
        Category2030Item item;

        public bool Init()
        {
            if (index >= cat.Indices.Count)
            {
                throw new Exception();
            }
            if (subindex > cat.GetItemSubItemNum(index))
            {
                throw new Exception();
            }

            cat[index, subindex].CopyTo(ref item);
            Redo();
            return true;
        }

        public void Undo()
        {
            int main_index = cat.Indices[index];
            cat.Items.Insert(main_index + subindex, item);
            cat.AdjustIndices(index + 1, 1);
            cat.OnSubElementAdded?.Invoke(cat.GetCategoryID(), index, subindex);
        }

        public void Redo()
        {
            int main_index = cat.Indices[index];
            cat.Items.RemoveAt(main_index + subindex);
            cat.AdjustIndices(index + 1, -1);
            cat.OnSubElementRemoved?.Invoke(cat.GetCategoryID(), index, subindex);
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Remove subitem at index {index}, {subindex}";
        }
    }

    public class UndoRedoSubElementSetField2030<U> : IUndoRedo
    {
        public Category2030 cat;
        public int index;
        public int subindex;
        public string field_name;
        public U value;
        Category2030Item new_item;
        Category2030Item previous_item;
        bool initialized = false;
        bool valid = false;

        public bool Init()
        {
            if (index >= cat.Indices.Count)
            {
                throw new Exception();
            }
            if (subindex > cat.GetItemSubItemNum(index))
            {
                throw new Exception();
            }

            // this is fine, because we NEVER modify coords with this
            previous_item = cat[index, subindex];
            Redo();
            if(!valid)
            {
                return false;
            }
            new_item = cat[index, subindex];
            initialized = true;
            return true;
        }

        public void Undo()
        {
            int main_index = cat.Indices[index];
            cat.Items[main_index + subindex] = previous_item;
            cat.OnSubElementModified?.Invoke(cat.GetCategoryID(), index, subindex);
        }

        public void Redo()
        {
            if (initialized)
            {
                int main_index = cat.Indices[index];
                cat.Items[main_index + subindex] = new_item;
                cat.OnSubElementModified?.Invoke(cat.GetCategoryID(), index, subindex);
            }
            else
            {
                bool result = false;
                Type t = typeof(Category2030Item);

                int real_index = cat.GetSubItemIndex(index, subindex);
                Span<Category2030Item> items_span = CollectionsMarshal.AsSpan(cat.Items);
                if ((real_index < 0) || (real_index >= items_span.Length))
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseMultiple<{t.Name}>.SetField(): Index out of range");
                    throw new Exception();
                }

                string field_real_name = field_name;
                ReadOnlySpan<char> field_span = field_name.AsSpan();
                // check if theres index in field name
                uint field_index = uint.MaxValue;
                if (field_span[^1] == ']')
                {
                    for (int i = field_span.Length - 2; i >= 0; i--)
                    {
                        if (field_span[i] == '[')
                        {
                            if (!uint.TryParse(field_span.Slice(i + 1, (field_span.Length - 1) - (i + 1)), out field_index))
                            {
                                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseMultiple<{t.Name}>.SetField(): Malformed array index");
                                throw new Exception();
                            }
                            field_real_name = new(field_span.Slice(0, i));
                            break;
                        }
                    }
                    if (field_index == uint.MaxValue)
                    {
                        LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseMultiple<{t.Name}>.SetField(): Malformed array index");
                        throw new Exception();
                    }
                }

                FieldInfo fi = t.GetField(field_real_name);
                if (fi == null)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseMultiple<{t.Name}>.SetField(): Unknown field {field_real_name}");
                    throw new Exception();
                }

                FixedBufferAttribute fb_attr = null;
                foreach (var attr in fi.GetCustomAttributes())
                {
                    if (attr is FixedBufferAttribute)
                    {
                        fb_attr = (FixedBufferAttribute)attr;
                        break;
                    }
                }
                if (fb_attr == null)
                {
                    // if there is array index supplied, error
                    if (field_index != uint.MaxValue)
                    {
                        LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseMultiple<{t.Name}>.SetField(): Array index used in non-buffer field {field_real_name}");
                        throw new Exception();
                    }
                    Type t2 = fi.FieldType;
                    if (t2 != typeof(U))
                    {
                        LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseMultiple<{t.Name}>.SetField(): Type of {field_real_name} doesnt match the argument type");
                        throw new Exception();
                    }

                    U cur_val = (U)fi.GetValue(items_span[real_index]);
                    if (!cur_val.Equals(value))
                    {
                        TypedReference tref = __makeref(items_span[real_index]);
                        fi.SetValueDirect(tref, value);
                        // undo/redo stuff

                        result = true;
                    }
                }
                else
                {
                    // if theres no index, set entire array with value
                    if (field_index == uint.MaxValue)
                    {
                        Type utype = typeof(U);
                        if (!utype.IsArray)
                        {
                            LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseMultiple<{t.Name}>.SetField(): Attempting to assign non-array to array field {field_real_name}");
                            throw new Exception();
                        }
                        Type utype_elem = utype.GetElementType();
                        if (fb_attr.ElementType != utype_elem)
                        {
                            LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseMultiple<{t.Name}>.SetField(): Type of {field_real_name} doesnt match the argument type");
                            throw new Exception();
                        }
                        Array arr = value as Array;
                        if (arr.Length != fb_attr.Length)
                        {
                            LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseMultiple<{t.Name}>.SetField(): Array length does not match array field {field_real_name}");
                            throw new Exception();
                        }
                        // copy
                        // https://stackoverflow.com/questions/30817924/obtain-non-explicit-field-offset
                        int field_offset = Marshal.ReadInt32(fi.FieldHandle.Value + (4 + IntPtr.Size)) & 0xFFFFFF;
                        unsafe
                        {
                            fixed (Category2030Item* ptr = items_span)
                            {
                                // https://stackoverflow.com/questions/63899222/how-to-get-a-pointer-to-memory-of-array-instance
                                U* ptr2 = (U*)(((byte*)(&ptr[real_index])) + field_offset);
                                GCHandle handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
                                try
                                {
                                    IntPtr address = handle.AddrOfPinnedObject();
                                    U* ptr3 = (U*)address.ToPointer();
                                    int arrsize = fb_attr.Length * Marshal.SizeOf(utype_elem);
                                    if (!Utility.MemoryEqual(ptr3, ptr2, (uint)arrsize))
                                    {
                                        Buffer.MemoryCopy(ptr3, ptr2, arrsize, arrsize);
                                        // undo/redo stuff

                                        result = true;
                                    }
                                }
                                finally
                                {
                                    handle.Free();
                                }
                            }
                        }
                    }
                    // otherwise, set single element of array
                    else
                    {
                        if (fb_attr.ElementType != typeof(U))
                        {
                            LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Type of {field_real_name} doesnt match the argument type");
                            throw new Exception();
                        }
                        if (field_index >= fb_attr.Length)
                        {
                            LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Array index out of range");
                            throw new Exception();
                        }

                        // https://stackoverflow.com/questions/30817924/obtain-non-explicit-field-offset
                        int field_offset = Marshal.ReadInt32(fi.FieldHandle.Value + (4 + IntPtr.Size)) & 0xFFFFFF;
                        unsafe
                        {
                            fixed (Category2030Item* ptr = items_span)
                            {
                                U* ptr2 = (U*)(((byte*)(&ptr[real_index])) + field_offset);
                                if (!ptr2[field_index].Equals(value))
                                {
                                    ptr2[field_index] = value;
                                    // undo/redo stuff

                                    result = true;
                                }
                            }
                        }
                    }
                }

                if (result)
                {
                    valid = true;
                    cat.OnSubElementModified?.Invoke(cat.GetCategoryID(), index, subindex);
                }
            }
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Modify subitem at index {index}, {subindex} (field: {field_name}, new value: {value}";
        }
    }

    public class UndoRedoVertexAdd2030 : IUndoRedo
    {
        public Category2030 cat;
        public int index;
        public int subindex;
        public int v_index;
        public short new_x;
        public short new_y;

        public bool Init()
        {
            if (index >= cat.Indices.Count)
            {
                throw new Exception();
            }
            if (subindex >= cat.GetItemSubItemNum(index))
            {
                throw new Exception();
            }
            if (v_index * 2 > cat[index, subindex].Coords.Count)
            {
                throw new Exception();
            }

            Redo();
            return true;
        }

        public void Undo()
        {
            int main_index = cat.Indices[index];
            main_index += subindex;

            cat.Items[main_index].Coords.RemoveAt(v_index * 2 + 0);
            cat.Items[main_index].Coords.RemoveAt(v_index * 2 + 0);
            cat.OnVertexRemoved?.Invoke(index, subindex, v_index);
        }

        public void Redo()
        {
            int main_index = cat.Indices[index];
            main_index += subindex;

            cat.Items[main_index].Coords.Insert(v_index * 2 + 0, new_x);
            cat.Items[main_index].Coords.Insert(v_index * 2 + 1, new_y);
            cat.OnVertexAdded?.Invoke(index, subindex, v_index);
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Add vertex at index {index}, {subindex}, {v_index} (x: {new_x}, y: {new_y})";
        }
    }

    public class UndoRedoVertexModify2030 : IUndoRedo
    {
        public Category2030 cat;
        public int index;
        public int subindex;
        public int v_index;
        public short new_x;
        public short new_y;
        short previous_x;
        short previous_y;

        public bool Init()
        {
            if (index >= cat.Indices.Count)
            {
                throw new Exception();
            }
            if (subindex >= cat.GetItemSubItemNum(index))
            {
                throw new Exception();
            }
            if (v_index * 2 >= cat[index, subindex].Coords.Count)
            {
                throw new Exception();
            }

            previous_x = cat[index, subindex].Coords[v_index * 2 + 0];
            previous_y = cat[index, subindex].Coords[v_index * 2 + 1];
            if((previous_x == new_x)&&(previous_y == new_y))
            {
                return false;
            }

            Redo();
            return true;
        }

        public void Undo()
        {
            int main_index = cat.Indices[index];
            main_index += subindex;

            cat.Items[main_index].Coords[v_index * 2 + 0] = previous_x;
            cat.Items[main_index].Coords[v_index * 2 + 1] = previous_y;
            cat.OnVertexModified?.Invoke(index, subindex, v_index);
        }

        public void Redo()
        {
            int main_index = cat.Indices[index];
            main_index += subindex;

            cat.Items[main_index].Coords[v_index * 2 + 0] = new_x;
            cat.Items[main_index].Coords[v_index * 2 + 1] = new_y;
            cat.OnVertexModified?.Invoke(index, subindex, v_index);
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Modify vertex at index {index}, {subindex}, {v_index} (new x: {new_x}, new y: {new_y})";
        }
    }

    public class UndoRedoVertexRemove2030 : IUndoRedo
    {
        public Category2030 cat;
        public int index;
        public int subindex;
        public int v_index;
        short previous_x;
        short previous_y;

        public bool Init()
        {
            if (index >= cat.Indices.Count)
            {
                throw new Exception();
            }
            if (subindex >= cat.GetItemSubItemNum(index))
            {
                throw new Exception();
            }
            if (v_index * 2 >= cat[index, subindex].Coords.Count)
            {
                throw new Exception();
            }

            previous_x = cat[index, subindex].Coords[v_index * 2 + 0];
            previous_y = cat[index, subindex].Coords[v_index * 2 + 1];

            Redo();
            return true;
        }

        public void Undo()
        {
            int main_index = cat.Indices[index];
            main_index += subindex;

            cat.Items[main_index].Coords.Insert(v_index * 2 + 0, previous_x);
            cat.Items[main_index].Coords.Insert(v_index * 2 + 1, previous_y);
            cat.OnVertexAdded?.Invoke(index, subindex, v_index);
        }

        public void Redo()
        {
            int main_index = cat.Indices[index];
            main_index += subindex;

            cat.Items[main_index].Coords.RemoveAt(v_index * 2 + 0);
            cat.Items[main_index].Coords.RemoveAt(v_index * 2 + 0);
            cat.OnVertexRemoved?.Invoke(index, subindex, v_index);
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Remove vertex at index {index}, {subindex}, {v_index}";
        }
    }
}
