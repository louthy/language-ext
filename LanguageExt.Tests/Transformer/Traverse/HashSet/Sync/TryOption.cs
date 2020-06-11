using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Sync
{
    public class TryOptionHashSet
    {
        [Fact]
        public void FailIsSingletonNone()
        {
            var ma = TryOptionFail<HashSet<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = HashSet(TryOptionFail<int>(new Exception("fail")));

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TryOptionSucc<HashSet<int>>(Empty);
            var mb = ma.Sequence();
            var mc = HashSet<TryOption<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccNonEmptyHashSetIsHashSetSuccs()
        {
            var ma = TryOptionSucc(HashSet(1, 2, 3));
            var mb = ma.Sequence();
            var mc = HashSet(TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3));

            Assert.True(mb == mc);
        }
    }
}
