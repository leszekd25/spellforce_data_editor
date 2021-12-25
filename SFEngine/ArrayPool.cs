using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine
{
    // used for contiguous memory, has more restrictions than LinearPool
    public class ArrayPool<T>
    {
        const int MIN_SIZE = 16;

        private T[] Data = new T[MIN_SIZE];
        public int Count { get; private set; } = 0;

        private void ResizeDouble()
        {
            T[] new_array = new T[Data.Length * 2];
            Array.Copy(Data, new_array, Data.Length);
            Data = new_array;
        }

        public void AddElem(T elem)
        {
            if (Count == Data.Length)
                ResizeDouble();
            Data[Count] = elem;
            Count += 1;
        }

        public void Clear()
        {
            Count = 0;
        }

        public void Set(int index, T elem)
        {
            Data[index] = elem;
        }

        public T[] GetData()
        {
            return Data;
        }

        public void Dispose()
        {
            Data = null;
            Count = 0;
        }
    }
}
