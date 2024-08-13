using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record IterableMainT<M, A>(K<M, MList<A>> runListT) : IterableT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT { get; } = runListT;

    public override IterableT<M, B> Map<B>(Func<A, B> f) =>
        new IterableMainT<M, B>(runListT.Map(la => la.Map(f)));
}
