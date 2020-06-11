using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Sync
{
    public class OptionOptionUnsafe
    {
        [Fact]
        public void NoneIsSomeNone()
        {
            var ma = Option<OptionUnsafe<int>>.None;
            var mb = ma.Sequence();
            var mc = SomeUnsafe(Option<int>.None);

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SomeNoneIsNone()
        {
            var ma = Some<OptionUnsafe<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionUnsafe<Option<int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SomeSomeIsSomeSome()
        {
            var ma = Some(SomeUnsafe(1234));
            var mb = ma.Sequence();
            var mc = SomeUnsafe(Some(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
