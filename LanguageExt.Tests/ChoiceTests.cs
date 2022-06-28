using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using Xunit;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Tests
{
    public class ChoiceTests
    {
        [Fact]
        public void TryFirstChoice()
        {
            var ma = Try(() => 123);
            var mb = Try<int>(() => throw new Exception());
            var mc = Try<int>(() => throw new Exception());

            var res = choice(ma, mb, mc);

            Assert.True(res.IfFail(0) == 123);
        }

        [Fact]
        public void TryMiddleChoice()
        {
            var ma = Try<int>(() => throw new Exception());
            var mb = Try(() => 123);
            var mc = Try<int>(() => throw new Exception());

            var res = choice(ma, mb, mc);

            Assert.True(res.IfFail(0) == 123);
        }

        [Fact]
        public void TryLastChoice()
        {
            var ma = Try<int>(() => throw new Exception());
            var mb = Try<int>(() => throw new Exception());
            var mc = Try(() => 123);

            var res = choice(ma, mb, mc);

            Assert.True(res.IfFail(0) == 123);
        }

        [Fact]
        public void TryNoChoice()
        {
            var ma = Try<int>(() => throw new Exception());
            var mb = Try<int>(() => throw new Exception());
            var mc = Try<int>(() => throw new Exception());

            var res = choice(ma, mb, mc);

            Assert.True(res.IsFail());
        }

        [Fact]
        public void TryOptionFirstChoice()
        {
            var ma = TryOption(() => 123);
            var mb = TryOption<int>(() => throw new Exception());
            var mc = TryOption<int>(() => throw new Exception());

            var res = choice(ma, mb, mc);

            Assert.True(res.IfNoneOrFail(0) == 123);
        }

        [Fact]
        public void TryOptionMiddleChoice()
        {
            var ma = TryOption<int>(() => throw new Exception());
            var mb = TryOption(() => 123);
            var mc = TryOption<int>(() => throw new Exception());

            var res = choice(ma, mb, mc);

            Assert.True(res.IfNoneOrFail(0) == 123);
        }

        [Fact]
        public void TryOptionLastChoice()
        {
            var ma = TryOption<int>(() => throw new Exception());
            var mb = TryOption<int>(() => throw new Exception());
            var mc = TryOption(() => 123);

            var res = choice(ma, mb, mc);

            Assert.True(res.IfNoneOrFail(0) == 123);
        }

        [Fact]
        public void TryOptionNoChoice()
        {
            var ma = TryOption<int>(() => throw new Exception());
            var mb = TryOption<int>(() => throw new Exception());
            var mc = TryOption<int>(() => throw new Exception());

            var res = choice(ma, mb, mc);

            Assert.True(res.IsNone());
        }

        [Fact]
        public async Task TryAsyncFirstChoice()
        {
            var ma = TryAsync(() => 123.AsTask());
            var mb = TryAsync<int>(() => throw new Exception());
            var mc = TryAsync<int>(() => throw new Exception());

            var res = choice(ma, mb, mc);

            Assert.True(await res.IfFail(0) == 123);
        }

        [Fact]
        public async Task TryAsyncMiddleChoice()
        {
            var ma = TryAsync<int>(() => throw new Exception());
            var mb = TryAsync(() => 123.AsTask());
            var mc = TryAsync<int>(() => throw new Exception());

            var res = choice(ma, mb, mc);

            Assert.True(await res.IfFail(0) == 123);
        }

        [Fact]
        public async Task TryAsyncLastChoice()
        {
            var ma = TryAsync<int>(() => throw new Exception());
            var mb = TryAsync<int>(() => throw new Exception());
            var mc = TryAsync(() => 123.AsTask());

            var res = choice(ma, mb, mc);

            Assert.True(await res.IfFail(0) == 123);
        }

        [Fact]
        public async Task TryAsyncNoChoice()
        {
            var ma = TryAsync<int>(() => throw new Exception());
            var mb = TryAsync<int>(() => throw new Exception());
            var mc = TryAsync<int>(() => throw new Exception());

            var res = choice(ma, mb, mc);

            Assert.True(await res.IsFail());
        }

        [Fact]
        public async Task TryOptionAsyncFirstChoice()
        {
            var ma = TryOptionAsync(() => 123.AsTask());
            var mb = TryOptionAsync<int>(() => throw new Exception());
            var mc = TryOptionAsync<int>(() => throw new Exception());

            var res = choice(ma, mb, mc);

            Assert.True(await res.IfNoneOrFail(0) == 123);
        }

        [Fact]
        public async Task TryOptionAsyncMiddleChoice()
        {
            var ma = TryOptionAsync<int>(() => throw new Exception());
            var mb = TryOptionAsync(() => 123.AsTask());
            var mc = TryOptionAsync<int>(() => throw new Exception());

            var res = choice(ma, mb, mc);

            Assert.True(await res.IfNoneOrFail(0) == 123);
        }

        [Fact]
        public async Task TryOptionAsyncLastChoice()
        {
            var ma = TryOptionAsync<int>(() => throw new Exception());
            var mb = TryOptionAsync<int>(() => throw new Exception());
            var mc = TryOptionAsync(() => 123.AsTask());

            var res = choice(ma, mb, mc);

            Assert.True(await res.IfNoneOrFail(0) == 123);
        }

        [Fact]
        public async Task TryOptionAsyncNoChoice()
        {
            var ma = TryOptionAsync<int>(() => throw new Exception());
            var mb = TryOptionAsync<int>(() => throw new Exception());
            var mc = TryOptionAsync<int>(() => throw new Exception());

            var res = choice(ma, mb, mc);

            Assert.True(await res.IsNone());
        }

        [Fact]
        public async Task TaskFirstChoice()
        {
            var ma = 123.AsTask();
            var mb = new Exception().AsFailedTask<int>();
            var mc = new Exception().AsFailedTask<int>();

            var res = choice(ma, mb, mc);

            Assert.True(await res == 123);
        }

        [Fact]
        public async Task TaskMiddleChoice()
        {
            var ma = new Exception().AsFailedTask<int>();
            var mb = 123.AsTask();
            var mc = new Exception().AsFailedTask<int>();

            var res = choice(ma, mb, mc);

            Assert.True(await res == 123);
        }

        [Fact]
        public async Task TaskLastChoice()
        {
            var ma = new Exception().AsFailedTask<int>();
            var mb = new Exception().AsFailedTask<int>();
            var mc = 123.AsTask();

            var res = choice(ma, mb, mc);

            Assert.True(await res == 123);
        }

        [Fact]
        public async Task TaskNoChoice()
        {
            var ma = new Exception().AsFailedTask<int>();
            var mb = new Exception().AsFailedTask<int>();
            var mc = new Exception().AsFailedTask<int>();

            var res = choice(ma, mb, mc);

            await Assert.ThrowsAsync<BottomException>(async () => await res);
        }
    }
}
