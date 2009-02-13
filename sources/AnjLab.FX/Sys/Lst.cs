using System;
using System.Collections.Generic;
using System.Text;

#if NET_3_5
using System.Linq;
using System.Text;

#endif

namespace AnjLab.FX.Sys
{
    public static class Lst
    {
        public static void ForEach<T>(IEnumerable<T> list, Action<T> action)
        {
            Guard.ArgumentNotNull("list", list);
            Guard.ArgumentNotNull("action", action);

            foreach (T t in list)
                action(t);
        }

        public static List<T> ToList<T>(IEnumerable<T> list)
        {
            Guard.ArgumentNotNull("list", list);
#if NET_3_5
            return list.ToList();
#else 
            List<T> result = new List<T>();

            foreach (T t in list)
                result.Add(t);

            return result;
#endif
        }

        public static List<TResult> ToList<TResult, TSource>(IEnumerable<TSource> list) where TSource : TResult
        {
            Guard.ArgumentNotNull("list", list);
#if NET_3_5
            return list.Cast<TResult>().ToList();
#else

            List<TResult> result = new List<TResult>();

            foreach (TSource t in list)
                result.Add(t);

            return result;
#endif
        }

        //Test trac notifications diffs
        public static T [] ToArray<T>(IEnumerable<T> list)
        {
            Guard.ArgumentNotNull("list", list);

            List<T> result = list as List<T>;
            if (result != null)
                return result.ToArray();

            result = new List<T>(list);
            
            foreach (T t in list)
                result.Add(t);

            return result.ToArray();
        }

        public static string ToString<T>(IEnumerable<T> list, string separator)
        {
            Guard.ArgumentNotNull("list", list);
            Guard.ArgumentNotNull("separator", separator);

            StringBuilder sb = new StringBuilder();

            foreach (T t in list)
            {
                sb.Append(t);
                sb.Append(separator);
            }
            int size = sb.Length - separator.Length;
            if (size > 0)
                return sb.ToString(0, size);
            else
                return sb.ToString();
        }

        public static string ToString<TA, TB>(IEnumerable<KeyValuePair<TA, TB>> list, string format, string separator)
        {
            Guard.ArgumentNotNull("list", list);
            Guard.ArgumentNotNull("format", format);
            Guard.ArgumentNotNull("separator", separator);

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<TA, TB> pair in list)
            {
                sb.AppendFormat(format, pair.Key, pair.Value);
                sb.Append(separator);
            }

            int size = sb.Length - separator.Length;
            if (size > 0)
                return sb.ToString(0, size);
            else
                return sb.ToString();
        }

        public static string ToString<T>(IEnumerable<T> list)
        {
            Guard.ArgumentNotNull("list", list);

            return ToString(list, ", ");
        }

        public static bool Exists<T>(IEnumerable<T> list, Predicate<T> predicate)
        {
            Guard.ArgumentNotNull("list", list);
            Guard.ArgumentNotNull("predicate", predicate);
#if NET_3_5
            return list.Any((x) => predicate(x));
#else 
            foreach (T item in list)
            {
                if (predicate(item))
                    return true;
            }

            return false;
#endif
        }

        public static IList<T> Clone<T>(IList<T> src) 
            where T: ICloneable
        {
            if (src == null)
                return null;

            var result = new List<T>(src.Count);
            foreach (var item in src) {
                var clone = (T)item.Clone();
                result.Add(clone);
            }
            return result;
        }
    }
}
