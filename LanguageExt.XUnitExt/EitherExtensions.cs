using Xunit;

namespace LanguageExt;

public static class EitherExtensions
{
    public static void AssertLeft<L, R>(this Either<L, R> ma) =>
        AssertLeft(ma, "Expected  to be in a Left state");

    public static void AssertLeft<L, R>(this Either<L, R> ma, string userMessage) =>
        Assert.True(ma.IsLeft, userMessage);

    public static void AssertRight<L, R>(this Either<L, R> ma) =>
        AssertRight(ma, "Expected to be in a Right state");

    public static void AssertRight<L, R>(this Either<L, R> ma, string userMessage) =>
        Assert.True(ma.IsRight, userMessage);
    
    public static void AssertLeft<L, R>(this Either<L, R> ma, L expected) =>
        AssertLeft(ma, expected, $"Expected to be in a Left state with a value of {expected}");

    public static void AssertLeft<L, R>(this Either<L, R> ma, L expected, string userMessage) =>
        Assert.True(ma.IsLeft && (expected?.Equals((L?)ma) ?? false), userMessage);

    public static void AssertRight<L, R>(this Either<L, R> ma, R expected) =>
        AssertRight(ma, expected, $"Expected to be in a Right state with a value of {expected}");

    public static void AssertRight<L, R>(this Either<L, R> ma, R expected, string userMessage) =>
        Assert.True(ma.IsRight && (expected?.Equals((R?)ma) ?? false), userMessage);
    
    public static void AssertLeft<L, R>(this Either<L, R> ma, Func<L, bool> predicate) =>
        AssertLeft(ma, predicate, "Expected to be in a Left state with a predicate that returns true");

    public static void AssertLeft<L, R>(this Either<L, R> ma, Func<L, bool> predicate, string userMessage) =>
        Assert.True(ma.IsLeft && predicate((L)ma), userMessage);

    public static void AssertRight<L, R>(this Either<L, R> ma, Func<R, bool> predicate) =>
        AssertRight(ma, predicate, "Expected to be in a Right state with a predicate that returns true");

    public static void AssertRight<L, R>(this Either<L, R> ma, Func<R, bool> predicate, string userMessage) =>
        Assert.True(ma.IsRight && predicate((R)ma), userMessage);    
}
