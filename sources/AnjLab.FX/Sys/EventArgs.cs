using System;

namespace AnjLab.FX.Sys
{
    public class EventArgs<T> : EventArgs
    {
        internal EventArgs(T item)
        {
            Item = item;
        }

        protected EventArgs()
        {
        }

        public T Item { get; set; }

        static readonly EventArgs<T> _empty = new EventArgs<T>();
        public static new EventArgs<T> Empty { get { return _empty; } }
    }

    public class EventArgs<T1, T2> : EventArgs
    {
        internal EventArgs(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        protected EventArgs()
        {
        }

        public T1 Item1 { get; set; }

        public T2 Item2 { get; set; }

        static readonly EventArgs<T1,T2> _empty = new EventArgs<T1,T2>();
        public static new EventArgs<T1,T2> Empty { get { return _empty; } }
    }

    public class EventArgs<T1, T2, T3> : EventArgs
    {
        internal EventArgs(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        protected EventArgs()
        {
        }

        public T1 Item1 { get; set; }

        public T2 Item2 { get; set; }

        public T3 Item3 { get; set; }

        static readonly EventArgs<T1, T2, T3> _empty = new EventArgs<T1, T2, T3>();
        public static new EventArgs<T1, T2, T3> Empty { get { return _empty; } }
    }

    /// <summary>
    /// Syntax helper
    /// </summary>
    public static class EventArg
    {
        public static EventArgs<TItem> New<TItem>(TItem item)
        {
            return new EventArgs<TItem>(item);
        }

        public static EventArgs<TItem1, TItem2> New<TItem1, TItem2>(TItem1 item1, TItem2 item2)
        {
            return new EventArgs<TItem1, TItem2>(item1, item2);
        }

        public static EventArgs<T1, T2, T3> New<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
        {
            return new EventArgs<T1, T2, T3>(item1, item2, item3);
        }

        public static EventArgs<TItem> Empty<TItem>()
        {
            return EventArgs<TItem>.Empty;
        }
    }
}
