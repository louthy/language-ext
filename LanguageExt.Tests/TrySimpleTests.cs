using System;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class TrySimpleTests
    {
        private Try<Uri> CreateUri(string uri) => () => 
        new Uri(uri);

        [Fact]
        void SuccessfulTry()
        {
            var uriTry = CreateUri("http://github.com");

            uriTry.Match(
                Succ: uri => Assert.NotNull(uri),
                Fail: ex => Assert.True(false, "should have succeeded")
            );
        }

        [Fact]
        void FailingTry()
        {
            var uriTry = CreateUri("rubbish");

            uriTry.Match(
                Succ: uri => Assert.True(false, "should have failed"),
                Fail: ex => Assert.NotNull(ex)
            );
        }

        [Fact]
        void ItIsLazy()
        {
            bool tried = false;

            Func<string, Try<Uri>> createUri = (uri) => () => 
            {
                tried = true;
                return new Uri(uri);
            };

            var uriTry = createUri("http://github.com");
            Assert.False(tried, "creating a Try should not run it");

            var schemeTry = uriTry.Map(uri => uri.Scheme);
            Assert.False(tried, "mapping onto a try should not run it");

            uriTry.Match(
                Succ: uri => Assert.NotNull(uri),
                Fail: ex => Assert.True(false, "should have succeeded")
            );
            Assert.True(tried, "matching should run the Try");
        }
    }
}
