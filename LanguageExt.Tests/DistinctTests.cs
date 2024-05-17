using Xunit;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests;

public class DistinctTests
{
    [Fact]
    void SeqDistinctIgnoreCase()
    {
        var items = Seq("Test", "other", "test");
            
        Assert.Equal(items, items.Distinct());
        Assert.Equal(items.Take(3), items.Distinct(_ => _, fun<string, string, bool>(EqStringOrdinal.Equals)));
        Assert.Equal(items.Take(2), items.Distinct(_ => _, fun<string, string, bool>(EqStringOrdinalIgnoreCase.Equals)));
    }
}
