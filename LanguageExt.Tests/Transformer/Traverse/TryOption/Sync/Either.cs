using System;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Sync
{
    public class EitherTryOption
    {
        [Fact]
        public void LeftIsSuccLeft()
        {
            var ma = Left<Error, TryOption<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = TryOptionSucc(Left<Error, int>(Error.New("alt")));

            Assert.True(default(EqTryOption<Either<Error, int>>).Equals(mb, mc));
        }

        [Fact]
        public void RightFailIsFail()
        {
            var ma = Right<Error, TryOption<int>>(TryOptionFail<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionFail<Either<Error, int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<Either<Error, int>>).Equals(mb, mc));
        }

        [Fact]
        public void RightSuccIsSuccRight()
        {
            var ma = Right<Error, TryOption<int>>(TryOptionSucc(1234));
            var mb = ma.Sequence();
            var mc = TryOptionSucc(Right<Error, int>(1234));

            Assert.True(default(EqTryOption<Either<Error, int>>).Equals(mb, mc));
        }
    }
}
