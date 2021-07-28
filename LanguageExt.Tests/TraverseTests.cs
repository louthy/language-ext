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
        public async Task TryAsyncList()
        {
            var result = await traverseAsyncSync<MTryAsync<int>, MLst<int>, TryAsync<int>, Lst<int>, int, int>(
                TryAsync(10.AsTask()),
                x => x % 2 == 0
                    ? List(2, 4, 6, 8, 10)
                    : List(1, 3, 5, 7, 9)
                );

            Assert.True(result == List(2, 4, 6, 8, 10));
        }

        [Fact]
        public async void ListTryAsync()
        {
            var result = traverseSyncAsync<MLst<int>, MTryAsync<string>, Lst<int>, TryAsync<string>, int, string>(
                List(1, 2, 3, 4),
                x => x % 2 == 0
                    ? TryAsync(async () => { await Task.Delay(1); return "even"; })
                    : TryAsync(async () => { await Task.Delay(100); return "odd"; })
                );

            var res = await result.IfFail("failed");

            Assert.True(res == "odd");
        }

        [Fact]
        public async void ListTryFirstAsync()
        {
            var result = traverseSyncAsync<MLst<int>, MTryFirstAsync<string>, Lst<int>, TryAsync<string>, int, string>(
                List(1, 2, 3, 4),
                x => x % 2 == 0
                    ? TryAsync(async () => { await Task.Delay(1); return "even"; })
                    : TryAsync(async () => { await Task.Delay(100); return "odd"; })
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
            var fs = Range(1, SysInfo.DefaultAsyncSequenceParallelism).Map(x => Task.Run(() =>
            {
                Thread.Sleep(3000);
                return x;
            }));
 
            var res = await fs.Freeze().Traverse(x => x * 2);
            var ms = (int)(DateTime.UtcNow - start).TotalMilliseconds;

            Assert.True(Set.createRange(res) == toSet(Range(2, SysInfo.DefaultAsyncSequenceParallelism, 2)));
            Assert.True(ms < 4000, $"Took {ms} ticks");
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

        [Fact]
        public static void SeqValidation1()
        {
            var x = Seq<Validation<string, int>>(1, 2, "error 1", 3, 4, "error 2");

            var y = x.Sequence();

            Assert.True(y.IsFail && y == Seq("error 1", "error 2"));

            y.Match(
                Succ: s => Assert.True(false),
                Fail: e =>
                {
                    Assert.True(e.Head == "error 1");
                    Assert.True(e.Tail.Head == "error 2");
                });
        }

        [Fact]
        public static void SeqValidation2()
        {
            var x = Seq<Validation<string, int>>(1, 2, 3, 4, 5);

            var y = x.Sequence();

            Assert.True(y.IsSuccess && y == Seq(1, 2, 3, 4, 5));
        }
    }
}
