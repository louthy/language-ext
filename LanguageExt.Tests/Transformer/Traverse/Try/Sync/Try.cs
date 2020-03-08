using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Sync
{
    public class TryTry
    {
        [Fact]
        public void FailIsSuccFail()
        {
            var ma = Try<Try<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TrySucc(TryFail<int>(new Exception("fail")));

            Assert.True(default(EqTry<Try<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void SuccFailIsFail()
        {
            var ma = TrySucc(TryFail<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryFail<Try<int>>(new Exception("fail"));

            Assert.True(default(EqTry<Try<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void SuccSuccIsSuccSucc()
        {
            var ma = TrySucc(TrySucc(1234));
            var mb = ma.Sequence();
            var mc = TrySucc(TrySucc(1234));

            Assert.True(default(EqTry<Try<int>>).Equals(mb, mc));
        }
    }
}
