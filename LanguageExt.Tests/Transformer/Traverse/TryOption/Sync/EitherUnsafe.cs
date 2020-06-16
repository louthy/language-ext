using System;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Sync
{
    public class EitherUnsafeTryOption
    {
        [Fact]
        public void LeftIsSuccLeft()
        {
            var ma = LeftUnsafe<Error, TryOption<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = TryOptionSucc(LeftUnsafe<Error, int>(Error.New("alt")));

            Assert.True(default(EqTryOption<EitherUnsafe<Error, int>>).Equals(mb, mc));
        }

        [Fact]
        public void RightFailIsFail()
        {
            var ma = RightUnsafe<Error, TryOption<int>>(TryOptionFail<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionFail<EitherUnsafe<Error, int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<EitherUnsafe<Error, int>>).Equals(mb, mc));
        }

        [Fact]
        public void RightSuccIsSuccRight()
        {
            var ma = RightUnsafe<Error, TryOption<int>>(TryOptionSucc(1234));
            var mb = ma.Sequence();
            var mc = TryOptionSucc(RightUnsafe<Error, int>(1234));

            Assert.True(default(EqTryOption<EitherUnsafe<Error, int>>).Equals(mb, mc));
        }
    }
}
