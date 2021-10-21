using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ValidationMonoid.Sync
{
    public class Either
    {
        [Fact]
        public void LeftIsSuccessLeft()
        {
            var ma = Left<Seq<Error>, Validation<MSeq<Error>, Seq<Error>, int>>(Seq1(Error.New("alt")));
            var mb = ma.Traverse(identity);
            var mc = Success<MSeq<Error>, Seq<Error>, Either<Seq<Error>, int>>(Left(Seq1(Error.New("alt"))));
            Assert.Equal(mc, mb);
        }

        [Fact]
        public void RightSuccessIsSuccess()
        {
            var ma = Right<Seq<Error>, Validation<MSeq<Error>, Seq<Error>, int>>(Success<MSeq<Error>, Seq<Error>, int>(12));
            var mb = ma.Traverse(identity);
            var mc = Success<MSeq<Error>, Seq<Error>, Either<Seq<Error>, int>>(Right(12));
            Assert.Equal(mc, mb);
        }

        [Fact]
        public void RightFailIsFail()
        {
            var ma = Right<Seq<Error>, Validation<MSeq<Error>, Seq<Error>, int>>(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("Error"))));
            var mb = ma.Traverse(identity);
            var mc = Fail<MSeq<Error>, Seq<Error>, Either<Seq<Error>, int>>(Seq1(Error.New("Error")));
            Assert.Equal(mc, mb);
        }
    }
}
