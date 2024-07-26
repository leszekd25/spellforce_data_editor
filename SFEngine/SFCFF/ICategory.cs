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

namespace SFEngine.SFCFF
{
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
        public int GetCurrentMaxID();
        public int GetByteCount();
        public bool Load(SFChunkFile file);
        public bool IsLoaded();
        public bool WriteRawData(ref byte[] data);
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
        public bool Undo();
        public bool Redo();
        public bool CanUndo();
        public bool CanRedo();
        public List<int> QueryItems();
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
}
