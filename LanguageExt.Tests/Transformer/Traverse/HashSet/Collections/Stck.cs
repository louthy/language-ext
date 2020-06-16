using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Collections
{
    public class StckHashSet
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Stck<HashSet<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = HashSet<Stck<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void StckHashSetCrossProduct()
        {
            var ma = Stack(HashSet(1, 2), HashSet(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = HashSet(Stack(1, 10), Stack(1, 20), Stack(1, 30), Stack(2, 10), Stack(2, 20), Stack(2, 30));
            
            Assert.True(mb == mc);
        }
        
                
        [Fact]
        public void StckOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Stack(HashSet<int>(), HashSet<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = HashSet<Stck<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void StckOfEmptiesIsEmpty()
        {
            var ma = Stack(HashSet<int>(), HashSet<int>());

            var mb = ma.Traverse(identity);

            var mc = HashSet<Stck<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
