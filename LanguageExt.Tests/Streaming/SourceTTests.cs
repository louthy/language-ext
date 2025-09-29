using System.Linq;
using Xunit;

namespace LanguageExt.Tests.Streaming;

public class SourceTTests
{
    [Fact]
    public void Skip_Should_Skip_Specified_Number_Of_Elements()
    {
        var source = SourceT.lift<IO, int>(Enumerable.Range(1, 5)).Skip(2).Collect().Run();
        Assert.Equal([3, 4, 5], source);
    }

    [Fact]
    public void Take_Should_Take_Specified_Number_Of_Elements_With_TransducerM()
    {
        var source = SourceT.lift<IO, int>(Enumerable.Range(1, 5)).Take(2).Collect().Run();
        Assert.Equal([1, 2], source);
    }
}
