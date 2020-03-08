using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Sync
{
    public class TryOptionArr
    {
        [Fact]
        public void FailIsSingletonNone()
        {
            var ma = TryOptionFail<Arr<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = Array(TryOptionFail<int>(new Exception("fail")));

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TryOptionSucc<Arr<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Array<TryOption<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccNonEmptyArrIsArrSuccs()
        {
            var ma = TryOptionSucc(Array(1, 2, 3));
            var mb = ma.Sequence();
            var mc = Array(TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3));

            Assert.True(mb == mc);
        }
    }
}
