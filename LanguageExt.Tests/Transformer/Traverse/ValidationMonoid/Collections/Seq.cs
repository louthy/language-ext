using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ValidationMonoid.Collections
{
    public class Seq
    {
        [Fact]
        public void EmptySeqIsSuccessEmptySeq()
        {
            Seq<Validation<MSeq<Error>, Seq<Error>, string>> ma = Empty;
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<MSeq<Error>, Seq<Error>, Seq<string>>(Empty), mb);
        }

        [Fact]
        public void SeqSuccessIsSuccessSeq()
        {
            var ma = Seq(Success<MSeq<Error>, Seq<Error>, int>(2), Success<MSeq<Error>, Seq<Error>, int>(8), Success<MSeq<Error>, Seq<Error>, int>(64));
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<MSeq<Error>, Seq<Error>, Seq<int>>(Seq(2, 8, 64)), mb);
        }

        [Fact]
        public void SeqFailedIsFailedSeq()
        {
            var ma = Seq(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("failed"))), Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("failuire"))));
            var mb = ma.Traverse(identity);
            var expected = Fail<MSeq<Error>, Seq<Error>, Seq<int>>(Seq(Error.New("failed"), Error.New("failuire")));
            Assert.Equal(expected, mb);
        }

        [Fact]
        public void SeqSuccAndFailIsFailedSeq()
        {
            var ma = Seq(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("failed"))), Success<MSeq<Error>, Seq<Error>, int>(12));
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<MSeq<Error>, Seq<Error>, Seq<int>>(Seq1(Error.New("failed"))), mb);
        }
    }
}
