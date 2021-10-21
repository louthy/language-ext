using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ValidationMonoid.Sync
{
    public class Identity
    {
        [Fact]
        public void IdentitySuccessIsSuccessIdentity()
        {
            var ma = Id(Success<MSeq<Error>, Seq<Error>, int>(12));
            var mb = ma.Traverse(identity);
            var mc = Success<MSeq<Error>, Seq<Error>, Identity<int>>(Id(12));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void IdentityFailIsFailIdentity()
        {
            var ma = Id(Fail<MSeq<Error>, Seq<Error>, int>(Seq1(Error.New("error"))));
            var mb = ma.Traverse(identity);
            var mc = Fail<MSeq<Error>, Seq<Error>, Identity<int>>(Seq1(Error.New("error")));

            Assert.Equal(mc, mb);
        }
    }
}
