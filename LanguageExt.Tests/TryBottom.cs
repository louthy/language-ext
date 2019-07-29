using LanguageExt;
using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class TryBottom
    {
        [Fact]
        public void TryBottomMatch()
        {
            var tryBottom = Try<string>((Exception)null); // produce bottom value
            Assert.True(tryBottom.Try().IsBottom, "Try not in Bottom (test requirement)");
            Assert.True(tryBottom.Match(some => false, ex => ex != null), "Try.Match in Bottom does give null as Exception");
            Assert.True(tryBottom.Match(some => false, ex => ex is BottomException), "Try.Match in Bottom does not give BottomException");
            Assert.Equal(nameof(BottomException), tryBottom.IfFail(ex => ex?.GetType().Name));
            Assert.Throws<BottomException>(() => tryBottom.IfFailThrow());
        }

        [Fact]
        public async System.Threading.Tasks.Task TryBottomMatchAsync()
        {
            var tryAsyncBottom = Try<string>((Exception)null).ToAsync(); // produce bottom value
            Assert.True((await tryAsyncBottom.Try()).IsBottom, "TryAsync not in Bottom (test requirement)");
            Assert.True(await tryAsyncBottom.Match(some => false, ex => ex is BottomException), "TryAsync.Match in Bottom does not give BottomException");
        }
    }
}
