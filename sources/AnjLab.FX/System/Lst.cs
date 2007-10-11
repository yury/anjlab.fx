using System;
using System.Collections.Generic;

namespace AnjLab.FX.System
{
    public static class Lst
    {
        public static void ForEach<T>(IEnumerable<T> list, Action<T> action)
        {
            Guard.NotNull("list", list);
            Guard.NotNull("action", action);

            foreach (T t in list)
                action(t);
        }

        public static List<T> ToList<T>(IEnumerable<T> list)
        {
            Guard.NotNull("list", list);

            List<T> result = new List<T>();

            foreach (T t in list)
                result.Add(t);

            return result;
        }

        public static T [] ToArray<T>(IEnumerable<T> list)
        {
            Guard.NotNull("list", list);

            List<T> result = new List<T>();
            
            foreach (T t in list)
                result.Add(t);

            return result.ToArray();
        }

        public static string ToString<T>(IEnumerable<T> list, string separator)
        {
            Guard.NotNull("list", list);
            Guard.NotNull("separator", separator);

            List<string> strings = new List<string>();

            foreach (T item in list)
                strings.Add(string.Format("{0}", item));

            return string.Join(separator, strings.ToArray());
        }

        public static string ToString<T>(IEnumerable<T> list)
        {
            Guard.NotNull("list", list);

            return ToString(list, ", ");
        }

        public static bool Exists<T>(IEnumerable<T> list, Predicate<T> predicate)
        {
            Guard.NotNull("list", list);
            Guard.NotNull("predicate", predicate);

            foreach (T item in list)
            {
                if (predicate(item))
                    return true;
            }

            return false;
        }
    }
}
