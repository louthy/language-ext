using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Collections
{
    public class QueHashSet
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Que<HashSet<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = HashSet<Que<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void QueHashSetCrossProduct()
        {
            var ma = Queue(HashSet(1, 2), HashSet(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = HashSet(Queue(1, 10), Queue(1, 20), Queue(1, 30), Queue(2, 10), Queue(2, 20), Queue(2, 30));
            
            Assert.True(mb == mc);
        }
        
                
        [Fact]
        public void QueOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Queue(HashSet<int>(), HashSet<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = HashSet<Que<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void QueOfEmptiesIsEmpty()
        {
            var ma = Queue(HashSet<int>(), HashSet<int>());

            var mb = ma.Traverse(identity);

            var mc = HashSet<Que<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
