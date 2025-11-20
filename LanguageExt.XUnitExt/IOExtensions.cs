using LanguageExt.Common;
using LanguageExt.Sys.Test;
using Xunit;

namespace LanguageExt;

public static class IOExtensions
{
    public static void AssertFail<A>(this IO<A> ma) => 
        ma.AssertFail(_ => true, "Expected to be in a Fail state");

    public static void AssertFail<A>(this IO<A> ma, string userMessage) =>
        ma.AssertFail(_ => true, userMessage);

    public static void AssertSucc<A>(this IO<A> ma) =>
        ma.AssertSucc(_ => true, "Expected to be in a Succ state");
    
    public static void AssertSucc<A>(this IO<A> ma, string userMessage) =>
        ma.AssertSucc(_ => true, userMessage);
    
    public static void AssertFail<A>(this IO<A> ma, Error expected) =>
        ma.AssertFail(e => e.Is(expected), 
                      e => $"Expected to be in a Fail state with an Error that equals '{expected}', instead got: '{e}'");

    public static void AssertFail<A>(this IO<A> ma, Error expected, string userMessage) =>
        ma.AssertFail(e => e.Is(expected), userMessage);

    public static void AssertSucc<A>(this IO<A> ma, A expected) =>
        ma.AssertSucc(x => expected?.Equals(x) ?? false, 
                      x => $"Expected to be in a Succ state with a result that equals {expected}, instead got: {x}");

    public static void AssertSucc<A>(this IO<A> ma, A expected, string userMessage) =>
        ma.AssertSucc(x => expected?.Equals(x) ?? false, userMessage);
    
    public static void AssertFail<A>(this IO<A> ma, Func<Error, bool> predicate)
    {
        try
        {
            ma.Run().Ignore();
        }
        catch (Exception e)
        {
            Assert.True(predicate(Error.New(e)), "Expected to be in a Fail state with a predicate that returns true");
        }
    }

    public static void AssertFail<A>(this IO<A> ma, Func<Error, bool> predicate, string userMessage)
    {
        try
        {
            ma.Run().Ignore();
        }
        catch (Exception e)
        {
            Assert.True(predicate(Error.New(e)), userMessage);
        }
    }

    public static void AssertFail<A>(this IO<A> ma, Func<Error, bool> predicate, Func<Error, string> userMessage)
    {
        try
        {
            ma.Run().Ignore();
        }
        catch (Exception e)
        {
            Assert.True(predicate(e), userMessage(e));
        }
    }

    public static void AssertSucc<A>(this IO<A> ma, Func<A, bool> predicate) =>
        Assert.True(predicate(ma.Run()), "Expected to be in a Succ state with a predicate that returns true");

    public static void AssertSucc<A>(this IO<A> ma, Func<A, bool> predicate, string userMessage) => 
        Assert.True(predicate(ma.Run()), userMessage);

    public static void AssertSucc<A>(this IO<A> ma, Func<A, bool> predicate, Func<A, string> userMessage)
    {
        var x = ma.Run();
        Assert.True(predicate(x), userMessage(x));
    }
}
