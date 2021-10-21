using System.Collections.Generic;
using System.Linq;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ValidationMonoid.Collections
{
    public class IEnumerable
    {
        [Fact]
        public void EmptyIEnumerableIsSuccessIEnumerable()
        {
            var ma = Enumerable.Empty<Validation<MSeq<Error>, Seq<Error>, int>>();
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<MSeq<Error>, Seq<Error>, IEnumerable<int>>(LanguageExt.Seq.empty<int>()), mb);
        }

        [Fact]
        public void IEnumerableSuccessIsSuccessIEnumerable()
        {
            var ma = List(Success<MSeq<Error>, Seq<Error>, int>(2), Success<MSeq<Error>, Seq<Error>, int>(8), Success<MSeq<Error>, Seq<Error>, int>(64))
                .AsEnumerable();
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<MSeq<Error>, Seq<Error>, IEnumerable<int>>(List(2, 8, 64).AsEnumerable()), mb);
        }

        [Fact]
        public void IEnumerableSuccAndFailIsFailedIEnumerable()
        {
            var ma = List(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("failed"))), Success<MSeq<Error>, Seq<Error>, int>(12)).AsEnumerable();
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<MSeq<Error>, Seq<Error>, IEnumerable<int>>(Seq1(Error.New("failed"))), mb);
        }
    }
}
