using SFEngine.SFChunk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                ushort coords_num = (ushort)(br.ReadByte() * 2);
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
                bw.Write((byte)item.Coords.Count * 2);
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
            AdjustIndices(index, from_end - from_start);

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

        public bool GetItemSubIndex(int id, int subid, out int index)
        {
            if (!GetItemIndex(id, out index))
            {
                return false;
            }

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

        public List<int> QueryItems()
        {
            return null;
        }
    }
}
