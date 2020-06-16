using System;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Sync
{
    public class OptionTryOption
    {
        [Fact]
        public void NoneIsSuccNone()
        {
            var ma = Option<TryOption<int>>.None;
            var mb = ma.Sequence();
            var mc = TryOptionSucc(Option<int>.None);

            Assert.True(default(EqTryOption<Option<int>>).Equals(mb, mc));
        }

        [Fact]
        public void SomeFailIsFail()
        {
            var ma = Some(TryOptionFail<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionFail<Option<int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<Option<int>>).Equals(mb, mc));
        }

        [Fact]
        public void SomeSomeIsSomeSome()
        {
            var ma = Some(TryOptionSucc(1234));
            var mb = ma.Sequence();
            var mc = TryOptionSucc(Some(1234));

            Assert.True(default(EqTryOption<Option<int>>).Equals(mb, mc));
        }
    }
}
