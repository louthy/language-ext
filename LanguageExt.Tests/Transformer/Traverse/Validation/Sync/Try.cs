using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Sync
{
    public class Try
    {
        [Fact]
        public void TrySuccessIsSuccessTry()
        {
            var ma = Try(Success<Error, int>(12));
            var mb = ma.Sequence();
            var mc = Success<Error, Try<int>>(Try(12));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void TryFailisFailTry()
        {
            var ma = Try<Validation<Error, int>>(new Exception("Fail"));
            var mb = ma.Sequence();
            var mc = Success<Error, Try<int>>(Try<int>(new Exception("Fail")));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void TryValidationFailIsValidationFailTry()
        {
            var ma = TrySucc(Success<Error, int>(12));
            var mb = ma.Sequence();
            var mc = Success<Error, Try<int>>(TrySucc(12));

            Assert.Equal(mc, mb);
        }
    }
}
