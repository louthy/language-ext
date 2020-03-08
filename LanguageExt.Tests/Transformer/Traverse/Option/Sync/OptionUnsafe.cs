using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Sync
{
    public class OptionUnsafeOption
    {
        [Fact]
        public void NoneIsSomeNone()
        {
            var ma = OptionUnsafe<Option<int>>.None;
            var mb = ma.Sequence();
            var mc = Some(OptionUnsafe<int>.None);

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SomeNoneIsNone()
        {
            var ma = SomeUnsafe<Option<int>>(None);
            var mb = ma.Sequence();
            var mc = Option<OptionUnsafe<int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SomeSomeIsSomeSome()
        {
            var ma = SomeUnsafe(Some(1234));
            var mb = ma.Sequence();
            var mc = Some(SomeUnsafe(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
