using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Collections
{
    public class LstOption
    {
        [Fact]
        public void EmptyLstIsSomeEmptyLst()
        {
            Lst<Option<int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == Some(Lst<int>.Empty));
        }
        
        [Fact]
        public void LstSomesIsSomeLsts()
        {
            var ma = List(Some(1), Some(2), Some(3));

            var mb = ma.Sequence();

            Assert.True(mb == Some(List(1, 2, 3)));
        }
        
        [Fact]
        public void LstSomeAndNoneIsNone()
        {
            var ma = List(Some(1), Some(2), None);

            var mb = ma.Sequence();

            Assert.True(mb == None);
        }
    }
}
