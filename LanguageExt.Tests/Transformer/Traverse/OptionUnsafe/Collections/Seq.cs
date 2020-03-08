using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Collections
{
    public class SeqOptionUnsafe
    {
        [Fact]
        public void EmptySeqIsSomeEmptySeq()
        {
            Seq<OptionUnsafe<int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == SomeUnsafe(Seq<int>.Empty));
        }
        
        [Fact]
        public void SeqSomesIsSomeSeqs()
        {
            var ma = Seq(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3));

            var mb = ma.Sequence();

            Assert.True(mb == SomeUnsafe(Seq(1, 2, 3)));
        }
        
        [Fact]
        public void SeqSomeAndNoneIsNone()
        {
            var ma = Seq(SomeUnsafe(1), SomeUnsafe(2), None);

            var mb = ma.Sequence();

            Assert.True(mb == None);
        }
    }
}
