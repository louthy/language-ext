using Xunit;

namespace LanguageExt;

public static class OptionExtensions
{
    public static void AssertNone<A>(this Option<A> ma) =>
        AssertNone(ma, "Expected to be in a None state");

    public static void AssertNone<A>(this Option<A> ma, string userMessage) =>
        Assert.True(ma.IsNone, userMessage);

    public static void AssertSome<A>(this Option<A> ma) =>
        AssertSome(ma, "Expected to be in a Some state");

    public static void AssertSome<A>(this Option<A> ma, string userMessage) =>
        Assert.True(ma.IsSome, userMessage);
    
    public static void AssertSome<A>(this Option<A> ma, A expected) =>
        AssertSome(ma, expected, $"Expected to be in a Some state with a value of {expected}");

    public static void AssertSome<A>(this Option<A> ma, A expected, string userMessage) =>
        Assert.True(ma.IsSome && expected.Equals((A)ma), userMessage);
    
    public static void AssertSome<A>(this Option<A> ma, Func<A, bool> predicate) =>
        AssertSome(ma, predicate, "Expected to be in a Some state with a predicate that returns true");

    public static void AssertSome<A>(this Option<A> ma, Func<A, bool> predicate, string userMessage) =>
        Assert.True(ma.IsSome && predicate((A)ma), userMessage);    
}
