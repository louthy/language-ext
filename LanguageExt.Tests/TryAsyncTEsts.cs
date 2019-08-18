using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using Xunit;
using LanguageExt.ClassInstances;
using System.Diagnostics;

namespace LanguageExt.Tests
{
    public static class AsyncHelper
    {
        public static A CompletesImmediately<A>(Func<A> f, string thing = "")
        {
            var w = Stopwatch.StartNew();
            try
            {
                return f();
            }
            finally
            {
                var elapsed = w.ElapsedMilliseconds;
                Assert.True(w.ElapsedMilliseconds < 20, $"Supposed to complete in immediately (< 20ms).  It took {elapsed}ms. " + thing);
            }
        }

        public static async Task<A> TakesRoughly<A>(int milliseconds, Func<Task<A>> f, string thing = "")
        {
            var w = Stopwatch.StartNew();
            try
            {
                return await f();
            }
            finally
            {
                var elapsed = w.ElapsedMilliseconds;
                Assert.True(Math.Abs(elapsed - milliseconds) < (milliseconds / 10), $"Supposed to complete in {milliseconds}ms.  It took {elapsed}ms. " + thing);
            }
        }

        public static async Task TakesRoughly(int milliseconds, Func<Task> f, string thing = "")
        {
            var w = Stopwatch.StartNew();
            try
            {
                await f();
            }
            finally
            {
                Assert.True(Math.Abs(w.ElapsedMilliseconds - milliseconds) < 100);
            }
        }
    }

    public class TryAsyncTests
    {
        [Fact]
        public async void BindingSuccess()
        {
            var ma = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var mb = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var res = AsyncHelper.CompletesImmediately(() =>
                MTryAsync<int>.Inst.Bind<MTryAsync<int>, TryAsync<int>, int>(ma, a =>
                    MTryAsync<int>.Inst.Bind<MTryAsync<int>, TryAsync<int>, int>(mb, b =>
                        TryAsync(a + b))));

            var ab = await AsyncHelper.TakesRoughly(2000, () => res.IfFail(0));

            Assert.True(ab == 200);
        }

        [Fact]
        public async void BindingFail()
        {
            var ma = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var mb = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var res = AsyncHelper.CompletesImmediately(() =>
                MTryAsync<int>.Inst.Bind<MTryAsync<int>, TryAsync<int>, int>(ma, a =>
                    MTryAsync<int>.Inst.Bind<MTryAsync<int>, TryAsync<int>, int>(mb, b =>
                        MTryAsync<int>.Inst.Fail())));

            var ab = await AsyncHelper.TakesRoughly(2000, () => res.IfFail(0));

            Assert.True(ab == 0);
        }

        [Fact]
        public async void BindingFail2()
        {
            var ma = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var mb = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var res = AsyncHelper.CompletesImmediately(() =>
                MTryAsync<int>.Inst.Bind<MTryAsync<int>, TryAsync<int>, int>(ma, a =>
                    MTryAsync<int>.Inst.BindAsync<MTryAsync<int>, TryAsync<int>, int>(mb, async b =>
                        await ThrowNotImplementedException<TryAsync<int>>())));

            var ab = await AsyncHelper.TakesRoughly(2000, () => res.IfFail(0));

            Assert.True(ab == 0);
        }

        Task<A> ThrowNotImplementedException<A>() =>
            throw new NotImplementedException();

        Task<A> ThrowNotSupportedException<A>(string msg) =>
            throw new NotSupportedException(msg);

        [Fact]
        public async void BindingWithTryOptionSuccess()
        {
            var ma = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var mb = TryOptionAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var res = AsyncHelper.CompletesImmediately(() =>
                MTryAsync<int>.Inst.Bind<MTryAsync<int>, TryAsync<int>, int>(ma, a =>
                    MTryOptionAsync<int>.Inst.Bind<MTryAsync<int>, TryAsync<int>, int>(mb, b =>
                        TryAsync(a + b))));

            var ab = await AsyncHelper.TakesRoughly(2000, () => res.IfFail(0));

            Assert.True(ab == 200);
        }

        [Fact]
        public async void BindingWithTryOptionFail1()
        {
            var ma = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var mb = TryOptionAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var res = AsyncHelper.CompletesImmediately(() =>
                MTryAsync<int>.Inst.Bind<MTryAsync<int>, TryAsync<int>, int>(ma, a =>
                    MTryOptionAsync<int>.Inst.BindAsync<MTryAsync<int>, TryAsync<int>, int>(mb, b =>
                        ThrowNotSupportedException<TryAsync<int>>("hello"))));

            var ab = await AsyncHelper.TakesRoughly(2000, () => 
                res.Match(
                    Succ: _  => "", 
                    Fail: ex => ex.Message));

            Assert.True(ab == "hello", $"Actually got {ab}");
        }

