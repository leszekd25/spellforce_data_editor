using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SFEngine.SFChunk;
using System.Runtime.CompilerServices;
using SFEngine.SFCFF.CTG;
using System.Collections;
using SFEngine.SFLua.LuaDecompiler;
using OpenTK.Windowing.Common.Input;

namespace SFEngine.SFCFF
{
    public delegate void dOnElementAdded(int cat_id, int elem_index);
    public delegate void dOnElementModified(int cat_id, int elem_index);
    public delegate void dOnElementRemoved(int cat_id, int elem_index);
    public delegate void dOnSubElementAdded(int cat_id, int elem_index, int subelem_index);
    public delegate void dOnSubElementModified(int cat_id, int elem_index, int subelem_index);
    public delegate void dOnSubElementRemoved(int cat_id, int elem_index, int subelem_index);


    [Flags]
    public enum SearchOption
    {
        NONE = 0x0,
        IS_NUMBER = 0x1,
        IS_STRING = 0x2,
        IGNORE_CASE = 0x1000,
        NUMBER_AS_BITMASK = 0x2000,
    }

    /* each category should have the following capabilities:
     *  - store items in ascending order
        - load all data from file in an efficient manner
        - save all data to file in an efficient manner
        - get a name string for each item
        - get a description string for each item
        - add new empty item
        - copy and paste an item
        - remove an existing item
        - get current number of items
        - get max existing ID
        - get item index from ID
        
     */
    public interface ICategory
    {
        public string GetName();
        public short GetCategoryID();
        public short GetCategoryType();
        public int GetNumOfItems();
        public int GetByteCount();
        public bool Load(SFChunkFile file);
        public bool IsLoaded();
        public bool Save(SFChunkFile file);
        public bool MergeFrom(ICategory c1, ICategory c2);
        public bool DiffFrom(ICategory c1, ICategory c2);
        public bool Clear();
        public bool Sort();
        public bool AddEmpty(int new_index);
        public bool AddID(int new_index, int new_id);
        public bool Copy(int from_index, int new_index);
        public bool Remove(int index);
        public bool GetID(int index, out int id);
        public bool SetID(int index, int id);
        public bool CalculateNewItemIndex(int new_id, out int index);
        public bool GetItemIndex(int id, out int index);
        public bool GetFirstUnusedID(out int id, out int index);  // returns the unused ID and the index at which an item with that ID can be inserted
        public bool GetLastUsedID(out int id, out int index);  // returns ID of the last element, and the index is equal to item count
        public bool EnableUndoRedo(UndoRedoQueue queue);   // once enabled, it cant be disabled unless Clear() is called
        public bool SetOnElementAddedCallback(dOnElementAdded cb);
        public bool SetOnElementModifiedCallback(dOnElementModified cb);
        public bool SetOnElementRemovedCallback(dOnElementRemoved cb);
        public bool SetOnSubElementAddedCallback(dOnSubElementAdded cb);
        public bool SetOnSubElementModifiedCallback(dOnSubElementModified cb);
        public bool SetOnSubElementRemovedCallback(dOnSubElementRemoved cb);
        public bool ClearCallbacks();
        public List<string> GetSearchableFields();
        public List<int> QueryItems(object value, string field_name, SearchOption option);
    }

    public interface ICategoryItem: IComparable<ICategoryItem>
    {
        public int GetID();
        public void SetID(int id);
        int IComparable<ICategoryItem>.CompareTo(ICategoryItem item) => GetID().CompareTo(item.GetID());
    }

    public interface ICategorySubItem: ICategoryItem, IComparable<ICategorySubItem>
    {
        public int GetSubID();
        public void SetSubID(int subid);

        int IComparable<ICategorySubItem>.CompareTo(ICategorySubItem item)
        {
            int result = GetID().CompareTo(item.GetID());
            if(result == 0)
            {
                return GetSubID().CompareTo(item.GetSubID());
            }
            return result;
        }
    }

    // undo/redo
    public interface IUndoRedo
    {
        public bool Init();    // most operators will call Redo() here at the end; returns whether the operator did its job; if not, it wont be added to queue
        public void Undo();
        public void Redo();

        public void Commit(UndoRedoQueue urq)
        {
            if (urq != null)
            {
                urq.Push(this);
            }
            else
            {
                Redo();
            }
        }
    }

