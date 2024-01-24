#nullable enable
using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses;

/// <summary>
/// State monad type class
/// </summary>
[Trait("M*")]
public interface MonadState<S, A> : Trait
{
    /// <summary>
    /// Returns the state from the internals of the monad.
    /// </summary>
    /// <returns>State value where the internal state and the bound value are the same</returns>
    [Pure]
    public static abstract State<S, S> Get();

    /// <summary>
    /// Replaces the state inside the monad.
    /// </summary>
    /// <typeparam name="S">Type of the value to use as the state</typeparam>
    /// <param name="state">State value to use</param>
    /// <returns>Updated state monad</returns>
    [Pure]
    public static abstract State<S, Unit> Put(S state);

    /// <summary>
    /// Embed a simple state action into the monad
    /// </summary>
    /// <param name="f">Action to embed</param>
    /// <returns>Updated state monad</returns>
    [Pure]
    public static abstract State<S, A> State(Func<S, A> f);
}
