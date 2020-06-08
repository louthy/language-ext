using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Sync
{
    public class Identity
    {
        [Fact]
        public void IdentitySuccessIsSuccessIdentity()
        {
            var ma = Id(Success<Error, int>(12));
            var mb = ma.Traverse(identity);
            var mc = Success<Error, Identity<int>>(Id(12));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void IdentityFailIsFailIdentity()
        {
            var ma = Id(Fail<Error, int>(Error.New("error")));
            var mb = ma.Traverse(identity);
            var mc = Fail<Error, Identity<int>>(Error.New("error"));

            Assert.Equal(mc, mb);
        }
    }
}
