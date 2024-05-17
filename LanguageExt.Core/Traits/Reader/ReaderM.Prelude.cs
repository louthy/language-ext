using System;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Retrieves the reader monad environment.
    /// </summary>
    /// <typeparam name="Env">Environment</typeparam>
    /// <returns>Reader monad with the environment in as the bound value</returns>
    [Pure]
    public static Ask<Env, Env> ask<Env>() =>
        new(identity);

    /// <summary>
    /// Retrieves a function of the current environment.
    /// </summary>
    /// <typeparam name="Env">Environment</typeparam>
    /// <typeparam name="A">Bound and mapped value type</typeparam>
    /// <returns>Reader monad with the mapped environment in as the bound value</returns>
    [Pure]
    public static Ask<Env, A> asks<Env, A>(Func<Env, A> f) =>
        new(f);
}
