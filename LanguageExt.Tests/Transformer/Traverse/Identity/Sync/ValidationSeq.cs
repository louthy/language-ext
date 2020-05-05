using Xunit;
using static LanguageExt.Prelude;
using LanguageExt.Common;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Sync
{
    public class ValidationSeq
    {
        [Fact]
        public void ValidationFailIsIdentityFail()
        {
            var ma = Fail<Error, Identity<int>>(Error.New("An Error"));

            var mb = ma.Traverse(identity);

            var mc = Id(Fail<Error, int>(Error.New("An Error")));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void ValidationSuccIsIdentitySuccess()
        {
            var ma = Success<Error, Identity<int>>(Id(42));

            var mb = ma.Traverse(identity);

            var mc = Id(Success<Error, int>(42));

            Assert.Equal(mc, mb);
        }
    }
}
