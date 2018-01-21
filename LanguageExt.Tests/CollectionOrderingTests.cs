using System;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class CollectionOrderingTests
    {
        [Fact]
        public void TestSetOrdering1()
        {
            var x = Set(1, 2, 3, 4, 5);
            var y = Set(1, 2, 3, 4, 5);

            Assert.True(x.CompareTo(y) == 0);
            Assert.False(x < y);
            Assert.True(x <= y);
            Assert.False(x > y);
            Assert.True(x >= y);
        }

        [Fact]
        public void TestSetOrdering2()
        {
            var x = Set(1, 2, 3, 4, 5);
            var y = Set(1, 2, 3, 4, 6);

            Assert.True(x.CompareTo(y) < 0);
            Assert.True(x.CompareTo(y) <= 0);
            Assert.True(x < y);
            Assert.True(x <= y);
            Assert.False(x > y);
            Assert.False(x >= y);
        }

        [Fact]
        public void TestSetOrdering3()
        {
            var x = Set(1, 2, 3, 4, 6);
            var y = Set(1, 2, 3, 4, 5);

            Assert.True(x.CompareTo(y) > 0);
            Assert.True(x.CompareTo(y) >= 0);
            Assert.False(x < y);
            Assert.False(x <= y);
            Assert.True(x > y);
            Assert.True(x >= y);
        }

        [Fact]
        public void TestSetOrdering4()
        {
            var x = Set(1, 2, 3, 4, 5);
            var y = Set(1, 2, 3, 4);

            Assert.True(x.CompareTo(y) > 0);
            Assert.True(x.CompareTo(y) >= 0);
            Assert.False(x < y);
            Assert.False(x <= y);
            Assert.True(x > y);
            Assert.True(x >= y);
        }

        [Fact]
        public void TestSetOrdering5()
        {
            var x = Set(1, 2, 3, 4);
            var y = Set(1, 2, 3, 4, 5);

            Assert.True(x.CompareTo(y) < 0);
            Assert.True(x.CompareTo(y) <= 0);
            Assert.True(x < y);
            Assert.True(x <= y);
            Assert.False(x > y);
            Assert.False(x >= y);
        }


        [Fact]
        public void TestListOrdering1()
        {
            var x = List(1, 2, 3, 4, 5);
            var y = List(1, 2, 3, 4, 5);

            Assert.True(x.CompareTo(y) == 0);
            Assert.False(x < y);
            Assert.True(x <= y);
            Assert.False(x > y);
            Assert.True(x >= y);
        }

        [Fact]
        public void TestListOrdering2()
        {
            var x = List(1, 2, 3, 4, 5);
            var y = List(1, 2, 3, 4, 6);

            Assert.True(x.CompareTo(y) < 0);
            Assert.True(x.CompareTo(y) <= 0);
            Assert.True(x < y);
            Assert.True(x <= y);
            Assert.False(x > y);
            Assert.False(x >= y);
        }

        [Fact]
        public void TestListOrdering3()
        {
            var x = List(1, 2, 3, 4, 6);
            var y = List(1, 2, 3, 4, 5);

            Assert.True(x.CompareTo(y) > 0);
            Assert.True(x.CompareTo(y) >= 0);
            Assert.False(x < y);
            Assert.False(x <= y);
            Assert.True(x > y);
            Assert.True(x >= y);
        }

        [Fact]
        public void TestListOrdering4()
        {
            var x = List(1, 2, 3, 4, 5);
            var y = List(1, 2, 3, 4);

            Assert.True(x.CompareTo(y) > 0);
            Assert.True(x.CompareTo(y) >= 0);
            Assert.False(x < y);
            Assert.False(x <= y);
            Assert.True(x > y);
            Assert.True(x >= y);
        }

        [Fact]
        public void TestListOrdering5()
        {
            var x = List(1, 2, 3, 4);
            var y = List(1, 2, 3, 4, 5);

            Assert.True(x.CompareTo(y) < 0);
            Assert.True(x.CompareTo(y) <= 0);
            Assert.True(x < y);
            Assert.True(x <= y);
            Assert.False(x > y);
            Assert.False(x >= y);
        }



        [Fact]
        public void TestMapOrdering1()
        {
            var x = Map((1, 'a'), (2, 'a'), (3, 'a'), (4, 'a'), (5, 'a'));
            var y = Map((1, 'a'), (2, 'a'), (3, 'a'), (4, 'a'), (5, 'a'));

            Assert.True(x.CompareTo(y) == 0);
            Assert.False(x < y);
            Assert.True(x <= y);
            Assert.False(x > y);
            Assert.True(x >= y);
        }

        [Fact]
        public void TestMapOrdering2()
        {
            var x = Map((1, 'a'), (2, 'a'), (3, 'a'), (4, 'a'), (5, 'a'));
            var y = Map((1, 'a'), (2, 'a'), (3, 'a'), (4, 'a'), (6, 'a'));

            Assert.True(x.CompareTo(y) < 0);
            Assert.True(x.CompareTo(y) <= 0);
            Assert.True(x < y);
            Assert.True(x <= y);
            Assert.False(x > y);
            Assert.False(x >= y);
        }

        [Fact]
        public void TestMapOrdering3()
        {
            var x = Map((1, 'a'), (2, 'a'), (3, 'a'), (4, 'a'), (6, 'a'));
            var y = Map((1, 'a'), (2, 'a'), (3, 'a'), (4, 'a'), (5, 'a'));

            Assert.True(x.CompareTo(y) > 0);
            Assert.True(x.CompareTo(y) >= 0);
            Assert.False(x < y);
            Assert.False(x <= y);
            Assert.True(x > y);
            Assert.True(x >= y);
        }

        [Fact]
        public void TestMapOrdering4()
        {
            var x = Map((1, 'a'), (2, 'a'), (3, 'a'), (4, 'a'), (5, 'a'));
            var y = Map((1, 'a'), (2, 'a'), (3, 'a'), (4, 'a'));

            Assert.True(x.CompareTo(y) > 0);
            Assert.True(x.CompareTo(y) >= 0);
            Assert.False(x < y);
            Assert.False(x <= y);
            Assert.True(x > y);
            Assert.True(x >= y);
        }

        [Fact]
        public void TestMapOrdering5()
        {
            var x = Map((1, 'a'), (2, 'a'), (3, 'a'), (4, 'a'));
            var y = Map((1, 'a'), (2, 'a'), (3, 'a'), (4, 'a'), (5, 'a'));

            Assert.True(x.CompareTo(y) < 0);
            Assert.True(x.CompareTo(y) <= 0);
            Assert.True(x < y);
            Assert.True(x <= y);
            Assert.False(x > y);
            Assert.False(x >= y);
        }
    }
}
