using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Collections
{
    public class SetOption
    {
        [Fact]
        public void EmptySetIsSomeEmptySet()
        {
            Set<Option<int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == Some(Set<int>.Empty));
        }
        
        [Fact]
        public void SetSomesIsSomeSets()
        {
            var ma = Set(Some(1), Some(2), Some(3));

            var mb = ma.Sequence();

            Assert.True(mb == Some(Set(1, 2, 3)));
        }
        
        [Fact]
        public void SetSomeAndNoneIsNone()
        {
            var ma = Set(Some(1), Some(2), None);

            var mb = ma.Sequence();

            Assert.True(mb == None);
        }
    }
}
