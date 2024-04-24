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

        public bool GetItemString(int index, SFGameData gd, out string str)
        {
            str = Items[index].GetID().ToString();
            return true;
        }

        public bool GetItemDescription(int index, SFGameData gd, out string desc)
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

        public bool SetField<U>(int index, string field_name, U value) where U: struct
        {
            Type t = typeof(T);

            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            if ((index < 0) || (index >= items_span.Length))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Index out of range");
                return false;
            }

            FieldInfo fi = t.GetField(field_name);
            if(fi == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Unknown field {field_name}");
                return false;
            }

            Type t2 = fi.FieldType;
            if(t2 != typeof(U))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Type of {field_name} doesnt match the argument type");
                return false;
            }

            U cur_val = (U)fi.GetValue(items_span[index]);
            if (!cur_val.Equals(value))
            {
                fi.SetValue(items_span[index], value);
                // undo/redo stuff
            }

            todo: merge below and this

            return true;
        }

        public bool SetArrayField<U>(int index, string field_name, int array_index, U value) where U: struct
        {
            Type t = typeof(T);

            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            if ((index < 0) || (index >= items_span.Length))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetArrayField(): Index out of range");
                return false;
            }

            FieldInfo fi = t.GetField(field_name);
            if (fi == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetArrayField(): Unknown field {field_name}");
                return false;
            }

            object[] attributes = fi.GetCustomAttributes(typeof(FixedBufferAttribute), false);
            if((attributes == null) || (attributes.Length == 0))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetArrayField(): {field_name} is not an array");
                return false;
            }
            FixedBufferAttribute fbattr = (FixedBufferAttribute)attributes[0];
            if (fbattr.ElementType != typeof(U))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetArrayField(): Type of {field_name} doesnt match the argument type");
                return false;
            }
            if ((array_index < 0) || (array_index >= fbattr.Length)) 
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetArrayField(): Array index out of range");
                return false;
            }

            // https://stackoverflow.com/questions/30817924/obtain-non-explicit-field-offset
            int field_offset = Marshal.ReadInt32(fi.FieldHandle.Value + (4 + IntPtr.Size)) & 0xFFFFFF;
            unsafe
            {
                fixed(T* ptr = items_span)
                {
                    U* ptr2 = (U*)(((byte*)(&ptr[index])) + field_offset);
                    if (!ptr2[array_index].Equals(value))
                    {
                        ptr2[array_index] = value;
                        // undo/redo stugg
                    }
                }
            }

            return true;
        }

        public object GetField(int index, string field_name)
        {
            Type t = typeof(T);

            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            if ((index < 0) || (index >= items_span.Length))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.GetField(): Index out of range");
                return null;
            }

            FieldInfo fi = t.GetField(field_name);
            if (fi == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.GetField(): Unknown field {field_name}");
                return null;
            }

            return fi.GetValue(items_span[index]);
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
            AdjustIndices(index, from_end - from_start);

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

        public bool GetItemString(int index, SFGameData gd, out string str)
        {
            str = Items[Indices[index]].GetID().ToString();
            return true;
        }

        public bool GetItemDescription(int index, SFGameData gd, out string desc)
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
            if(index == Indices.Count-1)
            {
                return Items.Count - Indices[index];
            }
            return Indices[index + 1] - Indices[index];
        }

        public bool GetItemSubIndex(int id, int subid, out int index)
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

        public bool SetField(int index, string field_name, object value)
        {
            Type t = typeof(T);

            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            if ((index < 0) || (index >= items_span.Length))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Index out of range");
                return false;
            }

            FieldInfo fi = t.GetField(field_name);
            if (fi == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Unknown field {field_name}");
                return false;
            }

            Type t2 = fi.FieldType;
            if (t2 != value.GetType())
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.SetField(): Type of {field_name} doesnt match the argument type");
                return false;
            }

            object cur_val = fi.GetValue(items_span[index]);
            if (!cur_val.Equals(value))
            {
                fi.SetValue(items_span[index], value);
                // undo/redo stuff
            }

            return true;
        }

        public object GetField(int index, string field_name)
        {
            Type t = typeof(T);

            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            if ((index < 0) || (index >= items_span.Length))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.GetField(): Index out of range");
                return null;
            }

            FieldInfo fi = t.GetField(field_name);
            if (fi == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, $"CategoryBaseSingle<{t.Name}>.GetField(): Unknown field {field_name}");
                return null;
            }

            return fi.GetValue(items_span[index]);
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
