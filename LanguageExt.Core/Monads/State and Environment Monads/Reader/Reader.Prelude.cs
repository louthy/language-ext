using System;
using System.Diagnostics.Contracts;
using L = LanguageExt;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Reader<Env, A> flatten<Env, A>(Reader<Env, Reader<Env, A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Reader monad constructor
    /// </summary>
    /// <typeparam name="Env">Environment</typeparam>
    /// <typeparam name="A">Wrapped type</typeparam>
    /// <param name="value">Wrapped value</param>
    /// <returns>Reader monad</returns>
    [Pure]
    public static Reader<Env, A> Reader<Env, A>(A value) =>
        L.Reader<Env, A>.Pure(value);

    /// <summary>
    /// Reader monad constructor
    /// </summary>
    /// <typeparam name="Env">Environment</typeparam>
    /// <typeparam name="A">Wrapped type</typeparam>
    /// <param name="value">Wrapped value</param>
    /// <returns>Reader monad</returns>
    [Pure]
    [Obsolete("Use `asks` - it's the same function")]
    public static Reader<Env, A> Reader<Env, A>(Func<Env, A> f) =>
        L.Reader<Env, A>.Asks(f);
    
    /// <summary>
    /// Retrieves the reader monad environment.
    /// </summary>
    /// <typeparam name="Env">Environment</typeparam>
    /// <returns>Reader monad with the environment in as the bound value</returns>
    [Pure]
    public static Ask<Env, Env> ask<Env>() =>
        new (identity);

    /// <summary>
    /// Retrieves a function of the current environment.
    /// </summary>
    /// <typeparam name="Env">Environment</typeparam>
    /// <typeparam name="A">Bound and mapped value type</typeparam>
    /// <returns>Reader monad with the mapped environment in as the bound value</returns>
    [Pure]
    public static Ask<Env, A> asks<Env, A>(Func<Env, A> f) =>
        new (f);

    /// <summary>
    /// Executes a computation in a modified environment
    /// </summary>
    /// <param name="f">The function to modify the environment.</param>
    /// <param name="m">Reader to modify</param>
    /// <returns>Modified reader</returns>
    [Pure]
    public static Reader<Env, A> local<Env, A>(Reader<Env, A> ma, Func<Env, Env> f) =>
        ma.Local(f);

    /// <summary>
    /// Chooses the first monad result that has a Some(x) for the value
    /// </summary>
    [Pure]
    public static Reader<Env, Option<A>> choose<Env, A>(Seq<Reader<Env, Option<A>>> ms) =>
        ms switch
        {
            { IsEmpty: true } => L.Reader<Env, Option<A>>.Pure(Option<A>.None),
            var (x, xs) => x.Bind(oa => oa.IsSome
                                            ? L.Reader<Env, Option<A>>.Pure(oa)
                                            : choose(xs))
        };

    /// <summary>
    /// Chooses the first monad result that has a Some(x) for the value
    /// </summary>
    [Pure]
    public static Reader<Env, Option<A>> choose<Env, A>(params Reader<Env, Option<A>>[] monads) =>
        choose(monads.ToSeq());
}
