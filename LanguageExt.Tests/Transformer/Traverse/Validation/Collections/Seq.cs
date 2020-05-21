using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Collections
{
    public class Seq
    {
        [Fact]
        public void EmptySeqIsSuccessEmptySeq()
        {
            Seq<Validation<Error, string>> ma = Empty;
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, Seq<string>>(Empty), mb);
        }

        [Fact]
        public void SeqSuccessIsSuccessSeq()
        {
            var ma = Seq(Success<Error, int>(2), Success<Error, int>(8), Success<Error, int>(64));
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, Seq<int>>(Seq(2, 8, 64)), mb);
        }

        [Fact]
        public void SeqFailedIsFailedSeq()
        {
            var ma = Seq(Fail<Error, int>(Error.New("failed")), Fail<Error, int>(Error.New("failuire")));
            var mb = ma.Traverse(identity);
            var expected = Fail<Error, Seq<int>>(Seq(Error.New("failed"), Error.New("failuire")));
            Assert.Equal(expected, mb);
        }

        [Fact]
        public void SeqSuccAndFailIsFailedSeq()
        {
            var ma = Seq(Fail<Error, int>(Error.New("failed")), Success<Error, int>(12));
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<Error, Seq<int>>(Error.New("failed")), mb);
        }
    }
}
