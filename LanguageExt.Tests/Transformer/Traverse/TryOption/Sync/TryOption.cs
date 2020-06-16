using System;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Sync
{
    public class TryOptionTryOption
    {
        [Fact]
        public void FailIsSuccFail()
        {
            var ma = TryOption<TryOption<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TryOption(TryOption<int>(new Exception("fail")));

            Assert.True(default(EqTryOption<TryOption<int>>).Equals(mb, mc));
        }

        [Fact]
        public void SuccFailIsFail()
        {
            var ma = TryOption(TryOptionFail<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionFail<TryOption<int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<TryOption<int>>).Equals(mb, mc));
        }

        [Fact]
        public void SuccSuccIsSuccSucc()
        {
            var ma = TryOption(TryOption(1234));
            var mb = ma.Sequence();
            var mc = TryOption(TryOption(1234));

            Assert.True(default(EqTryOption<TryOption<int>>).Equals(mb, mc));
        }
    }
}
