using System;
using System.Collections.Generic;

#if NET_3_5
using System.Linq;
#endif

namespace AnjLab.FX.System
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

        public static T [] ToArray<T>(IEnumerable<T> list)
        {
            Guard.ArgumentNotNull("list", list);

#if NET_3_5
            return list.ToArray();
#else 
            List<T> result = new List<T>();
            
            foreach (T t in list)
                result.Add(t);

            return result.ToArray();
#endif
        }

        public static string ToString<T>(IEnumerable<T> list, string separator)
        {
            Guard.ArgumentNotNull("list", list);
            Guard.ArgumentNotNull("separator", separator);

            List<string> strings = new List<string>();

            foreach (T item in list)
                strings.Add(string.Format("{0}", item));

            return string.Join(separator, strings.ToArray());
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
    }
}
