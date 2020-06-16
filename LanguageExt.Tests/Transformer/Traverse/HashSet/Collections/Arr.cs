using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Collections
{
    public class ArrHashSet
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Arr<HashSet<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = HashSet<Arr<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void ArrHashSetCrossProduct()
        {
            var ma = Array(HashSet(1, 2), HashSet(10, 20, 30));

            var mb = ma.Sequence();

            var mc = HashSet(Array(1, 10), Array(1, 20), Array(1, 30), Array(2, 10), Array(2, 20), Array(2, 30));
            
            Assert.True(mb == mc);
        }
        
                
        [Fact]
        public void ArrOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Array(HashSet<int>(), HashSet<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = HashSet<Arr<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void ArrOfEmptiesIsEmpty()
        {
            var ma = Array(HashSet<int>(), HashSet<int>());

            var mb = ma.Sequence();

            var mc = HashSet<Arr<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
