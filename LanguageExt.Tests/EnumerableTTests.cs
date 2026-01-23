using System.Collections;
using System.Linq;
using LanguageExt.ClassInstances;
using Xunit;

namespace LanguageExt.Tests;

public class EnumerableTTests
{
    [Fact]
    public void ChooseTest()
    {
        var input = Lst(
            Some(1),
            Some(2),
            Some(3),
            None,
            Some(4),
            None,
            Some(5));

        var actual = input.Choose(x => x).ToLst();

        var expected = Lst(1, 2, 3, 4, 5);

        var toString = fun((IEnumerable items) => string.Join(", ", items));

        Assert.True(EqEnumerable<int>.Equals(actual, expected), $"Expected {toString(expected)} but was {toString(actual)}");
    }
}
