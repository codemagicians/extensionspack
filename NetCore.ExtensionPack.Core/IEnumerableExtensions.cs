using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtensionsPack.Core
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static IList<T> CastToList<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            return collection as IList<T> ?? collection.ToList();
        }
    }
}