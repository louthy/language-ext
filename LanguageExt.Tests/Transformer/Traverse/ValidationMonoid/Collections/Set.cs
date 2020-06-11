using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ValidationMonoid.Collections
{
    public class Set
    {
        [Fact]
        public void EmptySetIsSuccessEmptySet()
        {
            Set<Validation<MSeq<Error>, Seq<Error>, string>> ma = Empty;
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<MSeq<Error>, Seq<Error>, Set<string>>(Empty), mb);
        }

        [Fact]
        public void SetSuccessIsSuccessSet()
        {
            var ma = Set(Success<MSeq<Error>, Seq<Error>, int>(2), Success<MSeq<Error>, Seq<Error>, int>(8), Success<MSeq<Error>, Seq<Error>, int>(64));
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<MSeq<Error>, Seq<Error>, Set<int>>(Set(2, 8, 64)), mb);
        }

        [Fact]
        public void SetSuccAndFailIsFailedSet()
        {
            var ma = Set(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("failed"))), Success<MSeq<Error>, Seq<Error>, int>(12));
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<MSeq<Error>, Seq<Error>, Set<int>>(Seq1(Error.New("failed"))), mb);
        }
    }
}
