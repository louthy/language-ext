using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Sync
{
    public class OptionUnsafeLst
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = OptionUnsafe<Lst<int>>.None;
            var mb = ma.Sequence();
            var mc = List(OptionUnsafe<int>.None);

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = SomeUnsafe<Lst<int>>(Empty);
            var mb = ma.Sequence();
            var mc = List<OptionUnsafe<int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SomeNonEmptyLstIsLstSomes()
        {
            var ma = SomeUnsafe(List(1, 2, 3));
            var mb = ma.Sequence();
            var mc = List(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3)); 
            
            Assert.True(mb == mc);
        }
    }
}
