using Xunit;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Collections;

public class ArrOption
{
    [Fact]
    public void EmptyArrIsSomeEmptyArr()
    {
        Arr<Option<int>> ma = Empty;

        var mb = ma.Traverse(mx => mx).As();


        Assert.True(mb == Some(Arr<int>.Empty));
    }
        
    [Fact]
    public void ArrSomesIsSomeArrs()
    {
        var ma = Array(Some(1), Some(2), Some(3));

        var mb = ma.Traverse(mx => mx).As();


        Assert.True(mb == Some(Array(1, 2, 3)));
    }
        
    [Fact]
    public void ArrSomeAndNoneIsNone()
    {
        var ma = Array(Some(1), Some(2), None);

        var mb = ma.Traverse(mx => mx).As();


        Assert.True(mb == None);
    }
}
