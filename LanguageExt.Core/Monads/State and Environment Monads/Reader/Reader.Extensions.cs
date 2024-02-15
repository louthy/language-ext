using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Reader monad extensions
/// </summary>
public static partial class ReaderExt
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Reader<Env, A> Flatten<Env, A>(this Reader<Env, Reader<Env, A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Impure iteration of the bound value in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public static Reader<Env, A> Do<Env, A>(this Reader<Env, A> ma, Action<A> f) =>
        ma.Map(a => { f(a); return a; });
}
