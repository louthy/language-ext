using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Sync
{
    public class OptionUnsafeTry
    {
        [Fact]
        public void NoneIsSuccNone()
        {
            var ma = OptionUnsafe<Try<int>>.None;
            var mb = ma.Sequence();
            var mc = TrySucc(OptionUnsafe<int>.None);

            Assert.True(default(EqTry<OptionUnsafe<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void SomeFailIsFail()
        {
            var ma = SomeUnsafe(TryFail<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryFail<OptionUnsafe<int>>(new Exception("fail"));

            Assert.True(default(EqTry<OptionUnsafe<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void SomeSomeIsSomeSome()
        {
            var ma = SomeUnsafe(TrySucc(1234));
            var mb = ma.Sequence();
            var mc = TrySucc(SomeUnsafe(1234));

            Assert.True(default(EqTry<OptionUnsafe<int>>).Equals(mb, mc));
        }
    }
}
