using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Sync
{
    public class Option
    {
        [Fact]
        public void SomeIdentityIsIdentitySome()
        {
            var ma = Some(Id(42));

            var mb = ma.Traverse(identity);

            var mc = Id(Some(42));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void NoneIdentityIsIdentityNone()
        {
            var ma = Option<Identity<int>>.None;

            var mb = ma.Traverse(identity);

            var mc = Id(Option<int>.None);

            Assert.Equal(mc, mb);
        }
    }
}
