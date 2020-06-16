using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Sync
{
    public class TryEitherUnsafe
    {
        [Fact]
        public void FailIsRightFail()
        {
            var ma = Try<EitherUnsafe<Error, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = RightUnsafe<Error, Try<int>>(TryFail<int>(new Exception("fail")));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccLeftIsLeft()
        {
            var ma = Try<EitherUnsafe<Error, int>>(Left(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = LeftUnsafe<Error, Try<int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccRightIsRight()
        {
            var ma = Try<EitherUnsafe<Error, int>>(Right(1234));
            var mb = ma.Sequence();
            var mc = RightUnsafe<Error, Try<int>>(TrySucc(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
