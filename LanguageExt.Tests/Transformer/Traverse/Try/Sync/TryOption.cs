using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Sync
{
    public class TryOptionTry
    {
        [Fact]
        public void FailIsSuccFail()
        {
            var ma = TryOption<Try<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TrySucc(TryOption<int>(new Exception("fail")));

            Assert.True(default(EqTry<TryOption<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void SuccFailIsFail()
        {
            var ma = TryOption(TryFail<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryFail<TryOption<int>>(new Exception("fail"));

            Assert.True(default(EqTry<TryOption<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void SuccSuccIsSuccSucc()
        {
            var ma = TryOption(TrySucc(1234));
            var mb = ma.Sequence();
            var mc = TrySucc(TryOption(1234));

            Assert.True(default(EqTry<TryOption<int>>).Equals(mb, mc));
        }
    }
}
