using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Sync
{
    public class TrySet
    {
        // TODO: OrdDefault
        // [Fact]
        // public void FailIsSingletonNone()
        // {
        //     var ma = TryFail<Set<int>>(new Exception("fail"));
        //     var mb = ma.Sequence();
        //     var mc = Set(TryFail<int>(new Exception("fail")));
        //     
        //     Assert.True(mb == mc);
        // }
        
        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TrySucc<HashSet<int>>(Empty);
            var mb = ma.Sequence();
            var mc = HashSet<Try<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccNonEmptySetIsHashSetSuccs()
        {
            var ma = TrySucc(HashSet(1, 2, 3));
            var mb = ma.Sequence();
            var mc = HashSet(TrySucc(1), TrySucc(2), TrySucc(3));

            Assert.True(mb == mc);
        }
    }
}
