using Xunit;

namespace LanguageExt.Tests.Streaming;

public class SourceTests
{
    [Fact]
    public void Empty_source_should_not_produce_values()
    {
        // Arrange
        var source = Source.empty<int>();
        var value  = source.FoldReduce(0, (s, _) => s + 1).Run();
        Assert.Equal(0, value);
    }
    
    [Fact]
    public void Pure_source_should_produce_single_value()
    {
        // Arrange
        var value  = 42;
        var source = Source.pure(value);
        var output = source.Collect().Run();

        // Act and Assert
        Assert.True(output == [42]);
    }

    [Fact]
    public void Map_should_transform_values()
    {
        // Arrange
        var source = Source.pure(42).Map(x => x * 2);
        var output = source.Collect().Run();

        // Act and Assert
        Assert.True(output == [84]);
    }
    
    [Fact]
    public void Where_should_filter_values()
    {
        // Arrange
        var source = Source.pure(42).Where(x => x > 50);
        var output = source.Collect().Run();

        // Act and Assert
        Assert.True(output == []);
    }

    [Fact]
    public void Add_should_concatenate_sources()
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
    public void Zip_should_combine_two_sources()
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
    
    [Fact]
    public void Or_operator_should_merge_two_sources()
    {
        // Arrange
        var source1 = Source.lift([1, 2, 3]);
        var source2 = Source.lift([10, 20, 30]);

        // Act
        var merged = source1 | source2;
        var output = merged.Collect().Run();

        // Assert that the output has 3 items lower than 10
        Assert.True(output.Filter(x => x < 10).Count  == 3);

        // Assert that the output has 3 items greater than 10
        Assert.True(output.Filter(x => x >= 10).Count == 3);

        // Assert that the output has the lower items in-order
        Assert.True(output.Filter(x => x < 10)        == [1, 2, 3]);
        
        // Assert that the output has the upper items in-order
        Assert.True(output.Filter(x => x >= 10)       == [10, 20, 30]);
    }    
}
