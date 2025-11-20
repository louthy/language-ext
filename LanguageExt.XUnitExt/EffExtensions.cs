using LanguageExt.Common;
using LanguageExt.Sys.Test;

namespace LanguageExt;

public static class EffExtensions
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Eff helpers
    //

    public static void AssertFail<A>(this Eff<Runtime, A> ma)
    {
        using var rt = Runtime.New();
        ma.Run(rt, EnvIO.New()).AssertFail();
    }

    public static void AssertFail<A>(this Eff<Runtime, A> ma, string userMessage)
    {
        using var rt = Runtime.New();
        ma.Run(rt, EnvIO.New()).AssertFail(userMessage);
    }

    public static void AssertSucc<A>(this Eff<Runtime, A> ma)
    {
        using var rt = Runtime.New();
        ma.Run(rt, EnvIO.New()).AssertSucc();
    }

    public static void AssertSucc<A>(this Eff<Runtime, A> ma, string userMessage)
    {
        using var rt = Runtime.New();
        ma.Run(rt, EnvIO.New()).AssertSucc(userMessage);
    }

    public static void AssertFail<A>(this Eff<Runtime, A> ma, Error expected)
    {
        using var rt = Runtime.New();
        ma.Run(rt, EnvIO.New()).AssertFail(expected);
    }

    public static void AssertFail<A>(this Eff<Runtime, A> ma, Error expected, string userMessage)
    {
        using var rt = Runtime.New();
        ma.Run(rt, EnvIO.New()).AssertFail(expected, userMessage);
    }

    public static void AssertSucc<A>(this Eff<Runtime, A> ma, A expected)
    {
        using var rt = Runtime.New();
        ma.Run(rt, EnvIO.New()).AssertSucc(expected);
    }

    public static void AssertSucc<A>(this Eff<Runtime, A> ma, A expected, string userMessage)
    {
        using var rt = Runtime.New();
        ma.Run(rt, EnvIO.New()).AssertSucc(expected, userMessage);
    }

    public static void AssertFail<A>(this Eff<Runtime, A> ma, Func<Error, bool> predicate)
    {
        using var rt = Runtime.New();
        ma.Run(rt, EnvIO.New()).AssertFail(predicate);
    }

    public static void AssertFail<A>(this Eff<Runtime, A> ma, Func<Error, bool> predicate, string userMessage)
    {
        using var rt = Runtime.New();
        ma.Run(rt, EnvIO.New()).AssertFail(predicate, userMessage);
    }

    public static void AssertSucc<A>(this Eff<Runtime, A> ma, Func<A, bool> predicate)
    {
        using var rt = Runtime.New();
        ma.Run(rt, EnvIO.New()).AssertSucc(predicate);
    }

    public static void AssertSucc<A>(this Eff<Runtime, A> ma, Func<A, bool> predicate, string userMessage)
    {
        using var rt = Runtime.New();
        ma.Run(rt, EnvIO.New()).AssertSucc(predicate, userMessage);
    }
    
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
