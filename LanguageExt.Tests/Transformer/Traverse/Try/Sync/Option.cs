using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Sync
{
    public class OptionTry
    {
        [Fact]
        public void NoneIsSuccNone()
        {
            var ma = Option<Try<int>>.None;
            var mb = ma.Sequence();
            var mc = TrySucc(Option<int>.None);

            Assert.True(default(EqTry<Option<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void SomeFailIsFail()
        {
            var ma = Some(TryFail<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryFail<Option<int>>(new Exception("fail"));

            Assert.True(default(EqTry<Option<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void SomeSomeIsSomeSome()
        {
            var ma = Some(TrySucc(1234));
            var mb = ma.Sequence();
            var mc = TrySucc(Some(1234));

            Assert.True(default(EqTry<Option<int>>).Equals(mb, mc));
        }
    }
}
