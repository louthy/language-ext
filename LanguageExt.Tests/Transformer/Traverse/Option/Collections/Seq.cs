using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Collections
{
    public class SeqOption
    {
        [Fact]
        public void EmptySeqIsSomeEmptySeq()
        {
            Seq<Option<int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == Some(Seq<int>.Empty));
        }
        
        [Fact]
        public void SeqSomesIsSomeSeqs()
        {
            var ma = Seq(Some(1), Some(2), Some(3));

            var mb = ma.Sequence();

            Assert.True(mb == Some(Seq(1, 2, 3)));
        }
        
        [Fact]
        public void SeqSomeAndNoneIsNone()
        {
            var ma = Seq(Some(1), Some(2), None);

            var mb = ma.Sequence();

            Assert.True(mb == None);
        }
    }
}
