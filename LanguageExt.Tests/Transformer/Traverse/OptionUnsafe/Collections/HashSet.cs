using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Collections
{
    public class HashSetOptionUnsafe
    {
        [Fact]
        public void EmptyHashSetIsSomeEmptyHashSet()
        {
            HashSet<OptionUnsafe<int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == SomeUnsafe(HashSet<int>.Empty));
        }
        
        [Fact]
        public void HashSetSomesIsSomeHashSets()
        {
            var ma = HashSet(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3));

            var mb = ma.Sequence();

            Assert.True(mb == SomeUnsafe(HashSet(1, 2, 3)));
        }
        
        [Fact]
        public void HashSetSomeAndNoneIsNone()
        {
            var ma = HashSet(SomeUnsafe(1), SomeUnsafe(2), None);

            var mb = ma.Sequence();

            Assert.True(mb == None);
        }
    }
}
