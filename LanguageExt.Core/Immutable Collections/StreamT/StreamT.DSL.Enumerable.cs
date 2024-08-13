using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record StreamEnumerableT<M, A>(IEnumerable<A> items) : StreamT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT =>
        Lift(items.GetEnumerator()).runListT;

    public static StreamT<M, A> Lift(IEnumerator<A> iter)
    {
        if (iter.MoveNext())
        {
            return new StreamEnumerableItemT<M, A>(
                M.Pure(MList<A>.Iter<M>(iter.Current, iter)));
        }
        else
        {
            iter.Dispose();
            return Empty;
        }
    }

    public override StreamT<M, B> Map<B>(Func<A, B> f) =>
        new StreamEnumerableT<M, B>(items.Select(f));
}

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record StreamEnumerableItemT<M, A>(K<M, MList<A>> runListT) : StreamT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT { get; } = runListT;

    public override StreamT<M, B> Map<B>(Func<A, B> f) =>
        new StreamEnumerableItemT<M, B>(runListT.Map(la => la.Map(f)));
}
