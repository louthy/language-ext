using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class OptionAsyncTests
    {
        /// <summary>
        /// TODO: This is just some initial tests, will need a full suite
        /// </summary>
        [Fact]
        public async void InitialTests()
        {
            var ma = SomeAsync(10);
            var mb = SomeAsync(20);

            var mr = from a in ma
                     from b in mb
                     select a + b;

            Assert.True(await mr.IfNone(0) == 30);


            var mc = Some(10).ToAsync().MatchAsync(
                Some: x => Task.FromResult(x * 10),
                None: () => 0
            );

            Assert.True(await mc == 100);


            Option<int> opt = 4;
            Unit unit = await opt.ToAsync().IfSome(i => DoWork());
        }

        [Fact]
        public async void InitialTests2()
        {
            var mc = Some(10).MatchAsync(
                Some: x => Task.FromResult(x * 10),
                None: () => 0
            );

            Assert.True(await mc == 100);

            Option<int> opt = 4;
            Unit unit = await opt.IfSomeAsync(i => DoWork());
        }

        [Fact]
        public async void InitialTests3()
        {
            var mc = Some(10).AsTask().MatchAsync(
                Some: x => Task.FromResult(x * 10),
                None: () => 0
            );

            Assert.True(await mc == 100);
        }
        
        [Fact]
        public async Task InitialTests4()
        {
            var someString = wrap("string");
            var noneString = wrap(null);

            Assert.True(await someString.IsSome);
            await someString.IfSome(v => Assert.Equal("string", v));

            Assert.True(await noneString.IsNone);
        }

        private static OptionAsync<string> wrap(string str) => Task.FromResult(str);

        private Task DoWork()
        {
            return Task.Run(() => Console.WriteLine("here"));
        }

        [Fact]
        public async void Issue206()
        {
            var sync = new object();
            var tasks = new List<Task>();
            var output = new List<string>();

            Console.WriteLine("OptionAsync");
            OptionAsync<int> optAsync = Some(4).ToAsync();
            var tskOptionAsync = optAsync.IfSome(async (i) =>
            {
                await Task.Delay(100);
                var x = DoWork();

                lock (sync)
                {
                    tasks.Add(x);
                    output.Add($"Inner id {x.Id}");
                }

                await x;
            });
            lock (sync)
            {
                output.Add($"Outer id {tskOptionAsync.Id}");
                tasks.Add(tskOptionAsync);
            }
            await tskOptionAsync;

            Assert.Equal(TaskStatus.RanToCompletion, tasks[0].Status);
            Assert.Equal(TaskStatus.RanToCompletion, tasks[1].Status);
        }

        [Fact]
        public async Task NoneTest()
        {
            var x = OptionAsync<int>.None;
            Assert.True(await x.IsNone);
        }

        [Fact]
        public async void FluentSomeNoneTest()
        {
            var res1 = await GetValue(true)
                .Some(x => x + 10)
                .None(0);

            var res2 = await GetValue(false)
                .Some(x => x + 10)
                .None(() => 0);

            var res3 = -1;
            await GetValue(true)
                .Some(x => { res3 = x + 10; })
                .None(() => { res3 = 0; });

            var res4 = -1;
            await GetValue(false)
                .Some(x => { res4 = x + 10; })
                .None(() => { res4 = 0; });

            Assert.True(res1 == 1010);
            Assert.True(res2 == 0);
            Assert.True(res3 == 1010);
            Assert.True(res4 == 0);
        }

        [Fact]
        public async Task MapTest()
        {
            var x = OptionAsync<int>.Some(1);
            Assert.True(await x.Select(i => i * 2).Exists(j => j == 2));
            Assert.True(await x.Map(i => i * 2).Exists(j => j == 2));

            var y = OptionAsync<int>.None;
            Assert.False(await y.Select(i => 2).Exists(j => j == 2));
            Assert.False(await y.Map(i => 2).Exists(j => j == 2));
        }

        [Fact]
        public async Task BindTest()
        {
            var x = OptionAsync<int>.Some(1);
            Assert.True(await x.SelectMany(i => SomeAsync(i * 2), (i, j) => j).Exists(j => j == 2));
            Assert.True(await x.Bind(i => SomeAsync(i * 2)).Exists(j => j == 2));

            var y = OptionAsync<int>.None;
            Assert.False(await y.SelectMany(i => SomeAsync(2), (i, j) => j).Exists(j => true));
            Assert.False(await y.Bind(i => SomeAsync(2)).Exists(j => true));
        }

        [Fact]
        public async Task BiExistsTest()
        {
            Assert.True(await OptionAsync<int>.Some(1).BiExists(i => i == 1, () => false));
            Assert.False(await OptionAsync<int>.Some(1).BiExists(i => i != 1, () => true));
            Assert.False(await OptionAsync<int>.None.BiExists(i => i == 1, () => false));
            Assert.True(await OptionAsync<int>.None.BiExists(i => i != 1, () => true));
        }

        [Fact]
        public async Task ExistsAsyncTest()
        {
            Assert.True(await OptionAsync<int>.Some(1).ExistsAsync(i => Task.FromResult(i == 1)));
            Assert.False(await OptionAsync<int>.None.ExistsAsync(i => Task.FromResult(true)));
        }

        [Fact]
        public async Task FilterTest()
        {
            Assert.True(await OptionAsync<int>.Some(1).Filter(i => i == 1).IsSome);
            Assert.True(await OptionAsync<int>.Some(1).FilterAsync(i => Task.FromResult(i == 1)).IsSome);
            Assert.False(await OptionAsync<int>.Some(2).Filter(i => i == 1).IsSome);
            Assert.False(await OptionAsync<int>.Some(2).FilterAsync(i => Task.FromResult(i == 1)).IsSome);
            Assert.False(await OptionAsync<int>.None.Filter(i => true).IsSome);
            Assert.False(await OptionAsync<int>.None.FilterAsync(i => Task.FromResult(true)).IsSome);

            Assert.True(await OptionAsync<int>.Some(1).Where(i => i == 1).IsSome);
            Assert.False(await OptionAsync<int>.Some(2).Where(i => i == 1).IsSome);
            Assert.False(await OptionAsync<int>.None.Where(i => true).IsSome);
        }

        [Fact]
        public async Task ParMapTest()
        {
            var multiply = fun<int, int, int>((x, y) => x * y);
            var oDoubler = SomeAsync(2).ParMap(multiply);
            var oFour = oDoubler.Map(doubler => doubler(2));
            Assert.True(await oFour.Exists(i => i == 4));
        }

        [Fact]
        public async Task MOptionAsync_BiFold_IsSome_DelegateSomeExecuted()
        {
            var subject = MOptionAsync<Unit>.Inst;
            var ma = Some(unit).AsTask().ToAsync();
            var result = await subject.BiFold(ma, true,
                (s, m) => true,
                (s, _) => false);
            Assert.True(result);
        }

        [Fact]
        public async Task MOptionAsync_BiFoldAsync_OverloadSyncAsync_IsSome_DelegateSomeExecuted()
        {
            var subject = MOptionAsync<Unit>.Inst;
            var ma = Some(unit).AsTask().ToAsync();
            var result = await subject.BiFoldAsync(ma, true,
                (s, m) => true,
                (s, _) => false.AsTask());
            Assert.True(result);
        }

        [Fact]
        public async Task MOptionAsync_BiFoldAsync_OverloadAsyncSync_IsSome_DelegateSomeExecuted()
        {
            var subject = MOptionAsync<Unit>.Inst;
            var ma = Some(unit).AsTask().ToAsync();
            var result = await subject.BiFoldAsync(ma, true,
                (s, m) => true.AsTask(),
                (s, _) => false);
            Assert.True(result);
        }

        [Fact]
        public async Task MOptionAsync_BiFoldAsync_OverloadAsyncAsync_IsSome_DelegateSomeExecuted()
        {
            var subject = MOptionAsync<Unit>.Inst;
            var ma = Some(unit).AsTask().ToAsync();
            var result = await subject.BiFoldAsync(ma, true,
                (s, m) => true.AsTask(),
                (s, _) => false.AsTask());
            Assert.True(result);
        }

        [Fact]
        public async Task MOptionAsync_BiFold_IsNone_DelegateNoneExecuted()
        {
            var subject = MOptionAsync<Unit>.Inst;
            var ma = Option<Unit>.None.AsTask().ToAsync();
            var result = await subject.BiFold(ma, true,
                (s, m) => false,
                (s, _) => true);
            Assert.True(result);
        }

        [Fact]
        public async Task MOptionAsync_BiFoldAsync_OverloadSyncAsync_IsNone_DelegateNoneExecuted()
        {
            var subject = MOptionAsync<Unit>.Inst;
            var ma = Option<Unit>.None.AsTask().ToAsync();
            var result = await subject.BiFoldAsync(ma, true,
                (s, m) => false,
                (s, _) => true.AsTask());
            Assert.True(result);
        }

        [Fact]
        public async Task MOptionAsync_BiFoldAsync_OverloadAsyncSync_IsNone_DelegateNoneExecuted()
        {
            var subject = MOptionAsync<Unit>.Inst;
            var ma = Option<Unit>.None.AsTask().ToAsync();
            var result = await subject.BiFoldAsync(ma, true,
                (s, m) => false.AsTask(),
                (s, _) => true);
            Assert.True(result);
        }

        [Fact]
        public async Task MOptionAsync_BiFoldAsync_OverloadAsyncAsync_IsNone_DelegateNoneExecuted()
        {
            var subject = MOptionAsync<Unit>.Inst;
            var ma = Option<Unit>.None.AsTask().ToAsync();
            var result = await subject.BiFoldAsync(ma, true,
                (s, m) => false.AsTask(),
                (s, _) => true.AsTask());
            Assert.True(result);
        }

        // Not valuable any more 
        //[Fact]
        //public async void SequenceFlip()
        //{
        //    Task<Option<int>> taskOpt = Task.Run(() => Some(10));

        //    Option<Task<int>> optTask = taskOpt.Sequence();
        //    var res = await optTask.IfNone(0.AsTask());
        //    Assert.True(res == 10);

        //    taskOpt = optTask.Sequence();
        //}

        private static OptionAsync<int> GetValue(bool select) =>
            select
                ? OptionAsync<int>.Some(1000)
                : OptionAsync<int>.None;
    }
}
