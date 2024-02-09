using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Sync
{
    public class TryOptionEitherUnsafe
    {
        [Fact]
        public void FailIsRightFail()
        {
            var ma = TryOption<EitherUnsafe<Error, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = RightUnsafe<Error, TryOption<int>>(TryOptionFail<int>(Error.New("fail")));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccLeftIsLeft()
        {
            var ma = TryOption<EitherUnsafe<Error, int>>(Left(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = LeftUnsafe<Error, TryOption<int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccRightIsRight()
        {
            var ma = TryOption<EitherUnsafe<Error, int>>(Right(1234));
            var mb = ma.Sequence();
            var mc = RightUnsafe<Error, TryOption<int>>(TryOption(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }

        [Fact]
        public void NoneIsRightNone()
        {
            var ma = TryOption<EitherUnsafe<Error, int>>(None);
            var mb = ma.Sequence();
            var mc = RightUnsafe<Error, TryOption<int>>(TryOption<int>(None));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
