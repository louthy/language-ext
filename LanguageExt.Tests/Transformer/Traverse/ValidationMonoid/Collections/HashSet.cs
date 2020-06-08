using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ValidationMonoid.Collections
{
    public class HashSet
    {
        [Fact]
        public void EmptyHashSetIsSuccessEmptyHashSet()
        {
            HashSet<Validation<MSeq<Error>, Seq<Error>, string>> ma = Empty;
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<MSeq<Error>, Seq<Error>, HashSet<string>>(Empty), mb);
        }

        [Fact]
        public void HashSetSuccessIsSuccessHashSet()
        {
            var ma = HashSet(Success<MSeq<Error>, Seq<Error>, int>(2), Success<MSeq<Error>, Seq<Error>, int>(8), Success<MSeq<Error>, Seq<Error>, int>(64));
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<MSeq<Error>, Seq<Error>, HashSet<int>>(HashSet(2, 8, 64)), mb);
        }

        [Fact]
        public void HashSetSuccAndFailIsFailedHashSet()
        {
            var ma = HashSet(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("failed"))), Success<MSeq<Error>, Seq<Error>, int>(12));
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<MSeq<Error>, Seq<Error>, HashSet<int>>(Seq1(Error.New("failed"))), mb);
        }
    }
}
