using Xunit;

namespace LanguageExt.Tests;

public class EitherCoalesceTests
{
    [Fact]
    public void EitherCoalesceTest1()
    {
        Either<string, int> either = 123;

        var value = either || 456;
        Assert.True(value == 123);
    }

    [Fact]
    public void EitherCoalesceTest2()
    {
        Either<string, int> either = "Hello";

        var value = either || 456;
        Assert.True(value == 456);
    }

    [Fact]
    public void EitherCoalesceTest3()
    {
        Either<string, int> either1 = "Hello";
        Either<string, int> either2 = "World";

        var value = either1 || either2 || 456;
        Assert.True(value == 456);
    }
}
