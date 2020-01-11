using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtensionsPack.Core
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Checks whether the target collection is null or empty
        /// </summary>
        /// <typeparam name="T">Type of each element of collection</typeparam>
        /// <param name="collection">The target collection</param>
        /// <returns>True if collection is null or empty</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        /// <summary>
        /// This method will attempt to cast the IEnumerable of T to List of T first and if it fails
        /// it will call the .ToList() method that duplicates the collection.
        /// </summary>
        /// <typeparam name="T">Type of each element of collection</typeparam>
        /// <param name="collection">The target collection</param>
        /// <returns></returns>
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