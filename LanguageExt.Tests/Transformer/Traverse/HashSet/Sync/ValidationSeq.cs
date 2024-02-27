using Xunit;
using LanguageExt.Common;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Sync;

public class ValidationSeqHashSet
{
    [Fact]
    public void FailIsSingletonFail()
    {
        var ma = Fail<Error, HashSet<int>>(Error.New("alt"));
        var mb = ma.Traverse(mx => mx).As();

        var mc = HashSet(Fail<Error, int>(Error.New("alt")));

        Assert.True(mb == mc);
    }
        
    [Fact]
    public void SuccessEmptyIsEmpty()
    {
        var ma = Success<Error, HashSet<int>>(Empty);
        var mb = ma.Traverse(mx => mx).As();

        var mc = HashSet<Validation<Error, int>>();

        Assert.True(mb == mc);
    }
        
    [Fact]
    public void SuccessNonEmptyHashSetIsHashSetSuccesses()
    {
        var ma = Success<Error, HashSet<int>>(HashSet(1, 2, 3, 4));
        var mb = ma.Traverse(mx => mx).As();

        var mc = HashSet(Success<Error, int>(1), Success<Error, int>(2), Success<Error, int>(3), Success<Error, int>(4));
            
        Assert.True(mb == mc);
    }
}
