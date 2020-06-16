using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Collections
{
    public class StckOptionUnsafe
    {
        [Fact]
        public void EmptyStckIsSomeEmptyStck()
        {
            Que<OptionUnsafe<int>> ma = Empty;

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == SomeUnsafe(Que<int>.Empty));
        }
        
        [Fact]
        public void StckSomesIsSomeStcks()
        {
            var ma = Stack(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3));

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == SomeUnsafe(Stack(1, 2, 3)));
        }
        
        [Fact]
        public void StckSomeAndNoneIsNone()
        {
            var ma = Stack(SomeUnsafe(1), SomeUnsafe(2), None);

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == None);
        }
    }
}
