using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Sync
{
    public class TryArr
    {
        [Fact]
        public void FailIsSingletonNone()
        {
            var ma = TryFail<Arr<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = Array(TryFail<int>(new Exception("fail")));

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TrySucc<Arr<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Array<Try<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccNonEmptyArrIsArrSuccs()
        {
            var ma = TrySucc(Array(1, 2, 3));
            var mb = ma.Sequence();
            var mc = Array(TrySucc(1), TrySucc(2), TrySucc(3));

            Assert.True(mb == mc);
        }
    }
}
