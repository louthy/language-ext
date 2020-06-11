using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Collections
{
    public class SeqHashSet
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Seq<HashSet<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = HashSet<Seq<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SeqHashSetCrossProduct()
        {
            var ma = Seq(HashSet(1, 2), HashSet(10, 20, 30));

            var mb = ma.Sequence();

            var mc = HashSet(Seq(1, 10), Seq(1, 20), Seq(1, 30), Seq(2, 10), Seq(2, 20), Seq(2, 30));
            
            Assert.True(mb == mc);
        }
        
                
        [Fact]
        public void SeqOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Seq(HashSet<int>(), HashSet<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = HashSet<Seq<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SeqOfEmptiesIsEmpty()
        {
            var ma = Seq(HashSet<int>(), HashSet<int>());

            var mb = ma.Sequence();

            var mc = HashSet<Seq<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
