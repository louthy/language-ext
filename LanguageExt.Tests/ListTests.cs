using System;
using System.Linq;
using NU = NUnit.Framework;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.List;

namespace LanguageExtTests
{
    [NU.TestFixture]
    public class ListTests
    {
        [NU.Test]
        public void ConsTest1()
        {
            var test = Prelude.Cons(1, Prelude.Cons(2, Prelude.Cons(3, Prelude.Cons(4, Prelude.Cons(5, LanguageExt.List.empty<int>())))));

            var array = test.ToArray();

            NU.Assert.IsTrue(array[0] == 1);
            NU.Assert.IsTrue(array[1] == 2);
            NU.Assert.IsTrue(array[2] == 3);
            NU.Assert.IsTrue(array[3] == 4);
            NU.Assert.IsTrue(array[4] == 5);
        }
        
        [NU.Test]
        public void ListConstruct()
        {
            var test = List(1, 2, 3, 4, 5);

            var array = test.ToArray();

            NU.Assert.IsTrue(array[0] == 1);
            NU.Assert.IsTrue(array[1] == 2);
            NU.Assert.IsTrue(array[2] == 3);
            NU.Assert.IsTrue(array[3] == 4);
            NU.Assert.IsTrue(array[4] == 5);
        }

        [NU.Test]
        public void MapTest()
        {
            // Generates 10,20,30,40,50
            var input = List(1, 2, 3, 4, 5);
            var output1 = map(input, x => x * 10);

            // Generates 30,40,50
            var output2 = filter(output1, x => x > 20);

            // Generates 120
            var output3 = fold(output2, 0, (x, s) => s + x);

            NU.Assert.IsTrue(output3 == 120);
        }

        [NU.Test]
        public void ReduceTest()
        {
            // Generates 10,20,30,40,50
            var input = List(1, 2, 3, 4, 5);
            var output1 = map(input, x => x * 10);

            // Generates 30,40,50
            var output2 = filter(output1, x => x > 20);

            // Generates 120
            var output3 = reduce(output2, (x, s) => s + x);

            NU.Assert.IsTrue(output3 == 120);
        }

        [NU.Test]
        public void MapTestFluent()
        {
            var res = List(1, 2, 3, 4, 5)
                        .Map(x => x * 10)
                        .Filter(x => x > 20)
                        .Fold(0, (x, s) => s + x);

            NU.Assert.IsTrue(res == 120);
        }

        [NU.Test]
        public void ReduceTestFluent()
        {
            var res = List(1, 2, 3, 4, 5)
                        .Map(x => x * 10)
                        .Filter(x => x > 20)
                        .Reduce((x, s) => s + x);

            NU.Assert.IsTrue(res == 120);
        }

        [NU.Test]
        public void RangeTest1()
        {
            var r = Range(0, 10).AsEnumerable();
            for (int i = 0; i < 10; i++)
            {
                NU.Assert.IsTrue(r.First() == i);
                r = r.Skip(1);
            }
        }

        [NU.Test]
        public void RangeTest2()
        {
            var r = Range(0, 100, 10).AsEnumerable();
            for (int i = 0; i < 10; i+=10)
            {
                NU.Assert.IsTrue(r.First() == i);
                r = r.Skip(1);
            }
        }

        [NU.Test]
        public void RangeTest3()
        {
            var r = Range(Range(0, 5), Range(10, 20));

            for (int i = 0; i < 5; i ++)
            {
                NU.Assert.IsTrue(r.First() == i);
                r = r.Skip(1);
            }

            for (int i = 0; i < 10; i++)
            {
                NU.Assert.IsTrue(r.First() == i + 10);
                r = r.Skip(1);
            }
        }

        [NU.Test]
        public void RangeTest4()
        {
            var r = Range('a', 'f');
            NU.Assert.IsTrue(String.Join("", r) == "abcdef");
        }

        [NU.Test]
        public void RangeTest5()
        {
            var r = Range('f', 'a');
            NU.Assert.IsTrue(String.Join("", r) == "fedcba");
        }

        [NU.Test]
        public void RepeatTest()
        {
            var r = repeat("Hello", 10);

            foreach (var item in r)
            {
                NU.Assert.IsTrue(item == "Hello");
            }
        }


        [NU.Test]
        public void InitTest()
        {
            var r = init(10, i => "Hello " + i );

            for (int i = 0; i < 10; i++)
            {
                NU.Assert.IsTrue(r.First() == "Hello "+i);
                r = r.Skip(1);
            }
        }

        [NU.Test]
        public void UnfoldTest()
        {
            var test = List(0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181);

            var fibs = take(unfold(Tuple(0, 1), tup => map(tup, (a, b) => Some(Tuple(a, Tuple(b, a + b))))), 20);

            NU.Assert.IsTrue( test.SequenceEqual(fibs) );
        }

        [NU.Test]
        public void UnfoldTupleTest()
        {
            var test = List(0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181);

            var fibs = take( unfold( Tuple(0, 1), (a, b) => Some(Tuple(a, b, a + b)) ), 20);

            NU.Assert.IsTrue(test.SequenceEqual(fibs));
        }

        [NU.Test]
        public void UnfoldSingleTest()
        {
            var e = new Exception("Outer", new Exception("Inner"));

            var list = unfold(e, (state) =>
                           state == null
                               ? None
                               : Optional(state.InnerException)
                               );

            var res = list.ToList();

            NU.Assert.IsTrue(res[0].Message == "Outer" && res[1].Message == "Inner");
        }

        [NU.Test]
        public void ReverseListTest1()
        {
            var list = List(1, 2, 3, 4, 5);
            var rev = list.Rev();

            NU.Assert.IsTrue(rev[0] == 5);
            NU.Assert.IsTrue(rev[4] == 1);
        }

        [NU.Test]
        public void ReverseListTest2()
        {
            var list = List(1, 2, 3, 4, 5);
            var rev = list.Rev();

            NU.Assert.IsTrue(rev.IndexOf(1) == 4, "Should have been 4, actually is: "+ rev.IndexOf(1));
            NU.Assert.IsTrue(rev.IndexOf(5) == 0, "Should have been 0, actually is: " + rev.IndexOf(5));
        }

        [NU.Test]
        public void ReverseListTest3()
        {
            var list = List(1, 1, 2, 2, 2);
            var rev = list.Rev();

            NU.Assert.IsTrue(rev.LastIndexOf(1) == 4, "Should have been 4, actually is: " + rev.LastIndexOf(1));
            NU.Assert.IsTrue(rev.LastIndexOf(2) == 2, "Should have been 2, actually is: " + rev.LastIndexOf(5));
        }
    }
}
