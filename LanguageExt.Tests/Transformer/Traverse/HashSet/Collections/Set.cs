using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Collections
{
    public class SetHashSet
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Set<HashSet<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = HashSet<Set<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SetHashSetCrossProduct()
        {
            var ma = Set(HashSet(1, 2), HashSet(10, 20, 30));

            var mb = ma.Sequence();

            var mc = HashSet(Set(1, 10), Set(1, 20), Set(1, 30), Set(2, 10), Set(2, 20), Set(2, 30));
            
            Assert.True(mb == mc);
        }
        
                
        [Fact]
        public void SetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Set(HashSet<int>(), HashSet<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = HashSet<Set<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SetOfEmptiesIsEmpty()
        {
            var ma = Set(HashSet<int>(), HashSet<int>());

            var mb = ma.Sequence();

            var mc = HashSet<Set<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
