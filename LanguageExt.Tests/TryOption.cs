using System;
using System.Linq;
using LanguageExt;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class TryOptionTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("/", "")]
        [InlineData("one", "one")]
        [InlineData("two/three", "three")]
        [InlineData("four/five/six", "six")]
        public void TryOptionTest1(string input, string expected)
        {
            //when
            string output = GetLastPathObj(input).Match(
                Some: v => v,
                None: () => "empty",
                Fail: e => e.ToString()
                );

            //then
            Assert.Contains(expected, output);
        }

        public TryOption<string> GetLastPathObj(string text) =>
            () => text.Split('/').Last();

        [Fact]
        public void TryOptionBottomMatch()
        {
            var tryOptionBottom = TryOption<string>((Exception)null); // produce bottom value
            Assert.True(tryOptionBottom.Try().IsBottom, "TryOption not in Bottom (test requirement)");
            Assert.True(tryOptionBottom.Match(some => false, () => false, ex => ex != null), "TryOption.Match in Bottom does give null as Exception");
            Assert.True(tryOptionBottom.Match(some => false, () => false, ex => ex is BottomException), "TryOption.Match in Bottom does not give BottomException");
            Assert.Equal(nameof(BottomException), tryOptionBottom.IfNoneOrFail(() => "", ex => ex?.GetType().Name));
        }

        [Fact]
        public async System.Threading.Tasks.Task TryOptionBottomMatchAsync()
        {
            var tryOptionBottom = TryOption<string>((Exception)null); // produce bottom value
            Assert.True(tryOptionBottom.Try().IsBottom, "TryOption not in Bottom (test requirement)");
            Assert.True(await tryOptionBottom.AsTask().MatchAsync(some => false, () => false, ex => ex is BottomException), "TryOption.Match in Bottom does not give BottomException");
        }

        [Fact]
        public void Do_CallTwice_EvaluatedOnce()
        {
            var count = 0;
            TryOption<Unit> createTryOption() => () =>
            {
                count++;
                return unit;
            };

            createTryOption()
                .Do(_ => { })
                .Do(_ => { })
                .Iter(u => { });

            Assert.Equal(1, count);
        }

    }
}
