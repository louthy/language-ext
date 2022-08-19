using System;
using Xunit;

namespace LanguageExt.Tests
{
    public class FSharpTests
    {
    [Theory]
    [InlineData("Error")]
    [InlineData("")]
    public void ErrorResult_to_Either(string error)
    {
        var result = Microsoft.FSharp.Core.FSharpResult<int, string>.NewError(error);
        
        var either = FSharp.fs(result);

        Prelude.match(either,
            r => Assert.False(true, "Shouldn't get here"),
            l => Assert.True(l == error));
    }
    
    [Fact]
    public void ErrorResult_to_Either_with_null()
    {
        var result = Microsoft.FSharp.Core.FSharpResult<int, string>.NewError(null);
        
        Assert.Throws<ValueIsNullException>(
            () => FSharp.fs(result)
        );
    }
    
    [Fact]
    public void OKResult_to_Either()
    {
        var result = Microsoft.FSharp.Core.FSharpResult<int, string>.NewOk(123);
        
        var either = FSharp.fs(result);
        
        Prelude.match(either,
            r => Assert.True(r == 123),
            l => Assert.False(true, "Shouldn't get here"));
    }
    
    [Fact]
    public void Either_to_ErrorResult()
    {
        var either = Either<string, int>.Left("Error");
        
        var result = FSharp.fs(either);
        
        Assert.True(result.IsError);
        Assert.True(result.ErrorValue == "Error");
    }
    
    [Fact]
    public void Either_to_OkResult()
    {
        var either = Either<string, int>.Right(123);
        
        var result = FSharp.fs(either);
        
        Assert.True(result.IsOk);
        Assert.True(result.ResultValue == 123);
    }
    
    [Theory]
    [InlineData("Error")]
    [InlineData(null)]
    public void EitherUnsafe_to_ErrorResult(string error)
    {
        var either = EitherUnsafe<string, int>.Left(error);
        
        var result = FSharp.fs(either);
        
        Assert.True(result.IsError);
        Assert.True(result.ErrorValue == error);
    }
    
    [Fact]
    public void EitherUnsafe_to_OkResult()
    {
        var either = EitherUnsafe<string, int>.Right(123);
        
        var result = FSharp.fs(either);
        
        Assert.True(result.IsOk);
        Assert.True(result.ResultValue == 123);
    }

    }
}
