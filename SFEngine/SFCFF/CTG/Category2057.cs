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
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2057Item : ICategorySubItem
    {
        public ushort ObjectID;
        public byte PolygonID;
        public byte CastsShadow;
        public List<short> Coords;

        public int GetByteCount() { return 5 + 2 * Coords.Count; }

        public int GetID() => ObjectID;
        public void SetID(int id) => ObjectID = (ushort)id;
        public int GetSubID() => PolygonID;
        public void SetSubID(int subid) => PolygonID = (byte)subid;
    }

    public class Category2057 : ICategory
    {
        int CurrentMaxID;
        bool Loaded;

        public virtual string GetName()
        {
            return "Interactive object collision data";
        }

        public virtual short GetCategoryID()
        {
            return 2057;
        }

        public virtual short GetCategoryType()
        {
            return 2;
        }

        public int GetNumOfItems()
        {
            return Indices.Count;
        }

        public int GetCurrentMaxID()
        {
            return CurrentMaxID;
        }

        public int GetByteCount()
        {
            int v = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                v += Items[i].GetByteCount();
            }
            return v;
        }

        public List<Category2057Item> Items = new List<Category2057Item>();
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

        public Category2057Item this[int index]
        {
            get
            {
                return Items[index];
            }
            set
            {
                Items[index] = value;
            }
        }

        public Category2057Item this[int index, int subindex]
        {
            get
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

                return Items[Indices[index] + subindex];
            }
            set
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

                Items[Indices[index] + subindex] = value;
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
                    CurrentMaxID = Math.Max(CurrentMaxID, item_id);
                }
            }
        }

        void AdjustIndices(int index_start, int increment)
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
                return false;
            }
            if (chunk.header.ChunkDataType != GetCategoryType())
            {
                return false;
            }
            BinaryReader br = chunk.Open();

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                Category2057Item item = new Category2057Item();
                item.ObjectID = br.ReadUInt16();
                item.PolygonID = br.ReadByte();
                item.CastsShadow = br.ReadByte();
                item.Coords = new List<short>();
                ushort coords_num = (ushort)(br.ReadByte());
                for (int i = 0; i < coords_num; i++)
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

        public bool WriteRawData(ref byte[] data)
        {
            data = new byte[GetByteCount()];
            using MemoryStream ms = new MemoryStream(data);
            using BinaryWriter bw = new BinaryWriter(ms);

            for (int i = 0; i < Items.Count; i++)
            {
                Category2057Item item = Items[i];
                bw.Write(item.ObjectID);
                bw.Write(item.PolygonID);
                bw.Write(item.CastsShadow);
                bw.Write((byte)(item.Coords.Count / 2));
                for (int j = 0; j < item.Coords.Count; j++)
                {
                    bw.Write(item.Coords[i]);
                }
            }

            return true;
        }

        public bool Clear()
        {
            Items.Clear();
            Indices.Clear();
            CurrentMaxID = 0;
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
            int main_index;
            if (new_index >= Indices.Count)
            {
                main_index = Items.Count;
            }
            else
            {
                main_index = Indices[new_index];
            }
            Items.Insert(main_index, new() { Coords = new() });
            Indices.Insert(new_index, main_index);
            AdjustIndices(new_index + 1, 1);
            return true;
        }

        public bool AddItem(int new_index, Category2057Item item)
        {
            int main_index;
            if (new_index >= Indices.Count)
            {
                main_index = Items.Count;
            }
            else
            {
                main_index = Indices[new_index];
            }
            Items.Insert(new_index, item);
            Indices.Insert(new_index, main_index);
            AdjustIndices(new_index + 1, 1);
            return true;
        }

        public bool AddSubItem(int new_index, int new_subindex, Category2057Item item)
        {
            if (new_index >= Indices.Count)
            {
                throw new Exception();
            }

            int main_index = Indices[new_index];
            int num = GetItemSubItemNum(new_index);
            if (new_subindex > num)
            {
                throw new Exception();
            }

            Items.Insert(main_index + new_subindex, item);
            AdjustIndices(new_index + 1, 1);
            return true;
        }

        public bool AddCoord(int index, int subindex, int new_coord_index, short x, short y)
        {
            if (index >= Indices.Count)
            {
                throw new Exception();
            }

            int main_index = Indices[index];
            int num = GetItemSubItemNum(index);
            if (subindex > num)
            {
                throw new Exception();
            }

            main_index += subindex;

            if (new_coord_index * 2 > Items[main_index].Coords.Count)
            {
                throw new Exception();
            }

            Items[main_index].Coords.Insert(new_coord_index * 2 + 0, x);
            Items[main_index].Coords.Insert(new_coord_index * 2 + 1, y);

            return true;
        }

        public bool SetCoord(int index, int subindex, int coord_index, short x, short y)
        {
            if (index >= Indices.Count)
            {
                throw new Exception();
            }

            int main_index = Indices[index];
            int num = GetItemSubItemNum(index);
            if (subindex > num)
            {
                throw new Exception();
            }

            main_index += subindex;

            if (coord_index * 2 >= Items[main_index].Coords.Count)
            {
                throw new Exception();
            }

            Items[main_index].Coords[coord_index * 2 + 0] = x;
            Items[main_index].Coords[coord_index * 2 + 1] = y;

            return true;
        }

        public bool RemoveCoord(int index, int subindex, int coord_index)
        {
            if (index >= Indices.Count)
            {
                throw new Exception();
            }

            int main_index = Indices[index];
            int num = GetItemSubItemNum(index);
            if (subindex > num)
            {
                throw new Exception();
            }

            main_index += subindex;

            if (coord_index * 2 >= Items[main_index].Coords.Count)
            {
                throw new Exception();
            }

            Items[main_index].Coords.RemoveAt(coord_index * 2 + 0);
            Items[main_index].Coords.RemoveAt(coord_index * 2 + 0);

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

            int main_index;
            if (new_index >= Indices.Count)
            {
                main_index = Items.Count;
            }
            else
            {
                main_index = Indices[new_index];
            }

            for (int i = 0; i <= (from_end - from_start); i++)
            {
                Category2057Item item = Items[from_start + i];
                item.Coords = new List<short>(item.Coords);
                Items.Insert(main_index + i, item);
            }
            Indices.Insert(new_index, main_index);
            AdjustIndices(new_index + 1, from_end - from_start);

            return true;
        }

        public bool Remove(int index)
        {
            int from_start = Indices[index];
            int from_end;
            if (index == Indices.Count - 1)
            {
                from_end = Items.Count - 1;
            }
            else
            {
                from_end = Indices[index + 1] - 1;
            }

            for (int i = 0; i <= (from_end - from_start); i++)
            {
                Items.RemoveAt(from_start);
            }
            Indices.RemoveAt(index);
            AdjustIndices(index, from_start - from_end);

            return true;
        }

        public bool RemoveSub(int index, int subindex)
        {
            if (index >= Indices.Count)
            {
                throw new Exception();
            }

            int main_index = Indices[index];
            int num = GetItemSubItemNum(index);
            if (subindex > num)
            {
                throw new Exception();
            }
            if (num == 1)
            {
                return false;
            }

            Items.RemoveAt(main_index + subindex);
            AdjustIndices(index + 1, -1);
            return true;
        }

        public bool GetID(int index, out int id)
        {
            id = Items[Indices[index]].GetID();
            return true;
        }

        public bool SetID(int index, int id)
        {
            int from_start = Indices[index];
            int from_end;
            if (index == Indices.Count - 1)
            {
                from_end = Items.Count - 1;
            }
            else
            {
                from_end = Indices[index + 1] - 1;
            }

            for (int i = from_start; i <= from_end; i++)
            {
                Category2057Item item = Items[i];
                item.SetID(id);
                Items[i] = item;
            }

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

        public void SetField<U>(int index, string field_name, U value)
        {
            Type t = typeof(Category2057Item);

            Span<Category2057Item> items_span = CollectionsMarshal.AsSpan(Items);
            if ((index < 0) || (index >= items_span.Length))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Index out of range");
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
                            LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Malformed array index");
                            throw new Exception();
                        }
                        field_real_name = new(field_span.Slice(0, i));
                        break;
                    }
                }
                if (field_index == uint.MaxValue)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Malformed array index");
                    throw new Exception();
                }
            }

            FieldInfo fi = t.GetField(field_real_name);
            if (fi == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Unknown field {field_real_name}");
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
                    LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Array index used in non-buffer field {field_real_name}");
                    throw new Exception();
                }
                Type t2 = fi.FieldType;
                if (t2 != typeof(U))
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Type of {field_real_name} doesnt match the argument type");
                    throw new Exception();
                }

                U cur_val = (U)fi.GetValue(items_span[index]);
                if (!cur_val.Equals(value))
                {
                    fi.SetValue(items_span[index], value);
                    // undo/redo stuff
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
                        LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Attempting to assign non-array to array field {field_real_name}");
                        throw new Exception();
                    }
                    Type utype_elem = utype.GetElementType();
                    if (fb_attr.ElementType != utype_elem)
                    {
                        LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Type of {field_real_name} doesnt match the argument type");
                        throw new Exception();
                    }
                    Array arr = value as Array;
                    if (arr.Length != fb_attr.Length)
                    {
                        LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Array length does not match array field {field_real_name}");
                        throw new Exception();
                    }
                    // copy
                    // https://stackoverflow.com/questions/30817924/obtain-non-explicit-field-offset
                    int field_offset = Marshal.ReadInt32(fi.FieldHandle.Value + (4 + IntPtr.Size)) & 0xFFFFFF;
                    unsafe
                    {
                        fixed (Category2057Item* ptr = items_span)
                        {
                            // https://stackoverflow.com/questions/63899222/how-to-get-a-pointer-to-memory-of-array-instance
                            U* ptr2 = (U*)(((byte*)(&ptr[index])) + field_offset);
                            GCHandle handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
                            try
                            {
                                IntPtr address = handle.AddrOfPinnedObject();
                                U* ptr3 = (U*)address.ToPointer();
                                for (int i = 0; i < fb_attr.Length; i++)
                                {
                                    ptr2[i] = ptr3[i];
                                }
                                // undo/redo stuff
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
                        fixed (Category2057Item* ptr = items_span)
                        {
                            U* ptr2 = (U*)(((byte*)(&ptr[index])) + field_offset);
                            if (!ptr2[field_index].Equals(value))
                            {
                                ptr2[field_index] = value;
                                // undo/redo stuff
                            }
                        }
                    }
                }
            }
        }

        public void SetField<U>(int index, int subindex, string field_name, U value)
        {
            SetField(GetSubItemIndex(index, subindex), field_name, value);
        }

        public bool Undo()
        {
            return false;
        }

        public bool Redo()
        {
            return false;
        }

        public bool CanUndo()
        {
            return false;
        }

        public bool CanRedo()
        {
            return false;
        }

        public List<int> QueryItems()
        {
            return null;
        }
    }
}
