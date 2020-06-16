using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Sync
{
    public class TryEither
    {
        [Fact]
        public void FailIsRightFail()
        {
            var ma = Try<Either<Error, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = Right<Error, Try<int>>(TryFail<int>(Error.New("fail")));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccLeftIsLeft()
        {
            var ma = Try<Either<Error, int>>(Left(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = Left<Error, Try<int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccRightIsRight()
        {
            var ma = Try<Either<Error, int>>(Right(1234));
            var mb = ma.Sequence();
            var mc = Right<Error, Try<int>>(TrySucc(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
