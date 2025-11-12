using LanguageExt.Common;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LanguageExt.Tests.IOTests;

public class IO_GeneralTests
{
    [Fact]
    public void Pure_ShouldReturnCorrectValue()
    {
        // Arrange
        var value = 42;
        var io = IO.pure(value);

        // Act
        var result = io.Run();

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Fail_ShouldThrowError()
    {
        // Arrange
        var error = Error.New("Test error");
        var io = IO.fail<int>(error);

        // Act & Assert
        var ex = Assert.ThrowsAny<ExpectedException>(() => io.Run());
        Assert.Equal(error, ex.ToError());
    }

    [Fact]
    public void Map_ShouldTransformValue()
    {
        // Arrange
        var io = IO.pure(5);

        // Act
        var result = io.Map(x => x * 2).Run();

        // Assert
        Assert.Equal(10, result);
    }

    [Fact]
    public void Bind_ShouldChainComputations()
    {
        // Arrange
        var io = IO.pure(10);

        // Act
        var result = io.Bind(x => IO.pure(x + 20)).Run();

        // Assert
        Assert.Equal(30, result);
    }

    [Fact]
    public void Catch_ShouldHandleErrorsGracefully()
    {
        // Arrange
        var io = IO.fail<int>(Error.New("Test error"));

        // Act
        var result = io.IfFail(42).Run();

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void Select_ShouldMapValue_WhenUsingLINQ()
    {
        // Arrange
        var io = IO.pure(4);

        // Act
        var result = from x in io select x * 2;

        // Assert
        Assert.Equal(8, result.Run());
    }

    [Fact]
    public void SelectMany_ShouldChainOperations_WhenUsingLINQ()
    {
        // Arrange
        var io1 = IO.pure(5);
        var io2 = IO.pure(3);

        // Act
        var result = from x in io1
                     from y in io2
                     select x + y;

        // Assert
        Assert.Equal(8, result.Run());
    }

    [Fact]
    public void Retry_ShouldRetryOnFailure_AndSucceedEventually()
    {
        // Arrange
        var count = 0;
        var io = IO.lift(() =>
        {
            if (count++ < 2) throw new Exception("Retry test");
            return 42;
        });

        // Act
        var result = io.Retry().Run();

        // Assert
        Assert.Equal(42, result);
        Assert.Equal(3, count); // 2 failures + 1 success
    }

    [Fact]
    public void Repeat_ShouldRepeatUntilConditionMet()
    {
        // Arrange
        var counter = 0;
        var io = IO.lift(() => ++counter);

        // Act
        var result = io.RepeatUntil(x => x == 5).Run();

        // Assert
        Assert.Equal(5, result);
        Assert.Equal(5, counter); // IO should repeat until the predicate is satisfied
    }

    [Fact]
    public void Timeout_ShouldThrowException_WhenTimeLimitExceeded()
    {
        // Arrange
        var io = IO.lift(() =>
        {
            Task.Delay(1000).Wait();
            return 42;
        });

        // Act & Assert
        Assert.Throws<TaskCanceledException>(() => io.Timeout(TimeSpan.FromMilliseconds(100)).Run());
    }
}
