using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Sync
{
    public class TryOption
    {
        [Fact]
        public void TrySuccessIsSuccessTry()
        {
            var ma = TryOption(Success<Error, int>(12));
            var mb = ma.Sequence();
            var mc = Success<Error, TryOption<int>>(TryOption(12));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void TryFailisFailTry()
        {
            var ma = TryOption<Validation<Error, int>>(new Exception("Fail"));
            var mb = ma.Sequence();
            var mc = Success<Error, TryOption<int>>(TryOption<int>(new Exception("Fail")));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void TryValidationFailIsValidationFailTry()
        {
            var ma = TryOptionSucc(Success<Error, int>(12));
            var mb = ma.Sequence();
            var mc = Success<Error, TryOption<int>>(TryOptionSucc(12));

            Assert.Equal(mc, mb);
        }
    }
}
