using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Sync;

public class IdentityOption
{
    [Fact]
    public void IdentityNoneIsNone()
    {
        var ma = new Identity<Option<int>>(None);
        var mb = ma.Traverse(x => x).As();
        var mc = Option<Identity<int>>.None;

        var mr = mb == mc;
            
        Assert.True(mr);
    }
        
    [Fact]
    public void IdentitySomeIsSomeIdentity()
    {
        var ma = new Identity<Option<int>>(1234);
        var mb = ma.Traverse(x => x).As();
        var mc = Some(new Identity<int>(1234));

        var mr = mb == mc;
            
        Assert.True(mr);
    }
}
