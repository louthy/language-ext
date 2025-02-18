using LanguageExt.Common;
using LanguageExt.Pipes.Concurrent;
using LanguageExt.XUnitExt;
using Xunit;

namespace LanguageExt.Tests.Streaming;

public class SourceTests
{
    [Fact]
    public void Empty_Source_Should_Not_Produce_Values()
    {
        // Arrange
        var source = Source.empty<int>();
        var iter   = source.GetIterator();

        // Act and Assert 
        XAssert.Throws(Errors.SourceClosed, () => iter.Read().Run());
    }

    [Fact]
    public void Pure_Source_Should_Produce_Single_Value()
    {
        // Arrange
        var value  = 42;
        var source = Source.pure(value);
        var iter   = source.GetIterator();

        // Act
        var result = iter.Read().Run();

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void Map_Should_Transform_Values()
    {
        // Arrange
        var source = Source.pure(42).Map(x => x * 2);
        var iter   = source.GetIterator();

        // Act
        var result = iter.Read().Run();

        // Assert
        Assert.Equal(84, result);
    }

    [Fact]
    public void Where_Should_Filter_Values()
    {
        // Arrange
        var source = Source.pure(42).Where(x => x > 50);
        var iter   = source.GetIterator();

        // Act and Assert
        XAssert.Throws(Errors.SourceClosed, () => iter.Read().Run());
    }

    /*
    [Fact]
    public async Task Merge_Should_Combine_Sources()
    {
        // Arrange
        var source1 = Source.pure(1);
        var source2 = Source.pure(2);

        // Act
        var merged = Source.merge(source1, source2);
        var results = new List<int>();

        var result1 = await merged.Read().Run();
        result1.IfSome(v => results.Add(v));

        var result2 = await merged.Read().Run();
        result2.IfSome(v => results.Add(v));

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Contains(1, results);
        Assert.Contains(2, results);
    }

    [Fact]
    public async Task Zip_Should_Combine_Two_Sources()
    {
        // Arrange
        var source1 = Source.pure(1);
        var source2 = Source.pure("A");

        // Act
        var zipped = source1.Zip(source2);
        var result = await zipped.Read().Run();

        // Assert
        Assert.True(result.IsSome);
        result.IfSome(v =>
        {
            Assert.Equal(1, v.First);
            Assert.Equal("A", v.Second);
        });
    }

    [Fact]
    public async Task Select_Should_Transform_Values()
    {
        // Arrange
        var source = Source.pure(42);

        // Act
        var transformed = source.Select(x => x.ToString());
        var result = await transformed.Read().Run();

        // Assert
        Assert.True(result.IsSome);
        result.IfSome(v => Assert.Equal("42", v));
    }

    [Fact]
    public async Task ReadyToRead_Should_Return_True_For_NonEmpty_Source()
    {
        // Arrange
        var source = Source.pure(42);

        // Act
        var ready = await source.ReadyToRead(CancellationToken.None);

        // Assert
        Assert.True(ready);
    }

    [Fact]
    public async Task ReadValue_Should_Return_Value_For_NonEmpty_Source()
    {
        // Arrange
        var source = Source.pure(42);

        // Act
        var value = await source.ReadValue(CancellationToken.None);

        // Assert
        Assert.Equal(42, value);
    }*/

}
