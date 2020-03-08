using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Sync
{
    public class OptionUnsafeArr
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = OptionUnsafe<Arr<int>>.None;
            var mb = ma.Sequence();
            var mc = Array(OptionUnsafe<int>.None);

            Assert.True(mb == mc);
        }

        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = SomeUnsafe<Arr<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Array<OptionUnsafe<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SomeNonEmptyArrIsArrSomes()
        {
            var ma = SomeUnsafe(Array(1, 2, 3));
            var mb = ma.Sequence();
            var mc = Array(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3));

            Assert.True(mb == mc);
        }
    }
}
