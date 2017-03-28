using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt.Tests
{
    public class TraverseTests
    {
        [Fact]
        public void OptionList()
        {
            var result = traverse<MOption<int>, MLst<int>, Option<int>, Lst<int>, int, int>(
                Some(1),
                x => x % 2 == 0
                    ? List(2, 4, 6, 8, 10)
                    : List(1, 3, 5, 7, 9)
                );

            Assert.True(result == List(1, 3, 5, 7, 9));
        }


        [Fact]
        public void ListList()
        {
            var result = traverse<MLst<int>, MLst<int>, Lst<int>, Lst<int>, int, int>(
                List(1, 2),
                x => x % 2 == 0
                    ? List(2, 4, 6, 8, 10)
                    : List(1, 3, 5, 7, 9)
                );

            Assert.True(result == List(1, 3, 5, 7, 9, 2, 4, 6, 8, 10));
        }

        [Fact]
        public void TryAsyncList()
        {
            var result = traverse<MTryAsync<int>, MLst<int>, TryAsync<int>, Lst<int>, int, int>(
                TryAsync(() => 10),
                x => x % 2 == 0
                    ? List(2, 4, 6, 8, 10)
                    : List(1, 3, 5, 7, 9)
                );

            Assert.True(result == List(2, 4, 6, 8, 10));
        }

        [Fact]
        public async void ListTryAsync()
        {
            var result = traverse<MLst<int>, MTryAsync<string>, Lst<int>, TryAsync<string>, int, string>(
                List(1, 2, 3, 4),
                x => x % 2 == 0
                    ? TryAsync(() => { Thread.Sleep(1); return "even"; })
                    : TryAsync(() => { Thread.Sleep(3000); return "odd"; })
                );

            var res = await result.IfFail("failed");

            Assert.True(res == "even");
        }

        [Fact]
        public static void TraverseTest1()
        {
            var x = List(Some(1), Some(2), Some(3), Some(4), Some(5));

            var inst = Trans<MLst<Option<int>>, Lst<Option<int>>, MOption<int>, Option<int>, int>.Inst;

            var y = inst.Sequence<MOption<Lst<int>>, Option<Lst<int>>, MLst<int>, Lst<int>>(x);

            Assert.True(y == Some(List(1, 2, 3, 4, 5)));
        }

        [Fact]
        public static void TraverseTest2()
        {
            var x = List(Some(1), Some(2), Some(3), None, Some(5));

            var inst = Trans<MLst<Option<int>>, Lst<Option<int>>, MOption<int>, Option<int>, int>.Inst;

            var y = inst.Sequence<MOption<Lst<int>>, Option<Lst<int>>, MLst<int>, Lst<int>>(x);

            Assert.True(y == None);
        }

        [Fact]
        public static void TraverseTest3()
        {
            var x = List(Some(1), Some(2), Some(3), Some(4), Some(5));

            var y = x.Traverse(identity);

            Assert.True(y == Some(List(1, 2, 3, 4, 5)));
        }

        [Fact]
        public static void TraverseTest4()
        {
            var x = List(Some(1), Some(2), Some(3), None, Some(5));

            var y = x.Sequence();

            Assert.True(y == None);
        }

        [Fact]
        public static async void TraverseAsync()
        {
            var start = DateTime.UtcNow;

            var f1 = Task.Run(() => { Thread.Sleep(3000); return 1; });
            var f2 = Task.Run(() => { Thread.Sleep(3000); return 2; });
            var f3 = Task.Run(() => { Thread.Sleep(3000); return 3; });
            var f4 = Task.Run(() => { Thread.Sleep(3000); return 4; });
            var f5 = Task.Run(() => { Thread.Sleep(3000); return 5; });
            var f6 = Task.Run(() => { Thread.Sleep(3000); return 6; });

            var res = await List(f1, f2, f3, f4, f5, f6).Traverse(x => x * 2);

            Assert.True(Set.createRange(res) == Set(2, 4, 6, 8, 10, 12));

            var ms = (int)(DateTime.UtcNow - start).TotalMilliseconds;
            Assert.True(ms < 3500, $"Took {ms} ticks");
        }

        [Fact]
        public static void EitherList1()
        {
            var x = Right<Exception, Lst<int>>(List(1, 2, 3, 4, 5));

            var y = x.Sequence();

            var expect = List(Right<Exception, int>(1), Right<Exception, int>(2), Right<Exception, int>(3), Right<Exception, int>(4), Right<Exception, int>(5));

            Assert.True(y == expect);
        }

        [Fact]
        public static void EitherList2()
        {
            var x = Left<string, Lst<int>>("error");

            var y = x.Sequence();

            Assert.True(y.Count == 1 && y[0].IsBottom);
        }

        [Fact]
        public static void ListEither1()
        {
            var x = List<Either<Exception, int>>(1, 2, 3, 4, 5);

            var y = x.Sequence();
            Assert.True(y == List(1, 2, 3, 4, 5));
        }

        [Fact]
        public static void ListEither2()
        {
            var x = List<Either<string, int>>(1, 2, 3, 4, "error");

            var y = x.Sequence();

            Assert.True(y.IsLeft && y == "error");
        }

    }
}
