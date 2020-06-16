using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Sync
{
    public class OptionArr
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = Option<Arr<int>>.None;
            var mb = ma.Sequence();
            var mc = Array(Option<int>.None);

            Assert.True(mb == mc);
        }

        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = Some<Arr<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Array<Option<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SomeNonEmptyArrIsArrSomes()
        {
            var ma = Some(Array(1, 2, 3));
            var mb = ma.Sequence();
            var mc = Array(Some(1), Some(2), Some(3));

            Assert.True(mb == mc);
        }
    }
}
