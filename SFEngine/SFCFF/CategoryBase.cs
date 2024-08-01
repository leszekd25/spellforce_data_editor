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
using System.Diagnostics.Metrics;
using Windows.UI.Composition.Interactions;
using SFEngine.SFLua.LuaDecompiler;
using OpenTK.Windowing.Common.Input;
using System.Reflection.Metadata;
using System.Media;

namespace SFEngine.SFCFF
{
    public abstract class CategoryBaseSingle<T> : ICategory where T : struct, ICategoryItem
    {
        bool Loaded = false;
        UndoRedoQueue urq = null;

        public dOnElementAdded OnElementAdded;
        public dOnElementModified OnElementModified;
        public dOnElementRemoved OnElementRemoved;

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
                return true;
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

            Loaded = true;
            return true;
        }

        public bool IsLoaded()
        {
            return Loaded;
        }

        public bool Save(SFChunkFile sfcf)
        {
            if(Items.Count == 0)
            {
                return true;
            }

            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            ReadOnlySpan<byte> items_span_raw = MemoryMarshal.Cast<T, byte>(items_span);
            sfcf.AddChunk(GetCategoryID(), 0, false, GetCategoryType(), items_span_raw);

            return true;
        }

        public bool MergeFrom(ICategory c1, ICategory c2)
        {
            if((!(c1 is CategoryBaseSingle<T>))||(!(c2 is CategoryBaseSingle<T>)))
            {
                return false;
            }
            CategoryBaseSingle<T> cat1 = c1 as CategoryBaseSingle<T>;
            CategoryBaseSingle<T> cat2 = c2 as CategoryBaseSingle<T>;

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
                    Items.Add(cat2[new_i]);
                }
                else if (new_end)
                {
                    Items.Add(cat1[orig_i]);
                }
                else
                {
                    if (orig_id == new_id)
                    {
                        Items.Add(cat2[new_i]);
                    }
                    else if (orig_id > new_id)
                    {
                        Items.Add(cat2[new_i]);
                        // addition!

                        orig_i -= 1;
                    }
                    else if (orig_id < new_id)
                    {
                        Items.Add(cat1[orig_i]);

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

            Loaded = true;
            return true;
        }

        public bool DiffFrom(ICategory c1, ICategory c2)
        {
            if ((!(c1 is CategoryBaseSingle<T>)) || (!(c2 is CategoryBaseSingle<T>)))
            {
                return false;
            }
            CategoryBaseSingle<T> cat1 = c1 as CategoryBaseSingle<T>;
            CategoryBaseSingle<T> cat2 = c2 as CategoryBaseSingle<T>;

            // climb double list ladder
            // elems have same IDs: only add elem2 if the elems themselves arent equal
            // elem1 has lower ID than elem2: dont add any elem
            // elem1 has higher ID than elem2: add elem2

            // double list ladder

            Span<T> items1_span = CollectionsMarshal.AsSpan(cat1.Items);
            Span<T> items2_span = CollectionsMarshal.AsSpan(cat2.Items);
            System.Diagnostics.Debug.WriteLine($"CAT {cat1.GetNumOfItems()} {cat2.GetNumOfItems()}");
            int orig_i = 0;
            int new_i = 0;
            int orig_id = 0;
            int new_id = 0;
            bool orig_end = false;
            bool new_end = false;

            unsafe
            {
                fixed (T* ptr1 = items1_span)
                {
                    fixed (T* ptr2 = items2_span)
                    {
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
                                Items.Add(cat2[new_i]);
                            }
                            else if (new_end)
                            {

                            }
                            else
                            {
                                if (orig_id == new_id)
                                {
                                    if (!Utility.MemoryEqual(&ptr1[orig_i], &ptr2[new_i], (uint)(sizeof(T))))
                                    {
                                        Items.Add(cat2[new_i]);
                                    }
                                }
                                else if (orig_id > new_id)
                                {
                                    Items.Add(cat2[new_i]);
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
                    }
                }
            }

            Loaded = true;
            return true;
        }

        public bool Clear()
        {
            Items.Clear();
            urq = null;
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
            IUndoRedo iur = new UndoRedoElementAddSingle<T>()
            {
                cat = this,
                index = new_index,
                item = new()
            };
            iur.Commit(urq);

            return true;
        }

        public bool AddID(int new_index, int new_id)
        {
            T item = new();
            item.SetID(new_id);

            IUndoRedo iur = new UndoRedoElementAddSingle<T>()
            {
                cat = this,
                index = new_index,
                item = item
            };
            iur.Commit(urq);

            return true;
        }

        public bool AddItem(int new_index, T item)
        {
            IUndoRedo iur = new UndoRedoElementAddSingle<T>()
            {
                cat = this,
                index = new_index,
                item = item
            };
            iur.Commit(urq);

            return true;
        }

        public bool SetItem(int index, T item)
        {
            IUndoRedo iur = new UndoRedoElementReplaceSingle<T>()
            {
                cat = this,
                index = index,
                new_item = item,
            };
            iur.Commit(urq);

            return true;
        }

        public bool Copy(int from_index, int new_index)
        {
            IUndoRedo iur = new UndoRedoElementAddSingle<T>()
            {
                cat = this,
                index = new_index,
                item = Items[from_index]
            };
            iur.Commit(urq);

            return true;
        }

        public bool Remove(int index)
        {
            IUndoRedo iur = new UndoRedoElementRemoveSingle<T>()
            {
                cat = this,
                index = index,
            };
            iur.Commit(urq);

            return true;
        }

        public bool GetID(int index, out int id)
        {
            id = Items[index].GetID();
            return true;
        }

        public bool SetID(int index, int id)
        {
            IUndoRedo iur = new UndoRedoElementSetIDSingle<T>()
            {
                cat = this,
                new_id = id
            };
            iur.Commit(urq);

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
            IUndoRedo iur = new UndoRedoElementSetFieldSingle<T, U>()
            {
                cat = this,
                index = index,
                field_name = field_name,
                value = value
            };
            iur.Commit(urq);
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
            OnElementModified = cb;
            return true;
        }

        public bool SetOnElementRemovedCallback(dOnElementRemoved cb)
        {
            OnElementRemoved = cb;
            return true;
        }

        public bool SetOnSubElementAddedCallback(dOnSubElementAdded cb)
        {
            return false;
        }

        public bool SetOnSubElementModifiedCallback(dOnSubElementModified cb)
        {
            return false;
        }

        public bool SetOnSubElementRemovedCallback(dOnSubElementRemoved cb)
        {
            return false;
        }

        public bool ClearCallbacks()
        {
            OnElementAdded = null;
            OnElementModified = null;
            OnElementRemoved = null;
            return true;
        }

        public virtual List<string> GetSearchableFields()
        {
            return Utility.GetFields<T>();
        }

        public List<int> QueryItems(object value, string field_name, SearchOption option)
        {
            if(field_name == "")  // search all fields
            {
                HashSet<int> result = new();
                List<int> intermediate = new();
                foreach(var f in GetSearchableFields())
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
                Utility.GetFieldData<T>(field_name, out uint field_offset, out Type ft, out uint field_num);
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
                else if((option & SearchOption.IS_STRING) != SearchOption.NONE)
                {
                    if (field_num == 1)
                    {
                        return;
                    }
                    if(ft != typeof(byte))
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
            catch(Exception ex)
            {
                return;
            }
        }

        void QueryItemsFieldNumber<U>(int num, uint field_offset, bool as_flag, List<int> result)
        {
            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            int cur_value = 0;
            unsafe
            {
                fixed (T* ptr = items_span)
                {
                    if (as_flag)
                    {
                        for (int i = 0; i < items_span.Length; i++)
                        {
                            U* ptr2 = (U*)(((byte*)(&ptr[i])) + field_offset);
                            cur_value = Convert.ToInt32(*ptr2);

                            if (cur_value == num)
                            {
                                result.Add(i);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < items_span.Length; i++)
                        {
                            U* ptr2 = (U*)(((byte*)(&ptr[i])) + field_offset);
                            cur_value = Convert.ToInt32(*ptr2);

                            if (cur_value == num)
                            {
                                result.Add(i);
                            }
                        }
                    }
                }
            }
        }

        void QueryItemsFieldString(string str, uint field_offset, uint field_size, bool ignore_case, List<int> result)
        {
            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            Encoding encoding = Encoding.GetEncoding(1252);
            unsafe
            {
                fixed (T* ptr = items_span)
                {
                    if (ignore_case)
                    {
                        str = str.ToLowerInvariant();
                        for(int i = 0; i < items_span.Length; i++)
                        {
                            byte* ptr2 = (((byte*)(&ptr[i])) + field_offset);

                            string str2 = encoding.GetString(ptr2, (int)field_size).ToLowerInvariant();
                            if(str2.Contains(str))
                            {
                                result.Add(i);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < items_span.Length; i++)
                        {
                            byte* ptr2 = (((byte*)(&ptr[i])) + field_offset);

                            string str2 = encoding.GetString(ptr2, (int)field_size);
                            if (str2.Contains(str))
                            {
                                result.Add(i);
                            }
                        }
                    }
                }
            }
        }
    }

    public class UndoRedoElementAddSingle<T> : IUndoRedo where T : struct, ICategoryItem
    {
        public CategoryBaseSingle<T> cat;
        public int index;
        public T item;

        public bool Init()
        {
            Redo();
            return true;
        }

        public void Undo()
        {
            cat.Items.RemoveAt(index);
            cat.OnElementRemoved?.Invoke(cat.GetCategoryID(), index);
        }

        public void Redo()
        {
            cat.Items.Insert(index, item);
            cat.OnElementAdded?.Invoke(cat.GetCategoryID(), index);
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Add item at index {index}";
        }
    }

    public class UndoRedoElementRemoveSingle<T> : IUndoRedo where T : struct, ICategoryItem
    {
        public CategoryBaseSingle<T> cat;
        public int index;
        T previous_item;

        public bool Init()
        {
            previous_item = cat.Items[index];
            Redo();
            return true;
        }

        public void Undo()
        {
            cat.Items.Insert(index, previous_item);
            cat.OnElementAdded?.Invoke(cat.GetCategoryID(), index);
        }

        public void Redo()
        {
            cat.Items.RemoveAt(index);
            cat.OnElementRemoved?.Invoke(cat.GetCategoryID(), index);
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Remove item at index {index}";
        }
    }

    public class UndoRedoElementReplaceSingle<T>: IUndoRedo where T: struct, ICategoryItem
    {
        public CategoryBaseSingle<T> cat;
        public int index;
        public T new_item;
        T previous_item;

        public bool Init()
        {
            previous_item = cat[index];
            Redo();
            return true;
        }

        public void Undo()
        {
            cat.Items[index] = previous_item;
            cat.OnElementModified?.Invoke(cat.GetCategoryID(), index);
        }

        public void Redo()
        {
            cat.Items[index] = new_item;
            cat.OnElementModified?.Invoke(cat.GetCategoryID(), index);
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Replace item at index {index}";
        }
    }

    public class UndoRedoElementSetFieldSingle<T, U> : IUndoRedo where T : struct, ICategoryItem
    {
        public CategoryBaseSingle<T> cat;
        public int index;
        public string field_name;
        public U value;
        T new_item;
        T previous_item;
        bool initialized = false;
        bool valid = false;

        public bool Init()
        {
            previous_item = cat[index];
            Redo();
            if(!valid)
            {
                return false;
            }
            new_item = cat[index];
            initialized = true;
            return true;
        }

        public void Undo()
        {
            cat.Items[index] = previous_item;
            cat.OnElementModified?.Invoke(cat.GetCategoryID(), index);
        }

        public void Redo()
        {
            if (initialized)
            {
                cat.Items[index] = new_item;
                cat.OnElementModified?.Invoke(cat.GetCategoryID(), index);
            }
            else
            {
                // index, member, value
                Type t = typeof(T);

                Span<T> items_span = CollectionsMarshal.AsSpan(cat.Items);
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
                        valid = true;
                        cat.OnElementModified?.Invoke(cat.GetCategoryID(), index);
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
                                    int arrsize = fb_attr.Length * Marshal.SizeOf(utype_elem);
                                    if (Utility.MemoryEqual(ptr3, ptr2, (uint)arrsize))
                                    {
                                        Buffer.MemoryCopy(ptr3, ptr2, arrsize, arrsize);
                                        // undo/redo stuff
                                        valid = true;
                                        cat.OnElementModified?.Invoke(cat.GetCategoryID(), index);
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
                            fixed (T* ptr = items_span)
                            {
                                U* ptr2 = (U*)(((byte*)(&ptr[index])) + field_offset);
                                if (!ptr2[field_index].Equals(value))
                                {
                                    ptr2[field_index] = value;
                                    // undo/redo stuff
                                    valid = true;
                                    cat.OnElementModified?.Invoke(cat.GetCategoryID(), index);
                                }
                            }
                        }
                    }
                }
            }
        }


        public override string ToString()
        {
            return $"{cat.GetName()}: Modify item at index {index} (field: {field_name}, new value: {value})";
        }
    }

    public class UndoRedoElementSetIDSingle<T> : IUndoRedo where T : struct, ICategoryItem
    {
        public CategoryBaseSingle<T> cat;
        public int index;
        public int new_id;
        int previous_id;

        public bool Init()
        {
            previous_id = cat[index].GetID();
            if(new_id == previous_id)
            {
                return false;
            }
            Redo();
            return true;
        }

        public void Undo()
        {
            Span<T> span = CollectionsMarshal.AsSpan(cat.Items);
            span[index].SetID(previous_id);
            cat.OnElementModified?.Invoke(cat.GetCategoryID(), index);
        }

        public void Redo()
        {
            Span<T> span = CollectionsMarshal.AsSpan(cat.Items);
            span[index].SetID(new_id);
            cat.OnElementModified?.Invoke(cat.GetCategoryID(), index);
        }

        public override string ToString()
        {
            return $"{cat.GetName()}: Set ID of item at index {index} (new ID: {new_id})";
        }
    }



    public class CategoryBaseMultiple<T>: ICategory where T: struct, ICategorySubItem
    {
        bool Loaded;
        UndoRedoQueue urq;

        public dOnElementAdded OnElementAdded;
        public dOnElementRemoved OnElementRemoved;
        public dOnSubElementAdded OnSubElementAdded;
        public dOnSubElementModified OnSubElementModified;
        public dOnSubElementRemoved OnSubElementRemoved;

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
        }

        public T this[int index, int subindex]
        {
            get
            {
                return Items[GetSubItemIndex(index, subindex)];
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
                }
            }
        }

        public void AdjustIndices(int index_start, int increment)
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
                return true;
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

        public bool Save(SFChunkFile sfcf)
        {
            if (Items.Count == 0)
            {
                return true;
            }

            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            ReadOnlySpan<byte> items_span_raw = MemoryMarshal.Cast<T, byte>(items_span);
            sfcf.AddChunk(GetCategoryID(), 0, false, GetCategoryType(), items_span_raw);

            return true;
        }

        public bool MergeFrom(ICategory c1, ICategory c2)
        {
            if ((!(c1 is CategoryBaseMultiple<T>)) || (!(c2 is CategoryBaseMultiple<T>)))
            {
                return false;
            }
            CategoryBaseMultiple<T> cat1 = c1 as CategoryBaseMultiple<T>;
            CategoryBaseMultiple<T> cat2 = c2 as CategoryBaseMultiple<T>;

            // double list ladder
            Span<T> items1_span = CollectionsMarshal.AsSpan(cat1.Items);
            Span<T> items2_span = CollectionsMarshal.AsSpan(cat2.Items);
            int orig_i = 0;
            int new_i = 0;
            int orig_id = 0;
            int new_id = 0;
            bool orig_end = false;
            bool new_end = false;

            unsafe
            {
                fixed (T* ptr1 = items1_span)
                {
                    fixed (T* ptr2 = items2_span)
                    {
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
                                    Items.Add(cat2[main_index + j]);
                                }
                            }
                            else if (new_end)
                            {
                                int main_index = cat1.Indices[orig_i];
                                for (int j = 0; j < cat1.GetItemSubItemNum(orig_i); j++)
                                {
                                    Items.Add(cat1[main_index + j]);
                                }
                            }
                            else
                            {
                                if (orig_id == new_id)
                                {
                                    if (!GetSubitemDiffBehavior())
                                    {
                                        int main_index = cat2.Indices[new_i];
                                        for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                                        {
                                            Items.Add(cat2[main_index + j]);
                                        }
                                    }
                                    else
                                    {
                                        int main_index1 = cat1.Indices[orig_i];
                                        int main_index2 = cat2.Indices[new_i];

                                        int max_id = -1;
                                        Dictionary<int, int> subelem1 = new();
                                        Dictionary<int, int> subelem2 = new();

                                        for (int j = 0; j < cat1.GetItemSubItemNum(orig_i); j++)
                                        {
                                            int subid;

                                            subid = cat1.Items[main_index1 + j].GetSubID();
                                            if (!subelem1.ContainsKey(subid))
                                            {
                                                subelem1.Add(subid, j);
                                            }
                                            else
                                            {
                                                subelem1[subid] = j;
                                            }
                                            max_id = Math.Max(max_id, subid);
                                        }
                                        for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                                        {
                                            int subid;

                                            subid = cat2.Items[main_index2 + j].GetSubID();
                                            if (!subelem2.ContainsKey(subid))
                                            {
                                                subelem2.Add(subid, j);
                                            }
                                            else
                                            {
                                                subelem2[subid] = j;
                                            }
                                            max_id = Math.Max(max_id, subid);
                                        }

                                        for (int j = 0; j <= max_id; j++)
                                        {
                                            if (subelem1.ContainsKey(j))
                                            {
                                                if (subelem2.ContainsKey(j))
                                                {
                                                    if (!Utility.MemoryEqual(&ptr1[main_index1 + subelem1[j]], &ptr2[main_index2 + subelem2[j]], (uint)(sizeof(T))))
                                                    {
                                                        Items.Add(cat2[main_index2 + subelem2[j]]);
                                                    }
                                                }
                                                else
                                                {
                                                    Items.Add(cat1[main_index1 + subelem1[j]]);
                                                }
                                            }
                                            else if (subelem2.ContainsKey(j))
                                            {
                                                Items.Add(cat2[main_index2 + subelem2[j]]);
                                            }
                                        }
                                    }
                                }
                                else if (orig_id > new_id)
                                {
                                    int main_index = cat2.Indices[new_i];
                                    for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                                    {
                                        Items.Add(cat2[main_index + j]);
                                    }
                                    // addition!

                                    orig_i -= 1;
                                }
                                else if (orig_id < new_id)
                                {
                                    int main_index = cat1.Indices[orig_i];
                                    for (int j = 0; j < cat1.GetItemSubItemNum(orig_i); j++)
                                    {
                                        Items.Add(cat1[main_index + j]);
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
                    }
                }
            }

            CalculateIndices();
            Loaded = true;
            return true;
        }

        public bool DiffFrom(ICategory c1, ICategory c2)
        {
            if ((!(c1 is CategoryBaseMultiple<T>)) || (!(c2 is CategoryBaseMultiple<T>)))
            {
                return false;
            }
            CategoryBaseMultiple<T> cat1 = c1 as CategoryBaseMultiple<T>;
            CategoryBaseMultiple<T> cat2 = c2 as CategoryBaseMultiple<T>;

            // climb double list ladder
            // elems have same IDs: only add elem2 if the elems themselves arent equal
            // elem1 has lower ID than elem2: dont add any elem
            // elem1 has higher ID than elem2: add elem2

            // double list ladder

            Span<T> items1_span = CollectionsMarshal.AsSpan(cat1.Items);
            Span<T> items2_span = CollectionsMarshal.AsSpan(cat2.Items);
            int orig_i = 0;
            int new_i = 0;
            int orig_id = 0;
            int new_id = 0;
            bool orig_end = false;
            bool new_end = false;

            unsafe
            {
                fixed (T* ptr1 = items1_span)
                {
                    fixed (T* ptr2 = items2_span)
                    {
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
                                    Items.Add(cat2[main_index + j]);
                                }
                            }
                            else if (new_end)
                            {

                            }
                            else
                            {
                                if (orig_id == new_id)
                                {
                                    if (!GetSubitemDiffBehavior())
                                    {
                                        if (cat1.GetItemSubItemNum(orig_i) != cat2.GetItemSubItemNum(new_i))
                                        {
                                            int main_index = cat2.Indices[new_i];
                                            for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                                            {
                                                Items.Add(cat2[main_index + j]);
                                            }
                                        }
                                        else
                                        {
                                            int main_index1 = cat1.Indices[orig_i];
                                            int main_index2 = cat2.Indices[new_i];

                                            int max_id = -1;
                                            Dictionary<int, int> subelem1 = new();
                                            Dictionary<int, int> subelem2 = new();
                                            bool equal = true;

                                            for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                                            {
                                                int subid;

                                                subid = cat1.Items[main_index1 + j].GetSubID();
                                                if (!subelem1.ContainsKey(subid))
                                                {
                                                    subelem1.Add(subid, j);
                                                }
                                                else
                                                {
                                                    subelem1[subid] = j;
                                                }
                                                max_id = Math.Max(max_id, subid);

                                                subid = cat2.Items[main_index2 + j].GetSubID();
                                                if (!subelem2.ContainsKey(subid))
                                                {
                                                    subelem2.Add(subid, j);
                                                }
                                                else
                                                {
                                                    subelem2[subid] = j;
                                                }
                                                max_id = Math.Max(max_id, subid);
                                            }

                                            for (int j = 0; j <= max_id; j++)
                                            {
                                                if (subelem1.ContainsKey(j))
                                                {
                                                    if (subelem2.ContainsKey(j))
                                                    {
                                                        if (!Utility.MemoryEqual(&ptr1[main_index1 + subelem1[j]], &ptr2[main_index2 + subelem2[j]], (uint)(sizeof(T))))
                                                        {
                                                            equal = false;
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        equal = false;
                                                        break;
                                                    }
                                                }
                                                else if (subelem2.ContainsKey(j))
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
                                                    Items.Add(cat2[main_index + j]);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int main_index1 = cat1.Indices[orig_i];
                                        int main_index2 = cat2.Indices[new_i];

                                        int max_id = -1;
                                        Dictionary<int, int> subelem1 = new();
                                        Dictionary<int, int> subelem2 = new();

                                        for (int j = 0; j < cat1.GetItemSubItemNum(orig_i); j++)
                                        {
                                            int subid;

                                            subid = cat1.Items[main_index1 + j].GetSubID();
                                            if (!subelem1.ContainsKey(subid))
                                            {
                                                subelem1.Add(subid, j);
                                            }
                                            else
                                            {
                                                subelem1[subid] = j;
                                            }
                                            max_id = Math.Max(max_id, subid);
                                        }
                                        for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                                        {
                                            int subid;

                                            subid = cat2.Items[main_index2 + j].GetSubID();
                                            if (!subelem2.ContainsKey(subid))
                                            {
                                                subelem2.Add(subid, j);
                                            }
                                            else
                                            {
                                                subelem2[subid] = j;
                                            }
                                            max_id = Math.Max(max_id, subid);
                                        }

                                        for (int j = 0; j <= max_id; j++)
                                        {
                                            if (subelem1.ContainsKey(j))
                                            {
                                                if (subelem2.ContainsKey(j))
                                                {
                                                    if (!Utility.MemoryEqual(&ptr1[main_index1 + subelem1[j]], &ptr2[main_index2 + subelem2[j]], (uint)(sizeof(T))))
                                                    {
                                                        Items.Add(cat2[main_index2 + subelem2[j]]);
                                                    }
                                                }
                                            }
                                            else if (subelem2.ContainsKey(j))
                                            {
                                                Items.Add(cat2[main_index2 + subelem2[j]]);
                                            }
                                        }
                                    }
                                }
                                else if (orig_id > new_id)
                                {
                                    int main_index = cat2.Indices[new_i];
                                    for (int j = 0; j < cat2.GetItemSubItemNum(new_i); j++)
                                    {
                                        Items.Add(cat2[main_index + j]);
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
                    }
                }
            }

            CalculateIndices();
            Loaded = true;
            return true;
        }

        // false - treat subitems as parts of whole, true - treat subitems as separate elements
        public virtual bool GetSubitemDiffBehavior() 
        { 
            return false;
        }  

        public bool Clear()
        {
            Items.Clear();
            Indices.Clear();
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
            IUndoRedo iur = new UndoRedoElementAddMultiple<T>()
            {
                cat = this,
                index = new_index,
                item = new()
            };
            iur.Commit(urq);

            return true;
        }

        public bool AddID(int new_index, int new_id)
        {
            T item = new();
            item.SetID(new_id);

            IUndoRedo iur = new UndoRedoElementAddMultiple<T>()
            {
                cat = this,
                index = new_index,
                item = item
            };
            iur.Commit(urq);

            return true;
        }

        public bool AddItem(int new_index, T item)
        {
            IUndoRedo iur = new UndoRedoElementAddMultiple<T>()
            {
                cat = this,
                index = new_index,
                item = item
            };
            iur.Commit(urq);

            return true;
        }


        public bool AddSubItem(int new_index, int new_subindex, T item)
        {
            IUndoRedo iur = new UndoRedoSubElementAddMultiple<T>()
            {
                cat = this,
                index = new_index,
                subindex = new_subindex,
                item = item
            };
            iur.Commit(urq);

            return true;
        }

        public bool SetSubItem(int index, int subindex, T item)
        {
            IUndoRedo iur = new UndoRedoSubElementReplaceMultiple<T>()
            {
                cat = this,
                index = index,
                subindex = subindex,
                new_item = item
            };
            iur.Commit(urq);

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

            urq?.OpenCluster();
            IUndoRedo iur = new UndoRedoElementAddMultiple<T>()
            {
                cat = this,
                index = new_index,
                item = Items[from_start]
            };
            iur.Commit(urq);
            for(int i = 1; i <= (from_end - from_start); i++)
            {
                IUndoRedo iur2 = new UndoRedoSubElementAddMultiple<T>()
                {
                    cat = this,
                    index = new_index,
                    subindex = i,
                    item = Items[from_start+i]
                };
                iur2.Commit(urq);
            }
            urq?.CloseCluster();

            return true;
        }

        public bool Remove(int index)
        {
            IUndoRedo iur = new UndoRedoElementRemoveMultiple<T>()
            {
                cat = this,
                index = index,
            };
            iur.Commit(urq);

            return true;
        }

        public bool RemoveSub(int index, int subindex)
        {
            IUndoRedo iur = new UndoRedoSubElementRemoveMultiple<T>()
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
            IUndoRedo iur = new UndoRedoElementSetIDMultiple<T>()
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

        public void SetField<U>(int index, int subindex, string field_name, U value)
        {
            IUndoRedo iur = new UndoRedoSubElementSetFieldMultiple<T, U>()
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

        public bool SetOnSubElementAddedCallback(dOnSubElementAdded cb)
        {
            OnSubElementAdded = cb;
            return true;
        }

        public bool SetOnSubElementModifiedCallback(dOnSubElementModified cb)
        {
            OnSubElementModified = cb;
            return true;
        }

        public bool SetOnSubElementRemovedCallback(dOnSubElementRemoved cb)
        {
            OnSubElementRemoved = cb;
            return true;
        }

        public bool ClearCallbacks()
        {
            OnElementAdded = null;
            OnElementRemoved = null;
            OnSubElementAdded = null;
            OnSubElementModified = null;
            OnSubElementRemoved = null;
            return true;
        }

        public virtual List<string> GetSearchableFields()
        {
            return Utility.GetFields<T>();
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
                Utility.GetFieldData<T>(field_name, out uint field_offset, out Type ft, out uint field_num);
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
            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            int cur_value = 0;
            unsafe
            {
                fixed (T* ptr = items_span)
                {
                    if (as_flag)
                    {
                        for (int i = 0; i < Indices.Count; i++)
                        {
                            for (int j = 0; j < GetItemSubItemNum(i); j++)
                            {
                                U* ptr2 = (U*)(((byte*)(&ptr[Indices[i]+j])) + field_offset);
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
            Span<T> items_span = CollectionsMarshal.AsSpan(Items);
            Encoding encoding = Encoding.GetEncoding(1252);
            unsafe
            {
                fixed (T* ptr = items_span)
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

    public class UndoRedoElementAddMultiple<T> : IUndoRedo where T : struct, ICategorySubItem
    {
        public CategoryBaseMultiple<T> cat;
        public int index;
        public T item;

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

    public class UndoRedoElementSetIDMultiple<T> : IUndoRedo where T : struct, ICategorySubItem
    {
        public CategoryBaseMultiple<T> cat;
        public int index;
        public int new_id;
        int previous_id;

        public bool Init()
        {
            if(index >= cat.Indices.Count)
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
            Span<T> span = CollectionsMarshal.AsSpan(cat.Items);
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
            Span<T> span = CollectionsMarshal.AsSpan(cat.Items);
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

    public class UndoRedoElementRemoveMultiple<T> : IUndoRedo where T : struct, ICategorySubItem
    {
        public CategoryBaseMultiple<T> cat;
        public int index;
        List<T> item;

        public bool Init()
        {
            int main_index;
            if (index >= cat.Indices.Count)
            {
                throw new Exception();
            }
            main_index = cat.Indices[index];

            item = new();
            for(int i = 0; i < cat.GetItemSubItemNum(index); i++)
            {
                item.Add(cat.Items[main_index + i]);
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

            for(int i = 0; i < item.Count; i++)
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

    public class UndoRedoSubElementAddMultiple<T> : IUndoRedo where T : struct, ICategorySubItem
    {
        public CategoryBaseMultiple<T> cat;
        public int index;
        public int subindex;
        public T item;

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

    public class UndoRedoSubElementReplaceMultiple<T> : IUndoRedo where T : struct, ICategorySubItem
    {
        public CategoryBaseMultiple<T> cat;
        public int index;
        public int subindex;
        public T new_item;
        T previous_item;

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

            previous_item = cat[index, subindex];
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

    public class UndoRedoSubElementRemoveMultiple<T> : IUndoRedo where T : struct, ICategorySubItem
    {
        public CategoryBaseMultiple<T> cat;
        public int index;
        public int subindex;
        T item;

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
            item = cat[index, subindex];

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

    public class UndoRedoSubElementSetFieldMultiple<T, U> : IUndoRedo where T : struct, ICategorySubItem
    {
        public CategoryBaseMultiple<T> cat;
        public int index;
        public int subindex;
        public string field_name;
        public U value;
        T new_item;
        T previous_item;
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

            previous_item = cat[index, subindex];
            Redo();
            if (!valid)
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
                Type t = typeof(T);

                int real_index = cat.GetSubItemIndex(index, subindex);
                Span<T> items_span = CollectionsMarshal.AsSpan(cat.Items);
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
                            fixed (T* ptr = items_span)
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
                            fixed (T* ptr = items_span)
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

                if(result)
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
}
