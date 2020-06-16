using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Sync
{
    public class OptionSet
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = Option<Set<int>>.None;
            var mb = ma.Sequence();
            var mc = Set(Option<int>.None);

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = Some<Set<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Set<Option<int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SomeNonEmptySetIsSetSomes()
        {
            var ma = Some(Set(1, 2, 3));
            var mb = ma.Sequence();
            var mc = Set(Some(1), Some(2), Some(3)); 
            
            Assert.True(mb == mc);
        }
    }
}
