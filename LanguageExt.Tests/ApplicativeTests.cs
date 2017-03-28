using LanguageExt.ClassInstances;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class ApplicativeTests
    {

        TryAsync<int> GetValueOne() =>
            TryAsync(2);

        TryAsync<int> GetValueTwo() =>
            TryAsync(() => 10);

        TryAsync<int> GetException() =>
            TryAsync<int>(() => throw new Exception());

        [Fact]
        public async void ApplyTryAsync2()
        {
            var op = apply(
                TryAsync<Func<int, int, int>>((x, y) => x + y),
                GetValueOne(),
                GetValueTwo()
            );

            var res = await op.IfFail(0);

            Assert.True(res == 12);
        }

        [Fact]
        public void ApplyTryAsync1()
        {
            var op = ApplTryAsync<int>.Inst.Apply(
                TryAsync<Func<int, Func<int, int>>>(x => y => x + y),
                GetValueOne(),
                GetValueTwo()
            );

            var res = op.IfFail(0).Result;

            Assert.True(res == 12);
        }


        [Fact]
        public void ApplyTryAsyncException1()
        {
            var op = apply(
                TryAsync<Func<int, int, int>>((x, y) => x + y),
                GetValueOne(),
                GetException()
            );

            var res = op.IfFail(0).Result;

            Assert.True(res == 0);
        }


        // Placeholder functions.  Imagine they're doing some work to get some remote
        // data lists.
        TryAsync<Lst<int>> GetListOneFromRemote() => TryAsync(List(1, 2, 3));
        TryAsync<Lst<int>> GetListTwoFromRemote() => TryAsync(List(4, 5, 6));

        // Combines two lists and sorts them.  Declared as a Func rather than a method
        // because the compiler can't infer TryAsync(CombineAndOrder) if the 
        // declaration is method.
        public static IEnumerable<int> CombineAndOrder(Lst<int> x, Lst<int> y) =>
            from item in (x + y)
            orderby item
            select item;

        // Uses the fact that TryAsync is an applicative, and therefore has the 
        // apply function available.  The apply function will run all three parts
        // of the applicative asynchronously, will handle errors from any term,
        // and will then apply them using the CombineAndOrder function.
        public TryAsync<IEnumerable<int>> GetRemoteListsAndCombine() =>
            apply(
                CombineAndOrder,
                GetListOneFromRemote(),
                GetListTwoFromRemote());

        [Fact]
        public async void ListCombineTest()
        {
            var res = await GetRemoteListsAndCombine().IfFail(Enumerable.Empty<int>());

            var arr = res.ToArray();

            Assert.True(arr[0] == 1);
            Assert.True(arr[1] == 2);
            Assert.True(arr[2] == 3);
            Assert.True(arr[3] == 4);
            Assert.True(arr[4] == 5);
            Assert.True(arr[5] == 6);
        }
    }

    class TestSigs1
    { 
        // Some example method prototypes
        public TryAsync<int> LongRunningAsyncTaskOp() => TryAsync(10);
        public Task<Try<int>> LongRunningAsyncOp()    => Task.FromResult(Try(10));
        public Try<int> LongRunningOp()               => Try(10);
        public Task<int> MapToTask(int x)             => Task.FromResult(x * 2);
        public int MapTo(int x)                       => x * 2;

        public async void Test()
        {
            var j = LongRunningOp().ToAsync();

            // These run synchronously
            int a = LongRunningOp().IfFail(0);
            Try<int> b = LongRunningOp().Map(MapTo);

            // These run asynchronously
            int u = await LongRunningAsyncTaskOp().IfFail(0);
            int v = await LongRunningAsyncOp().IfFail(0);
            int x = await LongRunningOp().IfFailAsync(0);
            TryAsync<int> y1 = LongRunningOp().MapAsync(MapTo);
            TryAsync<int> y2 = LongRunningOp().MapAsync(MapToTask);
            int z1 = await y1.IfFail(0);
            int z2 = await y2.IfFail(0);
        }
    }

    class TestSigs2
    {
        // Some example method prototypes
        public TryOptionAsync<int> LongRunningAsyncTaskOp() => TryOptionAsync(10);
        public Task<TryOption<int>> LongRunningAsyncOp()    => Task.FromResult(TryOption(10));
        public TryOption<int> LongRunningOp()               => TryOption(10);
        public Task<int> MapToTask(int x)                   => Task.FromResult(x * 2);
        public int MapTo(int x)                             => x * 2;

        public async void Test()
        {
            var j = LongRunningOp().ToAsync();
            var k = LongRunningAsyncOp().ToAsync();

            // These run synchronously
            int a = LongRunningOp().IfNoneOrFail(0);
            TryOption<int> b = LongRunningOp().Map(MapTo);

            // These run asynchronously
            int u = await LongRunningAsyncTaskOp().IfNoneOrFail(0);
            int v = await LongRunningAsyncOp().IfNoneOrFail(0);
            int x = await LongRunningOp().IfNoneOrFailAsync(0);
            TryOptionAsync<int> y1 = LongRunningOp().MapAsync(MapTo);
            TryOptionAsync<int> y2 = LongRunningOp().MapAsync(MapToTask);
            int z1 = await y1.IfNoneOrFail(0);
            int z2 = await y2.IfNoneOrFail(0);
        }
    }
}
