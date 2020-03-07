using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Sync
{
    public class OptionUnsafeHashSet
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = OptionUnsafe<HashSet<int>>.None;
            var mb = ma.Sequence();
            var mc = HashSet(OptionUnsafe<int>.None);

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = SomeUnsafe<HashSet<int>>(Empty);
            var mb = ma.Sequence();
            var mc = HashSet<OptionUnsafe<int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SomeNonEmptyHashSetIsHashSetSomes()
        {
            var ma = SomeUnsafe(HashSet(1, 2, 3));
            var mb = ma.Sequence();
            var mc = HashSet(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3)); 
            
            Assert.True(mb == mc);
        }
    }
}
