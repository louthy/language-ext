using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.XUnitExt;

public static class XAssert
{
    /// <summary>
    /// Asserts that the action throws an `Error`
    /// </summary>
    public static Unit Throws<A>(Error error, Func<A> action)
    {
        try
        {
            action();
            throw new Exception("Expected error: " + error + ", but got none");
        }
        catch (Exception e)
        {
            if (Error.New(e).Is(error))
            {
                return unit;
            }
            else
            {
                throw new Exception("Expected error: " + error + ", but got: " + e);
            }
        }
    }
    
    /// <summary>
    /// Asserts that the action throws an `Error`
    /// </summary>
    public static Unit Throws(Error error, Action action)
    {
        try
        {
            action();
            throw new Exception("Expected error: " + error + ", but got none");
        }
        catch (Exception e)
        {
            if (Error.New(e).Is(error))
            {
                return unit;
            }
            else
            {
                throw new Exception("Expected error: " + error + ", but got: " + e);
            }
        }
    }
}
