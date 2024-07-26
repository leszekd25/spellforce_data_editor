using SFEngine.SFCFF.CTG;
using SFEngine.SFChunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SFEngine.SFCFF
{
    public abstract class CategoryBaseSingle<T> : ICategory where T : struct, ICategoryItem
    {
        int CurrentMaxID;
        bool Loaded = false;

        public virtual string GetName()
        {
            return "";
        }

        public virtual short GetCategoryID()
        {
            return 0;
        }

        public virtual short GetCategoryType()
        {
            return 0;
        }

        public int GetNumOfItems()
        {
            return Items.Count;
        }

        public int GetCurrentMaxID()
        {
            return CurrentMaxID;
        }

        public int GetByteCount()
        {
            int v;
            unsafe
            {
                v = sizeof(T);
            }
            return v * Items.Count;
        }

        public List<T> Items = new List<T>();

        public T this[int index]
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

        public bool GetCategoryChunk(SFChunkFile file, out SFChunkFileChunk chunk)
        {
            chunk = file.GetChunkByID(GetCategoryID());
            if (chunk == null)
            {
                return false;
            }
            if (chunk.header.ChunkDataType != GetCategoryType())
            {
                return false;
            }
            return true;
        }

        public bool Load(SFChunkFile file)
        {
            if(!file.GetChunkSpanByID(GetCategoryID(), out int type, out int start, out int length))
            {
                return false;
            }
            if(type != GetCategoryType())
            {
                return false;
            }
            int item_count;
            unsafe
            {
                item_count = length / sizeof(T);
            }
            CollectionsMarshal.SetCount(Items, item_count);
            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            Span<byte> items_span_raw = MemoryMarshal.Cast<T, byte>(items_span);
            file.stream.Position = start;
            file.stream.Read(items_span_raw);

            for (int i = 0; i < GetNumOfItems(); i++)
            {
                CurrentMaxID = Math.Max(CurrentMaxID, Items[i].GetID());
            }

            Loaded = true;
            return true;
        }

        public bool IsLoaded()
        {
            return Loaded;
        }

        public bool WriteRawData(ref byte[] data)
        {
            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            ReadOnlySpan<byte> items_span_raw = MemoryMarshal.Cast<T, byte>(items_span);
            data = new byte[items_span_raw.Length];
            items_span_raw.CopyTo(data);

            return true;
        }

        public bool Clear()
        {
            Items.Clear();
            CurrentMaxID = 0;
            Loaded = false;
            return true;
        }

        public bool Sort()
        {
            Items.Sort();
            return true;
        }

        public bool AddEmpty(int new_index)
        {
            Items.Insert(new_index, new());
            return true;
        }

        public bool AddID(int new_index, int new_id)
        {
            Items.Insert(new_index, new());
            SetID(new_index, new_id);
            return true;
        }

        public bool AddItem(int new_index, T item)
        {
            Items.Insert(new_index, item);
            return true;
        }

        public bool Copy(int from_index, int new_index)
        {
            Items.Insert(new_index, Items[from_index]);
            return true;
        }

        public bool Remove(int index)
        {
            Items.RemoveAt(index);
            return true;
        }

        public bool GetID(int index, out int id)
        {
            id = Items[index].GetID();
            return true;
        }

        public bool SetID(int index, int id)
        {
            Span<T> span = CollectionsMarshal.AsSpan(Items);
            span[index].SetID(id);
            return true;
        }

        public bool CalculateNewItemIndex(int new_id, out int index)
        {
            int current_start = 0;
            int current_end = Items.Count - 1;
            int current_center;
            int val;
            while (current_start <= current_end)
            {
                current_center = (current_start + current_end) / 2;    //care about overflow (though its not happening in this case)
                val = Items[current_center].GetID();
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
            str = Items[index].GetID().ToString();
            return true;
        }

        public bool GetItemDescription(int index, SFGameDataNew gd, out string desc)
        {
            desc = $"DESC {Items[index].GetID()}";
            return true;
        }

        public bool GetItemIndex(int id, out int index)
        {
            int current_start = 0;
            int current_end = Items.Count - 1;
            int current_center;
            while (current_start <= current_end)
            {
                current_center = (current_start + current_end) / 2;    //care about overflow
                int cur_id = Items[current_center].GetID();
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

        public void SetField<U>(int index, string field_name, U value)
        {
            Type t = typeof(T);

            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
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
                    TypedReference tref = __makeref(items_span[index]);
                    fi.SetValueDirect(tref, value);
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
                        fixed (T* ptr = items_span)
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
                        fixed (T* ptr = items_span)
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

        public bool GetFirstUnusedID(out int id, out int index)
        {
            if(Items.Count == 0)
            {
                id = 1;
                index = 0;
                return true;
            }
            int cur_id = 1;
            for(int i = 0; i < Items.Count; i++)
            {
                if(Items[i].GetID() == 0)
                {
                    continue;
                }
                if(cur_id != Items[i].GetID())
                {
                    id = cur_id;
                    index = i;
                    return true;
                }
                cur_id++;
            }
            id = Items[^1].GetID() + 1;
            index = Items.Count;
            return true;
        }

        public bool GetLastUsedID(out int id, out int index)
        {
            if (Items.Count == 0)
            {
                id = 0;
                index = 0;
                return true;
            }
            id = Items[^1].GetID();
            index = Items.Count;
            return true;
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

    public class CategoryBaseMultiple<T>: ICategory where T: struct, ICategorySubItem
    {
        int CurrentMaxID;
        bool Loaded;

        public virtual string GetName()
        {
            return "";
        }

        public virtual short GetCategoryID()
        {
            return 0;
        }

        public virtual short GetCategoryType()
        {
            return 0;
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
            int v;
            unsafe
            {
                v = sizeof(T);
            }
            return v * Items.Count;
        }

        public List<T> Items = new List<T>();
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

        public T this[int index]
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

        public T this[int index, int subindex]
        {
            get
            {
                return Items[GetSubItemIndex(index, subindex)];
            }
            set 
            {
                Items[GetSubItemIndex(index, subindex)] = value;
            }
        }

        void CalculateIndices()
        {
            int cur_item_id = -1;
            for(int i = 0; i < Items.Count; i++)
            {
                int item_id = Items[i].GetID();
                if(item_id != cur_item_id)
                {
                    cur_item_id = item_id;
                    Indices.Add(i);
                    CurrentMaxID = Math.Max(CurrentMaxID, item_id);
                }
            }
        }

        void AdjustIndices(int index_start, int increment)
        {
            for(int i = index_start; i < Indices.Count; i++)
            {
                Indices[i] += increment;
            }
        }

        public bool Load(SFChunkFile file)
        {
            if (!file.GetChunkSpanByID(GetCategoryID(), out int type, out int start, out int length))
            {
                return false;
            }
            if (type != GetCategoryType())
            {
                return false;
            }
            int item_count;
            unsafe
            {
                item_count = length / sizeof(T);
            }
            CollectionsMarshal.SetCount(Items, item_count);
            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            Span<byte> items_span_raw = MemoryMarshal.Cast<T, byte>(items_span);
            file.stream.Position = start;
            file.stream.Read(items_span_raw);

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
            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            ReadOnlySpan<byte> items_span_raw = MemoryMarshal.Cast<T, byte>(items_span);
            data = new byte[items_span_raw.Length];
            items_span_raw.CopyTo(data);

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
            if(new_index >= Indices.Count)
            {
                main_index = Items.Count;
            }
            else
            {
                main_index = Indices[new_index];
            }
            Items.Insert(main_index, new());
            Indices.Insert(new_index, main_index);
            AdjustIndices(new_index + 1, 1);
            return true;
        }

        public bool AddID(int new_index, int new_id)
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
            Items.Insert(main_index, new());
            Indices.Insert(new_index, main_index);
            AdjustIndices(new_index + 1, 1);
            SetID(new_index, new_id);
            return true;
        }

        public bool AddItem(int new_index, T item)
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

        public bool AddSubItem(int new_index, int new_subindex, T item)
        {
            if(new_index >= Indices.Count)
            {
                throw new Exception();
            }

            int main_index = Indices[new_index];
            int num = GetItemSubItemNum(new_index);
            if(new_subindex > num)
            {
                throw new Exception();
            }

            Items.Insert(main_index + new_subindex, item);
            AdjustIndices(new_index + 1, 1);
            return true;
        }

        public bool Copy(int from_index, int new_index)
        {
            int from_start = Indices[from_index];
            int from_end;
            if(from_index == Indices.Count-1)
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
                Items.Insert(main_index + i, Items[from_start + i]);
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
            AdjustIndices(index, from_start-from_end);

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
            if(num == 1)
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
            Span<T> span = CollectionsMarshal.AsSpan(Items);
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

            for(int i = from_start; i <= from_end; i++)
            {
                span[i].SetID(id);
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

        public int GetItemSubItemNum(int index)
        {
            if(index == Indices.Count-1)
            {
                return Items.Count - Indices[index];
            }
            return Indices[index + 1] - Indices[index];
        }

        public bool GetItemSubItemIndex(int id, int subid, out int index)
        {
            if(!GetItemIndex(id, out index))
            {
                return false;
            }
            index = Indices[index];

            for(;index < Items.Count; index++)
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
            Type t = typeof(T);

            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            if ((index < 0) || (index >= items_span.Length))
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

                U cur_val = (U)fi.GetValue(items_span[index]);
                if (!cur_val.Equals(value))
                {
                    TypedReference tref = __makeref(items_span[index]);
                    fi.SetValueDirect(tref, value);
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
                        fixed (T* ptr = items_span)
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
                        fixed (T* ptr = items_span)
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
