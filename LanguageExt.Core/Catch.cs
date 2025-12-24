using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Used by `@catch`, `@exceptional`, `@expected` to represent the catching of errors
/// </summary>
public readonly record struct CatchM<E, M, A>(Func<E, bool> Match, Func<E, K<M, A>> Action)
    where M : Fallible<E, M>
{
    public K<M, A> Run(E error, K<M, A> otherwise) =>
        Match(error) ? Action(error) : otherwise;
}

public static class CatchMExtensions
{
    extension<E, M, A>(CatchM<E, M, A> self)
        where M : Functor<M>, Fallible<E, M>
    {
        public CatchM<E, M, B> Map<B>(Func<A, B> f) =>
            new(self.Match, e => self.Action(e).Map(f));
    }
}
