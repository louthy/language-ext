using System;
using Xunit;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Sync
{
    public class TryLst
    {
        
        [Fact]
        public void FailIsSingletonNone()
        {
            var ma = TryFail<Lst<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = List(TryFail<int>(new Exception("fail")));
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TrySucc<HashSet<int>>(Empty);
            var mb = ma.Sequence();
            var mc = HashSet<Try<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccNonEmptyLstIsHashSetSuccs()
        {
            var ma = TrySucc(HashSet(1, 2, 3));
            var mb = ma.Sequence();
            var mc = HashSet(TrySucc(1), TrySucc(2), TrySucc(3));

            Assert.True(mb == mc);
        }
    }
}
