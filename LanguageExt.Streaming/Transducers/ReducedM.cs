using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Result of a `Reducer` delegate
/// </summary>
public readonly record struct ReducedM<M, A>(bool Continue, K<M, A> Value);

/// <summary>
/// `Reduced〈A〉` constructors
/// </summary>
public static class ReducedM
{
    /// <summary>
    /// Return a result and indicate that we're happy to continue
    /// </summary>
    /// <param name="value">Result</param>
    /// <typeparam name="A">Result value type</typeparam>
    /// <returns>`ReducedM` structure in a continue state</returns>
    public static ReducedM<M, A> Continue<M, A>(A value) 
        where M : Applicative<M> =>
        new(true, M.Pure(value));
    
    /// <summary>
    /// Return a result and indicate that we're happy to continue
    /// </summary>
    /// <param name="ma">Result</param>
    /// <typeparam name="A">Result value type</typeparam>
    /// <returns>`ReducedM` structure in a continue state</returns>
    public static ReducedM<M, A> Continue<M, A>(K<M, A> ma) 
        where M : Applicative<M> =>
        new(true, ma);
    
    /// <summary>
    /// Return a result and indicate that we're done and don't want to process any more
    /// </summary>
    /// <param name="value">Result</param>
    /// <typeparam name="A">Result value type</typeparam>
    /// <returns>`ReducedM` structure in a done state</returns>
    public static ReducedM<M, A> Done<M, A>(A value) 
        where M : Applicative<M> =>
        new(false, M.Pure(value));
    
    /// <summary>
    /// Return a result and indicate that we're done and don't want to process any more
    /// </summary>
    /// <param name="ma">Result</param>
    /// <typeparam name="A">Result value type</typeparam>
    /// <returns>`ReducedM` structure in a done state</returns>
    public static ReducedM<M, A> Done<M, A>(K<M, A> ma) 
        where M : Applicative<M> =>
        new(false, ma);

    /// <summary>
    /// `ReducedM〈A〉` in a continue state
    /// </summary>
    public static ReducedM<M, Unit> Unit<M>() 
        where M : Applicative<M> =>
        ReducedMInternal<M>.Unit;
    
    /// <summary>
    /// Return a result and indicate that we're happy to continue
    /// </summary>
    /// <param name="value">Result</param>
    /// <typeparam name="A">Result value type</typeparam>
    /// <returns>`ReducedM` structure in a continue state</returns>
    public static ValueTask<ReducedM<M, A>> ContinueAsync<M, A>(A value) 
        where M : Applicative<M> =>
        new(new ReducedM<M, A>(true, M.Pure(value)));
    
    /// <summary>
    /// Return a result and indicate that we're happy to continue
    /// </summary>
    /// <param name="ma">Result</param>
    /// <typeparam name="A">Result value type</typeparam>
    /// <returns>`ReducedM` structure in a continue state</returns>
    public static ValueTask<ReducedM<M, A>> ContinueAsync<M, A>(K<M, A> ma) 
        where M : Applicative<M> =>
        new(new ReducedM<M, A>(true, ma));
    
    /// <summary>
    /// Return a result and indicate that we're done and don't want to process any more
    /// </summary>
    /// <param name="value">Result</param>
    /// <typeparam name="A">Result value type</typeparam>
    /// <returns>`ReducedM` structure in a done state</returns>
    public static ValueTask<ReducedM<M, A>> DoneAsync<M, A>(A value) 
        where M : Applicative<M> =>
        new(new ReducedM<M, A>(false, M.Pure(value)));
    
    /// <summary>
    /// Return a result and indicate that we're done and don't want to process any more
    /// </summary>
    /// <param name="ma">Result</param>
    /// <typeparam name="A">Result value type</typeparam>
    /// <returns>`ReducedM` structure in a done state</returns>
    public static ValueTask<ReducedM<M, A>> DoneAsync<M, A>(K<M, A> ma) 
        where M : Applicative<M> =>
        new(new ReducedM<M, A>(false, ma));

    /// <summary>
    /// `ReducedM〈A〉` in a continue state
    /// </summary>
    public static ValueTask<ReducedM<M, Unit>> UnitAsync<M>()
        where M : Applicative<M> =>
        ReducedMInternal<M>.UnitAsync;
}

/// <summary>
/// `Reduced〈A〉` constructors
/// </summary>
static class ReducedMInternal<M>
    where M : Applicative<M>
{
    static ReducedMInternal()
    {
        Unit = new ReducedM<M, Unit>(true, M.Pure<Unit>(default));
        UnitAsync = new ValueTask<ReducedM<M, Unit>>(Unit);
    }

    /// <summary>
    /// `ReducedM〈A〉` in a continue state
    /// </summary>
    public static readonly ReducedM<M, Unit> Unit;
    
    /// <summary>
    /// `ReducedM〈A〉` in a continue state
    /// </summary>
    public static readonly ValueTask<ReducedM<M, Unit>> UnitAsync;
}
