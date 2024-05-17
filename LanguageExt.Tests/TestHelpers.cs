using System;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Sys.Test;
using Xunit;

namespace LanguageExt;

public static class AssertExt
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Either helpers
    //
    
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
        Assert.True(ma.IsLeft && expected.Equals((L)ma), userMessage);

    public static void AssertRight<L, R>(this Either<L, R> ma, R expected) =>
        AssertRight(ma, expected, $"Expected to be in a Right state with a value of {expected}");

    public static void AssertRight<L, R>(this Either<L, R> ma, R expected, string userMessage) =>
        Assert.True(ma.IsRight && expected.Equals((R)ma), userMessage);
    
    public static void AssertLeft<L, R>(this Either<L, R> ma, Func<L, bool> predicate) =>
        AssertLeft(ma, predicate, "Expected to be in a Left state with a predicate that returns true");

    public static void AssertLeft<L, R>(this Either<L, R> ma, Func<L, bool> predicate, string userMessage) =>
        Assert.True(ma.IsLeft && predicate((L)ma), userMessage);

    public static void AssertRight<L, R>(this Either<L, R> ma, Func<R, bool> predicate) =>
        AssertRight(ma, predicate, "Expected to be in a Right state with a predicate that returns true");

    public static void AssertRight<L, R>(this Either<L, R> ma, Func<R, bool> predicate, string userMessage) =>
        Assert.True(ma.IsRight && predicate((R)ma), userMessage);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Option helpers
    //
    
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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Fin helpers
    //
    
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
        Assert.True(ma.IsSucc && expected.Equals((A)ma), userMessage);
    
    public static void AssertFail<A>(this Fin<A> ma, Func<Error, bool> predicate) =>
        AssertFail(ma, predicate, "Expected to be in a Fail state with a predicate that returns true");

    public static void AssertFail<A>(this Fin<A> ma, Func<Error, bool> predicate, string userMessage) =>
        Assert.True(ma.IsFail && predicate((Error)ma), userMessage);

    public static void AssertSucc<A>(this Fin<A> ma, Func<A, bool> predicate) =>
        AssertSucc(ma, predicate, "Expected to be in a Succ state with a predicate that returns true");

    public static void AssertSucc<A>(this Fin<A> ma, Func<A, bool> predicate, string userMessage) =>
        Assert.True(ma.IsSucc && predicate((A)ma), userMessage);    

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Eff helpers
    //

    public static void AssertFail<A>(this Eff<Runtime, A> ma) =>
        ma.Run(Runtime.New(), EnvIO.New()).AssertFail();

    public static void AssertFail<A>(this Eff<Runtime, A> ma, string userMessage) =>
        ma.Run(Runtime.New(), EnvIO.New()).AssertFail(userMessage);

    public static void AssertSucc<A>(this Eff<Runtime, A> ma) =>
        ma.Run(Runtime.New(), EnvIO.New()).AssertSucc();

    public static void AssertSucc<A>(this Eff<Runtime, A> ma, string userMessage) =>
        ma.Run(Runtime.New(), EnvIO.New()).AssertSucc(userMessage);
    
    public static void AssertFail<A>(this Eff<Runtime, A> ma, Error expected) =>
        ma.Run(Runtime.New(), EnvIO.New()).AssertFail(expected);

    public static void AssertFail<A>(this Eff<Runtime, A> ma, Error expected, string userMessage) =>
        ma.Run(Runtime.New(), EnvIO.New()).AssertFail(expected, userMessage);

    public static void AssertSucc<A>(this Eff<Runtime, A> ma, A expected) =>
        ma.Run(Runtime.New(), EnvIO.New()).AssertSucc(expected);

    public static void AssertSucc<A>(this Eff<Runtime, A> ma, A expected, string userMessage) =>
        ma.Run(Runtime.New(), EnvIO.New()).AssertSucc(expected, userMessage);
    
    public static void AssertFail<A>(this Eff<Runtime, A> ma, Func<Error, bool> predicate) =>
        ma.Run(Runtime.New(), EnvIO.New()).AssertFail(predicate);

    public static void AssertFail<A>(this Eff<Runtime, A> ma, Func<Error, bool> predicate, string userMessage) =>
        ma.Run(Runtime.New(), EnvIO.New()).AssertFail(predicate, userMessage);

    public static void AssertSucc<A>(this Eff<Runtime, A> ma, Func<A, bool> predicate) =>
        ma.Run(Runtime.New(), EnvIO.New()).AssertSucc(predicate);

    public static void AssertSucc<A>(this Eff<Runtime, A> ma, Func<A, bool> predicate, string userMessage) =>
        ma.Run(Runtime.New(), EnvIO.New()).AssertSucc(predicate, userMessage);
    

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Eff helpers
    //

    public static void AssertFail<A>(this Eff<A> ma) =>
        ma.Run().AssertFail();

    public static void AssertFail<A>(this Eff<A> ma, string userMessage) =>
        ma.Run().AssertFail(userMessage);

    public static void AssertSucc<A>(this Eff<A> ma) =>
        ma.Run().AssertSucc();

    public static void AssertSucc<A>(this Eff<A> ma, string userMessage) =>
        ma.Run().AssertSucc(userMessage);
    
    public static void AssertFail<A>(this Eff<A> ma, Error expected) =>
        ma.Run().AssertFail(expected);

    public static void AssertFail<A>(this Eff<A> ma, Error expected, string userMessage) =>
        ma.Run().AssertFail(expected, userMessage);

    public static void AssertSucc<A>(this Eff<A> ma, A expected) =>
        ma.Run().AssertSucc(expected);

    public static void AssertSucc<A>(this Eff<A> ma, A expected, string userMessage) =>
        ma.Run().AssertSucc(expected, userMessage);
    
    public static void AssertFail<A>(this Eff<A> ma, Func<Error, bool> predicate) =>
        ma.Run().AssertFail(predicate);

    public static void AssertFail<A>(this Eff<A> ma, Func<Error, bool> predicate, string userMessage) =>
        ma.Run().AssertFail(predicate, userMessage);

    public static void AssertSucc<A>(this Eff<A> ma, Func<A, bool> predicate) =>
        ma.Run().AssertSucc(predicate);

    public static void AssertSucc<A>(this Eff<A> ma, Func<A, bool> predicate, string userMessage) =>
        ma.Run().AssertSucc(predicate, userMessage);
    
}
