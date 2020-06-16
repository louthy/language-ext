using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Sync
{
    public class TryHashSet
    {
        [Fact]
        public void FailIsSingletonNone()
        {
            var ma = TryFail<HashSet<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = HashSet(TryFail<int>(new Exception("fail")));

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
        public void SuccNonEmptyHashSetIsHashSetSuccs()
        {
            var ma = TrySucc(HashSet(1, 2, 3));
            var mb = ma.Sequence();
            var mc = HashSet(TrySucc(1), TrySucc(2), TrySucc(3));

            Assert.True(mb == mc);
        }
    }
}
