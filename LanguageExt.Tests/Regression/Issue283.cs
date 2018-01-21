using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Regression
{
    public class Issue283
    {
        [Fact]
        public async Task AsyncFunctionShouldReturnFail()
        {
            var result = await Try(async () =>
            {
                var never = await Task.FromException<string>(new TestException());
                return never.ToUpperInvariant();
            }).ToAsync().Match(
                Succ: _ => Option<Exception>.None,
                Fail: ex => Some(ex)
            );

            Assert.True(result.IsSome);
            result.Some(e => Assert.IsType<TestException>(e));
        }

        [Fact]
        public async Task AsyncActionShouldReturnFail()
        {
            string never;
            var result = await Try(async () =>
            {
                var s = await Task.FromException<string>(new TestException());
                never = s.ToUpperInvariant();
            }).ToAsync().Match(
                Succ: _ => Option<Exception>.None,
                Fail: ex => Some(ex)
            );

            Assert.True(result.IsSome);
            result.Some(e => Assert.IsType<TestException>(e));
        }

        [Fact]
        public async Task SyncFunctionShouldReturnFail()
        {
            var result = await Try(() => Task.FromException<string>(new Exception())).ToAsync().Match(
                Succ: _ =>
                    Option<Exception>.None,
                Fail: ex =>
                    Some(ex)
            );

            Assert.True(result.IsSome);
            result.Some(e => Assert.IsType<TestException>(e));
        }

        [Fact]
        public async Task SyncActionShouldReturnFail()
        {
            var result = await Try(() => Task.FromException(new Exception())).ToAsync().Match(
                Succ: _ =>
                    Option<Exception>.None,
                Fail: ex =>
                    Some(ex)
            );

            Assert.True(result.IsSome);
            result.Some(e => Assert.IsType<TestException>(e));
        }


        public class TestException : Exception
        {
        }
    }
}
