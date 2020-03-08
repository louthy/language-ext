using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Collections
{
    public class SetOptionUnsafe
    {
        [Fact]
        public void EmptySetIsSomeEmptySet()
        {
            Set<OptionUnsafe<int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == SomeUnsafe(Set<int>.Empty));
        }
        
        [Fact]
        public void SetSomesIsSomeSets()
        {
            var ma = Set(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3));

            var mb = ma.Sequence();

            Assert.True(mb == SomeUnsafe(Set(1, 2, 3)));
        }
        
        [Fact]
        public void SetSomeAndNoneIsNone()
        {
            var ma = Set(SomeUnsafe(1), SomeUnsafe(2), None);

            var mb = ma.Sequence();

            Assert.True(mb == None);
        }
    }
}
