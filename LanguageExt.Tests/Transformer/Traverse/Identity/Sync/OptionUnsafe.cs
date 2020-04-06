using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Sync
{
    public class OptionUnsafe
    {
        [Fact]
        public void SomeIdentityIsIdentitySome()
        {
            var ma = SomeUnsafe(Id(42));

            var mb = ma.Traverse(identity);

            var mc = Id(SomeUnsafe(42));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void NoneIdentityIsIdentityNone()
        {
            var ma = OptionUnsafe<Identity<int>>.None;

            var mb = ma.Traverse(identity);

            var mc = Id(OptionUnsafe<int>.None);

            Assert.Equal(mc, mb);
        }
    }
}
