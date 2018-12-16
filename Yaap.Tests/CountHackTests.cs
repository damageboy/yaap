using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Yaap.Tests
{
    public class CountHackTests
    {
        [Test]
        public static void IIListProviderExists()
        {
            var iilp = typeof(Enumerable).Assembly.GetType("System.Linq.IIListProvider`1");

            Assert.That(iilp.IsGenericTypeDefinition, Is.True);

            var specializedIilp = iilp.MakeGenericType(typeof(int));

            Assert.That(specializedIilp.IsGenericTypeDefinition, Is.False);

            var range = Enumerable.Range(0, 100);

            Assert.That(specializedIilp.IsInstanceOfType(range), Is.True);
        }


        [Test]
        public static void EnumerableRangeWorks()
        {
            var range = Enumerable.Range(0, 100);
            Assert.That(YaapEnumerable<int>.GetCheapCount(range), Is.EqualTo(100));
        }

        [Test]
        public static void ConcatWorks()
        {
            var range = Enumerable.Range(0, 100).Concat(Enumerable.Range(100, 100));
            Assert.That(YaapEnumerable<int>.GetCheapCount(range), Is.EqualTo(200));
        }

        [Test]
        public static void SelectWorks()
        {
            var range = Enumerable.Range(0, 100).Select(x => x * x);
            Assert.That(YaapEnumerable<int>.GetCheapCount(range), Is.EqualTo(100));
        }

        [Test]
        public static void ReverseWorks()
        {
            var range = Enumerable.Range(0, 100).Reverse();
            Assert.That(YaapEnumerable<int>.GetCheapCount(range), Is.EqualTo(100));
        }

        [Test]
        public static void OrderByWorks()
        {
            var range = Enumerable.Range(0, 100).OrderByDescending(x => x);
            Assert.That(YaapEnumerable<int>.GetCheapCount(range), Is.EqualTo(100));
        }

        [Test]
        public static void AppendWorks()
        {
            var range = Enumerable.Range(0, 100).Append(101);
            Assert.That(YaapEnumerable<int>.GetCheapCount(range), Is.EqualTo(101));
        }


        [Test]
        public static void PrependWorks()
        {
            var range = Enumerable.Range(0, 100).Prepend(-1);
            Assert.That(YaapEnumerable<int>.GetCheapCount(range), Is.EqualTo(101));
        }

        [Test]
        public static void ListWorks()
        {
            var list = Enumerable.Range(0, 100).ToList();
            Assert.That(YaapEnumerable<int>.GetCheapCount(list), Is.EqualTo(100));
        }

        [Test]
        public static void ArrayWorks()
        {
            var array = Enumerable.Range(0, 100).ToArray();
            Assert.That(YaapEnumerable<int>.GetCheapCount(array), Is.EqualTo(100));
        }

        [Test]
        public static void DictionaryWorks()
        {
            var dict = Enumerable.Range(0, 100).ToDictionary(x => x, x => x);
            Assert.That(YaapEnumerable<KeyValuePair<int, int>>.GetCheapCount(dict), Is.EqualTo(100));
        }
    }
}
