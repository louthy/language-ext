using System;
using Xunit;
using LanguageExt.Common;

namespace LanguageExt.Tests.IOTests;

public class IO_MapFailTests
{
    [Fact]
    public void MapFail_ShouldNotAlterSuccessValue()
    {
        // Arrange
        var ioValue = IO.pure(10); // Successful computation
        Func<Error, Error> errorMapper = error => Error.New("Mapped Error"); // Transformation function (not used here)
        
        // Act
        var result = ioValue.MapFail(errorMapper).Run(); // Run the computation

        // Assert
        Assert.Equal(10, result); // Ensure the success value is unchanged
    }

    [Fact]
    public void MapFail_ShouldTransformFailure()
    {
        // Arrange
        var initialError = Error.New("Initial Error");
        var failedIO = IO.fail<int>(initialError); // Failing computation

        // Transformation function to map the error
        Func<Error, Error> errorMapper = error => Error.New($"{error.Message} -> Mapped Error");

        // Act & Assert
        var ex = Assert.ThrowsAny<ErrorException>(() => failedIO.MapFail(errorMapper).Run());

        // Assert
        Assert.NotNull(ex);
        Assert.Equal("Initial Error -> Mapped Error", ex.ToError().Message); // Verify the error was transformed
    }

    [Fact]
    public void MapFail_ShouldHandleNestedErrors()
    {
        // Arrange
        var nestedError = Error.New("Nested Error");
        var failedIO    = IO.fail<int>(nestedError);

        // Transformation function that appends extra context to the error
        Func<Error, Error> errorMapper = error => Error.New($"{error.Message}, with additional info");

        // Act & Assert
        var ex = Assert.ThrowsAny<ErrorException>(() => failedIO.MapFail(errorMapper).Run());

        // Assert
        Assert.NotNull(ex);
        Assert.Equal("Nested Error, with additional info", ex.ToError().Message); // Verify error transformation
    }
    
    [Fact]
    public void MapFail_ShouldNotSwallowExceptionsOnMappingFailure()
    {
        // Arrange
        var initialError = Error.New("Original Error");
        var failedIO     = IO.fail<int>(initialError); // Failing computation
        
        // Transformation function that throws an exception
        Func<Error, Error> errorMapper = error => throw new InvalidOperationException("Transformation failed");

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => failedIO.MapFail(errorMapper).Run());

        // Assert
        Assert.Equal("Transformation failed", ex.Message); // Verify the exception from the mapping function
    }
}
