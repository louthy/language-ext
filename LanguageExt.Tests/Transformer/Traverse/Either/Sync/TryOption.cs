using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Sync
{
    public class TryOptionEither
    {
        [Fact]
        public void FailIsRightFail()
        {
            var ma = TryOption<Either<Error, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = Right<Error, TryOption<int>>(TryOptionFail<int>(Error.New("fail")));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void NoneIsRightNone()
        {
            var ma = TryOption<Either<Error, int>>(None);
            var mb = ma.Sequence();
            var mc = Right<Error, TryOption<int>>(TryOption<int>(None));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
       
        [Fact]
        public void SuccLeftIsLeft()
        {
            var ma = TryOption<Either<Error, int>>(Left(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = Left<Error, TryOption<int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccRightIsRight()
        {
            var ma = TryOption<Either<Error, int>>(Right(1234));
            var mb = ma.Sequence();
            var mc = Right<Error, TryOption<int>>(TryOption(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
