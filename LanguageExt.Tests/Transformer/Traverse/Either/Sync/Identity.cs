using Xunit;
using LanguageExt.Common;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Sync;

public class IdentityEither
{
    [Fact]
    public void IdentityLeftIsLeft()
    {
        var ma = new Identity<Either<Error, int>>(Error.New("alt"));
        var mb = ma.Traverse(mx => mx).As();
        var mc = Left<Error, Identity<int>>(Error.New("alt"));

        var mr = mb == mc;
            
        Assert.True(mr);
    }
        
    [Fact]
    public void IdentityRightIsRightIdentity()
    {
        var ma = new Identity<Either<Error, int>>(1234);
        var mb = ma.Traverse(mx => mx).As();
        var mc = Right<Error, Identity<int>>(new Identity<int>(1234));

        var mr = mb == mc;
            
        Assert.True(mr);
    }
}
