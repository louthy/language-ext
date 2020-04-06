using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Sync
{
    public class EitherUnsafe
    {
        [Fact]
        public void LeftUnsafeIsIdentityLeftUnsafe()
        {
            var ma = LeftUnsafe<Error, Identity<int>>(Error.New("An Error"));
            
            var mb = ma.Traverse(identity);
            
            var mc = Id(LeftUnsafe<Error, int>(Error.New("An Error")));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void RightUnsafeIsIdentityRightUnsafe()
        {
            var ma = RightUnsafe<Error, Identity<int>>(Id(42));

            var mb = ma.Traverse(identity);

            var mc = Id(RightUnsafe<Error, int>(42));

            Assert.Equal(mc, mb);
        }
    }
}
