using System;
using System.Collections.Generic;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Collections
{
    public class LstHashSet
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Lst<HashSet<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = HashSet<Lst<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void LstHashSetCrossProduct()
        {
            var ma = List(HashSet(1, 2), HashSet(10, 20, 30));

            var mb = ma.Sequence();

            var mc = HashSet(List(1, 10), List(1, 20), List(1, 30), List(2, 10), List(2, 20), List(2, 30));
            
            Assert.True(mb == mc);
        }
        
                
        [Fact]
        public void LstOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = List(HashSet<int>(), HashSet<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = HashSet<Lst<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void LstOfEmptiesIsEmpty()
        {
            var ma = List(HashSet<int>(), HashSet<int>());

            var mb = ma.Sequence();

            var mc = HashSet<Lst<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
