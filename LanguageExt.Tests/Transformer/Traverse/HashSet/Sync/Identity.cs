using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Sync
{
    public class IdentityHashSet
    {
        [Fact]
        public void IdEmptyIsEmpty()
        {
            var ma = Id<HashSet<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = HashSet<Identity<int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void IdNonEmptyHashSetIsHashSetId()
        {
            var ma = Id(HashSet(1, 2, 3));
            var mb = ma.Traverse(identity);
            var mc = HashSet(Id(1), Id(2), Id(3));
            
            Assert.True(mb == mc);
        }
    }
}
