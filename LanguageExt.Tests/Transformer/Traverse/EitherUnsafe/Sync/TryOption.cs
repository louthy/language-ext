using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Sync
{
    public class TryOptionEitherUnsafe
    {
        [Fact]
        public void WithErrorLeft_FailIsLeft()
        {
            var ma = TryOption<EitherUnsafe<Error, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = LeftUnsafe<Error, TryOption<int>>(Error.New("fail"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void WithExceptionLeft_FailIsLeft()
        {
            var ma = TryOption<EitherUnsafe<Exception, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = LeftUnsafe<Exception, TryOption<int>>(new Exception("fail"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void WithStringLeft_FailIsLeft()
        {
            var ma = TryOption<EitherUnsafe<string, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = LeftUnsafe<string, TryOption<int>>("fail");
            
            var mr = mb == mc;
            
            Assert.True(mr);
        }        
        
        [Fact]
        public void WithUnknownLeft_ResultIsBottom()
        {
            var ma = TryOption<EitherUnsafe<bool, int>>(new Exception("fail"));
            var mb = ma.Sequence();

            Assert.True(mb.IsBottom);
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
