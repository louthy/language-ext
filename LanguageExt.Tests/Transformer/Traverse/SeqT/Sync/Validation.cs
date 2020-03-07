using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Sync
{
    public class ValidationSeq
    {
        [Fact]
        public void FailIsEmpty()
        {
            var ma = Fail<Error, Seq<int>>(Error.New("alt"));

            var mb = ma.Sequence();

            Assert.True(mb == Empty);
        }

        [Fact]
        public void SuccessEmptyIsEmpty()
        {
            var ma = Success<Error, Seq<int>>(Empty);

            var mb = ma.Sequence();

            Assert.True(mb == Empty);
        }

        [Fact]
        public void SuccessSeqIsSeqSuccess()
        {
            var ma = Success<Error, Seq<int>>(Seq(1, 2, 3));

            var mb = ma.Sequence();

            Assert.True(mb == Seq(Success<Error, int>(1), Success<Error, int>(2), Success<Error, int>(3)));
        }
    }
}
