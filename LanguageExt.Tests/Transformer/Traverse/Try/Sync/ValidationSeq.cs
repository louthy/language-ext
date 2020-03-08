using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Sync
{
    public class ValidationSeqTry
    {
        [Fact]
        public void FailIsSuccFail()
        {
            var ma = Fail<Error, Try<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = TrySucc<Validation<Error, int>>(Fail<Error, int>(Error.New("alt")));

            Assert.True(default(EqTry<Validation<Error, int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void RightFailIsFail()
        {
            var ma = Success<Error, Try<int>>(TryFail<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryFail<Validation<Error, int>>(new Exception("fail"));

            Assert.True(default(EqTry<Validation<Error, int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void RightSuccIsSuccRight()
        {
            var ma = Success<Error, Try<int>>(TrySucc(1234));
            var mb = ma.Sequence();
            var mc = TrySucc(Success<Error, int>(1234));
            
            Assert.True(default(EqTry<Validation<Error, int>>).Equals(mb, mc));
        }

    }
}
