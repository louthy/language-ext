using System;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Sync
{
    public class TryTryOption
    {
        [Fact]
        public void FailIsSuccFail()
        {
            var ma = Try<TryOption<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TryOption(TryFail<int>(new Exception("fail")));

            Assert.True(default(EqTryOption<Try<int>>).Equals(mb, mc));
        }

        [Fact]
        public void SuccFailIsFail()
        {
            var ma = TrySucc(TryOptionFail<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionFail<Try<int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<Try<int>>).Equals(mb, mc));
        }

        [Fact]
        public void SuccSuccIsSuccSucc()
        {
            var ma = TrySucc(TryOption(1234));
            var mb = ma.Sequence();
            var mc = TryOption(TrySucc(1234));

            Assert.True(default(EqTryOption<Try<int>>).Equals(mb, mc));
        }
    }
}
