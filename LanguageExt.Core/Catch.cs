using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Used by `@catch`, `@exceptional`, `@expected` to represent the catching of errors
/// </summary>
public readonly record struct CatchM<E, M, A>(Func<E, bool> Match, Func<E, K<M, A>> Value)
    where M : Fallible<E, M>
{
    public K<M, A> Run(E error, K<M, A> otherwise) =>
        Match(error) ? Value(error) : otherwise;

    public static K<M, A> operator |(K<M, A> lhs, CatchM<E, M, A> rhs) =>
        lhs.Catch(rhs.Match, rhs.Value);
}
