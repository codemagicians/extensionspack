using System;
using System.Linq;
using NUnit.Framework;
using ExtensionsPack.Core;

namespace Tests
{
    public class Tests
    {
        private static readonly Random Rand = new Random();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetRandomString_CanGenerateRandomNumber()
        {
            var targetLength = Rand.Next(3, 100);
            var numbers = Enumerable.Range(0, 10).Select(x => RandomExtensions.GetRandomString(
                targetLength,
                false,
                false,
                true,
                repeatChars: true)).ToList();

            Assert.That(numbers, Is.Not.Null);
            Assert.That(numbers, Is.Not.Empty);
            Assert.That(numbers.Distinct().Count(), Is.EqualTo(numbers.Count));
            Assert.That(numbers.All(n => n.Length == targetLength), Is.True);
            Assert.That(numbers.All(n => n.All(char.IsNumber)));
            Assert.That(numbers.Any(n => n.Distinct().Count() != n.Length));
        }

        [Test]
        public void GetRandomString_WithRepeatCharsFalse_DoesNotRepeatLetters()
        {
            var targetLength = Rand.Next(3, 20);
            var numbers = Enumerable.Range(0, 100).Select(x => RandomExtensions.GetRandomString(targetLength)).ToList();

            Assert.That(numbers, Is.Not.Null);
            Assert.That(numbers, Is.Not.Empty);
            Assert.That(numbers.Distinct().Count(), Is.EqualTo(numbers.Count));
            Assert.That(numbers.All(n => n.Length == targetLength), Is.True);
            Assert.That(numbers.All(n => n.Distinct().Count() == n.Length));
        }

        [Test]
        [TestCase(15, 9)]
        [TestCase(5, 4)]
        public void GetRandomString_WithMinDistinctCharsSet_HasRequiredNumberOfChars(int targetLength, int minDistinctChars)
        {
            var numbers = Enumerable.Range(0, 5000).Select(x => RandomExtensions.GetRandomString(
                targetLength,
                upperCaseLetters: false,
                lowerCaseLetters: false,
                repeatChars: true,
                minDistinctChars: minDistinctChars)).ToList();

            Assert.That(numbers, Is.Not.Null);
            Assert.That(numbers, Is.Not.Empty);
            Assert.That(numbers.All(n => n.Length == targetLength), Is.True);
            Assert.That(numbers.All(n => n.Distinct().Count() >= minDistinctChars));
        }
    }
}