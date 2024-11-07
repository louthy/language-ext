using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Module for higher-kinded structures that have a failure state `E`
/// </summary>
/// <typeparam name="F">Higher-kinded structure</typeparam>
/// <typeparam name="E">Failure type</typeparam>
public static partial class Prelude
{
    /// <summary>
    /// Raise a failure state in the `Fallible` structure `F` 
    /// </summary>
    /// <param name="error">Error to raise</param>
    /// <typeparam name="F">Fallible trait</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static K<F, A> fail<E, F, A>(E error)
        where F : Fallible<E, F> =>
        F.Fail<A>(error);
    
    /// <summary>
    /// Raise a failure state in the `Fallible` structure `F` 
    /// </summary>
    /// <param name="error">Error to raise</param>
    /// <typeparam name="F">Fallible trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static K<F, A> error<F, A>(Error error)
        where F : Fallible<Error, F> =>
        F.Fail<A>(error);
    
    /// <summary>
    /// Raise a failure state in the `Fallible` structure `F` 
    /// </summary>
    /// <param name="error">Error to raise</param>
    /// <typeparam name="F">Fallible trait</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static K<F, Unit> fail<E, F>(E error)
        where F : Fallible<E, F> =>
        F.Fail<Unit>(error);    
    
    /// <summary>
    /// Raise a failure state in the `Fallible` structure `F` 
    /// </summary>
    /// <param name="error">Error to raise</param>
    /// <typeparam name="F">Fallible trait</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static K<F, Unit> error<F>(Error error)
        where F : Fallible<Error, F> =>
        F.Fail<Unit>(error);    
}
