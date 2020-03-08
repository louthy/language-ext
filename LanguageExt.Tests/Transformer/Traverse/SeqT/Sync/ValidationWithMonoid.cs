using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Sync
{
    public class ValidationWithMonoidSeq
    {
        [Fact]
        public void FailIsEmpty()
        {
            var ma = Fail<MSeq<Error>, Seq<Error>, Seq<int>>(Seq1(Error.New("alt")));

            var mb = ma.Traverse(identity);

            var mc = Seq1(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("alt"))));
            
            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccessEmptyIsEmpty()
        {
            var ma = Success<MSeq<Error>, Seq<Error>, Seq<int>>(Empty);

            var mb = ma.Traverse(identity);

            Assert.True(mb == Empty);
        }

        [Fact]
        public void SuccessSeqIsSeqSuccess()
        {
            var ma = Success<MSeq<Error>, Seq<Error>, Seq<int>>(Seq(1, 2, 3));

            var mb = ma.Traverse(identity);

            var expectedResult = Seq(
                Success<MSeq<Error>, Seq<Error>, int>(1),
                Success<MSeq<Error>, Seq<Error>, int>(2),
                Success<MSeq<Error>, Seq<Error>, int>(3));

            Assert.True(mb == expectedResult);
        }
    }
}
