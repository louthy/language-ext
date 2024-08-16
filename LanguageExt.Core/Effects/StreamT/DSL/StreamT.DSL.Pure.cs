using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record StreamPureT<M, A>(A value) : StreamT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT { get; } =
        M.Pure(MList<A>.Cons(value, M.Pure(MList<A>.Nil)));

    public override StreamT<M, A> Tail =>
        Empty;

    public override StreamT<M, B> Map<B>(Func<A, B> f) =>
        new StreamPureT<M, B>(f(value));
}
