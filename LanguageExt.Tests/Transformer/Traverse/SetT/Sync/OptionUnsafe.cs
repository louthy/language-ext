using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Sync
{
    public class OptionUnsafeSet
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = OptionUnsafe<Set<int>>.None;
            var mb = ma.Sequence();
            var mc = Set(OptionUnsafe<int>.None);

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = SomeUnsafe<Set<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Set<OptionUnsafe<int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SomeNonEmptySetIsSetSomes()
        {
            var ma = SomeUnsafe(Set(1, 2, 3));
            var mb = ma.Sequence();
            var mc = Set(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3)); 
            
            Assert.True(mb == mc);
        }
    }
}
