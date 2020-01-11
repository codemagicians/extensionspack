using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExtensionsPack.Core
{
    public static class RandomExtensions
    {
        private static Random Rand { get; }
        private static List<char> UpperCaseLettersChars { get; }
        private static List<char> LowerCaseLettersChars { get; }
        private static List<char> AllNumberChars { get; }
        private static List<char> AllOtherChars { get; }

        static RandomExtensions()
        {
            Rand = new Random();
            UpperCaseLettersChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList();
            LowerCaseLettersChars = UpperCaseLettersChars.Select(char.ToLowerInvariant).ToList();
            AllNumberChars = Enumerable.Range(0, 10).Select(x => Convert.ToChar(x.ToString())).ToList();
            AllOtherChars = "~!@#$%^&*(){}[]\"';:.>/?,<|\\`-+".ToList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="existingValue"></param>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        public static T GetDifferentRandom<T>(this IEnumerable<T> collection, Func<T, bool> filterExpression)
        {
            T result = GetDifferentRandomOrDefault(collection, filterExpression);

            if (result.Equals(default(T)))
            {
                throw new ArgumentException("Collection contains no matching elements");
            }
            return result;
        }

        /// <summary>
        /// Gets first element that
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        public static T GetDifferentRandomOrDefault<T>(this IEnumerable<T> collection, Func<T, bool> filterExpression)
        {
            var filteredCollection = collection.Where(filterExpression);
            return !filteredCollection.IsNullOrEmpty() ? filteredCollection.GetRandom() : default(T);
        }

        public static T GetRandom<T>(this List<T> list)
        {
            if (list.IsNullOrEmpty())
            {
                throw new ArgumentException("Collection cannot be null or empty");
            }
            return list.Count == 1 ? list.First() : list[Rand.Next(0, list.Count)];
        }

        public static T GetRandom<T>(this IEnumerable<T> collection)
        {
            if (collection.IsNullOrEmpty())
            {
                throw new ArgumentException("Collection cannot be null or empty");
            }
            var index = Rand.Next(0, collection.Count());
            return collection.ElementAt(index);
        }

        public static T GetRandomOrDefault<T>(this IEnumerable<T> collection, Expression<Func<T, bool>> filterExpression)
        {
            if (filterExpression == null)
            {
                throw new ArgumentNullException(nameof(filterExpression));
            }
            var filter = filterExpression.Compile();
            var collectionAsList = collection.CastToList();

            if (collectionAsList?.Any() != true)
            {
                return default(T);
            }

            if (collectionAsList.Count == 1)
            {
                var element = collectionAsList.First();
                return filter.Invoke(element) == false ? default(T) : element;
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

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection, int numberOfTimes = 1)
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

        public static string GetRandomString(
            int numberOfChars,
            bool upperCaseLetters = true,
            bool lowerCaseLetters = true,
            bool numbers = true,
            bool otherSymbols = false,
            bool withWhitespaces = false,
            bool repeatChars = false,
            int minDistinctChars = 1)
        {
            return new string(GetRandomChars(
                numberOfChars,
                upperCaseLetters,
                lowerCaseLetters,
                numbers,
                otherSymbols,
                withWhitespaces,
                repeatChars,
                minDistinctChars).ToArray());
        }

        public static void FillWithRandomStrings<T>(this IEnumerable<T> collection,
            Action<T, string> customAction,
            int stringLength,
            bool lowerCaseLetters = true,
            bool upperCaseLetters = true,
            bool numbers = true,
            bool otherSymbols = false,
            bool withWhitespaces = false,
            bool repeatChars = true,
            int minDistinctChars = 1)
        {
            if (collection?.Any() != true)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            var charsPool = GetRandomChars(
                stringLength,
                lowerCaseLetters,
                upperCaseLetters,
                numbers,
                otherSymbols,
                withWhitespaces,
                repeatChars,
                minDistinctChars).ToList();

            foreach (var element in collection)
            {
                customAction(element, new string(charsPool.ToArray()));
                charsPool.Shuffle();
            }
        }

        public static IEnumerable<char> GetRandomChars(
            int numberOfChars,
            bool allowLowerCaseLetters = true,
            bool allowUpperCaseLetters = true,
            bool allowNumbers = true,
            bool allowOtherSymbols = false,
            bool allowWhitespaces = false,
            bool repeatChars = true,
            int minDistinctChars = 1)
        {
            if (!(allowLowerCaseLetters || allowUpperCaseLetters) && !allowNumbers && !allowOtherSymbols)
            {
                throw new ArgumentException("At least one flag must be set to true in order to create a random string");
            }

            if (numberOfChars <= 0)
            {
                throw new ArgumentException("String cannot be less than 1 character long");
            }
            var charsPool = new List<char>(140);

            if (allowUpperCaseLetters)
            {
                charsPool.AddRange(UpperCaseLettersChars);
            }

            if (allowLowerCaseLetters)
            {
                charsPool.AddRange(LowerCaseLettersChars);
            }

            if (allowNumbers)
            {
                charsPool.AddRange(AllNumberChars);
            }

            if (allowOtherSymbols)
            {
                charsPool.AddRange(AllOtherChars);
            }

            if (allowWhitespaces)
            {
                charsPool.Add(' ');
            }

            if ((!repeatChars && numberOfChars > charsPool.Count) || (repeatChars && minDistinctChars > charsPool.Count))
            {
                throw new ArgumentException("Impossible to compose a random string of selected length with parameters specified");
            }
            return repeatChars
                ? new string(charsPool.Take(minDistinctChars).Concat(Enumerable
                    .Range(0, numberOfChars - minDistinctChars).Select(x =>
                        charsPool[Rand.Next(0, charsPool.Count)])).Shuffle().ToArray())
                : new string(charsPool.Shuffle().Take(numberOfChars).ToArray());
        }
    }
}
