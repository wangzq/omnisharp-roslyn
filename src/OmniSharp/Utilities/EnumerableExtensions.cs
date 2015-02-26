using System;
using System.Collections.Generic;

namespace OmniSharp.Utilities
{
    public static class EnumerableExtensions
    {
        public static bool HasAny<T>(this IEnumerable<T> enumerable)
        {
            return enumerable != null && enumerable.HasAny();
        }
    }
}
