using System.Collections.Generic;
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
        var value  = source.Reduce(0, (s, _) => s + 1).Run();
        Assert.Equal(0, value);
    }
    
    [Fact]
    public void Pure_Source_Should_Produce_Single_Value()
    {
        // Arrange
        var value  = 42;
        var source = Source.pure(value);
        var output = source.Collect().Run();

        // Act and Assert
        Assert.True(output == [42]);
    }

    [Fact]
    public void Map_Should_Transform_Values()
    {
        // Arrange
        var source = Source.pure(42).Map(x => x * 2);
        var output = source.Collect().Run();

        // Act and Assert
        Assert.True(output == [84]);
    }
    
    [Fact]
    public void Where_Should_Filter_Values()
    {
        // Arrange
        var source = Source.pure(42).Where(x => x > 50);
        var output = source.Collect().Run();

        // Act and Assert
        Assert.True(output == []);
    }

    [Fact]
    public void Add_Should_Concatenate_Sources()
    {
        // Arrange
        var source1 = Source.pure(1);
        var source2 = Source.pure(2);

        // Act
        var concat = source1 + source2;
        var output = concat.Collect().Run();

        // Assert
        Assert.Equal(2, output.Count);
        Assert.True(output == [1, 2]);
    }
    
    [Fact]
    public void Zip_Should_Combine_Two_Sources()
    {
        // Arrange
        var source1 = Source.pure(1);
        var source2 = Source.pure("A");

        // Act
        var zipped = source1.Zip(source2);
        var output = zipped.Collect().Run();

        // Assert
        Assert.True(output == [(1, "A")]);
    }
    
    
    /*

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
