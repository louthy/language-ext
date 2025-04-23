using System.Threading.Tasks;

namespace LanguageExt;

/// <summary>
/// Result of a `Reducer` delegate
/// </summary>
public readonly record struct Reduced<A>(bool Continue, A Value);

/// <summary>
/// `Reduced〈A〉` constructors
/// </summary>
public static class Reduced
{
    static Reduced()
    {
        Unit = Continue<Unit>(default);
        UnitAsync = new ValueTask<Reduced<Unit>>(Unit);
    }
    
    /// <summary>
    /// Return a result and indicate that we're happy to continue
    /// </summary>
    /// <param name="value">Result</param>
    /// <typeparam name="A">Result value type</typeparam>
    /// <returns>`Reduced` structure in a continue state</returns>
    public static Reduced<A> Continue<A>(A value) =>
        new(true, value);
    
    /// <summary>
    /// Return a result and indicate that we're done and don't want to process any more
    /// </summary>
    /// <param name="value">Result</param>
    /// <typeparam name="A">Result value type</typeparam>
    /// <returns>`Reduced` structure in a done state</returns>
    public static Reduced<A> Done<A>(A value) =>
        new(false, value);

    /// <summary>
    /// `Reduced〈A〉` in a continue state
    /// </summary>
    public static readonly Reduced<Unit> Unit;
    
    /// <summary>
    /// Return a result and indicate that we're happy to continue
    /// </summary>
    /// <param name="value">Result</param>
    /// <typeparam name="A">Result value type</typeparam>
    /// <returns>`Reduced` structure in a continue state</returns>
    public static ValueTask<Reduced<A>> ContinueAsync<A>(A value) =>
        new(new Reduced<A>(true, value));
    
    /// <summary>
    /// Return a result and indicate that we're done and don't want to process any more
    /// </summary>
    /// <param name="value">Result</param>
    /// <typeparam name="A">Result value type</typeparam>
    /// <returns>`Reduced` structure in a done state</returns>
    public static ValueTask<Reduced<A>> DoneAsync<A>(A value) =>
        new(new Reduced<A>(false, value));

    /// <summary>
    /// `Reduced〈A〉` in a continue state
    /// </summary>
    public static readonly ValueTask<Reduced<Unit>> UnitAsync;
}
