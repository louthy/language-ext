using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Collections
{
    public class LstOptionUnsafe
    {
        [Fact]
        public void EmptyLstIsSomeEmptyLst()
        {
            Lst<OptionUnsafe<int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == SomeUnsafe(Lst<int>.Empty));
        }
        
        [Fact]
        public void LstSomesIsSomeLsts()
        {
            var ma = List(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3));

            var mb = ma.Sequence();

            Assert.True(mb == SomeUnsafe(List(1, 2, 3)));
        }
        
        [Fact]
        public void LstSomeAndNoneIsNone()
        {
            var ma = List(SomeUnsafe(1), SomeUnsafe(2), None);

            var mb = ma.Sequence();

            Assert.True(mb == None);
        }
    }
}
