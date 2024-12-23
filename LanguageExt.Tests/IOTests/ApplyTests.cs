using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using Xunit;

namespace LanguageExt.Tests.IOTests;

public class IO_ApplyTests
{
    [Fact]
    public void Apply_ShouldApplyFunctionToValue()
    {
        // Arrange
        var ioValue    = IO.pure(5);                // IO computation with a value of 5
        var ioFunction = IO.pure((int x) => x * 2); // IO computation with a function (x => x * 2)

        // Act
        var appliedIO = ioFunction.Apply(ioValue); // Apply function to value
        var result    = appliedIO.Run();           // Perform the computation to get the result

        // Assert
        Assert.Equal(10, result); // Ensure the function application happened correctly
    }

    [Fact]
    public void Apply_ShouldBubbleUpFailure()
    {
        // Arrange
        var failError  = Error.New("Failed IO");
        var failedIO   = IO.fail<int>(failError);   // Failing IO computation
        var ioFunction = IO.pure((int x) => x * 2); // IO computation with a function

        // Act & Assert
        // Since the IO computation failed, the result must throw the expected error
        var ex = Assert.ThrowsAny<ErrorException>(() => ioFunction.Apply(failedIO).Run());
        Assert.Equal(failError, ex.ToError()); // Check that the bubbled-up error matches
    }

    [Fact]
    public void Apply_ShouldNotRunFunctionIfFunctionIsFailed()
    {
        // Arrange
        var failError      = Error.New("Failed Function");
        var ioValue        = IO.pure(5);
        var failedFunction = IO.fail<Func<int, int>>(failError);

        // Act & Assert
        // The failed function should result in a failure even if the value is valid
        var ex = Assert.ThrowsAny<ErrorException>(() => failedFunction.Apply(ioValue).Run());
        Assert.Equal(failError, ex.ToError()); // Check the error matches the failed function's error
    }    
    
    [Fact]
    public async Task Apply_WithAsyncAndSyncTermsShouldRunInParallel()
    {
        var af = Atom(0);
        var aa = Atom(0);
        
        // Create a task that takes longer to return than `fa`.  This relies on `fa` completing first
        // so that aa.Value has been set.  This proves that the applicative parallel behaviour is working 
        var ff = IO.liftAsync(() => Task.Delay(100)
                                        .ToUnit()
                                        .Map(_ => af.Swap(x => x + aa.Value))
                                        .Map(_ => new Func<int, int>(x => x * af.Value)));
        
        var fa = IO.liftAsync(() => Task.Delay(10)
                                        .ToUnit()
                                        .Map(_ => aa.Swap(x => x + 5)));
        
        var fr = ff.Apply(fa);
        var r  = await fr.RunAsync();
        Assert.Equal(25, r);
    }
    
    [Fact]
    public async Task Apply_WithAsyncAndSyncTermsShouldApplyFunctionToValue()
    {
        var af = Atom(0);
        var aa = Atom(0);
        
        // Create a task that takes longer to return than `fa`.  This relies on `fa` completing first
        // so that aa.Value has been set.  This proves that the applicative parallel behaviour is working 
        var ff = IO.liftAsync(() => Task.Delay(100)
                                        .ToUnit()
                                        .Map(_ => af.Swap(x => x + aa.Value))
                                        .Map(_ => new Func<int, int>(x => x * af.Value)));
        
        var fa = IO.lift(() => aa.Swap(x => x + 5));
        
        var fr = ff.Apply(fa);
        var r  = await fr.RunAsync();
        Assert.Equal(25, r);
    }
    
    [Fact]
    public async Task Apply_WithSyncAndAsyncTermsShouldApplyFunctionToValue()
    {
        var ff = IO.liftAsync(() => Task.Delay(100)
                                        .ToUnit()
                                        .Map(_ => new Func<int, int>(x => x * 5)));
        var fa = IO.pure(5);
        var fr = ff.Apply(fa);
        var r  = await fr.RunAsync();
        Assert.Equal(25, r);
    }
}
