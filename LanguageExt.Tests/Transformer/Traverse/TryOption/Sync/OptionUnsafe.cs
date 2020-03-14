using System;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Sync
{
    public class OptionUnsafeTryOption
    {
        [Fact]
        public void NoneIsSuccNone()
        {
            var ma = OptionUnsafe<TryOption<int>>.None;
            var mb = ma.Sequence();
            var mc = TryOptionSucc(OptionUnsafe<int>.None);

            Assert.True(default(EqTryOption<OptionUnsafe<int>>).Equals(mb, mc));
        }

        [Fact]
        public void SomeFailIsFail()
        {
            var ma = SomeUnsafe(TryOptionFail<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionFail<OptionUnsafe<int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<OptionUnsafe<int>>).Equals(mb, mc));
        }

        [Fact]
        public void SomeSomeIsSomeSome()
        {
            var ma = SomeUnsafe(TryOptionSucc(1234));
            var mb = ma.Sequence();
            var mc = TryOptionSucc(SomeUnsafe(1234));

            Assert.True(default(EqTryOption<OptionUnsafe<int>>).Equals(mb, mc));
        }
    }
}
