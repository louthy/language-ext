using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record IterablePureT<M, A>(A value) : IterableT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT { get; } = 
        M.Pure(MList<A>.Cons(value, M.Pure(MList<A>.Nil)));

    public override IterableT<M, B> Map<B>(Func<A, B> f) =>
        new IterablePureT<M, B>(f(value));
}
