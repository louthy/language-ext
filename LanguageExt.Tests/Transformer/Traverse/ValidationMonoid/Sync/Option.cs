using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ValidationMonoid.Sync
{
    public class Option
    {
        [Fact]
        public void NoneIsSuccessNone()
        {
            var ma = Option<Validation<MSeq<Error>, Seq<Error>, int>>.None;
            var mb = ma.Traverse(identity);
            var mc = Success<MSeq<Error>, Seq<Error>, Option<int>>(None);

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SomeSuccessIsSuccessSome()
        {
            var ma = Some(Success<MSeq<Error>, Seq<Error>, int>(12));
            var mb = ma.Traverse(identity);
            var mc = Success<MSeq<Error>, Seq<Error>, Option<int>>(12);

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SomeFailIsFailSome()
        {
            var ma = Some(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("Err"))));
            var mb = ma.Traverse(identity);
            var mc = Fail<MSeq<Error>, Seq<Error>, Option<int>>(Seq1(Error.New("Err")));

            Assert.Equal(mc, mb);
        }
    }
}
