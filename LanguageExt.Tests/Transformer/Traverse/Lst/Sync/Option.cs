using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Sync
{
    public class OptionLst
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = Option<Lst<int>>.None;
            var mb = ma.Sequence();
            var mc = List(Option<int>.None);

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = Some<Lst<int>>(Empty);
            var mb = ma.Sequence();
            var mc = List<Option<int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SomeNonEmptyLstIsLstSomes()
        {
            var ma = Some(List(1, 2, 3));
            var mb = ma.Sequence();
            var mc = List(Some(1), Some(2), Some(3)); 
            
            Assert.True(mb == mc);
        }
    }
}
