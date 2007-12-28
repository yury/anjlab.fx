using System;
using System.Collections.Generic;
using System.IO;

namespace AnjLab.FX.System
{
    public static class Str
    {
        public static bool EqualsInvariantIgnoreCase(string a, string b)
        {
            return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool ContainsInvariantIgnoreCase(string src, params string[] dest)
        {
            if (dest == null || src == null)
                return false;

            for (int i = 0; i < dest.Length; i++)
            {
                string d = dest[i];
                if (d == null)
                    continue;

                if (d.IndexOf(src, StringComparison.InvariantCultureIgnoreCase) >= 0)
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
