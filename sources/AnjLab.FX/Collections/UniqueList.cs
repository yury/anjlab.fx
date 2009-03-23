using System;
using System.Collections;
using System.Collections.Generic;

namespace AnjLab.FX.Collections
{
    public class UniqueList<TItem>: IList<TItem>
    {
        private readonly List<TItem> _innerList;

        public UniqueList()
        {
            _innerList = new List<TItem>();
        }

        public int IndexOf(TItem item)
        {
            return _innerList.IndexOf(item);
        }

        public void Insert(int index, TItem item)
        {
            if (Contains(item))
                return;

            _innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _innerList.RemoveAt(index);
        }

        public TItem this[int index]
        {
            get { return _innerList[index]; }
            set { _innerList[index] = value; }
        }

        public void Add(object obj)
        {
            Add((TItem) obj);
        }

        public void Add(TItem item)
        {
            if (Contains(item))
                return;

            _innerList.Add(item);
        }

        public void Clear()
        {
            _innerList.Clear();
        }

        public bool Contains(TItem item)
        {
            return _innerList.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(TItem item)
        {
            return _innerList.Remove(item);
        }

        public int Count
        {
            get { return _innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<TItem>)_innerList).IsReadOnly; }
        }

        IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<TItem>) this).GetEnumerator();
        }
    }
}