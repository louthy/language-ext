using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafe.Sync
{
    public class TryEitherUnsafe
    {
        [Fact]
        public void WithErrorLeft_FailIsLeft()
        {
            var ma = Try<EitherUnsafe<Error, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = LeftUnsafe<Error, Try<int>>(Error.New("fail"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void WithExceptionLeft_FailIsLeft()
        {
            var ma = Try<EitherUnsafe<Exception, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = LeftUnsafe<Exception, Try<int>>(new Exception("fail"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void WithStringLeft_FailIsLeft()
        {
            var ma = Try<EitherUnsafe<string, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = LeftUnsafe<string, Try<int>>("fail");
            
            var mr = mb == mc;
            
            Assert.True(mr);
        }        
        
        [Fact]
        public void WithUnknownLeft_ResultIsBottom()
        {
            var ma = Try<EitherUnsafe<bool, int>>(new Exception("fail"));
            var mb = ma.Sequence();

            Assert.True(mb.IsBottom);
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
            var mc = RightUnsafe<Error, Try<int>>(Try(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
