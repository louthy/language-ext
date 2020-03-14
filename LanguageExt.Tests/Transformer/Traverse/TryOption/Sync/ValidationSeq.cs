using System;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Sync
{
    public class ValidationSeqTryOption
    {
        [Fact]
        public void FailIsSuccFail()
        {
            var ma = Fail<Error, TryOption<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = TryOption(Fail<Error, int>(Error.New("alt")));

            Assert.True(default(EqTryOption<Validation<Error, int>>).Equals(mb, mc));
        }

        [Fact]
        public void SuccFailIsFail()
        {
            var ma = Success<Error, TryOption<int>>(TryOptionFail<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionFail<Validation<Error, int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<Validation<Error, int>>).Equals(mb, mc));
        }

        [Fact]
        public void SuccSuccIsSuccSucc()
        {
            var ma = Success<Error, TryOption<int>>(TryOption(1234));
            var mb = ma.Sequence();
            var mc = TryOption(Success<Error, int>(1234));

            Assert.True(default(EqTryOption<Validation<Error, int>>).Equals(mb, mc));
        }
    }
}