        [Fact]
        public async void BindingWithTryOptionFail2()
        {
            var ma = TryOptionAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var mb = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var res = AsyncHelper.CompletesImmediately(() =>
                MTryOptionAsync<int>.Inst.Bind<MTryOptionAsync<int>, TryOptionAsync<int>, int>(ma, a =>
                    MTryAsync<int>.Inst.BindAsync<MTryOptionAsync<int>, TryOptionAsync<int>, int>(mb, b =>
                        ThrowNotSupportedException<TryOptionAsync<int>>("hello"))));

            var ab = await AsyncHelper.TakesRoughly(2000, () =>
                res.Match(
                    Some: _ => "",
                    None: () => "NONE",
                    Fail: ex => ex.Message));

            Assert.True(ab == "hello", $"Actually got {ab}");
        }
        [Fact]
        public async void BindingWithTryOptionFail3()
        {
            var ma = TryOptionAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var mb = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var res = AsyncHelper.CompletesImmediately(() =>
                MTryOptionAsync<int>.Inst.Bind<MTryOptionAsync<int>, TryOptionAsync<int>, int>(ma, a =>
                    MTryAsync<int>.Inst.Bind<MTryOptionAsync<int>, TryOptionAsync<int>, int>(mb, b =>
                         MTryOptionAsync<int>.Inst.Fail())));

            var ab = await AsyncHelper.TakesRoughly(2000, () =>
                res.Match(
                    Some: _ => "",
                    None: () => "NONE",
                    Fail: ex => ex.Message));

            Assert.True(ab == "NONE", $"Actually got {ab}");
        }

        [Fact]
        public async void LinqBindingSuccess()
        {
            var ma = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var mb = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var res = AsyncHelper.CompletesImmediately(() =>
                from a in ma
                from b in mb
                select a + b);

            var ab = await AsyncHelper.TakesRoughly(2000, () => res.IfFail(0));

            Assert.True(ab == 200);
        }

        [Fact]
        public async void LinqBindingFail()
        {
            var ma = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var mb = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var res = AsyncHelper.CompletesImmediately(() =>
                from a in ma
                from b in mb
                select raise<int>(BottomException.Default));

            var ab = await AsyncHelper.TakesRoughly(2000, () => res.IfFail(0));

            Assert.True(ab == 0);
        }

        [Fact]
        public async void LinqBindingFail2()
        {
            var ma = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var mb = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var res = AsyncHelper.CompletesImmediately(() =>
                from a in ma
                from b in mb
                select raise<int>(new NotImplementedException()));

            var ab = await AsyncHelper.TakesRoughly(2000, () => res.IfFail(0));

            Assert.True(ab == 0);
        }

        [Fact]
        public async void LinqBindingWithTryOptionSuccess()
        {
            var ma = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var mb = TryOptionAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var res = AsyncHelper.CompletesImmediately(() =>
                from a in ma
                from b in mb.ToTry(None: () => 0)
                select a + b);

            var ab = await AsyncHelper.TakesRoughly(2000, () => res.IfFail(0));

            Assert.True(ab == 200);
        }

        [Fact]
        public async void LinqBindingWithTryOptionFail1()
        {
            var ma = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var mb = TryOptionAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var res = AsyncHelper.CompletesImmediately(() =>
                from a in ma
                from b in mb.ToTry()
                select raise<string>(new NotSupportedException("hello")));

            var ab = await AsyncHelper.TakesRoughly(2000, () =>
                res.Match(
                    Succ: _ => "",
                    Fail: ex => ex.Message));

            Assert.True(ab == "hello", $"Actually got {ab}");
        }

        [Fact]
        public async void LinqBindingWithTryOptionFail2()
        {
            var ma = TryOptionAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var mb = TryAsync(async () =>
            {
                await Task.Delay(1000);
                return 100;
            });

            var res = AsyncHelper.CompletesImmediately(() =>
                from a in ma
                from b in mb.ToTryOption()
                select raise<string>(new NotSupportedException("hello")));

            var ab = await AsyncHelper.TakesRoughly(2000, () =>
                res.Match(
                    Some: _ => "",
                    None: () => "NONE",
                    Fail: ex => ex.Message));

            Assert.True(ab == "hello", $"Actually got {ab}");
        }
    }
}
