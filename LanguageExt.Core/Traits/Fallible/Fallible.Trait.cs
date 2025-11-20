using System;
using LanguageExt.Common;

namespace LanguageExt.Traits;

/// <summary>
/// Trait for higher-kinded structures that have a failure state `E`
/// </summary>
/// <typeparam name="E">Failure type</typeparam>
/// <typeparam name="F">Higher-kinded structure</typeparam>
public interface Fallible<E, F>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //
    
    /// <summary>
    /// Raise a failure state in the `Fallible` structure `F`
    /// </summary>
    /// <param name="error">Error value</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static abstract K<F, A> Fail<A>(E error);

    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Predicate">Predicate to test any failure values</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static abstract K<F, A> Catch<A>(
        K<F, A> fa,
        Func<E, bool> Predicate, 
        Func<E, K<F, A>> Fail);
}

/// <summary>
/// Trait for higher-kinded structures that have a failure state `Error`
/// </summary>
/// <typeparam name="F">Higher-kinded structure</typeparam>
public interface Fallible<F> : Fallible<Error, F>;
