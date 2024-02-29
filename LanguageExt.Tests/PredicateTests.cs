using Xunit;
using LanguageExt.ClassInstances.Pred;

namespace LanguageExt.Tests;

public class PredicateTests
{
    public struct NumbersPattern : Const<string>
    {
        public static string Value => "^[0-9]*$";
    }

    [Fact]
    public void StringMatchesPattern() =>
        Assert.True(Matches<NumbersPattern>.True("12345"));

    [Fact]
    public void StringDoesNotMatchPattern() =>
        Assert.False(Matches<NumbersPattern>.True("1ABC5"));
}
