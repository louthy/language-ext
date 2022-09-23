using Xunit;
using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.List;

namespace LanguageExt.Tests
{
    public class ArrayTests
    {
        [Fact]
        public void ConsTest1()
        {
            var test = Prelude.Cons(1, Prelude.Cons(2, Prelude.Cons(3, Prelude.Cons(4, Prelude.Cons(5, LanguageExt.List.empty<int>())))));

            var array = test.ToArray();

            Assert.True(array[0] == 1);
            Assert.True(array[1] == 2);
            Assert.True(array[2] == 3);
            Assert.True(array[3] == 4);
            Assert.True(array[4] == 5);
        }

        [Fact]
        public void ListConstruct()
        {
            var test = Array(1, 2, 3, 4, 5);

            var array = test.ToArray();

            Assert.True(array[0] == 1);
            Assert.True(array[1] == 2);
            Assert.True(array[2] == 3);
            Assert.True(array[3] == 4);
            Assert.True(array[4] == 5);
        }

        [Fact]
        public void MapTestFluent()
        {
            var res = Array(1, 2, 3, 4, 5)
                        .Map(x => x * 10)
                        .Filter(x => x > 20)
                        .Fold(0, (x, s) => s + x);

            Assert.True(res == 120);
        }

        [Fact]
        public void ReduceTestFluent()
        {
            var res = Array(1, 2, 3, 4, 5)
                        .Map(x => x * 10)
                        .Filter(x => x > 20)
                        .Reduce((x, s) => s + x);

            Assert.True(res == 120);
        }

        [Fact]
        public void ReverseListTest1()
        {
            var list = Array(1, 2, 3, 4, 5);
            var rev = list.Reverse();

            Assert.True(rev[0] == 5);
            Assert.True(rev[4] == 1);
        }

        [Fact]
        public void ReverseListTest2()
        {
            var list = Array(1, 2, 3, 4, 5);
            var rev = list.Reverse();

            Assert.True(rev.IndexOf(1) == 4, "Should have been 4, actually is: "+ rev.IndexOf(1));
            Assert.True(rev.IndexOf(5) == 0, "Should have been 0, actually is: " + rev.IndexOf(5));
        }

        [Fact]
        public void ReverseListTest3()
        {
            var list = Array(1, 1, 2, 2, 2);
            var rev = list.Reverse();

            Assert.True(rev.LastIndexOf(1) == 4, "Should have been 4, actually is: " + rev.LastIndexOf(1));
            Assert.True(rev.LastIndexOf(2) == 2, "Should have been 2, actually is: " + rev.LastIndexOf(5));
        }

        [Fact]
        public void OpEqualTest()
        {
            var goodOnes = Array(
                Tuple(List(1, 2, 3), List(1, 2, 3)),
                Tuple(Lst<int>.Empty, Lst<int>.Empty)
            );
            var badOnes = Array(
                Tuple(List(1, 2, 3), List(1, 2, 4)),
                Tuple(List(1, 2, 3), Lst<int>.Empty)
            );

            goodOnes.Iter(t => t.Iter((fst, snd) =>
            {
                Assert.True(fst == snd, $"'{fst}' == '{snd}'");
                Assert.False(fst != snd, $"'{fst}' != '{snd}'");
            }));

            badOnes.Iter(t => t.Iter((fst, snd) =>
            {
                Assert.True(fst != snd, $"'{fst}' != '{snd}'");
                Assert.False(fst == snd, $"'{fst}' == '{snd}'");
            }));
        }


        [Fact]
        public void ArrShouldNotStackOverflowOnEquals()
        {
            Arr<Arr<double>> arr = default(Arr<Arr<double>>);

            Assert.True(arr.Equals(arr));
        }

        [Fact]
        public void EqualsTest()
        {
            Assert.False(Array(1, 2, 3).Equals(Array<int>()));
            Assert.False(Array<int>().Equals(Array<int>(1, 2, 3)));
            Assert.True(Array<int>().Equals(Array<int>()));
            Assert.True(Array<int>(1).Equals(Array<int>(1)));
            Assert.True(Array<int>(1, 2).Equals(Array<int>(1, 2)));
            Assert.False(Array<int>(1, 2).Equals(Array<int>(1, 2, 3)));
            Assert.False(Array<int>(1, 2, 3).Equals(Array<int>(1, 2)));
        }


        [Fact]
        public void itemLensGetShouldGetExistingValue()
        {
            var expected = "3";
            var array = Array("0", "1", "2", "3", "4", "5");
            var actual = Arr<string>.item(3).Get(array);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void itemLensGetShouldThrowExceptionForNonExistingValue()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var array = Array("0", "1", "2", "3", "4", "5");
                var actual = Arr<string>.item(10).Get(array);
            });
        }

        [Fact]
        public void itemOrNoneLensGetShouldGetExistingValue()
        {
            var expected = "3";
            var array = Array("0", "1", "2", "3", "4", "5");
            var actual = Arr<string>.itemOrNone(3).Get(array);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void itemOrNoneLensGetShouldReturnNoneForNonExistingValue()
        {
            var expected = Option<string>.None;
            var array = Array("0", "1", "2", "3", "4", "5");
            var actual = Arr<string>.itemOrNone(10).Get(array);

            Assert.Equal(expected, actual);
        }
    }
}
