using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ValidationMonoid.Collections
{
    public class Lst
    {
        [Fact]
        public void EmptyLstIsSuccessEmptyLst()
        {
            Lst<Validation<MSeq<Error>, Seq<Error>, string>> ma = Empty;
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<MSeq<Error>, Seq<Error>, Lst<string>>(Empty), mb);
        }

        [Fact]
        public void LstSuccessIsSuccessLst()
        {
            var ma = List(Success<MSeq<Error>, Seq<Error>, int>(2), Success<MSeq<Error>, Seq<Error>, int>(8), Success<MSeq<Error>, Seq<Error>, int>(64));
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<MSeq<Error>, Seq<Error>, Lst<int>>(List(2, 8, 64)), mb);
        }

        [Fact]
        public void LstFailedIsFailedLst()
        {
            var ma = List(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("failed"))), Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("failure"))));
            var mb = ma.Traverse(identity);
            var expected = Fail<MSeq<Error>, Seq<Error>, Lst<int>>(Seq(Error.New("failed"), Error.New("failure")));
            Assert.Equal(expected, mb);
        }

        [Fact]
        public void LstSuccAndFailIsFailedLst()
        {
            var ma = List(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("failed"))), Success<MSeq<Error>, Seq<Error>, int>(12));
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<MSeq<Error>, Seq<Error>, Lst<int>>(Seq1(Error.New("failed"))), mb);
        }
    }
}
