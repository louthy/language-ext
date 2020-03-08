using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Collections
{
    public class StckOption
    {
        [Fact]
        public void EmptyStckIsSomeEmptyStck()
        {
            Que<Option<int>> ma = Empty;

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == Some(Que<int>.Empty));
        }
        
        [Fact]
        public void StckSomesIsSomeStcks()
        {
            var ma = Stack(Some(1), Some(2), Some(3));

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == Some(Stack(1, 2, 3)));
        }
        
        [Fact]
        public void StckSomeAndNoneIsNone()
        {
            var ma = Stack(Some(1), Some(2), None);

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == None);
        }
    }
}
