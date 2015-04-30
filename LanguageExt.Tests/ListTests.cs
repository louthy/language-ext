using System;
using System.Linq;
using NUnit.Framework;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.List;

namespace LanguageExtTests
{
    [TestFixture]
    public class ListTests
    {

        [Test]
        public void ConsTest1()
        {
            var test = Prelude.cons(1, Prelude.cons(2, Prelude.cons(3, Prelude.cons(4, Prelude.cons(5, empty<int>())))));

            var array = test.ToArray();

            Assert.IsTrue(array[0] == 1);
            Assert.IsTrue(array[1] == 2);
            Assert.IsTrue(array[2] == 3);
            Assert.IsTrue(array[3] == 4);
            Assert.IsTrue(array[4] == 5);
        }


        
        [Test]
        public void ListConstruct()
        {
            var test = list(1, 2, 3, 4, 5);

            var array = test.ToArray();

            Assert.IsTrue(array[0] == 1);
            Assert.IsTrue(array[1] == 2);
            Assert.IsTrue(array[2] == 3);
            Assert.IsTrue(array[3] == 4);
            Assert.IsTrue(array[4] == 5);
        }

        [Test]
        public void MapTest()
        {
            // Generates 10,20,30,40,50
            var input = list(1, 2, 3, 4, 5);
            var output1 = map(input, x => x * 10);

            // Generates 30,40,50
            var output2 = filter(output1, x => x > 20);

            // Generates 120
            var output3 = fold(output2, 0, (x, s) => s + x);

            Assert.IsTrue(output3 == 120);
        }

        [Test]
        public void ReduceTest()
        {
            // Generates 10,20,30,40,50
            var input = list(1, 2, 3, 4, 5);
            var output1 = map(input, x => x * 10);

            // Generates 30,40,50
            var output2 = filter(output1, x => x > 20);

            // Generates 120
            var output3 = reduce(output2, (x, s) => s + x);

            Assert.IsTrue(output3 == 120);
        }

        [Test]
        public void MapTestFluent()
        {
            var res = list(1, 2, 3, 4, 5)
                        .Map(x => x * 10)
                        .Filter(x => x > 20)
                        .Fold(0, (x, s) => s + x);

            Assert.IsTrue(res == 120);
        }

        [Test]
        public void ReduceTestFluent()
        {
            var res = list(1, 2, 3, 4, 5)
                        .Map(x => x * 10)
                        .Filter(x => x > 20)
                        .Reduce((x, s) => s + x);

            Assert.IsTrue(res == 120);
        }

        [Test]
        public void RangeTest1()
        {
            var r = range(0, 10).AsEnumerable();
            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(r.First() == i);
                r = r.Skip(1);
            }
        }

        [Test]
        public void RangeTest2()
        {
            var r = range(0, 100, 10).AsEnumerable();
            for (int i = 0; i < 10; i+=10)
            {
                Assert.IsTrue(r.First() == i);
                r = r.Skip(1);
            }
        }

        [Test]
        public void RangeTest3()
        {
            var r = range(range(0, 5), range(10, 20));

            for (int i = 0; i < 5; i ++)
            {
                Assert.IsTrue(r.First() == i);
                r = r.Skip(1);
            }

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(r.First() == i + 10);
                r = r.Skip(1);
            }
        }

        [Test]
        public void RangeTest4()
        {
            var r = range('a', 'f');
            Assert.IsTrue(String.Join("", r) == "abcdef");
        }

        [Test]
        public void RangeTest5()
        {
            var r = range('f', 'a');
            Assert.IsTrue(String.Join("", r) == "fedcba");
        }

        [Test]
        public void RepeatTest()
        {
            var r = repeat("Hello", 10);

            foreach (var item in r)
            {
                Assert.IsTrue(item == "Hello");
            }
        }


        [Test]
        public void InitTest()
        {
            var r = init(10, i => "Hello " + i );

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(r.First() == "Hello "+i);
                r = r.Skip(1);
            }
        }

        [Test]
        public void UnfoldTest()
        {
            var test = list(0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181);

            var fibs = take(unfold(tuple(0, 1), tup => map(tup, (a, b) => Some(tuple(a, tuple(b, a + b))))), 20);

            Assert.IsTrue( test.SequenceEqual(fibs) );
        }

        [Test]
        public void UnfoldTupleTest()
        {
            var test = list(0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181);

            var fibs = take( unfold( tuple(0, 1), (a, b) => Some(tuple(a, b, a + b)) ), 20);

            Assert.IsTrue(test.SequenceEqual(fibs));
        }

        [Test]
        public void UnfoldSingleTest()
        {
            var e = new Exception("Outer", new Exception("Inner"));

            var list = unfold(e, (state) =>
                           state == null
                               ? None
                               : Optional(state.InnerException)
                               );

            var res = list.ToList();

            Assert.IsTrue(res[0].Message == "Outer" && res[1].Message == "Inner");
        }
    }
}
