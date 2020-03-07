using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Collections
{
    public class HashSetHashSet
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            HashSet<HashSet<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = HashSet<HashSet<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void HashSetHashSetCrossProduct()
        {
            var ma = HashSet(HashSet(1, 2), HashSet(10, 20, 30));

            var mb = ma.Sequence();

            var mc = HashSet(HashSet(1, 10), HashSet(1, 20), HashSet(1, 30), HashSet(2, 10), HashSet(2, 20), HashSet(2, 30));
            
            Assert.True(mb == mc);
        }
        
                
        [Fact]
        public void HashSetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = HashSet(HashSet<int>(), HashSet<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = HashSet<HashSet<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void HashSetOfEmptiesIsEmpty()
        {
            var ma = HashSet(HashSet<int>(), HashSet<int>());

            var mb = ma.Sequence();

            var mc = HashSet<HashSet<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
