using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record StreamLiftM<M, A>(K<M, A> pureMA) : StreamT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT { get; } =
        pureMA.Map(a => MList<A>.Cons(a, M.Pure(MList<A>.Nil)));

    public override StreamT<M, A> Tail() =>
        Empty;

    public override StreamT<M, B> Map<B>(Func<A, B> f) =>
        new StreamLiftM<M, B>(pureMA.Map(f));
}
