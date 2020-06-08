using System;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ValidationMonoid.Sync
{
    public class TryOption
    {
        [Fact]
        public void TrySuccessIsSuccessTry()
        {
            var ma = TryOption(Success<MSeq<Error>, Seq<Error>, int>(12));
            var mb = ma.Traverse(identity);
            var mc = Success<MSeq<Error>, Seq<Error>, TryOption<int>>(TryOption(12));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void TryFailisFailTry()
        {
            var ma = TryOption<Validation<MSeq<Error>, Seq<Error>, int>>(new Exception("Fail"));
            var mb = ma.Traverse(identity);
            var mc = Success<MSeq<Error>, Seq<Error>, TryOption<int>>(TryOption<int>(new Exception("Fail")));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void TryValidationFailIsValidationFailTry()
        {
            var ma = TryOptionSucc(Success<MSeq<Error>, Seq<Error>, int>(12));
            var mb = ma.Traverse(identity);
            var mc = Success<MSeq<Error>, Seq<Error>, TryOption<int>>(TryOptionSucc(12));

            Assert.Equal(mc, mb);
        }
    }
}
