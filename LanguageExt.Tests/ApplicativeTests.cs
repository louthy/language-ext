using LanguageExt.ClassInstances;
using System;
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
            var op = FTryAsync<int>.Inst.Apply(
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
        var k = LongRunningAsyncOp().ToAsync();

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
