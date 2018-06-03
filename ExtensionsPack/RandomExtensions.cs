using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExtensionsPack
{
    public static class RandomExtensions
    {
        private static Random Rand { get; }
        private static List<char> AllLettersChars { get; }
        private static List<char> AllNumberChars { get; }
        private static List<char> AllOtherChars { get; }

        static RandomExtensions()
        {
            Rand = new Random();
            AllLettersChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToList();
            AllNumberChars = Enumerable.Range(0, 10).Select(x => Convert.ToChar(x.ToString())).ToList();
            AllOtherChars = "~!@#$%^&*(){}[]\"';:.>/?,<|\\`-+".ToList();
        }

        public static T GetDifferentRandom<T>(this IEnumerable<T> collection, T existingValue, Expression<Func<T, T, bool>> filterExpression = null)
        {
            T result = GetDifferentRandomOrDefault(collection, existingValue, filterExpression);

            if (result.Equals(default(T)))
            {
                throw new ArgumentException("Collection contains no matching elements");
            }
            return result;
        }

        public static T GetDifferentRandomOrDefault<T>(this IEnumerable<T> collection, T existingValue, Expression<Func<T, T, bool>> filterExpression = null)
        {
            Func<T, T, bool> filter = filterExpression?.Compile();
            var collectionAsList = collection as IList<T> ?? collection?.ToList();

            if (collectionAsList?.Any() != true)
            {
                return default(T);
            }

            if (collectionAsList.Count == 1)
            {
                var first = collectionAsList.First();
                return filter?.Invoke(existingValue, first) == true ? first : default(T);
            }
            var filteredCollection = filter != null
                ? collectionAsList.Where(x => filter(existingValue, x)).ToList()
                : collectionAsList.Where(x => !x.Equals(existingValue)).ToList();
            return filteredCollection.Any() ? GetRandom(filteredCollection) : default(T);
        }

        public static T GetRandom<T>(this IEnumerable<T> collection)
        {
            var collectionAsList = collection as IList<T> ?? collection?.ToList();

            if (collectionAsList?.Any() != true)
            {
                throw new ArgumentException("Collection cannot be null or empty");
            }

            if (collectionAsList.Count == 1)
            {
                return collectionAsList.First();
            }
            return collectionAsList[Rand.Next(0, collectionAsList.Count)];
        }

        public static T GetRandomOrDefault<T>(this IEnumerable<T> collection, Expression<Func<T, bool>> filterExpression = null)
        {
            var filter = filterExpression?.Compile();
            var collectionAsList = collection as IList<T> ?? collection?.ToList();

            if (collectionAsList?.Any() != true)
            {
                return default(T);
            }

            if (collectionAsList.Count == 1)
            {
                var element = collectionAsList.First();
                return filter?.Invoke(element) == false ? default(T) : element;
            }

            if (filter == null)
            {
                return collectionAsList[Rand.Next(0, collectionAsList.Count)];
            }
            var filteredCollection = collectionAsList.Where(filter).ToList();

            if (!filteredCollection.Any())
            {
                return default(T);
            }
            return filteredCollection.Count == 1
                ? filteredCollection.First()
                : filteredCollection[Rand.Next(0, filteredCollection.Count)];
        }

        public static int GetRandom(int from, int to)
        {
            return Rand.Next(from, to);
        }

        public static int GetRandom(int to)
        {
            return Rand.Next(0, to);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection, int numberOfTimes = 3)
        {
            if (numberOfTimes < 1)
            {
                throw new ArgumentException("Invalid value", nameof(numberOfTimes));
            }

            var list = collection as IList<T> ?? collection.ToList();
            for (; numberOfTimes > 0; numberOfTimes--)
            {
                for (var i = 0; i < collection.Count() - 1; i++)
                {
                    var temp = list[i];
                    var randIndex = Rand.Next(i + 1, list.Count);
                    list[i] = list[randIndex];
                    list[randIndex] = temp;
                }
            }
            return list;
        }

        public static string GetRandomString(int numberOfChars, bool letters = true, bool numbers = true, bool otherSymbols = false, bool withWhitspaces = false)
        {
            return new string(GetCharsPool(numberOfChars, letters, numbers, otherSymbols, withWhitspaces).ToArray());
        }

        public static void FillWithRandomStrings<T>(this IEnumerable<T> collection,
            Action<T, string> fillValueAction,
            int? stringLength = null,
            bool letters = true,
            bool numbers = true,
            bool otherSymbols = false,
            bool withWhitspaces = false)
        {
            if (collection?.Any() != true)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            var charsPool = GetCharsPool(stringLength, letters, numbers, otherSymbols, withWhitspaces).ToList();
            var collectionAsList = collection as IList<T> ?? collection.ToList();
            foreach (var element in collectionAsList)
            {
                fillValueAction(element, new string(charsPool.ToArray()));
                charsPool.Shuffle(1);
            }
        }

        private static IEnumerable<char> GetCharsPool(
            int? numberOfChars = null,
            bool letters = true,
            bool numbers = true,
            bool otherSymbols = false,
            bool withWhitspaces = false,
            bool shuffled = true)
        {
            if (!letters && !numbers && !otherSymbols)
            {
                throw new ArgumentException("At least one flag must be set to true in order to create a random string");
            }

            if (numberOfChars <= 0)
            {
                throw new ArgumentException("String cannot be less than 1 character long");
            }

            var charsPool = new List<char>(AllLettersChars.Count + AllNumberChars.Count + AllOtherChars.Count);

            if (letters)
            {
                charsPool.AddRange(AllLettersChars);
            }

            if (numbers)
            {
                charsPool.AddRange(AllNumberChars);
            }

            if (otherSymbols)
            {
                charsPool.AddRange(AllOtherChars);
            }

            if (withWhitspaces)
            {
                charsPool.Add(' ');
            }

            if (numberOfChars != null)
            {
                for (var multiplier = numberOfChars.Value / (double)charsPool.Count; multiplier > 1; multiplier--)
                {
                    charsPool = charsPool.Concat(charsPool).ToList();
                }
                return (shuffled ? charsPool.Shuffle(1) : charsPool).Take(numberOfChars.Value).ToList();
            }
            return shuffled ? charsPool.Shuffle(1) : charsPool;
        }
    }
}
