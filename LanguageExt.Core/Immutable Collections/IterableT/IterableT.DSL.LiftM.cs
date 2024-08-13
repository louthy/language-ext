using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record IterableLiftM<M, A>(K<M, A> pureMA) : IterableT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT { get; } = 
        pureMA.Map(a => MList<A>.Cons(a, M.Pure(MList<A>.Nil)));

    public override IterableT<M, B> Map<B>(Func<A, B> f) =>
        new IterableLiftM<M, B>(pureMA.Map(f));
}
