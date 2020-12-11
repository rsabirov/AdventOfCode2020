using System.Collections.Generic;

namespace AdventOfCode2020
{
    internal static class Extensions
    {
        public static string Join(this IEnumerable<string> value, string separator)
        {
            return string.Join(separator, value);
        }
    }
}
