using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Collections
{
    public class ArrOptionUnsafe
    {
        [Fact]
        public void EmptyArrIsSomeEmptyArr()
        {
            Arr<OptionUnsafe<int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == SomeUnsafe(Arr<int>.Empty));
        }
        
        [Fact]
        public void ArrSomesIsSomeArrs()
        {
            var ma = Array(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3));

            var mb = ma.Sequence();

            Assert.True(mb == SomeUnsafe(Array(1, 2, 3)));
        }
        
        [Fact]
        public void ArrSomeAndNoneIsNone()
        {
            var ma = Array(SomeUnsafe(1), SomeUnsafe(2), None);

            var mb = ma.Sequence();

            Assert.True(mb == None);
        }
    }
}
