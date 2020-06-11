using System;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Sync
{
    public class ValidationWithMonoidSeq
    {
        [Fact]
        public void FailIsSingletonFail()
        {
            var ma = Fail<MSeq<Error>, Seq<Error>, Seq<int>>(Seq1(Error.New("alt")));
            var mb = ma.Traverse(identity);
            var mc = Seq1(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New(new Exception("alt")))));

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccessEmptyIsEmpty()
        {
            var ma = Success<MSeq<Error>, Seq<Error>, Seq<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Seq<Validation<MSeq<Error>, Seq<Error>, int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccessNonEmptySeqIsSeqSuccesses()
        {
            var ma = Success<MSeq<Error>, Seq<Error>, Seq<int>>(Seq(1, 2, 3, 4));
            var mb = ma.Traverse(identity);
            var mc = Seq(
                Success<MSeq<Error>, Seq<Error>, int>(1),
                Success<MSeq<Error>, Seq<Error>, int>(2),
                Success<MSeq<Error>, Seq<Error>, int>(3),
                Success<MSeq<Error>, Seq<Error>, int>(4));

            Assert.True(mb == mc);
        }
    }
}
