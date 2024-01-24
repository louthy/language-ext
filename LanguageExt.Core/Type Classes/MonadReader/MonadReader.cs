#nullable enable
using System;
using LanguageExt.Attributes;
using System.Diagnostics.Contracts;

namespace LanguageExt.TypeClasses;

/// <summary>
/// Reader monad type class
/// </summary>
[Trait("M*")]
public interface MonadReader<Env, A> : Trait
{
    /// <summary>
    /// Returns the state from the internals of the monad.
    /// </summary>
    /// <returns>State value where the internal state and the bound value are the same</returns>
    [Pure]
    public static abstract Reader<Env, Env> Ask();

    /// <summary>
    /// Retrieves a function of the current environment.
    /// </summary>
    /// <param name="f">The function to modify the environment.</param>
    /// <returns></returns>
    [Pure]
    public static abstract Reader<Env, A> Local(Reader<Env, A> ma, Func<Env, Env> f);

    /// <summary>
    /// Retrieves a function of the current environment.
    /// </summary>
    /// <param name="f">The function to modify the environment.</param>
    /// <returns></returns>
    [Pure]
    public static abstract Reader<Env, A> Reader(Func<Env, A> f);
}
