using System;
using System.Collections;
using System.Collections.Generic;

namespace AnjLab.FX.Collections
{
    public class PriorityQueue<TPriority, TItem>: ICollection, IEnumerable<TItem>
    {
        readonly SortedList<TPriority, Queue<TItem>> _parts = new SortedList<TPriority, Queue<TItem>>();
        private int _count;
        private readonly bool _isReadOnly = false;
        private readonly object _syncRoot = new object();
        private readonly bool _isSynchronized = false;

        public void CopyTo(Array array, int index)
        {
            ArrayList items = new ArrayList(this);
            items.CopyTo(array, index);
        }

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        public bool IsSynchronized
        {
            get { return _isSynchronized; }
        }

        public int Count
        {
            get { return _count; }
        }

        public int GetPriorityCount(TPriority priority)
        {
            if (!_parts.ContainsKey(priority))
                return 0;
            return _parts[priority].Count;
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
        }

        public void Enqueue(TPriority priority, TItem item)
        {
            if (!_parts.ContainsKey(priority))
            {
                _parts.Add(priority, new Queue<TItem>());
            }
            _parts[priority].Enqueue(item);
            _count++;
        }

        public TItem Dequeue()
        {
            foreach (KeyValuePair<TPriority, Queue<TItem>> part in _parts)
            {
                if (part.Value.Count == 0)
                    continue;
                _count--;
                return part.Value.Dequeue();
            }
            throw new InvalidOperationException("Queue is empty");
        }

        public TItem Dequeue(TPriority priority)
        {
            if (!_parts.ContainsKey(priority))
                throw new InvalidOperationException("Queue is empty");
            Queue<TItem> items = _parts[priority];
            if (items.Count > 0 )
            {
                _count--;
                return items.Dequeue();
            }
            throw new InvalidOperationException("Queue is empty");
        }

        public TItem Peek()
        {
            foreach (KeyValuePair<TPriority, Queue<TItem>> part in _parts)
            {
                if (part.Value.Count == 0)
                    continue;
                
                return part.Value.Peek();
            }
            throw new InvalidOperationException("Queue is empty");
        }

        IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
        {
            return GetTypedEnumerator();
        }

        IEnumerator<TItem> GetTypedEnumerator()
        {
            foreach (KeyValuePair<TPriority, Queue<TItem>> part in _parts)
            {
                foreach (TItem item in part.Value)
                {
                    yield return item;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return GetTypedEnumerator();
        }
    }
}
