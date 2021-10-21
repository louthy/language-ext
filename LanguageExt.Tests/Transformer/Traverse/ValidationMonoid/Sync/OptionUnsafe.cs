using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ValidationMonoid.Sync
{
    public class OptionUnsafe
    {
        [Fact]
        public void NoneIsSuccessNone()
        {
            var ma = OptionUnsafe<Validation<MSeq<Error>, Seq<Error>, int>>.None;
            var mb = ma.Traverse(identity);
            var mc = Success<MSeq<Error>, Seq<Error>, OptionUnsafe<int>>(None);

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SomeSuccessIsSuccessSome()
        {
            var ma = SomeUnsafe(Success<MSeq<Error>, Seq<Error>, int>(12));
            var mb = ma.Traverse(identity);
            var mc = Success<MSeq<Error>, Seq<Error>, OptionUnsafe<int>>(12);

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SomeFailIsFailSome()
        {
            var ma = SomeUnsafe(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("Err"))));
            var mb = ma.Traverse(identity);
            var mc = Fail<MSeq<Error>, Seq<Error>, OptionUnsafe<int>>(Seq1(Error.New("Err")));

            Assert.Equal(mc, mb);
        }
    }
}
