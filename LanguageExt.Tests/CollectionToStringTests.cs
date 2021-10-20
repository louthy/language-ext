using System;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class CollectionToStringTests
    {
        readonly static int[] zeroToFour = Range(0, 5).ToArray();
        readonly static int[] zeroToFourtyNine = Range(0, 50).ToArray();
        readonly static int[] zeroToFiftyNine = Range(0, 60).ToArray();

        readonly static string zeroToFourString = "[0, 1, 2, 3, 4]";
        readonly static string zeroToFourtyNineString = "[0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49]";
        readonly static string zeroToFiftyNineString = "[0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 ... 10 more]";

        readonly static (string, int)[] zeroToFourkeyValue = new[] { ("A", 0), ("B", 1), ("C", 2), ("D", 3), ("E", 4) };
        readonly static string zeroToFourKeyValueString = "[(A: 0), (B: 1), (C: 2), (D: 3), (E: 4)]";

        [Fact]
        public void ArrShortToString()
        {
            var items = toArray(zeroToFour);
            Assert.True(items.ToString() == zeroToFourString);
        }

        [Fact]
        public void ArrMedToString()
        {
            var items = toArray(zeroToFourtyNine);
            Assert.True(items.ToString() == zeroToFourtyNineString);
        }

        [Fact]
        public void ArrLongToString()
        {
            var items = toArray(zeroToFiftyNine);
            Assert.True(items.ToString() == zeroToFiftyNineString);
        }

        [Fact]
        public void LstShortToString()
        {
            var items = toList(zeroToFour);
            Assert.True(items.ToString() == zeroToFourString);
        }

        [Fact]
        public void LstMedToString()
        {
            var items = toList(zeroToFourtyNine);
            Assert.True(items.ToString() == zeroToFourtyNineString);
        }

        [Fact]
        public void LstLongToString()
        {
            var items = toList(zeroToFiftyNine);
            Assert.True(items.ToString() == zeroToFiftyNineString);
        }

        [Fact]
        public void SeqShortToString()
        {
            var items = toSeq(zeroToFour);
            Assert.True(items.ToString() == zeroToFourString);
        }

        [Fact]
        public void SeqMedToString()
        {
            var items = toSeq(zeroToFourtyNine);
            Assert.True(items.ToString() == zeroToFourtyNineString);
        }

        [Fact]
        public void SeqLongToString()
        {
            var items = toSeq(zeroToFiftyNine);
            Assert.True(items.ToString() == zeroToFiftyNineString);
        }

        [Fact]
        public void SetShortToString()
        {
            var items = toSet(zeroToFour);
            Assert.True(items.ToString() == zeroToFourString);
        }

        [Fact]
        public void SetMedToString()
        {
            var items = toSet(zeroToFourtyNine);
            Assert.True(items.ToString() == zeroToFourtyNineString);
        }

        [Fact]
        public void SetLongToString()
        {
            var items = toSet(zeroToFiftyNine);
            Assert.True(items.ToString() == zeroToFiftyNineString);
        }

        [Fact]
        public void MapToString()
        {
            var items = toMap(zeroToFourkeyValue);
            Assert.True(items.ToString() == zeroToFourKeyValueString);
        }
    }
}
