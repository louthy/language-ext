using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ValidationMonoid.Collections
{
    public class Arr
    {
        [Fact]
        public void EmptyArrayIsSuccessEmptyArray()
        {
            Arr<Validation<MSeq<Error>, Seq<Error>, string>> ma = Empty;
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<MSeq<Error>, Seq<Error>, Arr<string>>(Empty), mb);
        }

        [Fact]
        public void ArraySuccessIsSuccessArray()
        {
            var ma = Array(Success<MSeq<Error>, Seq<Error>, int>(2), Success<MSeq<Error>, Seq<Error>, int>(8), Success<MSeq<Error>, Seq<Error>, int>(64));
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<MSeq<Error>, Seq<Error>, Arr<int>>(Array(2, 8, 64)), mb);
        }

        [Fact]
        public void ArrayFailedIsFailedArray()
        {
            var ma = Array(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("failed"))), Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("failure"))));
            var mb = ma.Traverse(identity);
            var expected = Fail<MSeq<Error>, Seq<Error>, Arr<int>>(Seq(Error.New("failed"), Error.New("failure")));
            Assert.Equal(expected, mb);
        }

        [Fact]
        public void ArrSuccAndFailIsFailedArr()
        {
            var ma = Array(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("failed"))), Success<MSeq<Error>, Seq<Error>, int>(12));
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<MSeq<Error>, Seq<Error>, Arr<int>>(Seq1(Error.New("failed"))), mb);
        }
    }
}
