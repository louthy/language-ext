using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Sync
{
    public class OptionHashSet
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = Option<HashSet<int>>.None;
            var mb = ma.Sequence();
            var mc = HashSet(Option<int>.None);

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = Some<HashSet<int>>(Empty);
            var mb = ma.Sequence();
            var mc = HashSet<Option<int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SomeNonEmptyHashSetIsHashSetSomes()
        {
            var ma = Some(HashSet(1, 2, 3));
            var mb = ma.Sequence();
            var mc = HashSet(Some(1), Some(2), Some(3)); 
            
            Assert.True(mb == mc);
        }
    }
}
