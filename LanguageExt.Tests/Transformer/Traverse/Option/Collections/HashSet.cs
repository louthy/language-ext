using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Collections
{
    public class HashSetOption
    {
        [Fact]
        public void EmptyHashSetIsSomeEmptyHashSet()
        {
            HashSet<Option<int>> ma = Empty;

            var mb = ma.Traverse(mx => mx).As();


            Assert.True(mb == Some(HashSet<int>.Empty));
        }
        
        [Fact]
        public void HashSetSomesIsSomeHashSets()
        {
            var ma = HashSet(Some(1), Some(2), Some(3));

            var mb = ma.Traverse(mx => mx).As();


            Assert.True(mb == Some(HashSet(1, 2, 3)));
        }
        
        [Fact]
        public void HashSetSomeAndNoneIsNone()
        {
            var ma = HashSet(Some(1), Some(2), None);

            var mb = ma.Traverse(mx => mx).As();


            Assert.True(mb == None);
        }
    }
}
