using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Sync
{
    public class Either
    {
        [Fact]
        public void LeftIsIdentityLeft()
        {
            var ma = Left<Error, Identity<int>>(Error.New("An Error"));
            
            var mb = ma.Traverse(identity);

            var mc = Id(Left<Error, int>(Error.New("An Error")));
            
            Assert.Equal(mc, mb);
        }

        [Fact]
        public void RightIsIdentityRight()
        {
            var ma = Right<Error, Identity<int>>(Id(42));
            
            var mb = ma.Traverse(identity);
            
            var mc = Id(Right<Error, int>(42));
            
            Assert.Equal(mc, mb);
        }
    }
}
