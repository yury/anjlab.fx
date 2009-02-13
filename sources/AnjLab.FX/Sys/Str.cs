using System;
using System.Collections.Generic;
using System.IO;

namespace AnjLab.FX.Sys
{
    public static class Str
    {
        public static bool EqualsInvariantIgnoreCase(string a, string b)
        {
            return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
        }

        [Obsolete("Use Str.InvariantIgnoreCaseIsPartAnyOf")]
        public static bool ContainsInvariantIgnoreCase(string src, params string[] anyOf)
        {
            return InvariantIgnoreCaseIsPartAnyOf(src, anyOf);
        }

        public static bool InvariantIgnoreCaseIsPartAnyOf(string src, params string[] anyOf)
        {
            if (anyOf == null || src == null)
                return false;

            for (int i = 0; i < anyOf.Length; i++)
            {
                string d = anyOf[i];
                if (d == null)
                    continue;

                if (d.IndexOf(src, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        public static bool InvariantIgnoreCaseContainsAnyOf(string src, params string [] anyOf)
        {
            if (anyOf == null || src == null)
                return false;

            for (int i = 0; i < anyOf.Length; i++)
            {
                string p = anyOf[i];
                if (p == null)
                    continue;
                if (src.IndexOf(p, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        public static IEnumerable<string> LinesFrom(TextReader reader)
        {
            using (reader)
            {
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    yield return line;
                }
            }
        }

        public static IEnumerable<string> LinesFrom(string txt)
        {
            return LinesFrom(new StringReader(txt));
        }
    }
}
