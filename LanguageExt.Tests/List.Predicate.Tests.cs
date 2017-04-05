using LanguageExt.ClassInstances.Pred;
using static LanguageExt.Prelude;
using Xunit;
using System;
using LanguageExt.ClassInstances.Const;

namespace LanguageExt.Tests
{
    public class ListPredicateTests
    {
        [Fact]
        public void UninitialisedEmptyListThrows()
        {
            var x = new Lst<NonEmpty, int>();

            Assert.Throws<BottomException>(() =>
            {
                var u = x.Count;
            });
        }

        [Fact]
        public void EmptyListThrows()
        {
            var x = List<NonEmpty, int>(1, 2, 3, 4);

            x = x.RemoveAt(0);
            x = x.RemoveAt(0);
            x = x.RemoveAt(0);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                x = x.RemoveAt(0);
            });
        }

        [Fact]
        public void RangeListThrows()
        {
            var x = List<MaxCount<I5>, int>(1, 2, 3, 4);

            x = x.Add(5);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                x = x.Add(6);
            });
        }

        [Fact]
        public void NoNullItems()
        {
            var x = List<AnySize, NonNullItems<string>, string>("one", "two", "three");

            x = x.Add("four");

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                x = x.Add(null);
            });
        }
    }
}