    public class UndoRedoCluster : IUndoRedo
    {
        public List<IUndoRedo> items = new();

        // this operator is unique: it doesnt call Redo() directly
        public bool Init()
        {
            List<int> items_to_remove = new();
            for (int i = 0; i < items.Count; i++)
            {
                if(!items[i].Init())
                {
                    items_to_remove.Add(i);
                }
            }
            for(int i = 0; i < items_to_remove.Count; i++)
            {
                items.RemoveAt(i);
            }
            if(items.Count == 0)
            {
                return false;
            }

            return true;
        }

        public void Undo()
        {
            for(int i = items.Count-1; i >= 0; i--)
            {
                items[i].Undo();
            }
        }

        public void Redo()
        {
            for(int i = 0; i < items.Count; i++)
            {
                items[i].Redo();
            }
        }

        public override string ToString()
        {
            return $"Multiple modifications [{items.Count}]";
        }
    }

    public delegate void dOnPush(IUndoRedo iur);
    public delegate void dOnPop();
    public delegate void dOnUndoStateChange(bool state);
    public delegate void dOnRedoStateChange(bool state);

    public class UndoRedoQueue
    {
        List<IUndoRedo> queue = new();
        UndoRedoCluster current_cluster = null;
        int current_index = SFEngine.Utility.NO_INDEX;

        public dOnUndoStateChange OnUndoStateChange;
        public dOnRedoStateChange OnRedoStateChange;
        public dOnPush OnPush;
        public dOnPop OnPop;

        public IUndoRedo this[int index]
        {
            get
            {
                return queue[index];
            }
        }

        public int GetCurrentIndex()
        {
            return current_index;
        }

        public int GetNumOfItems()
        {
            return queue.Count;
        }

        public bool IsClusterOpen()
        {
            return (current_cluster != null);
        }

        public void OpenCluster()
        {
            if(IsClusterOpen())
            {
                return;
            }

            current_cluster = new();
        }

        public void CloseCluster()
        {
            UndoRedoCluster cl = current_cluster;
            current_cluster = null;

            if(cl.items.Count == 0)
            {
                return;
            }
            if(cl.items.Count == 1)
            {
                Push(cl.items[0]);
                return;
            }
            Push(cl);
        }

        public void Push(IUndoRedo item)
        {
            if (IsClusterOpen())
            {
                current_cluster.items.Add(item);
            }
            else
            {
                if (!item.Init())
                {
                    return;
                }

                while (current_index < queue.Count - 1)
                {
                    queue.RemoveAt(queue.Count - 1);
                    OnPop?.Invoke();
                }
                queue.Add(item);
                current_index++;
                OnPush?.Invoke(item);

                OnRedoStateChange?.Invoke(false);
                OnUndoStateChange?.Invoke(true);
            }
        }

        public void Pop()
        {
            if (IsClusterOpen())
            {
                current_cluster.items.RemoveAt(current_cluster.items.Count - 1);
            }
            else
            {
                queue.RemoveAt(queue.Count - 1);
                OnPop?.Invoke();
                if(current_index == queue.Count)
                {
                    current_index--;
                    OnRedoStateChange?.Invoke(false);
                }
            }
        }

        public void Undo()
        {
            if(current_index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            queue[current_index].Undo();
            current_index--;
            OnRedoStateChange?.Invoke(true);

            if(current_index == SFEngine.Utility.NO_INDEX)
            {
                OnUndoStateChange?.Invoke(false);
            }
        }

        public void Redo()
        {
            if(current_index == queue.Count - 1)
            {
                return;
            }

            current_index++;
            queue[current_index].Redo();
            OnUndoStateChange?.Invoke(true);

            if(current_index == queue.Count - 1)
            {
                OnRedoStateChange?.Invoke(false);
            }
        }

        public bool CanUndo()
        {
            return (current_index != SFEngine.Utility.NO_INDEX);
        }

        public bool CanRedo()
        {
            return (current_index != queue.Count - 1);
        }

        public void Clear()
        {
            current_cluster = null;
            queue.Clear();
            current_index = SFEngine.Utility.NO_INDEX;
            OnUndoStateChange?.Invoke(false);
            OnRedoStateChange?.Invoke(false);
        }
    }
}
