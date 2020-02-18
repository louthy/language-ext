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

        [Fact]
        public void ImplicitConversionTest1()
        {
            var res = Product(List(1, 2, 3, 4, 5));

            Assert.True(res == 120);
        }

        [Fact]
        public void ImplicitConversionTest1Fail()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var res = Product(List<int>());
            });
        }

        [Fact]
        public void ImplicitConversionTest2()
        {
            var res = Divify(List("1", "2", "3"));

            Assert.True(res == "<div>1</div><div>2</div><div>3</div>");
        }

        [Fact]
        public void ImplicitConversionTest2Fail1()
        {
            Assert.Throws<InvalidCastException>(() =>
            {
                var res = Divify(List("1", "2", null));
            });
        }

        [Fact]
        public void ImplicitConversionTest2Fail2()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var res = Divify(List<string>());
            });
        }

        static int Product(Lst<NonEmpty, int> list) =>
            list.Fold(1, (s, x) => s * x);

        public string Divify(Lst<NonEmpty, NonNullItems<string>, string> items) =>
            String.Join("", items.Map(x => $"<div>{x}</div>"));
    }
}
