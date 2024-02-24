using Xunit;
using System;
using LanguageExt;
using LanguageExt.Common;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace EffTests;

public class EffMapFailTests
{
    [Fact] // Fails
    public void Eff_WithFailingTaskInEff_EndsWithMapFailValue()
    {
        // Arrange
        var affWithException       = liftEff(async () => await FailingTask());
        var affWithMappedFailState = affWithException.MapFail(error => Error.New(error.Message + "MapFail Eff"));

        // Act
        var fin    = affWithMappedFailState.Run();
        var result = fin.Match(_ => "Success!", error => error.Message);

        // Assert
        Assert.True(result.IndexOf("MapFail Eff", StringComparison.Ordinal) > 0); 
    }

    [Fact] // Fails
    public void Eff_WithExceptionInEff_EndsWithMapFailValue()
    {
        // Arrange
        var affWithException       = lift(() => throw new Exception()).ToEff();
        var affWithMappedFailState = affWithException.MapFail(error => Error.New(error.Message + "MapFail Eff"));

        // Act
        var fin    = affWithMappedFailState.Run();
        var result = fin.Match(_ => "Success!", error => error.Message);

        // Assert
        Assert.True(result.IndexOf("MapFail Eff", StringComparison.Ordinal) > 0); 
    }

    [Fact] // Fails
    public void Eff_FailingTaskToEff_EndsWithMapFailValue()
    {
        // Arrange
        var affWithException       = liftEff(async () => await FailingTask());
        var affWithMappedFailState = affWithException.MapFail(error => Error.New(error.Message + "MapFail Eff"));

        // Act
        var fin    = affWithMappedFailState.Run();
        var result = fin.Match(_ => "Success!", error => error.Message);

        // Assert
        Assert.True(result.IndexOf("MapFail Eff", StringComparison.Ordinal) > 0); 
    }

    [Fact] // Succeeds
    public void EffToEff_WithExceptionInEff_EndsWithMapFailValue()
    {
        // Arrange
        var effWithException       = lift<Unit>(() => throw new Exception("Error!")).ToEff();
        var effWithMappedFailState = effWithException.MapFail(error => Error.New(error.Message + "MapFail Eff"));

        // Act
        var fin    = effWithMappedFailState.Run();
        var result = fin.Match(_ => "Success!", error => error.Message);

        // Assert
        Assert.True(result.IndexOf("MapFail Eff", StringComparison.Ordinal) > 0); 
    }

    private static async Task<Unit> FailingTask()
    {
        await Task.Delay(1);
        throw new Exception("Error!");
    }
}
