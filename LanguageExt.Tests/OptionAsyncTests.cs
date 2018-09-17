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

            Assert.True(tasks[0].Status == TaskStatus.RanToCompletion);
            Assert.True(tasks[1].Status == TaskStatus.RanToCompletion);
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
    }
}
