using Xunit;
using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.List;

namespace LanguageExtTests
{
    public class ListTests
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
            var test = List(1, 2, 3, 4, 5);

            var array = test.ToArray();

            Assert.True(array[0] == 1);
            Assert.True(array[1] == 2);
            Assert.True(array[2] == 3);
            Assert.True(array[3] == 4);
            Assert.True(array[4] == 5);
        }

        [Fact]
        public void MapTest()
        {
            // Generates 10,20,30,40,50
            var input = List(1, 2, 3, 4, 5);
            var output1 = map(input, x => x * 10);

            // Generates 30,40,50
            var output2 = filter(output1, x => x > 20);

            // Generates 120
            var output3 = fold(output2, 0, (x, s) => s + x);

            Assert.True(output3 == 120);
        }

        [Fact]
        public void ReduceTest()
        {
            // Generates 10,20,30,40,50
            var input = List(1, 2, 3, 4, 5);
            var output1 = map(input, x => x * 10);

            // Generates 30,40,50
            var output2 = filter(output1, x => x > 20);

            // Generates 120
            var output3 = reduce(output2, (x, s) => s + x);

            Assert.True(output3 == 120);
        }

        [Fact]
        public void MapTestFluent()
        {
            var res = List(1, 2, 3, 4, 5)
                        .Map(x => x * 10)
                        .Filter(x => x > 20)
                        .Fold(0, (x, s) => s + x);

            Assert.True(res == 120);
        }

        [Fact]
        public void ReduceTestFluent()
        {
            var res = List(1, 2, 3, 4, 5)
                        .Map(x => x * 10)
                        .Filter(x => x > 20)
                        .Reduce((x, s) => s + x);

            Assert.True(res == 120);
        }

        [Fact]
        public void RangeTest1()
        {
            var r = Range(0, 10).AsEnumerable();
            for (int i = 0; i < 10; i++)
            {
                Assert.True(r.First() == i);
                r = r.Skip(1);
            }
        }

        [Fact]
        public void RangeTest2()
        {
            var r = Range(0, 100, 10).AsEnumerable();
            for (int i = 0; i < 10; i+=10)
            {
                Assert.True(r.First() == i);
                r = r.Skip(1);
            }
        }

        [Fact]
        public void RangeTest3()
        {
            var r = Range(Range(0, 5), Range(10, 20));

            for (int i = 0; i < 5; i ++)
            {
                Assert.True(r.First() == i);
                r = r.Skip(1);
            }

            for (int i = 0; i < 10; i++)
            {
                Assert.True(r.First() == i + 10);
                r = r.Skip(1);
            }
        }

        [Fact]
        public void RangeTest4()
        {
            var r = Range('a', 'f');
            Assert.True(String.Join("", r) == "abcdef");
        }

        [Fact]
        public void RangeTest5()
        {
            var r = Range('f', 'a');
            Assert.True(String.Join("", r) == "fedcba");
        }

        [Fact]
        public void RepeatTest()
        {
            var r = repeat("Hello", 10);

            foreach (var item in r)
            {
                Assert.True(item == "Hello");
            }
        }


        [Fact]
        public void InitTest()
        {
            var r = init(10, i => "Hello " + i );

            for (int i = 0; i < 10; i++)
            {
                Assert.True(r.First() == "Hello "+i);
                r = r.Skip(1);
            }
        }

        [Fact]
        public void UnfoldTest()
        {
            var test = List(0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181);

            var fibs = take(unfold(Tuple(0, 1), tup => map(tup, (a, b) => Some(Tuple(a, Tuple(b, a + b))))), 20);

            Assert.True( test.SequenceEqual(fibs) );
        }

        [Fact]
        public void UnfoldTupleTest()
        {
            var test = List(0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181);

            var fibs = take( unfold( Tuple(0, 1), (a, b) => Some(Tuple(a, b, a + b)) ), 20);

            Assert.True(test.SequenceEqual(fibs));
        }

        [Fact]
        public void UnfoldSingleTest()
        {
            var e = new Exception("Outer", new Exception("Inner"));

            var list = unfold(e, (state) =>
                           state == null
                               ? None
                               : Optional(state.InnerException)
                               );

            var res = list.ToList();

            Assert.True(res[0].Message == "Outer" && res[1].Message == "Inner");
        }

        [Fact]
        public void ReverseListTest1()
        {
            var list = List(1, 2, 3, 4, 5);
            var rev = list.Rev();

            Assert.True(rev[0] == 5);
            Assert.True(rev[4] == 1);
        }

        [Fact]
        public void ReverseListTest2()
        {
            var list = List(1, 2, 3, 4, 5);
            var rev = list.Rev();

            Assert.True(rev.IndexOf(1) == 4, "Should have been 4, actually is: "+ rev.IndexOf(1));
            Assert.True(rev.IndexOf(5) == 0, "Should have been 0, actually is: " + rev.IndexOf(5));
        }

        [Fact]
        public void ReverseListTest3()
        {
            var list = List(1, 1, 2, 2, 2);
            var rev = list.Rev();

            Assert.True(rev.LastIndexOf(1) == 4, "Should have been 4, actually is: " + rev.LastIndexOf(1));
            Assert.True(rev.LastIndexOf(2) == 2, "Should have been 2, actually is: " + rev.LastIndexOf(5));
        }

        [Fact]
        public void OpEqualTest()
        {
            var goodOnes = List(
                Tuple(List(1, 2, 3), List(1, 2, 3)),
                Tuple(Lst<int>.Empty, Lst<int>.Empty),
                Tuple((Lst<int>)null, (Lst<int>)null)
            );
            var badOnes = List(
                Tuple(List(1, 2, 3), List(1, 2, 4)),
                Tuple(List(1, 2, 3), Lst<int>.Empty),
                Tuple(List(1, 2, 3), (Lst<int>)null)
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
        public void DistinctTest1()
        {
            var lst = List(Tuple(1, 1), Tuple(1, 9), Tuple(9, 9));

            Assert.True(lst.Distinct((a, b) => a.Item1 == b.Item1).Count() == 2);
            Assert.True(lst.Distinct((a, b) => a.Item2 == b.Item2).Count() == 2);

            Assert.True(lst.Distinct(o => o.Item1).Count() == 2);
            Assert.True(lst.Distinct(o => o.Item1).Count() == 2);
        }

        [Fact]
        public void DistinctTest2()
        {
            var lst = List(Tuple(1, 1), Tuple(1, 9), Tuple(9, 9));

            var dlist = lst.Distinct(o => o.Item1).ToList();
            
            Assert.True(dlist[0].Item1 == 1 && dlist[0].Item2 == 1);
            Assert.True(dlist[1].Item1 == 9 && dlist[1].Item2 == 9);
        }

        [Fact]
        public void Issue180Tests1()
        {
            var x = List("Chorizo", "Avocado", "Feta", "Banana", "Pineapple", "Bacon");
            var y = x.RemoveAt(0);
            var z = List("Avocado", "Feta", "Banana", "Pineapple", "Bacon");
            Assert.True(y == z);
        }

        [Fact]
        public void Issue180Tests2()
        {
            var x = List("Chorizo", "Avocado", "Feta", "Banana", "Pineapple", "Bacon");
            var y = x.RemoveAt(1);
            var z = List("Chorizo", "Feta", "Banana", "Pineapple", "Bacon");
            Assert.True(y == z);
        }

        [Fact]
        public void Issue180Tests3()
        {
            var x = List("Chorizo", "Avocado", "Feta", "Banana", "Pineapple", "Bacon");
            var y = x.RemoveAt(3);
            var z = List("Chorizo", "Avocado", "Feta", "Pineapple", "Bacon");
            Assert.True(y == z);
        }
    }
}
