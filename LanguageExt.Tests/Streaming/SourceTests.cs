using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using LanguageExt.Common;
using LanguageExt.Pipes.Concurrent;

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
        AssertExt.SourceIsClosed(iter);
    }

    [Fact]
    public void Pure_Source_Should_Produce_Single_Value()
    {
        // Arrange
        var value  = 42;
        var source = Source.pure(value);
        var iter   = source.GetIterator();

        // Act and Assert
        iter.CheckReadyAndRead().AssertSucc(42);
        AssertExt.SourceIsClosed(iter);
    }

    [Fact]
    public void Map_Should_Transform_Values()
    {
        // Arrange
        var source = Source.pure(42).Map(x => x * 2);
        var iter   = source.GetIterator();

        // Act and Assert
        iter.CheckReadyAndRead().AssertSucc(84);
    }

    [Fact]
    public void Where_Should_Filter_Values()
    {
        // Arrange
        var source = Source.pure(42).Where(x => x > 50);
        var iter   = source.GetIterator();

        // Act and Assert
        AssertExt.Throws(Errors.SourceClosed, () => iter.CheckReadyAndRead().Run());
    }

    [Fact]
    public void Merge_Should_Combine_Sources()
    {
        // Arrange
        var source1 = Source.pure(1);
        var source2 = Source.pure(2);

        // Act
        var merged  = source1 + source2;
        var iter    = merged.GetIterator();
        var results = new List<int>();

        var x1 = iter.CheckReadyAndRead().Run();
        results.Add(x1);

        var x2 = iter.CheckReadyAndRead().Run();
        results.Add(x2);

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Contains(1, results);
        Assert.Contains(2, results);
    }
/*
    [Fact]
    public async Task Zip_Should_Combine_Two_Sources()
    {
        // Arrange
        var source1 = Source.pure(1);
        var source2 = Source.pure("A");

        // Act
        var zipped = source1.Zip(source2);
        var result = await zipped.CheckReadyAndRead().Run();

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
        var result = await transformed.CheckReadyAndRead().Run();

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
        var ready = await source.CheckReadyAndReadyToRead(CancellationToken.None);

        // Assert
        Assert.True(ready);
    }

    [Fact]
    public async Task ReadValue_Should_Return_Value_For_NonEmpty_Source()
    {
        // Arrange
        var source = Source.pure(42);

        // Act
        var value = await source.CheckReadyAndReadValue(CancellationToken.None);

        // Assert
        Assert.Equal(42, value);
    }*/

}
