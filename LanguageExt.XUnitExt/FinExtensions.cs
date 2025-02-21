using LanguageExt.Common;
using Xunit;

namespace LanguageExt;

public static class FinExtensions
{
    public static void AssertFail<A>(this Fin<A> ma) =>
        AssertFail(ma, "Expected to be in a Fail state");

    public static void AssertFail<A>(this Fin<A> ma, string userMessage) =>
        Assert.True(ma.IsFail, userMessage);

    public static void AssertSucc<A>(this Fin<A> ma) =>
        AssertSucc(ma, "Expected to be in a Succ state");

    public static void AssertSucc<A>(this Fin<A> ma, string userMessage) =>
        Assert.True(ma.IsSucc, userMessage);
    
    public static void AssertFail<A>(this Fin<A> ma, Error expected) =>
        AssertFail(ma, expected, $"Expected to be in a Fail state with a value of {expected}");

    public static void AssertFail<A>(this Fin<A> ma, Error expected, string userMessage) =>
        Assert.True(ma.IsFail && expected.Is((Error)ma), userMessage);

    public static void AssertSucc<A>(this Fin<A> ma, A expected) =>
        AssertSucc(ma, expected, $"Expected to be in a Succ state with a value of {expected}");

    public static void AssertSucc<A>(this Fin<A> ma, A expected, string userMessage) =>
        Assert.True(ma.IsSucc && (expected?.Equals((A?)ma) ?? false), userMessage);
    
    public static void AssertFail<A>(this Fin<A> ma, Func<Error, bool> predicate) =>
        AssertFail(ma, predicate, "Expected to be in a Fail state with a predicate that returns true");

    public static void AssertFail<A>(this Fin<A> ma, Func<Error, bool> predicate, string userMessage) =>
        Assert.True(ma.IsFail && predicate((Error)ma), userMessage);

    public static void AssertSucc<A>(this Fin<A> ma, Func<A, bool> predicate) =>
        AssertSucc(ma, predicate, "Expected to be in a Succ state with a predicate that returns true");

    public static void AssertSucc<A>(this Fin<A> ma, Func<A, bool> predicate, string userMessage) =>
        Assert.True(ma.IsSucc && predicate((A)ma), userMessage);        
}
